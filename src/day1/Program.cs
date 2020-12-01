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
                .Where(g => g.Sum().Equals(2020))
                .Select(g => g.Aggregate((s, i) => s * i))
                .Single();

            Console.WriteLine(q1);

            var q2 = data
                .TripleWhere(t => t.Item1 + t.Item2 + t.Item3 == 2020)
                .Select(t => t.Item1 * t.Item2 * t.Item3);

            foreach (var c in q2)
            {
                Console.WriteLine(c);
            }

            Console.ReadLine();
        }

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
        internal static IEnumerable<T[]> DistinctGroup<T>(this IEnumerable<T> source)
        {
            //  There has to be a way to genericize this by taking in a group size
            //      and squashing these windows recursively. Need to look into this

            static KeyValuePair<T, T[]> shrinkFromLeft(T[] arr, int indx) => new(arr[indx], arr[(indx + 1)..]);
            static IEnumerable<KeyValuePair<T,T>> getForSource(T[] tees)
            {
                for (int i = 0, j = tees.Length; i < j; i++)
                {
                    var (c, r) = shrinkFromLeft(tees, i);

                    foreach (var t in r)
                        yield return new(c, t);
                }
            }

            T[] ts = source.ToArray();

            var x = getForSource(ts).ToArray();

            return x.Select(x => new T[]{ x.Key, x.Value });
        }

        internal static IEnumerable<(T, T, T)> TripleWhere<T>(this IEnumerable<T> source, Predicate<(T, T, T)> predicate)
        {
            //  Buffer the IEnumerable to prevent changes during enumeration
            T[] ts = source.ToArray();
            (T, T, T) set;

            for (int a = 0, b = ts.Length; a < b; a++)
            {
                var x = ts[a];
                for (int c = a + 1, d = ts.Length - 1; c < d; c++)
                {
                    var y = ts[c];

                    foreach (T data in ts[(c + 1)..])
                    {
                        set = (x, y, data);

                        if (predicate(set))
                            yield return set;
                    }
                }
            }
        }
    }
}
