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

           // Console.Write("Enter number: ");
           // int n = int.Parse(Console.ReadLine());
            int n = 9;
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


            ///print butterfly pattern


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
