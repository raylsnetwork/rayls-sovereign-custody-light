using Rayls.Custody.HSM.DTO.Configuration;

namespace Rayls.Custody.HSM.Repository.PostgreSql
{
    public class PostgreSqlConfiguration
    {
        public PostgreSqlConfiguration(ApiConfig config)
        {
            Server = config.POSTGRESQL_SERVER;
            Port = config.POSTGRESQL_PORT;
            Username = config.POSTGRESQL_USERNAME;
            Password = config.POSTGRESQL_PASSWORD;
            DBName = config.POSTGRESQL_DBNAME;
            ConnectionString = $"Server={Server};User Id={Username}; Password={Password};Database={DBName};Port={Port}";
        }

        public string Server { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DBName { get; set; }
        public string Schema { get; set; }
        public string ConnectionString { get; set; }
    }
}
