using System;
using System.Collections.Generic;
using System.IO;

namespace PriceRecognition
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a file path");
                return;
            }
            if (args.Length > 1)
            {
                Console.WriteLine("Too much arguments");
                return;
            }

            if (File.Exists(args[0]))
            {
                Recognizer rec = new Recognizer(args);
                rec.Preprocess();
                rec.ImageToText();
                List<string> prices = rec.FindPrices();
                if (prices.Count == 0)
                {
                    Console.WriteLine("There are no prices found");
                }
                else
                {
                    foreach (string price in prices)
                    {
                        Console.Write(price + ' ');
                    }
                }
            }
            else
            {
                Console.WriteLine("{0} is not a valid file", args[0]);
            }
        }
    }
}
