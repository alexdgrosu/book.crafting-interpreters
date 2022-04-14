namespace Lox.Interpreter;

public class Scanner
{
  private readonly string _source;

  public Scanner(string source)
  {
    Console.WriteLine($"Scanner:ctor(source=\n{source}\n)");
    _source = source;
  }

  public ICollection<Token> ScanTokens()
  {
    Console.WriteLine($"Scanner:ScanTokens() -> would scan {_source.Length} characters");
    return new List<Token>();
  }
}
