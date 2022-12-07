using System;
using System.Collections.Generic;
using FMODUnity;
using Logic;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class GameSpeed
{
	// Token: 0x14000003 RID: 3
	// (add) Token: 0x0600103B RID: 4155 RVA: 0x000ACC1C File Offset: 0x000AAE1C
	// (remove) Token: 0x0600103C RID: 4156 RVA: 0x000ACC50 File Offset: 0x000AAE50
	public static event Action<bool> OnPaused;

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x0600103D RID: 4157 RVA: 0x000ACC84 File Offset: 0x000AAE84
	// (remove) Token: 0x0600103E RID: 4158 RVA: 0x000ACCB8 File Offset: 0x000AAEB8
	public static event Action<float> OnSpeedChange;

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x0600103F RID: 4159 RVA: 0x000ACCEB File Offset: 0x000AAEEB
	// (set) Token: 0x06001040 RID: 4160 RVA: 0x000ACCF2 File Offset: 0x000AAEF2
	public static float CurrentGameSpeed { get; private set; }

	// Token: 0x06001041 RID: 4161 RVA: 0x000ACCFC File Offset: 0x000AAEFC
	static GameSpeed()
	{
		GameSpeed.PauseAudio(false);
		GameSpeed.CurrentGameSpeed = UnityEngine.Time.timeScale;
		GameSpeed.SetAudioSpeed(UnityEngine.Time.timeScale);
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x000ACDC4 File Offset: 0x000AAFC4
	private static void GetDef()
	{
		GameSpeed.def = global::Defs.GetDefField("GameSpeed", null);
		if (GameSpeed.def == null)
		{
			return;
		}
		GameSpeed.defVesion = global::Defs.Version;
		List<DT.SubValue> list = GameSpeed.def.GetValue("speed_map", null, true, true, true, '.').obj_val as List<DT.SubValue>;
		if (list != null)
		{
			GameSpeed.speedSteps.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].value.type == Value.Type.Int)
				{
					GameSpeed.speedSteps.Add((float)list[i].value.int_val);
				}
				else if (list[i].value.type == Value.Type.Float)
				{
					GameSpeed.speedSteps.Add(list[i].value.float_val);
				}
				else
				{
					GameSpeed.speedSteps.Add(1f);
				}
			}
		}
		List<DT.SubValue> list2 = GameSpeed.def.GetValue("speed_buttons", null, true, true, true, '.').obj_val as List<DT.SubValue>;
		if (list2 != null)
		{
			GameSpeed.slotBinds.Clear();
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].value.type == Value.Type.Int)
				{
					GameSpeed.slotBinds.Add(list2[j].value.int_val);
				}
				else
				{
					GameSpeed.slotBinds.Add(1);
				}
			}
		}
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x000ACF20 File Offset: 0x000AB120
	private static void CheckDef()
	{
		if (GameSpeed.defVesion == global::Defs.Version)
		{
			return;
		}
		GameSpeed.GetDef();
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x000ACF34 File Offset: 0x000AB134
	public static void OnSetSpeed(float speed)
	{
		GameSpeed.CurrentGameSpeed = speed;
		bool flag = GameLogic.Get(true).IsPaused();
		UnityEngine.Time.timeScale = (flag ? 0f : GameSpeed.CurrentGameSpeed);
		GameSpeed.PauseAudio(flag);
		GameSpeed.SetAudioSpeed(UnityEngine.Time.timeScale);
		if (GameSpeed.OnSpeedChange != null)
		{
			GameSpeed.OnSpeedChange(speed);
		}
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x000ACF87 File Offset: 0x000AB187
	public static void Pause(bool paused)
	{
		UnityEngine.Time.timeScale = (paused ? 0f : GameSpeed.CurrentGameSpeed);
		GameSpeed.PauseAudio(paused);
		GameSpeed.SetAudioSpeed(UnityEngine.Time.timeScale);
		if (GameSpeed.OnPaused != null)
		{
			GameSpeed.OnPaused(paused);
		}
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000ACFBF File Offset: 0x000AB1BF
	public static bool IsPaused()
	{
		return UnityEngine.Time.timeScale == 0f;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x000ACFCD File Offset: 0x000AB1CD
	private static void PauseAudio(bool paused)
	{
		if (paused)
		{
			GameSpeed.gamePauseSnapshot.StartSnapshot();
			return;
		}
		GameSpeed.gamePauseSnapshot.EndSnapshot();
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x000ACFE8 File Offset: 0x000AB1E8
	private static void SetAudioSpeed(float speed)
	{
		RuntimeManager.StudioSystem.setParameterByName("gameSpeed", speed, false);
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x000AD00C File Offset: 0x000AB20C
	public static float GetNextSpeedDown(Game g)
	{
		GameSpeed.CheckDef();
		float num = g.GetSpeed();
		int speedStep = GameSpeed.GetSpeedStep(num);
		if (speedStep != -1 && speedStep - 1 >= 0)
		{
			num = GameSpeed.speedSteps[speedStep - 1];
		}
		else if (Game.CheckCheatLevel(Game.CheatLevel.High, string.Empty, false))
		{
			num -= ((num > 1f) ? GameSpeed.maxIncrement : GameSpeed.minIcrement);
		}
		return Mathf.Max(num, 0.1f);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x000AD07C File Offset: 0x000AB27C
	public static float GetNextSpeedUp(Game g)
	{
		GameSpeed.CheckDef();
		float num = g.GetSpeed();
		int speedStep = GameSpeed.GetSpeedStep(num);
		if (speedStep != -1 && speedStep + 1 < GameSpeed.speedSteps.Count)
		{
			num = GameSpeed.speedSteps[speedStep + 1];
		}
		else if (Game.CheckCheatLevel(Game.CheatLevel.High, string.Empty, false))
		{
			num += ((num > 1f) ? GameSpeed.maxIncrement : GameSpeed.minIcrement);
		}
		return Mathf.Min(num, 100f);
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x000AD0F8 File Offset: 0x000AB2F8
	public static int GetSpeedStep(float speed)
	{
		GameSpeed.GetDef();
		int result = -1;
		for (int i = 0; i < GameSpeed.speedSteps.Count; i++)
		{
			if (GameSpeed.speedSteps[i] == speed)
			{
				result = i;
			}
		}
		return result;
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x000AD134 File Offset: 0x000AB334
	public static int GetSpeedBindSlot()
	{
		GameSpeed.GetDef();
		float currentSpeed = GameSpeed.GetCurrentSpeed();
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < GameSpeed.slotBinds.Count; i++)
		{
			float num2 = Math.Abs(GameSpeed.speedSteps[GameSpeed.slotBinds[i]] - currentSpeed);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	// Token: 0x0600104D RID: 4173 RVA: 0x000AD194 File Offset: 0x000AB394
	public static bool IsSpeedUnderLowerBound(float speed)
	{
		GameSpeed.GetDef();
		for (int i = 0; i < GameSpeed.speedSteps.Count; i++)
		{
			if (GameSpeed.slotBinds.Contains(i) && GameSpeed.speedSteps[i] <= speed)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600104E RID: 4174 RVA: 0x000AD1DC File Offset: 0x000AB3DC
	public static bool IsSpeedOverUpperBound(float speed)
	{
		GameSpeed.GetDef();
		for (int i = 0; i < GameSpeed.speedSteps.Count; i++)
		{
			if (GameSpeed.slotBinds.Contains(i) && GameSpeed.speedSteps[i] >= speed)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600104F RID: 4175 RVA: 0x000AD224 File Offset: 0x000AB424
	public static float GetSpeedAtBound(int idx)
	{
		GameSpeed.GetDef();
		int num;
		if (idx < 0)
		{
			num = GameSpeed.slotBinds[0];
		}
		else if (idx >= GameSpeed.slotBinds.Count)
		{
			num = GameSpeed.slotBinds[GameSpeed.slotBinds.Count - 1];
		}
		else
		{
			num = GameSpeed.slotBinds[idx];
		}
		if (num < 0)
		{
			return GameSpeed.speedSteps[0];
		}
		if (num > GameSpeed.speedSteps.Count)
		{
			return GameSpeed.speedSteps[GameSpeed.speedSteps.Count - 1];
		}
		return GameSpeed.speedSteps[num];
	}

	// Token: 0x06001050 RID: 4176 RVA: 0x000AD2BC File Offset: 0x000AB4BC
	public static float GetCurrentSpeed()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return UnityEngine.Time.timeScale;
		}
		return game.GetSpeed();
	}

	// Token: 0x06001051 RID: 4177 RVA: 0x000AD2DF File Offset: 0x000AB4DF
	public static float GetMaxSpeed()
	{
		if (GameSpeed.speedSteps == null || GameSpeed.speedSteps.Count < 1)
		{
			return 10f;
		}
		return GameSpeed.speedSteps[GameSpeed.speedSteps.Count - 1];
	}

	// Token: 0x06001052 RID: 4178 RVA: 0x000AD314 File Offset: 0x000AB514
	public static Value GetPauseResumeCooldown()
	{
		Game game = GameLogic.Get(false);
		if (((game != null) ? game.pause : null) == null)
		{
			return Value.Null;
		}
		float num;
		if (game.IsPaused())
		{
			num = game.pause.CalcCannotUnpauseTime(-2);
		}
		else
		{
			num = game.pause.CalcCannotPauseTime(-2);
		}
		if (num <= 0f)
		{
			return Value.Null;
		}
		if (num == float.PositiveInfinity)
		{
			return Value.Null;
		}
		return num;
	}

	// Token: 0x04000AAF RID: 2735
	private static float maxIncrement = 10f;

	// Token: 0x04000AB0 RID: 2736
	private static float minIcrement = 0.1f;

	// Token: 0x04000AB1 RID: 2737
	private static DT.Field def;

	// Token: 0x04000AB2 RID: 2738
	private static int defVesion;

	// Token: 0x04000AB3 RID: 2739
	private static List<float> speedSteps = new List<float>
	{
		0.5f,
		1f,
		2f,
		4f,
		6f,
		8f,
		10f
	};

	// Token: 0x04000AB4 RID: 2740
	private static List<int> slotBinds = new List<int>
	{
		1,
		2,
		3
	};

	// Token: 0x04000AB5 RID: 2741
	public static bool SupressSpeedChangesByPlayer = false;

	// Token: 0x04000AB6 RID: 2742
	private static FMODWrapper.Snapshot gamePauseSnapshot = new FMODWrapper.Snapshot("pause_game_snapshot");
}
