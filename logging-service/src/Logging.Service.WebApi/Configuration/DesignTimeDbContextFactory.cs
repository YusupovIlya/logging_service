using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Monq.Core.BasicDotNetMicroservice.Extensions;
using static Logging.Server.Service.StreamData.Configuration.AppConstants.Configuration;

namespace Logging.Server.Service.StreamData.Configuration
{
#pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
    //public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StreamDataContext>
    //{
    //    public StreamDataContext CreateDbContext(string[] args)
    //    {
    //        var environment = new HostingEnvironment
    //        {
    //            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
    //            ContentRootPath = Directory.GetCurrentDirectory()
    //        };

    //        if (string.IsNullOrEmpty(environment.EnvironmentName))
    //        {
    //            Console.Error.WriteLine("ASPNETCORE_ENVIRONMENT not set, using Development.");
    //            environment.EnvironmentName = "Development";
    //        }

    //        if (!HostEnvironmentEnvExtensions.IsDevelopment(environment)
    //            && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_APPLICATION_NAME")))
    //        {
    //            throw new ArgumentException("ASPNETCORE_APPLICATION_NAME not set.");
    //        }

    //        environment.ApplicationName = Environment.GetEnvironmentVariable("ASPNETCORE_APPLICATION_NAME");

    //        Console.WriteLine(
    //            $"Using environment (ASPNETCORE_ENVIRONMENT={Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}).");
    //        Console.WriteLine(
    //            $"Application name: {Environment.GetEnvironmentVariable("ASPNETCORE_APPLICATION_NAME")}.");
    //        Console.WriteLine(
    //            $"Consul config file: {Environment.GetEnvironmentVariable("ASPNETCORE_CONSUL_CONFIG_FILE")}.");

    //        var configuration = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    //            .AddEnvironmentVariables(prefix: "ASPNETCORE_")
    //            .ConfigureConsul(environment)
    //            .Build();

    //        var builder = new DbContextOptionsBuilder<StreamDataContext>();
    //        var connectionString = configuration[ConnectionString];
    //        builder.UseNpgsql(connectionString,
    //            opts => opts.CommandTimeout((int) TimeSpan.FromMinutes(10).TotalSeconds));
    //        return new StreamDataContext(builder.Options);
    //    }
    //}
}
