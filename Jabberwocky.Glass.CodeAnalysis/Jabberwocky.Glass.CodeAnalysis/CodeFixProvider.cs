using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jabberwocky.Glass.CodeAnalysis
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(JabberwockyGlassCodeAnalysisCodeFixProvider)), Shared]
	public class JabberwockyGlassCodeAnalysisCodeFixProvider : CodeFixProvider
	{
		private const string Title = "Make abstract";

		public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(GlassFactoryTypeIsAbstractAnalyzer.DiagnosticId);

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

				// Find the type declaration identified by the diagnostic.
				var declaration =
					root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();

				// Only operate on class declarations explicitly
				if (declaration == null) return;

				// Register a code action that will invoke the fix.
				context.RegisterCodeFix(
					CodeAction.Create(
						title: Title,
						createChangedSolution: c => MakeAbstractAsync(context.Document, declaration, c),
						equivalenceKey: Title),
					diagnostic);
			}
		}

		private async Task<Solution> MakeAbstractAsync(Document document, ClassDeclarationSyntax typeDecl, CancellationToken cancellationToken)
		{
			var newModifiers = SyntaxFactory.TokenList(typeDecl.Modifiers.Concat(new[] { SyntaxFactory.Token(SyntaxKind.AbstractKeyword) }));
			var newTypeDecl = typeDecl.WithModifiers(newModifiers);
			
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

			var newRoot = root.ReplaceNode(typeDecl, newTypeDecl);
			var newDocument = document.WithSyntaxRoot(newRoot);
			var newSolution = newDocument.Project.Solution;

			// Return the new solution with the now-abstract type name.
			return newSolution;
		}
	}
}