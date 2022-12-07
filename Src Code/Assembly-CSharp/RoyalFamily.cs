using System;
using Logic;
using UnityEngine;

// Token: 0x02000175 RID: 373
[Serializable]
public class RoyalFamily : IListener
{
	// Token: 0x0600130E RID: 4878 RVA: 0x000C6E20 File Offset: 0x000C5020
	public static void CreateVisuals(Logic.Object logic_obj)
	{
		Logic.RoyalFamily royalFamily = logic_obj as Logic.RoyalFamily;
		if (royalFamily == null)
		{
			return;
		}
		new global::RoyalFamily().Init(royalFamily);
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x000C6E44 File Offset: 0x000C5044
	public void Init(Logic.RoyalFamily logic)
	{
		this.logic = logic;
		logic.visuals = this;
		Logic.Kingdom kingdom = logic.GetKingdom();
		if (kingdom != null)
		{
			kingdom.AddListener(this);
		}
	}

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06001310 RID: 4880 RVA: 0x000C6E70 File Offset: 0x000C5070
	// (remove) Token: 0x06001311 RID: 4881 RVA: 0x000C6EA8 File Offset: 0x000C50A8
	public event Action<object, string, object> onLogic;

	// Token: 0x06001312 RID: 4882 RVA: 0x000C6EE0 File Offset: 0x000C50E0
	public void OnMessage(object obj, string message, object param)
	{
		if (!(message == "destroying") && !(message == "finishing"))
		{
			if (!(message == "royal_new_born"))
			{
				if (!(message == "royal_new_sovereign"))
				{
					if (!(message == "royal_new_heir"))
					{
						if (message == "royal_new_spouse")
						{
							WorldUI worldUI = WorldUI.Get();
							if (worldUI != null)
							{
								global::Kingdom kingdom = global::Kingdom.Get(worldUI.GetCurrentKingdomId());
								Logic.Kingdom kingdom2 = obj as Logic.Kingdom;
								Logic.RoyalFamily royalFamily = (kingdom2 != null) ? kingdom2.royalFamily : null;
								if (kingdom != null && royalFamily != null && kingdom.id == royalFamily.kingdom_id && royalFamily.Spouse != null && royalFamily.Spouse != param)
								{
									string def_id = "RoyalFamilyMarriage";
									Vars vars = new Vars(royalFamily);
									MessageIcon.Create(def_id, vars, true, null);
								}
							}
						}
					}
				}
				else
				{
					WorldUI worldUI2 = WorldUI.Get();
					if (worldUI2 != null)
					{
						global::Kingdom kingdom3 = global::Kingdom.Get(worldUI2.GetCurrentKingdomId());
						Logic.Kingdom kingdom4 = obj as Logic.Kingdom;
						Logic.RoyalFamily royalFamily2 = (kingdom4 != null) ? kingdom4.royalFamily : null;
						if (kingdom3 != null && royalFamily2 != null && kingdom3.id == royalFamily2.kingdom_id)
						{
							Vars vars2 = null;
							if (param != null)
							{
								vars2 = (param as Vars);
							}
							if (vars2 == null)
							{
								vars2 = new Vars(royalFamily2.Sovereign);
								vars2.Set<bool>("isHeir", false);
							}
							bool flag = vars2.Get<bool>("isHeir", false);
							int num = global::Defs.GetInt("CrownAuthority", "kingDeath", null, 0);
							string def_id2 = "RoyalFamilyNewSovereign";
							if (!flag)
							{
								num += global::Defs.GetInt("CrownAuthority", "noSuccessor", null, 0);
							}
							vars2.Set<int>("crownAuthority", num);
							Vars vars3 = new Vars();
							vars3.Set<Logic.Character>("heir", flag ? royalFamily2.Sovereign : null);
							MessageIcon.Create(def_id2, vars2, true, vars3);
						}
					}
				}
			}
			else
			{
				WorldUI worldUI3 = WorldUI.Get();
				if (worldUI3 != null)
				{
					global::Kingdom kingdom5 = global::Kingdom.Get(worldUI3.GetCurrentKingdomId());
					Logic.Kingdom kingdom6 = obj as Logic.Kingdom;
					Logic.RoyalFamily royalFamily3 = (kingdom6 != null) ? kingdom6.royalFamily : null;
					if (kingdom5 != null && royalFamily3 != null && kingdom5.id == royalFamily3.kingdom_id && param is Logic.Character)
					{
						Logic.Character c = param as Logic.Character;
						Vars vars4 = new Vars(c);
						if (c.sex == Logic.Character.Sex.Male)
						{
							MessageIcon messageIcon = MessageIcon.Create("PrinceBorn", vars4, true, null);
							if (messageIcon != null)
							{
								messageIcon.on_update = delegate(MessageIcon i)
								{
									CharacterClass.Def class_def = c.class_def;
									string a;
									if (class_def == null)
									{
										a = null;
									}
									else
									{
										DT.Field field = class_def.field;
										a = ((field != null) ? field.key : null);
									}
									if (a != "ClasslessPrince")
									{
										i.Dismiss(true);
									}
								};
							}
						}
						else
						{
							MessageIcon.Create("PrincessBorn", vars4, true, null);
						}
					}
				}
			}
		}
		else if (obj == this.logic)
		{
			Logic.RoyalFamily royalFamily4 = this.logic;
			if (royalFamily4 != null)
			{
				Logic.Kingdom kingdom7 = royalFamily4.GetKingdom();
				if (kingdom7 != null)
				{
					kingdom7.DelListener(this);
				}
			}
			this.logic = null;
		}
		if (this.onLogic != null)
		{
			this.onLogic(obj, message, param);
		}
	}

	// Token: 0x04000CB0 RID: 3248
	public GameObject Obj;

	// Token: 0x04000CB1 RID: 3249
	public Logic.RoyalFamily logic;
}
