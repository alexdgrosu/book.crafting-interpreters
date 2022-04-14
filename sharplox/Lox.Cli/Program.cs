using Lox.Cli;

// https://www.freebsd.org/cgi/man.cgi?query=sysexits&apropos=0&sektion=0&manpath=FreeBSD+4.3-RELEASE&format=html
//  EX_USAGE (64)    The command was used incorrectly, e.g., with the wrong number of arguments, a bad flag, a bad
//                   syntax in a parameter, or whatever.
const int EX_USAGE = 64;

CancellationTokenSource cancellationTokenSource = new();
Console.CancelKeyPress += (source, eventArgs) =>
{
  cancellationTokenSource.Cancel();
  eventArgs.Cancel = true;
};

if (args.Length > 1)
{
  Console.WriteLine("Usage: sharplox [script].lox");
  Environment.Exit(EX_USAGE);
}
else if (args.Length == 1)
{
  await Interpret.FromFile(args[0], cancellationTokenSource);
}
else
{
  Interpret.FromPrompt(cancellationTokenSource);
}
