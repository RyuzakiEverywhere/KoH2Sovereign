using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using UnityEngine;

// Token: 0x020000B6 RID: 182
public class ActionVisuals : IListener
{
	// Token: 0x060006FC RID: 1788 RVA: 0x00048CB4 File Offset: 0x00046EB4
	public static void Create(Action logic)
	{
		new ActionVisuals().Init(logic);
	}

	// Token: 0x060006FD RID: 1789 RVA: 0x00048CC1 File Offset: 0x00046EC1
	public void Init(Action logic)
	{
		this.logic = logic;
		logic.visuals = this;
		Action.get_requirement_texts = new Action.GetRequirementTexts(ActionVisuals.GetRequirementTexts);
		Action.get_prisoners_text = new Action.ListTexts(ActionVisuals.EscapeDeadPrisoners);
	}

	// Token: 0x060006FE RID: 1790 RVA: 0x00048CF4 File Offset: 0x00046EF4
	public void Begin()
	{
		if (UICommon.GetKey(KeyCode.LeftControl, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat action success", true))
		{
			this.logic.ForceOutcomes("*success");
		}
		else if (UICommon.GetKey(KeyCode.RightShift, false) && Game.CheckCheatLevel(Game.CheatLevel.High, "cheat action fail", true))
		{
			this.logic.ForceOutcomes("*fail");
		}
		string confirmationMessageKey = this.logic.GetConfirmationMessageKey();
		if (!string.IsNullOrEmpty(confirmationMessageKey))
		{
			Vars vars = new Vars(this.logic);
			DT.Field @ref = this.logic.def.field.GetRef(confirmationMessageKey, this.logic, true, true, true, '.');
			if (@ref != null && !UICommon.GetKey(KeyCode.LeftShift, false))
			{
				MessageWnd.Create(@ref, vars, null, new MessageWnd.OnButton(this.OnConfirmationMessageButton)).on_update = delegate(MessageWnd w)
				{
					if (this.logic == null || this.logic.Validate(true) != "ok")
					{
						w.Close(false);
					}
				};
				return;
			}
		}
		this.PickTarget();
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00048DDD File Offset: 0x00046FDD
	public virtual bool OnConfirmationMessageButton(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.PickTarget();
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x00048DFA File Offset: 0x00046FFA
	public virtual bool OnTargetConfirmationMessageButton(MessageWnd wnd, string btn_id)
	{
		if (btn_id == "ok")
		{
			this.logic.Execute(this.logic.target);
		}
		else
		{
			this.logic.Cancel(true, true);
		}
		wnd.CloseAndDismiss(true);
		return true;
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00048E38 File Offset: 0x00047038
	public void PickTarget()
	{
		if (this.logic.target != null)
		{
			this.TargetPicked(this.logic.target);
			return;
		}
		if (this.logic.NeedsTarget())
		{
			this.logic.SetState(Action.State.PickingTarget, true);
			return;
		}
		if (!this.logic.HasAllArgs())
		{
			this.logic.SetState(Action.State.PickingArgs, true);
			return;
		}
		this.logic.Execute(null);
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00048EAC File Offset: 0x000470AC
	public virtual void ShowTargetPicker()
	{
		WorldUI worldUI = WorldUI.Get();
		Vars vars = new Vars(this.logic);
		if (this.logic != null && this.logic.def != null)
		{
			GameObject obj = global::Defs.GetObj<GameObject>(this.logic.def.id, "target_picker_prefab", null);
			if (obj)
			{
				if (worldUI == null)
				{
					return;
				}
				GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
				if (gameObject != null)
				{
					UICommon.DeleteChildren(gameObject.transform, typeof(UIActionInfo));
				}
				GameObject gameObject2 = global::Common.Spawn(obj, false, false);
				gameObject2.transform.SetParent(gameObject.transform, false);
				UIActionInfo component = gameObject2.GetComponent<UIActionInfo>();
				if (component)
				{
					component.Init(this);
				}
				return;
			}
		}
		List<Logic.Object> possibleTargets = this.logic.GetPossibleTargets();
		List<Vars> possibleTargetsVars = this.logic.GetPossibleTargetsVars(possibleTargets);
		Func<Value, bool> validate = (Value t) => this.logic.ValidateTarget(t.obj_val as Logic.Object);
		Func<bool> func = delegate()
		{
			List<Logic.Object> possibleTargets2 = this.logic.GetPossibleTargets();
			return possibleTargets2 != null && possibleTargets2.Count != 0 && this.logic.Validate(true) == "ok";
		};
		Func<Value, bool> func2 = delegate(Value target)
		{
			Logic.Object @object = target.obj_val as Logic.Object;
			return !(this.logic.Validate(true) != "ok") && this.logic.ValidateArgs() && !(this.logic.ValidateTargetsAndArgs() != "ok") && (@object == null || this.logic.ValidateTarget(@object)) && (@object == null || this.logic.CheckCost(@object)) && this.logic.CheckCost(this.logic.target);
		};
		Action<BSGButton> action = delegate(BSGButton b)
		{
			Tooltip.Get(b.gameObject, true).SetObj(this.logic, null, vars);
		};
		List<TargetPickerData> eligableTargets = TargetPickerData.Create(possibleTargets, possibleTargetsVars, validate);
		Logic.Object suggestedObject = (worldUI != null) ? worldUI.selected_logic_obj : null;
		Action<Value> select = new Action<Value>(this.TargetPicked);
		Action cancel = new Action(this.TargetPickCancelled);
		Vars vars2 = vars;
		Func<bool> validate2 = func;
		Func<Value, bool> validateOkButtton = func2;
		Action<BSGButton> setupOkButtton = action;
		string targetPickerKey = this.logic.GetTargetPickerKey();
		DT.Field targetTable = ActionVisuals.GetTargetTable(this.logic);
		if (!UITargetSelectWindow.ShowDialog(eligableTargets, suggestedObject, select, cancel, vars2, validate2, validateOkButtton, setupOkButtton, targetPickerKey, ((targetTable != null) ? targetTable.key : null) ?? ""))
		{
			if (worldUI == null)
			{
				return;
			}
			if (!this.logic.ValidateTarget(worldUI.selected_logic_obj))
			{
				return;
			}
			this.TargetPicked(worldUI.selected_logic_obj);
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00049088 File Offset: 0x00047288
	public virtual void ShowArgsPicker(int arg_idx = 0)
	{
		List<Value>[] possibleArgs = this.logic.GetPossibleArgs();
		List<Value> list = (possibleArgs == null) ? new List<Value>() : possibleArgs[arg_idx];
		List<Vars> possibleArgVars = this.logic.GetPossibleArgVars(list, arg_idx);
		WorldUI worldUI = WorldUI.Get();
		Vars vars = new Vars(this.logic);
		this.last_arg_idx = arg_idx;
		if (this.logic != null && this.logic.def != null)
		{
			GameObject obj = global::Defs.GetObj<GameObject>(this.logic.def.id, "target_picker_prefab", null);
			if (obj)
			{
				if (worldUI == null)
				{
					return;
				}
				GameObject gameObject = global::Common.FindChildByName(worldUI.gameObject, "id_MessageContainer", true, true);
				if (gameObject != null)
				{
					UICommon.DeleteChildren(gameObject.transform, typeof(UIActionInfo));
				}
				GameObject gameObject2 = global::Common.Spawn(obj, false, false);
				gameObject2.transform.SetParent(gameObject.transform, false);
				UIActionInfo component = gameObject2.GetComponent<UIActionInfo>();
				if (component)
				{
					component.Init(this);
				}
				return;
			}
		}
		Func<bool> validate = delegate()
		{
			if (this.logic.NeedsTarget() && (this.logic.target == null || !this.logic.target.IsValid()))
			{
				return false;
			}
			if (this.logic.Validate(true) != "ok")
			{
				return false;
			}
			List<Value>[] possibleArgs2 = this.logic.GetPossibleArgs();
			return possibleArgs2 != null && possibleArgs2[arg_idx] != null && possibleArgs2[arg_idx].Count != 0;
		};
		Func<Value, bool> validateOkButtton = delegate(Value target)
		{
			Logic.Object @object = target.obj_val as Logic.Object;
			return !(this.logic.Validate(true) != "ok") && !(this.logic.ValidateTargetsAndArgs() != "ok") && (@object == null || this.logic.ValidateArg(@object, arg_idx)) && (@object == null || this.logic.CheckCost(@object)) && this.logic.CheckCost(this.logic.target);
		};
		Action<BSGButton> setupOkButtton = delegate(BSGButton b)
		{
			Tooltip.Get(b.gameObject, true).SetObj(this.logic, null, vars);
		};
		if (!UITargetSelectWindow.ShowDialog(TargetPickerData.Create(list, possibleArgVars, null), (worldUI != null) ? worldUI.selected_logic_obj : null, new Action<Value>(this.ArgPicked), new Action(this.ArgPickCancelled), vars, validate, validateOkButtton, setupOkButtton, this.logic.GetArgPickerKey(arg_idx), ""))
		{
			if (worldUI == null)
			{
				return;
			}
			if (!this.logic.ValidateTarget(worldUI.selected_logic_obj))
			{
				return;
			}
			this.ArgPicked(worldUI.selected_logic_obj);
		}
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x000023FD File Offset: 0x000005FD
	public virtual void HideTargetPicker()
	{
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00049274 File Offset: 0x00047474
	public void TargetPicked(Value target)
	{
		Logic.Object @object = target.obj_val as Logic.Object;
		if (@object == null)
		{
			this.logic.Cancel(false, true);
		}
		if (!this.logic.ValidateTarget(@object))
		{
			this.logic.Cancel(false, true);
			return;
		}
		string targetConfirmationMessageKey = this.logic.GetTargetConfirmationMessageKey(target);
		if (!string.IsNullOrEmpty(targetConfirmationMessageKey))
		{
			Vars vars = new Vars(this.logic);
			DT.Field @ref = this.logic.def.field.GetRef(targetConfirmationMessageKey, null, true, true, true, '.');
			if (@ref != null)
			{
				this.logic.target = @object;
				MessageWnd.Create(@ref, vars, null, new MessageWnd.OnButton(this.OnTargetConfirmationMessageButton));
				return;
			}
		}
		this.logic.Execute(@object);
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x00049334 File Offset: 0x00047534
	public void ArgPicked(Value target)
	{
		if (!this.logic.ValidateArg(target, this.last_arg_idx))
		{
			this.logic.Cancel(false, true);
			return;
		}
		if (this.logic.args == null)
		{
			this.logic.args = new List<Value>();
		}
		if (this.logic.args.Count > this.last_arg_idx)
		{
			this.logic.args[this.last_arg_idx] = target;
		}
		else
		{
			this.logic.args.Add(target);
		}
		this.logic.Execute(this.logic.target);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x000493D9 File Offset: 0x000475D9
	public void ArgPickCancelled()
	{
		if (this.logic.state != Action.State.PickingArgs)
		{
			return;
		}
		this.logic.Cancel(false, true);
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x000493F7 File Offset: 0x000475F7
	public void TargetPickCancelled()
	{
		if (this.logic.state != Action.State.PickingTarget)
		{
			return;
		}
		if (this.logic.owner == null)
		{
			return;
		}
		if (!this.logic.owner.IsValid())
		{
			return;
		}
		this.logic.Cancel(false, true);
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00049438 File Offset: 0x00047638
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "leave_state")
		{
			if (this.logic.state == Action.State.PickingTarget)
			{
				this.HideTargetPicker();
			}
			return;
		}
		if (message == "enter_state")
		{
			if (this.logic.state == Action.State.PickingTarget && this.logic.state_end_time != this.logic.state_start_time)
			{
				this.ShowTargetPicker();
				return;
			}
			if (this.logic.state != Action.State.PickingArgs || !(this.logic.state_end_time != this.logic.state_start_time))
			{
				if ((Action.State)param != this.logic.state)
				{
					if (this.logic.state == Action.State.Preparing)
					{
						Logic.Kingdom kingdom = this.logic.own_kingdom;
						if (this.logic.outcome_vars != null)
						{
							Logic.Kingdom kingdom2 = this.logic.outcome_vars.Get<Logic.Kingdom>("old_kingdom", null);
							if (kingdom2 != null)
							{
								kingdom = kingdom2;
							}
						}
						if (kingdom != null)
						{
							int id = kingdom.id;
							Logic.Kingdom kingdom3 = BaseUI.LogicKingdom();
							int? num = (kingdom3 != null) ? new int?(kingdom3.id) : null;
							if (id == num.GetValueOrDefault() & num != null)
							{
								BaseUI.PlayVoiceEvent(this.logic.GetVar("prepare_voice_line", this.logic, true), this.logic.GetVoiceVars());
								BaseUI.PlaySoundEvent(this.logic.def.field.GetRandomString("prepare_sound_effect", this.logic, "", true, true, true, '.'), null);
								if (this.logic.own_kingdom != this.logic.target_kingdom)
								{
									Logic.Kingdom target_kingdom = this.logic.target_kingdom;
									if (((target_kingdom != null) ? target_kingdom.religion : null) != null)
									{
										BackgroundMusic.OnTrigger("ActionMissionKingdomTrigger", this.logic.target_kingdom.religion.name);
									}
								}
							}
						}
						if (this.logic.owner is Logic.Kingdom && this.logic.owner == BaseUI.LogicKingdom())
						{
							MessageIcon.Create(this.logic, true);
						}
					}
					if (this.logic.state == Action.State.Running || (this.logic.state == Action.State.Preparing && this.logic.state_end_time != this.logic.state_start_time))
					{
						Logic.Kingdom kingdom4 = this.logic.own_kingdom;
						if (this.logic.outcome_vars != null)
						{
							Logic.Kingdom kingdom5 = this.logic.outcome_vars.Get<Logic.Kingdom>("old_kingdom", null);
							if (kingdom5 != null)
							{
								kingdom4 = kingdom5;
							}
						}
						if (kingdom4 != null)
						{
							int id2 = kingdom4.id;
							Logic.Kingdom kingdom6 = BaseUI.LogicKingdom();
							int? num = (kingdom6 != null) ? new int?(kingdom6.id) : null;
							if (id2 == num.GetValueOrDefault() & num != null)
							{
								BaseUI.PlayVoiceEvent(this.logic.GetVar("done_voice_line", this.logic, true), this.logic.GetVoiceVars());
								BaseUI.PlaySoundEvent(this.logic.def.field.GetRandomString("done_sound_effect", this.logic, "", true, true, true, '.'), null);
							}
						}
					}
				}
				return;
			}
			Logic.Object owner = this.logic.owner;
			if (((owner != null) ? owner.GetKingdom() : null) == BaseUI.LogicKingdom())
			{
				this.ShowArgsPicker(0);
				return;
			}
			this.ArgPickCancelled();
			return;
		}
		else
		{
			if (message == "show_picker")
			{
				this.ShowTargetPicker();
				return;
			}
			if (message == "show_args")
			{
				this.ShowArgsPicker(0);
				return;
			}
			if (message == "cancelled" && this.logic.own_kingdom == BaseUI.LogicKingdom() && this.logic.state >= this.logic.cancel_voice_first_state && this.logic.state <= this.logic.cancel_voice_last_state)
			{
				if (this.logic.cancelled_manually)
				{
					BaseUI.PlayVoiceEvent(this.logic.GetVar("cancel_voice_line", this.logic, true), this.logic.GetVoiceVars());
					return;
				}
				BaseUI.PlayVoiceEvent(this.logic.GetVar("cancelled_voice_line", this.logic, true), this.logic.GetVoiceVars());
			}
			return;
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00049878 File Offset: 0x00047A78
	public static void PlayDeclineSound(Action action, ActionVisuals.ExecuteDeclinedReason error)
	{
		string key = null;
		string key2 = null;
		switch (error)
		{
		case ActionVisuals.ExecuteDeclinedReason.Unavailable:
			key = "decline_voice_line";
			key2 = "action_unavailable";
			break;
		case ActionVisuals.ExecuteDeclinedReason.AlreadyActive:
			key = "decline_voice_line";
			key2 = "action_inactive";
			break;
		case ActionVisuals.ExecuteDeclinedReason.NoTarget:
		case ActionVisuals.ExecuteDeclinedReason.NoArgs:
			key = "decline_voice_line";
			key2 = "action_missing_targets";
			break;
		case ActionVisuals.ExecuteDeclinedReason.NoResources:
			key = "decline_voice_line";
			key2 = "action_cost_not_met";
			break;
		}
		DT.Field soundsDef = BaseUI.soundsDef;
		BaseUI.PlaySoundEvent((soundsDef != null) ? soundsDef.GetString(key2, null, "", true, true, true, '.') : null, null);
		Value? value = (action != null) ? new Value?(action.GetVar(key, action, true)) : null;
		BaseUI.PlayVoiceEvent((value != null) ? value.GetValueOrDefault() : null, (action != null) ? action.owner : null);
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00049948 File Offset: 0x00047B48
	public static ActionVisuals.ExecuteDeclinedReason AllowExecuteAction(Action action)
	{
		if (action == null)
		{
			return ActionVisuals.ExecuteDeclinedReason.Unavailable;
		}
		if (action.state != Action.State.Inactive)
		{
			return ActionVisuals.ExecuteDeclinedReason.AlreadyActive;
		}
		if (action.NeedsTarget())
		{
			List<Logic.Object> possibleTargets = action.GetPossibleTargets();
			if (possibleTargets == null || possibleTargets.Count == 0)
			{
				return ActionVisuals.ExecuteDeclinedReason.NoTarget;
			}
		}
		else if (action.NeedsArgs())
		{
			List<Value>[] possibleArgs = action.GetPossibleArgs();
			if (possibleArgs == null || possibleArgs.Length == 0)
			{
				return ActionVisuals.ExecuteDeclinedReason.NoArgs;
			}
		}
		else if (!action.CheckCost(null))
		{
			return ActionVisuals.ExecuteDeclinedReason.NoResources;
		}
		return ActionVisuals.ExecuteDeclinedReason.None;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x000499A4 File Offset: 0x00047BA4
	public static void ExecuteActionNoChecks(Action action)
	{
		if (action.actions != null)
		{
			for (int i = 0; i < action.actions.Count; i++)
			{
				if (action.actions[i].state == Action.State.PickingTarget || action.actions[i].state == Action.State.PickingArgs)
				{
					action.actions[i].SetState(Action.State.Inactive, true);
				}
			}
		}
		(action.visuals as ActionVisuals).Begin();
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00049A1C File Offset: 0x00047C1C
	public static void ExecuteAction(Action action)
	{
		ActionVisuals.ExecuteDeclinedReason executeDeclinedReason = ActionVisuals.AllowExecuteAction(action);
		if (executeDeclinedReason != ActionVisuals.ExecuteDeclinedReason.None)
		{
			ActionVisuals.PlayDeclineSound(action, executeDeclinedReason);
			return;
		}
		ActionVisuals.ExecuteActionNoChecks(action);
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00049A44 File Offset: 0x00047C44
	public static string GetRequirementText(Action action, DT.Field rf, Vars vars)
	{
		bool flag = action.ValidateRequirement(rf);
		string text_key = flag ? "Action.requirement_defaults.requrement_met" : "Action.requirement_defaults.requrement_not_met";
		vars.Set<DT.Field>("req_field", rf);
		vars.Set<bool>("met", flag);
		vars.Set<Value>("value", rf.Value(vars, true, true));
		string val = "#" + global::Defs.Localize(rf, "name", vars, null, true, true);
		vars.Set<string>("name", val);
		return global::Defs.Localize(text_key, vars, null, true, true);
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00049AC4 File Offset: 0x00047CC4
	public static string GetRequirementTexts(Action action)
	{
		if (action.def.requirements == null)
		{
			return null;
		}
		Vars vars = new Vars(action);
		StringBuilder stringBuilder = new StringBuilder("#");
		for (int i = 0; i < action.def.requirements.Count; i++)
		{
			DT.Field rf = action.def.requirements[i];
			string requirementText = ActionVisuals.GetRequirementText(action, rf, vars);
			if (!string.IsNullOrEmpty(requirementText))
			{
				stringBuilder.AppendLine(requirementText);
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00049B48 File Offset: 0x00047D48
	public static string EscapeDeadPrisoners(List<Logic.Character> characters)
	{
		if (characters.Count == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder("#");
		for (int i = 0; i < characters.Count; i++)
		{
			Logic.Character character = characters[i];
			if (character != null)
			{
				Vars vars = new Vars(character);
				string text = global::Defs.LocalizedObjName(character, vars, "", true);
				if (i != characters.Count - 1)
				{
					text += ", ";
				}
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.Append(text);
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x00049BD4 File Offset: 0x00047DD4
	private static DT.Field GetArgTable(Action logic, int arg_idx)
	{
		string argTableKey = logic.GetArgTableKey(arg_idx);
		if (!string.IsNullOrEmpty(argTableKey))
		{
			new Vars(logic);
			DT.Field @ref = logic.def.field.GetRef(argTableKey, null, true, true, true, '.');
			if (@ref != null)
			{
				return @ref;
			}
		}
		return null;
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00049C1C File Offset: 0x00047E1C
	private static DT.Field GetTargetTable(Action logic)
	{
		string targetTableKey = logic.GetTargetTableKey();
		if (!string.IsNullOrEmpty(targetTableKey))
		{
			new Vars(logic);
			DT.Field @ref = logic.def.field.GetRef(targetTableKey, null, true, true, true, '.');
			if (@ref != null)
			{
				return @ref;
			}
		}
		return null;
	}

	// Token: 0x04000608 RID: 1544
	public Action logic;

	// Token: 0x04000609 RID: 1545
	private int last_arg_idx = -1;

	// Token: 0x02000592 RID: 1426
	public enum ExecuteDeclinedReason
	{
		// Token: 0x040030D9 RID: 12505
		None,
		// Token: 0x040030DA RID: 12506
		Unavailable,
		// Token: 0x040030DB RID: 12507
		AlreadyActive,
		// Token: 0x040030DC RID: 12508
		NoTarget,
		// Token: 0x040030DD RID: 12509
		NoArgs,
		// Token: 0x040030DE RID: 12510
		NoResources
	}
}
