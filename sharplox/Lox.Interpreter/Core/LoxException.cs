using Lox.Interpreter.Lexer;

namespace Lox.Interpreter.Core;

public class LoxException : Exception
{
  public LoxException() : this(default) { }

  public LoxException(string? message) : base(message) { }
}

public class ParseError : LoxException
{
  public ParseError() : this(default) { }
  public ParseError(string? message) : base(message) { }
}

public class RuntimeError : LoxException
{
  public Token Token { get; }

  public RuntimeError(Token token, string message) : base(message)
  {
    Token = token;
  }
}
