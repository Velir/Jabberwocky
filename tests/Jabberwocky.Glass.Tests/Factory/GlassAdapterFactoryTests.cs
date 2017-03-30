using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Attributes;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;
using NSubstitute;
using NUnit.Framework;

namespace Jabberwocky.Glass.Tests.Factory
{
	[TestFixture]
	public class GlassAdapterFactoryTests
	{
		private ISitecoreService _mockService;
		private IImplementationFactory _implFactory;
		private IGlassTemplateCacheService _templateCache;

		private ILookup<Type, GlassInterfaceMetadata> _interfaceMappings;

		private const string FakeTemplateString = "6C1F0868-7542-4B77-BAA7-4BB9CFBE60F3";
		private const string FakeEmptyBaseTemplate = "EFA3C0E7-F03B-494F-A0D9-C33B3B27E4AF";
		private const string FakeTemplateWithBase = "34BCA8F1-8F4E-4983-957C-0B18F0CD5C3F";
		private const string FakeNoDirectWithBaseTemplate = "15BCA8F1-8F4E-4983-957C-0B18F0CD5C3F";
		private const string FakeIntermediateTemplate = "92BCF3F1-8F4E-4983-957C-0B18F0CD5C3F";

		// SUT
		private GlassAdapterFactory _glassFactory;

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
							IsFallback = true,
							ZIndex = 0
						},
						// Actual sitecore template type, inherits BaseType
						new GlassInterfaceMetadata
						{
							GlassType = typeof (IInheritedTemplate),
							ImplementationType = typeof (IInheritedTemplateLowerPriorityModel),
							IsFallback = false,
							ZIndex = 0
						},
						// Actual sitecore template type, inherits BaseType
						new GlassInterfaceMetadata
						{
							GlassType = typeof (IInheritedTemplate),
							ImplementationType = typeof (IInheritedTemplateModel),
							IsFallback = false,
							ZIndex = 1
						},
						new GlassInterfaceMetadata
						{
							GlassType = typeof (IIntermediateTemplate),
							ImplementationType = typeof(IIntermediateTemplateModel),
							IsFallback = false,
							ZIndex = 0
						}
					}
				},
				{
					typeof(INestedTestInterface), new[]
					{
						new GlassInterfaceMetadata
						{
							GlassType = typeof (IIntermediateTemplate),
							ImplementationType = typeof(IIntermediateTemplateModel),
							IsFallback = false,
							ZIndex = 0
						},
						// Base Type, matches no direct template
						new GlassInterfaceMetadata
						{
							GlassType = typeof (IBaseType),
							ImplementationType = typeof (IBaseTypeModel),
							IsFallback = true,
							ZIndex = 0
						},
					}
				}
			};

			_interfaceMappings = typeDictionary
				.SelectMany(pair => pair.Value,
					(pair, metadata) => new KeyValuePair<Type, GlassInterfaceMetadata>(pair.Key, metadata))
				.ToLookup(pair => pair.Key, pair => pair.Value);

			// Tightly-coupled test dependency... hmm
			_implFactory = new ProxyImplementationFactory((t, model) => new FallbackInterceptor(t, model, _glassFactory.TemplateCacheService, _implFactory));

			_templateCache = new GlassTemplateCacheService(_interfaceMappings, () => _mockService);

			// System Under Test
			_glassFactory = new GlassAdapterFactory(_templateCache, _implFactory);
		}

		[Test]
		public void GlassFactory_NoMatchingTemplate_Fallsthrough()
		{
			var mockItem = Substitute.For<IBaseType>();
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

		[Test]
		public void GlassFactory_DepthFirstBaseTemplateSearch_ReturnsMatchBeforeFallbackImplementation()
		{
			var mockItem = Substitute.For<ITemplateWithBase>();
			{
				mockItem._Id = Guid.NewGuid();
				mockItem._TemplateId.Returns(new Guid(FakeTemplateWithBase)); // matching template
				mockItem._BaseTemplates.Returns(new[] {new Guid(FakeEmptyBaseTemplate), new Guid(FakeTemplateString)});
				var templateItem = Substitute.For<IBaseTemplates>();
				templateItem.BaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.TemplateBaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.Template.Returns(Guid.NewGuid());

				_mockService.GetItem<IBaseTemplates>(mockItem._TemplateId).Returns(templateItem);
			}

			{
				var item = Substitute.For<IEmptyBaseTemplate>();
				item._Id = Guid.NewGuid();
				item._TemplateId.Returns(new Guid(FakeEmptyBaseTemplate));  // matching template
				item._BaseTemplates.Returns(new Guid[0]);
				var templateItem = Substitute.For<IBaseTemplates>();
				templateItem.BaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.TemplateBaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.Template.Returns(Guid.NewGuid());

				_mockService.GetItem<IBaseTemplates>(item._TemplateId).Returns(templateItem);
			}

			{
				var item = Substitute.For<IInheritedTemplate>();
				item._Id = Guid.NewGuid();
				item._TemplateId.Returns(new Guid(FakeTemplateString));	// matching template
				item._BaseTemplates.Returns(new Guid[0]);
				var templateItem = Substitute.For<IBaseTemplates>();
				templateItem.BaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.TemplateBaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.Template.Returns(Guid.NewGuid());

				_mockService.GetItem<IBaseTemplates>(item._TemplateId).Returns(templateItem);
			}

			var testItem = _glassFactory.GetItem<ITestInterface>(mockItem);
			Assert.IsNotNull(testItem);
			Assert.IsTrue(testItem.VirtualIsNotFallback);
		}

		[Test]
		public void GlassFactory_FallbackBehavior_WillSelectDepthFirstImplementation()
		{
			// Key notes about this test:
			// 1. Validates fallback behavior
			// 2. Fallback selection aglorithm will select next concrete (non-abstract) implementation of GlassFactoryType model's template hierarchy,
			//	  and not the initial Glass model's template hierarchy.  This is desirable to more closely mimic standard object-oriented inheritance...
			// 3. However, it is an open question if we want to make this configurable (is there a need?)

			var mockItem = Substitute.For<INoMatchWithBase>();
			{
				var nonExistentId = new Guid("2E812B94-0C52-41C8-8DF7-66B512933391");

				mockItem._Id = Guid.NewGuid();
				mockItem._TemplateId.Returns(nonExistentId); // no direct match, or even nested match
				mockItem._BaseTemplates.Returns(new[] { new Guid(FakeEmptyBaseTemplate), new Guid(FakeTemplateString) });
				var templateItem = Substitute.For<IBaseTemplates>();
				templateItem.BaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.TemplateBaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.Template.Returns(new Guid(FakeIntermediateTemplate)); // This template will be used for initial model

				_mockService.GetItem<IBaseTemplates>(mockItem._TemplateId).Returns(templateItem);
			}

			{
				var item = Substitute.For<IIntermediateTemplate>();
				item._Id = Guid.NewGuid();
				item._TemplateId.Returns(new Guid(FakeIntermediateTemplate));  // matching template
				item._BaseTemplates.Returns(new Guid[0]);
				var templateItem = Substitute.For<IBaseTemplates>();
				templateItem.BaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.TemplateBaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.Template.Returns(Guid.NewGuid());

				_mockService.GetItem<IBaseTemplates>(item._TemplateId).Returns(templateItem);
			}

			{
				var item = Substitute.For<IEmptyBaseTemplate>();
				item._Id = Guid.NewGuid();
				item._TemplateId.Returns(new Guid(FakeEmptyBaseTemplate));  // matching template
				item._BaseTemplates.Returns(new Guid[0]);
				var templateItem = Substitute.For<IBaseTemplates>();
				templateItem.BaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.TemplateBaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.Template.Returns(Guid.NewGuid());

				_mockService.GetItem<IBaseTemplates>(item._TemplateId).Returns(templateItem);
			}

			{
				var item = Substitute.For<IInheritedTemplate>();
				item._Id = Guid.NewGuid();
				item._TemplateId.Returns(new Guid(FakeTemplateString)); // matching template
				item._BaseTemplates.Returns(new Guid[0]);
				var templateItem = Substitute.For<IBaseTemplates>();
				templateItem.BaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.TemplateBaseTemplates.Returns(ci => mockItem._BaseTemplates.ToArray());
				templateItem.Template.Returns(Guid.NewGuid());

				_mockService.GetItem<IBaseTemplates>(item._TemplateId).Returns(templateItem);
			}

			var testItem = _glassFactory.GetItem<ITestInterface>(mockItem);
			Assert.IsNotNull(testItem);
			Assert.IsTrue(testItem.IsFallback); // Should fallback to IInheritedTemplateModel -> BaseType
		}

		[Test]
		public void GlassFactory_ZIndexBehavior_WillHigherPriorityImplementation()
		{
			// Key notes about this test:
			// 1. Validates fallback behavior
			// 2. Fallback selection aglorithm will select next concrete (non-abstract) implementation of GlassFactoryType model's template hierarchy,
			//	  and not the initial Glass model's template hierarchy.  This is desirable to more closely mimic standard object-oriented inheritance...
			// 3. However, it is an open question if we want to make this configurable (is there a need?)

			var mockItem = Substitute.For<IInheritedTemplate>();
			mockItem._Id = Guid.NewGuid();
			mockItem._TemplateId.Returns(new Guid(FakeTemplateString)); // matching template
			mockItem._BaseTemplates.Returns(new Guid[0]);

			var testItem = _glassFactory.GetItem<ITestInterface>(mockItem);
			Assert.IsNotNull(testItem);
			Assert.AreEqual("IInheritedTemplateModel", testItem.ZIndexTest); // Should fallback to IInheritedTemplateModel -> BaseType
		}

		[Test]
		public void GlassFactory_FallbackBehavior_WorksWithNestedInterfaces()
		{
			
			var mockItem = Substitute.For<IIntermediateTemplate>();
			mockItem._Id = Guid.NewGuid();
			mockItem._TemplateId.Returns(new Guid(FakeIntermediateTemplate)); // matching template
			mockItem._BaseTemplates.Returns(new Guid[0]);

			var testItem = _glassFactory.GetItem<INestedTestInterface>(mockItem);
			Assert.IsNotNull(testItem);
			Assert.AreEqual("IBaseTypeModel", testItem.ZIndexTest); // Should fallback to IIntermediateTemplateModel -> BaseType
		}

		private ITestInterface GetItemWithFallback()
		{
			var mockItem = Substitute.For<IBaseType>();
			mockItem._TemplateId.Returns(new Guid(FakeTemplateString)); // Matching template
			mockItem._BaseTemplates.Returns(new Guid[0]);

			var testItem = _glassFactory.GetItem<ITestInterface>(mockItem);
			return testItem;
		}

		#region Glass Interface & Model Definitions

		// No templateID (or even attribute!)
		public interface IBaseType : IGlassBase
		{
		}

		// Fake base template with no direct match (but match on base template)
		[SitecoreType(TemplateId = FakeNoDirectWithBaseTemplate)]
		public interface INoMatchWithBase : IIntermediateTemplate, IInheritedTemplate
		{
		}

		[SitecoreType(TemplateId = FakeIntermediateTemplate)]
		public interface IIntermediateTemplate : IBaseType
		{

		}

		// Fake base template
		[SitecoreType(TemplateId = FakeEmptyBaseTemplate)]
		public interface IEmptyBaseTemplate : IGlassBase { }

		// This template models a parent template that contains 2 base templates, the first of which itself has 0 base templates
		[SitecoreType(TemplateId = FakeTemplateWithBase)]
		public interface ITemplateWithBase : IEmptyBaseTemplate, IInheritedTemplate, IGlassBase { }

		// Fake template
		[SitecoreType(TemplateId = FakeTemplateString)]
		public interface IInheritedTemplate : IBaseType
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

			object DontCallMe();

			string ZIndexTest { get; }
		}

		[GlassFactoryInterface]
		private interface INestedTestInterface : ITestInterface
		{
			
		}

		[GlassFactoryType(typeof (IInheritedTemplate), ZIndex = 1)]
		public abstract class IInheritedTemplateModel : ITestInterface
		{
			public abstract bool IsFallback { get; }

			public bool IsNotFallback => true;

			// Important for test: must be marked virtual
			public virtual bool VirtualIsNotFallback => true;

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

			public object DontCallMe()
			{
				return new object();
			}

			public string ZIndexTest => typeof(IInheritedTemplateModel).Name;
		}

		[GlassFactoryType(typeof(IInheritedTemplate))]
		public abstract class IInheritedTemplateLowerPriorityModel : ITestInterface
		{
			public abstract bool IsFallback { get; }

			public bool IsNotFallback => true;

			// Important for test: must be marked virtual
			public virtual bool VirtualIsNotFallback => true;

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

			public object DontCallMe()
			{
				return new object();
			}

			public string ZIndexTest => typeof(IInheritedTemplateLowerPriorityModel).Name;
		}

		[GlassFactoryType(typeof (IBaseType))]
		public abstract class IBaseTypeModel : ITestInterface, INestedTestInterface
		{
			private readonly IBaseType _innerItem;
			protected IBaseTypeModel(IBaseType innerItem)
			{
				_innerItem = innerItem;
			}

			public bool IsFallback => true;

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

			public object DontCallMe()
			{
				Assert.Fail("I should never be called");
				return null;
			}

			public string ZIndexTest => typeof(IBaseTypeModel).Name;
		}

		[GlassFactoryType(typeof(IIntermediateTemplate))]
		public abstract class IIntermediateTemplateModel : INestedTestInterface
		{
			public abstract bool IsNotFallback { get; } 
			public abstract bool IsFallback { get; }
			public abstract bool VirtualIsNotFallback { get; }

			public abstract T GetGenericItem<T>() where T : new();
			public abstract object NonVirtualNotImplementedMethod();
			public abstract object VirtualNotImplementedMethod();

			public abstract object DontCallMe();
			public abstract string ZIndexTest { get; }
		}

		#endregion
	}
}
