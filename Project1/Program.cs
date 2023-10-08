using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project1.Configurations;
using Project1.Controllers;
using Project1.HealthChecks;
using Project1.Models;
using System;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ModelContext>(); //veri imporant do not delete
            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddHealthChecks().AddSqlServerHealthCheck(
                KeyVaultImplementation.GetSecretFromKeyVault("ConnectionStrings--pysiec"),
                4,
                10,
                "pysiecdb"
                );

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddCors();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });


            app.MapControllerRoute(
               name: "default",
               pattern: "{controller}/{action=Index}/{id?}");

            app.MapControllerRoute(
              name: "handler",
              pattern: "{handler}/{action=Index}/{id?}");

            app.MapControllerRoute(
              name: "orders",
              pattern: "{orders}/{action=Index}/{id?}");

            app.MapControllers();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHealthChecks("/healthz")
                    .RequireHost("*:5146");
            });

            app.Run();
        }

        catch (ArgumentNullException ArgNullEx)
        {
            Console.Write($"{ArgNullEx.Message}");
        }
        catch (InvalidOperationException InvOpEx)
        {
            Console.Write($"{InvOpEx.Message}");
        }

    }
}
