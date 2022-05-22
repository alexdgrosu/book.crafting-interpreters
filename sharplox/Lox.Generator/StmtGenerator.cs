using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using static Lox.Generator.GenerateAst;

namespace Lox.Generator;

[Generator]
public class StmtGenerator : ISourceGenerator
{
  public void Execute(GeneratorExecutionContext context)
  {
    string stmtSource = DefineAst("Stmt", new[]
  {
      "Expression : Expr xpression",
      "Print      : Expr xpression"
    });

    context.AddSource("Stmt.g.cs", SourceText.From(stmtSource, Encoding.UTF8));
  }

  public void Initialize(GeneratorInitializationContext context) { }
}
