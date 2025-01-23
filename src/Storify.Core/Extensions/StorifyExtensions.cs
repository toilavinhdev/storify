using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Storify.Core.Endpoints;
using Storify.Core.Models;

namespace Storify.Core.Extensions;

public static class StorifyExtensions
{
    public static void WithStorifyCore(
        this WebApplicationBuilder builder,
        Action<IServiceCollection, AppSettings> actionServices,
        Action<WebApplication, AppSettings> actionApplication
    )
    {
        var environment = builder.Environment;
        var appSettings = new AppSettings();
        new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile(Path.Combine("AppSettings", $"appsettings.{environment.EnvironmentName}.json"))
            .AddEnvironmentVariables()
            .Build()
            .Bind(appSettings);
        
        var services = builder.Services;
        services.AddSingleton(appSettings);
        services.AddHttpContextAccessor();
        services.AddCoreCors();
        services.AddCoreAuth();
        services.AddApiEndpoints();
        services.AddCoreSwagger();
        actionServices.Invoke(services, appSettings);
        
        var app = builder.Build();
        app.UseCoreExceptionHandler();
        app.UseCoreCors();
        app.UseCoreAuth();
        app.UseCoreSwagger();
        app.MapApiEndpoints(app.MapGroup("/api"));
        actionApplication.Invoke(app, appSettings);
        app.Run();
    }
    
    # region Swagger
    private static void AddCoreSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FastAcademy",
                    Version = "v1",
                });
                
                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. " +
                                  "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                                  "\r\n\r\nExample: \"Bearer accessToken\"",
                });
                
                o.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, []
                    }
                });
            });
    }
    
    private static void UseCoreSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    # endregion
    
    # region Endpoints
    private static void AddApiEndpoints(this IServiceCollection services)
    {
        var serviceDescriptors = typeof(Program).Assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IApiEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IApiEndpoint), type))
            .ToList();
        services.TryAddEnumerable(serviceDescriptors);
    }
    
    private static void MapApiEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;
        app.Services
            .GetRequiredService<IEnumerable<IApiEndpoint>>()
            .ToList()
            .ForEach(endpoint => endpoint.MapEndpoints(builder));
    }
    
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<TRequest>>();
    }
    
    private class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
            if (request is null) return await next(context);
            var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
            var errorMessages = result.Errors
                .Select(x => x.ErrorMessage.Replace($"'{x.PropertyName}'", x.PropertyName))
                .ToArray();
            if (!result.IsValid) return StorifyResult.BadRequest(errorMessages);
            return await next(context);
        }
    }
    # endregion
    
    # region CORS
    private static void AddCoreCors(this IServiceCollection services)
    {
        services.AddCors(o =>
        {
            o.AddDefaultPolicy(b => b.AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true));
        });
    }

    private static void UseCoreCors(this WebApplication app)
    {
        app.UseCors();
    }
    # endregion
    
    # region Auth
    private static void AddCoreAuth(this IServiceCollection services)
    {
        
    }
    
    private static void UseCoreAuth(this WebApplication app)
    {
        
    }
    # endregion
    
    # region Global exception handler
    private static void UseCoreExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async ctx =>
            {
                var feature = ctx.Features.Get<IExceptionHandlerFeature>();

                if (feature is not null)
                {
                    var exception = feature.Error;
                    ctx.Response.ContentType = "application/problem+json";
                    ctx.Response.StatusCode = exception switch
                    {
                        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                        _ => (int)HttpStatusCode.InternalServerError
                    };
                    await Task.CompletedTask;
                }
            });
        });
    }
    # endregion
}

