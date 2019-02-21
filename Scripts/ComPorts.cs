using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComPorts : MonoBehaviour
{
    private Dictionary<string, ComPort> comPorts;

    public GameObject testConnect;

    // Use this for initialization
    void Start()
    {
        comPorts = new Dictionary<string, ComPort>();

        //Temporary test
        if (testConnect != null)
        {
            ComPort port = testConnect.GetComponent<ComPort>();
            if (port != null) ConnectPort(port);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool ConnectPort(ComPort port)
    {
        string portName = port.get_name();
        if (!comPorts.ContainsKey(portName))
        {
            comPorts.Add(portName, port);
            return true;
        }
        return false;
    }

    public bool DisconnectPort(ComPort port)
    {
        string portName = port.get_name();
        return comPorts.Remove(portName);
    }

    public void DisconnectAllPorts()
    {
        comPorts.Clear();
    }

    public string PrintPorts()
    {
        string s = "";
        foreach (string portName in comPorts.Keys)
        {
            s += portName + "\n";
        }
        return s;
    }

    public List<string> GetPorts()
    {
        List<string> l = new List<string>();
        foreach (string key in comPorts.Keys) l.Add(key);
        return l;
    }

    public ComPort GetPort(string portName)
    {
        if (comPorts.ContainsKey(portName))
        {
            return comPorts[portName];
        }
        return null;
    }

    public string PrintPortCommands(string portName)
    {
        if (comPorts.ContainsKey(portName))
        {
            return comPorts[portName].get_commands();
        }
        return "Port name " + portName + " does not exist!\n";
    }
}
