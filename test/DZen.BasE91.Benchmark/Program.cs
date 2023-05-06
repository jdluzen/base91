using System;
using System.Linq;
using System.Text;
using DZen;

namespace DZen.BasE91.Console
{
    class Program
    {
        static void MainOriginal(string[] args)
        {
            while (true)
            {
                Random r = new Random();
                byte[] data = new byte[r.Next(100 * 1024 * 1024, 800 * 1024 * 1024)];
                r.NextBytes(data);

                DateTime begin = DateTime.UtcNow;
                StringBuilder enc = BasE91.Encode(data);
                DateTime end = DateTime.UtcNow;

                System.Console.WriteLine("Encode {0}: {1}mb in {2}ms. {3}mb/sec. +{4}%",
                    enc.ToString(0, 10),
                    data.Length / (1024 * 1024),
                    (int)(end - begin).TotalMilliseconds,
                    Math.Round((data.Length / (end - begin).TotalSeconds) / (1024 * 1024), 1),
                    ((long)enc.Length * 100) / data.Length - 100);

                string encoded = enc.ToString();
                begin = DateTime.UtcNow;
				BasE91.Decode(encoded);
                end = DateTime.UtcNow;

                System.Console.WriteLine("Decode: {0}mb in {1}ms. {2}mb/sec",
                    enc.Length / (1024 * 1024),
                    (int)(end - begin).TotalMilliseconds,
                    Math.Round((enc.Length / (end - begin).TotalSeconds) / (1024 * 1024), 1));
            }
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Random r = new Random();
                byte[] data = new byte[r.Next(100 * 1024 * 1024, 800 * 1024 * 1024)];
                r.NextBytes(data);

                DateTime begin = DateTime.UtcNow;
                StringBuilder enc = BasE91.Encode(data);
                DateTime end = DateTime.UtcNow;

                System.Console.WriteLine("Encode {0}: {1}mb in {2}ms. {3}mb/sec. +{4}%",
                    enc.ToString(0, 10),
                    data.Length / (1024 * 1024),
                    (int)(end - begin).TotalMilliseconds,
                    Math.Round((data.Length / (end - begin).TotalSeconds) / (1024 * 1024), 1),
                    ((long)enc.Length * 100) / data.Length - 100);

                string encoded = enc.ToString();
                Span<byte> decoded = new Span<byte>(new byte[data.Length]);
                begin = DateTime.UtcNow;
				BasE91.Decode(encoded, decoded);
                end = DateTime.UtcNow;

                System.Console.WriteLine("Decode: {0}mb in {1}ms. {2}mb/sec",
                    enc.Length / (1024 * 1024),
                    (int)(end - begin).TotalMilliseconds,
                    Math.Round((enc.Length / (end - begin).TotalSeconds) / (1024 * 1024), 1));
            }
        }
    }
}
