using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Regex ruleEngine = new("(\\d+)-(\\d+)\\s(\\w):\\s(\\w+)");

            int day1 = await CountOccurrencesAsync(ruleEngine);

            Console.WriteLine(day1);

            int day2 = await TestOccurrencesAsync(ruleEngine);

            Console.WriteLine(day2);

            Console.ReadLine();
        }

        private static async Task<int> TestOccurrencesAsync(Regex ruleEngine)
        {
            int hits = 0;

            await foreach (var pwd in ReadDataAsync())
            {
                var segments = ruleEngine.Split(pwd);

                var comparitor = char.TryParse(segments[3], out char c) ? c : default;

                if (int.TryParse(segments[1], out int p1)
                    && int.TryParse(segments[2], out int p2))
                {
                    var occurrances = segments[4]
                        .AtIndicies(p1-1, p2-1)
                        .Count(comparitor.Equals);

                    if (occurrances == 1)
                    {
                        hits++;
                    }
                }
            }

            return hits;
        }

        private static async Task<int> CountOccurrencesAsync(Regex ruleEngine)
        {
            int hits = 0;

            await foreach (var pwd in ReadDataAsync())
            {
                var segments = ruleEngine.Split(pwd);

                var comparitor = char.TryParse(segments[3], out char c) ? c : default;

                var occurrances = segments[4]
                    .Where(comparitor.Equals)
                    .Count();

                if (int.TryParse(segments[1], out int min)
                    && int.TryParse(segments[2], out int max)
                    && (min <= occurrances && occurrances <= max))
                {
                    hits++;
                }
            }

            return hits;
        }

        private static async IAsyncEnumerable<string> ReadDataAsync()
        {
            var dataFile = $"{AppContext.BaseDirectory}../../../../../data/day2.txt";

            using var data = File.OpenRead(dataFile);
            using var rdr = new StreamReader(data);

            while (!rdr.EndOfStream)
            {
                yield return await rdr.ReadLineAsync();
            }
        }
    }
    internal static class Extensions
    {
        internal static IEnumerable<T> AtIndicies<T>(this IEnumerable<T> source, params int[] indicies)
        {
            foreach(int index in indicies)
            {
                yield return source.ElementAtOrDefault(index);
            }
        }
    }
}
