using Lox.Interpreter.Ast;
using Lox.Interpreter.Core;
using Lox.Interpreter.Lexer;

using static Lox.Interpreter.Lexer.TokenType;

namespace Lox.Interpreter;

public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
{
  private readonly IReporter _reporter;

  public Interpreter(IReporter reporter)
  {
    _reporter = reporter;
  }

  public void Interpret(IList<Stmt> statements)
  {
    try
    {
      foreach (var stmt in statements)
      {
        Execute(stmt);
      }
    }
    catch (RuntimeError err)
    {
      _reporter.RuntimeError(err);
    }
  }

  public object VisitBinaryExpr(Expr.Binary expr)
  {
    object left = Evaluate(expr.Left);
    object right = Evaluate(expr.Right);

    switch (expr.Operator.Type)
    {
      case BANG_EQUAL:
        return !IsEqual(left, right);
      case EQUAL_EQUAL:
        return IsEqual(left, right);
      case GREATER:
        CheckNumberOperands(expr.Operator, left, right);
        return (double)left > (double)right;
      case GREATER_EQUAL:
        CheckNumberOperands(expr.Operator, left, right);
        return (double)left >= (double)right;
      case LESS:
        CheckNumberOperands(expr.Operator, left, right);
        return (double)left < (double)right;
      case LESS_EQUAL:
        CheckNumberOperands(expr.Operator, left, right);
        return (double)left <= (double)right;
      case MINUS:
        CheckNumberOperands(expr.Operator, left, right);
        return (double)left - (double)right;
      case SLASH:
        CheckNumberOperands(expr.Operator, left, right);

        if (right is double number && number == 0.0d)
        {
          throw new RuntimeError(expr.Operator, "Cannot divide by zero.");
        }

        return (double)left / (double)right;
      case STAR:
        CheckNumberOperands(expr.Operator, left, right);
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

        if ((left is string && right is double)
            || (right is string && left is double))
        {
          return string.Concat(left, right);
        }

        throw new RuntimeError(expr.Operator,
                               "Operands must be two numbers or two strings.");
    }

    // Unreachable
    return null!;
  }

  public object VisitGroupingExpr(Expr.Grouping expr)
  {
    return Evaluate(expr.Xpression);
  }

  public object VisitLiteralExpr(Expr.Literal expr)
  {
    return expr.Value;
  }

  public object VisitUnaryExpr(Expr.Unary expr)
  {
    object right = expr.Right;

    switch (expr.Operator.Type)
    {
      case MINUS:
        CheckNumberOperand(expr.Operator, right);
        return -(double)right;
      case BANG:
        return IsTruthy(right);
    }

    // Unreachable
    return null!;
  }

  object Stmt.IVisitor<object>.VisitExpressionStmt(Stmt.Expression stmt)
  {
    Evaluate(stmt.Xpression);
    return null!;
  }

  object Stmt.IVisitor<object>.VisitPrintStmt(Stmt.Print stmt)
  {
    object value = Evaluate(stmt.Xpression);
    Console.WriteLine(Stringify(value));
    return null!;
  }

  private void Execute(Stmt stmt)
  {
    stmt.Accept(this);
  }

  private static string? Stringify(object value)
  {
    return value switch
    {
      null => "nil",
      double number => number.ToString("G29"),
      _ => value.ToString()
    };
  }

  private static void CheckNumberOperands(Token @operator, object left, object right)
  {
    if (left is not double && right is not double)
    {
      throw new RuntimeError(@operator, "Operands must be numbers.");
    }
  }

  private static void CheckNumberOperand(Token @operator, object operand)
  {
    if (operand is not double)
    {
      throw new RuntimeError(@operator, "Operand must be a number.");
    }
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
