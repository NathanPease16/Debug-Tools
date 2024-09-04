using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Command
{
    private static Dictionary<string, MethodInfo> _commandDatabase;

    public abstract int ExpectedArguments { get; }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        _commandDatabase = new();

        var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Command)))
            .ToArray();
        
        foreach (var type in commandTypes)
        {
            var executeMethod = type.GetMethod(nameof(Execute));

            string name = type.Name.ToLower();
            
            if (name.EndsWith("command"))
                name = name.TrimEnd("command");

            _commandDatabase.Add(name, executeMethod);
        }
    }

    public static void TryExecuteCommand(string command, string[] arguments)
    {
        if (_commandDatabase.TryGetValue(command, out var method))
        {
            var instance = Activator.CreateInstance(method.DeclaringType);

            var expectedArguments = (int)method.DeclaringType.GetProperty(nameof(ExpectedArguments)).GetValue(instance);
            if (arguments.Length != expectedArguments)
            {
                Debug.LogError($"Error executing {command}; Expected {expectedArguments} arguments, got {arguments.Length}");
                return;
            }

            method.Invoke(instance, new object[] { arguments });
        }
    }

    public abstract void Execute(string[] arguments);
}
