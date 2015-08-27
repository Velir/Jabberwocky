using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jabberwocky.Glass.CodeAnalysis.GlassFactory
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(GlassFactorySuspiciousPropertyCodeFixProvider)), Shared]
	public class GlassFactorySuspiciousPropertyCodeFixProvider : CodeFixProvider
	{
		private const string MakeAbstractTitle = "Make Abstract";
		private const string CreateInitializerTitle = "Initialize With Default Value";

		public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(GlassFactorySuspiciousPropertyAnalyzer.DiagnosticId);

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			foreach (var diagnostic in context.Diagnostics)
			{
				var diagnosticSpan = diagnostic.Location.SourceSpan;

				// Find the property declaration identified by the diagnostic.
				var declaration =
					root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().FirstOrDefault();

				// Only operate on class declarations explicitly
				if (declaration == null) return;

				// Register a code action that will invoke the fix.
				context.RegisterCodeFix(
					CodeAction.Create(
						title: MakeAbstractTitle,
						createChangedSolution: c => MakeAbstractAsync(context.Document, declaration, c),
						equivalenceKey: MakeAbstractTitle),
					diagnostic);

				context.RegisterCodeFix(
					CodeAction.Create(
						title: CreateInitializerTitle,
						createChangedSolution: c => CreateInitializerAsync(context.Document, declaration, c),
						equivalenceKey: CreateInitializerTitle),
					diagnostic);
			}
		}

		private async Task<Solution> MakeAbstractAsync(Document document, PropertyDeclarationSyntax propDecl, CancellationToken cancellationToken)
		{
			var newModifiers = SyntaxFactory.TokenList(propDecl.Modifiers.Concat(new[] { SyntaxFactory.Token(SyntaxKind.AbstractKeyword) }));
			var newTypeDecl = propDecl.WithModifiers(newModifiers);

			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

			var newRoot = root.ReplaceNode(propDecl, newTypeDecl);
			var newDocument = document.WithSyntaxRoot(newRoot);
			var newSolution = newDocument.Project.Solution;

			// Return the new solution with the now-abstract property.
			return newSolution;
		}

		private async Task<Solution> CreateInitializerAsync(Document document, PropertyDeclarationSyntax propDecl, CancellationToken cancellationToken)
		{
			var propertyTypeSyntax = propDecl.Type;

			var initializerExpression = SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(propertyTypeSyntax));
			var newTypeDecl = propDecl.WithoutTrailingTrivia().WithInitializer(initializerExpression).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

			var newRoot = root.ReplaceNode(propDecl, newTypeDecl);
			var newDocument = document.WithSyntaxRoot(newRoot);
			var newSolution = newDocument.Project.Solution;

			// Return the new solution with the now-abstract type name.
			return newSolution;
		}
	}
}
