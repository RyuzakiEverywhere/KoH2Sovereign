using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200028A RID: 650
public class UICourtMemberMerchantImportActions : MonoBehaviour, IListener
{
	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060027C0 RID: 10176 RVA: 0x00158285 File Offset: 0x00156485
	// (set) Token: 0x060027C1 RID: 10177 RVA: 0x0015828D File Offset: 0x0015648D
	public Logic.Character Character { get; private set; }

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060027C2 RID: 10178 RVA: 0x00158296 File Offset: 0x00156496
	// (set) Token: 0x060027C3 RID: 10179 RVA: 0x0015829E File Offset: 0x0015649E
	public Vars Vars { get; private set; }

	// Token: 0x060027C4 RID: 10180 RVA: 0x001582A7 File Offset: 0x001564A7
	public void SetData(Logic.Character character, Vars vars)
	{
		this.Init();
		Logic.Character character2 = this.Character;
		if (character2 != null)
		{
			character2.DelListener(this);
		}
		this.Character = character;
		Logic.Character character3 = this.Character;
		if (character3 != null)
		{
			character3.AddListener(this);
		}
		this.Vars = vars;
		this.Refresh();
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x001582E8 File Offset: 0x001564E8
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		for (int i = 0; i < this.m_Icons.Length; i++)
		{
			UIMerchantImportGoodIcon uimerchantImportGoodIcon = this.m_Icons[i];
			uimerchantImportGoodIcon.OnSelect = (Action<UIMerchantImportGoodIcon, PointerEventData>)Delegate.Combine(uimerchantImportGoodIcon.OnSelect, new Action<UIMerchantImportGoodIcon, PointerEventData>(this.OnIconSelect));
		}
		this.m_innerPpaddingEmpty = new RectOffset(5, 5, 5, 5);
		this.m_innerPaddingsPopluated = new RectOffset(15, 15, 4, 28);
		this.m_Initalized = true;
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x000DF539 File Offset: 0x000DD739
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
	}

	// Token: 0x060027C7 RID: 10183 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisbale()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x00158369 File Offset: 0x00156569
	private void Refresh()
	{
		this.PopulateIcons();
		this.UpdateLayout();
	}

	// Token: 0x060027C9 RID: 10185 RVA: 0x00158378 File Offset: 0x00156578
	private void UpdateLayout()
	{
		if (this.m_IconsContainer == null)
		{
			return;
		}
		if (this.m_Icons == null)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < this.m_Icons.Length; i++)
		{
			if (this.m_Icons[i].Def != null)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			this.m_IconsContainer.padding = this.m_innerPpaddingEmpty;
			this.m_IconsContainer.spacing = new Vector2(4f, 0f);
			return;
		}
		this.m_IconsContainer.padding = this.m_innerPaddingsPopluated;
		this.m_IconsContainer.spacing = new Vector2(20f, 0f);
	}

	// Token: 0x060027CA RID: 10186 RVA: 0x00158420 File Offset: 0x00156620
	private void PopulateIcons()
	{
		if (this.m_Icons == null)
		{
			return;
		}
		if (this.Character == null)
		{
			return;
		}
		for (int i = 0; i < this.m_Icons.Length; i++)
		{
			this.m_Icons[i].SetData(i, this.Character);
		}
	}

	// Token: 0x060027CB RID: 10187 RVA: 0x00158468 File Offset: 0x00156668
	private void OnIconSelect(UIMerchantImportGoodIcon icon, PointerEventData ev)
	{
		int num = -1;
		for (int i = 0; i < this.m_Icons.Length; i++)
		{
			UIMerchantImportGoodIcon uimerchantImportGoodIcon = this.m_Icons[i];
			if (uimerchantImportGoodIcon == icon)
			{
				num = i;
			}
			else
			{
				uimerchantImportGoodIcon.Select(false);
			}
		}
		icon.Select(!icon.IsSelected());
		if (icon.Def != null)
		{
			Logic.Character character = this.Character;
			object obj;
			if (character == null)
			{
				obj = null;
			}
			else
			{
				Actions actions = character.actions;
				obj = ((actions != null) ? actions.Find("CancelImportGoodAction") : null);
			}
			CancelImportGoodAction cancelImportGoodAction = obj as CancelImportGoodAction;
			if (cancelImportGoodAction != null && num > -1)
			{
				cancelImportGoodAction.args = new List<Value>(1)
				{
					num
				};
				ActionVisuals.ExecuteAction(cancelImportGoodAction);
				this.UpdateLayout();
			}
		}
	}

	// Token: 0x060027CC RID: 10188 RVA: 0x000023FD File Offset: 0x000005FD
	private void Clear()
	{
	}

	// Token: 0x060027CD RID: 10189 RVA: 0x00158512 File Offset: 0x00156712
	public void Close()
	{
		this.Clear();
		Logic.Character character = this.Character;
		if (character != null)
		{
			character.DelListener(this);
		}
		global::Common.DestroyObj(base.gameObject);
	}

	// Token: 0x060027CE RID: 10190 RVA: 0x00158537 File Offset: 0x00156737
	private void OnDestroy()
	{
		Logic.Character character = this.Character;
		if (character == null)
		{
			return;
		}
		character.DelListener(this);
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x0015854A File Offset: 0x0015674A
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "importing_goods_changed")
		{
			this.PopulateIcons();
			this.UpdateLayout();
		}
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x00158565 File Offset: 0x00156765
	public static GameObject GetPrefab()
	{
		return UICommon.GetPrefab("CourtMemberMerchantImportActions", null);
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x00158574 File Offset: 0x00156774
	public static UICourtMemberMerchantImportActions Create(Logic.Character character, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (prototype == null)
		{
			return null;
		}
		if (parent == null)
		{
			return null;
		}
		UICourtMemberMerchantImportActions orAddComponent = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent).GetOrAddComponent<UICourtMemberMerchantImportActions>();
		orAddComponent.SetData(character, vars);
		UICommon.SetAligment(orAddComponent.transform as RectTransform, TextAnchor.MiddleCenter);
		return orAddComponent;
	}

	// Token: 0x04001B08 RID: 6920
	[UIFieldTarget("id_ImportIcon")]
	private UIMerchantImportGoodIcon[] m_Icons;

	// Token: 0x04001B09 RID: 6921
	[UIFieldTarget("id_IconsContainer")]
	private GridLayoutGroup m_IconsContainer;

	// Token: 0x04001B0C RID: 6924
	private RectOffset m_innerPpaddingEmpty;

	// Token: 0x04001B0D RID: 6925
	private RectOffset m_innerPaddingsPopluated;

	// Token: 0x04001B0E RID: 6926
	private bool m_Initalized;
}
