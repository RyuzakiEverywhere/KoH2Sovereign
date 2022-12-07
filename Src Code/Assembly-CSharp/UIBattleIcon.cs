using System;
using Logic;
using UnityEngine;

// Token: 0x020001B0 RID: 432
public class UIBattleIcon : MonoBehaviour, IListener
{
	// Token: 0x06001993 RID: 6547 RVA: 0x000F931E File Offset: 0x000F751E
	private void Start()
	{
		this.Populate();
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x000F9326 File Offset: 0x000F7526
	private void OnDestroy()
	{
		this.RemoveListeners();
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x000F932E File Offset: 0x000F752E
	private void Update()
	{
		this.UpdateFocus();
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x000F9338 File Offset: 0x000F7538
	private void Populate()
	{
		this.icon = base.GetComponent<MessageIcon>();
		if (this.icon != null)
		{
			MessageIcon messageIcon = this.icon;
			this.logic = ((messageIcon != null) ? messageIcon.vars.Get<Logic.Battle>("battle", null) : null);
			Logic.Battle battle = this.logic;
			this.visuals = (((battle != null) ? battle.visuals : null) as global::Battle);
		}
		if (this.logic != null && !(this.visuals == null))
		{
			this.focused = global::Common.FindChildByName(base.gameObject, "id_Focused", true, true);
			this.AddListeners();
			this.Refresh();
			return;
		}
		if (this.icon != null)
		{
			this.Dismiss();
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x000F93FC File Offset: 0x000F75FC
	public void Refresh()
	{
		DT.Field defField = global::Defs.GetDefField(this.visuals.OngoingMessageDefId(), null);
		if (defField != null)
		{
			this.icon.def_field = defField;
		}
		this.icon.UpdateCaption(true);
		this.icon.UpdateIcon();
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x000F9444 File Offset: 0x000F7644
	private void UpdateFocus()
	{
		if (this.focused == null)
		{
			return;
		}
		WorldUI worldUI = WorldUI.Get();
		if (worldUI == null)
		{
			return;
		}
		this.focused.SetActive(worldUI.selected_logic_obj == this.logic);
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x000F9489 File Offset: 0x000F7689
	private void Dismiss()
	{
		MessageIcon messageIcon = this.icon;
		if (messageIcon == null)
		{
			return;
		}
		messageIcon.Dismiss(true);
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x000F949C File Offset: 0x000F769C
	private void AddListeners()
	{
		if (this.logic != null)
		{
			this.logic.AddListener(this);
		}
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x000F94B2 File Offset: 0x000F76B2
	private void RemoveListeners()
	{
		if (this.logic != null)
		{
			this.logic.DelListener(this);
		}
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x000F94C8 File Offset: 0x000F76C8
	public void OnMessage(object obj, string message, object param)
	{
		if (this.logic == null)
		{
			return;
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1649643086U)
		{
			if (num > 1177151643U)
			{
				if (num != 1211309691U)
				{
					if (num != 1649643086U)
					{
						return;
					}
					if (!(message == "finishing"))
					{
						return;
					}
				}
				else if (!(message == "destroying"))
				{
					return;
				}
				this.Dismiss();
				return;
			}
			if (num == 264539004U)
			{
				message == "enter_battle";
				return;
			}
			if (num != 1177151643U)
			{
				return;
			}
			message == "changed";
			return;
		}
		else if (num <= 2313807619U)
		{
			if (num == 2153339806U)
			{
				message == "started";
				return;
			}
			if (num != 2313807619U)
			{
				return;
			}
			message == "armies_changed";
			return;
		}
		else if (num != 2790208658U)
		{
			if (num != 3326472242U)
			{
				return;
			}
			message == "stage_changed";
			return;
		}
		else
		{
			if (!(message == "type_changed"))
			{
				return;
			}
			this.Refresh();
			return;
		}
	}

	// Token: 0x0400107A RID: 4218
	[NonSerialized]
	public Logic.Battle logic;

	// Token: 0x0400107B RID: 4219
	private MessageIcon icon;

	// Token: 0x0400107C RID: 4220
	private global::Battle visuals;

	// Token: 0x0400107D RID: 4221
	private GameObject focused;
}
