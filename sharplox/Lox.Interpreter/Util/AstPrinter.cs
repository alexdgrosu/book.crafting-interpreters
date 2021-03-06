using System.Text;
using Lox.Interpreter.Ast;

namespace Lox.Interpreter.Util;

public class AstPrinter : Expr.IVisitor<string?>
{
  public string? Print(Expr? expr)
  {
    if (expr is null)
    {
      return string.Empty;
    }

    return expr.Accept(this);
  }

  public string? VisitBinaryExpr(Expr.Binary expr)
  {
    return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
  }

  public string? VisitGroupingExpr(Expr.Grouping expr)
  {
    return Parenthesize("group", expr.Xpression);
  }

  public string? VisitLiteralExpr(Expr.Literal expr)
  {
    if (expr.Value is null)
    {
      return "nil";
    }

    return expr.Value.ToString();
  }

  public string? VisitUnaryExpr(Expr.Unary expr)
  {
    return Parenthesize(expr.Operator.Lexeme, expr.Right);
  }

  private string Parenthesize(string name, params Expr[] exprs)
  {
    StringBuilder builder = new();

    builder.Append('(')
           .Append(name);

    foreach (Expr expr in exprs)
    {
      builder.Append(' ')
             .Append(expr.Accept(this));
    }

    builder.Append(')');

    return builder.ToString();
  }
}
