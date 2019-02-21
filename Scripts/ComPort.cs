using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComPort : MonoBehaviour
{
    public string name;
    private List<string> commands;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void InitCommands()
    {
        commands = new List<string>();
    }

    protected void AddCommand(string command)
    {
        commands.Add(command);
    }

    //These are used by Python
    public string get_name()
    {
        if (name != null) return name;
        else return "UnnamedPort";
    }

    public string get_commands()
    {
        string s = "";
        foreach (string command in commands)
        {
            s += command + "\n";
        }
        return s;
    }
}
