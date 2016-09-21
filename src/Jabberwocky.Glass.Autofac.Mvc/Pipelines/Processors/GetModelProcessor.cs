using System;
using Glass.Mapper.Sc.Configuration.Attributes;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using Jabberwocky.Glass.Mvc.Models.Factory;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Data;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.GetModel;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Autofac.Mvc.Pipelines.Processors
{
	public class GetModelProcessor : ProcessorBase<GetModelArgs>
	{
		/// <summary>
		/// The model type field
		/// </summary>
		public const string ModelTypeField = "Model Type";

		/// <summary>
		/// The model field
		/// </summary>
		public const string ModelField = "Model";

		private static readonly Type RenderingModelType = typeof(IRenderingModel);

		/// <summary>
		/// Recursive depth limit for guarding against infinite loops during model resolution
		/// </summary>
		/// <remarks>
		/// This value is 'inclusive', so depth of 2 is valid; depth of 3 is not
		/// </remarks>
		private const int RecursionDepthLimit = 2;

		private readonly IViewModelFactory _viewModelFactory;

		public GetModelProcessor(IViewModelFactory viewModelFactory)
		{
			if (viewModelFactory == null) throw new ArgumentNullException(nameof(viewModelFactory));
			_viewModelFactory = viewModelFactory;
		}

		protected internal override void Run(GetModelArgs args)
		{
			if (args.Result != null) return;

			var rendering = args.Rendering;

			if (rendering.RenderingType == "Layout")
			{
				args.Result = GetFromItem(rendering, args)
					?? GetFromLayout(rendering, args);
			}

			if (args.Result == null)
			{
				args.Result = GetFromPropertyValue(rendering, args);
			}
			if (args.Result == null)
			{
				args.Result = GetFromField(rendering, args);
			}

			if (args.Result != null)
			{
				args.AbortPipeline();
			}
		}

		protected virtual object GetObject(string model, Database database, Rendering rendering, int recursionGuard = 0)
		{
			if (string.IsNullOrEmpty(model)) return null;

			if (++recursionGuard > RecursionDepthLimit)
			{
				throw new InvalidOperationException(string.Format("GetObject model resolution exceeded maximum level of recursion: {0}", RecursionDepthLimit));
			}

			// must be a path to a Model item
			if (model.StartsWith("/sitecore"))
			{
				var target = database.GetItem(model);
				if (target == null)
					return null;

				string newModel = target[ModelTypeField];
				return GetObject(newModel, database, rendering, recursionGuard);
			}
			//if guid must be that to Model item
			Guid targetId;
			if (Guid.TryParse(model, out targetId))
			{
				var target = database.GetItem(new ID(targetId));
				if (target == null)
					return null;

				string newModel = target[ModelTypeField];
				return GetObject(newModel, database, rendering, recursionGuard);
			}
			
			// If we get here, we're in the 'primary' resolution path; no more recursion
			var type = Type.GetType(model, false);

			if (type == null || RenderingModelType.IsAssignableFrom(type))
				return null;

			// TODO: Maybe cache existence of SitecoreType attribute per Type, so as to avoid 'costly' reflection every time a model is created?
			// If this is a Glass Mapper model (template type), then don't handle this type, and continue the pipeline execution
			if (type.IsDefined(typeof(SitecoreTypeAttribute), true))
			{
				return null;
			}

			return _viewModelFactory.Create(type);
		}

		/// <summary>
		/// Gets from field.
		/// </summary>
		/// <param name="rendering">The rendering.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		protected virtual object GetFromField(Rendering rendering, GetModelArgs args)
		{
			Item obj = rendering.RenderingItem.ValueOrDefault(i => i.InnerItem);
			if (obj == null)
				return null;

			return rendering.Item == null
				? null
				: GetObject(obj[ModelField], rendering.Item.Database, rendering);
		}

		/// <summary>
		/// Gets from property value.
		/// </summary>
		/// <param name="rendering">The rendering.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		protected virtual object GetFromPropertyValue(Rendering rendering, GetModelArgs args)
		{
			string model = rendering.Properties[ModelField];
			if (model.IsWhiteSpaceOrNull())
				return null;
			else
				return GetObject(model, rendering.Item.Database, rendering);
		}

		/// <summary>
		/// Gets from layout.
		/// </summary>
		/// <param name="rendering">The rendering.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		protected virtual object GetFromLayout(Rendering rendering, GetModelArgs args)
		{
			string pathOrId = rendering.Properties["LayoutId"];
			if (pathOrId.IsWhiteSpaceOrNull())
				return null;
			string model = MvcSettings.GetRegisteredObject<ItemLocator>().GetItem(pathOrId).ValueOrDefault(i => i[ModelField]);
			if (model.IsWhiteSpaceOrNull())
				return null;
			else
				return GetObject(model, rendering.Item.Database, rendering);
		}

		/// <summary>
		/// Gets from item.
		/// </summary>
		/// <param name="rendering">The rendering.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		protected virtual object GetFromItem(Rendering rendering, GetModelArgs args)
		{
			string model = rendering.Item.ValueOrDefault(i => i["MvcLayoutModel"]);
			if (model.IsWhiteSpaceOrNull())
				return null;
			else
				return GetObject(model, rendering.Item.Database, rendering);
		}
	}
}
