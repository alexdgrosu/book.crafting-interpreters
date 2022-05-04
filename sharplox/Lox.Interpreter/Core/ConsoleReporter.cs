using Lox.Interpreter.Lexer;

namespace Lox.Interpreter.Core;

public class ConsoleReporter : IReporter
{
  public bool HadError { get; set; } = false;

  public void Error(long line, string message)
  {
    Report(line, where: string.Empty, message);
  }

  public void Error(Token token, string message)
  {
    if (token.Type is TokenType.EOF)
    {
      Report(token.Line, " at end", message);
    }
    else
    {
      Report(token.Line, $" at '{token.Lexeme}'", message);
    }
  }

  private void Report(long line, string where, string message)
  {
    Console.Error.WriteLine($"[line {line}] Error{where}: {message}");

    HadError = true;
  }
}
