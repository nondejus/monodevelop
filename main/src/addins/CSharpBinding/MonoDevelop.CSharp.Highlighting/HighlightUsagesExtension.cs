// 
// HighlightUsagesExtension.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using MonoDevelop.Core;
using MonoDevelop.Ide.FindInFiles;
using System.Threading;
using MonoDevelop.SourceEditor;
using Microsoft.CodeAnalysis;
using MonoDevelop.Ide;
using MonoDevelop.Refactoring;
using Microsoft.CodeAnalysis.FindSymbols;
using MonoDevelop.Ide.TypeSystem;
using System.Threading.Tasks;
using System.Collections.Immutable;
using MonoDevelop.Ide.Editor;
using MonoDevelop.Ide.Editor.Extension;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using ICSharpCode.NRefactory6.CSharp;
using Microsoft.CodeAnalysis.Editor.CSharp.KeywordHighlighting.KeywordHighlighters;
using Microsoft.CodeAnalysis.Editor.Implementation.Highlighting;

namespace MonoDevelop.CSharp.Highlighting
{
	class UsageData
	{
		public RefactoringSymbolInfo SymbolInfo;
		public Document Document;
		public int Offset;

		public ISymbol Symbol {
			get { return SymbolInfo != null ? SymbolInfo.Symbol ?? SymbolInfo.DeclaredSymbol : null; }
		}
	}
	
	class HighlightUsagesExtension : AbstractUsagesExtension<UsageData>
	{
		CSharpSyntaxMode syntaxMode;
		static IHighlighter [] highlighters;

		static HighlightUsagesExtension ()
		{
			highlighters = typeof (HighlightUsagesExtension).Assembly
				.GetTypes ()
				.Where (t => !t.IsAbstract && typeof (IHighlighter).IsAssignableFrom (t))
				.Select (t => (IHighlighter)Activator.CreateInstance (t)).ToArray ();
		}
		protected override void Initialize ()
		{
			base.Initialize ();
			Editor.SetSelectionSurroundingProvider (new CSharpSelectionSurroundingProvider (Editor, DocumentContext));
			syntaxMode = new CSharpSyntaxMode (Editor, DocumentContext);
			Editor.SemanticHighlighting = syntaxMode;
		}

		public override void Dispose ()
		{
			if (syntaxMode != null) {
				Editor.SemanticHighlighting = null;
				syntaxMode.Dispose ();
				syntaxMode = null;
			}
			base.Dispose ();
		}
		
		protected async override Task<UsageData> ResolveAsync (CancellationToken token)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.FileName == FilePath.Null)
				return new UsageData ();
			var analysisDocument = doc.AnalysisDocument;
			if (analysisDocument == null)
				return new UsageData ();

			var symbolInfo = await RefactoringSymbolInfo.GetSymbolInfoAsync (doc, doc.Editor, token);
			if (symbolInfo.Symbol == null && symbolInfo.DeclaredSymbol == null)
				return new UsageData {
					Document = analysisDocument,
					Offset = doc.Editor.CaretOffset
				};
			if (symbolInfo.Symbol != null && !symbolInfo.Node.IsKind (SyntaxKind.IdentifierName)) 
				return new UsageData  {
					Document = analysisDocument,
					Offset = doc.Editor.CaretOffset
				};
			
			return new UsageData {
				Document = analysisDocument,
				SymbolInfo = symbolInfo,
				Offset = doc.Editor.CaretOffset
			};
		}

		protected override async Task<IEnumerable<MemberReference>> GetReferencesAsync (UsageData resolveResult, CancellationToken token)
		{
			var result = new List<MemberReference> ();
			if (resolveResult.Symbol == null) {
				if (resolveResult.Document == null)
					return result;
				var root = await resolveResult.Document.GetSyntaxRootAsync (token).ConfigureAwait (false);
				var doc2 = resolveResult.Document;

				foreach (var highlighter in highlighters) {
					try {
						foreach (var span in highlighter.GetHighlights (root, resolveResult.Offset, token)) {
							result.Add (new MemberReference (span, doc2.FilePath, span.Start, span.Length) {
								ReferenceUsageType = ReferenceUsageType.Keyword
							});
						}
					} catch (Exception e) {
						LoggingService.LogError ("Highlighter " + highlighter + " threw exception.", e);
					}
				}
				return result;
			}

			var doc = resolveResult.Document;
			var documents = ImmutableHashSet.Create (doc); 
			var symbol = resolveResult.Symbol;
			foreach (var loc in symbol.Locations) {
				if (loc.IsInSource && loc.SourceTree.FilePath == doc.FilePath)
					result.Add (new MemberReference (symbol, doc.FilePath, loc.SourceSpan.Start, loc.SourceSpan.Length) {
						ReferenceUsageType = ReferenceUsageType.Declariton	
					});
			}
			foreach (var mref in await SymbolFinder.FindReferencesAsync (symbol, TypeSystemService.Workspace.CurrentSolution, documents, token)) {
				foreach (var loc in mref.Locations) {
					result.Add (new MemberReference (symbol, doc.FilePath, loc.Location.SourceSpan.Start, loc.Location.SourceSpan.Length) {
						ReferenceUsageType = GetUsage (loc.Location.SourceTree.GetRoot ().FindNode (loc.Location.SourceSpan))
					});
				}
			}

			return result;
		}

		static ReferenceUsageType GetUsage (SyntaxNode node)
		{
			if (node == null)
				return ReferenceUsageType.Read;
			
			var parent = node.AncestorsAndSelf ().OfType<ExpressionSyntax> ().FirstOrDefault();
			if (parent == null)
				return ReferenceUsageType.Read;
			if (parent.IsOnlyWrittenTo ())
				return ReferenceUsageType.Write;
			if (parent.IsWrittenTo ())
				return ReferenceUsageType.ReadWrite;
			return ReferenceUsageType.Read;
		}

	}
}

