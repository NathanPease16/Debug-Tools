using TMPro;
using UnityEngine;

public class ConsoleLog : MonoBehaviour
{   
    [Header("Logs")]
    [SerializeField] private TMP_Text _textPrefab;
    [SerializeField] private Transform _textLog;

    [Header("Commands")]
    [SerializeField] private TMP_InputField _commandField;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Application.logMessageReceived += Log;

        _commandField.onEndEdit.AddListener(InputCommand);
    }

    private void InputCommand(string command)
    {   
        _commandField.text = string.Empty;

        command = command.TrimEnd(' ');

        var commandArguments = command.Split(' ');
        
        string commandName = string.Empty;
        var arguments = new string[] { };

        if (commandArguments.Length > 0)
            commandName = commandArguments[0];
        
        if (commandArguments.Length > 1)
            arguments = commandArguments[1..^0];

        Command.TryExecuteCommand(commandName, arguments);
    }

    private void Log(string log, string stackTrace, LogType type)
    {
        var text = Instantiate(_textPrefab, _textLog);

        text.transform.SetAsFirstSibling();
        text.text = $"[{type}] {log}";
        text.gameObject.SetActive(true);
    }
}
