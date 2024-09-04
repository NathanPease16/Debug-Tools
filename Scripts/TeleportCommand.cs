using UnityEngine;

public class TeleportCommand : Command
{
    public override int ExpectedArguments { get => 3; }
    public override string Name { get => "teleport"; }

    public override void Execute(string[] arguments)
    {
        Debug.Log($"Teleported to: ({arguments[0]}, {arguments[1]}, {arguments[2]})");
    }
}
