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

    // Use this for initialization
    void Start()
    {

        //Create python engine
        m_engine = Python.CreateEngine();
        Reset();
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
    }
}
