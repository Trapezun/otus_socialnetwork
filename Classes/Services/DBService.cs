using Npgsql;

namespace SocialNetwork.Classes.Services
{
    public class DBService
    {
        private readonly IConfiguration configuration;

        public DBService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task ExecuteSelect(string sqlCommand, Action<NpgsqlDataReader> action, List<NpgsqlParameter>? parameters = null)
        {

            var connectionString = getConnectionString();
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using var command = dataSource.CreateCommand(sqlCommand);
            if (parameters != null)
            {
                parameters.ForEach(x =>
                {
                    command.Parameters.Add(x);
                });
            }
            await using var reader = await command.ExecuteReaderAsync();
            action(reader);
        }

        public async Task Execute(string sqlCommand, List<NpgsqlParameter> parameters)
        {
            var connectionString = getConnectionString();
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sqlCommand, conn);                 
            if (parameters != null)
            {
                parameters.ForEach(x =>
                {
                    cmd.Parameters.Add(x);
                });
            }
            await cmd.ExecuteNonQueryAsync();
        }



        private string getConnectionString()
        {
            string connection = configuration.GetConnectionString("dbconnection");
            return connection;
        }

    }
}
