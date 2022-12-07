using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020002D8 RID: 728
public class TitleErrorWindow : MonoBehaviour
{
	// Token: 0x06002DFB RID: 11771 RVA: 0x0017CE1C File Offset: 0x0017B01C
	public static TitleErrorWindow Get()
	{
		if (TitleErrorWindow.sm_Instance == null)
		{
			DT.Field defField = global::Defs.GetDefField("TitleErrorWindow", null);
			if (defField != null)
			{
				GameObject obj = global::Defs.GetObj<GameObject>(defField, "window_prefab", null);
				if (obj != null)
				{
					Transform transform = GameObject.Find("Canvas").transform;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(obj, transform);
					TitleErrorWindow component = gameObject.GetComponent<TitleErrorWindow>();
					if (component != null)
					{
						TitleErrorWindow.sm_Instance = component;
						TitleErrorWindow.sm_Instance.gameObject.SetActive(false);
					}
					else
					{
						Debug.Log("ErrorWindow: provided prefab is not eligable");
						UnityEngine.Object.Destroy(gameObject);
					}
				}
				else
				{
					Debug.Log("ErrorWindow: fail to find def field \"TitleErrorWindow\"");
				}
			}
		}
		return TitleErrorWindow.sm_Instance;
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x0017CEC4 File Offset: 0x0017B0C4
	private void OnOkay(BSGButton btn)
	{
		this.Show(false, null, null, null);
	}

	// Token: 0x06002DFD RID: 11773 RVA: 0x0017CED0 File Offset: 0x0017B0D0
	private void SetCaption(string key)
	{
		UIText.SetTextKey(global::Common.FindChildComponent<TextMeshProUGUI>(TitleErrorWindow.sm_Instance.gameObject, "id_Caption"), key, null, null);
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x0017CEEE File Offset: 0x0017B0EE
	private void SetMessage1(string key)
	{
		UIText.SetTextKey(global::Common.FindChildComponent<TextMeshProUGUI>(TitleErrorWindow.sm_Instance.gameObject, "id_Message1"), key, null, null);
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x0017CF0C File Offset: 0x0017B10C
	private void SetMessage2(string key)
	{
		UIText.SetTextKey(global::Common.FindChildComponent<TextMeshProUGUI>(TitleErrorWindow.sm_Instance.gameObject, "id_Message2"), key, null, null);
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x0017CF2C File Offset: 0x0017B12C
	public void Show(bool show, string captionKey = null, string message1Key = null, string message2Key = null)
	{
		if (show)
		{
			BSGButton bsgbutton = global::Common.FindChildComponent<BSGButton>(TitleErrorWindow.sm_Instance.gameObject, "btn_Okay");
			if (bsgbutton != null)
			{
				bsgbutton.onClick = new BSGButton.OnClick(this.OnOkay);
			}
			else
			{
				Debug.LogWarning("Okay button missing!");
			}
			this.SetCaption(captionKey);
			this.SetMessage1(message1Key);
			this.SetMessage2(message2Key);
		}
		else if (!string.IsNullOrEmpty(captionKey) || !string.IsNullOrEmpty(message1Key) || !string.IsNullOrEmpty(message2Key))
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"Hiding title error window but providing caption: ",
				captionKey,
				", message1: ",
				message1Key,
				", message2: ",
				message2Key
			}));
		}
		TitleErrorWindow.sm_Instance.gameObject.SetActive(show);
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x0017CFEE File Offset: 0x0017B1EE
	public static bool IsShown()
	{
		return !(TitleErrorWindow.sm_Instance == null) && TitleErrorWindow.sm_Instance.gameObject.activeInHierarchy;
	}

	// Token: 0x04001F26 RID: 7974
	private static TitleErrorWindow sm_Instance;
}
