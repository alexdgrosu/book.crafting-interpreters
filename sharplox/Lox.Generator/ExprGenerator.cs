using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using static Lox.Generator.GenerateAst;

namespace Lox.Generator;

[Generator]
public class ExprGenerator : ISourceGenerator
{
  public void Execute(GeneratorExecutionContext context)
  {
    string exprSource = DefineAst("Expr", new[]
    {
      "Binary   : Expr left, Token @operator, Expr right",
      "Grouping : Expr xpression",
      "Literal  : object value",
      "Unary    : Token @operator, Expr right",
      "Variable : Token name"
    });

    context.AddSource("Expr.g.cs", SourceText.From(exprSource, Encoding.UTF8));
  }

  public void Initialize(GeneratorInitializationContext context) { }
}
