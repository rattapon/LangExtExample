using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LangExt; //Install from NuGet
using Enumerable = System.Linq.Enumerable;



namespace LangExtExample
{
    public enum Nationality
    {
        Thai,
        Japanese,
    }

    public class Program
    {
        static void Main(string[] args)
        {
            //PlayAroundFunction();
            //PlayAroundSeq();
            //PlayAroundOption();
            //PlayAroundMaybeMonad();
            //PlayAroundSeqMonad();
            PlayAroundEitherMonad();
            PlayMaybeFromEricExample();
            Console.ReadLine();
        }

        private static void PlayMaybeFromEricExample()
        {
            Console.WriteLine("==================== Maybe Monad from Eric's example ====================");
            Console.WriteLine(1.Unit().QuerySyntax());
            Console.WriteLine(1000.Unit().QuerySyntax());
            Console.WriteLine(Maybe<int>.Null.QuerySyntax());
        }

        private static void PlayAroundEitherMonad()
        {
            var exec1 = from x in 0.ToExceptional()
                       from y in Exceptional.Execute(() => 6 / x)
                       from z in 7.ToExceptional()
                       select x + y + z;
            Console.WriteLine("Exceptional Result 1: " + exec1);

            var exec2 = Exceptional.From(0)
                       .ThenExecute(x => x + 6 / x)
                       .ThenExecute(y => y + 7);
            Console.WriteLine("Exceptional Result 2: " + exec2);

            var exec3 = Exceptional.From(3)
                       .ThenExecute(x => x + 6 / x)
                       .ThenExecute(y => y + 7);
            Console.WriteLine("Exceptional Result 3: " + exec3);
            
        }

        private static void PlayAroundSeqMonad()
        {
            Console.WriteLine("==================== Seq Monad ====================");

            var mNum = Seq.Init(5, i => i + 1);
            var mChar = new[] {"A", "B", "C", "D", "E", "F"}.ToSeq();

            Func<int, string, string> concat = (n1, n2) => string.Concat(n1, n2);

            var result =
                    from num1 in mNum
                    from character in mChar
                    select concat(num1, character);

            Console.WriteLine(result);
            result.ToSeq().Iter(n => Console.Write("{0}", n));
            Console.WriteLine();

            Func<int, string> convertToString = n => string.Format("*{0}*", n);
            var bindResult = mNum.Bind(n => Seq.Create(convertToString(n)));
            Console.WriteLine("{0} : {1}", bindResult, bindResult.Count());
            foreach (var item in bindResult)
            {
                Console.WriteLine("{0} ", item);
            }

            var num100 = Seq.Init(100, i => i + 1);
            num100.Where(a => a%5 == 0).SelectMany(n => Seq.Singleton(convertToString(n))).ToSeq().Iter(s => Console.WriteLine(s));

        }

        private static void PlayAroundMaybeMonad()
        {
            Console.WriteLine("==================== Maybe Monad ====================");
            var mTen = Option.Some(10);
            Func<int, string> makeString = n => string.Format("({0})", n);

            var result = mTen.Bind(n => Option.Some(makeString(10)));
            Console.WriteLine(result);

            var mNone = Option.None;
            var mTwenty = Option.Some(20);
            Func<int, int, int> multiply = (n1, n2) => n1*n2;

            var multiplyList = from ten in mTen
                from twenty in mTwenty
                select multiply(ten, twenty);

            Console.WriteLine(multiplyList);

            var multiplyNone = from ten in mTen
                from none in (Option<int>) mNone
                select multiply(ten, none);

            Console.WriteLine(multiplyNone);

        }

