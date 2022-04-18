using Lox.Cli;

CancellationTokenSource cancellationTokenSource = new();
Console.CancelKeyPress += (source, eventArgs) =>
{
  cancellationTokenSource.Cancel();
  eventArgs.Cancel = true;
};

if (args.Length > 1)
{
  Console.WriteLine("Usage: sharplox [script].lox");
  Environment.Exit(SysExits.EX_USAGE);
}
else if (args.Length == 1)
{
  await Interpret.FromFile(args[0], cancellationTokenSource.Token);
}
else
{
  Interpret.FromPrompt(cancellationTokenSource.Token);
}
