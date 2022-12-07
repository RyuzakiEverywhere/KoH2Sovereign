using System;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001ED RID: 493
public class UICrown : MonoBehaviour, IListener
{
	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06001D96 RID: 7574 RVA: 0x00115FA6 File Offset: 0x001141A6
	// (set) Token: 0x06001D97 RID: 7575 RVA: 0x00115FAE File Offset: 0x001141AE
	public Logic.Character Data { get; private set; }

	// Token: 0x06001D98 RID: 7576 RVA: 0x00115FB7 File Offset: 0x001141B7
	protected virtual void Awake()
	{
		this.wasInitalzied = true;
		if (this.addListeners)
		{
			this.AddListeners();
		}
	}

	// Token: 0x06001D99 RID: 7577 RVA: 0x00115FCE File Offset: 0x001141CE
	protected virtual void AddListeners()
	{
		if (this.Data != null)
		{
			this.Data.AddListener(this);
		}
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x00115FE4 File Offset: 0x001141E4
	public virtual void RemoveListeners()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x00115FFC File Offset: 0x001141FC
	public void OnEnable()
	{
		UICharacterIcon componentInParent = base.GetComponentInParent<UICharacterIcon>();
		if (componentInParent != null)
		{
			this.SetData(componentInParent.Data);
		}
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x00116025 File Offset: 0x00114225
	public void SetData(Logic.Character character)
	{
		this.RemoveListeners();
		this.Data = character;
		if (this.wasInitalzied)
		{
			this.AddListeners();
		}
		else
		{
			this.addListeners = true;
		}
		this.Refresh();
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x00116054 File Offset: 0x00114254
	private void Refresh()
	{
		if (this.Data != null && this.Data.title != null && this.Image_CrownIcon != null)
		{
			bool flag = this.Image_CrownIcon.rectTransform.rect.width < 28f;
			string text = this.Data.title;
			if (this.Data.IsRoyalRelative())
			{
				text = "Relative";
			}
			if (this.Data.IsPatriarch())
			{
				text = "Patriarch";
			}
			if (this.Data.IsEcumenicalPatriarch())
			{
				text = "EcumenicalPatriarch";
			}
			if (this.Data.IsDead() && this.Data.FindStatus<DeadPatriarchStatus>() != null)
			{
				text = (this.Data.GetKingdom().is_ecumenical_patriarchate ? "EcumenicalPatriarch" : "Patriarch");
			}
			if (this.Data.IsPope())
			{
				text = "Pope";
			}
			if (this.Data.IsHeir())
			{
				text = "Hair";
			}
			string key = flag ? (text + ".small") : text;
			Sprite obj = global::Defs.GetObj<Sprite>("CrownSettings", key, null);
			if (obj == null)
			{
				key = text;
				obj = global::Defs.GetObj<Sprite>("CrownSettings", key, null);
			}
			if (obj != null)
			{
				this.Image_CrownIcon.overrideSprite = obj;
			}
		}
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x0011619B File Offset: 0x0011439B
	private void LateUpdate()
	{
		if (this.invalidate)
		{
			this.Refresh();
			this.invalidate = false;
		}
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x001161B2 File Offset: 0x001143B2
	protected void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x06001DA0 RID: 7584 RVA: 0x001161BC File Offset: 0x001143BC
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "title_changed")
		{
			this.invalidate = true;
			return;
		}
		if (message == "refresh_tags")
		{
			this.invalidate = true;
			return;
		}
		if (!(message == "destroying") && !(message == "finishing"))
		{
			return;
		}
		this.RemoveListeners();
	}

	// Token: 0x04001371 RID: 4977
	[SerializeField]
	private Image Image_CrownIcon;

	// Token: 0x04001373 RID: 4979
	private bool wasInitalzied;

	// Token: 0x04001374 RID: 4980
	private bool addListeners;

	// Token: 0x04001375 RID: 4981
	private bool invalidate;
}
