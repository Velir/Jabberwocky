using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Castle.DynamicProxy;
using Jabberwocky.Glass.Autofac.Util;
using GlassMapper = Glass.Mapper;

namespace Jabberwocky.Glass.Autofac.Glass.Tasks
{
	/// <summary>
	/// Allow Glass models to use Autofac construction.  See http://glass.lu/2013/08/08/MixingInIoc.html
	/// </summary>
	public class AutofacGlassContructorTask : GlassMapper.Pipelines.ObjectConstruction.IObjectConstructionTask
	{
		public void Execute(GlassMapper.Pipelines.ObjectConstruction.ObjectConstructionArgs args)
		{
			//check that no other task has created an object
			//also check that this is a dynamic object
			if (args.Result == null && !args.Configuration.Type.IsAssignableFrom(typeof(IDynamicMetaObjectProvider)))
			{
				//Get IOC container
				var container = AutofacConfig.ServiceLocator;
				//check to see if the type is registered with the Autofac container
				//if it isn't, return
				if (container == null || !container.IsRegistered(args.Configuration.Type))
				{
					return;
				}

				Action<object> mappingAction = target => args.Configuration.MapPropertiesToObject(target, args.Service, args.AbstractTypeCreationContext);
				object result;
				if (args.AbstractTypeCreationContext.IsLazy)
				{
					result = container.ResolveNamed(args.Configuration.Type.FullName + ":lazy", args.Configuration.Type);
					var proxy = result as IProxyTargetAccessor;
					var interceptor = proxy.GetInterceptors().First(x => x is LazyObjectInterceptor) as LazyObjectInterceptor;
					interceptor.MappingAction = mappingAction;
					interceptor.Actual = result;
				}
				else
				{
					result = container.Resolve(args.Configuration.Type);
					if (result != null)
					{
						mappingAction(result);
					}
				}

				//set the new object as the returned result
				args.Result = result;
			}
		}
	}
}
