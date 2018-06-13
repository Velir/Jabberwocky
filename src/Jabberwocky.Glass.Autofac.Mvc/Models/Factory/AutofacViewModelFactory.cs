using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Jabberwocky.Glass.Mvc.Services;

namespace Jabberwocky.Glass.Autofac.Mvc.Models.Factory
{
	public class AutofacViewModelFactory : Jabberwocky.Glass.Mvc.Models.Factory.ViewModelFactory
	{
		private readonly IComponentContext _resolver;
		
		public AutofacViewModelFactory(IComponentContext resolver, IRenderingContextService renderingContextService) : base(null, renderingContextService)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			_resolver = resolver;
		}

		protected override object ResolveViewModel(Type model, Type glassModelType, object glassModel, Type renderingParameterType,
			object renderingParameterModel)
		{
			return _resolver.ResolveOptional(model, GetModelConstructorParams(glassModelType, glassModel, renderingParameterType, renderingParameterModel));
		}

		private static Parameter[] GetModelConstructorParams(Type glassModelType, object glassModel, Type renderingModelType = null, object renderingModel = null)
		{
			var parameterArray = glassModel == null || glassModelType == null
					? new Parameter[0]
					: new Parameter[] { new TypedParameter(glassModelType, glassModel) };

			return renderingModelType == null || renderingModel == null
					? parameterArray
					: parameterArray
							.Concat(new[] { new TypedParameter(renderingModelType, renderingModel) })
							.ToArray();
		}
	}
}
