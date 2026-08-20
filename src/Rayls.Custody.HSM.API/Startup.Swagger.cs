using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Rayls.Custody.HSM.DTO.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rayls.Custody.HSM.API
{
    public partial class Startup
    {
        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var logo = new
                {
                    url = "https://app.rayls.io/assets/images/icons/rayls-logo.svg",
                    altText = "Logo",
                };

                // c.SchemaFilter<CustodyHSMSwaggerSnakeCaseFilter>();
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Rayls Custody HSM API",
                    Version = "v1",
                    Description = "Responsible to create wallets, transactions and setup their configuration.",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                      {"x-logo", new OpenApiObject
                        {
                           { "url", new OpenApiString("https://app.rayls.io/assets/images/icons/rayls-logo.svg")},
                           { "altText", new OpenApiString("Rayls API Logo")}
                        }
                      }
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.EnableAnnotations();

            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        private void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                // c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rayls Custody HSM (Release At: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " )");
                c.SwaggerEndpoint("v1/swagger.json", "Rayls Custody HSM (Release At: " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " )");
                c.InjectStylesheet("/css/doc.css");
            });

            app.UseReDoc(c =>
            {
                c.DocumentTitle = "API Documentation";
                c.SpecUrl = "/swagger/v1/swagger.json";
                c.InjectStylesheet("/css/doc.css");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}