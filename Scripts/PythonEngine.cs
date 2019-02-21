using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IronPython;
using IronPython.Modules;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Runtime.Remoting;

using System.IO;
using System.Text;
using Application;

using System.Threading;

public class PythonEngine : MonoBehaviour
{

    private string path;
    private ScriptEngine m_engine;
    private ScriptScope m_scope;
    private StreamStdio pythonStdin;
    private StreamStdio pythonStdout;
    private ObjectHandle exception;
    private int pythonStdoutLen = 0;
    private Thread myThread;

    //Get comports
    private ComPorts comPorts;

    // Use this for initialization
    void Start()
    {
        //Create python engine
        m_engine = Python.CreateEngine();

        //Connect to gamobject comports
        //TODO: Initialize here only
        comPorts = GetComponent<ComPorts>();
        Debug.Log("comports " + comPorts);
        
        //Reset engine
        //Reset();
    }

    // Update is called once per frame
    void Update()
    {
        //m_engine.Execute("output = 'Python says hi!'", m_scope);
        //m_engine.ExecuteFile(path, m_scope);
        /*
        // Get scope variables
        if (m_scope != null)
        {
        IEnumerable<string> variables = m_scope.GetVariableNames();
        bool variable_exists = false;
        foreach (string varname in variables)
        {
        if (varname == "data_available")
        {
            variable_exists = true;
        }
        }

        if (variable_exists)
        {
        bool data_available = m_scope.GetVariable<bool>("data_available");
        if (data_available)
        {
            string data = m_scope.GetVariable<string>("data");
            string s = string.Format("{0}: {1}", Time.time, data);
            Debug.Log("From scope:" + s);
            //Debug.Log("Stream size: " + pythonStdout.Length);
            m_scope.SetVariable("data_available", false);
        }
        }
        }
        */
    }

    void OnDestroy()
    {
        // Clears thread if still running
        if (IsRunning())
        {
            myThread.Abort();
        }
    }

    //Setup PythonEngine for new execution
    private void Reset()
    {
        // Create a new scope and output stream
        m_scope = m_engine.CreateScope();
        pythonStdin = new StreamStdio();
        pythonStdout = new StreamStdio();
        m_engine.Runtime.IO.SetInput(pythonStdin, Encoding.ASCII);
        m_engine.Runtime.IO.SetOutput(pythonStdout, Encoding.ASCII);
        m_engine.Runtime.IO.SetErrorOutput(pythonStdout, Encoding.ASCII);

        //Add listed comports in comports
        InitPorts();
        
        pythonStdoutLen = 0;
    }

    //Is the engine running?
    public bool IsRunning()
    {
        if (myThread != null) return myThread.IsAlive;
        else return false;
    }

    //Execute file path
    public void ExecuteFile(string p)
    {
        if (!IsRunning())
        {
            // Reset before execution
            Reset();
            path = p;
            myThread = new Thread(Run);
            myThread.Start();
        }
    }

    //True if new data on stdout since last GetStdOut
    public bool StdOutChanged()
    {
        if (pythonStdout == null) return false;
        int l = (int)pythonStdout.Length;
        return l > pythonStdoutLen;
    }

    //Returns whole stdout buffer as a string
    public string GetStdOut()
    {
        pythonStdoutLen = (int)pythonStdout.Length;
        return pythonStdout.GetText();
    }

    //Returns exceptions in python script as string
    public string GetException()
    {
        // Get exception if crashed
        if (exception != null)
        {
            return m_engine.GetService<ExceptionOperations>().FormatException(exception);
        }
        else
        {
            return "";
        }
    }

    //Write stdin to script
    public void WriteStdIn(string s, bool end)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(s);
        pythonStdin.Write(bytes, 0, bytes.Length);
        if (end) pythonStdin.WriteEnd();
    }

    // Thread function
    void Run()
    {
        // Run script
        ScriptSource m_source = m_engine.CreateScriptSourceFromFile(path);
        m_source.ExecuteAndWrap(m_scope, out exception);
        if (exception != null)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(GetException());
            pythonStdout.Write(bytes, 0, bytes.Length);
        }
    }


    //Comport communication
    void InitPorts()
    {
        foreach (string portName in comPorts.GetPorts())
        {
            AddPort(portName);
        }
    }

    void AddPort(string portName)
    {
        ComPort comPort = comPorts.GetPort(portName);
        if (comPort != null && !m_scope.ContainsVariable(portName))
        {
            Debug.Log(portName + " added to scope");
            m_scope.SetVariable(portName, comPort);
        }
    }

    void RemovePort(string portName)
    {
        if (m_scope.ContainsVariable(portName))
        {
            m_scope.RemoveVariable(portName);
        }
    }
}