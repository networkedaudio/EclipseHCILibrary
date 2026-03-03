using System.Xml.Serialization;
using EclipseHXSNMP.Models;

namespace EclipseHXSNMP.Services;

/// <summary>
/// Manages loading and saving the matrix configuration XML file.
/// </summary>
public class ConfigurationService
{
    private readonly string _configPath;
    private readonly object _lock = new();
    private MatrixConfiguration _configuration = new();

    public ConfigurationService(string? configPath = null)
    {
        _configPath = configPath ?? Path.Combine(
            AppContext.BaseDirectory, "matrix-config.xml");
        Load();
    }

    /// <summary>
    /// Gets the current configuration.
    /// </summary>
    public MatrixConfiguration Configuration
    {
        get { lock (_lock) return _configuration; }
    }

    /// <summary>
    /// Loads the configuration from the XML file.
    /// Creates a default configuration if the file doesn't exist.
    /// </summary>
    public void Load()
    {
        lock (_lock)
        {
            if (!File.Exists(_configPath))
            {
                _configuration = new MatrixConfiguration();
                Save();
                return;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(MatrixConfiguration));
                using var stream = File.OpenRead(_configPath);
                _configuration = (MatrixConfiguration?)serializer.Deserialize(stream)
                    ?? new MatrixConfiguration();
            }
            catch
            {
                _configuration = new MatrixConfiguration();
            }
        }
    }

    /// <summary>
    /// Saves the current configuration to the XML file.
    /// </summary>
    public void Save()
    {
        lock (_lock)
        {
            var serializer = new XmlSerializer(typeof(MatrixConfiguration));
            using var stream = File.Create(_configPath);
            serializer.Serialize(stream, _configuration);
        }
    }

    /// <summary>
    /// Adds a new matrix connection and saves.
    /// </summary>
    public void AddMatrix(MatrixConnection matrix)
    {
        lock (_lock)
        {
            _configuration.Matrices.Add(matrix);
        }
        Save();
    }

    /// <summary>
    /// Removes a matrix connection by index and saves.
    /// </summary>
    public void RemoveMatrix(int index)
    {
        lock (_lock)
        {
            if (index >= 0 && index < _configuration.Matrices.Count)
            {
                _configuration.Matrices.RemoveAt(index);
            }
        }
        Save();
    }

    /// <summary>
    /// Updates a matrix connection at the given index and saves.
    /// </summary>
    public void UpdateMatrix(int index, MatrixConnection matrix)
    {
        lock (_lock)
        {
            if (index >= 0 && index < _configuration.Matrices.Count)
            {
                _configuration.Matrices[index] = matrix;
            }
        }
        Save();
    }
}
