using System;
using System.Collections.Generic;
using System.Linq;

namespace Jabberwocky.Core.Utils.Extensions
{
	public static class LinqExtensions
	{

		public static IEnumerable<T> Union<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
		{
			return first.Union(second, new LambdaEqualityComparer<T>(comparer));
		}

		public static IEnumerable<T> Intersect<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
		{
			return first.Intersect(second, new LambdaEqualityComparer<T>(comparer));
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
		{
			return first.Except(second, new LambdaEqualityComparer<T>(comparer));
		}

		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
		{
			return source.Distinct(new LambdaEqualityComparer<T>(comparer));
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
			IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
			Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TKey, bool> comparer)
		{
			return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer));
		}

		public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
			Func<TKey, TKey, int> comparer)
		{
			return source.OrderBy(keySelector, new LambdaComparer<TKey>(comparer));
		}

		public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
			Func<TKey, TKey, int> comparer)
		{
			return source.OrderByDescending(keySelector, new LambdaComparer<TKey>(comparer));
		}

		#region Comparer Implementations

		private class LambdaEqualityComparer<T> : IEqualityComparer<T>
		{
			private Func<T, T, bool> _innerComparer;

			public LambdaEqualityComparer(Func<T, T, bool> comparer)
			{
				if (comparer == null)
					throw new ArgumentNullException(nameof(comparer));
				_innerComparer = comparer;
			}

			public bool Equals(T x, T y)
			{
				return _innerComparer(x, y);
			}

			public int GetHashCode(T obj)
			{
				return 0;   // Don't care
			}
		}

		private class LambdaComparer<T> : IComparer<T>
		{
			private Func<T, T, int> _innerComparer;

			public LambdaComparer(Func<T, T, int> innerComparer)
			{
				_innerComparer = innerComparer;
			}

			public int Compare(T x, T y)
			{
				return _innerComparer(x, y);
			}
		}

		#endregion

	}
}
