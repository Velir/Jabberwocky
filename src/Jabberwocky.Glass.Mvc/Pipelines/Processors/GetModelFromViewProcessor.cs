using System;
using System.Web.Compilation;
using Glass.Mapper;
using Glass.Mapper.Sc.ModelCache;
using Jabberwocky.Glass.Mvc.Models.Factory;
using Jabberwocky.Glass.Pipelines.Processors;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.GetModel;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Mvc.Pipelines.Processors
{
	public class GetModelFromViewProcessor : ProcessorBase<GetModelArgs>
	{
		private readonly IModelCacheManager _modelCacheManager;
		private readonly IViewModelFactory _viewModelFactory;

		public GetModelFromViewProcessor(IModelCacheManager modelCacheManager, IViewModelFactory viewModelFactory)
		{
			if (modelCacheManager == null) throw new ArgumentNullException(nameof(modelCacheManager));
			if (viewModelFactory == null) throw new ArgumentNullException(nameof(viewModelFactory));
			_modelCacheManager = modelCacheManager;
			_viewModelFactory = viewModelFactory;

			ContextName = Context.DefaultContextName;
		}

		/// <summary>
		/// Gets or sets the name of the context.
		/// </summary>
		/// <value>
		/// The name of the context.
		/// </value>
		public string ContextName { get; set; }

		protected internal override void Run(GetModelArgs args)
		{
			if (!IsValidForProcessing(args))
			{
				return;
			}

			string path = GetViewPath(args);

			if (string.IsNullOrWhiteSpace(path))
			{
				return;
			}

			if (path.StartsWith("/sitecore/", StringComparison.InvariantCultureIgnoreCase))
			{
				// Exclude Sitecore client/Speak views
				return;
			}

			string cacheKey = _modelCacheManager.GetKey(path);
			Type modelType = _modelCacheManager.Get(cacheKey);

			if (modelType == typeof(NullModel))
			{
				// The model has been attempted before and is not useful
				return;
			}

			// The model type hasn't been found before or has been cleared.
			if (modelType == null)
			{
				modelType = GetModel(args, path);
				if (typeof(RenderingModel).IsAssignableFrom(modelType))
                		{
                    			//Abort if its RenderingModel
                    			return;
                		}
				_modelCacheManager.Add(cacheKey, modelType);

				if (modelType == typeof(NullModel))
				{
					// This is not the type we are looking for
					return;
				}
			}

			args.Result = _viewModelFactory.Create(modelType); ;
		}

		private string GetPathFromLayout(
			Database db,
			ID layoutId)
		{
			Item layout = db.GetItem(layoutId);

			return layout != null
				? layout["path"]
				: null;
		}

		private string GetViewPath(GetModelArgs args)
		{
			string path = args.Rendering.RenderingItem.InnerItem["path"];

			if (string.IsNullOrWhiteSpace(path) && args.Rendering.RenderingType == "Layout")
			{
				path = GetPathFromLayout(args.PageContext.Database, new ID(args.Rendering.LayoutId));
			}
			return path;
		}

		private Type GetModel(GetModelArgs args, string path)
		{
			Type compiledViewType = BuildManager.GetCompiledType(path);
			Type baseType = compiledViewType.BaseType;

			if (baseType == null || !baseType.IsGenericType)
			{
				Log.Error(string.Format(
					"View {0} compiled type {1} base type {2} does not have a single generic argument.",
					args.Rendering.RenderingItem.InnerItem["path"],
					compiledViewType,
					baseType), this);
				return typeof(NullModel);
			}

			Type proposedType = baseType.GetGenericArguments()[0];
			return proposedType == typeof(object)
				? typeof(NullModel)
				: proposedType;
		}

		private static bool IsValidForProcessing(GetModelArgs args)
		{
			if (args.Result != null)
			{
				return false;
			}

			if (!String.IsNullOrEmpty(args.Rendering.RenderingItem.InnerItem["Model"]))
			{
				return false;
			}

			return args.Rendering.RenderingType == "Layout" ||
					 args.Rendering.RenderingType == "View" ||
					 args.Rendering.RenderingType == "r" ||
					 args.Rendering.RenderingType == String.Empty;
		}
	}

	public class NullModel
	{

	}
}
