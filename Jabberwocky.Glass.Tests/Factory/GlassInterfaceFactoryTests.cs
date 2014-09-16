using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Attributes;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;
using NSubstitute;
using NUnit.Framework;

namespace Jabberwocky.Glass.Tests.Factory
{
	[TestFixture]
	public class GlassInterfaceFactoryTests
	{
		private ISitecoreService _mockService;
		private IImplementationFactory _implFactory;

		private ILookup<Type, GlassInterfaceMetadata> _interfaceMappings;

		private const string FakeTemplateString = "6C1F0868-7542-4B77-BAA7-4BB9CFBE60F3";

		// SUT
		private GlassInterfaceFactory _glassFactory;

		[SetUp]
		public void Initialize()
		{
			_mockService = Substitute.For<ISitecoreService>();

			var typeDictionary = new Dictionary<Type, IEnumerable<GlassInterfaceMetadata>>
			{
				// Type of Glass Factory Interface (and associated 'abstract' implementations)
				{
					typeof (ITestInterface), new[]
					{
						// Base Type, matches no direct template
						new GlassInterfaceMetadata
						{
							GlassType = typeof (IBaseType),
							ImplementationType = typeof (IBaseTypeModel),
							IsFallback = true
						},
						// Actual sitecore template type, inherits BaseType
						new GlassInterfaceMetadata
						{
							GlassType = typeof (IInheritedTemplate),
							ImplementationType = typeof (IInheritedTemplateModel),
							IsFallback = false
						},
					}
				}
			};

			_interfaceMappings = typeDictionary
				.SelectMany(pair => pair.Value,
					(pair, metadata) => new KeyValuePair<Type, GlassInterfaceMetadata>(pair.Key, metadata))
				.ToLookup(pair => pair.Key, pair => pair.Value);

			// Tightly-coupled test dependency... hmm
			_implFactory = new ProxyImplementationFactory((t, model) => new FallbackInterceptor(t, model, _glassFactory, _implFactory, _mockService));

			// System Under Test
			_glassFactory = new GlassInterfaceFactory(_interfaceMappings, _implFactory, () => _mockService);
		}

		[Test]
		public void GlassFactory_NoMatchingTemplate_Fallsthrough()
		{
			var mockItem = Substitute.For<IGlassBase>();
			mockItem._TemplateId.Returns(Guid.NewGuid()); // No match
			mockItem._BaseTemplates.Returns(new[] {Guid.Empty});

			var testItem = _glassFactory.GetItem<ITestInterface>(mockItem);

			Assert.IsNotNull(testItem);
			Assert.IsTrue(testItem.IsFallback);
			Assert.IsFalse(testItem.IsNotFallback);
		}

		[Test]
		public void GlassFactory_MatchingTemplate_DoesNotFallThrough()
		{
			var testItem = GetItemWithFallback();

			Assert.IsNotNull(testItem);
			Assert.IsTrue(testItem.IsFallback);
			Assert.IsTrue(testItem.IsNotFallback);
		}

		[Test]
		public void GlassFactory_MatchingTemplateWithVirtualProperty_DoesNotFallThrough()
		{
			var testItem = GetItemWithFallback();

			Assert.IsNotNull(testItem);
			Assert.IsTrue(testItem.VirtualIsNotFallback);
		}

		[Test]
		public void GlassFactory_GenericMethod_Fallsthrough()
		{
			var testItem = GetItemWithFallback();

			Assert.IsNotNull(testItem);
			Assert.IsNotNull(testItem.GetGenericItem<object>());
		}

		[Test]
		public void GlassFactory_VirtualNotImplementedMethod_FallsthroughReturnsNull()
		{
			var testItem = GetItemWithFallback();

			Assert.IsNotNull(testItem);
			Assert.IsNull(testItem.VirtualNotImplementedMethod());
		}

		[Test]
		[ExpectedException(typeof (NotImplementedException))]
		public void GlassFactory_NonVirtualNotImplementedMethod_Throws()
		{
			var testItem = GetItemWithFallback();

			Assert.IsNotNull(testItem);
			// Should throw
			testItem.NonVirtualNotImplementedMethod();
		}

		private ITestInterface GetItemWithFallback()
		{
			var mockItem = Substitute.For<IGlassBase>();
			mockItem._TemplateId.Returns(new Guid(FakeTemplateString)); // Matching template
			mockItem._BaseTemplates.Returns(new[] {Guid.Empty});

			var testItem = _glassFactory.GetItem<ITestInterface>(mockItem);
			return testItem;
		}

		#region Glass Interface & Model Definitions

		// No templateID (or even attribute!)
		private interface IBaseType
		{
		}

		// Fake template
		[SitecoreType(TemplateId = FakeTemplateString)]
		private interface IInheritedTemplate : IBaseType
		{
		}

		[GlassFactoryInterface]
		private interface ITestInterface
		{
			bool IsFallback { get; }
			bool IsNotFallback { get; }
			bool VirtualIsNotFallback { get; }

			object VirtualNotImplementedMethod();
			object NonVirtualNotImplementedMethod();

			// Generic method with constraint
			T GetGenericItem<T>() where T : new();
		}

		[GlassFactoryType(typeof (IInheritedTemplate))]
		public abstract class IInheritedTemplateModel : ITestInterface
		{
			public abstract bool IsFallback { get; }

			public bool IsNotFallback
			{
				get { return true; }
			}

			// Important for test: must be marked virtual
			public virtual bool VirtualIsNotFallback
			{
				get { return true; }
			}

			// Important for test: must be marked virtual
			public virtual object VirtualNotImplementedMethod()
			{
				throw new NotImplementedException();
			}

			// Important for test: must NOT be marked virtual
			public object NonVirtualNotImplementedMethod()
			{
				throw new NotImplementedException();
			}

			public abstract T GetGenericItem<T>() where T : new();
		}

		[GlassFactoryType(typeof (IBaseType))]
		public abstract class IBaseTypeModel : ITestInterface
		{
			public bool IsFallback
			{
				get { return true; }
			}

			public abstract bool IsNotFallback { get; }
			public abstract bool VirtualIsNotFallback { get; }

			// Important for test: must be marked virtual
			public virtual object VirtualNotImplementedMethod()
			{
				throw new NotImplementedException();
			}

			// Important for test: must NOT be marked virtual
			public abstract object NonVirtualNotImplementedMethod();

			public T GetGenericItem<T>() where T : new()
			{
				return new T();
			}
		}

		#endregion
	}
}
