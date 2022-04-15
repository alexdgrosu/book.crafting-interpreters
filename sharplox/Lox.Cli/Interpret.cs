using Lox.Interpreter;

namespace Lox.Cli;

public static class Interpret
{
  private static bool _hadError;

  public static async Task FromFile(string path, CancellationTokenSource cancellationTokenSource)
  {
    if (string.IsNullOrEmpty(path))
    {
      throw new ArgumentNullException(nameof(path));
    }

    Run(await File.ReadAllTextAsync(path, cancellationTokenSource.Token));

    if (_hadError)
    {
      Environment.Exit(SysExits.EX_DATAERR);
    }
  }

  public static void FromPrompt(CancellationTokenSource cancellationTokenSource)
  {
    while (!cancellationTokenSource.IsCancellationRequested)
    {
      Console.Write("> ");
      string? line = Console.ReadLine();

      if (string.IsNullOrEmpty(line))
      {
        break;
      }

      Run(line);
      _hadError = false;
    }
  }

  public static void Error(long line, string message)
  {
    Report(line, where: string.Empty, message);
  }

  private static void Run(string source)
  {
    Scanner scanner = new(source);
    var tokens = scanner.ScanTokens();

    foreach (Token token in tokens)
    {
      Console.WriteLine(token);
    }
  }

  // TODO §4.1.1:
  //  Ideally, we would have an actual abstraction, some kind of
  //  “ErrorReporter” interface that gets passed to the scanner and parser so
  //  that we can swap out different reporting strategies.
  private static void Report(long line, string where, string message)
  {
    Console.Error.WriteLine($"[line {line}] Error{where}: {message}");

    _hadError = true;
  }
}
