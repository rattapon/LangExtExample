using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using LangExt; //Install from NuGet

namespace LangExtExample
{
    class Program
    {
        static void Main(string[] args)
        {
            PlayAroundFunction();
            Console.ReadLine();
        }

        private static void PlayAroundFunction()
        {
            Func<string, string, string> concat = (s, s1) => string.Concat(s, s1);
            Func<string, Func<string, string>> concatCurry = a => b => string.Concat(a, b);
            Func<Tuple<string, string>, string> concatTuple = t => t.Match((a, b) => string.Concat(a, b));

            Console.WriteLine("==================== Func ====================");
            var funcId1 = Func.Id(1);
            var funcId2 = Func.Id("str1");
            Console.WriteLine("Func.Id(1) is type of {0}", funcId1.GetType());
            Console.WriteLine(@"Func.Id(""str1"") is type of {0}", funcId2.GetType());

            Console.WriteLine("Call concat function : {0}", concat("Hello", "World"));
            Console.WriteLine("Call concatCurry function : {0}", concatCurry("Hello")("World"));
            Console.WriteLine("Call concatTuple function : {0}", concatTuple(new Tuple<string, string>("Hello", "World")));
            Console.WriteLine("Call Func.Flip : {0}", concat.Flip()("Hello", "World"));
            Console.WriteLine("Call Func.File on curry function : {0}", concatCurry.Flip()("Hello")("World"));

            Func<string, string, string> concatUncurry = Func.Uncurry(concatCurry);
            Console.WriteLine("{0} >> uncurry to >> {1}", concatCurry, concatUncurry);
            Console.WriteLine("Call concatUncurry function : {0}", concatUncurry("Hello", "World"));

            Console.WriteLine();
        }
    }
}
