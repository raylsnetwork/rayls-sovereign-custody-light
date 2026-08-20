using Microsoft.Extensions.DependencyInjection;
using Rayls.Custody.HSM.DTO.Configuration;
using System;

namespace Rayls.Custody.HSM.API
{
    public partial class Startup
{
        private ApiConfig ConfigureAws(IServiceCollection services)
        {
            var apiConfig = new ApiConfig();
            apiConfig.ASPNETCORE_ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            apiConfig.EVM_JSON_RPC = Environment.GetEnvironmentVariable("EVM_JSON_RPC");
            apiConfig.EVM_JSON_RPC_CHAIN_ID = Environment.GetEnvironmentVariable("EVM_JSON_RPC_CHAIN_ID");

            apiConfig.POSTGRESQL_SERVER = Environment.GetEnvironmentVariable("POSTGRESQL_SERVER");
            apiConfig.POSTGRESQL_PORT = Environment.GetEnvironmentVariable("POSTGRESQL_PORT");
            apiConfig.POSTGRESQL_USERNAME = Environment.GetEnvironmentVariable("POSTGRESQL_USERNAME");
            apiConfig.POSTGRESQL_PASSWORD = Environment.GetEnvironmentVariable("POSTGRESQL_PASSWORD");
            apiConfig.POSTGRESQL_DBNAME = Environment.GetEnvironmentVariable("POSTGRESQL_DBNAME");
            apiConfig.AUTH_API_KEY = Environment.GetEnvironmentVariable("AUTH_API_KEY");
            apiConfig.AUTH_JWT_SECRET = Environment.GetEnvironmentVariable("AUTH_JWT_SECRET");
            apiConfig.KEYSTORE_MAX_WALLETS = Environment.GetEnvironmentVariable("KEYSTORE_MAX_WALLETS");
            apiConfig.KMS_MAX_WALLETS = Environment.GetEnvironmentVariable("KMS_MAX_WALLETS");

            if (bool.TryParse(Environment.GetEnvironmentVariable("USES_AWS"), out _))
                apiConfig.USES_AWS = bool.Parse(Environment.GetEnvironmentVariable("USES_AWS"));            
            else apiConfig.USES_AWS = true;

            services.AddSingleton(apiConfig);


            return apiConfig;
        }
    }
}
