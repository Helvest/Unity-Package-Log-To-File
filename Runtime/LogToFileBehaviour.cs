using EasyPath;
using UnityEngine;

namespace LogToFile
{
    public class LogToFileBehaviour : MonoBehaviour
    {
        #region Fields

        public PathDataWithParent pathData = new PathDataWithParent()
        {
            PathSystem = PathSystem.GameData,
            SubPath = "../Log",
            FileName = "Log",
            Extension = "log"
        };

        [SerializeField] private int _maxByteSize = 1048576; // 1048576 bytes = 1 MB

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool _useInEditor;
#endif

        public LogToFile LogToFileUtility { get; protected set; }

        public bool CanWrite => LogToFileUtility is { CanWrite: true };

        #endregion

        #region Init

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!_useInEditor)
            {
                enabled = false;
                return;
            }
#endif

            LogToFileUtility = new LogToFile()
            {
                MaxByteSize = _maxByteSize
            };

            LogToFileUtility.Init(pathData);
        }

        private void OnEnable()
        {
            if (LogToFileUtility != null)
            {
                Application.logMessageReceived += LogToFileUtility.OnLog;
            }
        }

        private void OnDisable()
        {
            if (LogToFileUtility != null)
            {
                Application.logMessageReceived -= LogToFileUtility.OnLog;
            }
        }

        private void OnApplicationQuit()
        {
            if (LogToFileUtility != null)
            {
                Application.logMessageReceived -= LogToFileUtility.OnLog;
                LogToFileUtility.Dispose();
            }

            LogToFileUtility = null;
        }

        #endregion
    }
}