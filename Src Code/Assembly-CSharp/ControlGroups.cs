using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class ControlGroups : MonoBehaviour
{
	// Token: 0x060008A8 RID: 2216 RVA: 0x0005DCD4 File Offset: 0x0005BED4
	public static ControlGroups Get(Logic.Battle battle = null)
	{
		if (ControlGroups.instance != null)
		{
			return ControlGroups.instance;
		}
		ControlGroups.instance = new GameObject().AddComponent<ControlGroups>();
		ControlGroups.instance.name = "ControlGroupsManager";
		UnityEngine.Object.DontDestroyOnLoad(ControlGroups.instance);
		return ControlGroups.instance;
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0005DD21 File Offset: 0x0005BF21
	private void Update()
	{
		this.UpdateInput();
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0005DD2C File Offset: 0x0005BF2C
	public void ClearSquad(global::Squad squad)
	{
		if (squad == null)
		{
			return;
		}
		for (int i = squad.control_groups.Count - 1; i >= 0; i--)
		{
			int control_group = squad.control_groups[i];
			this.ClearSquad(squad, control_group);
		}
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0005DD70 File Offset: 0x0005BF70
	public void ClearSquad(global::Squad squad, int control_group)
	{
		List<GameObject> list;
		if (this.groups.TryGetValue(control_group, out list))
		{
			list.Remove(squad.gameObject);
		}
		squad.control_groups.Remove(control_group);
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0005DDA8 File Offset: 0x0005BFA8
	public void AddSquad(global::Squad squad, int group_index, List<GameObject> group = null)
	{
		if (squad == null)
		{
			return;
		}
		if (group == null && !this.groups.TryGetValue(group_index, out group))
		{
			return;
		}
		if (squad.control_groups.Contains(group_index))
		{
			return;
		}
		group.Add(squad.gameObject);
		squad.control_groups.Add(group_index);
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0005DDFC File Offset: 0x0005BFFC
	private void UpdateInput()
	{
		if (BattleMap.battle == null)
		{
			return;
		}
		KeyCode keyCode = KeyCode.Alpha0;
		while (keyCode < KeyCode.Alpha9)
		{
			if (UICommon.GetKeyUp(keyCode, UICommon.ModifierKey.None, UICommon.ModifierKey.None))
			{
				int num = keyCode - KeyCode.Alpha0;
				List<GameObject> list;
				if (!this.groups.TryGetValue(num, out list))
				{
					list = new List<GameObject>();
					this.groups[num] = list;
				}
				BattleViewUI battleViewUI = BattleViewUI.Get();
				if (UICommon.GetKey(KeyCode.LeftControl, false) || UICommon.GetKey(KeyCode.RightControl, false))
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						global::Squad component = list[i].GetComponent<global::Squad>();
						this.ClearSquad(component, num);
					}
					for (int j = 0; j < Troops.squads.Length; j++)
					{
						global::Squad squad = Troops.squads[j];
						if (!(squad == null) && squad.Selected)
						{
							GameObject gameObject = squad.gameObject;
							if (!(gameObject == null) && !list.Contains(gameObject))
							{
								this.AddSquad(squad, num, list);
							}
						}
					}
					UIBattleViewArmyContainer armyWindow = battleViewUI.GetArmyWindow();
					if (armyWindow != null)
					{
						armyWindow.Refresh();
					}
					battleViewUI.UpdateContexSelection();
					return;
				}
				if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
				{
					for (int k = 0; k < Troops.squads.Length; k++)
					{
						global::Squad squad2 = Troops.squads[k];
						if (!(squad2 == null) && squad2.Selected)
						{
							GameObject gameObject2 = squad2.gameObject;
							if (!(gameObject2 == null) && !list.Contains(gameObject2))
							{
								this.AddSquad(squad2, num, list);
							}
						}
					}
					UIBattleViewArmyContainer armyWindow2 = battleViewUI.GetArmyWindow();
					if (armyWindow2 != null)
					{
						armyWindow2.Refresh();
					}
					battleViewUI.UpdateContexSelection();
					return;
				}
				if (battleViewUI == null)
				{
					return;
				}
				battleViewUI.SelectObjects(null, false, false);
				battleViewUI.SelectObjects(list, false, true);
				return;
			}
			else
			{
				keyCode++;
			}
		}
	}

	// Token: 0x040006D5 RID: 1749
	private static ControlGroups instance;

	// Token: 0x040006D6 RID: 1750
	public Dictionary<int, List<GameObject>> groups = new Dictionary<int, List<GameObject>>();
}
