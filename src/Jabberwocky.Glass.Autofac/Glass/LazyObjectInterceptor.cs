using System;
using Castle.DynamicProxy;

namespace Jabberwocky.Glass.Autofac.Glass
{
	/// <summary>
	/// Class LazyObjectInterceptor
	/// </summary>
	public class LazyObjectInterceptor : IInterceptor
	{
		public Action<object> MappingAction { get; set; }
		private bool _isMapped = false;

		public object Actual { get; set; }



		#region IInterceptor Members

		/// <summary>
		/// Intercepts the specified invocation.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		public void Intercept(IInvocation invocation)
		{
			//create class
			if (Actual != null && _isMapped == false)
			{
				lock (Actual)
				{
					if (_isMapped == false)
					{
						_isMapped = true;
						MappingAction(Actual);

					}
				}
			}
			invocation.Proceed();
		}

		#endregion


	}
}
