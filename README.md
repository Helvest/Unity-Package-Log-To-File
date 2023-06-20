# Log To File Unity Package

*Created by Helvest*

Log To File is a versatile Unity package designed to aid in the development and debugging process by providing a simple and efficient way to log and store console messages in text files. These logs include detailed runtime information, error messages, warnings, and more, providing a valuable resource for diagnosing and troubleshooting your Unity projects.

## Features

    Easy Configuration: Set the folder name and maximum byte size of your log file with just a couple of clicks in the Unity Inspector.
    Robust Logging: Captures and records all console logs, including standard log messages, warnings, errors, exceptions, and assertions.
    Asynchronous Writing: Log entries are written to file asynchronously to prevent blocking the main thread.
    Automatic Size Check: If a log file reaches its maximum size limit, it automatically halts writing operations and provides a warning message.
    Flexible Usage: LogToFile can be used directly in your code, or through MonoBehaviour with the provided LogToFileBehaviour script.

## Usage

### Setup

    Add the LogToFileBehaviour script to any GameObject in your scene. This GameObject will act as the manager of log file writing.
    In the Unity Inspector, you can set the Folder Name (where the log files will be saved) and the Max Byte Size (the maximum size of a single log file, in bytes).

### Configuration Options

    Folder Name: Specify the directory name where your log files will be saved. The directory will be created if it does not exist.
    Max Byte Size: Set the maximum byte size of your log file. When this limit is reached, logging will be halted.

### Output

    The log file is named using the pattern "Log {DateTime}.log". Each new log session will create a new file, so you will have a history of logs for each play session.
    The output log files are located in the same directory as the Unity project, in the folder specified by Folder Name.

### Console Logs

    The log file captures all console messages, including logs, warnings, errors, exceptions, and assertions, in the same order they are received. Each entry is timestamped and appended with the frame count at the time the message was logged.

<sub>Please note: You should be cautious when using the LogToFile package on the live game, as the I/O operations may impact game performance.</sub>

Enjoy debugging with Log To File!