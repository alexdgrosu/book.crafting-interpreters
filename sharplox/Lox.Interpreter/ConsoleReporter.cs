namespace Lox.Interpreter;

public class ConsoleReporter : IReporter
{
  public bool HadError { get; set; } = false;

  public void Error(long line, string message)
  {
    Report(line, where: string.Empty, message);
  }

  private void Report(long line, string where, string message)
  {
    Console.Error.WriteLine($"[line {line}] Error{where}: {message}");

    HadError = true;
  }
}
