using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000035 RID: 53
[RequireComponent(typeof(AttributeConsoleManager))]
public class AttributeConsoleGUI : MonoBehaviour
{
	// Token: 0x06000120 RID: 288 RVA: 0x0000B055 File Offset: 0x00009255
	private void Awake()
	{
		this.manager = base.GetComponent<AttributeConsoleManager>();
	}

	// Token: 0x06000121 RID: 289 RVA: 0x0000B064 File Offset: 0x00009264
	private void Update()
	{
		if (UICommon.GetKeyDown(this.displayKey, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
		{
			this.display = !this.display;
		}
		if (this.display && !Title.IsInternalBranch())
		{
			string commandLine = Environment.CommandLine;
			if (commandLine == null || commandLine.IndexOf("--ForceEnableConsole") < 0)
			{
				this.display = false;
			}
		}
		if (this.objectFetching && this.display)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, float.PositiveInfinity, this.objectFetchingMask))
			{
				this.objectFetched = true;
				this.objectFetchedID = raycastHit.collider.gameObject.GetInstanceID();
				if (Input.GetMouseButtonDown(0))
				{
					this.input = this.input + this.objectFetchedID + " ";
					return;
				}
			}
			else
			{
				this.objectFetched = false;
			}
		}
	}

	// Token: 0x06000122 RID: 290 RVA: 0x0000B144 File Offset: 0x00009344
	private void OnGUI()
	{
		if (!this.display)
		{
			return;
		}
		Event current = Event.current;
		GUI.skin = this.skin;
		int depth = GUI.depth;
		GUI.depth = 1000000000;
		GUI.skin.box.normal.background = this.bgr;
		GUI.skin.textField.normal.background = this.edit_bgr;
		GUI.skin.textField.focused.background = this.edit_bgr;
		GUI.skin.textField.hover.background = this.edit_bgr;
		GUI.Box(new Rect(0f, 0f, (float)Screen.width, (float)(Screen.height / 3)), "");
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Height((float)(Screen.height / 3)),
			GUILayout.Width((float)Screen.width)
		});
		ConsoleLog[] logs = this.manager.GetLogs();
		for (int i = 0; i < logs.Length; i++)
		{
			this.DrawLogElement(logs[i]);
		}
		GUILayout.EndScrollView();
		if (current.type == EventType.KeyDown && current.isKey)
		{
			KeyCode keyCode = current.keyCode;
			if (GUI.GetNameOfFocusedControl() != "ConsoleInput")
			{
				GUI.FocusControl("ConsoleInput");
			}
			if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter)
			{
				this.manager.AttemptExecute(this.input);
				this.AddHistory(this.input);
				this.input = "";
			}
			if (keyCode == KeyCode.UpArrow)
			{
				this.input = this.GetHistory();
			}
			if (keyCode == KeyCode.DownArrow)
			{
				this.input = this.GetPreviousHistory();
			}
			if (keyCode == this.displayKey)
			{
				this.display = false;
				char character = current.character;
				if (char.IsLetterOrDigit(character) || char.IsPunctuation(character))
				{
					this.input = this.input.Remove(this.input.Length - 1);
				}
			}
		}
		GUI.SetNextControlName("ConsoleInput");
		this.input = GUI.TextField(new Rect(0f, (float)(Screen.height / 3), (float)Screen.width, 25f), this.input);
		if (this.objectFetching)
		{
			string str = this.objectFetched ? this.objectFetchedID.ToString() : "";
			GUIContent content = new GUIContent("[ " + str + " ]");
			Vector2 vector = GUI.skin.label.CalcSize(content);
			GUI.Box(new Rect((float)Screen.width / 2f - vector.x / 2f - 5f, (float)Screen.height - 50f, vector.x + 10f, vector.y + 2f), new GUIContent());
			GUI.Label(new Rect((float)Screen.width / 2f - vector.x / 2f, (float)(Screen.height - 50), vector.x, vector.y), content);
		}
		GUI.depth = depth;
	}

	// Token: 0x06000123 RID: 291 RVA: 0x0000B470 File Offset: 0x00009670
	private void DrawLogElement(ConsoleLog log)
	{
		GUI.color = ((log.type == ELogType.Log) ? Color.white : ((log.type == ELogType.Warning) ? Color.yellow : ((log.type == ELogType.Command) ? Color.gray : Color.red)));
		GUILayout.BeginHorizontal(new GUILayoutOption[]
		{
			GUILayout.Height(20f)
		});
		GUILayout.Space(10f);
		GUILayout.Label(log.GetPrefix() + log.GetMessage(), Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUI.color = Color.white;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x0000B503 File Offset: 0x00009703
	private void AddHistory(string s)
	{
		this.history.Add(s);
		this.historyDepth = 0;
	}

	// Token: 0x06000125 RID: 293 RVA: 0x0000B518 File Offset: 0x00009718
	private string GetHistory()
	{
		int num = Mathf.Max(0, this.history.Count - 1 - this.historyDepth);
		int value = this.historyDepth + 1;
		this.historyDepth = value;
		this.historyDepth = Mathf.Clamp(value, 0, this.history.Count - 1);
		if (num >= this.history.Count)
		{
			return "";
		}
		return this.history[num];
	}

	// Token: 0x06000126 RID: 294 RVA: 0x0000B58C File Offset: 0x0000978C
	private string GetPreviousHistory()
	{
		int num = Mathf.Max(0, this.history.Count - 1 - this.historyDepth);
		int value = this.historyDepth - 1;
		this.historyDepth = value;
		this.historyDepth = Mathf.Clamp(value, 0, this.history.Count - 1);
		if (num >= this.history.Count)
		{
			return "";
		}
		return this.history[num];
	}

	// Token: 0x06000127 RID: 295 RVA: 0x0000B5FE File Offset: 0x000097FE
	[ConsoleMethod("clearHistory", "Clears your command history")]
	private void ClearHistory()
	{
		this.historyDepth = 0;
		this.history.Clear();
	}

	// Token: 0x06000128 RID: 296 RVA: 0x0000B612 File Offset: 0x00009812
	private void OnConsoleLog(AttributeConsoleManager manager)
	{
		this.scrollPosition.y = (float)(1000 * manager.GetLogSize());
	}

	// Token: 0x040001E8 RID: 488
	private AttributeConsoleManager manager;

	// Token: 0x040001E9 RID: 489
	public GUISkin skin;

	// Token: 0x040001EA RID: 490
	public KeyCode displayKey = KeyCode.BackQuote;

	// Token: 0x040001EB RID: 491
	public LayerMask objectFetchingMask = -1;

	// Token: 0x040001EC RID: 492
	private bool objectFetching;

	// Token: 0x040001ED RID: 493
	public bool display;

	// Token: 0x040001EE RID: 494
	private int objectFetchedID = -1;

	// Token: 0x040001EF RID: 495
	private bool objectFetched;

	// Token: 0x040001F0 RID: 496
	private string input = "";

	// Token: 0x040001F1 RID: 497
	private List<string> history = new List<string>();

	// Token: 0x040001F2 RID: 498
	private int historyDepth;

	// Token: 0x040001F3 RID: 499
	private Vector2 scrollPosition = Vector2.zero;

	// Token: 0x040001F4 RID: 500
	public Texture2D bgr;

	// Token: 0x040001F5 RID: 501
	public Texture2D edit_bgr;
}
