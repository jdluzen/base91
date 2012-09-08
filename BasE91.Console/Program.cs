using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasE91.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                BasE91 b = new BasE91();
                Random r = new Random();
                byte[] data = new byte[r.Next(1024, 10 * 1024 * 1024)];
                r.NextBytes(data);

                DateTime begin = DateTime.UtcNow;
                string enc = b.Encode(data).ToString();
                byte[] denc = new BasE91().Decode(enc).ToArray();
                DateTime end = DateTime.UtcNow;

                for (int i = 0; i < data.Length; i++)
                    if (data[i] != denc[i])
                        throw new Exception("It broke");

                System.Console.WriteLine("{0}: {1} in {2}. {3} / sec", enc.Substring(0, 20), data.Length, (end - begin).TotalMilliseconds, (data.Length / (end - begin).TotalSeconds) / (1024 * 1024));
            }
        }
    }
}
