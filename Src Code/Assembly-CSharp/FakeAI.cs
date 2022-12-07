using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;

// Token: 0x0200012D RID: 301
[Obsolete("Don't use this Behavior the true logic is implemented in Logic.AI")]
public class FakeAI : MonoBehaviour
{
	// Token: 0x0600101C RID: 4124 RVA: 0x000AC145 File Offset: 0x000AA345
	private void GetUnitPrefabs()
	{
		this.ArmyPrefab = global::Army.Prefab();
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x000AC152 File Offset: 0x000AA352
	private void Start()
	{
		this.GetUnitPrefabs();
		if (this.ArmyPrefab == null)
		{
			base.enabled = false;
			return;
		}
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x000AC170 File Offset: 0x000AA370
	private void UpdateArmies()
	{
		bool flag = false;
		for (int i = 0; i < this.armies.Count; i++)
		{
			global::Army army = this.armies[i];
			if (army == null || army.logic == null)
			{
				this.armies.RemoveAt(i);
				i--;
			}
			else
			{
				int num = this.UpdateArmy(army, flag);
				if (num > 0)
				{
					flag = true;
				}
				else if (num < 0)
				{
					global::Common.DestroyObj(army.gameObject);
					this.armies.RemoveAt(i);
					i--;
				}
			}
		}
		if (!flag && this.armies.Count < this.MaxArmies)
		{
			this.SpawnArmy();
		}
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x000AC214 File Offset: 0x000AA414
	private void LookForBattles()
	{
		for (int i = 0; i < this.armies.Count; i++)
		{
			global::Army army = this.armies[i];
			if (army.logic.battle == null && army.logic.castle == null)
			{
				for (int j = i + 1; j < this.armies.Count; j++)
				{
					global::Army army2 = this.armies[j];
					if (army2.logic.battle == null && army2.logic.castle == null && (army2.transform.position - army.transform.position).sqrMagnitude <= 9f && army.logic.IsEnemy(army2.logic))
					{
						Logic.Battle.StartBattle(army.logic, army2.logic, false);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x000AC300 File Offset: 0x000AA500
	private void SpawnArmy()
	{
		if (this.ArmyPrefab == null)
		{
			return;
		}
		if (global::Realm.Count() <= 0)
		{
			return;
		}
		global::Realm realm = global::Realm.Get(Random.Range(1, global::Realm.Count() + 1));
		if (!this.SpawnPlayerArmies)
		{
			WorldUI worldUI = WorldUI.Get();
			if (worldUI != null && realm.kingdom.id == worldUI.GetCurrentKingdomId())
			{
				return;
			}
		}
		if (realm.logic.castle == null || realm.logic.castle.army != null || realm.Neighbors.Count < 1)
		{
			return;
		}
		if (realm.logic.castle.visuals as global::Settlement == null)
		{
			return;
		}
		GameObject gameObject = global::Common.Spawn(this.ArmyPrefab, false, false);
		if (gameObject == null)
		{
			return;
		}
		global::Army component = gameObject.GetComponent<global::Army>();
		if (component == null)
		{
			return;
		}
		component.logic.AddNoble(true);
		component.transform.SetParent(base.transform, false);
		component.logic.position = (component.transform.position = realm.CastlePos);
		component.logic.leader.kingdom_id = (component.logic.kingdom_id = (component.kingdom = realm.kingdom.id));
		component.ResetFormation(true);
		component.logic.Start();
		component.logic.EnterCastle(realm.logic.castle, true);
		this.armies.Add(component);
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x000AC494 File Offset: 0x000AA694
	private void PlayVoice(global::Army a, string line)
	{
		if (!this.PlayVoices)
		{
			return;
		}
		if (this.VoiceOnlyIfVisible && !a.IsVisible())
		{
			return;
		}
		float time = UnityEngine.Time.time;
		if (time < this.last_move_voice_time + this.MinVoiceTime)
		{
			return;
		}
		if (Random.Range(0, 100) >= this.VoiceChance)
		{
			return;
		}
		this.last_move_voice_time = time;
		a.PlayVoiceLine(line, null, null);
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x000AC4F4 File Offset: 0x000AA6F4
	private int UpdateArmy(global::Army a, bool cleanup_only)
	{
		if (a.logic.battle != null)
		{
			if (a.logic.battle.settlement == null || a.logic != a.logic.battle.GetArmy(0))
			{
				return 0;
			}
			if (a.logic.game.time >= a.logic.battle.stage_time + 10f)
			{
				return -1;
			}
			return 0;
		}
		else if (a.logic.movement.path != null)
		{
			if (a.logic.movement.path.state == Path.State.Failed)
			{
				return -1;
			}
			this.PlayVoice(a, "move");
			return 0;
		}
		else
		{
			if (a.logic.realm_in == null)
			{
				return -1;
			}
			global::Realm realm = a.logic.realm_in.visuals as global::Realm;
			if (realm == null || realm.Neighbors.Count < 1)
			{
				return -1;
			}
			if (cleanup_only)
			{
				return 0;
			}
			if (a.units.Count < a.logic.MaxUnits() + 1 && a.logic.castle != null)
			{
				a.AddUnit("Militia", true, false);
				return 1;
			}
			int num = Random.Range((a.logic.castle == null) ? -1 : 0, realm.Neighbors.Count);
			global::Realm realm2 = null;
			if (num >= 0)
			{
				realm2 = global::Realm.Get(realm.Neighbors[num].rid);
			}
			if (realm2 == null)
			{
				realm2 = realm;
			}
			if (a.logic.castle == realm2.logic.castle)
			{
				return 0;
			}
			if (realm2 == null || realm2.logic == null || realm2.logic.castle == null || realm2.logic.castle.visuals == null)
			{
				return 0;
			}
			global::Settlement s = realm2.logic.castle.visuals as global::Settlement;
			a.MoveTo(s, false, true);
			return 1;
		}
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x000AC6C8 File Offset: 0x000AA8C8
	private void Update()
	{
		float time = UnityEngine.Time.time;
		if (time < this.last_update + 0.5f)
		{
			return;
		}
		this.last_update = time;
		this.UpdateArmies();
		this.LookForBattles();
	}

	// Token: 0x04000A94 RID: 2708
	public int MaxArmies = 10;

	// Token: 0x04000A95 RID: 2709
	public bool PlayVoices = true;

	// Token: 0x04000A96 RID: 2710
	public bool VoiceOnlyIfVisible = true;

	// Token: 0x04000A97 RID: 2711
	public float MinVoiceTime = 20f;

	// Token: 0x04000A98 RID: 2712
	public int VoiceChance = 5;

	// Token: 0x04000A99 RID: 2713
	public bool SpawnPlayerArmies;

	// Token: 0x04000A9A RID: 2714
	private GameObject ArmyPrefab;

	// Token: 0x04000A9B RID: 2715
	private List<global::Army> armies = new List<global::Army>();

	// Token: 0x04000A9C RID: 2716
	public float last_move_voice_time;

	// Token: 0x04000A9D RID: 2717
	private float last_update;
}
