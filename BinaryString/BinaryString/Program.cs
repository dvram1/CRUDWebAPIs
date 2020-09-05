using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryString
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputString = Console.ReadLine();
            Console.WriteLine(goodBinaryString(inputString));
            Console.ReadLine();
        }

        static bool goodBinaryString(string str)
        {

            int val = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '0')
                    val--; // for every '0' encountered, decrement the variable val
                else if (str[i] == '1')
                    val++; // for every '1' encountered, increment the variable val
                if (val < 0) // if there are more or equal number of '1's when compared to zeros for each prefix, val will be >=0. Otherwise val will be <0
                    return false;
            }
            if (val != 0) // if the number of '1's and '0's are same at the end of parsing th estring, then val should be 0. Otherwise val != 0
                return false;

            return true;
        }
    }
}
