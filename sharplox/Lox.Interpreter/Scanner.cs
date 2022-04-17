using static Lox.Interpreter.TokenType;

namespace Lox.Interpreter;

public class Scanner
{
  private readonly string _source;
  private readonly IReporter _reporter;
  private readonly ICollection<Token> _tokens = new List<Token>();
  private int _start = 0;
  private int _current = 0;
  private long _line = 1;

  public Scanner(string source, IReporter reporter)
  {
    _source = source;
    _reporter = reporter;
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
  }

  private bool IsAtEnd()
  {
    return _current >= _source.Length;
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

      case '!':
        AddToken(Match('=') ? BANG_EQUAL : BANG);
        break;
      case '=':
        AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
        break;
      case '<':
        AddToken(Match('=') ? LESS_EQUAL : LESS);
        break;
      case '>':
        AddToken(Match('=') ? GREATER_EQUAL : GRATER);
        break;
      case '/':
        if (Match('/'))
          while (Peek() != '\n' && !IsAtEnd())
            Advance();
        else
          AddToken(SLASH);
        break;

      case ' ':
      case '\r':
      case '\t':
        // Ignore whitespace
        break;

      case '\n':
        _line++;
        break;

      case '"':
        String();
        break;

      default:
        // §4.6.2:  It’s kind of tedious to add cases for every decimal digit,
        //          so we’ll stuff it in the default case instead.
        if (IsDigit(character))
          Number();
        else
          _reporter.Error(_line, $"Unexpected character '{character}'.");
        break;
    }

    char Advance()
    {
      return _source[_current++];
    }

    char Peek()
    {
      if (IsAtEnd())
      {
        return '\0';
      }

      return _source[_current];
    }

    // TODO §4.6.1: Maybe extract this?
    void AddToken(TokenType type)
    {
      this.AddToken(type, default);
    }

    bool Match(char expected)
    {
      if (IsAtEnd())
      {
        return false;
      }

      if (_source[_current] != expected)
      {
        return false;
      }

      _current++;
      return true;
    }

    void String()
    {
      while (Peek() != '"' && !IsAtEnd())
      {
        if (Peek() == '\n')
        {
          _line++;
        }

        Advance();
      }

      if (IsAtEnd())
      {
        _reporter.Error(_line, "Unterminated string.");
        return;
      }

      // Skip past the closing ".
      Advance();

      // Trim surrounding quotes
      // TODO §4.6.1: Implement .Substring (start, end) overload
      int length = _current - _start;
      string value = _source.Substring(_start + 1, length - 2);
      this.AddToken(STRING, value);
    }

    bool IsDigit(char c)
    {
      return c >= '0' && c <= '9';
    }

    void Number()
    {
      while (IsDigit(Peek()))
      {
        Advance();
      }

      // Look for fractional part.
      if (Peek() == '.' && IsDigit(PeekNext()))
      {
        // Consume the .
        Advance();

        while (IsDigit(Peek()))
        {
          Advance();
        }
      }

      int length = _current - _start;
      string number = _source.Substring(_start, length);
      this.AddToken(NUMBER, double.Parse(number));
    }

    char PeekNext()
    {
      if (_current + 1 >= _source.Length)
      {
        return '\0';
      }

      return _source[_current + 1];
    }
  }

  private void AddToken(TokenType type, object? literal)
  {
    int length = _current - _start;
    string text = _source.Substring(_start, length);
    _tokens.Add(new(type, text, literal, _line));
  }
}
