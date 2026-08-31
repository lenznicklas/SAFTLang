namespace SAFTLang;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            PrintHelp();
            return 1;
        }

        string command = args[0];
        string sourceFile = args[1];

        return command switch
        {
            "build" => Cli.Build(sourceFile),
            "run" => Cli.Run(sourceFile),

            _ => UnknownCommand(command)
        };
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine(
            $"Unknown command '{command}'"
        );

        PrintHelp();

        return 1;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("""
                          SAFTLang Compiler

                          Usage:
                            saft build <file.sft>
                            saft run   <file.sft>
                          """);
    }
}