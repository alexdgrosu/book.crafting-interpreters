using System.Collections.Immutable;

using static Lox.Interpreter.Lexer.TokenType;

namespace Lox.Interpreter.Lexer;

public static class Keyword
{
  public static IImmutableDictionary<string, TokenType> Keywords =>
    new Dictionary<string, TokenType>()
    {
      { "and"     , AND     },
      { "class"   , CLASS   },
      { "else"    , ELSE    },
      { "false"   , FALSE   },
      { "for"     , FOR     },
      { "fun"     , FUN     },
      { "if"      , IF      },
      { "nil"     , NIL     },
      { "or"      , OR      },
      { "print"   , PRINT   },
      { "return"  , RETURN  },
      { "super"   , SUPER   },
      { "this"    , THIS    },
      { "true"    , TRUE    },
      { "var"     , VAR     },
      { "while"   , WHILE   }
    }.ToImmutableDictionary();
}
