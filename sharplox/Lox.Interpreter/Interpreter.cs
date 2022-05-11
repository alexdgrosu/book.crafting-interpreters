using Lox.Interpreter.Ast;

using static Lox.Interpreter.Lexer.TokenType;

namespace Lox.Interpreter;

public class Interpreter : Expr.IVisitor<object>
{
  public object VisitBinaryExpr(Expr.Binary expr)
  {
    object left = expr.Left;
    object right = expr.Right;

    switch (expr.Operator.Type)
    {
      case BANG_EQUAL:
        return !IsEqual(left, right);
      case EQUAL_EQUAL:
        return IsEqual(left, right);
      case GREATER:
        return (double)left > (double)right;
      case GREATER_EQUAL:
        return (double)left >= (double)right;
      case LESS:
        return (double)left < (double)right;
      case LESS_EQUAL:
        return (double)left <= (double)right;
      case MINUS:
        return (double)left - (double)right;
      case SLASH:
        return (double)left / (double)right;
      case STAR:
        return (double)left * (double)right;
      case PLUS:
        if (left is double leftNumber && right is double rightNumber)
        {
          return leftNumber + rightNumber;
        }

        if (left is string leftString && right is string rightString)
        {
          return leftString + rightString;
        }

        break;
    }

    // Unreachable
    return null!;
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

  private static bool IsEqual(object left, object right)
  {
    if (left is null && right is null)
    {
      return true;
    }

    if (left is null)
    {
      return false;
    }

    return left.Equals(right);
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
