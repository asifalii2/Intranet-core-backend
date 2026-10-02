using System;
using System.Collections.Generic;
using System.Text;

namespace Intranet.Core.Helpers
{
    public class UniqueNumberGenerator
    {
        public static int Generate()
        {
            int length = 6;
            Random random = new Random();
            int minValue = (int)Math.Pow(10, length - 1); // Minimum value for the specified length
            int maxValue = (int)Math.Pow(10, length) - 1; // Maximum value for the specified length
            return random.Next(minValue, maxValue + 1); // Generates a random number within the range
        }
    }
}
