namespace Lox.Cli;

/// <summary>
/// See: https://www.freebsd.org/cgi/man.cgi?query=sysexits&apropos=0&sektion=0&manpath=FreeBSD+4.3-RELEASE&format=html
/// </summary>
public static class SysExits
{
  /// <summary>
  /// EX_USAGE (64) The command was used incorrectly, e.g., with the wrong
  ///               number of arguments, a bad flag, a bad
  ///               syntax in a parameter, or whatever.
  /// </summary>
  public const int EX_USAGE = 64;

  /// <summary>
  /// EX_DATAERR (65)	The input data was incorrect in some way. This
  ///                 should only be used for user's data and not system
  ///                 files.
  /// </summary>
  public const int EX_DATAERR = 65;
}
