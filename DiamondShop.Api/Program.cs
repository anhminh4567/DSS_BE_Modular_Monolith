using DiamondShop.Api.Middlewares;
using DiamondShop.Infrastructure;
using DiamondShop.Application;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using DiamondShop.Api.Configurations.ProblemErrors;
using DiamondShop.Api.Extensions;
using Microsoft.OpenApi.Models;
using System.Collections;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Runtime.CompilerServices;
public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddNewtonsoftJson(setup =>
            {
                setup.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                setup.SerializerSettings.ContractResolver = null;
            });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
        builder.Services.AddSingleton<ProblemDetailsFactory, DiamonShopProblemDetailsFactory>();
        builder.Services.AddScoped<CustomExeptionHandlerMiddleware>();
        builder.Services.AddCors(setup =>
        {
            setup.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
        });

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);
        foreach (var config in builder.Configuration.AsEnumerable())
        {
            if (!string.IsNullOrEmpty(config.Value))  // Ensure there is a value
            {
                Console.WriteLine($"{config.Key}: {config.Value}");
            }
        }
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.EnvironmentName != "Production")
        {
            //app.SeedData();
        }
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors();
        app.UseMiddleware<CustomExeptionHandlerMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
