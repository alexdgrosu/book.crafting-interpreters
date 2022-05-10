using Lox.Interpreter.Ast;

using static Lox.Interpreter.Lexer.TokenType;

namespace Lox.Interpreter;

public class Interpreter : Expr.IVisitor<object>
{
  public object VisitBinaryExpr(Expr.Binary expr)
  {
    throw new NotImplementedException();
  }

  public object VisitGroupingExpr(Expr.Grouping expr)
  {
    return Evaluate(expr.Expression);
  }

  public object VisitLiteralExpr(Expr.Literal expr)
  {
    return expr.Value;
  }

  public object VisitUnaryExpr(Expr.Unary expr)
  {
    object right = expr.Right;

    return expr.Operator.Type switch
    {
      MINUS => -(double)right,
      BANG => IsTruthy(right),

      // Unreachable
      _ => null!
    };
  }

  private static bool IsTruthy(object obj)
  {
    return obj switch
    {
      null => false,
      bool boolean => boolean,
      _ => true
    };
  }

  private object Evaluate(Expr expr)
  {
    return expr.Accept(this);
  }
}
