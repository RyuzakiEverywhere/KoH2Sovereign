using System;
using System.Collections.Generic;
using System.Linq;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200027F RID: 639
public class RelationLatestHistory : MonoBehaviour
{
	// Token: 0x06002711 RID: 10001 RVA: 0x0015478C File Offset: 0x0015298C
	public static RelationLatestHistory Create(string type, GameObject parent, Logic.Object obj1, Logic.Object obj2)
	{
		if (type == null || type.Length == 0 || parent == null)
		{
			return null;
		}
		DT.Field defField = global::Defs.GetDefField("RelationsLastHistory", null);
		if (defField == null)
		{
			return null;
		}
		GameObject gameObject = defField.GetValue("ui_prefab_" + type, null, true, true, true, '.').obj_val as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2 = global::Common.Spawn(gameObject, false, false);
		if (gameObject2 == null)
		{
			return null;
		}
		gameObject2.transform.SetParent(parent.transform, false);
		RelationLatestHistory relationLatestHistory = gameObject2.GetComponent<RelationLatestHistory>();
		if (relationLatestHistory == null)
		{
			relationLatestHistory = gameObject2.AddComponent<RelationLatestHistory>();
		}
		relationLatestHistory.dtField = defField;
		relationLatestHistory.SetRelation(obj1, obj2, false);
		relationLatestHistory.maxButtons = defField.GetInt("numOfLatest", null, 0, true, true, true, '.');
		relationLatestHistory.Refresh();
		return relationLatestHistory;
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x00154859 File Offset: 0x00152A59
	public void SetRelation(Logic.Object o1, Logic.Object o2, bool refresh = false)
	{
		this.obj1 = o1;
		this.obj2 = o2;
		if (refresh)
		{
			this.Refresh();
		}
	}

	// Token: 0x06002713 RID: 10003 RVA: 0x00154874 File Offset: 0x00152A74
	private void SetButtonImage(GameObject obj, Reason reason)
	{
		Image image = global::Common.FindChildComponent<Image>(obj, "Image");
		if (image == null)
		{
			return;
		}
		if (reason.value >= 0f)
		{
			image.sprite = (this.dtField.GetValue("imgPlus", null, true, true, true, '.').obj_val as Sprite);
			return;
		}
		image.sprite = (this.dtField.GetValue("imgMinus", null, true, true, true, '.').obj_val as Sprite);
	}

	// Token: 0x06002714 RID: 10004 RVA: 0x001548F4 File Offset: 0x00152AF4
	private void AddButton(Reason reason)
	{
		if (this.buttonPrefab == null)
		{
			return;
		}
		GameObject gameObject = global::Common.Spawn(this.buttonPrefab, false, false);
		if (gameObject == null)
		{
			return;
		}
		gameObject.name = reason.field.key;
		this.SetButtonImage(gameObject, reason);
		gameObject.transform.SetParent(base.transform, false);
		gameObject.transform.SetAsFirstSibling();
		BSGButton component = gameObject.GetComponent<BSGButton>();
		this.buttons.Add(component);
		this.reasons.Add(reason);
	}

	// Token: 0x06002715 RID: 10005 RVA: 0x0015497D File Offset: 0x00152B7D
	private bool checkForModChages(List<Reason> newReasons)
	{
		if (this.reasons == null)
		{
			this.reasons = new List<Reason>();
		}
		return !this.reasons.SequenceEqual(newReasons);
	}

	// Token: 0x06002716 RID: 10006 RVA: 0x001549A4 File Offset: 0x00152BA4
	private void Clean()
	{
		this.buttons = new List<BSGButton>();
		this.reasons = new List<Reason>();
		foreach (object obj in base.transform)
		{
			global::Common.DestroyObj(((Transform)obj).gameObject);
		}
	}

	// Token: 0x06002717 RID: 10007 RVA: 0x00154A18 File Offset: 0x00152C18
	public void Refresh()
	{
		Logic.Kingdom k = this.obj1 as Logic.Kingdom;
		List<Reason> relatioReasonsFor = (this.obj2 as Logic.Kingdom).GetRelatioReasonsFor(k);
		relatioReasonsFor.Sort((Reason m1, Reason m2) => Mathf.Abs(m2.value).CompareTo(Mathf.Abs(m1.value)));
		List<Reason> list = new List<Reason>();
		int num = 0;
		for (int i = 0; i < relatioReasonsFor.Count; i++)
		{
			if (relatioReasonsFor[i].source == this.obj2)
			{
				bool textKey = Diplomacy.GetTextKey(Diplomacy.TextType.Angry, "", this.obj1 as Logic.Kingdom, this.obj2 as Logic.Kingdom, relatioReasonsFor[i]) != null;
				string textKey2 = Diplomacy.GetTextKey(Diplomacy.TextType.Pleased, "", this.obj1 as Logic.Kingdom, this.obj2 as Logic.Kingdom, relatioReasonsFor[i]);
				if (textKey || textKey2 != null)
				{
					num++;
					if (num > this.maxButtons)
					{
						break;
					}
					list.Add(relatioReasonsFor[i]);
				}
			}
		}
		list.Sort((Reason m1, Reason m2) => m2.value.CompareTo(m1.value));
		if (list.Count == 0)
		{
			this.Clean();
			return;
		}
		if (!this.checkForModChages(list))
		{
			return;
		}
		this.Clean();
		for (int j = 0; j < list.Count; j++)
		{
			this.AddButton(list[j]);
		}
	}

	// Token: 0x06002718 RID: 10008 RVA: 0x00154B7C File Offset: 0x00152D7C
	public void Refresh(RelationLatestHistory.DelegateEvent delE, RelationLatestHistory.DelegateClick delC, List<object> parameters = null)
	{
		this.Refresh();
		this.SetOnActions(delE, delC, parameters);
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x00154B8D File Offset: 0x00152D8D
	public void SetOnEvent(RelationLatestHistory.DelegateEvent del, List<object> parameters = null)
	{
		this.SetOnActions(del, null, parameters);
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x00154B98 File Offset: 0x00152D98
	public void SetOnClick(RelationLatestHistory.DelegateClick del, List<object> parameters = null)
	{
		this.SetOnActions(null, del, parameters);
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x00154BA4 File Offset: 0x00152DA4
	public void SetOnActions(RelationLatestHistory.DelegateEvent delE, RelationLatestHistory.DelegateClick delC, List<object> parameters = null)
	{
		for (int i = 0; i < this.buttons.Count; i++)
		{
			List<object> individualParams = new List<object>(parameters);
			individualParams.Add(this.reasons[i]);
			this.buttons[i].AllowSelection(true);
			BSGButton bsgbutton = this.buttons[i];
			bsgbutton.onClick = (BSGButton.OnClick)Delegate.Combine(bsgbutton.onClick, new BSGButton.OnClick(delegate(BSGButton BSGBtn)
			{
				delC(BSGBtn, this, individualParams);
			}));
			BSGButton bsgbutton2 = this.buttons[i];
			bsgbutton2.onEvent = (BSGButton.OnEvent)Delegate.Combine(bsgbutton2.onEvent, new BSGButton.OnEvent(delegate(BSGButton BSGBtn, BSGButton.Event ev, PointerEventData evd)
			{
				delE(BSGBtn, ev, evd, this, individualParams);
			}));
		}
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x00154C84 File Offset: 0x00152E84
	public void RemoveModifier(Reason reason)
	{
		for (int i = 0; i < this.reasons.Count; i++)
		{
			if (this.reasons[i] == reason)
			{
				(this.obj2 as Logic.Kingdom).diplomacyReasons.Remove(reason);
				return;
			}
		}
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x00154CD0 File Offset: 0x00152ED0
	public void DeselectAll()
	{
		for (int i = 0; i < this.buttons.Count; i++)
		{
			this.buttons[i].SetSelected(false, false);
		}
	}

	// Token: 0x0600271E RID: 10014 RVA: 0x00154D08 File Offset: 0x00152F08
	public void SelectButton(Reason reason)
	{
		for (int i = 0; i < this.reasons.Count; i++)
		{
			if (this.reasons[i] == reason)
			{
				this.buttons[i].SetSelected(!this.buttons[i].IsSelected(), false);
			}
			else
			{
				this.buttons[i].SetSelected(false, false);
			}
		}
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x00154D78 File Offset: 0x00152F78
	public int CheckButtonSelection(Reason reason)
	{
		int result = -1;
		for (int i = 0; i < this.reasons.Count; i++)
		{
			if (this.reasons[i] == reason)
			{
				result = (this.buttons[i].IsSelected() ? 1 : 0);
			}
			else
			{
				this.buttons[i].SetSelected(false, false);
			}
		}
		return result;
	}

	// Token: 0x04001A7D RID: 6781
	public GameObject buttonPrefab;

	// Token: 0x04001A7E RID: 6782
	private Logic.Object obj1;

	// Token: 0x04001A7F RID: 6783
	private Logic.Object obj2;

	// Token: 0x04001A80 RID: 6784
	private DT.Field dtField;

	// Token: 0x04001A81 RID: 6785
	private int maxButtons = 6;

	// Token: 0x04001A82 RID: 6786
	private List<BSGButton> buttons;

	// Token: 0x04001A83 RID: 6787
	private List<Reason> reasons;

	// Token: 0x04001A84 RID: 6788
	public RelationLatestHistory.DelegateClick delClick;

	// Token: 0x04001A85 RID: 6789
	public RelationLatestHistory.DelegateEvent delEvent;

	// Token: 0x020007DD RID: 2013
	// (Invoke) Token: 0x06004E7F RID: 20095
	public delegate void DelegateClick(BSGButton btn, RelationLatestHistory history, List<object> parameters);

	// Token: 0x020007DE RID: 2014
	// (Invoke) Token: 0x06004E83 RID: 20099
	public delegate void DelegateEvent(BSGButton btn, BSGButton.Event ev, PointerEventData eventData, RelationLatestHistory history, List<object> parameters);
}
