using System;
using System.IO;
using Sitecore.Mvc.Presentation;
using StackExchange.Profiling;
using Profiler = StackExchange.Profiling.MiniProfiler;

namespace Jabberwocky.Autofac.Extras.MiniProfiler.Sc.Renderer
{
	public class MiniProfilerRenderer : Sitecore.Mvc.Presentation.Renderer
	{
		private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;
		private readonly Rendering _rendering;

		public MiniProfilerRenderer(Sitecore.Mvc.Presentation.Renderer innerRenderer, Rendering rendering)
		{
			if (innerRenderer == null) throw new ArgumentNullException(nameof(innerRenderer));
			if (rendering == null) throw new ArgumentNullException(nameof(rendering));
			_innerRenderer = innerRenderer;
			_rendering = rendering;
		}

		public override string CacheKey => _innerRenderer.CacheKey;

		public override void Render(TextWriter writer)
		{
			using (Profiler.Current.Step($"Rendering:{_rendering.RenderingItem?.Name}"))
			{
				_innerRenderer.Render(writer);
			}
		}

		public override string ToString()
		{
			return _innerRenderer.ToString();
		}
	}
}
