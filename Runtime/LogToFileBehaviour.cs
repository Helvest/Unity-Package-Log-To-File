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

		[SerializeField]
		protected int maxByteSize = 1048576; // 1048576 bytes = 1 MB

#if UNITY_EDITOR || DEVELOPMENT_BUILD
		[SerializeField]
		protected bool useInEditor = false;
#endif

		public virtual LogToFile LogToFileUtility { get; protected set; }

		public bool CanWrite => LogToFileUtility != null && LogToFileUtility.CanWrite;

		#endregion

		#region Init

		protected virtual void Awake()
		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			if (!useInEditor)
			{
				enabled = false;
				return;
			}
#endif

			LogToFileUtility = new LogToFile()
			{ 
				MaxByteSize = maxByteSize 
			};

			LogToFileUtility.Init(pathData);
		}

		protected virtual void OnEnable()
		{
			if (LogToFileUtility != null)
			{
				Application.logMessageReceived += LogToFileUtility.OnLog;
			}
		}

		protected virtual void OnDisable()
		{
			if (LogToFileUtility != null)
			{
				Application.logMessageReceived -= LogToFileUtility.OnLog;
			}
		}

		protected virtual void OnApplicationQuit()
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
