using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CmdLine;

namespace CleanProject
{
    /// <summary>
    ///     Program to clean solutions
    /// </summary>
    /// <remarks>
    ///     Setup Visual Studio with an external tool
    ///     Title: Clean, Remove Source Bindings and Zip Solution
    ///     Command: CleanProject.exe
    ///     Arguments: /D:$(SolutionDir) /Z /R
    ///     Use Output Window
    /// </remarks>
    internal class Program
    {
        /// <summary>
        ///     The options.
        /// </summary>
        internal static CleanOptions Options;

        /// <summary>
        ///     The win string builder.
        /// </summary>
        private static readonly StringBuilder WinStringBuilder = new StringBuilder();

        /// <summary>
        ///     The console window.
        /// </summary>
        private static IntPtr consoleWindow;

        /// <summary>
        ///     The win text out.
        /// </summary>
        private static TextWriter winTextOut;

        /// <summary>
        ///     The write verbose message.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        internal static void WriteVerboseMessage(string format, params object[] args)
        {
            if (Options.Verbose)
            {
                Console.WriteLine(format, args);
            }
        }

        /// <summary>
        ///     The confirm options.
        /// </summary>
        private static void ConfirmOptions()
        {
            if (Options.ZipProject)
            {
                Options.QuietMode = true;
                WriteVerboseMessage("Will copy to a temporary directory, clean and zip the project");
            }

            if (Options.RemoveSourceControl && Options.Verbose)
            {
                WriteVerboseMessage("Will remove source control bindings from projects");
            }

            if (!Options.QuietMode)
            {
                Console.WriteLine("Will clean the following directories");
                foreach (var directory in Options.Directories)
                {
                    CommandLine.WriteLineColor(ConsoleColor.Yellow, directory);
                }

                if (Options.WindowsMode)
                {
                    var sb = new StringBuilder(WinStringBuilder.ToString());
                    sb.AppendLine();
                    sb.Append("This will delete files, do you want to continue?");
                    if (MessageBox.Show(
                        sb.ToString(),
                        CleanOptions.Title,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2,
                        (MessageBoxOptions) 0x40000) == DialogResult.No)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        WinStringBuilder.Clear();
                    }
                }
                else
                {
                    if (CommandLine.PromptKey("This will delete files, do you want to continue?", 'y', 'n') == 'n')
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }

        /// <summary>
        ///     The copy solution to temp dir.
        /// </summary>
        /// <param name="directory">
        ///     The directory.
        /// </param>
        /// <returns>
        ///     A solution info
        /// </returns>
        private static SolutionInfo CopySolutionToTempDir(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new ApplicationException($"Directory \"{directory}\" does not exist");
            }

            if (!Options.WindowsMode)
            {
                Console.WriteLine("Copying solution {0} to temporary directory", directory);
            }

            var solutionInfo = new SolutionInfo {Directory = GetLongDirectoryName(directory)};

            directory.CopyDirectory(solutionInfo.TempPath, true, true);

            return solutionInfo;
        }

        /// <summary>
        ///     The enable windows mode.
        /// </summary>
        private static void EnableWindowsMode()
        {
            consoleWindow = NativeMethods.FindWindow(null, CleanOptions.Title);
            if (consoleWindow != IntPtr.Zero)
            {
                // Hide the console Window
                NativeMethods.ShowWindow(consoleWindow, 0);
            }

            winTextOut = new StringWriter(WinStringBuilder);
            Console.SetOut(winTextOut);
        }

        /// <summary>
        ///     The get directories.
        /// </summary>
        /// <returns>
        /// </returns>
        private static List<SolutionInfo> GetDirectories()
        {
            return Options.Directories.Select(directory => new SolutionInfo {Directory = GetLongDirectoryName(directory)}).ToList();
        }

        private static string GetLongDirectoryName(string directory)
        {
            var sb = new StringBuilder(255);
            NativeMethods.GetLongPathName(directory, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        ///     The get temp directories.
        /// </summary>
        /// <returns>
        /// </returns>
        private static List<SolutionInfo> GetTempDirectories()
        {
            return Options.Directories.Select(CopySolutionToTempDir).ToList();
        }

        /// <summary>
        ///     The main.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ParseCommandLine();
            Console.Title = CleanOptions.Title;

            if (Options.WindowsMode)
            {
                EnableWindowsMode();
            }

            Console.WriteLine("{0} {1} - {2}", CleanOptions.Title, Assembly.GetEntryAssembly().GetName().Version.ToString(3), CleanOptions.Description);

            if (Options.Directories.Count == 0)
            {
                Options.Directories.Add(Directory.GetCurrentDirectory());
            }

            ConfirmOptions();

            try
            {
                var directories = Options.ZipProject ? GetTempDirectories() : GetDirectories();

                foreach (var solutionInfo in directories)
                {
                    Console.WriteLine("Cleaning Solution Directory {0}", solutionInfo.WorkingPath);
                }

                directories.CleanDirectories();

                if (Options.ZipProject)
                {
                    directories.ZipDirectories();
                }
            }
            catch (ApplicationException exception)
            {
                if (Options.WindowsMode)
                {
                    MessageBox.Show(
                        exception.Message,
                        CleanOptions.Title,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        (MessageBoxOptions) 0x40000);
                }
                else
                {
                    CommandLine.WriteLineColor(ConsoleColor.Red, "{0}", exception.Message);
                }

                Environment.Exit(1);
            }

            ShowCleaningComplete();
        }

        /// <summary>
        ///     The parse command line.
        /// </summary>
        private static void ParseCommandLine()
        {
            try
            {
                Options = CommandLine.Parse<CleanOptions>();
            }
            catch (CommandLineHelpException exception)
            {
                Console.WriteLine(exception.ArgumentHelp.GetHelpText(Console.BufferWidth));
                Environment.Exit(1);
            }
            catch (CommandLineArgumentInvalidException exception)
            {
                CommandLine.WriteLineColor(ConsoleColor.Red, exception.ArgumentHelp.Message);
                CommandLine.WriteLineColor(ConsoleColor.Cyan, exception.ArgumentHelp.GetHelpText(Console.BufferWidth));

                CommandLine.Pause();
                Environment.Exit(1);
            }
            catch (Exception exception)
            {
                CommandLine.WriteLineColor(ConsoleColor.Red, exception.Message);
                CommandLine.Pause();
                Environment.Exit(1);
            }
        }

        /// <summary>
        ///     The show cleaning complete.
        /// </summary>
        private static void ShowCleaningComplete()
        {
            Console.WriteLine("Cleaning complete");

            if (Options.WindowsMode)
            {
                MessageBox.Show(
                    WinStringBuilder.ToString(), CleanOptions.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (!Options.QuietMode)
                {
                    CommandLine.Pause();
                }
            }
        }
    }
}