using Lox.Interpreter.Ast;
using Lox.Interpreter.Core;
using Lox.Interpreter.Lexer;
using Lox.Interpreter.Syntax;
using Lox.Interpreter.Util;

namespace Lox.Cli;

public static class Interpret
{
  private static readonly IReporter _reporter = new ConsoleReporter();
  private static readonly Interpreter.Interpreter _interpreter = new(_reporter);

  public static async Task FromFile(string path, CancellationToken token)
  {
    if (string.IsNullOrEmpty(path))
    {
      throw new ArgumentNullException(nameof(path));
    }

    Run(await File.ReadAllTextAsync(path, token));

    if (_reporter.HadError)
    {
      Environment.Exit(SysExits.EX_DATAERR);
    }

    if (_reporter.HadRuntimeError)
    {
      Environment.Exit(SysExits.EX_SOFTWARE);
    }
  }

  public static void FromPrompt(CancellationToken token)
  {
    while (!token.IsCancellationRequested)
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
    Parser parser = new(scanner.ScanTokens(),
                        _reporter);
    Expr? expression = parser.Parse();

    if (_reporter.HadError)
    {
      return;
    }

    _interpreter.Interpret(expression);
  }
}
