using Api.Prometheus.CustomMetrics;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();
app.UseHttpMetrics();
app.MapMetrics();   

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// app.RunCounterMetrics();

app.Run();