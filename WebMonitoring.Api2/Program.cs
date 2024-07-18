using JobScheduler.Plugin.WebPage;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebMonitoring.DataAccess.Data;
using WebMonitoring.DataAccess.UnitOfWork;
using WebMonitoring.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

//// Web page monitored entity
// Define API endpoints for MonitoredWebPage
var monitoredWebPageApi = app.MapGroup("/monitoredWebPages");

// Get all monitored web pages with optional pagination
monitoredWebPageApi.MapGet("/", async (IUnitOfWork unitOfWork, int offset = 0, int limit = 10) =>
{
	var monitoredWebPages = unitOfWork.MonitoredWebPageRepository.GetAll(null, offset, limit);
	return Results.Ok(monitoredWebPages);
});

// Get a single monitored web page by ID
monitoredWebPageApi.MapGet("/{id}", async (int id, IUnitOfWork unitOfWork) =>
{
	var monitoredWebPage = unitOfWork.MonitoredWebPageRepository.Get(x => x.Id == id);
	return monitoredWebPage != null ? Results.Ok(monitoredWebPage) : Results.NotFound();
});

// Add a new monitored web page
monitoredWebPageApi.MapPost("/", async (MonitoredWebPage monitoredWebPage, IUnitOfWork unitOfWork) =>
{
	unitOfWork.MonitoredWebPageRepository.Add(monitoredWebPage);
	unitOfWork.MonitoredWebPageRepository.Save();
	return Results.Created($"/monitoredWebPages/{monitoredWebPage.Id}", monitoredWebPage);
});

// Update an existing monitored web page
monitoredWebPageApi.MapPut("/{id}", async (int id, MonitoredWebPage updatedMonitoredWebPage, IUnitOfWork unitOfWork) =>
{
	unitOfWork.MonitoredWebPageRepository.Update(updatedMonitoredWebPage);
	unitOfWork.MonitoredWebPageRepository.Save();
	return Results.Ok(updatedMonitoredWebPage);
});

// Delete a monitored web page
monitoredWebPageApi.MapDelete("/{id}", async (int id, IUnitOfWork unitOfWork) =>
{
	var monitoredWebPage = unitOfWork.MonitoredWebPageRepository.Get(x => x.Id == id);
	if (monitoredWebPage == null)
	{
		return Results.NotFound();
	}

	unitOfWork.MonitoredWebPageRepository.Remove(monitoredWebPage);
	unitOfWork.MonitoredWebPageRepository.Save();
	return Results.NoContent();
});

//// Web page results

// Get all results for web page
monitoredWebPageApi.MapGet("/listResults/{pageEntityId}", async (IUnitOfWork unitOfWork, string pageEntityId, int offset = 0, int limit = 10) =>
{
	var monitoredWebPages = unitOfWork.WebpageResultRepository.GetAll(mw => mw.EntityId == pageEntityId, offset, limit);
	return Results.Ok(monitoredWebPages);
});

// Get a single web page result by ID
monitoredWebPageApi.MapGet("result/{id}", async (string id, IUnitOfWork unitOfWork) =>
{
	Guid guid = Guid.Parse(id);
	var monitoredWebPage = unitOfWork.WebpageResultRepository.Get(x => x.Id == guid);
	return monitoredWebPage != null ? Results.Ok(monitoredWebPage) : Results.NotFound();
});

// Add a new monitored web page
monitoredWebPageApi.MapPost("/addResult", async (WebpageResult result, IUnitOfWork unitOfWork) =>
{
	unitOfWork.WebpageResultRepository.Add(result);
	unitOfWork.MonitoredWebPageRepository.Save();
	return Results.Created($"/result/{result.Id}", result);
});

app.Run();

[JsonSerializable(typeof(MonitoredWebPage[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}