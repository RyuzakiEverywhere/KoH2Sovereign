using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000037 RID: 55
[DisallowMultipleComponent]
public class AttributeConsoleManager : MonoBehaviour
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000130 RID: 304 RVA: 0x0000B832 File Offset: 0x00009A32
	public static AttributeConsoleManager instance
	{
		get
		{
			if (!AttributeConsoleManager._instance)
			{
				AttributeConsoleManager._instance = Object.FindObjectOfType<AttributeConsoleManager>();
			}
			return AttributeConsoleManager._instance;
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0000B850 File Offset: 0x00009A50
	private void Awake()
	{
		if (AttributeConsoleManager.instance != this)
		{
			AttributeConsoleGUI component = base.GetComponent<AttributeConsoleGUI>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			Object.Destroy(this);
			return;
		}
		base.gameObject.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0000B8A4 File Offset: 0x00009AA4
	private void Update()
	{
		if (this.streamOutput && this.flushDelay > 0f && this.flushTime > 0f && Time.realtimeSinceStartup > this.flushTime)
		{
			this.flushTime = -1f;
			this.stream.Flush();
		}
	}

	// Token: 0x06000133 RID: 307 RVA: 0x0000B8F8 File Offset: 0x00009AF8
	private void OnEnable()
	{
		if (this.streamOutput)
		{
			this.DebugLog("Creating Stream...", AttributeConsoleManager.LogLevel.Info);
			this.stream = new StreamWriter(new FileStream(Application.dataPath + this.streamFile, FileMode.Create));
		}
		Application.logMessageReceived += this.OnUnityLog;
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.automaticlyFetchMethods)
		{
			this.DebugLog("Loading functions...", AttributeConsoleManager.LogLevel.Info);
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			this.FetchAllFunctions();
			float num = Time.realtimeSinceStartup - realtimeSinceStartup;
			this.DebugLog("Finished loading all functions. Took " + num + "s", AttributeConsoleManager.LogLevel.Info);
		}
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000B997 File Offset: 0x00009B97
	private void OnDisable()
	{
		Application.logMessageReceived -= this.OnUnityLog;
		if (this.stream != null)
		{
			this.DebugLog("Closing Stream...", AttributeConsoleManager.LogLevel.Info);
			this.stream.Close();
			this.stream = null;
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x0000B9D0 File Offset: 0x00009BD0
	public bool AttemptExecute(string inputline)
	{
		string input = inputline.Replace("\n", "").Trim();
		MatchCollection matchCollection = new Regex("\"([^\"\\\\]*(?:\\\\.[^\"\\\\]*)*)\"|\\S+").Matches(input);
		string command = "";
		string[] array = (matchCollection.Count > 0) ? new string[matchCollection.Count - 1] : new string[0];
		for (int i = 0; i < matchCollection.Count; i++)
		{
			string text = matchCollection[i].Value;
			if (text.StartsWith("\"", StringComparison.Ordinal) && text.EndsWith("\"", StringComparison.Ordinal))
			{
				text = text.Substring(1, text.Length - 2);
			}
			text = text.Replace("\\\"", "\"");
			if (i == 0)
			{
				command = text;
			}
			else
			{
				array[i - 1] = text;
			}
		}
		return this.AttemptExecute(command, array);
	}

	// Token: 0x06000136 RID: 310 RVA: 0x0000BAAC File Offset: 0x00009CAC
	public bool AttemptExecute(string command, string[] args)
	{
		AttributeConsoleManager.ConsoleFunction[] array = (from f in this.functions
		where f.command.Equals(command)
		select f).ToArray<AttributeConsoleManager.ConsoleFunction>();
		if (array.Length == 0)
		{
			this.DebugLog("Command '" + command + "' not found!", AttributeConsoleManager.LogLevel.Warning);
			return false;
		}
		array = (from f in array
		where f.parameters.Length == args.Length
		select f).ToArray<AttributeConsoleManager.ConsoleFunction>();
		if (array.Length == 0)
		{
			this.DebugLog(string.Concat(new object[]
			{
				"Command '",
				command,
				"' with ",
				args.Length,
				" arguments was not found!"
			}), AttributeConsoleManager.LogLevel.Warning);
			return false;
		}
		string text = "";
		foreach (string str in args)
		{
			text = text + " " + str;
		}
		this.OnLog(command + text, "", ELogType.Command);
		if (array.Length == 1)
		{
			this.InvokeFunction(array[0], args, false);
			return true;
		}
		this.DebugLog(string.Concat(new object[]
		{
			"Ambiguity in the command '",
			command,
			"' with ",
			args.Length,
			" arguments! ",
			array.Length,
			" possibilites detected! Attempting each one untill correct is given."
		}), AttributeConsoleManager.LogLevel.Warning);
		foreach (AttributeConsoleManager.ConsoleFunction func in array)
		{
			if (this.InvokeFunction(func, args, true))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000BC58 File Offset: 0x00009E58
	public void FetchAllFunctions()
	{
		this.functions.Clear();
		Assembly callingAssembly = Assembly.GetCallingAssembly();
		BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		foreach (MethodInfo methodInfo in (from m in callingAssembly.GetTypes().SelectMany((Type t) => t.GetMethods(flags))
		where m.GetCustomAttributes(typeof(ConsoleMethod), false).Length != 0
		select m).ToArray<MethodInfo>())
		{
			ConsoleMethod[] array2 = methodInfo.GetCustomAttributes(typeof(ConsoleMethod), false) as ConsoleMethod[];
			this.DebugLog(string.Concat(new object[]
			{
				"Found ",
				array2.Length,
				" attributes on ",
				methodInfo.Name
			}), AttributeConsoleManager.LogLevel.Info);
			ConsoleMethod[] array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				ConsoleMethod consoleMethod = array3[j];
				string text = consoleMethod.command.Equals("") ? methodInfo.Name : consoleMethod.command;
				string help = consoleMethod.help;
				this.DebugLog("Parsing Command '" + text + "'", AttributeConsoleManager.LogLevel.Info);
				AttributeConsoleManager.ConsoleFunction func = new AttributeConsoleManager.ConsoleFunction();
				func.command = text;
				func.help = help;
				func.methodInfo = methodInfo;
				if (this.GetMethodParameters(methodInfo, out func.parameters))
				{
					int num = (from f in this.functions
					where f.command == func.command
					where f.parameters == func.parameters
					select f).Count<AttributeConsoleManager.ConsoleFunction>();
					string text2 = "";
					foreach (AttributeConsoleManager.ConsoleParameter consoleParameter in func.parameters)
					{
						text2 = text2 + consoleParameter.ToString() + ", ";
					}
					if (text2.Length > 2)
					{
						text2 = text2.Remove(text2.Length - 2);
					}
					if (num != 0)
					{
						this.DebugLog(string.Concat(new string[]
						{
							"Command ",
							func.command,
							" with params (",
							text2,
							") already exists!"
						}), AttributeConsoleManager.LogLevel.Error);
					}
					else
					{
						this.DebugLog(string.Concat(new string[]
						{
							"Adding Command ",
							func.command,
							" with params (",
							text2,
							")"
						}), AttributeConsoleManager.LogLevel.Info);
						this.functions.Add(func);
					}
				}
			}
		}
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000BF14 File Offset: 0x0000A114
	private bool InvokeFunction(AttributeConsoleManager.ConsoleFunction func, string[] args, bool silent = false)
	{
		AttributeConsoleManager.ConsoleParameter[] parameters = func.parameters;
		if (args.Length != parameters.Length)
		{
			if (!silent)
			{
				this.DebugLog("Parameter length miss match", AttributeConsoleManager.LogLevel.Warning);
			}
			return false;
		}
		object[] array = new object[parameters.Length];
		for (int i = 0; i < array.Length; i++)
		{
			switch (parameters[i])
			{
			case AttributeConsoleManager.ConsoleParameter.Boolean:
			{
				bool flag;
				if (!bool.TryParse(args[i], out flag))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting bool but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = flag;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.Integer:
			{
				int num;
				if (!int.TryParse(args[i], out num))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting int but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = num;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.Float:
			{
				float num2;
				if (!float.TryParse(args[i], out num2))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting float but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = num2;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.Double:
			{
				double num3;
				if (!double.TryParse(args[i], out num3))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting double but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = num3;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.Byte:
			{
				byte b;
				if (!byte.TryParse(args[i], out b))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting byte but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = b;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.Long:
			{
				long num4;
				if (!long.TryParse(args[i], out num4))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting long but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = num4;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.Vector2:
			{
				Vector2 vector;
				if (!AttributeConsoleHelper.TryParseVector2(args[i], out vector))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting Vector2 but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = vector;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.Vector3:
			{
				Vector3 vector2;
				if (!AttributeConsoleHelper.TryParseVector3(args[i], out vector2))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting Vector3 but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = vector2;
				break;
			}
			case AttributeConsoleManager.ConsoleParameter.GameObject:
			{
				GameObject gameObject;
				if (!AttributeConsoleHelper.TryParseGameObject(args[i], out gameObject))
				{
					if (!silent)
					{
						this.DebugLog("Method Type Mismatch. Expecting GameObject but got " + args[i], AttributeConsoleManager.LogLevel.Warning);
					}
					return false;
				}
				array[i] = gameObject;
				break;
			}
			default:
				array[i] = args[i];
				break;
			}
		}
		if (func.methodInfo.IsStatic)
		{
			func.methodInfo.Invoke(null, array);
			return true;
		}
		Object[] array2 = Object.FindObjectsOfType(func.methodInfo.DeclaringType);
		if (array2.Length == 0)
		{
			this.DebugLog("Could not find any object in scene with the method " + func.command, AttributeConsoleManager.LogLevel.Warning);
			return false;
		}
		foreach (Object obj in array2)
		{
			func.methodInfo.Invoke(obj, array);
		}
		return true;
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000C1D8 File Offset: 0x0000A3D8
	private bool GetMethodParameters(MethodInfo info, out AttributeConsoleManager.ConsoleParameter[] parameters)
	{
		List<AttributeConsoleManager.ConsoleParameter> list = new List<AttributeConsoleManager.ConsoleParameter>();
		ParameterInfo[] parameters2 = info.GetParameters();
		for (int i = 0; i < parameters2.Length; i++)
		{
			Type parameterType = parameters2[i].ParameterType;
			if (parameterType.Equals(typeof(string)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.String);
			}
			else if (parameterType.Equals(typeof(bool)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Boolean);
			}
			else if (parameterType.Equals(typeof(int)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Integer);
			}
			else if (parameterType.Equals(typeof(float)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Float);
			}
			else if (parameterType.Equals(typeof(double)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Double);
			}
			else if (parameterType.Equals(typeof(byte)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Byte);
			}
			else if (parameterType.Equals(typeof(long)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Long);
			}
			else if (parameterType.Equals(typeof(Vector2)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Vector2);
			}
			else if (parameterType.Equals(typeof(Vector3)))
			{
				list.Add(AttributeConsoleManager.ConsoleParameter.Vector3);
			}
			else
			{
				if (!parameterType.Equals(typeof(GameObject)))
				{
					this.DebugLog(string.Concat(new object[]
					{
						"Cannot add type ",
						parameterType,
						" as its invalid in method ",
						info.Name
					}), AttributeConsoleManager.LogLevel.Error);
					parameters = null;
					return false;
				}
				list.Add(AttributeConsoleManager.ConsoleParameter.GameObject);
			}
		}
		parameters = list.ToArray();
		return true;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000C370 File Offset: 0x0000A570
	public void ClearLogs()
	{
		this.logs.Clear();
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000C37D File Offset: 0x0000A57D
	public ConsoleLog[] GetLogs()
	{
		return this.logs.ToArray();
	}

	// Token: 0x0600013C RID: 316 RVA: 0x0000C38A File Offset: 0x0000A58A
	public int GetLogSize()
	{
		return this.logs.Count;
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0000C397 File Offset: 0x0000A597
	private void OnUnityLog(string log, string stacktrace, LogType type)
	{
		this.OnLog(log, stacktrace, (ELogType)type);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0000C3A4 File Offset: 0x0000A5A4
	private void OnLog(string log, string stacktrace, ELogType type)
	{
		this.AddLog(new ConsoleLog
		{
			log = log,
			stacktrace = stacktrace,
			type = type,
			time = DateTime.UtcNow
		});
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0000C3E0 File Offset: 0x0000A5E0
	private void AddLog(ConsoleLog log)
	{
		this.logs.Add(log);
		if (this.maxLogs > 0 && this.logs.Count > this.maxLogs)
		{
			this.logs.RemoveAt(0);
		}
		if (this.streamOutput)
		{
			this.stream.WriteLine(log.GetPrefix() + log.GetMessage());
			if (log.type != ELogType.Log)
			{
				this.stream.WriteLine(log.GetStackTrace());
			}
			if (this.flushDelay > 0f)
			{
				this.flushTime = Time.realtimeSinceStartup + this.flushDelay;
			}
			else if (this.flushDelay == 0f)
			{
				this.stream.Flush();
			}
		}
		base.SendMessage("OnConsoleLog", this, SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000C4A5 File Offset: 0x0000A6A5
	private void DebugLog(object o, AttributeConsoleManager.LogLevel level = AttributeConsoleManager.LogLevel.Info)
	{
		if (level != AttributeConsoleManager.LogLevel.Warning)
		{
			if (level != AttributeConsoleManager.LogLevel.Error)
			{
				if (this.debugLogLevel <= AttributeConsoleManager.LogLevel.Info)
				{
					Debug.Log(o);
					return;
				}
			}
			else if (this.debugLogLevel <= AttributeConsoleManager.LogLevel.Error)
			{
				Debug.LogError(o);
			}
		}
		else if (this.debugLogLevel <= AttributeConsoleManager.LogLevel.Warning)
		{
			Debug.LogWarning(o);
			return;
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000C4E0 File Offset: 0x0000A6E0
	[ConsoleMethod("help", "Gives a list of methods")]
	private void Help()
	{
		string text = "Help List:";
		foreach (AttributeConsoleManager.ConsoleFunction consoleFunction in this.functions)
		{
			if (!this.checkForAvaliablity || Object.FindObjectsOfType(consoleFunction.methodInfo.DeclaringType).Length != 0)
			{
				text = text + "\n" + consoleFunction.ToString();
			}
		}
		Debug.Log(text);
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000C568 File Offset: 0x0000A768
	[ConsoleMethod("help", "Gives help to a specific method.")]
	private void Help(string command)
	{
		AttributeConsoleManager.ConsoleFunction[] array = (from f in this.functions
		where f.command.Equals(command)
		select f).ToArray<AttributeConsoleManager.ConsoleFunction>();
		if (array.Length == 0)
		{
			Debug.Log("Cannot give help on '" + command + "' as it does not exist");
			return;
		}
		string text = "Help avaliable on " + command + ": ";
		foreach (AttributeConsoleManager.ConsoleFunction consoleFunction in array)
		{
			text = string.Concat(new string[]
			{
				text,
				"\n",
				consoleFunction.ToString(),
				" - ",
				consoleFunction.help
			});
		}
		Debug.Log(text);
	}

	// Token: 0x06000143 RID: 323 RVA: 0x0000C370 File Offset: 0x0000A570
	[ConsoleMethod("clear", "Clears all the logs. Does not effect the stream.")]
	private void Clear()
	{
		this.logs.Clear();
	}

	// Token: 0x040001F6 RID: 502
	public AttributeConsoleManager.LogLevel debugLogLevel = AttributeConsoleManager.LogLevel.Warning;

	// Token: 0x040001F7 RID: 503
	private static AttributeConsoleManager _instance;

	// Token: 0x040001F8 RID: 504
	public bool streamOutput = true;

	// Token: 0x040001F9 RID: 505
	private StreamWriter stream;

	// Token: 0x040001FA RID: 506
	public float flushDelay = 1f;

	// Token: 0x040001FB RID: 507
	private float flushTime;

	// Token: 0x040001FC RID: 508
	public string streamFile = "/console.txt";

	// Token: 0x040001FD RID: 509
	private List<AttributeConsoleManager.ConsoleFunction> functions = new List<AttributeConsoleManager.ConsoleFunction>();

	// Token: 0x040001FE RID: 510
	private List<ConsoleLog> logs = new List<ConsoleLog>();

	// Token: 0x040001FF RID: 511
	public int maxLogs = 200;

	// Token: 0x04000200 RID: 512
	public bool automaticlyFetchMethods = true;

	// Token: 0x04000201 RID: 513
	public bool checkForAvaliablity = true;

	// Token: 0x020004F2 RID: 1266
	public enum LogLevel
	{
		// Token: 0x04002E45 RID: 11845
		Info,
		// Token: 0x04002E46 RID: 11846
		Warning,
		// Token: 0x04002E47 RID: 11847
		Error
	}

	// Token: 0x020004F3 RID: 1267
	private class ConsoleFunction
	{
		// Token: 0x06004252 RID: 16978 RVA: 0x001F8A68 File Offset: 0x001F6C68
		public override string ToString()
		{
			string text = "";
			for (int i = 0; i < this.parameters.Length; i++)
			{
				text = text + ((i == 0) ? "" : ", ") + this.parameters[i].ToString();
			}
			return this.command + " (" + text + ") ";
		}

		// Token: 0x04002E48 RID: 11848
		public MethodInfo methodInfo;

		// Token: 0x04002E49 RID: 11849
		public string command;

		// Token: 0x04002E4A RID: 11850
		public string help;

		// Token: 0x04002E4B RID: 11851
		public AttributeConsoleManager.ConsoleParameter[] parameters;
	}

	// Token: 0x020004F4 RID: 1268
	private enum ConsoleParameter
	{
		// Token: 0x04002E4D RID: 11853
		String,
		// Token: 0x04002E4E RID: 11854
		Boolean,
		// Token: 0x04002E4F RID: 11855
		Integer,
		// Token: 0x04002E50 RID: 11856
		Float,
		// Token: 0x04002E51 RID: 11857
		Double,
		// Token: 0x04002E52 RID: 11858
		Byte,
		// Token: 0x04002E53 RID: 11859
		Long,
		// Token: 0x04002E54 RID: 11860
		Vector2,
		// Token: 0x04002E55 RID: 11861
		Vector3,
		// Token: 0x04002E56 RID: 11862
		GameObject
	}
}
