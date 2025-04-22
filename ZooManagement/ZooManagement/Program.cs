using Domain.ValueObjects;
using Infrastructure;
using Presentation.DTOs;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerGen;

using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ZooManagement.Server
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Extensions["requestId"] = ctx.HttpContext.TraceIdentifier;
                    ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                };
            });


            builder.Services.AddInfrastructure();

            builder.Services.AddApplication();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();


            var app = builder.Build();


            app.UseExceptionHandler(exceptionHandlerApp
                => exceptionHandlerApp.Run(async context
                    => await Results.Problem().ExecuteAsync(context)));


            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();


            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    var problemDetails = new ProblemDetails
                    {
                        Title = "An error occurred",
                        Status = exception switch
                        {
                            ArgumentException => StatusCodes.Status400BadRequest,
                            InvalidOperationException => StatusCodes.Status400BadRequest,
                            _ => StatusCodes.Status500InternalServerError
                        },
                        Detail = exception?.Message
                    };

                    context.Response.StatusCode = problemDetails.Status.Value;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                });
            });


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zoo API v1");
                    c.RoutePrefix = "api-docs";
                });
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/swagger/v1/swagger.json")
                {
                    Console.WriteLine("Swagger endpoint requested");
                }
                await next();
            });



            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zoo API v1");
                c.ConfigObject.DisplayRequestDuration = true;

            });


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
