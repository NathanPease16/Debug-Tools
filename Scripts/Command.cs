using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Initializes all commands in the assembly and provides functionality for
/// executing commands
/// </summary>
public abstract class Command
{
    private static Dictionary<string, Data> _commandDatabase;

    // The name of the command (what needs to be entered in the console for it to be called)
    public abstract string Name { get; }
    // The number of arguments that the command expects (i.e. a teleport command might expect 3; an x, y, and z coordinate)
    public abstract int ExpectedArguments { get; }
    
    /// <summary>
    /// Initializes all commands on startup of the application by creating a dictionary pairing
    /// of each command in the assembly's name and important info (an instance and expected
    /// number of parameters) 
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // Create new dictionary
        _commandDatabase = new();

        // Find all commands in the assembly (type must be a class, not be abstract, and 
        // be a subclass of the Command type)
        var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Command)))
            .ToArray();
        
        // For each command type...
        foreach (var type in commandTypes)
        {
            // Create an instance
            var instance = (Command)Activator.CreateInstance(type);

            // Create new data struct for storing instances and expected argument counts
            var data = new Data(instance, instance.ExpectedArguments);

            // Add to the dictionary
            _commandDatabase.Add(instance.Name, data);
        }
    }

    /// <summary>
    /// Attempts to execute a specific command with a given set of arguments as strings
    /// </summary>
    /// <param name="command">Name of the command to attempt to execute</param>
    /// <param name="arguments">List of all arguments to be fed to the command</param>
    public static void TryExecuteCommand(string command, string[] arguments)
    {
        // Make sure the command exists
        if (_commandDatabase.TryGetValue(command, out var data))
        {   
            // Make sure there is the correct number of arguments
            if (arguments.Length != data.ExpectedArguments)
            {
                Debug.LogError($"Error executing {command}; Expected {data.ExpectedArguments} arguments, got {arguments.Length}");
                return;
            }

            // Execute the command
            data.Instance.Execute(arguments);
            return;
        }

        Debug.LogError($"Unkown command: {command}");
    }

    /// <summary>
    /// Function called to execute a command
    /// </summary>
    /// <param name="arguments">A list of arguments taken in by the command</param>
    public abstract void Execute(string[] arguments);

    /// <summary>
    /// Provides data storage for commands, storing an instance to be re-used for efficiency (i.e. not making
    /// an instance every time the command is executed) and expected arguments (so it doesn't need to be obtained
    /// from the type data every time)
    /// </summary>
    public class Data
    {
        public Command Instance { get; }
        public int ExpectedArguments { get; }
        
        public Data(Command instance, int expectedArguments)
        {
            Instance = instance;
            ExpectedArguments = expectedArguments;
        }
    }
}
