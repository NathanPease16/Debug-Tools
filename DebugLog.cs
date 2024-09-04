using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Reads all logs sent to the console and appends it to the most recent log file
/// for the specified log level (i.e. build, editor, development, etc.)
/// </summary>
public static class DebugLog
{
    private static readonly string _directoryName = "logs";
    private static readonly string _oldDirectoryName = $"{_directoryName}.old";
    private static readonly DebugEnvironment _logLevel = DebugEnvironment.BUILD;

    private static readonly string _logDirectory = Path.Combine(Application.dataPath, "..", _directoryName);
    private static readonly string _oldLogDirectory = Path.Combine(_logDirectory, _oldDirectoryName);
    private static string _logFile;

    /// <summary>
    /// Initializes a new log file each time the game is started
    /// </summary>
    // Attach the static script to the Unity runtime so it doesn't have to be "called", it just runs
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // TODO: Implement the option to choose the log environment level in project settings
        // Make sure the current environment is the build environment
        if (Environment.Is(_logLevel))
        {   
            // Attach the Log method to the logMessageReceived callback
            Application.logMessageReceived += Log;
            
            // Create the log directory and directory to store old logs in
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
            if (!Directory.Exists(_oldLogDirectory))
                Directory.CreateDirectory(_oldLogDirectory);

            // Get all files in the log directory
            var files = Directory.GetFiles(_logDirectory);

            // Move all the old log files to the logs.old folder
            foreach (var file in files)
                File.Move(file, Path.Combine(_oldLogDirectory, Path.GetFileName(file)));
            
            // Create a new log file with the current date as the name (for easy organization)
            _logFile = Path.Combine(_logDirectory, $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.txt");

            File.Create(_logFile);
        }
    }

    /// <summary>
    /// Adds every log to the currently active log file
    /// </summary>
    /// <param name="log">The log to add to the log file</param>
    /// <param name="stackTrace">Stack trace of the log (ignored)</param>
    /// <param name="type">Log type (i.e. "log", "warning," "error," etc.</param>
    private static void Log(string log, string stackTrace, LogType type)
    {
        // Append the log to the log file
        File.AppendAllText(_logFile, $"[{type}] {log} {DateTime.Now}\n");
    }
}
