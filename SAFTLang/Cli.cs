using System.Diagnostics;

namespace SAFTLang;

public static class Cli
{
    public static int Build(string sourcePath)
    {
        if (!ValidateFile(sourcePath))
        {
            return 1;
        }

        string? cCode =
            CompilerDriver.CompileFile(sourcePath);

        if (cCode is null)
        {
            return 1;
        }

        string fullSourcePath =
            Path.GetFullPath(sourcePath);

        string directory =
            Path.GetDirectoryName(fullSourcePath)!;

        string outputName =
            Path.GetFileNameWithoutExtension(fullSourcePath);

        string outputPath =
            Path.Combine(directory, outputName);

        bool success = CompileC(
            cCode,
            outputPath
        );

        if (!success)
        {
            return 1;
        }

        Console.WriteLine(
            $"Built: {outputPath}"
        );

        return 0;
    }

    public static int Run(string sourcePath)
    {
        if (!ValidateFile(sourcePath))
        {
            return 1;
        }

        string? cCode =
            CompilerDriver.CompileFile(sourcePath);

        if (cCode is null)
        {
            return 1;
        }

        string tempDirectory =
            Path.Combine(
                Path.GetTempPath(),
                $"saft-{Guid.NewGuid():N}"
            );

        Directory.CreateDirectory(tempDirectory);

        string executable =
            Path.Combine(tempDirectory, "program");

        try
        {
            if (!CompileC(cCode, executable))
            {
                return 1;
            }

            return Execute(executable);
        }
        finally
        {
            try
            {
                Directory.Delete(
                    tempDirectory,
                    recursive: true
                );
            }
            catch
            {
                // Cleanup failure should not
                // crash the compiler.
            }
        }
    }

    private static bool CompileC(
        string cCode,
        string outputPath)
    {
        string tempCFile =
            Path.Combine(
                Path.GetTempPath(),
                $"saft-{Guid.NewGuid():N}.c"
            );

        try
        {
            File.WriteAllText(
                tempCFile,
                cCode
            );

            var startInfo =
                new ProcessStartInfo
                {
                    FileName = "cc",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

            startInfo.ArgumentList.Add("-std=c11");
            startInfo.ArgumentList.Add("-Wall");
            startInfo.ArgumentList.Add("-Wextra");

            startInfo.ArgumentList.Add(
                tempCFile
            );

            startInfo.ArgumentList.Add("-o");

            startInfo.ArgumentList.Add(
                outputPath
            );

            Process? process;

            try
            {
                process = Process.Start(startInfo);
            }
            catch
            {
                Console.Error.WriteLine(
                    "Could not start C compiler 'cc'."
                );

                Console.Error.WriteLine(
                    "Make sure GCC or Clang is installed."
                );

                return false;
            }

            if (process is null)
            {
                Console.Error.WriteLine(
                    "Failed to start C compiler."
                );

                return false;
            }

            string stdout =
                process.StandardOutput.ReadToEnd();

            string stderr =
                process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(stdout))
            {
                Console.Write(stdout);
            }

            if (!string.IsNullOrWhiteSpace(stderr))
            {
                Console.Error.Write(stderr);
            }

            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine(
                    "Native compilation failed."
                );

                return false;
            }

            return true;
        }
        finally
        {
            if (File.Exists(tempCFile))
            {
                File.Delete(tempCFile);
            }
        }
    }

    private static int Execute(
        string executable)
    {
        var startInfo =
            new ProcessStartInfo
            {
                FileName = executable,
                UseShellExecute = false
            };

        Process? process =
            Process.Start(startInfo);

        if (process is null)
        {
            Console.Error.WriteLine(
                "Could not start program."
            );

            return 1;
        }

        process.WaitForExit();

        return process.ExitCode;
    }

    private static bool ValidateFile(
        string sourcePath)
    {
        if (!File.Exists(sourcePath))
        {
            Console.Error.WriteLine(
                $"File not found: {sourcePath}"
            );

            return false;
        }

        if (Path.GetExtension(sourcePath)
            .Equals(
                ".sft",
                StringComparison.OrdinalIgnoreCase
            ) == false)
        {
            Console.Error.WriteLine(
                "SAFTLang source files must use .sft"
            );

            return false;
        }

        return true;
    }
}