using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000253 RID: 595
public class UICredits : UIWindow
{
	// Token: 0x06002469 RID: 9321 RVA: 0x001463A9 File Offset: 0x001445A9
	public override string GetDefId()
	{
		return UICredits.wnd_def_id;
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x001463B0 File Offset: 0x001445B0
	private void Init()
	{
		if (this.m_Initialized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.id_Back.onClick = new BSGButton.OnClick(this.HandleClose);
		BSGScrollRect bsgscrollRect = this.id_ScrollView;
		bsgscrollRect.OnMouseScroll = (Action<PointerEventData>)Delegate.Combine(bsgscrollRect.OnMouseScroll, new Action<PointerEventData>(this.HandleOnMouseScroll));
		this.m_Initialized = true;
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x00146414 File Offset: 0x00144614
	private void OnEnable()
	{
		this.Init();
		if (Application.isPlaying)
		{
			if (this.id_ScrollView != null)
			{
				this.id_ScrollView.gameObject.SetActive(true);
			}
			if (this.id_Styles != null)
			{
				this.id_Styles.gameObject.SetActive(false);
			}
			if (this.id_BackLabel != null)
			{
				UIText.SetTextKey(this.id_BackLabel, "CreditsWindow.back", null, null);
			}
		}
		if (!this.PopulateContainer())
		{
			GameObject gameObject = this.id_CreditsContainer;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		if (this.id_ScrollView != null && Application.isPlaying)
		{
			float height = (this.id_ScrollView.transform as RectTransform).rect.height;
			VerticalLayoutGroup component = this.id_ScrollView.content.GetComponent<VerticalLayoutGroup>();
			if (component != null)
			{
				component.padding = new RectOffset(0, 0, (int)height, (int)height);
			}
			this.id_ScrollView.verticalNormalizedPosition = 1f;
			this.scrollSpeed = this.window_def.GetFloat("scroll_speed", null, this.scrollSpeed, true, true, true, '.');
			this.userInteractionTimeoutDuration = this.window_def.GetFloat("autoscroll_restart_duration", null, this.userInteractionTimeoutDuration, true, true, true, '.');
		}
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x0014655E File Offset: 0x0014475E
	public override void Show()
	{
		base.Show();
		base.gameObject.SetActive(true);
		this.isScrolling = true;
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x0014657C File Offset: 0x0014477C
	protected override void Update()
	{
		base.Update();
		if (UICommon.GetKey(KeyCode.Space, false))
		{
			this.InteruptScroll();
		}
		if (UICommon.GetKey(KeyCode.Mouse0, false))
		{
			this.InteruptScroll();
		}
		if (UICommon.GetKey(KeyCode.Home, false))
		{
			this.id_ScrollView.verticalNormalizedPosition = 1f;
			this.isScrolling = true;
		}
		if (!this.isScrolling)
		{
			this.isScrolling = (this.lastUserInteraction + this.userInteractionTimeoutDuration < UnityEngine.Time.unscaledTime);
		}
		if (this.isScrolling)
		{
			this.id_ScrollView.verticalNormalizedPosition = this.id_ScrollView.verticalNormalizedPosition - UnityEngine.Time.unscaledDeltaTime * this.scrollSpeed;
			if (this.id_ScrollView.verticalNormalizedPosition <= 0f)
			{
				this.Close(false);
			}
		}
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x0014663C File Offset: 0x0014483C
	private void InteruptScroll()
	{
		this.lastUserInteraction = UnityEngine.Time.unscaledTime;
		this.isScrolling = false;
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x00146650 File Offset: 0x00144850
	private void HandleOnMouseScroll(PointerEventData eventData)
	{
		this.InteruptScroll();
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x0011FFF8 File Offset: 0x0011E1F8
	private void HandleClose(BSGButton b)
	{
		this.Close(false);
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x00146658 File Offset: 0x00144858
	public override void Close(bool silent = false)
	{
		BaseUI.Get<TitleUI>().DisableCreditsWindow();
		if (this.on_close != null)
		{
			this.on_close(this);
		}
		this.Hide(silent);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x0014668C File Offset: 0x0014488C
	private bool LoadStyles()
	{
		this.styles.Clear();
		if (this.id_Styles == null)
		{
			return false;
		}
		Transform transform = this.id_Styles.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			this.styles.Add(gameObject.name, gameObject);
		}
		return true;
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x001466F4 File Offset: 0x001448F4
	private bool PopulateContainer()
	{
		if (this.id_CreditsContainer == null)
		{
			return false;
		}
		UICommon.DeleteChildren(this.id_CreditsContainer.transform);
		if (!this.LoadStyles())
		{
			return false;
		}
		string text = global::Defs.texts_path + "dev/credits.txt";
		string text2 = DT.ReadTextFile(text);
		if (text2 == null)
		{
			return false;
		}
		this.cur_pos.text = text2;
		this.cur_pos.filename = text;
		this.cur_pos.line = 1;
		this.cur_pos.line_idx = 0;
		this.cur_pos.idx = 0;
		this.cur_style = null;
		this.cur_prefab = null;
		while (this.cur_pos.Valid)
		{
			if (this.cur_pos.Skip("#", true))
			{
				this.ParseStyle();
			}
			else if (this.cur_pos.Skip("//", false))
			{
				this.cur_pos.SkipLine(false);
			}
			else
			{
				this.ParseText();
			}
		}
		return true;
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x001467E4 File Offset: 0x001449E4
	private void ParseStyle()
	{
		this.cur_style = this.cur_pos.ReadIdentifier(true);
		this.cur_pos.SkipLine(false);
		if (string.IsNullOrEmpty(this.cur_style))
		{
			this.Error("Identifier expected (style name)");
			return;
		}
		if (!this.styles.TryGetValue(this.cur_style, out this.cur_prefab))
		{
			this.Error("Unknown style: '" + this.cur_style + "'");
			return;
		}
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x00146860 File Offset: 0x00144A60
	private void ParseText()
	{
		if (this.cur_style.StartsWith("PositionAndName", StringComparison.OrdinalIgnoreCase))
		{
			this.ParsePositionAndNameText();
			return;
		}
		if (this.cur_style.StartsWith("TwoNames", StringComparison.OrdinalIgnoreCase))
		{
			this.ParseTwoNamesText();
			return;
		}
		int idx = this.cur_pos.idx;
		while (this.cur_pos.Valid && this.cur_pos.Char(0) != '#')
		{
			if (this.cur_pos.Skip("//", false))
			{
				this.cur_pos.idx = this.cur_pos.line_idx;
				break;
			}
			this.cur_pos.SkipLine(false);
		}
		if (this.cur_pos.idx == idx)
		{
			return;
		}
		GameObject gameObject = this.SpawnRow();
		TextMeshProUGUI textMeshProUGUI = (gameObject != null) ? gameObject.GetComponent<TextMeshProUGUI>() : null;
		if (textMeshProUGUI == null)
		{
			return;
		}
		int num = this.cur_pos.idx;
		if (num > idx)
		{
			char c = this.cur_pos.text[num - 1];
			if (c == '\r' || c == '\n')
			{
				num--;
				if (this.cur_pos.text[num - 1] == '\u0017' - c)
				{
					num--;
				}
			}
		}
		string str = this.cur_pos.text.Substring(idx, num - idx);
		UIText.SetTextKey(textMeshProUGUI, "@" + str, null, null);
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x001469AC File Offset: 0x00144BAC
	private void ParsePositionAndNameText()
	{
		GameObject go = this.SpawnRow();
		char c = '\0';
		int idx = this.cur_pos.idx;
		while (this.cur_pos.Valid)
		{
			c = this.cur_pos.Char(0);
			if (c == ':' || c == '\r' || c == '\n')
			{
				break;
			}
			this.cur_pos.MoveNext(true);
		}
		if (c == ':')
		{
			int idx2 = this.cur_pos.idx;
			string str = this.cur_pos.text.Substring(idx, idx2 - idx);
			UIText.SetTextKey(go, "id_Position", "@" + str, null, null);
			this.cur_pos.MoveNext(true);
		}
		else
		{
			UIText.SetText(go, "id_Position", "");
			this.cur_pos.idx = idx;
			c = this.cur_pos.Char(0);
			c = ':';
		}
		string text = "";
		if (c == ':')
		{
			while (this.cur_pos.Valid)
			{
				this.cur_pos.SkipBlanks(true);
				int idx3 = this.cur_pos.idx;
				while (this.cur_pos.Valid)
				{
					c = this.cur_pos.Char(0);
					if (c == ';' || c == '\r' || c == '\n')
					{
						break;
					}
					this.cur_pos.MoveNext(true);
				}
				int idx4 = this.cur_pos.idx;
				string str2 = this.cur_pos.text.Substring(idx3, idx4 - idx3);
				if (text != "")
				{
					text += "\n";
				}
				text += str2;
				if (c != ';')
				{
					break;
				}
				this.cur_pos.MoveNext(true);
			}
		}
		UIText.SetTextKey(go, "id_Name", "@" + text, null, null);
		this.cur_pos.SkipLine(false);
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x00146B78 File Offset: 0x00144D78
	private void ParseTwoNamesText()
	{
		GameObject go = this.SpawnRow();
		char c = '\0';
		int idx = this.cur_pos.idx;
		while (this.cur_pos.Valid)
		{
			c = this.cur_pos.Char(0);
			if (c == ';' || c == '\r' || c == '\n')
			{
				break;
			}
			this.cur_pos.MoveNext(true);
		}
		int idx2 = this.cur_pos.idx;
		string str = this.cur_pos.text.Substring(idx, idx2 - idx);
		UIText.SetTextKey(go, "id_Name1", "@" + str, null, null);
		string str2 = "";
		if (c == ';')
		{
			this.cur_pos.MoveNext(true);
			this.cur_pos.SkipBlanks(true);
			int idx3 = this.cur_pos.idx;
			this.cur_pos.SkipToEndOfLine(true);
			int idx4 = this.cur_pos.idx;
			str2 = this.cur_pos.text.Substring(idx3, idx4 - idx3);
		}
		UIText.SetTextKey(go, "id_Name2", "@" + str2, null, null);
		this.cur_pos.SkipLine(false);
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x00146C99 File Offset: 0x00144E99
	private GameObject SpawnRow()
	{
		if (this.cur_prefab == null)
		{
			return null;
		}
		GameObject gameObject = global::Common.Spawn(this.cur_prefab, this.id_CreditsContainer.transform, false, "");
		gameObject.hideFlags = HideFlags.DontSave;
		return gameObject;
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x00146CCF File Offset: 0x00144ECF
	private void Warning(string msg)
	{
		this.Warning(msg, this.cur_pos);
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x00146CDE File Offset: 0x00144EDE
	private void Error(string msg)
	{
		this.Error(msg, this.cur_pos);
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x00146CED File Offset: 0x00144EED
	private void Warning(string msg, Wiki.Parser.Pos pos)
	{
		Game.Log(string.Format("{0}({1}): {2}: {3}", new object[]
		{
			pos.filename,
			pos.line,
			msg,
			pos.LineText()
		}), Game.LogType.Warning);
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x00146D2A File Offset: 0x00144F2A
	private void Error(string msg, Wiki.Parser.Pos pos)
	{
		Game.Log(string.Format("{0}({1}): {2}: {3}", new object[]
		{
			pos.filename,
			pos.line,
			msg,
			pos.LineText()
		}), Game.LogType.Error);
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x00146D67 File Offset: 0x00144F67
	private void Warning(string msg, Wiki.Location fl)
	{
		Game.Log(string.Format("{0}: {1}", fl, msg), Game.LogType.Warning);
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x00146D80 File Offset: 0x00144F80
	private void Error(string msg, Wiki.Location fl)
	{
		Game.Log(string.Format("{0}): {1}", fl, msg), Game.LogType.Error);
	}

	// Token: 0x040018AE RID: 6318
	private static string wnd_def_id = "CreditsWindow";

	// Token: 0x040018AF RID: 6319
	[UIFieldTarget("id_Styles")]
	private GameObject id_Styles;

	// Token: 0x040018B0 RID: 6320
	[UIFieldTarget("id_CreditsContainer")]
	private GameObject id_CreditsContainer;

	// Token: 0x040018B1 RID: 6321
	[UIFieldTarget("id_ScrollView")]
	private BSGScrollRect id_ScrollView;

	// Token: 0x040018B2 RID: 6322
	[UIFieldTarget("id_Back")]
	private BSGButton id_Back;

	// Token: 0x040018B3 RID: 6323
	[UIFieldTarget("id_BackLabel")]
	private TextMeshProUGUI id_BackLabel;

	// Token: 0x040018B4 RID: 6324
	private bool isScrolling;

	// Token: 0x040018B5 RID: 6325
	private float lastUserInteraction;

	// Token: 0x040018B6 RID: 6326
	public float scrollSpeed = 2f;

	// Token: 0x040018B7 RID: 6327
	public float userInteractionTimeoutDuration = 2f;

	// Token: 0x040018B8 RID: 6328
	private Dictionary<string, GameObject> styles = new Dictionary<string, GameObject>();

	// Token: 0x040018B9 RID: 6329
	private Wiki.Parser.Pos cur_pos;

	// Token: 0x040018BA RID: 6330
	private string cur_style;

	// Token: 0x040018BB RID: 6331
	private GameObject cur_prefab;
}
