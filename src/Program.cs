using AluraCrawler;
using AluraCrawler.Domain.Repositories;
using AluraCrawler.Domain.Services;
using AluraCrawler.Infra.Services;
using Microsoft.Data.SqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Data;

var builder = Host.CreateApplicationBuilder(args);

 IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), true, true)
            .Build();

var cs = config.GetConnectionString("database");
builder.Services.AddTransient<IDbConnection>((sp) => new SqlConnection(cs));

// Chrome driver
builder.Services.AddSingleton<IWebDriver, ChromeDriver>();

// Domain layer
builder.Services.AddSingleton<ICursoRepo, CursoRepo>();

// Application layer
builder.Services.AddSingleton<IAlureService, AlureService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();