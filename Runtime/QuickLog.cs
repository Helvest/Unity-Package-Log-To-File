using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace LogToFile
{

	[Serializable]
	public class QuickLog
	{

		#region Enums

		public enum BuildType
		{
			None,
			Build,
			Debug,
			Editor
		}

		#endregion

		#region Fields

		public Object cachedContext = default;

		private static BuildType _buildType = BuildType.None;

		public BuildType GetBuildType
		{
			get
			{
				if (_buildType == BuildType.None)
				{
					if (Application.isEditor)
					{
						_buildType = BuildType.Editor;
					}
					else if (Debug.isDebugBuild)
					{
						_buildType = BuildType.Debug;
					}
					else
					{
						_buildType = BuildType.Build;
					}
				}

				return _buildType;
			}
		}

		public DebugLevel debugLevelInBuild = DebugLevel.Exception;
		public DebugLevel debugLevelInDebug = DebugLevel.Warning;
		public DebugLevel debugLevelInEditor = DebugLevel.Debug;

		private DebugLevel ActualDebugLevel
		{
			get
			{
				switch (GetBuildType)
				{
					default:
					case BuildType.Build:
						return debugLevelInBuild;
					case BuildType.Debug:
						return debugLevelInDebug;
					case BuildType.Editor:
						return debugLevelInEditor;
				}
			}
		}

		[FormerlySerializedAs("_useDebugActionInBuild")]
		public bool useDebugActionInBuild = false;
		[FormerlySerializedAs("_useDebugActionInDebug")]
		public bool useDebugActionInDebug = false;
		[FormerlySerializedAs("_useDebugActionInEditor")]
		public bool useDebugActionInEditor = true;

		public bool UseDebugAction
		{
			get
			{
				switch (GetBuildType)
				{
					default:
					case BuildType.Build:
						return useDebugActionInBuild;
					case BuildType.Debug:
						return useDebugActionInDebug;
					case BuildType.Editor:
						return useDebugActionInEditor;
				}
			}
		}

		#endregion

		#region Enum

		public enum DebugLevel
		{
			Info = 0,
			Debug = 1,
			Warning = 2,
			Exception = 3,
			Error = 4,
			None = 5
		}

		#endregion

		#region Logs

		public void Log(string msg, DebugLevel level, Object context = null)
		{
			if (level < ActualDebugLevel)
			{
				return;
			}

			context = context != null ? context : cachedContext;

			switch (level)
			{
				case DebugLevel.Info:
				case DebugLevel.Debug:
					Debug.Log(msg, context);
					break;
				case DebugLevel.Warning:
					Debug.LogWarning(msg, context);
					break;
				case DebugLevel.Exception:
				case DebugLevel.Error:
					Debug.LogError(msg, context);
					break;
				case DebugLevel.None:
					break;
			}
		}

		public void LogInfo(string msg, Object context = null)
		{
			Log(msg, DebugLevel.Info, context);
		}

		public void Log(string msg, Object context = null)
		{
			Log(msg, DebugLevel.Debug, context);
		}

		public void LogWarning(string msg, Object context = null)
		{
			Log(msg, DebugLevel.Warning, context);
		}

		public void LogException(Exception e, Object context = null)
		{
			if (DebugLevel.Exception < ActualDebugLevel)
			{
				return;
			}

			context = context != null ? context : cachedContext;
			Debug.LogException(e, context);
		}

		public void LogError(string msg, Object context = null)
		{
			Log(msg, DebugLevel.Error, context);
		}

		#endregion

	}

}