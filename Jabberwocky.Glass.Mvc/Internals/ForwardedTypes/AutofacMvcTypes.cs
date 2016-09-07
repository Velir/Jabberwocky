using System;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Mvc.Internals;

// ReSharper disable once CheckNamespace
namespace Jabberwocky.Glass.Autofac.Mvc.Attributes
{
    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.HttpGetOrInvalidHttpPostAttributeType)]
    public class HttpGetOrInvalidHttpPostAttribute : Glass.Mvc.Attributes.HttpGetOrInvalidHttpPostAttribute
    {
    }

    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.ValidateFormHandlerAttributeType)]
    public class ValidateFormHandlerAttribute : Glass.Mvc.Attributes.ValidateFormHandlerAttribute
    {
    }

    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.ValidHttpPostAttributeType)]
    public class ValidHttpPostAttribute : Glass.Mvc.Attributes.ValidHttpPostAttribute
    {
    }
}

namespace Jabberwocky.Glass.Autofac.Mvc.Models.Factory
{
    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.IViewModelFactoryType)]
    public interface IViewModelFactory : Glass.Mvc.Models.Factory.IViewModelFactory { }
}

namespace Jabberwocky.Glass.Autofac.Mvc.Services
{
    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.IRenderingContextServiceType)]
    public interface IRenderingContextService : Glass.Mvc.Services.IRenderingContextService { }

    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.RenderingContextServiceType)]
    public class RenderingContextService : Glass.Mvc.Services.RenderingContextService {
        public RenderingContextService(IGlassHtml glassHtml, ISitecoreContext context) : base(glassHtml, context)
        {
        }
    }

    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.DatasourceNestingOptionsType)]
    public enum DatasourceNestingOptions
    {
        Default,
        Never,
        Always
    }
}

namespace Jabberwocky.Glass.Autofac.Mvc.Util
{
    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.CustomSitecoreHelperType)]
    public class CustomSitecoreHelper : Glass.Mvc.Util.CustomSitecoreHelper {
        public CustomSitecoreHelper(HtmlHelper htmlHelper) : base(htmlHelper)
        {
        }
    }
}

namespace Jabberwocky.Glass.Autofac.Mvc.Views
{
    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.CustomGlassViewType)]
    public abstract class CustomGlassView<TModel> : Glass.Mvc.Views.CustomGlassView<TModel> where TModel : class
    {
    }
}