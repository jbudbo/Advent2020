using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace day1
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            int[] data = await ReadDataAsync().ToArrayAsync();

            var q1 = data
                .DistinctGroup()
                .Select(g => new KeyValuePair<int,int>(g.Aggregate(Add), g.Aggregate(Mult)))
                .Single(For2020);

            Console.WriteLine(q1.Value);

            var q2 = data
                .DistinctGroup(3)
                .Select(g => new KeyValuePair<int, int>(g.Aggregate(Add), g.Aggregate(Mult)))
                .Single(For2020);

            Console.WriteLine(q2.Value);

            Console.ReadLine();
        }

        private static int Add(int a, int b) => a + b;
        private static int Mult(int a, int b) => a * b;
        private static bool For2020(KeyValuePair<int,int> kvp) => kvp.Key.Equals(2020);

        private static async IAsyncEnumerable<int> ReadDataAsync() 
        {
            var dataFile = $"{AppContext.BaseDirectory}../../../../../data/day1.txt";

            using var data = File.OpenRead(dataFile);
            using var rdr = new StreamReader(data);

            while (!rdr.EndOfStream)
            {
                if (int.TryParse(await rdr.ReadLineAsync(), out int r))
                    yield return r;
            }
        }
    }

    internal static class Extensions
    {
        internal static KeyValuePair<T, T[]> HT<T>(T[] source) => new(source[0], source[1..]);

        internal static IEnumerable<T[]> DistinctGroup<T>(this IEnumerable<T> source, int size = 2)
        {
            //  If we get smaller than 2 just return singles
            if (size < 2) return null;

            T[] outBuf = new T[size];

            IEnumerable<T[]> getForSource(T[] source, int depth)
            {
                if (source.Length < size) yield break;

                var (head, tail) = HT(source);

                outBuf[depth] = head;

                if (depth < size - 2)
                {
                    foreach (var heads in getForSource(tail, depth + 1))
                        yield return heads;
                }
                else
                {
                    //  We're at the end, start tailing
                    foreach (var t in tail)
                    {
                        outBuf[size - 1] = t;
                        yield return outBuf;
                    }
                }

                foreach (var nSource in getForSource(tail, depth))
                    yield return nSource;
            }

            //  Buffer the IEnumerable to prevent changes during enumeration
            T[] ts = source.ToArray();

            return getForSource(ts, 0);
        }
    }
}
