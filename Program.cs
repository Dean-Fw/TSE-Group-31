using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

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

            List<int> regionCrimeTotals = new List<int>();
            List<int> regionalTotalSpending = new List<int>();
            retrieveIntData(connection, "select * from regioncrimetotal",regionCrimeTotals);
            retrieveIntData(connection, "select * from regiontotalspending", regionalTotalSpending);

            double correlation = calculateCorrelationCoefficient(regionCrimeTotals, regionalTotalSpending);

        }
        static void retrieveIntData(MySqlConnection connection, string query, List<int> listOfValues)
        {
            MySqlCommand command = new MySqlCommand(query,connection);
            MySqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                listOfValues.Add((int)reader[1]);
            }
            reader.Close();
        }

        static double calculateCorrelationCoefficient(List<int> x, List<int> y)
        {
            List<float> differencesX = new List<float>();
            List<float> differencesY = new List<float>();
            List<float> differencesSquaredX = new List<float>();
            List<float> differencesSquaredY = new List<float>();

            calculateValues(x, differencesX, differencesSquaredX);
            calculateValues(y, differencesY, differencesSquaredY);

            float sumOfDifferences = 0;
            for (int i = 0; i < differencesX.Count(); i++)
            {
                sumOfDifferences += differencesX[i] * differencesY[i];
            }

            float sumOfSquaredX = sumList(differencesSquaredX);
            float sumOfSquaredY = sumList(differencesSquaredY);

            double correlationCoefficient = sumOfDifferences / Math.Sqrt(sumOfSquaredX * sumOfSquaredY);
            return correlationCoefficient;
        }
        static void calculateValues(List<int> list, List<float> differences, List<float> differencesSquared)
        {
            int total = 0;
            foreach(int x in list)
            {
                total += x;
            }
            float mean = total / list.Count();

            foreach(int x in list)
            {
                differences.Add(x - mean);
            }

            foreach(float x in differences)
            {
                differencesSquared.Add(x * x);
            }
        }
        static float sumList(List<float> listOfNums)
        {
            float total = 0;
            foreach(float num in listOfNums)
            {
                total += num;
            }
            return total;
        }
    }
}
