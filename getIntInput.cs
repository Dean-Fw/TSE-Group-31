using System;

namespace TSEg31Project
{
    class getIntInput
    {
        // Option is an attribute to the class because otherwise all of the code cannot access it
        public int option { get; private set; }
        // Function to get and validate an integer input from the user
        public int GetIntInput(int max_val)
        {
            bool valid_input = false;
            // While the input is invalid
            while (valid_input == false)
            {
                try
                {
                    option = Convert.ToInt32(Console.ReadLine());
                    // If the input is bigger than the maximum valid value or it is less than 0
                    if (option > max_val || option < 0)
                    {
                        Console.WriteLine("Invalid input! Input is out of range of options given, please re-enter below:");
                    }
                    else
                    {
                        valid_input = true;
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input! Input was not an integer, please enter an integer below:");
                }
            }
            return option;

        }
    }
}
