using Lox.Interpreter.Ast;
using Lox.Interpreter.Core;
using Lox.Interpreter.Lexer;
using Lox.Interpreter.Syntax;
using Lox.Interpreter.Util;

namespace Lox.Cli;

public static class Interpret
{
  private static readonly IReporter _reporter = new ConsoleReporter();

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
    IList<Token> tokens = scanner.ScanTokens();
    Parser parser = new(tokens, _reporter);
    Expr? expression = parser.Parse();


    if (_reporter.HadError)
    {
      return;
    }

    Console.WriteLine(new AstPrinter().Print(expression));
  }
}
