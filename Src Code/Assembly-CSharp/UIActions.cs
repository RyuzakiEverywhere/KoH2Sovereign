using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200018F RID: 399
public class UIActions : MonoBehaviour, IListener
{
	// Token: 0x1700011A RID: 282
	// (get) Token: 0x0600160D RID: 5645 RVA: 0x000DF4AE File Offset: 0x000DD6AE
	// (set) Token: 0x0600160E RID: 5646 RVA: 0x000DF4B6 File Offset: 0x000DD6B6
	public Logic.Object Data { get; private set; }

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x0600160F RID: 5647 RVA: 0x000DF4BF File Offset: 0x000DD6BF
	// (set) Token: 0x06001610 RID: 5648 RVA: 0x000DF4C7 File Offset: 0x000DD6C7
	public Actions Actions { get; private set; }

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06001611 RID: 5649 RVA: 0x000DF4D0 File Offset: 0x000DD6D0
	// (set) Token: 0x06001612 RID: 5650 RVA: 0x000DF4D8 File Offset: 0x000DD6D8
	public Vars Vars { get; private set; }

	// Token: 0x06001613 RID: 5651 RVA: 0x000DF4E4 File Offset: 0x000DD6E4
	public void SetData(Logic.Object obj, Vars vars)
	{
		this.CleanActionEvents();
		this.Data = obj;
		if (this.Data != null)
		{
			this.Data.AddListener(this);
		}
		this.Vars = vars;
		this.Actions = obj.GetComponent<Actions>();
		UICommon.FindComponents(this, false);
		this.m_Initialzied = true;
		this.Refresh();
	}

	// Token: 0x06001614 RID: 5652 RVA: 0x000DF539 File Offset: 0x000DD739
	private void OnEnable()
	{
		TooltipPlacement.AddBlocker(base.gameObject, null);
	}

	// Token: 0x06001615 RID: 5653 RVA: 0x000DF547 File Offset: 0x000DD747
	private void OnDisable()
	{
		TooltipPlacement.DelBlocker(base.gameObject);
	}