        private static void PlayAroundOption()
        {
            Console.WriteLine("==================== Option ====================");
            
            var thai = Create.Option(Nationality.Thai);
            var japanese = Create.Option(Nationality.Japanese);
            var nullOption = Create.Option<Nationality>(null);
            Console.WriteLine("{0} is {1}", thai, thai.IsSome ? "Some" : "None");
            Console.WriteLine("{0} is {1}", japanese, japanese.IsSome ? "Some" : "None");
            Console.WriteLine("{0} is None = {1}", nullOption, nullOption.IsNone);
            Console.WriteLine("{0} if Thai",Nationality.Thai.ToString().NoneIf("Thai"));
            string nullStr = null;
            Console.WriteLine("{0} if null", nullStr.NoneIfNull());

            Func<string, Nationality> func = a =>
            {
                switch (a.ToLower())
                {
                    case "thai":
                        return Nationality.Thai;
                    case "japanese":
                        return Nationality.Japanese;
                    default:
                        throw new Exception();
                }
            };

            Console.WriteLine(Option.FromFunc(() => func("Thai")));
            Console.WriteLine(Option.FromFunc(() => func("THAI")));
            Console.WriteLine(Option.FromFunc(() => func("japanese")));
            Console.WriteLine(Option.FromFunc(() => func("Chinese")));

            var emptyOption = new Option<int>();
            Console.WriteLine("{0} is None : {1}", emptyOption.GetType(), emptyOption.IsNone);

            var sizes = new[] {"S", "M", "L", "XL", "", null};
            foreach (var option in Enumerable.Select(sizes, Create.Option))
            {
                Console.WriteLine(option.GetOr("F"));
                Console.WriteLine(option.GetOrElse(() => "No option ==> Do this instead"));
                Console.WriteLine("{0} is Some {1}", option, option.Match(
                    Some: v => true,
                    None: () => false));
                option.Match(
                    Some: v => Console.WriteLine("{0} do something", v),
                    None: () => Console.WriteLine("None do nothing"));

                if (option == Option.Some("XL")) Console.WriteLine("You're overweight, do more exercise!!!");
                else Console.WriteLine("Skinny!!");

                Console.WriteLine(option.Maybe("F", v => string.Format("{0} is a Valid Size", v)));
            }

            //Fold?
            var opt = Option.Some("abcd");
            Console.WriteLine("Fold : {0}", opt.Fold("efgh", (a, b) => string.Concat(a, b)));

          
            Console.WriteLine();
        }

        private static void PlayAroundSeq()
        {
            Console.WriteLine("==================== Seq ====================");
            var fruits = new[] { "Mango", "Melon", "Durian", "Strawberry", "Kiwi", "Cherry", "Plum" };
            var numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var fruitsSeq = Seq.Create(fruits);
            var numSeq = Seq.Create(numbers);
            Console.WriteLine(fruitsSeq);
            Console.WriteLine(numSeq);
            Func<int, int> add1 = a => a + 1;
            var first100 = Seq.Init(100, add1);
            Console.WriteLine("{0} has {1} elements", first100, first100.Count());
            var repeated = Seq.Repeat(10, "A");
            Console.WriteLine("{0} has {1} elements", repeated, repeated.Count());
            var numInfinite = Seq.InitInfinite(add1);
            var take200 = numInfinite.Take(200);
            //Cannot count the infinite Seq, unless result in infinite loop
            //Console.WriteLine("Infinite Seq {0} has {1} elements", numInfinite, numInfinite.Count());
            Console.WriteLine("{0} is infinite Seq", numInfinite);
            Console.WriteLine("{0} has {1} elements", take200, take200.Count());

            Seq.Iter(fruitsSeq, a => Console.WriteLine("{0} is {1}", a, a.ToLower().Contains("e") ? "delicious" : "not tasty"));
            fruitsSeq.Iter(a => Console.WriteLine("{0} is {1}", a, a.ToLower().Contains("durian") ? "a king of fruits" : "so so"));

            var zip1 = Seq.Zip(fruitsSeq, numSeq, repeated);
            Console.WriteLine("{0} type of {1}", zip1, zip1.GetType());
            Seq.Iter(zip1, a => Console.WriteLine("{0}", a));

            var unzip1 = Seq.Unzip(zip1);
            Console.WriteLine(unzip1);
            unzip1.Item1.Iter(s => Console.WriteLine("{0} is {1}", s.ToString(), s is string ? "String" : "!String"));
            Action<string> writeLine = Console.WriteLine;
            var filterMapO = Seq.Filter(fruitsSeq, s => s.ToLower().Contains("o")).Map(a => String.Concat(a, @" has ""o"" or ""O"" character"));
            filterMapO.Iter(writeLine);

            Console.WriteLine("Sort : {0}", Seq.SortBy(fruitsSeq, a => a));
            Console.WriteLine("Revert Sort : {0}", Seq.RevSortBy(fruitsSeq, a => a));

            Console.WriteLine();
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
