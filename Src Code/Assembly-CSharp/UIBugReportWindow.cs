using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Logic;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200029B RID: 667
public class UIBugReportWindow : MonoBehaviour
{
	// Token: 0x170001FD RID: 509
	// (get) Token: 0x0600291E RID: 10526 RVA: 0x0015E37B File Offset: 0x0015C57B
	// (set) Token: 0x0600291F RID: 10527 RVA: 0x0015E382 File Offset: 0x0015C582
	public static UIBugReportWindow current { get; private set; }

	// Token: 0x06002920 RID: 10528 RVA: 0x0015E38C File Offset: 0x0015C58C
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_Close != null)
		{
			this.m_Close.onClick = delegate(BSGButton b)
			{
				this.Close();
			};
		}
		if (this.m_Browse != null)
		{
			this.m_Browse.onClick = delegate(BSGButton b)
			{
				this.Browse();
			};
		}
		if (this.m_btnCapture != null)
		{
			this.m_btnCapture.onClick = delegate(BSGButton b)
			{
				this.OnCaptureGame();
			};
		}
		if (this.m_btnOpenDestination != null)
		{
			this.m_btnOpenDestination.onClick = delegate(BSGButton b)
			{
				this.OpenCaptureDestinationFolder();
			};
		}
		if (this.m_OpenOnCreate != null)
		{
			this.m_OpenOnCreate.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleOpenOnCreate));
		}
		if (this.m_ReprotPathTextField != null)
		{
			TMP_InputField reprotPathTextField = this.m_ReprotPathTextField;
			reprotPathTextField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(reprotPathTextField.onValidateInput, new TMP_InputField.OnValidateInput((string input, int charIndex, char addedChar) => this.ValidateReportNameInput(input, charIndex, addedChar)));
		}
		this.m_Initialzied = true;
		this.Build();
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000023FD File Offset: 0x000005FD
	private void LocalizeStatics()
	{
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x0015E4AC File Offset: 0x0015C6AC
	public void Build()
	{
		this.LocalizeStatics();
		Game game = GameLogic.Get(false);
		if (game != null)
		{
			Pause pause = game.pause;
			if (pause != null)
			{
				pause.AddRequest("BugReportPause", -2);
			}
		}
		if (this.m_inputDirFieldText != null)
		{
			TextMeshProUGUI component = this.m_inputDirFieldText.placeholder.GetComponent<TextMeshProUGUI>();
			if (component != null)
			{
				UIText.SetText(component, GameCapture.CaptureDefaultDirectory());
			}
			this.m_inputDirFieldText.onValueChanged.AddListener(new UnityAction<string>(this.ValidateDirInputValue));
		}
		if (this.m_OpenOnCreate != null)
		{
			this.m_OpenOnCreate.SetIsOnWithoutNotify(PlayerPrefs.GetInt("BSG_BugReport_OpenOnCreate") == 1);
		}
		base.StartCoroutine(this.CaptureScreenShot());
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x0015E564 File Offset: 0x0015C764
	public void Close()
	{
		UserInteractionLogger.LogNewLine("Close save/load menu.");
		Game game = GameLogic.Get(true);
		CameraController cameraController = CameraController.Get();
		List<GameCamera> list = (cameraController != null) ? cameraController.AllCameras : null;
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Lock(false);
			}
		}
		if (game != null)
		{
			Pause pause = game.pause;
			if (pause != null)
			{
				pause.DelRequest("BugReportPause", -2);
			}
		}
		if (this.screenShot != null)
		{
			UnityEngine.Object.Destroy(this.screenShot);
			this.screenShot = null;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x0015E5FB File Offset: 0x0015C7FB
	private void OnDisable()
	{
		UIBugReportWindow.current = null;
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x0015E603 File Offset: 0x0015C803
	private IEnumerator BrowseDirectory()
	{
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, null, null, "Select Directory", "Select");
		Debug.Log(FileBrowser.Success);
		if (FileBrowser.Success)
		{
			for (int i = 0; i < FileBrowser.Result.Length; i++)
			{
				Debug.Log(FileBrowser.Result[i]);
				this.m_inputDirFieldText.text = FileBrowser.Result[i];
				PlayerPrefs.SetString("BSG_BugReport_DirDestination", FileBrowser.Result[i]);
			}
		}
		yield break;
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x0015E612 File Offset: 0x0015C812
	public void Browse()
	{
		base.StartCoroutine(this.BrowseDirectory());
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x0015E624 File Offset: 0x0015C824
	public static int CountWords(string test)
	{
		int num = 0;
		bool flag = false;
		for (int i = 0; i < test.Length; i++)
		{
			if (char.IsWhiteSpace(test[i]))
			{
				flag = false;
			}
			else
			{
				if (!flag)
				{
					num++;
				}
				flag = true;
			}
		}
		return num;
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x0015E664 File Offset: 0x0015C864
	private void OnCaptureGame()
	{
		if (string.IsNullOrEmpty(this.m_DescrActualFieldText.text) || UIBugReportWindow.CountWords(this.m_DescrActualFieldText.text) == 0)
		{
			Vars vars = new Vars();
			vars.Set<string>("caption", "#No issue description");
			vars.Set<string>("body", "#Please describe the problem.");
			MessageWnd messageWnd = MessageWnd.Create("DebugMessage", vars, null, delegate(MessageWnd wnd, string btn_id)
			{
				wnd.Close(false);
				return true;
			});
			if (messageWnd != null)
			{
				messageWnd.transform.SetParent(base.transform);
			}
			return;
		}
		if (UIBugReportWindow.CountWords(this.m_DescrActualFieldText.text) == 1)
		{
			Vars vars2 = new Vars();
			vars2.Set<string>("caption", "#Insufficient description");
			vars2.Set<string>("body", "#Only a '" + this.m_DescrActualFieldText.text + "' is not enough for a description. \nEven if its clear to you what the problem is, its not for someone without the context you have from the last few seconds/minutes of gameplay. We lose time trying to figure out what happened when we don't know that context. \nPlease, go back and write a description. The more details the better. What happened, what might have lead to it, etc.");
			MessageWnd messageWnd2 = MessageWnd.Create("DebugMessage", vars2, null, delegate(MessageWnd wnd, string btn_id)
			{
				wnd.Close(false);
				return true;
			});
			if (messageWnd2 != null)
			{
				messageWnd2.transform.SetParent(base.transform);
			}
			return;
		}
		string.IsNullOrEmpty(this.m_DescrExpectedFieldText.text);
		string.IsNullOrEmpty(this.m_DescrReproduceFieldText.text);
		UserInteractionLogger.LogNewLine("Clicked Capture");
		string text = this.m_inputDirFieldText.text;
		if (string.IsNullOrEmpty(text))
		{
			text = this.m_inputDirFieldText.placeholder.GetComponent<TextMeshProUGUI>().text;
		}
		string text2 = this.m_ReprotPathTextField.text;
		if (string.IsNullOrEmpty(text2))
		{
			text2 = this.m_ReprotPathTextField.placeholder.GetComponent<TextMeshProUGUI>().text;
		}
		this.background.SetActive(false);
		this.m_Menu.SetActive(false);
		GameCapture.Capture(text, text2, this.screenShot, this.m_DescrActualFieldText.text, this.m_DescrExpectedFieldText.text, this.m_DescrReproduceFieldText.text, this.m_OpenOnCreate != null && this.m_OpenOnCreate.isOn);
		this.Close();
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x0015E88C File Offset: 0x0015CA8C
	private char ValidateDirNameInput(char charToValidate)
	{
		char[] invalidPathChars = Path.GetInvalidPathChars();
		int num = invalidPathChars.Length;
		for (int i = 0; i < num; i++)
		{
			if (invalidPathChars[i] == charToValidate)
			{
				return '\0';
			}
		}
		return charToValidate;
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x0015E8B8 File Offset: 0x0015CAB8
	private char ValidateReportNameInput(string input, int charIndex, char charToValidate)
	{
		if (input.Length >= 50)
		{
			return '\0';
		}
		char[] invalidPathChars = Path.GetInvalidPathChars();
		int num = invalidPathChars.Length;
		for (int i = 0; i < num; i++)
		{
			if (invalidPathChars[i] == charToValidate)
			{
				return '\0';
			}
		}
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		num = invalidFileNameChars.Length;
		for (int j = 0; j < num; j++)
		{
			if (invalidFileNameChars[j] == charToValidate)
			{
				return '\0';
			}
		}
		return charToValidate;
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x0015E914 File Offset: 0x0015CB14
	private void OpenCaptureDestinationFolder()
	{
		string text = this.m_inputDirFieldText.text;
		if (!Directory.Exists(text))
		{
			text = this.m_inputDirFieldText.placeholder.GetComponent<TextMeshProUGUI>().text;
		}
		if (Directory.Exists(text))
		{
			Process.Start(new ProcessStartInfo
			{
				Arguments = text,
				FileName = "explorer.exe"
			});
		}
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x0015E970 File Offset: 0x0015CB70
	private void ToggleOpenOnCreate(bool toggled)
	{
		PlayerPrefs.SetInt("BSG_BugReport_OpenOnCreate", toggled ? 1 : 0);
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x0015E984 File Offset: 0x0015CB84
	private void ValidateDirInputValue(string s)
	{
		if (Directory.Exists(this.m_inputDirFieldText.text))
		{
			this.m_inputDirFieldText.textComponent.color = new Color(1f, 1f, 1f);
			return;
		}
		this.m_inputDirFieldText.textComponent.color = new Color(1f, 0f, 0f);
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x0015E9EC File Offset: 0x0015CBEC
	private IEnumerator CaptureScreenShot()
	{
		this.background.SetActive(false);
		this.m_Menu.SetActive(false);
		yield return new WaitForEndOfFrame();
		this.screenShot = ScreenCapture.CaptureScreenshotAsTexture();
		UIBugReportWindow.RemoveAlphaChannel(this.screenShot);
		this.background.SetActive(true);
		this.m_Menu.SetActive(true);
		yield break;
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x0015E9FC File Offset: 0x0015CBFC
	private static void RemoveAlphaChannel(Texture2D texture)
	{
		Color32[] pixels = texture.GetPixels32(0);
		for (int i = 0; i < pixels.Length; i++)
		{
			Color32 color = pixels[i];
			color.a = byte.MaxValue;
			pixels[i] = color;
		}
		texture.SetPixels32(pixels, 0);
		texture.Apply();
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x0015EA49 File Offset: 0x0015CC49
	public static bool IsActive()
	{
		return UIBugReportWindow.current != null;
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x0015EA58 File Offset: 0x0015CC58
	public static UIBugReportWindow Create()
	{
		CameraController cameraController = CameraController.Get();
		List<GameCamera> list = (cameraController != null) ? cameraController.AllCameras : null;
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Lock(true);
			}
		}
		UserInteractionLogger.LogNewLine("Create Report Tool menu");
		if (UIBugReportWindow.current != null)
		{
			UIBugReportWindow.current.Close();
		}
		GameObject prefab = UICommon.GetPrefab("BugReportWindow", null);
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(prefab, false, false);
		gameObject.name = "BugReportWindow";
		UIBugReportWindow uibugReportWindow = gameObject.GetOrAddComponent<UIBugReportWindow>();
		if (uibugReportWindow == null)
		{
			uibugReportWindow = gameObject.AddComponent<UIBugReportWindow>();
		}
		uibugReportWindow.Init();
		UIBugReportWindow.current = uibugReportWindow;
		return uibugReportWindow;
	}

	// Token: 0x04001BDD RID: 7133
	[UIFieldTarget("id_PointerBlocker")]
	private GameObject background;

	// Token: 0x04001BDE RID: 7134
	[UIFieldTarget("id_Menu")]
	private GameObject m_Menu;

	// Token: 0x04001BDF RID: 7135
	[UIFieldTarget("id_TitleLabel")]
	private TextMeshProUGUI m_TitleLabel;

	// Token: 0x04001BE0 RID: 7136
	[UIFieldTarget("id_Close")]
	private BSGButton m_Close;

	// Token: 0x04001BE1 RID: 7137
	[UIFieldTarget("btn_Capture")]
	private BSGButton m_btnCapture;

	// Token: 0x04001BE2 RID: 7138
	[UIFieldTarget("id_CaptureLabel")]
	private TextMeshProUGUI m_CaptureLabel;

	// Token: 0x04001BE3 RID: 7139
	[UIFieldTarget("id_Browse")]
	private BSGButton m_Browse;

	// Token: 0x04001BE4 RID: 7140
	[UIFieldTarget("id_OpenOnCreate")]
	private Toggle m_OpenOnCreate;

	// Token: 0x04001BE5 RID: 7141
	[UIFieldTarget("id_OpenDestination")]
	private BSGButton m_btnOpenDestination;

	// Token: 0x04001BE6 RID: 7142
	[UIFieldTarget("id_ReprotPathTextField")]
	private TMP_InputField m_ReprotPathTextField;

	// Token: 0x04001BE7 RID: 7143
	[UIFieldTarget("id_InputDirFieldText")]
	private TMP_InputField m_inputDirFieldText;

	// Token: 0x04001BE8 RID: 7144
	[UIFieldTarget("id_DescrActualFieldText")]
	private TMP_InputField m_DescrActualFieldText;

	// Token: 0x04001BE9 RID: 7145
	[UIFieldTarget("id_DescrExpectedFieldText")]
	private TMP_InputField m_DescrExpectedFieldText;

	// Token: 0x04001BEA RID: 7146
	[UIFieldTarget("id_DescrReproduceFieldText")]
	private TMP_InputField m_DescrReproduceFieldText;

	// Token: 0x04001BEB RID: 7147
	[UIFieldTarget("id_WarringLabel")]
	private TextMeshProUGUI m_WarringLabel;

	// Token: 0x04001BEC RID: 7148
	private Texture2D screenShot;

	// Token: 0x04001BED RID: 7149
	private bool m_Initialzied;
}
