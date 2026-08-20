namespace Rayls.Custody.HSM.DTO.Configuration
{
    public class ApiConfig
    {
        public string ASPNETCORE_ENVIRONMENT { get; set; }
        public string EVM_JSON_RPC { get; set; }
        public string EVM_JSON_RPC_CHAIN_ID { get; set; }
        public string POSTGRESQL_SERVER { get; set; }
        public string POSTGRESQL_PORT { get; set; }
        public string POSTGRESQL_USERNAME { get; set; }
        public string POSTGRESQL_PASSWORD { get; set; }
        public string POSTGRESQL_DBNAME { get; set; }
        public string AUTH_API_KEY { get; set; }
        public string AUTH_JWT_SECRET { get; set; }
        public bool USES_AWS { get; set; }
        public string KEYSTORE_MAX_WALLETS { get; set; }
        public string KMS_MAX_WALLETS { get; set; }
    }
}
