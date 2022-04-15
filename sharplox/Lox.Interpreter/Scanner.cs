using static Lox.Interpreter.TokenType;

namespace Lox.Interpreter;

public class Scanner
{
  private readonly string _source;
  private readonly ICollection<Token> _tokens = new List<Token>();
  private int _start = 0;
  private int _current = 0;
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

    bool IsAtEnd()
    {
      return _current >= _source.Length;
    }
  }

  private void ScanToken()
  {
    char character = Advance();
    switch (character)
    {
      case '(': AddToken(LEFT_PAREN); break;
      case ')': AddToken(RIGHT_PAREN); break;
      case '{': AddToken(LEFT_BRACE); break;
      case '}': AddToken(RIGHT_BRACE); break;
      case ',': AddToken(COMMA); break;
      case '.': AddToken(DOT); break;
      case '-': AddToken(MINUS); break;
      case '+': AddToken(PLUS); break;
      case ';': AddToken(SEMICOLON); break;
      case '*': AddToken(STAR); break;
    }

    char Advance()
    {
      return _source[_current++];
    }

    void AddToken(TokenType type)
    {
      this.AddToken(type, default);
    }
  }

  private void AddToken(TokenType type, object? literal)
  {
    int length = _current - _start;
    string text = _source.Substring(_start, length);
    _tokens.Add(new(type, text, literal, _line));
  }
}
