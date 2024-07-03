using Api.Prometheus.Consumers;
using MassTransit;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.UseHttpClientMetrics();
builder.Services.ConfigureMassTransit(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.UseHttpMetrics(o =>
{
    o.AddCustomLabel("host", ctx => ctx.Request.Host.Value);
});
app.MapMetrics();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// app.RunCounterMetrics();

app.Run();