using System;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory.Interfaces
{
	public abstract class BaseInterface<T> where T : class, IGlassBase
	{
		protected virtual T InnerItem { get; private set; }

		protected BaseInterface(T innerItem)
		{
			if (innerItem == null) throw new ArgumentNullException("innerItem");
			InnerItem = innerItem;
		}
	}
}
