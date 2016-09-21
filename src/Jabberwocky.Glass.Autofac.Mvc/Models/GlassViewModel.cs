using Jabberwocky.Glass.Models;
using Jabberwocky.Glass.Mvc.Models;

namespace Jabberwocky.Glass.Autofac.Mvc.Models
{
	public abstract class GlassViewModel<TGlassModel> : InjectableGlassViewModelBase where TGlassModel : class, IGlassBase
	{
		public virtual TGlassModel GlassModel => InternalDatasourceModel as TGlassModel;
	}

	public abstract class GlassViewModel<TDatasource, TRenderingParameter> : GlassViewModel<TDatasource>
			where TDatasource : class, IGlassBase where TRenderingParameter : class
	{
		public virtual TRenderingParameter RenderingParameters => InternalRenderingParameterModel as TRenderingParameter;
	}
}
