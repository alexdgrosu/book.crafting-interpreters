namespace Lox.Interpreter.Lexer;

public class Token
{
  public Token(TokenType type, string lexeme, object? literal, long line)
  {
    Type = type;
    Lexeme = lexeme;
    Literal = literal;
    Line = line;
  }

  public TokenType Type { get; }
  public string Lexeme { get; }
  public object? Literal { get; }
  public long Line { get; }

  public override string ToString()
  {
    return $"[token at line {Line}] {Type} {Lexeme} {Literal}";
  }
}
