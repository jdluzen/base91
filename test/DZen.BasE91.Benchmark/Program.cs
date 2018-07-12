using System;
using System.Linq;
using System.Text;
using DZen;

namespace DZen.BasE91.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                BasE91 b = new BasE91();
                Random r = new Random();
                byte[] data = new byte[r.Next(1024 * 1024, 20 * 1024 * 1024)];
                r.NextBytes(data);

                DateTime begin = DateTime.UtcNow;
                StringBuilder enc = b.Encode(data);
                DateTime end = DateTime.UtcNow;

                System.Console.WriteLine("Encode {0}: {1}mb in {2}ms. {3}mb/sec. +{4}%",
                    enc.ToString(0, 10),
                    data.Length / (1024 * 1024),
                    (int)(end - begin).TotalMilliseconds,
                    Math.Round((data.Length / (end - begin).TotalSeconds) / (1024 * 1024), 1),
                    ((long)enc.Length * 100) / data.Length - 100);

                b = new BasE91();
                string encoded = enc.ToString();
                begin = DateTime.UtcNow;
                b.Decode(encoded);
                end = DateTime.UtcNow;

                System.Console.WriteLine("Decode: {0}mb in {1}ms. {2}mb/sec",
                    enc.Length / (1024 * 1024),
                    (int)(end - begin).TotalMilliseconds,
                    Math.Round((enc.Length / (end - begin).TotalSeconds) / (1024 * 1024), 1));
            }
        }
    }
}
