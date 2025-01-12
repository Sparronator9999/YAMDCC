// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2023-2025.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace YAMDCC.Logs;

/// <summary>
/// A simple logger class for writing logs to
/// the console or a configurable file path.
/// </summary>
public sealed class Logger : IDisposable
{
    /// <summary>
    /// The <see cref="StreamWriter"/> to write log files to.
    /// </summary>
    private StreamWriter LogWriter;

    /// <summary>
    /// Used with <see langword="lock"/> to prevent more
    /// than one thread writing to the console at once.
    /// </summary>
    private readonly object conLock = new();

    /// <summary>
    /// The newline characters to split provided log message lines by.
    /// </summary>
    private static readonly char[] NewLineChars = ['\r', '\n'];

    private static string LogString(string text, LogLevel level, bool date)
    {
        return (date ? $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}] " : "") + $"[{level}]".PadRight(8).ToUpperInvariant() + text;
    }

    /// <summary>
    /// The directory in which log files are saved.
    /// </summary>
    public string LogDir { get; set; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

    /// <summary>
    /// The base name of the log file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Log files will have the <c>.log</c> extension appended.
    /// </para>
    /// <para>
    /// Archives will have a number appended before the <c>.log</c>
    /// extension, with higher numbers indicating older logs.
    /// </para>
    /// </remarks>
    public string LogName { get; set; } = Path.GetFileName(Assembly.GetEntryAssembly().Location);

    private string LogPath => Path.Combine(LogDir, LogName);

    /// <summary>
    /// The maximum number of logs to archive.
    /// </summary>
    public int MaxArchivedLogs { get; set; } = 9;

    /// <summary>
    /// How verbose should console logs be?
    /// </summary>
    public LogLevel ConsoleLogLevel { get; set; } = LogLevel.Info;

    /// <summary>
    /// How verbose should logs written to disk be?
    /// </summary>
    public LogLevel FileLogLevel { get; set; } = LogLevel.Info;

    /// <summary>
    /// Should the log time be shown in console logs?
    /// </summary>
    public bool LogTimeToConsole { get; set; }

    /// <summary>
    /// Should the log time be shown in logs written to disk?
    /// </summary>
    public bool LogTimeToFile { get; set; } = true;

    /// <summary>
    /// Writes a Debug event to the <see cref="Logger"/>.
    /// </summary>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    public void Debug(string msg)
    {
        if (FileLogLevel >= LogLevel.Debug)
        {
            LogFile(msg, LogLevel.Debug);
        }

        if (ConsoleLogLevel >= LogLevel.Debug)
        {
            LogConsole(msg, LogLevel.Debug);
        }
    }

    /// <summary>
    /// Writes a Debug event to the <see cref="Logger"/>,
    /// replacing format items with the objects in <paramref name="args"/>.
    /// </summary>
    /// <remarks>
    /// Equivalent to passing a <see cref="string.Format(string, object[])"/>
    /// to the <paramref name="msg"/> argument.
    /// </remarks>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    /// <param name="args">
    /// The objects to format.
    /// </param>
    public void Debug(string msg, params object[] args)
    {
        Debug(string.Format(CultureInfo.InvariantCulture, msg, args));
    }

    /// <summary>
    /// Writes an Info event to the <see cref="Logger"/>.
    /// </summary>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    public void Info(string msg)
    {
        if (FileLogLevel >= LogLevel.Info)
        {
            LogFile(msg, LogLevel.Info);
        }

        if (ConsoleLogLevel >= LogLevel.Info)
        {
            LogConsole(msg, LogLevel.Info);
        }
    }

    /// <summary>
    /// Writes an Info event to the <see cref="Logger"/>,
    /// replacing format items with the objects in <paramref name="args"/>.
    /// </summary>
    /// <remarks>
    /// Equivalent to passing a <see cref="string.Format(string, object[])"/>
    /// to the <paramref name="msg"/> argument.
    /// </remarks>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    /// <param name="args">
    /// The objects to format.
    /// </param>
    public void Info(string msg, params object[] args)
    {
        Info(string.Format(CultureInfo.InvariantCulture, msg, args));
    }

    /// <summary>
    /// Writes a Warning to the <see cref="Logger"/>.
    /// </summary>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    public void Warn(string msg)
    {
        if (FileLogLevel >= LogLevel.Warn)
        {
            LogFile(msg, LogLevel.Warn);
        }

        if (ConsoleLogLevel >= LogLevel.Warn)
        {
            LogConsole(msg, LogLevel.Warn);
        }
    }

    /// <summary>
    /// Writes a Warning to the <see cref="Logger"/>,
    /// replacing format items with the objects in <paramref name="args"/>.
    /// </summary>
    /// <remarks>
    /// Equivalent to passing a <see cref="string.Format(string, object[])"/>
    /// to the <paramref name="msg"/> argument.
    /// </remarks>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    /// <param name="args">
    /// The objects to format.
    /// </param>
    public void Warn(string msg, params object[] args)
    {
        Warn(string.Format(CultureInfo.InvariantCulture, msg, args));
    }

    /// <summary>
    /// Writes an Error to the <see cref="Logger"/>.
    /// </summary>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    public void Error(string msg)
    {
        if (FileLogLevel >= LogLevel.Error)
        {
            LogFile(msg, LogLevel.Error);
        }

        if (ConsoleLogLevel >= LogLevel.Error)
        {
            LogConsole(msg, LogLevel.Error);
        }
    }

    /// <summary>
    /// Writes an Error to the <see cref="Logger"/>,
    /// replacing format items with the objects in <paramref name="args"/>.
    /// </summary>
    /// <remarks>
    /// Equivalent to passing a <see cref="string.Format(string, object[])"/>
    /// to the <paramref name="msg"/> argument.
    /// </remarks>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    /// <param name="args">
    /// The objects to format.
    /// </param>
    public void Error(string msg, params object[] args)
    {
        Error(string.Format(CultureInfo.InvariantCulture, msg, args));
    }

    /// <summary>
    /// Writes a Fatal error to the <see cref="Logger"/>. Use when an
    /// application is about to terminate due to a fatal error.
    /// </summary>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    public void Fatal(string msg)
    {
        if (FileLogLevel >= LogLevel.Fatal)
        {
            LogFile(msg, LogLevel.Fatal);
        }

        if (ConsoleLogLevel >= LogLevel.Fatal)
        {
            LogConsole(msg, LogLevel.Fatal);
        }
    }

    /// <summary>
    /// Writes a Fatal error to the <see cref="Logger"/>,
    /// replacing format items with the objects in <paramref name="args"/>.
    /// Use when an application is about to terminate due to a fatal error.
    /// </summary>
    /// <remarks>
    /// Equivalent to passing a <see cref="string.Format(string, object[])"/>
    /// to the <paramref name="msg"/> argument.
    /// </remarks>
    /// <param name="msg">
    /// The message to write to the log.
    /// </param>
    /// <param name="args">
    /// The objects to format.
    /// </param>
    public void Fatal(string msg, params object[] args)
    {
        Fatal(string.Format(CultureInfo.InvariantCulture, msg, args));
    }

    /// <summary>
    /// Deletes all archived logs (files ending with .[number].log.gz).
    /// </summary>
    public void DeleteArchived()
    {
        for (int i = 1; i <= MaxArchivedLogs; i++)
        {
            try
            {
                File.Delete($"{LogPath}.{i}.log.gz");
            }
            catch (FileNotFoundException) { }
        }
    }

    private void LogFile(string msg, LogLevel level)
    {
        if (msg is null)
        {
            return;
        }

        if (LogWriter is null)
        {
            InitLogFile();
        }

        lock (LogWriter)
        {
            foreach (string str in msg.Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries))
            {
                LogWriter.WriteLine(LogString(str, level, LogTimeToFile));
            }
        }
    }

    private void LogConsole(string msg, LogLevel level)
    {
        if (msg is null)
        {
            return;
        }

        lock (conLock)
        {
            ConsoleColor
                bgColour = Console.BackgroundColor,
                fgColour = Console.ForegroundColor;

            switch (level)
            {
                case LogLevel.Fatal:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            foreach (string str in msg.Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries))
            {
                Console.WriteLine(LogString(str, level, LogTimeToConsole));
            }

            Console.BackgroundColor = bgColour;
            Console.ForegroundColor = fgColour;
        }
    }

    /// <summary>
    /// Initialises the log file.
    /// Call before any attempts to write a log file.
    /// </summary>
    private void InitLogFile()
    {
        // create log directory if it doesn't exist already
        // if anyone knows why the fuck Directory.CreateDirectory
        // is doing nothing when running from a windows service and
        // pretending everything is ok i would LOVE to know
        // (workaround in YAMDCC.GUI/Program.cs)
        Directory.CreateDirectory(LogDir);

        // Rename old log files, and delete the oldest file if
        // there's too many log files
        for (int i = MaxArchivedLogs; i >= 0; i--)
        {
            try
            {
                if (i == MaxArchivedLogs)
                {
                    File.Delete($"{LogPath}.{i}.log.gz");
                }
                else
                {
                    File.Move($"{LogPath}.{i}.log.gz", $"{LogPath}.{i + 1}.log.gz");
                }
            }
            catch (FileNotFoundException) { }
        }

        try
        {
            // Set up file streams
            using (FileStream original = File.OpenRead($"{LogPath}.log"))
            using (FileStream compressed = File.Create($"{LogPath}.{1}.log.gz"))
            using (GZipStream gzStream = new(compressed, CompressionLevel.Optimal))
            {
                // Compress the file
                original.CopyTo(gzStream);
            }

            // Delete the unarchived copy of the log
            File.Delete($"{LogPath}.log");
        }
        catch (FileNotFoundException)
        {
            // Log files probably don't exist yet,
            // do nothing to avoid crash
        }

        LogWriter = new StreamWriter($"{LogPath}.log")
        {
            AutoFlush = true
        };
    }

    /// <summary>
    /// Releases all resources used by this <see cref="Logger"/> instance.
    /// </summary>
    public void Dispose()
    {
        LogWriter.Dispose();
    }
}
