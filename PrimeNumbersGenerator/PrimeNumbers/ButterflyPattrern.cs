using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers
{
    internal class ButterflyPattrern
    {
        using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers
    {
        internal class Program
        {
            static void Main(string[] args)
            {


                ///print butterfly pattern

                int n = 9;

                for (int i = 0; i < n + 1; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        Console.Write("*");
                    }
                    for (int k = 0; k < 2 * (n - i); k++)
                    {
                        Console.Write(" ");
                    }
                    for (int j = 0; j < i; j++)
                    {
                        Console.Write("*");
                    }
                    Console.WriteLine();
                }
                for (int i = n - 1; i >= 0; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        Console.Write("*");
                    }
                    for (int k = 0; k < 2 * (n - i); k++)
                    {
                        Console.Write(" ");
                    }
                    for (int j = 0; j < i; j++)
                    {
                        Console.Write("*");
                    }
                    Console.WriteLine();
                }
            }
        }
    }

}
}
