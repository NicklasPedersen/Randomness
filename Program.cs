using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Randomness
{
    class Program
    {
        static TimeSpan Benchmark(Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.Elapsed;
        }
        static void Main(string[] args)
        {
            int[] int_rands = new int[100000000];
            // to make sure we get the same amount of bytes in each array
            byte[] byte_rands = new byte[Buffer.ByteLength(int_rands)];

            Random r = new Random();

            TimeSpan randomTs = Benchmark(() => {
                for (int i = 0; i < int_rands.Length; i++)
                {
                    int_rands[i] = r.Next();
                }
            });
            TimeSpan randomTsAlt = Benchmark(() => {
                r.NextBytes(byte_rands);
            });
            TimeSpan cryptoTs = Benchmark(() => {
                using (var crypto = new RNGCryptoServiceProvider())
                {
                    crypto.GetBytes(byte_rands);
                }
            });
            TimeSpan cryptoTsAlt = Benchmark(() => {
                using (var crypto = new RNGCryptoServiceProvider())
                {
                    //for (int i = 0; i < int_rands.Length; i++)
                    //{
                    //    byte[] b = new byte[4];
                    //    crypto.GetBytes(b);
                    //    int_rands[i] = BitConverter.ToInt32(b, 0);
                    //}
                }
            });
            // for 400000000 bytes i get:
            //      Random:
            //          Next():     3.0612250 seconds
            //          GetBytes(): 7.1647845 seconds
            // RNGCryptoServiceProvider:
            //          GetBytes(): 0.7655989 seconds
            //          ToInt32():  way too long (don't bother)
            Console.WriteLine($"Random {Buffer.ByteLength(int_rands)} bytes: {randomTs}");
            Console.WriteLine($"Random {Buffer.ByteLength(byte_rands)} bytes: {randomTsAlt}");
            Console.WriteLine($"Crypto {Buffer.ByteLength(byte_rands)} bytes: {cryptoTs}");
            Console.WriteLine($"Crypto {Buffer.ByteLength(byte_rands)} bytes: {cryptoTsAlt}");
            Console.ReadLine();
        }
    }
}
