using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LangExt;

namespace LangExtExample
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("======== Func ========");
            var funcId1 = Func.Id(1);
            var funcId2 = Func.Id("str1");
            Console.WriteLine("Func.Id(1) is type of {0}", funcId1.GetType());
            Console.WriteLine(@"Func.Id(""str1"") is type of {0}", funcId2.GetType());


            Console.ReadLine();

        }
    }
}
