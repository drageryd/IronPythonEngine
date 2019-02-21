using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogExample : MonoBehaviour {

    PythonEngine pythonEngine;
    public string path;
    public int count;
    bool running;

    [TextArea]
    public string stdin;

	// Use this for initialization
	void Start () {
        pythonEngine = GetComponent<PythonEngine>();
        count = 0;
        running = false;
    }
	
	// Update is called once per frame
	void Update () {
        //Print stdout if it changed
        if (pythonEngine.StdOutChanged())
        {
            Debug.Log(pythonEngine.GetStdOut());
        }

        //Print one last time when terminated (includes exceptions)
        if (running && !pythonEngine.IsRunning())
        {
            running = false;
            Debug.Log(pythonEngine.GetStdOut() + 
                      pythonEngine.GetException());
        }
	}

    // Send text on sdtin when trigger occurrs (5 times)
    // e.g bouncing ball with sphere collider set to trigger
    private void OnTriggerEnter(Collider other)
    {
        //if (count < 2) 
        //{
            if (!pythonEngine.IsRunning())
            {
                count += 1;
                Debug.Log("Hej");
                pythonEngine.ExecuteFile(path);
            }
        //}
        /*
        if (!pythonEngine.IsRunning())
        {
            running = true;
            Debug.Log("Execution " + stdin);
            pythonEngine.ExecuteFile(path);
            count = 0;
        }
        else if (count < 4)
        {
            pythonEngine.WriteStdIn(stdin, false);
            count++;
        }
        else if (count == 4)
        {
            Debug.Log("stdin ended");
            pythonEngine.WriteStdIn(stdin, true);
            count++;
        }
        */

        //pythonEngine.ListPorts();
        //pythonEngine.RunPortCommand("Cube", "test");
    }
}
