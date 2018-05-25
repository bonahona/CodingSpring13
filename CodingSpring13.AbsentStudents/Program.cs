using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingSpring13.AbsentStudents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    static class Extesions
    {
        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> items, Func<T, U> function)
        {
            foreach (var item in items) {
                yield return function(item);
            }
        }
    }

    class Solution
    {


        // Complete the findTheAbsentStudents function below.
        static int[] findTheAbsentStudents(int n, int[] a)
        {
            var allStudents = new Dictionary<int, bool>();
            for(int i = 1; i <= n; i++) {
                allStudents.Add(i, false);
            }

            foreach(var id in a) {
                allStudents[id] = true;
            }

            return allStudents.Where(s => !s.Value).Map(s => s.Key).ToArray();
        }

        static void Main(string[] args)
        {
            TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

            int n = Convert.ToInt32(Console.ReadLine());

            int[] a = Array.ConvertAll(Console.ReadLine().Split(' '), aTemp => Convert.ToInt32(aTemp))
            ;
            int[] result = findTheAbsentStudents(n, a);

            textWriter.WriteLine(string.Join(" ", result));

            textWriter.Flush();
            textWriter.Close();
        }
    }

}
