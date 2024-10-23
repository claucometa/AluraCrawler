using AluraCrawler;
using AluraCrawler.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IWebDriver, ChromeDriver>();
builder.Services.AddSingleton<AlureService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();