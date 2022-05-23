using MySql.Data.MySqlClient;

namespace TSEg31Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "localhost";
            string database = "tse_g31_database";
            string uid = "root";
            string password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
        }
    }
}
