using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class Skirmish
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x0600098E RID: 2446 RVA: 0x0006CCBF File Offset: 0x0006AEBF
	// (set) Token: 0x0600098D RID: 2445 RVA: 0x0006CCB6 File Offset: 0x0006AEB6
	public Vector3 pos { get; private set; }

	// Token: 0x0600098F RID: 2447 RVA: 0x0006CCC8 File Offset: 0x0006AEC8
	public override string ToString()
	{
		return "Skirmish\n" + string.Format("Alive : {0}, absolute {1}\n", this.num_alive, this.num_alive_absolute) + string.Format("Cavalry : {0}, absolute {1}\n", this.num_cavalry, this.num_cavalry_absolute) + string.Format("Fighting : {0}, absolute {1}", this.num_weapons, this.num_weapons_absolute);
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x0006CD40 File Offset: 0x0006AF40
	public static void CreateSkirmish(Squad squad, Squad enemy)
	{
		if (BattleMap.battle == null || BattleMap.battle.IsFinishing() || BattleMap.battle.battle_map_finished)
		{
			return;
		}
		if (enemy.skirmish != null)
		{
			enemy.skirmish.AddSquad(squad);
			return;
		}
		Skirmish skirmish = new Skirmish();
		skirmish.AddSquad(squad);
		skirmish.AddSquad(enemy);
		BattleMap.skirmishes.Add(skirmish);
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x0006CDA4 File Offset: 0x0006AFA4
	public void SetParameters()
	{
		this.CalcParams();
		foreach (KeyValuePair<string, EventInstance> keyValuePair in this.sound_events)
		{
			this.SetParameters(keyValuePair.Key);
		}
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0006CE04 File Offset: 0x0006B004
	private unsafe void CalcParams()
	{
		if (this.squads.Count == 0)
		{
			return;
		}
		this.pos = Vector3.zero;
		this.num_alive = 0f;
		this.num_cavalry = 0f;
		this.num_weapons = 0f;
		this.num_alive_absolute = 0;
		this.num_cavalry_absolute = 0;
		this.num_weapons_absolute = 0;
		int min_troops = BattleMap.battle.def.min_troops;
		int max_troops = BattleMap.battle.def.max_troops;
		int min_cavalry = BattleMap.battle.def.min_cavalry;
		int max_cavalry = BattleMap.battle.def.max_cavalry;
		int min_fighting = BattleMap.battle.def.min_fighting;
		int max_fighting = BattleMap.battle.def.max_fighting;
		for (int i = this.squads.Count - 1; i >= 0; i--)
		{
			Squad squad = this.squads[i];
			if (squad == null || squad.data == null)
			{
				this.squads.RemoveAt(i);
			}
			else
			{
				this.pos += this.squads[i].transform.position;
				int logic_alive = squad.data->logic_alive;
				this.num_alive_absolute += logic_alive;
				if (squad.def.is_cavalry)
				{
					this.num_cavalry_absolute += logic_alive;
				}
				Troops.Troop troop = squad.data->FirstTroop;
				while (troop <= squad.data->LastTroop)
				{
					if (!troop.HasFlags(Troops.Troop.Flags.Dead | Troops.Troop.Flags.Destroyed) && troop.HasFlags(Troops.Troop.Flags.Fighting))
					{
						this.num_weapons_absolute++;
					}
					troop = ++troop;
				}
			}
		}
		this.num_alive = Common.map((float)this.num_alive_absolute, (float)min_troops, (float)max_troops, 0f, 1f, true);
		this.num_cavalry = Common.map((float)this.num_cavalry_absolute, (float)min_cavalry, (float)max_cavalry, 0f, 1f, true);
		this.num_weapons = Common.map((float)this.num_weapons_absolute, (float)min_fighting, (float)max_fighting, 0f, 1f, true);
		this.pos /= (float)this.squads.Count;
		this.attributes_3d.forward = Vector3.forward.ToFMODVector();
		this.attributes_3d.up = Vector3.up.ToFMODVector();
		this.attributes_3d.position = this.pos.ToFMODVector();
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x0006D094 File Offset: 0x0006B294
	public void SetParameters(string Event)
	{
		EventInstance eventInstance = this.sound_events[Event];
		eventInstance.set3DAttributes(this.attributes_3d);
		eventInstance.setParameterByName("NumFight", (float)this.num_alive_absolute, false);
		eventInstance.setParameterByName("NumCavalry", (float)this.num_cavalry_absolute, false);
		eventInstance.setParameterByName("NumWeapons", (float)this.num_weapons_absolute, false);
		eventInstance.setParameterByName("bv_TroopsSize", this.num_alive, false);
		eventInstance.setParameterByName("bv_HorsesIntensity", this.num_cavalry, false);
		eventInstance.setParameterByName("bv_FightingIntensity", this.num_weapons, false);
		if (Squad.DebugAudio)
		{
			bool flag = false;
			for (int i = 0; i < this.squads.Count; i++)
			{
				Squad squad = this.squads[i];
				if (!(squad == null) && squad.data != null && squad.Selected)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				UnityEngine.Debug.Log(this.ToString());
			}
		}
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x0006D190 File Offset: 0x0006B390
	public void AddSquad(Squad squad)
	{
		if (squad.skirmish == this)
		{
			return;
		}
		this.squads.Add(squad);
		this.AddSoundEvent(squad.def.BattleSoundVoiceLoop);
		this.AddSoundEvent(squad.def.BattleSoundWeaponsLoop);
		this.AddSoundEvent(squad.def.BattleSoundHorsesLoop);
		squad.skirmish = this;
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x0006D1F0 File Offset: 0x0006B3F0
	public void AddSoundEvent(string Event)
	{
		if (string.IsNullOrEmpty(Event))
		{
			return;
		}
		foreach (KeyValuePair<string, EventInstance> keyValuePair in this.sound_events)
		{
			if (keyValuePair.Key == Event)
			{
				return;
			}
		}
		EventInstance value = FMODWrapper.CreateInstance(Event, false);
		this.sound_events[Event] = value;
		value.start();
	}

	// Token: 0x06000996 RID: 2454 RVA: 0x0006D274 File Offset: 0x0006B474
	public void DelSoundEvents(string Event, Squad squad)
	{
		if (string.IsNullOrEmpty(Event))
		{
			return;
		}
		for (int i = 0; i < this.squads.Count; i++)
		{
			Squad squad2 = this.squads[i];
			if (!(squad2 == squad) && squad2.def != null)
			{
				if (squad2.def.BattleSoundVoiceLoop == Event)
				{
					return;
				}
				if (squad2.def.BattleSoundWeaponsLoop == Event)
				{
					return;
				}
				if (squad2.def.BattleSoundHorsesLoop == Event)
				{
					return;
				}
			}
		}
		this.sound_events[Event].release();
		this.sound_events[Event].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this.sound_events.Remove(Event);
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x0006D334 File Offset: 0x0006B534
	public void DelSquad(Squad squad, bool set_parameters = true)
	{
		if (squad == null)
		{
			return;
		}
		this.squads.Remove(squad);
		if (squad.def == null)
		{
			return;
		}
		this.DelSoundEvents(squad.def.BattleSoundVoiceLoop, squad);
		this.DelSoundEvents(squad.def.BattleSoundWeaponsLoop, squad);
		this.DelSoundEvents(squad.def.BattleSoundHorsesLoop, squad);
		squad.skirmish = null;
		this.CheckOver();
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x0006D3A8 File Offset: 0x0006B5A8
	public bool CheckOver()
	{
		if (BattleMap.battle == null || BattleMap.battle.IsFinishing() || BattleMap.battle.battle_map_finished)
		{
			this.Destroy();
			return true;
		}
		bool flag = true;
		bool flag2 = true;
		for (int i = this.squads.Count - 1; i >= 0; i--)
		{
			Squad squad = this.squads[i];
			if (((squad != null) ? squad.logic : null) == null)
			{
				this.squads.RemoveAt(i);
			}
			else if (!this.squads[i].logic.IsDefeated())
			{
				int battle_side = this.squads[i].logic.battle_side;
				if (battle_side == 0)
				{
					flag = false;
				}
				else if (battle_side == 1)
				{
					flag2 = false;
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
		}
		if (flag || flag2)
		{
			this.Destroy();
			return true;
		}
		return false;
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x0006D470 File Offset: 0x0006B670
	public void Destroy()
	{
		for (int i = this.squads.Count - 1; i >= 0; i--)
		{
			this.squads[i].skirmish = null;
		}
		foreach (KeyValuePair<string, EventInstance> keyValuePair in this.sound_events)
		{
			keyValuePair.Value.release();
			keyValuePair.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		BattleMap.skirmishes.Remove(this);
	}

	// Token: 0x040007C4 RID: 1988
	public List<Squad> squads = new List<Squad>();

	// Token: 0x040007C5 RID: 1989
	public Dictionary<string, EventInstance> sound_events = new Dictionary<string, EventInstance>();

	// Token: 0x040007C6 RID: 1990
	private ATTRIBUTES_3D attributes_3d;

	// Token: 0x040007C8 RID: 1992
	private int num_alive_absolute;

	// Token: 0x040007C9 RID: 1993
	private int num_cavalry_absolute;

	// Token: 0x040007CA RID: 1994
	private int num_weapons_absolute;

	// Token: 0x040007CB RID: 1995
	private float num_alive;

	// Token: 0x040007CC RID: 1996
	private float num_cavalry;

	// Token: 0x040007CD RID: 1997
	private float num_weapons;
}
