using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlatformX.Common.Constants;
using PlatformX.Common.Types.DataContract;
using System.Reflection;

namespace PlatformX.Startup.Extensions
{
    public static class ApiExtensions
    {
        /// <summary>
        /// Add Container Service
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="loggingBuilder"></param>
        /// <param name="bootstrapConfiguration"></param>
        public static void AddContainerService(this IServiceCollection services, 
            IConfiguration configuration, 
            ILoggingBuilder loggingBuilder,
            BootstrapConfiguration bootstrapConfiguration)
        {
            services.AddOpenApiInfo(configuration);

            var scope = services.BuildServiceProvider();
            var openApiInfo = scope.GetRequiredService<OpenApiInfo>();

            services.AddSerilog(loggingBuilder);
            services.AddApiServiceCore(openApiInfo, bootstrapConfiguration);

        }

        /// <summary>
        /// AddLambdaService
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="bootstrapConfiguration"></param>
        public static void AddLambdaService(this IServiceCollection services,
            IConfiguration configuration,
            BootstrapConfiguration bootstrapConfiguration)
        {
            services.AddOpenApiInfo(configuration);

            var scope = services.BuildServiceProvider();
            var openApiInfo = scope.GetRequiredService<OpenApiInfo>();

            services.AddSerilog();
            services.AddApiServiceCore(openApiInfo, bootstrapConfiguration);
        }

        public static void AddOpenApiInfo(this IServiceCollection services, IConfiguration configuration)
        {
            var openApiInfo = new OpenApiInfo { };
            configuration.GetSection(nameof(OpenApiInfo)).Bind(openApiInfo);
            services.AddSingleton(openApiInfo);
        }

        public static void AddApiServiceCore(this IServiceCollection services, 
            OpenApiInfo openApiInfo,
            BootstrapConfiguration bootstrapConfiguration)
        {
            if (!string.IsNullOrEmpty(bootstrapConfiguration.CorsOrigins))
            {
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                                .SetIsOriginAllowedToAllowWildcardSubdomains()
                                .WithOrigins(bootstrapConfiguration.CorsOrigins.Split(','))
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        });
                });
            }

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            if (bootstrapConfiguration.EnvironmentName != EnvironmentName.Production)
            {
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(c =>
                {
                    if (openApiInfo != null)
                    {
                        c.SwaggerDoc(openApiInfo.Version, openApiInfo);
                    }

                    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                    if (File.Exists(xmlFileName))
                    {
                        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
                    }

                    c.AddSecurityDefinition("AppKey", new OpenApiSecurityScheme
                    {
                        Description = "Application Key must appear in header",
                        Type = SecuritySchemeType.ApiKey,
                        Name = "x-app-key",
                        In = ParameterLocation.Header,
                        Scheme = "AppKeyScheme"
                    });

                    c.AddSecurityDefinition("AppEnvironment", new OpenApiSecurityScheme
                    {
                        Description = "Application Environment must appear in header",
                        Type = SecuritySchemeType.ApiKey,
                        Name = "x-app-env",
                        In = ParameterLocation.Header,
                        Scheme = "AppEnvironmentScheme"
                    });

                    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                    {
                        Description = "Api Key must appear in header",
                        Type = SecuritySchemeType.ApiKey,
                        Name = "x-api-key",
                        In = ParameterLocation.Header,
                        Scheme = "ApiKeyScheme"
                    });

                    c.AddSecurityDefinition("ApiSecret", new OpenApiSecurityScheme
                    {
                        Description = "Api Secret must appear in header",
                        Type = SecuritySchemeType.ApiKey,
                        Name = "x-api-secret",
                        In = ParameterLocation.Header,
                        Scheme = "ApiSecretScheme"
                    });

                    var appKey = new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "AppKey"
                        },
                        In = ParameterLocation.Header
                    };

                    var appEnvironment = new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "AppEnvironment"
                        },
                        In = ParameterLocation.Header
                    };

                    var apiKey= new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        },
                        In = ParameterLocation.Header
                    };

                    var apiSecret = new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiSecret"
                        },
                        In = ParameterLocation.Header
                    };


                    var requirement = new OpenApiSecurityRequirement
                    {
                        { appKey, new List<string>() },
                        { appEnvironment, new List<string>() },
                        { apiKey, new List<string>() },
                        { apiSecret, new List<string>() }
                    };

                    c.AddSecurityRequirement(requirement);

                    c.EnableAnnotations();
                });
            }

            services.AddHealthChecks();
        }
    }
}
