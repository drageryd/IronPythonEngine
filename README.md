# IronPythonEngine

Small library for running Python scripts in a Unity game.

## Setup

- Import `IronPythonEngine` to your Unity Assets directory
- Drag PythonEngine script to an object
- Execute script with `PythonEngine.ExecuteFile`
- Send `stdin` and read `stdout` with `WriteStdIn` and `GetStdOut`

## Examples
Located in `Examples`

### `DebugLogExample.cs`
Prints `stdout` to `Debug.Log` console.

- Drag to same object as `PythonEngine`
- Set path to a python script in `path` (e.g `./Assets/IronPythonEngine/Examples/test.py`)
- Dont do anything with `count`
- Write any `stdin` text in Stdin text field
- Run, python stdout is printed in `Debug.Log`