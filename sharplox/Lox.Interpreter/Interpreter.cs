using Lox.Interpreter.Ast;
using Lox.Interpreter.Core;
using Lox.Interpreter.Lexer;

using static Lox.Interpreter.Lexer.TokenType;

namespace Lox.Interpreter;

public class Interpreter : Expr.IVisitor<object>
{
  private readonly IReporter _reporter;

  public Interpreter(IReporter reporter)
  {
    _reporter = reporter;
  }

  public void Interpret(Expr? expression)
  {
    try
    {
      object value = Evaluate(expression
                              ?? throw new ArgumentNullException(nameof(expression)));
      Console.WriteLine(Stringify(value));
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

        throw new RuntimeError(expr.Operator,
                               "Operands must be two numbers or two strings.");
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
