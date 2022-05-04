using Lox.Interpreter.Lexer;

namespace Lox.Interpreter.Core;

public interface IReporter
{
  bool HadError { get; set; }
  void Error(long line, string message);
  void Error(Token token, string message);
}
