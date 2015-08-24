using Microsoft.CodeAnalysis;

namespace Jabberwocky.Glass.CodeAnalysis.Util
{
	public static class GlassFactoryAnalyzerUtil
	{
		private const string GlassFactoryTypeAttributeName = "GlassFactoryTypeAttribute";
		private const string GlassFactoryTypeAttributeNamespace = "Jabberwocky.Glass.Factory.Attributes";

		private const string BaseInterfaceTypeName = "BaseInterface";
		private const string BaseInterfaceTypeNamespace = "Jabberwocky.Glass.Factory.Interfaces";

		public static bool IsGlassFactoryTypeAttribute(AttributeData data)
		{
			var symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
			string fullyQualifiedName = data.AttributeClass.ToDisplayString(symbolDisplayFormat);

			return fullyQualifiedName == $"{GlassFactoryTypeAttributeNamespace}.{GlassFactoryTypeAttributeName}";
		}

		public static bool InheritsBaseInterfaceClass(INamedTypeSymbol symbol)
		{
			var symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
			string fullyQualifiedName = symbol.BaseType?.ToDisplayString(symbolDisplayFormat);

			return fullyQualifiedName == $"{BaseInterfaceTypeNamespace}.{BaseInterfaceTypeName}";
		}

		public static string GetFullyQualifiedTypeName(ITypeSymbol symbol)
		{
			var symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
			return symbol.BaseType?.ToDisplayString(symbolDisplayFormat);
		}
	}
}
