using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LangExt;

namespace LangExtExample
{
    public struct Maybe<T>
    {
        public static Maybe<T> Null = default (Maybe<T>);
        private T value;

        public bool HasValue
        {
            get; private set;
        }

        public T Value
        {
            get
            {
                if (!HasValue) throw new Exception();
                return value;
            }
        }

        public Maybe(T value) : this()
        {
            HasValue = true;
            this.value = value;
        }

        public override string ToString()
        {
            return HasValue ? Value.ToString() : "Null";
        }
    }

    public static class MaybeExtensions
    {
        public static Maybe<T> Unit<T>(this T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<R> Bind<A, R>(this Maybe<A> maybe, Func<A, Maybe<R>> function)
        {
            return maybe.HasValue ? function(maybe.Value) : Maybe<R>.Null;
        }

        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> maybe, Func<A, Maybe<B>> function,
            Func<A, B, C> projection)
        {
            return maybe.Bind(outer => function(outer).Bind(inner => projection(outer, inner).Unit()));
        }

        public static Maybe<short> AsSmall(this int x)
        {
            return x >= 0 && x <= 100 ? ((short)x).Unit() : Maybe<short>.Null;
        }

        public static Maybe<short> QuerySyntax(this Maybe<int> maybe)
        {
            return from outer in maybe
                from inner in outer.AsSmall()
                select inner;
        }
    }
}