	// Token: 0x06001616 RID: 5654 RVA: 0x000DF554 File Offset: 0x000DD754
	private void OnDestroy()
	{
		if (this.Data != null)
		{
			this.Data.DelListener(this);
		}
		this.CleanActionEvents();
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x000DF570 File Offset: 0x000DD770
	private void CleanActionEvents()
	{
		Logic.Object data = this.Data;
		if (data != null)
		{
			data.DelListener(this);
		}
		if (this.Data != null && this.Actions != null && this.Data != null && this.Actions != null && this.Actions.Count > 0)
		{
			int i = 0;
			int count = this.Actions.Count;
			while (i < count)
			{
				if (this.Actions[i] != null)
				{
					ActionVisuals actionVisuals = this.Actions[i].visuals as ActionVisuals;
					if (actionVisuals != null)
					{
						actionVisuals.logic.DelListener(this);
					}
				}
				i++;
			}
		}
		this.m_CurrentActions.Clear();
	}

	// Token: 0x06001618 RID: 5656 RVA: 0x000DF614 File Offset: 0x000DD814
	private void Update()
	{
		if (!this.m_Initialzied)
		{
			return;
		}
		if (this.m_Invalidate)
		{
			this.Refresh();
			this.m_Invalidate = false;
		}
		this.Actions = this.Data.GetComponent<Actions>();
		if (this.Actions == null)
		{
			return;
		}
		List<Action> sorted = this.GetSorted(this.Actions);
		int i = 0;
		int count = sorted.Count;
		while (i < count)
		{
			Action action = sorted[i];
			if (action != null && action.def.opportunity == null && (this.layoutType != UIActions.LayoutType.CharacterWindow || action.def.field.GetBool("show_in_character_widnow", null, false, true, true, true, '.')) && (this.layoutType != UIActions.LayoutType.ActionBar || action.def.field.GetBool("show_in_action_bar", action, false, true, true, true, '.')))
			{
				ActionVisuals item = action.visuals as ActionVisuals;
				bool flag = this.IsEligableToShow(action, true);
				if ((!flag && this.m_CurrentActions.Contains(item)) || (flag && !this.m_CurrentActions.Contains(item)))
				{
					this.Refresh();
					return;
				}
			}
			i++;
		}
	}

	// Token: 0x06001619 RID: 5657 RVA: 0x000DF730 File Offset: 0x000DD930
	private bool IsEligableToShow(Action action, bool ignoreRecall = true)
	{
		if (action == null)
		{
			return false;
		}
		if (ignoreRecall && action is RecallAction)
		{
			return false;
		}
		string text = action.Validate(false);
		bool result = false;
		if (text == "ok")
		{
			result = true;
		}
		else if (action.def.field.GetBool("show_if_invalid", null, false, true, true, true, '.'))
		{
			result = true;
		}
		else if (!string.IsNullOrEmpty(text) && text[0] == '_')
		{
			result = true;
		}
		else if (action.target != null)
		{
			result = true;
		}
		else if (action.state == Action.State.Preparing)
		{
			result = true;
		}
		return result;
	}

	// Token: 0x0600161A RID: 5658 RVA: 0x000DF7BC File Offset: 0x000DD9BC
	private List<Action> GetSorted(Actions actions)
	{
		this.m_TmpActionList.Clear();
		int i = 0;
		int count = actions.Count;
		while (i < count)
		{
			this.m_TmpActionList.Add(actions[i]);
			i++;
		}
		UIActions.InsertionSort<Action>(this.m_TmpActionList, new Comparison<Action>(UIActions.CompareActionsUIOrder));
		return this.m_TmpActionList;
	}

	// Token: 0x0600161B RID: 5659 RVA: 0x000DF818 File Offset: 0x000DDA18
	private static int CompareActionsUIOrder(Action x, Action y)
	{
		int result = 1;
		if (x != null && y != null)
		{
			result = y.def.ui_order.CompareTo(x.def.ui_order);
		}
		return result;
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x000DF84C File Offset: 0x000DDA4C
	private static void InsertionSort<T>(IList<T> list, Comparison<T> comparison)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (comparison == null)
		{
			throw new ArgumentNullException("comparison");
		}
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			T t = list[i];
			int num = i - 1;
			while (num >= 0 && comparison(list[num], t) > 0)
			{
				list[num + 1] = list[num];
				num--;
			}
			list[num + 1] = t;
		}
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x000DF8CC File Offset: 0x000DDACC
	private void Refresh()
	{
		this.CleanActionEvents();
		if (this.Container == null)
		{
			return;
		}
		UICommon.DeleteChildren(this.Container.transform);
		if (this.Data == null)
		{
			return;
		}
		GameObject gameObject = null;
		if (this.Actions != null && this.Actions.Count > 0)
		{
			List<Action> sorted = this.GetSorted(this.Actions);
			int i = 0;
			int count = sorted.Count;
			while (i < count)
			{
				Action action = sorted[i];
				if (action != null && action.def.opportunity == null && (this.layoutType != UIActions.LayoutType.CharacterWindow || action.def.field.GetBool("show_in_character_widnow", null, false, true, true, true, '.')) && (this.layoutType != UIActions.LayoutType.ActionBar || action.def.field.GetBool("show_in_action_bar", action, false, true, true, true, '.')) && this.IsEligableToShow(action, true))
				{
					ActionVisuals actionVisuals = action.visuals as ActionVisuals;
					if (actionVisuals != null)
					{
						GameObject icon = ObjectIcon.GetIcon(actionVisuals.logic, this.Vars, this.Container.transform as RectTransform);
						if (icon != null)
						{
							actionVisuals.logic.AddListener(this);
							UIActionIcon component = icon.GetComponent<UIActionIcon>();
							if (component != null)
							{
								component.OnSelect = new Action<UIActionIcon, PointerEventData>(this.HandleOnIconSelect);
							}
							if (action is ExileAction)
							{
								gameObject = icon;
							}
							this.m_CurrentActions.Add(actionVisuals);
						}
						else
						{
							Debug.LogWarning("Missing icon asset for logic object: " + actionVisuals.logic);
						}
					}
				}
				i++;
			}
		}
		Logic.Status status = this.Data.GetStatus();
		if (status != null)
		{
			Game game = GameLogic.Get(true);
			DT.Field field = status.def.field.FindChild("buttons", null, true, true, true, '.');
			if (this.layoutType == UIActions.LayoutType.ActionBar && game != null)
			{
				if (status is OngoingActionStatus)
				{
					Action action2 = (status as OngoingActionStatus).GetAction();
					if (action2 != null && action2.def != null && action2.def.field != null && action2.visuals != null && action2.def.field.GetBool("show_while_executing", null, false, true, true, true, '.'))
					{
						ActionVisuals relatedActionVisuals = action2.visuals as ActionVisuals;
						Vars vars = new Vars();
						vars.Set<Logic.Object>("obj", this.Data);
						vars.Set<string>("tooltip", action2.def.field.GetString("tooltip", null, "", true, true, true, '.'));
						vars.Set<Sprite>("sprite", global::Defs.GetObj<Sprite>(action2.def.field, "icon", null));
						UIGenericActionIcon.Create(delegate
						{
							relatedActionVisuals.ShowTargetPicker();
						}, this.PrototypeInspect, this.Container.transform as RectTransform, vars);
					}
				}
				else
				{
					Vars vars2 = new Vars();
					vars2.Set<Logic.Object>("obj", this.Data);
					if (field != null)
					{
						List<DT.Field> list = field.Children();
						if (list != null)
						{
							foreach (DT.Field field2 in list)
							{
								string @string = field2.GetString("related_action", null, "", true, true, true, '.');
								if (!(@string == ""))
								{
									Action action3 = Action.Find(this.Data, @string);
									if (action3 != null && field2.GetBool("show_in_action_bar", action3, false, true, true, true, '.'))
									{
										ActionVisuals relatedActionVisuals = action3.visuals as ActionVisuals;
										if (relatedActionVisuals != null)
										{
											vars2.Set<string>("tooltip", field2.GetString("tooltip", null, "", true, true, true, '.'));
											vars2.Set<Sprite>("sprite", global::Defs.GetObj<Sprite>(field2, "icon", null));
											UIGenericActionIcon.Create(delegate
											{
												relatedActionVisuals.ShowTargetPicker();
											}, this.PrototypeInspect, this.Container.transform as RectTransform, vars2);
										}
									}
								}
							}
						}
					}
				}
			}
			if (gameObject != null)
			{
				gameObject.transform.SetAsLastSibling();
			}
		}
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x000DFD4C File Offset: 0x000DDF4C
	public bool HasActiveActions()
	{
		if (this.Actions == null)
		{
			return false;
		}
		if (this.Actions.Count == 0)
		{
			return false;
		}
		int i = 0;
		int count = this.Actions.Count;
		while (i < count)
		{
			Action action = this.Actions[i];
			if (action != null && action.def.opportunity == null && (this.layoutType != UIActions.LayoutType.CharacterWindow || action.def.field.GetBool("show_in_character_widnow", null, false, true, true, true, '.')) && (this.layoutType != UIActions.LayoutType.ActionBar || action.def.field.GetBool("show_in_action_bar", action, false, true, true, true, '.')) && this.IsEligableToShow(action, true))
			{
				return true;
			}
			i++;
		}
		return false;
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x000DFE01 File Offset: 0x000DE001
	private void HandleOnIconSelect(UIActionIcon icon, PointerEventData e)
	{
		Action onActionIconSeleced = this.OnActionIconSeleced;
		if (onActionIconSeleced == null)
		{
			return;
		}
		onActionIconSeleced();
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x000DFE14 File Offset: 0x000DE014
	public void OnMessage(object obj, string message, object param)
	{
		bool flag = false;
		if (obj is Action)
		{
			flag = true;
		}
		else if (obj is Logic.Kingdom)
		{
			flag = true;
		}
		else if (obj is Logic.Character)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (message == "force_refresh_actions")
		{
			this.m_Invalidate = true;
			return;
		}
		if (message == "status_changed")
		{
			this.m_Invalidate = true;
			return;
		}
		if (message == "enter_state" || message == "action_ran")
		{
			this.m_Invalidate = true;
			return;
		}
		if (message == "destroying" || message == "finishing")
		{
			if (obj is Logic.Character)
			{
				(obj as Logic.Character).DelListener(this);
				return;
			}
			if (obj is Logic.Kingdom)
			{
				(obj as Logic.Kingdom).DelListener(this);
				return;
			}
			if (obj is Action)
			{
				(obj as Action).DelListener(this);
				return;
			}
		}
	}

	// Token: 0x06001621 RID: 5665 RVA: 0x000DFEEE File Offset: 0x000DE0EE
	public static UIActions Possess(Logic.Object obj, GameObject posessee, Vars vars)
	{
		if (obj == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no character data e provided.");
			return null;
		}
		if (posessee == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no prototype provided.");
			return null;
		}
		UIActions orAddComponent = posessee.GetOrAddComponent<UIActions>();
		orAddComponent.SetData(obj, vars);
		return orAddComponent;
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000DFF24 File Offset: 0x000DE124
	public static UIActions Create(Logic.Object obj, GameObject prototype, RectTransform parent, Vars vars)
	{
		if (obj == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no character data e provided.");
			return null;
		}
		if (prototype == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no prototype provided.");
			return null;
		}
		if (parent == null)
		{
			Debug.LogWarning("Fail to create character Info widnow! Reson: no parent provided.");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prototype, Vector3.zero, Quaternion.identity, parent);
		UIActions uiactions = gameObject.GetComponent<UIActions>();
		if (uiactions == null)
		{
			uiactions = gameObject.AddComponent<UIActions>();
		}
		uiactions.SetData(obj, vars);
		return uiactions;
	}

	// Token: 0x04000E40 RID: 3648
	[SerializeField]
	private GameObject PrototypeInspect;

	// Token: 0x04000E41 RID: 3649
	[SerializeField]
	private GameObject Container;

	// Token: 0x04000E45 RID: 3653
	public UIActions.LayoutType layoutType;

	// Token: 0x04000E46 RID: 3654
	public Action OnActionIconSeleced;

	// Token: 0x04000E47 RID: 3655
	private bool m_Invalidate;

	// Token: 0x04000E48 RID: 3656
	private bool m_Initialzied;

	// Token: 0x04000E49 RID: 3657
	private List<Action> m_TmpActionList = new List<Action>();

	// Token: 0x04000E4A RID: 3658
	private List<ActionVisuals> m_CurrentActions = new List<ActionVisuals>();

	// Token: 0x020006C7 RID: 1735
	public enum LayoutType
	{
		// Token: 0x04003702 RID: 14082
		ActionBar,
		// Token: 0x04003703 RID: 14083
		CharacterWindow,
		// Token: 0x04003704 RID: 14084
		ShowAll
	}
}
