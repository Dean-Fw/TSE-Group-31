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
            // Connection is established with the database
            string server = "localhost";
            string database = "tse_g31_database";
            string uid = "root";
            string password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            
        }
        // Function to retrieve data from the database and put it into a list
        static void retrieveData(MySqlConnection connection, string query, List<long> listOfValues, int pos)
        {
            // A new SQL command is made by using the query given along with the connection to the database
            MySqlCommand command = new MySqlCommand(query,connection);
            MySqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                listOfValues.Add((int)reader[pos]);
            }
            reader.Close();
        }
        // Function to calculate the correlation coefficient with two sets of data
        static double calculateCorrelationCoefficient(List<long> x, List<long> y)
        {
            // Various lists are initialised for later use
            List<float> differencesX = new List<float>();
            List<float> differencesY = new List<float>();
            List<float> differencesSquaredX = new List<float>();
            List<float> differencesSquaredY = new List<float>();

            // Calculations on both lists of data are performed using the function
            calculateValues(x, differencesX, differencesSquaredX);
            calculateValues(y, differencesY, differencesSquaredY);

            float sumOfDifferences = 0;
            for (int i = 0; i < differencesX.Count(); i++)
            {
                sumOfDifferences += differencesX[i] * differencesY[i];
            }

            // The lists are both added up individually
            float sumOfSquaredX = sumFloatList(differencesSquaredX);
            float sumOfSquaredY = sumFloatList(differencesSquaredY);

            double correlationCoefficient = sumOfDifferences / Math.Sqrt(sumOfSquaredX * sumOfSquaredY);
            return correlationCoefficient;
        }
        // Function to perform some of the maths required to calculate the correlation coefficient
        static void calculateValues(List<long> list, List<float> differences, List<float> differencesSquared)
        {
            long total = 0;
            // All elements of the list are added up
            foreach(int x in list)
            {
                total += x;
            }
            float mean = total / list.Count();

            // Each element subtracts the mean and this value is added to a new list
            foreach(int x in list)
            {
                differences.Add(x - mean);
            }

            // These differences are now squared
            foreach(float x in differences)
            {
                differencesSquared.Add(x * x);
            }
        }
        // Function to add all the elements of a list of 'float' type numbers
        static float sumFloatList(List<float> listOfNums)
        {
            float total = 0;
            foreach(float num in listOfNums)
            {
                total += num;
            }
            return total;
        }
        // Function to add all the elements of a list of 'long' type numbers
        static long sumLongList(List<long> listOfNums)
        {
            long total = 0;
            foreach (long num in listOfNums)
            {
                total += num;
            }
            return total;
        }
        // Function to get the population density values and return it
        static List<long> getPopulationDensity(MySqlConnection connection)
        {
            List<long> regionalPopDensity = new List<long>();
            retrieveData(connection, "select * from regionpopdensity",regionalPopDensity,1);
            return regionalPopDensity;
        }
        // Function to get the correlation between population density and crime rate in the regions
        static void getPopDensityAndCrimeRateCorrelation(MySqlConnection connection)
        {
            // Both lists of values are aquired and then put into the correlation coefficient function
            List<long> regionCrimeTotals = new List<long>();
            retrieveData(connection, "select * from regioncrimetotal", regionCrimeTotals, 1);
            List<long> regionPopDensity = getPopulationDensity(connection);
            double correlation = calculateCorrelationCoefficient(regionCrimeTotals, regionPopDensity);
        }
        // Function to get population spending for both regionally and the country
        static void getPopSpending(MySqlConnection connection)
        {
            // The spending values are in millions in the database so they are got first and then converted to their actual values
            List<long> regionalTotalSpendinginMillions = new List<long>();
            List<long> regionalTotalSpending = new List<long>();
            retrieveData(connection, "select * from regiontotalspending", regionalTotalSpendinginMillions,1);
            for(int i = 0; i < regionalTotalSpendinginMillions.Count(); i++)
            {
                regionalTotalSpending.Add(regionalTotalSpendinginMillions[i] * 1000000);
            }
            List<long> regionalPopulations = new List<long>();
            retrieveData(connection, "select * from regiontotalpop", regionalPopulations,1);
            // Both lists are summed
            long totalPop = sumLongList(regionalPopulations);
            long totalSpending = sumLongList(regionalTotalSpending);
            // Population spending for England is calculated
            float countryPopSpending = totalSpending / totalPop;
            // Regional population spending is calculated for each region and added to a list
            List<float> regionalPopSpending = new List<float>();
            for (int i = 0; i < regionalTotalSpending.Count(); i++)
            {
                regionalPopSpending.Add(regionalTotalSpending[i] / regionalPopulations[i]);
            }
        }
        // Function to get the correlation between spending and crime rate in the regions
        static void getSpendingAndCrimeRateCorrelation(MySqlConnection connection)
        {
            // All required data is found in the database and added to a list
            List<long> regionCrimeTotals = new List<long>();
            retrieveData(connection, "select * from regioncrimetotal",regionCrimeTotals,1);
            // Spending is in millions in the database so needs to be converted and then used
            List<long> regionalTotalSpendinginMillions = new List<long>();
            List<long> regionalTotalSpending = new List<long>();
            retrieveData(connection, "select * from regiontotalspending", regionalTotalSpendinginMillions, 1);
            for (int i = 0; i < regionalTotalSpendinginMillions.Count(); i++)
            {
                regionalTotalSpending.Add(regionalTotalSpendinginMillions[i] * 1000000);
            }
            double correlation = calculateCorrelationCoefficient(regionCrimeTotals, regionalTotalSpending);
        }
    }
}
