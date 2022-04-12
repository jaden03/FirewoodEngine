using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine
{
    public static class Logging
    {
        public static void Print(string message)
        {
            Console.WriteLine(message);
        }

        public static void Print(int message)
        {
            Console.WriteLine(message);
        }

        public static void Print(float message)
        {
            Console.WriteLine(message);
        }

        public static void Print(bool message)
        {
            Console.WriteLine(message);
        }
    }
}
