using Npgsql;

namespace MoviePro.Services
{
    public class ConnectionService
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration["DefaultConnection"];
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL"); //determines whether
            //we're running locally or remotely - if there's a value it will be stored in DATABASE_URL
            //only available on Heroku
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        //take in remote DATABASE_URL environment value
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl); //typed instance of the class to get values off the URL
                                                    //more easily by referncing properties of a URI

            var userInfo = databaseUri.UserInfo.Split(':'); //stores both username and password - instance
                                                            //of databaseUri has UserInfo property, look at
                                                            //the value of the property - has two strings, one
                                                            //appears before : and after : store both into an
                                                            //array named userInfo

            var builder = new NpgsqlConnectionStringBuilder
            {
                //values for properties of this class
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return builder.ToString();

        }
    }
}
