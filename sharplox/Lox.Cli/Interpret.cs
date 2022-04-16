using Lox.Interpreter;

namespace Lox.Cli;

public static class Interpret
{
  private static readonly IReporter _reporter = new ConsoleReporter();

  public static async Task FromFile(string path, CancellationTokenSource cancellationTokenSource)
  {
    if (string.IsNullOrEmpty(path))
    {
      throw new ArgumentNullException(nameof(path));
    }

    Run(await File.ReadAllTextAsync(path, cancellationTokenSource.Token));

    if (_reporter.HadError)
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
      _reporter.HadError = false;
    }
  }

  private static void Run(string source)
  {
    Scanner scanner = new(source, _reporter);
    var tokens = scanner.ScanTokens();

    foreach (Token token in tokens)
    {
      Console.WriteLine(token);
    }
  }
}
