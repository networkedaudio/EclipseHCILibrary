using EclipseHXSNMP;
using EclipseHXSNMP.Models;
using EclipseHXSNMP.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configuration
builder.Services.AddSingleton<ConfigurationService>();

// SNMP data pipeline: HCI → MatrixStatus → ObjectStore → Agent
builder.Services.AddSingleton<EclipseHxMatrixStatus>();
builder.Services.AddSingleton<EclipseHxSnmpAgent>(sp =>
{
    var status = sp.GetRequiredService<EclipseHxMatrixStatus>();
    return new EclipseHxSnmpAgent(status, port: 10161); // non-privileged port
});

// SNMP browser (for the MIB tree UI)
builder.Services.AddSingleton<SnmpBrowserService>();

// Background service: connects to matrices via HCI and feeds SNMP data
builder.Services.AddHostedService<HciPollingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

// Start the SNMP agent
var snmpAgent = app.Services.GetRequiredService<EclipseHxSnmpAgent>();
snmpAgent.Start();

app.MapRazorComponents<EclipseHXSNMP.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
