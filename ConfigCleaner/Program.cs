// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var configuration = builder.Build();


