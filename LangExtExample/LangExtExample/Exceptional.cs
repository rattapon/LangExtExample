using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangExtExample
{
    public class Exceptional<T>
    {
        public bool HasException { get; private set; }
        public Exception Exception { get; private set; }
        public T Value { get; private set; }

        public Exceptional(T value)
        {
            HasException = false;
            Value = value;
        }

        public Exceptional(Exception exception)
        {
            HasException = true;
            Exception = exception;
        }

        public Exceptional(Func<T> getValue)
        {
            try
            {
                Value = getValue();
                HasException = false;
            }
            catch (Exception exc)
            {
                Exception = exc;
                HasException = true;
            }
        }

        public override string ToString()
        {
            return (this.HasException ? Exception.GetType().Name : ((Value != null) ? Value.ToString() : "null"));
        }
    }

    public static class ExceptionalMonadExtensions
    {
        public static Exceptional<T> ToExceptional<T>(this T value)
        {
            return new Exceptional<T>(value);
        }

        public static Exceptional<T> ToExceptional<T>(this Func<T> getValue)
        {
            return new Exceptional<T>(getValue);
        }

        public static Exceptional<U> SelectMany<T, U>(this Exceptional<T> value, Func<T, Exceptional<U>> k)
        {
            return (value.HasException)
                ? new Exceptional<U>(value.Exception)
                : k(value.Value);
        }

        public static Exceptional<V> SelectMany<T, U, V>(this Exceptional<T> value, Func<T, Exceptional<U>> k, Func<T, U, V> m)
        {
            return value.SelectMany(t => k(t).SelectMany(u => m(t, u).ToExceptional()));
        }
    }

    public static class Exceptional
    {
        public static Exceptional<T> From<T>(T value)
        {
            return value.ToExceptional();
        }

        public static Exceptional<T> Execute<T>(Func<T> getValue)
        {
            return getValue.ToExceptional();
        }
    }

    public static class ExceptionalExtensions
    {
        public static Exceptional<U> ThenExecute<T, U>(this Exceptional<T> value, Func<T, U> getValue)
        {
            return value.SelectMany(x => Exceptional.Execute(() => getValue(x)));
        }
    }
}
