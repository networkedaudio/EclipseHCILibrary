using HCILibrary.Models;
using System.Threading.RateLimiting;

namespace HCILibrary;

/// <summary>
/// Manages a queue of HCI requests with rate limiting and priority support.
/// </summary>
public class HCIRequestQueue : IDisposable
{
    private readonly List<HCIRequest> _queue = new();
    private readonly object _queueLock = new();
    private readonly RateLimiter _rateLimiter;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Func<HCIRequest, Task> _sendAction;
    private Task? _processingTask;
    private bool _disposed;

    /// <summary>
    /// Event raised when a request is dequeued and about to be sent.
    /// </summary>
    public event EventHandler<HCIRequest>? RequestSending;

    /// <summary>
    /// Event raised when a request has been sent.
    /// </summary>
    public event EventHandler<HCIRequest>? RequestSent;

    /// <summary>
    /// Creates a new HCIRequestQueue with the specified rate limiter configuration.
    /// </summary>
    /// <param name="sendAction">The action to execute when sending a request.</param>
    /// <param name="messagesPerSecond">Maximum messages per second (default: 10).</param>
    public HCIRequestQueue(Func<HCIRequest, Task> sendAction, int messagesPerSecond = 10)
    {
        _sendAction = sendAction ?? throw new ArgumentNullException(nameof(sendAction));
        
        // Configure rate limiter with a token bucket algorithm
        _rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = messagesPerSecond,
            TokensPerPeriod = messagesPerSecond,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = int.MaxValue,
            AutoReplenishment = true
        });
    }

    /// <summary>
    /// Gets the current number of items in the queue.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_queueLock)
            {
                return _queue.Count;
            }
        }
    }

    /// <summary>
    /// Starts processing the queue.
    /// </summary>
    public void Start()
    {
        if (_processingTask != null && !_processingTask.IsCompleted)
        {
            return;
        }

        _processingTask = Task.Run(ProcessQueueAsync);
    }

    /// <summary>
    /// Stops processing the queue.
    /// </summary>
    public async Task StopAsync()
    {
        _cancellationTokenSource.Cancel();
        
        if (_processingTask != null)
        {
            await _processingTask;
        }
    }

    /// <summary>
    /// Adds a request to the queue.
    /// </summary>
    /// <param name="request">The request to add.</param>
    /// <param name="urgent">If true, adds the request to the front of the queue (behind other urgent messages).</param>
    public void Enqueue(HCIRequest request, bool urgent = false)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        request.IsUrgent = urgent;

        lock (_queueLock)
        {
            if (urgent)
            {
                // Find the position after the last urgent message
                int insertIndex = 0;
                for (int i = 0; i < _queue.Count; i++)
                {
                    if (_queue[i].IsUrgent)
                    {
                        // Keep urgent messages sorted by timestamp
                        if (_queue[i].CreatedAt <= request.CreatedAt)
                        {
                            insertIndex = i + 1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                _queue.Insert(insertIndex, request);
            }
            else
            {
                _queue.Add(request);
            }
        }
    }

    /// <summary>
    /// Adds a request to the queue and returns a task that completes when a response is received.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="urgent">If true, adds the request to the front of the queue.</param>
    /// <param name="timeout">Timeout for waiting for a response (default: 30 seconds).</param>
    /// <returns>A task that completes with the response, or null if no response or timeout.</returns>
    public async Task<HCIReply?> EnqueueAndWaitAsync(HCIRequest request, bool urgent = false, TimeSpan? timeout = null)
    {
        if (request.ExpectedReplyMessageID == null)
        {
            throw new InvalidOperationException("Request must have ExpectedReplyMessageID set to wait for a response.");
        }

        request.ResponseCompletionSource = new TaskCompletionSource<HCIReply?>();
        Enqueue(request, urgent);

        var timeoutDuration = timeout ?? TimeSpan.FromSeconds(30);
        
        using var cts = new CancellationTokenSource(timeoutDuration);
        
        try
        {
            var completedTask = await Task.WhenAny(
                request.ResponseCompletionSource.Task,
                Task.Delay(timeoutDuration, cts.Token)
            );

            if (completedTask == request.ResponseCompletionSource.Task)
            {
                return await request.ResponseCompletionSource.Task;
            }
            else
            {
                // Timeout occurred
                request.ResponseCompletionSource.TrySetResult(null);
                return null;
            }
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    /// <summary>
    /// Continuously processes the queue, respecting rate limits.
    /// </summary>
    private async Task ProcessQueueAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            HCIRequest? request = null;

            lock (_queueLock)
            {
                if (_queue.Count > 0)
                {
                    request = _queue[0];
                    _queue.RemoveAt(0);
                }
            }

            if (request != null)
            {
                // Wait for rate limiter permission
                using var lease = await _rateLimiter.AcquireAsync(1, _cancellationTokenSource.Token);
                
                if (lease.IsAcquired)
                {
                    try
                    {
                        RequestSending?.Invoke(this, request);
                        await _sendAction(request);
                        RequestSent?.Invoke(this, request);
                    }
                    catch (Exception)
                    {
                        // Request failed - could implement retry logic here
                        request.ResponseCompletionSource?.TrySetResult(null);
                    }
                }
            }
            else
            {
                // No items in queue, wait a bit before checking again
                await Task.Delay(10, _cancellationTokenSource.Token);
            }
        }
    }

    /// <summary>
    /// Disposes of the request queue and its resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _rateLimiter.Dispose();
    }
}
