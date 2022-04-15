using static Lox.Interpreter.TokenType;

namespace Lox.Interpreter;

public class Scanner
{
  private readonly string _source;
  private readonly ICollection<Token> _tokens = new List<Token>();
  private long _start = 0;
  private long _current = 0;
  private long _line = 1;

  public Scanner(string source)
  {
    _source = source;
  }

  public ICollection<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      _start = _current;
      ScanToken();
    }

    _tokens.Add(new(EOF, string.Empty, default, _line));

    return _tokens;

    bool IsAtEnd() => _current >= _source.Length;
  }

  private void ScanToken()
  {
    throw new NotImplementedException();
  }
}
