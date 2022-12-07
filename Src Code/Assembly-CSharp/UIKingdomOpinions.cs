using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200020F RID: 527
public class UIKingdomOpinions : MonoBehaviour
{
	// Token: 0x06001FF7 RID: 8183 RVA: 0x00125EBD File Offset: 0x001240BD
	private IEnumerator Start()
	{
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		yield break;
	}

	// Token: 0x06001FF8 RID: 8184 RVA: 0x00125EC5 File Offset: 0x001240C5
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		if (this.m_OppionionIconPrototype != null)
		{
			this.m_OppionionIconPrototype.gameObject.SetActive(false);
		}
		this.m_Initialzied = true;
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x00125EFD File Offset: 0x001240FD
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Init();
		this.kingdom = k;
		this.PopulateOpinions();
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x00125F14 File Offset: 0x00124114
	private void PopulateOpinions()
	{
		if (this.m_IconContainer == null)
		{
			return;
		}
		UICommon.DeleteActiveChildren(this.m_IconContainer);
		if (this.m_OppionionIconPrototype == null)
		{
			return;
		}
		List<Opinion> opinions = this.kingdom.opinions.opinions;
		if (opinions == null || opinions.Count == 0)
		{
			return;
		}
		for (int i = 0; i < opinions.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_OppionionIconPrototype, this.m_IconContainer);
			gameObject.gameObject.SetActive(true);
			UIKingdomOpinions.OppinionIcon oppinionIcon = gameObject.AddComponent<UIKingdomOpinions.OppinionIcon>();
			oppinionIcon.SetData(opinions[i], i + 1 == opinions.Count);
			oppinionIcon.OnSelect = (Action<UIKingdomOpinions.OppinionIcon>)Delegate.Combine(oppinionIcon.OnSelect, new Action<UIKingdomOpinions.OppinionIcon>(this.HandleOnIconClick));
		}
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x00125FD4 File Offset: 0x001241D4
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if ((this.kingdom == null || this.kingdom.visuals == null || this.kingdom != kingdom) && kingdom != null && kingdom.IsValid())
		{
			this.SetKingdom(kingdom);
		}
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x000023FD File Offset: 0x000005FD
	private void HandleOnIconClick(UIKingdomOpinions.OppinionIcon icon)
	{
	}

	// Token: 0x04001526 RID: 5414
	[UIFieldTarget("id_OppionionIconPrototype")]
	private GameObject m_OppionionIconPrototype;

	// Token: 0x04001527 RID: 5415
	[UIFieldTarget("id_IconContainer")]
	private RectTransform m_IconContainer;

	// Token: 0x04001528 RID: 5416
	private Logic.Kingdom kingdom;

	// Token: 0x04001529 RID: 5417
	private bool m_Initialzied;

	// Token: 0x02000748 RID: 1864
	internal class OppinionIcon : Hotspot
	{
		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06004A68 RID: 19048 RVA: 0x0022092D File Offset: 0x0021EB2D
		// (set) Token: 0x06004A69 RID: 19049 RVA: 0x00220935 File Offset: 0x0021EB35
		public Opinion Data { get; private set; }

		// Token: 0x06004A6A RID: 19050 RVA: 0x0022093E File Offset: 0x0021EB3E
		public void SetData(Opinion opinion, bool last = false)
		{
			this.Init();
			this.Data = opinion;
			this.m_Last = last;
			this.Refresh();
		}

		// Token: 0x06004A6B RID: 19051 RVA: 0x0022095A File Offset: 0x0021EB5A
		private void Init()
		{
			if (this.m_Initialzied)
			{
				return;
			}
			UICommon.FindComponents(this, false);
			if (this.m_Glow != null)
			{
				this.m_Glow.gameObject.SetActive(false);
			}
			this.m_Initialzied = true;
		}

		// Token: 0x06004A6C RID: 19052 RVA: 0x00220994 File Offset: 0x0021EB94
		private void Refresh()
		{
			Tooltip.Get(base.gameObject, true).SetObj(this.Data, null, null);
			if (this.m_Icon != null)
			{
				this.m_Icon.sprite = global::Defs.GetObj<Sprite>(this.Data.def.field, "icon", this.Data.kingdom);
			}
		}

		// Token: 0x06004A6D RID: 19053 RVA: 0x002209F8 File Offset: 0x0021EBF8
		private void UpdateDynamics()
		{
			if (this.m_OppinionValue != null)
			{
				this.m_OppinionValue.text = Mathf.RoundToInt(this.Data.value).ToString();
			}
			if (this.m_Background != null && this.m_prevValue != this.Data.value)
			{
				this.m_Background.overrideSprite = global::Defs.GetObj<Sprite>(this.Data.def.field, (this.Data.value == 0f) ? "backgrounds.neutral" : ((this.Data.value > 0f) ? "backgrounds.positive" : "backgrounds.negative"), null);
				this.m_prevValue = this.Data.value;
			}
		}

		// Token: 0x06004A6E RID: 19054 RVA: 0x00220AC0 File Offset: 0x0021ECC0
		private void Update()
		{
			this.UpdateDynamics();
		}

		// Token: 0x06004A6F RID: 19055 RVA: 0x00220AC8 File Offset: 0x0021ECC8
		public override void OnClick(PointerEventData e)
		{
			base.OnClick(e);
			if (this.OnSelect != null)
			{
				this.OnSelect(this);
				return;
			}
		}

		// Token: 0x06004A70 RID: 19056 RVA: 0x00220AE6 File Offset: 0x0021ECE6
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004A71 RID: 19057 RVA: 0x00220AF5 File Offset: 0x0021ECF5
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.UpdateHighlight();
		}

		// Token: 0x06004A72 RID: 19058 RVA: 0x000DB26E File Offset: 0x000D946E
		public void UpdateHighlight()
		{
			bool isPlaying = Application.isPlaying;
		}

		// Token: 0x04003969 RID: 14697
		[UIFieldTarget("id_Icon")]
		private Image m_Icon;

		// Token: 0x0400396A RID: 14698
		[UIFieldTarget("id_Background")]
		private Image m_Background;

		// Token: 0x0400396B RID: 14699
		[UIFieldTarget("id_OppinionValue")]
		private TextMeshProUGUI m_OppinionValue;

		// Token: 0x0400396C RID: 14700
		[UIFieldTarget("id_Glow")]
		private Image m_Glow;

		// Token: 0x0400396D RID: 14701
		public Action<UIKingdomOpinions.OppinionIcon> OnSelect;

		// Token: 0x0400396F RID: 14703
		private bool m_Last;

		// Token: 0x04003970 RID: 14704
		private bool m_Initialzied;

		// Token: 0x04003971 RID: 14705
		private float m_prevValue = 2.1474836E+09f;
	}
}
