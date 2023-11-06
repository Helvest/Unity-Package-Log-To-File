using System;
using UnityEngine;

namespace LogToFile
{
	public class LogToFileBehaviour : MonoBehaviour
	{

		#region Fields

		[SerializeField]
		protected string folderName = "Logs";

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
				FolderName = folderName, 
				MaxByteSize = maxByteSize 
			};

			LogToFileUtility.Init();
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
			}

			LogToFileUtility.Dispose();
			LogToFileUtility = null;
		}

		#endregion

	}
}
