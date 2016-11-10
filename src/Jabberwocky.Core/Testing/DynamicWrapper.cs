using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Jabberwocky.Core.Testing
{
	/// <summary>
	/// Dynamic wrapper for accessing private class members
	/// </summary>
	/// <remarks>
	/// See: http://www.amazedsaint.com/2010/05/accessprivatewrapper-c-40-dynamic.html
	/// </remarks>
	public class DynamicWrapper : DynamicObject
	{

		/// <summary>
		/// The object we are going to wrap
		/// </summary>
		private readonly object _wrapped;

		private readonly Type _targetType;

		/// <summary>
		/// Specify the flags for accessing members
		/// </summary>
		private static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance
		                                    | BindingFlags.Static | BindingFlags.Public;

		/// <summary>
		/// Create a simple dynamic wrapper
		/// </summary>
		public DynamicWrapper(object o)
		{
			_wrapped = o;
			_targetType = o.GetType();
		}

		public DynamicWrapper(object o, Type type)
		{
			_wrapped = o;
			_targetType = type;
		}

		public static dynamic For<T>(T o)
		{
			return new DynamicWrapper(o, typeof(T));
		}

		public static dynamic For(object o)
		{
			return new DynamicWrapper(o, o.GetType());
		}

		/// <summary>
		/// Create an instance via the constructor matching the args 
		/// </summary>
		public static dynamic FromType(Assembly asm, string type, params object[] args)
		{

			var allt = asm.GetTypes();
			var t = allt.First(item => item.Name == type);


			var types = from a in args
				select a.GetType();

			//Gets the constructor matching the specified set of args
			var ctor = t.GetConstructor(flags, null, types.ToArray(), null);

			if (ctor != null)
			{
				var instance = ctor.Invoke(args);
				return DynamicWrapper.For(instance);
			}

			return null;
		}

		/// <summary>
		/// Try invoking a method
		/// </summary>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var types = from a in args
				select a.GetType();

			var method = _targetType.GetMethod
				(binder.Name, flags, null, types.ToArray(), null);

			if (method == null)
				return base.TryInvokeMember(binder, args, out result);

			result = method.Invoke(_wrapped, args);
			return true;
		}

		/// <summary>
		/// Tries to get a property or field with the given name
		/// </summary>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			//Try getting a property of that name
			var prop = _targetType.GetProperty(binder.Name, flags);
			
			if (prop == null)
			{
				//Try getting a field of that name
				var fld = _targetType.GetField(binder.Name, flags);
				if (fld != null)
				{
					result = fld.GetValue(_wrapped);
					return true;
				}
				return base.TryGetMember(binder, out result);
			}

			result = prop.GetValue(_wrapped, null);
			return true;
		}

		/// <summary>
		/// Tries to set a property or field with the given name
		/// </summary>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			var prop = _targetType.GetProperty(binder.Name, flags);
			if (prop == null)
			{
				var fld = _targetType.GetField(binder.Name, flags);
				if (fld != null)
				{
					fld.SetValue(_wrapped, value);
					return true;
				}
				return base.TrySetMember(binder, value);
			}

			prop.SetValue(_wrapped, value, null);
			return true;
		}
	}
}
