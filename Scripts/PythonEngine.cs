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
using CustomStream;

using System.Threading;

public class PythonEngine : MonoBehaviour
{

    //Path to script
    private string path;
    //Directory from where python was executed
    private string cwd;
    private string unityCwd;
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
        cwd = Directory.GetCurrentDirectory();
        unityCwd = Directory.GetCurrentDirectory();

        //Connect to gamobject comports
        //TODO: Initialize here only
        comPorts = GetComponent<ComPorts>();
        Debug.Log("comports " + comPorts);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnDestroy()
    {
        // Clears thread if still running
        if (IsRunning())
        {
            myThread.Abort();
        }

        //Reset working directory
        Directory.SetCurrentDirectory(unityCwd);
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

    public void SetCwd(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        if (di.Exists) {
            cwd = path;
        }
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

    public bool StdOutAvailable()
    {
        return (pythonStdout.Length != 0);
    }

    public string GetLine()
    {
        StreamReader sr = new StreamReader(pythonStdout);
        return sr.ReadLine()+"\n";
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
        //Set working directory
        Directory.SetCurrentDirectory(cwd);

        //Run script
        ScriptSource m_source = m_engine.CreateScriptSourceFromFile(path);
        m_source.ExecuteAndWrap(m_scope, out exception);
        if (exception != null)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(GetException());
            pythonStdout.Write(bytes, 0, bytes.Length);
        }

        //Reset working directory
        Directory.SetCurrentDirectory(unityCwd);
    }

    public void Abort()
    {
        if (myThread.IsAlive)
        {
            myThread.Abort();
            if (exception != null)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(GetException());
                pythonStdout.Write(bytes, 0, bytes.Length);
            }

            //Reset working directory
            Directory.SetCurrentDirectory(unityCwd);
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