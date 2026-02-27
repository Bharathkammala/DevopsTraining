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

            Console.Write("Enter number: ");
            int n = int.Parse(Console.ReadLine());

            for (int i = 2; i <= n; i++)
            {
                bool isPrime = true;

                if (i > 2 && i % 2 == 0)
                    isPrime = false;
                else
                {
                    for (int j = 3; j * j <= i; j += 2)
                    {
                        if (i % j == 0)
                        {
                            isPrime = false;
                            break;   
                        }
                    }
                }

                if (i == 2 || isPrime)
                    Console.Write(i + " ");
            }
        }
    }
}
