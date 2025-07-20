using FitnessTracker.Api.Services;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using FitnessTracker.API.Services;
using FitnessTracker.API.Services.FitnessTracker.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContext<FitnessTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IMacroGoalService, MacroGoalService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IProgressService, ProgressService>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
