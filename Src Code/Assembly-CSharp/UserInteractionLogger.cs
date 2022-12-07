using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000156 RID: 342
public static class UserInteractionLogger
{
	// Token: 0x06001188 RID: 4488 RVA: 0x000B8337 File Offset: 0x000B6537
	public static void StartLogging()
	{
		UserInteractionLoggerLogic.StartLogging();
	}

	// Token: 0x06001189 RID: 4489 RVA: 0x000B833E File Offset: 0x000B653E
	public static void Log(BSGButton button, string message = null)
	{
		UserInteractionLogger.Log(UserInteractionLogger.PrepareButtonMessage(button, message));
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x000B834C File Offset: 0x000B654C
	public static void LogNewLine(BSGButton button, string message = null)
	{
		UserInteractionLogger.LogNewLine(UserInteractionLogger.PrepareButtonMessage(button, message));
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x000B835A File Offset: 0x000B655A
	public static void Log(string text = null)
	{
		UserInteractionLoggerLogic.Log(text);
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x000B8362 File Offset: 0x000B6562
	public static void LogNewLine(string text = null)
	{
		UserInteractionLoggerLogic.LogNewLine(text);
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x000B836C File Offset: 0x000B656C
	public static string PrepareButtonMessage(BSGButton button, string message = null)
	{
		if (button == null)
		{
			UserInteractionLogger.Error("BSGButton is null!");
			return null;
		}
		List<string> list = new List<string>();
		string text = string.Empty;
		string text2 = string.Empty;
		Transform transform = button.transform;
		if (transform != null)
		{
			Transform parent = transform.parent;
			while (parent != null && parent.name != "UI")
			{
				list.Add(parent.name);
				parent = parent.parent;
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				text = text + list[i] + "\\";
			}
			if (transform.childCount > 0)
			{
				Transform child = transform.GetChild(0);
				if (child != null)
				{
					TMP_Text component = child.GetComponent<TMP_Text>();
					if (component != null)
					{
						text2 += component.text;
					}
				}
			}
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = button.name;
		}
		string text3 = string.Concat(new string[]
		{
			"Clicked ",
			text2,
			" (",
			text,
			text2,
			")"
		});
		if (!string.IsNullOrEmpty(message))
		{
			text3 = text3 + " - " + message;
		}
		return text3;
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x000B84B1 File Offset: 0x000B66B1
	private static void Warning(string message)
	{
		Debug.LogWarning(message);
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x000B84B9 File Offset: 0x000B66B9
	private static void Error(string message)
	{
		Debug.LogError(message);
	}
}
