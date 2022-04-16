namespace Lox.Interpreter;

public interface IReporter
{
  bool HadError { get; set; }
  void Error(long line, string message);
}
