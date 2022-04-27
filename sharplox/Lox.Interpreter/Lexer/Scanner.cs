using static Lox.Interpreter.Lexer.TokenType;
using static Lox.Interpreter.Lexer.Keyword;

namespace Lox.Interpreter.Lexer;

public class Scanner
{
  private readonly IReporter _reporter;
  private readonly string _source;
  private readonly ICollection<Token> _tokens = new List<Token>();
  private int _tokenStartingCharacter = 0;
  private int _currentCharacter = 0;
  private long _line = 1;

  public Scanner(string source, IReporter reporter)
  {
    _source = source;
    _reporter = reporter;
  }

  public ICollection<Token> ScanTokens()
  {
    while (!IsAtSourceEnd())
    {
      _tokenStartingCharacter = _currentCharacter;
      ScanNextToken();
    }

    _tokens.Add(new(EOF, string.Empty, default, _line));
    return _tokens;
  }

  private bool IsAtSourceEnd()
  {
    return _currentCharacter >= _source.Length;
  }

  private void ScanNextToken()
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
        AddToken(MatchNext('=') ? BANG_EQUAL : BANG);
        break;
      case '=':
        AddToken(MatchNext('=') ? EQUAL_EQUAL : EQUAL);
        break;
      case '<':
        AddToken(MatchNext('=') ? LESS_EQUAL : LESS);
        break;
      case '>':
        AddToken(MatchNext('=') ? GREATER_EQUAL : GREATER);
        break;
      case '/':
        if (MatchNext('/'))
          while (Peek() != '\n' && !IsAtSourceEnd()) Advance();
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
        ScanString();
        break;

      default:
        // §4.6.2:  It’s kind of tedious to add cases for every decimal digit,
        //          so we’ll stuff it in the default case instead.
        if (IsDigit(character))
          ScanNumber();
        else if (IsAlpha(character))
          ScanIdentifier();
        else
          _reporter.Error(_line, $"Unexpected character '{character}'.");
        break;
    }

    void ScanString()
    {
      while (Peek() != '"' && !IsAtSourceEnd())
      {
        if (Peek() == '\n')
        {
          _line++;
        }

        Advance();
      }

      if (IsAtSourceEnd())
      {
        _reporter.Error(_line, "Unterminated string.");
        return;
      }

      // Skip past the closing ".
      Advance();

      // Trim surrounding quotes
      string value = _source[(_tokenStartingCharacter + 1)..(_currentCharacter - 1)];
      AddToken(STRING, value);
    }

    void ScanNumber()
    {
      AdvanceWhileIsDigit();

      // Look for fractional part.
      if (Peek() == '.' && IsDigit(PeekNext()))
      {
        // Consume the .
        Advance();
        AdvanceWhileIsDigit();
      }

      string number = _source[_tokenStartingCharacter.._currentCharacter];
      AddToken(NUMBER, double.Parse(number));

      void AdvanceWhileIsDigit()
      {
        while (IsDigit(Peek())) Advance();
      }
    }

    void ScanIdentifier()
    {
      while (IsAlphaNumberic(Peek()))
      {
        Advance();
      }

      string text = _source[_tokenStartingCharacter.._currentCharacter];
      if (!Keywords.TryGetValue(text, out TokenType type))
      {
        type = IDENTIFIER;
      }

      AddToken(type);
    }
  }

  private char Advance()
  {
    return _source[_currentCharacter++];
  }

  private void AddToken(TokenType type)
  {
    AddToken(type, default);
  }

  private void AddToken(TokenType type, object? literal)
  {
    string lexeme = _source[_tokenStartingCharacter.._currentCharacter];
    _tokens.Add(new(type, lexeme, literal, _line));
  }

  private bool MatchNext(char expected)
  {
    if (IsAtSourceEnd())
    {
      return false;
    }

    if (_source[_currentCharacter] != expected)
    {
      return false;
    }

    _currentCharacter++;
    return true;
  }

  private char Peek()
  {
    if (IsAtSourceEnd())
    {
      return '\0';
    }

    return _source[_currentCharacter];
  }

  private char PeekNext()
  {
    if (_currentCharacter + 1 >= _source.Length)
    {
      return '\0';
    }

    return _source[_currentCharacter + 1];
  }

  private static bool IsAlpha(char c)
  {
    return (c >= 'a' && c <= 'z') ||
           (c >= 'A' && c <= 'Z') ||
            c == '_';
  }

  private static bool IsDigit(char c)
  {
    return c >= '0' && c <= '9';
  }

  private static bool IsAlphaNumberic(char c)
  {
    return IsAlpha(c) || IsDigit(c);
  }
}
