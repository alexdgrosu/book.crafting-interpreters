using Lox.Interpreter.Ast;
using Lox.Interpreter.Core;
using Lox.Interpreter.Lexer;

using static Lox.Interpreter.Lexer.TokenType;

namespace Lox.Interpreter.Syntax;

public class Parser
{
  private readonly IList<Token> _tokens;
  private readonly IReporter _reporter;
  private int _currentToken = 0;

  public Parser(IList<Token> tokens, IReporter reporter)
  {
    _tokens = tokens;
    _reporter = reporter;
  }

  public Expr? Parse()
  {
    try
    {
      return Expression();
    }
    catch (ParseError)
    {
      return default;
    }
  }

  private Expr Expression()
  {
    return Equality();
  }

  private Expr Equality()
  {
    Expr expr = Comparison();

    while (Match(BANG_EQUAL, EQUAL_EQUAL))
    {
      Token @operator = Previous();
      Expr right = Comparison();
      expr = new Expr.Binary(expr, @operator, right);
    }

    return expr;
  }

  private Expr Comparison()
  {
    Expr expr = Term();

    while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
    {
      Token @operator = Previous();
      Expr right = Term();
      expr = new Expr.Binary(expr, @operator, right);
    }

    return expr;
  }

  private Expr Term()
  {
    Expr expr = Factor();

    while (Match(MINUS, PLUS))
    {
      Token @operator = Previous();
      Expr right = Factor();
      expr = new Expr.Binary(expr, @operator, right);
    }

    return expr;
  }

  private Expr Factor()
  {
    Expr expr = Unary();

    while (Match(SLASH, STAR))
    {
      Token @operator = Previous();
      Expr right = Unary();
      expr = new Expr.Binary(expr, @operator, right);
    }

    return expr;
  }

  private Expr Unary()
  {
    if (Match(BANG, MINUS))
    {
      Token @operator = Previous();
      Expr right = Unary();
      return new Expr.Unary(@operator, right);
    }

    return Primary();
  }

  private Expr Primary()
  {
    if (Match(FALSE))
    {
      return new Expr.Literal(false);
    }
    if (Match(TRUE))
    {
      return new Expr.Literal(true);
    }
    if (Match(NIL))
    {
      return new Expr.Literal(null);
    }

    if (Match(NUMBER, STRING))
    {
      return new Expr.Literal(Previous().Literal);
    }

    if (Match(LEFT_PAREN))
    {
      Expr expr = Expression();
      Consume(RIGHT_PAREN, "Expect ')' after expression.");
      return new Expr.Grouping(expr);
    }

    throw Error(Peek(), "Expect expression");
  }

  private Token Consume(TokenType type, string message)
  {
    if (Check(type))
    {
      return Advance();
    }

    throw Error(Peek(), message);
  }

  private LoxException Error(Token token, string message)
  {
    _reporter.Error(token, message);
    return new ParseError();
  }

  // §6.3.3: We don’t get to see this method in action, since we don’t have
  //         statements yet. For now, if an error occurs, we’ll panic and unwind
  //         all the way to the top and stop parsing. Since we can parse only a
  //         single expression anyway, that’s no big loss.
#pragma warning disable IDE0051
  private void Synchronize()
  {
    Advance();

    while (!IsAtEnd())
    {
      if (Previous().Type is SEMICOLON)
      {
        return;
      }

      switch (Peek().Type)
      {
        case CLASS:
        case FUN:
        case VAR:
        case FOR:
        case IF:
        case WHILE:
        case PRINT:
        case RETURN:
          return;
      }

      Advance();
    }
  }
#pragma warning restore IDE0051

  private bool Match(params TokenType[] types)
  {
    foreach (TokenType type in types)
    {
      if (Check(type))
      {
        Advance();
        return true;
      }
    }

    return false;
  }

  private bool Check(TokenType type)
  {
    if (IsAtEnd())
    {
      return false;
    }

    return Peek().Type == type;
  }

  private Token Peek()
  {
    return _tokens[_currentToken];
  }

  private Token Advance()
  {
    if (!IsAtEnd())
    {
      _currentToken++;
    }

    return Previous();
  }

  private Token Previous()
  {
    return _tokens[_currentToken - 1];
  }

  private bool IsAtEnd()
  {
    return Peek().Type == EOF;
  }
}
