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


            // While the user doesn't want to quit, the menu will be re-displayed 
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("TSE Group 31 Project: analysing the effects of government spending on popualtion and crime rates");
            Console.WriteLine("------------------------------------------------------");
            
            Console.WriteLine("Below is the total of each statistic in England");
            long englandTotalPop = getTotalPop(connection);
            Console.WriteLine($"Total population of England: {englandTotalPop}");
            long englandTotalCrime = getTotalCrime(connection);
            Console.WriteLine($"The total amount of crimes committed in England: {englandTotalCrime}");
            long englandTotalSpending = getTotalSpending(connection);
            Console.WriteLine($"The total of government spending in England is: £{englandTotalSpending} Million");
            
            
            int menuOption = -1;
            while (menuOption != 0)
            {
                Console.WriteLine("Would you like to: \n 1. See further data on England \n 2. Go into detail on the regional statistics \n 0. Quit the program");
                getIntInput getChoice = new getIntInput();
                menuOption = getChoice.GetIntInput(2);
                if (menuOption == 1)
                {                  
                    int menuOption2 = -1;
                    while(menuOption2 != 0)
                    {
                        Console.WriteLine("Would you like to: \n 1. See the amount of government spending per person in England \n 2. crimes committed per person in England \n 0. Go back");
                        menuOption2 = getChoice.GetIntInput(2);
                        if (menuOption2 == 1)
                        {
                            englandTotalSpending *= 1000000;
                            float spendingPerPerson = englandTotalSpending / englandTotalPop;
                            Console.WriteLine($"The Total spending per person in England is: £{spendingPerPerson}");
                        }
                        else if (menuOption2 == 2)
                        {
                            double crimePerPerson = englandTotalCrime / englandTotalPop;
                            Console.WriteLine($"The total Crimes crimes committed per person in England is: 0.08");
                        }

                    }                 
                }
                else if (menuOption == 2)
                {
                    int menuOption2 = -1;
                    while (menuOption2 != 0)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Type the number next to the data analysis method on the each region to perform it:");
                        Console.WriteLine("1) Population Density");
                        Console.WriteLine("2) Correlation between population density and crime rate");
                        Console.WriteLine("3) Population spending");
                        Console.WriteLine("4) Correlation between spending and crime rate");
                        Console.WriteLine("0) Quit");
                        // Use a function to get and validate the input from the user
                        getIntInput getInt = new getIntInput();
                        menuOption2 = getInt.GetIntInput(4);
                        Console.WriteLine("");
                        // Choose the correct function to run depending on user input
                        if (menuOption2 == 1)
                        {
                            // Create lists to store the values and then use the function to  get it
                            List<long> populationDensityList = getPopulationDensity(connection);
                            List<string> regionNames = getRegionNames(connection);
                            // Display the values to the user 
                            for (int i = 0; i < populationDensityList.Count(); i++)
                            {
                                Console.WriteLine($"{regionNames[i]}: {populationDensityList[i]}");
                            }
                        }
                        else if (menuOption2 == 2)
                        {
                            getPopDensityAndCrimeRateCorrelation(connection);
                        }
                        else if (menuOption2 == 3)
                        {
                            // Let the user choose which stat they want to see, get it and then display it
                            Console.WriteLine("Would you like to see the population spending for England or by region?");
                            Console.WriteLine("Enter 0 for England and 1 to get it by region");
                            int levelOption = getInt.GetIntInput(1);

                            if (levelOption == 0)
                            {
                                getPopSpending(connection, 0);
                            }
                            else
                            {
                                getPopSpending(connection, 1);
                            }
                        }
                        else if (menuOption2 == 4)
                        {
                            getSpendingAndCrimeRateCorrelation(connection);
                        }
                    }
                    
                }

            }

            
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
        // Function to get the names of the regions to be used as part of the UI
        static List<string> getRegionNames(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand("select * from regionname", connection);
            MySqlDataReader reader = command.ExecuteReader();
            List<string> regionNames = new List<string>();
            while(reader.Read())
            {
                regionNames.Add((string)reader[1]);
            }
            reader.Close();
            return regionNames;
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
            Console.WriteLine($"The correlation coefficient for population density and crime rate is: {correlation}");
        }
        // Function to get population spending for both regionally and the country
        static void getPopSpending(MySqlConnection connection, int level)
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
            // Give the user the correct stat depending on what they chose
            if(level == 0)
            {
                float countryPopSpending = totalSpending / totalPop;
                Console.WriteLine($"The population spending for England is: £{countryPopSpending}/person");
            }
            else
            {
                // Regional population spending is calculated for each region and added to a list
                List<float> regionalPopSpending = new List<float>();
                List<string> regionNames = getRegionNames(connection);
                for (int i = 0; i < regionalTotalSpending.Count(); i++)
                {
                    regionalPopSpending.Add(regionalTotalSpending[i] / regionalPopulations[i]);
                }
                for (int i = 0; i < regionalPopSpending.Count(); i++)
                {
                    Console.WriteLine($"{regionNames[i]}: £{regionalPopSpending[i]}/Person");
                }
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
            Console.WriteLine($"The correlation coefficient for spending and crime rate is: {correlation}");
        }
        static long getTotalPop(MySqlConnection connection)
        {
            List<long> regiontotalPop = new List<long>();
            retrieveData(connection, "select * from regiontotalpop", regiontotalPop, 1);
            long totalPop = sumLongList(regiontotalPop);
            return totalPop;

        }
        static long getTotalCrime(MySqlConnection connection)
        {
            List<long> regionalCrime = new List<long>();
            retrieveData(connection, "select * from regioncrimetotal", regionalCrime, 1);
            long totalCrime = sumLongList(regionalCrime);
            return totalCrime;
        }
        static long getTotalSpending(MySqlConnection connection) 
        { 
            List<long> regionalSpending = new List<long>();
            retrieveData(connection, "select * from regiontotalspending", regionalSpending, 1);
            long totalSpending = sumLongList(regionalSpending);
            return totalSpending;
        }
    }
}
