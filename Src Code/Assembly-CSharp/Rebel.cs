using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200016F RID: 367
[Serializable]
public class Rebel : IListener
{
	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x060012BF RID: 4799 RVA: 0x000C3A74 File Offset: 0x000C1C74
	public GameObject Obj
	{
		get
		{
			if (this.logic == null)
			{
				return null;
			}
			if (this.logic.army == null)
			{
				return null;
			}
			GameLogic.Behaviour behaviour = this.logic.army.visuals as GameLogic.Behaviour;
			if (behaviour == null)
			{
				return null;
			}
			return behaviour.gameObject;
		}
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x000C3AC4 File Offset: 0x000C1CC4
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.Rebel rebel = logic_obj as Logic.Rebel;
		if (rebel == null)
		{
			return;
		}
		new global::Rebel().Init(rebel);
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x000C3AE7 File Offset: 0x000C1CE7
	public void Init(Logic.Rebel logic)
	{
		this.logic = logic;
		logic.visuals = this;
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x000C3AF8 File Offset: 0x000C1CF8
	public void OnMessage(object obj, string message, object param)
	{
		GameObject obj2 = this.Obj;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(message);
		if (num <= 1235150096U)
		{
			if (num != 225870362U)
			{
				if (num != 647584087U)
				{
					if (num != 1235150096U)
					{
						return;
					}
					if (!(message == "retreat"))
					{
						return;
					}
					if (obj2 != null && obj2.activeSelf)
					{
						FloatingText.Create(this.Obj, "FloatingTexts.Attacking", "rebel_retreat", null, false);
						return;
					}
				}
				else
				{
					if (!(message == "attack_target"))
					{
						return;
					}
					if (obj2 != null && obj2.activeSelf)
					{
						Vars vars = new Vars(this.logic);
						vars.Set<object>("target", param);
						string text = "rebel_attack";
						Castle castle = param as Castle;
						Logic.Settlement settlement = param as Logic.Settlement;
						Logic.Army army = param as Logic.Army;
						if (castle != null)
						{
							if (castle.battle != null && castle.battle.CanJoin(this.logic.army))
							{
								text = "rebel_assist_battle";
							}
							else
							{
								text = "rebel_siege";
							}
						}
						else if (settlement != null)
						{
							if (settlement.battle != null && settlement.battle.CanJoin(this.logic.army))
							{
								text = "rebel_assist_battle";
							}
							else
							{
								text = (((param as Logic.Settlement).type == "Keep") ? "rebel_siege" : "rebel_plunder");
							}
						}
						else if (army != null)
						{
							if (army.battle != null && army.battle.CanJoin(this.logic.army))
							{
								text = "rebel_assist_battle";
							}
							else
							{
								text = "rebel_attack";
							}
						}
						FloatingText.Create(this.Obj, "FloatingTexts.Attacking", text, vars, false);
						return;
					}
				}
			}
			else
			{
				if (!(message == "reinforced"))
				{
					return;
				}
				if (obj2 != null && obj2.activeSelf)
				{
					FloatingText.Create(this.Obj, "FloatingTexts.Normal", "rebel_reinforce", null, false);
					return;
				}
			}
		}
		else if (num <= 3294324549U)
		{
			if (num != 1260891610U)
			{
				if (num != 3294324549U)
				{
					return;
				}
				message == "destroy";
				return;
			}
			else
			{
				if (!(message == "reinforce"))
				{
					return;
				}
				if (this.logic.game.isInVideoMode)
				{
					return;
				}
				if (param != null && param is Point)
				{
					this.SpawnReinforcements(new Point?((Point)param));
					return;
				}
				this.SpawnReinforcements(null);
				return;
			}
		}
		else
		{
			if (num == 3385198376U)
			{
				message == "disband";
				return;
			}
			if (num != 3938270615U)
			{
				return;
			}
			if (!(message == "rest"))
			{
				return;
			}
			if (obj2 != null && obj2.activeSelf)
			{
				FloatingText.Create(this.Obj, "FloatingTexts.Attacking", "rebel_rest", null, false);
			}
		}
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x000C3DC4 File Offset: 0x000C1FC4
	private void SpawnReinforcements(Point? startingPoint = null)
	{
		GameObject obj = global::Defs.GetObj<GameObject>(this.logic.def.field, "reinforcements", null);
		if (obj == null)
		{
			return;
		}
		Logic.Realm realm_in = this.logic.army.realm_in;
		List<Logic.Settlement> list = new List<Logic.Settlement>();
		Point? point = null;
		if (startingPoint == null)
		{
			for (int i = 0; i < realm_in.settlements.Count; i++)
			{
				Logic.Settlement settlement = realm_in.settlements[i];
				if (!settlement.razed && settlement.IsActiveSettlement())
				{
					list.Add(realm_in.settlements[i]);
				}
			}
			if (list.Count > 0)
			{
				Logic.Settlement settlement2 = list[Random.Range(0, list.Count)];
				point = new Point?(settlement2.GetRandomExitPoint(true, false));
			}
		}
		else
		{
			point = startingPoint;
		}
		if (point != null)
		{
			GameObject gameObject;
			if (GameLogic.instance != null)
			{
				gameObject = global::Common.Spawn(obj, GameLogic.instance.transform, false, "Reinforcements");
			}
			else
			{
				gameObject = global::Common.Spawn(obj, false, false);
			}
			RebelReinforcements rebelReinforcements = gameObject.AddComponent<RebelReinforcements>();
			rebelReinforcements.SetPosition(point.Value);
			rebelReinforcements.SetTarget(this);
		}
	}

	// Token: 0x04000C96 RID: 3222
	public Logic.Rebel logic;
}
