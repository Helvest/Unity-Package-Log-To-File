using System;
using System.IO;
using System.Text;
using EasyPath;
using UnityEngine;

namespace LogToFile
{
    public class LogToFile : IDisposable
    {
        #region Fields

        public int MaxByteSize { get; set; } = 1048576;

        private string _filePathAndName;
        private bool _canWrite;

        private readonly StringBuilder _messageBuilder = new StringBuilder(4096);
        private readonly StringBuilder _stackTraceBuilder = new StringBuilder(4096);

        private const string REPLACE_SYMBOL = "�";
        private const string NEW_LINE = "\n";
        private const string NEW_LINE_TAB = "\n\t";
        private const string NEW_LINE_TWO_TAB = "\n\t\t";

        /// <summary>
        /// Can log messages be written to the file?
        /// </summary>
        public bool CanWrite
        {
            get => _canWrite;
            set => _canWrite = value && !MaxSizeReached;
        }

        /// <summary>
        /// The actual size of the log file.
        /// </summary>
        public long ActualSize { get; private set; }

        /// <summary>
        /// Has the maximum log file size been reached?
        /// </summary>
        public bool MaxSizeReached => ActualSize >= MaxByteSize;

        #endregion

        #region Init

        /// <summary>
        /// Constructor for LogToFileUtility.
        /// </summary>
        public void Init<T>(T pathData) where T : PathData, new()
        {
            var path = new T();
            path.Copy(pathData);

            if (string.IsNullOrWhiteSpace(path.FileName))
            {
                path.FileName = $"Log {DateTime.Now:yyyy'-'MM'-'dd HH'h'mm'm'ss's'}";
            }
            else
            {
                path.FileName = $"{path.FileName} {DateTime.Now:yyyy'-'MM'-'dd HH'h'mm'm'ss's'}";
            }

            if (string.IsNullOrWhiteSpace(path.Extension))
            {
                path.Extension = "log";
            }

            _filePathAndName = path.GetFullPath();

            if (!Directory.Exists(path.GetDirectoryPath()))
            {
                Directory.CreateDirectory(path.GetDirectoryPath());
            }

            CanWrite = true;
        }

        #endregion

        #region Dispose

        private bool _disposed;

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_canWrite)
            {
                WriteToLogFileAsync("Application Quit");
            }

            // Clean up unmanaged resources here.
            _messageBuilder.Clear();
            _stackTraceBuilder.Clear();

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        // Finalizer
        ~LogToFile()
        {
            Dispose();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Log message handler.
        /// </summary>
        public void OnLog(string message, string stackTrace, LogType type)
        {
            if (!_canWrite)
            {
                return;
            }

            switch (type)
            {
                case LogType.Log:
                case LogType.Warning:
                    FormatAndWriteToLogFileAsync(message, type);
                    break;

                default:
                    FormatAndWriteToLogFileAsync(message, stackTrace, type);
                    break;
            }
        }

        /// <summary>
        /// Formats and writes a log message to file asynchronously.
        /// </summary>
        private void FormatAndWriteToLogFileAsync(string message, LogType type)
        {
            _messageBuilder
                .Clear()
                .Append(message)
                .Replace("\r\n", REPLACE_SYMBOL)
                .Replace("\r", REPLACE_SYMBOL)
                .Replace("\n", REPLACE_SYMBOL)
                .Replace(REPLACE_SYMBOL, NEW_LINE_TAB);

            WriteToLogFileAsync(_messageBuilder.ToString(), type);
        }

        private void FormatAndWriteToLogFileAsync(string message, string stackTrace, LogType type)
        {
            _messageBuilder
                .Clear()
                .Append(message)
                .Replace("\r\n", REPLACE_SYMBOL)
                .Replace("\r", REPLACE_SYMBOL)
                .Replace("\n", REPLACE_SYMBOL)
                .Replace(REPLACE_SYMBOL, NEW_LINE_TAB);

            _stackTraceBuilder
                .Clear()
                .Append(REPLACE_SYMBOL)
                .Append(stackTrace)
                .Replace("\r\n", REPLACE_SYMBOL)
                .Replace("\r", REPLACE_SYMBOL)
                .Replace("\n", REPLACE_SYMBOL)
                .Replace(REPLACE_SYMBOL, NEW_LINE_TWO_TAB);

            _messageBuilder.Append(_stackTraceBuilder);


            WriteToLogFileAsync(_messageBuilder.ToString(), type);
        }

        private int _frameCount = -1;

        /// <summary>
        /// Writes a message to the log file asynchronously.
        /// </summary>
        private async void WriteToLogFileAsync(string message, LogType type = LogType.Log)
        {
            if (_frameCount != Time.frameCount)
            {
                _frameCount = Time.frameCount;

                using (var logFile = new StreamWriter(_filePathAndName, true))
                {
                    await logFile.WriteLineAsync(
                        $"{NEW_LINE}{DateTime.Now:yyyy'.'MM'.'dd HH':'mm':'ss} frame {_frameCount}{NEW_LINE}");
                    ActualSize = logFile.BaseStream.Length;
                }

                if (MaxSizeReached)
                {
                    _canWrite = false;

                    string finalText = $"The file has exceeded the allowed size: {ActualSize} > {MaxByteSize}";
                    Debug.LogWarning(finalText);

                    return;
                }
            }

            try
            {
                using var logFile = new StreamWriter(_filePathAndName, true);
                
                if (type == LogType.Log)
                {
                    await logFile.WriteLineAsync($"\t{message}{NEW_LINE}");
                }
                else
                {
                    await logFile.WriteLineAsync($"\t[{type}] {message}{NEW_LINE}");
                }
            }
            catch (Exception e)
            {
                _canWrite = false;
                Debug.LogError($"Error while trying to write into log file {e.Message}");
            }
        }

        #endregion
    }
}