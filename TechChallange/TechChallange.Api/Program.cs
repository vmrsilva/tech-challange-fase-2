using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using TechChallange.Api.Mapper;
using TechChallange.IoC;
using Prometheus;
using Prometheus.SystemMetrics;
using Prometheus.DotNetRuntime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSystemMetrics();

DomainInjection.AddInfraestructure(builder.Services, builder.Configuration);

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseRouting();
app.UseHttpMetrics();
app.UseMetricServer();

//var collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();

app.MapMetrics();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
aa
public partial class Program
{
}