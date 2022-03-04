using System;
using System.Drawing;

namespace ConceptTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            //var service = new CopService();
            //var response = service.ShouldBeArrested(Image.FromFile(@"Input\image.png"));

            //Console.WriteLine("Arrested? " + response);

            int[] A = {1, 2, 4};
            int min = 1;

            foreach (var current in A)
            {
                if (current > min)
                {
                    min = current;
                }
            }

            Console.WriteLine(min);
        }
    }
}
