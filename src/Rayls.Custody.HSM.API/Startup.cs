using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rayls.Custody.HSM.DTO.ClientProviders;
using Rayls.Custody.HSM.Repository;
using Rayls.Custody.HSM.Repository.PostgreSql;
using Rayls.Custody.HSM.Service;
using Rayls.Custody.HSM.Service.Interface.Repositories;
using Rayls.Custody.HSM.Service.Interface.Services;

namespace Rayls.Custody.HSM.API
{
    public partial class Startup
    {
        private readonly IWebHostEnvironment _environment;
        /// <summary>
        /// Configuration
        /// </summary>
        public readonly IConfiguration _configuration;

        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();

            _configuration = builder.Build();

            _environment = env;
        }

        /// <summary>
        /// Configure Services. This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureApp(services);
            ConfigureSwagger(services);
            ConfigureHttp(services);

            var apiConfig = ConfigureAws(services);

            ConfigureAutoMapper(services);

            services.AddMemoryCache();

            services.AddTransient<CustodyHSMAPIClientProvider>();
            services.AddTransient<IWalletRepository, WalletRepository>();
            services.AddTransient<IWalletService, WalletService>();

            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<Erc20Console, Erc20Console>();

            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<ITransactionService, TransactionService>();

            services.AddTransient<IHsm, AwsKmsHelper>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var configuration = new PostgreSqlConfiguration(apiConfig);

            services.AddDbContext<RaylzDbContext>(options =>
                options.UseNpgsql(configuration.ConnectionString));

        }

        /// <summary>
        /// Configure. This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            ConfigureApp(app);
            ConfigureSwagger(app);


            using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RaylzDbContext>();
            bool created = dbContext.Database.EnsureCreated();
        }
    }
}
