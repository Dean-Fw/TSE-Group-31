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
            
            List<long> regionCrimeTotals = new List<long>();
            List<long> regionalTotalSpendinginMillions = new List<long>();
            List<long> regionalTotalSpending = new List<long>();
            retrieveData(connection, "select * from regioncrimetotal",regionCrimeTotals,1);
            retrieveData(connection, "select * from regiontotalspending", regionalTotalSpendinginMillions,1);
            for(int i = 0; i < regionalTotalSpendinginMillions.Count(); i++)
            {
                regionalTotalSpending.Add(regionalTotalSpendinginMillions[i] * 1000000);
            }
            double correlation = calculateCorrelationCoefficient(regionCrimeTotals, regionalTotalSpending);

            List<long> regionalPopulations = new List<long>();
            retrieveData(connection, "select * from regiontotalpop", regionalPopulations,1);
            long totalPop = sumLongList(regionalPopulations);
            long totalSpending = sumLongList(regionalTotalSpending);
            float countryPopSpending = totalSpending / totalPop;
            List<float> regionalPopSpending = new List<float>();
            for (int i = 0; i < regionalTotalSpending.Count(); i++)
            {
                regionalPopSpending.Add(regionalTotalSpending[i] / regionalPopulations[i]);
            }
        }
        static void retrieveData(MySqlConnection connection, string query, List<long> listOfValues, int pos)
        {
            MySqlCommand command = new MySqlCommand(query,connection);
            MySqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                listOfValues.Add((int)reader[pos]);
            }
            reader.Close();
        }

        static double calculateCorrelationCoefficient(List<long> x, List<long> y)
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

            float sumOfSquaredX = sumFloatList(differencesSquaredX);
            float sumOfSquaredY = sumFloatList(differencesSquaredY);

            double correlationCoefficient = sumOfDifferences / Math.Sqrt(sumOfSquaredX * sumOfSquaredY);
            return correlationCoefficient;
        }
        static void calculateValues(List<long> list, List<float> differences, List<float> differencesSquared)
        {
            long total = 0;
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
        static float sumFloatList(List<float> listOfNums)
        {
            float total = 0;
            foreach(float num in listOfNums)
            {
                total += num;
            }
            return total;
        }
        static long sumLongList(List<long> listOfNums)
        {
            long total = 0;
            foreach (long num in listOfNums)
            {
                total += num;
            }
            return total;
        }
    }
}
