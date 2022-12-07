using System;
using System.Collections;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class UIPauseNotification : MonoBehaviour
{
	// Token: 0x06002540 RID: 9536 RVA: 0x0014B758 File Offset: 0x00149958
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		this.game = GameLogic.Get(false);
		if (this.game == null)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		GameSpeed.OnPaused += this.OnPaused;
		GameSpeed.OnSpeedChange += this.OnSpeedChange;
		if (this.m_Title != null)
		{
			UIText.SetTextKey(this.m_Title, "PauseRequest.caption", null, null);
		}
		this.RefreshState();
		this.m_Initalized = true;
	}

	// Token: 0x06002541 RID: 9537 RVA: 0x0014B7D9 File Offset: 0x001499D9
	private void OnSpeedChange(float obj)
	{
		this.RefreshState();
	}

	// Token: 0x06002542 RID: 9538 RVA: 0x0014B7D9 File Offset: 0x001499D9
	private void OnPaused(bool obj)
	{
		this.RefreshState();
	}

	// Token: 0x06002543 RID: 9539 RVA: 0x0014B7E1 File Offset: 0x001499E1
	private IEnumerator Start()
	{
		yield return null;
		bool notready = true;
		while (notready)
		{
			BaseUI baseUI = BaseUI.Get();
			if (baseUI == null || baseUI.kingdom == 0)
			{
				yield return null;
			}
			else
			{
				notready = false;
			}
		}
		UIPauseNotification.currnet = this;
		this.Init();
		yield break;
	}

	// Token: 0x06002544 RID: 9540 RVA: 0x0014B7F0 File Offset: 0x001499F0
	private void RefreshState()
	{
		bool flag = GameSpeed.IsPaused();
		if (this.m_TitleTween != null)
		{
			this.m_TitleTween.gameObject.SetActive(flag);
		}
		if (this.m_DescriptionTween != null)
		{
			this.m_DescriptionTween.gameObject.SetActive(flag);
		}
		this.RefreshBody(flag);
	}

	// Token: 0x06002545 RID: 9541 RVA: 0x0014B848 File Offset: 0x00149A48
	private void RefreshBody(bool paused)
	{
		this.needs_update = false;
		if (this.m_ReasonText == null)
		{
			return;
		}
		Game game = GameLogic.Get(false);
		Pause pause = (game != null) ? game.pause : null;
		if (!paused || pause == null || !pause.IsMultiplayer())
		{
			UIText.SetText(this.m_ReasonText, "");
			if (this.m_ReasonShadow != null)
			{
				this.m_ReasonShadow.gameObject.SetActive(false);
			}
			return;
		}
		UIPauseNotification.sb.Clear();
		for (int i = 0; i < pause.requests.Count; i++)
		{
			Pause.Request request = pause.requests[i];
			string value = global::Defs.Localize(request.def.field, "label", request, null, true, true);
			if (!string.IsNullOrEmpty(value))
			{
				UIPauseNotification.sb.AppendLine(value);
			}
		}
		if (!pause.CanUnpause(-2))
		{
			string value2 = global::Defs.Localize("PauseRequest.cannot_unpause_label", pause, null, true, true);
			if (!string.IsNullOrEmpty(value2))
			{
				UIPauseNotification.sb.AppendLine(value2);
				this.needs_update = true;
			}
		}
		string text = UIPauseNotification.sb.ToString();
		UIText.SetText(this.m_ReasonText, text);
		if (this.m_ReasonShadow != null)
		{
			this.m_ReasonShadow.gameObject.SetActive(text.Length > 0);
		}
	}

	// Token: 0x06002546 RID: 9542 RVA: 0x0014B990 File Offset: 0x00149B90
	private void Update()
	{
		if (!this.needs_update)
		{
			return;
		}
		this.RefreshBody(GameSpeed.IsPaused());
	}

	// Token: 0x06002547 RID: 9543 RVA: 0x0014B9A6 File Offset: 0x00149BA6
	private void OnDestroy()
	{
		UIPauseNotification.currnet = null;
		GameSpeed.OnPaused -= this.OnPaused;
		GameSpeed.OnSpeedChange -= this.OnSpeedChange;
	}

	// Token: 0x04001957 RID: 6487
	private static UIPauseNotification currnet;

	// Token: 0x04001958 RID: 6488
	[UIFieldTarget("id_BodyTweenGroup")]
	private TweenCanvasGroupAplha m_DescriptionTween;

	// Token: 0x04001959 RID: 6489
	[UIFieldTarget("id_TitleTweenGroup")]
	private TweenCanvasGroupAplha m_TitleTween;

	// Token: 0x0400195A RID: 6490
	[UIFieldTarget("id_Title")]
	private TextMeshProUGUI m_Title;

	// Token: 0x0400195B RID: 6491
	[UIFieldTarget("id_ReasonText")]
	private TextMeshProUGUI m_ReasonText;

	// Token: 0x0400195C RID: 6492
	[UIFieldTarget("id_ReasonShadow")]
	private GameObject m_ReasonShadow;

	// Token: 0x0400195D RID: 6493
	private Game game;

	// Token: 0x0400195E RID: 6494
	private bool m_Initalized;

	// Token: 0x0400195F RID: 6495
	private DT.Field m_WindowDef;

	// Token: 0x04001960 RID: 6496
	private static StringBuilder sb = new StringBuilder(256);

	// Token: 0x04001961 RID: 6497
	private bool needs_update;
}
