using System;
using Logic;
using TMPro;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class UIBattleViewTimePanel : MonoBehaviour, IListener, IVars
{
	// Token: 0x06001C15 RID: 7189 RVA: 0x00109F60 File Offset: 0x00108160
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_SpeedVars = new Vars(this);
		if (this.m_ButtonSettings != null)
		{
			this.m_ButtonSettings.onClick = new BSGButton.OnClick(this.ToggleOptions);
		}
		if (this.m_ButtonPause != null)
		{
			this.m_ButtonPause.onClick = new BSGButton.OnClick(this.HanldePause);
			this.m_ButtonPause.AllowSelection(true);
			Tooltip.Get(this.m_ButtonPause.gameObject, true).SetDef("GamePauseButtonTooltip", this.m_SpeedVars);
		}
		if (this.m_ButtonPlayFast != null)
		{
			this.m_ButtonPlayFast.onClick = new BSGButton.OnClick(this.HandlePlayFast);
			this.m_ButtonPlayFast.AllowSelection(true);
			Tooltip.Get(this.m_ButtonPlayFast.gameObject, true).SetDef("GameSpeedButtonTooltip", this.m_SpeedVars);
			Tooltip.Get(this.m_ButtonPlayFast.gameObject, true).handler = new Tooltip.Handler(this.HandleSpeedButtonTooltip);
		}
		if (this.m_ButtonPlayFaster != null)
		{
			this.m_ButtonPlayFaster.onClick = new BSGButton.OnClick(this.HandlePlayFaster);
			this.m_ButtonPlayFaster.AllowSelection(true);
			Tooltip.Get(this.m_ButtonPlayFaster.gameObject, true).SetDef("GameSpeedButtonTooltip", this.m_SpeedVars);
			Tooltip.Get(this.m_ButtonPlayFaster.gameObject, true).handler = new Tooltip.Handler(this.HandleSpeedButtonTooltip);
		}
		if (this.m_ButtonPlay != null)
		{
			this.m_ButtonPlay.onClick = new BSGButton.OnClick(this.HandlePlay);
			this.m_ButtonPlay.AllowSelection(true);
			Tooltip.Get(this.m_ButtonPlay.gameObject, true).SetDef("GameSpeedButtonTooltip", this.m_SpeedVars);
			Tooltip.Get(this.m_ButtonPlay.gameObject, true).handler = new Tooltip.Handler(this.HandleSpeedButtonTooltip);
		}
		if (this.m_MuteButton != null)
		{
			this.m_MuteButton.onClick = new BSGButton.OnClick(this.HandleMute);
			this.m_MuteButton.AllowSelection(true);
		}
		if (this.m_UnmuteButton != null)
		{
			this.m_UnmuteButton.onClick = new BSGButton.OnClick(this.HandleUnmute);
			this.m_UnmuteButton.AllowSelection(true);
		}
		if (this.m_ValueSpeed != null)
		{
			Tooltip.Get(this.m_ValueSpeed.gameObject, true).SetDef("GameSpeedTooltip", this.m_SpeedVars);
		}
		GameSpeed.OnPaused += this.OnPaused;
		GameSpeed.OnSpeedChange += this.OnSpeedChange;
		this.battleLogic = BattleMap.battle;
		this.m_WindowDef = global::Defs.GetDefField("ImportantShortcutsPanel", null);
		this.m_Initalized = true;
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x0010A232 File Offset: 0x00108432
	private void OnSpeedChange(float obj)
	{
		this.UpdateSpeedSettings(true);
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x0010A232 File Offset: 0x00108432
	private void OnPaused(bool obj)
	{
		this.UpdateSpeedSettings(true);
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x000DF44F File Offset: 0x000DD64F
	private void Start()
	{
		UICommon.FindComponents(this, false);
	}

	// Token: 0x06001C19 RID: 7193 RVA: 0x0010A23C File Offset: 0x0010843C
	public void SetKingdom(Logic.Kingdom k)
	{
		this.Init();
		Logic.Kingdom kingdom = this.Kingdom;
		if (kingdom != null)
		{
			kingdom.DelListener(this);
		}
		this.Kingdom = k;
		Logic.Kingdom kingdom2 = this.Kingdom;
		if (kingdom2 != null)
		{
			kingdom2.AddListener(this);
		}
		Logic.Battle battle = this.battleLogic;
		if (battle != null)
		{
			battle.DelListener(this);
		}
		Logic.Battle battle2 = this.battleLogic;
		if (battle2 != null)
		{
			battle2.AddListener(this);
		}
		this.m_SpeedLabelVars.obj = k;
		this.SetupTooltips();
		this.UpdateSpeedSettings(true);
		this.UpdateMuteButtons();
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x0010A2C4 File Offset: 0x001084C4
	private void Update()
	{
		Logic.Kingdom kingdom = BaseUI.LogicKingdom();
		if (kingdom == null)
		{
			return;
		}
		if (this.Kingdom != kingdom && kingdom != null && kingdom.IsValid())
		{
			this.SetKingdom(kingdom);
			return;
		}
		this.UpdateSpeedSettings(false);
		this.UpdateMuteButtons();
		this.UpdateSpeedButtons();
		if (this.m_NextGameTimeUpdate < UnityEngine.Time.unscaledTime && !GameSpeed.IsPaused())
		{
			this.UpdateGameTime();
			this.m_NextGameTimeUpdate += this.m_GameTimeUpdateInterval;
		}
	}

	// Token: 0x06001C1B RID: 7195 RVA: 0x0010A338 File Offset: 0x00108538
	private void SetupTooltips()
	{
		Vars vars = new Vars(this.Kingdom);
		if (this.m_ButtonSettings != null)
		{
			Tooltip.Get(this.m_ButtonSettings.gameObject, true).SetDef("PauseMenuTooltip", vars);
		}
		if (this.m_MuteButton != null)
		{
			Tooltip.Get(this.m_MuteButton.gameObject, true).SetDef("ToggleAudioButtonTooltip", vars);
		}
		if (this.m_UnmuteButton != null)
		{
			Tooltip.Get(this.m_UnmuteButton.gameObject, true).SetDef("ToggleAudioButtonTooltip", vars);
		}
	}

	// Token: 0x06001C1C RID: 7196 RVA: 0x0010A3D4 File Offset: 0x001085D4
	private void UpdateMuteButtons()
	{
		this.m_MuteButton.gameObject.SetActive(UserSettings.MasterOn);
		this.m_UnmuteButton.gameObject.SetActive(!UserSettings.MasterOn);
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x0010A404 File Offset: 0x00108604
	private void UpdateSpeedSettings(bool force = false)
	{
		float currentGameSpeed = GameSpeed.CurrentGameSpeed;
		if (currentGameSpeed == this.m_prevSpeed && !force)
		{
			return;
		}
		bool val = GameSpeed.IsPaused();
		if (this.m_ValueSpeed != null)
		{
			this.m_SpeedLabelVars.Set<bool>("game_paused", val);
			this.m_SpeedLabelVars.Set<float>("game_speed", Mathf.Round(currentGameSpeed * 100f) / 100f);
			UIText.SetTextKey(this.m_ValueSpeed, "ImportantShortcuts.speed", this.m_SpeedLabelVars, null);
		}
		if (this.m_ValueTimePlayed != null)
		{
			UIText.SetTextKey(this.m_ValueTimePlayed, "SaveLoadMenuWindow.game_time", this.m_SpeedLabelVars, null);
		}
		this.m_prevSpeed = currentGameSpeed;
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x0010A4B0 File Offset: 0x001086B0
	private void UpdateGameTime()
	{
		if (this.m_ValueSpeed == null)
		{
			return;
		}
		Logic.Battle battle = BattleMap.battle;
		if (battle == null || battle.batte_view_game == null)
		{
			return;
		}
		int num = (int)((battle.batte_view_game.time.milliseconds - battle.battle_view_game_start.milliseconds) / 1000L);
		int num2 = num % 60;
		int num3 = num / 60;
		int num4 = num3 % 60;
		int num5 = num3 / 60;
		this.m_SpeedLabelVars.Set<string>("time_seconds", "#" + num2.ToString("D2"));
		this.m_SpeedLabelVars.Set<string>("time_minutes", "#" + num4.ToString("D2"));
		this.m_SpeedLabelVars.Set<string>("time_hours", "#" + num5.ToString("D2"));
		if (this.m_ValueSpeed != null)
		{
			UIText.SetTextKey(this.m_ValueSpeed, "ImportantShortcuts.speed", this.m_SpeedLabelVars, null);
		}
		if (this.m_ValueTimePlayed != null)
		{
			UIText.SetTextKey(this.m_ValueTimePlayed, "SaveLoadMenuWindow.game_time", this.m_SpeedLabelVars, null);
		}
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x0010A5D0 File Offset: 0x001087D0
	private void UpdateSpeedButtons()
	{
		bool flag = GameSpeed.IsPaused();
		Game game = GameLogic.Get(false);
		float currentSpeed = GameSpeed.GetCurrentSpeed();
		int speedBindSlot = GameSpeed.GetSpeedBindSlot();
		bool enable = game.pause.CanPause(-2);
		bool flag2 = flag && game.pause.CanUnpause(-2);
		if (this.m_ButtonPause != null)
		{
			this.m_ButtonPause.SetSelected(flag, true);
			if (!flag)
			{
				this.m_ButtonPause.Enable(enable, false);
			}
		}
		if (this.m_ButtonPlay != null)
		{
			this.m_ButtonPlay.Enable(flag2 || !flag, false);
			this.m_ButtonPlay.SetSelected(speedBindSlot == 0, false);
		}
		if (this.m_ButtonPlayFast != null)
		{
			this.m_ButtonPlayFast.Enable(flag2 || !flag, false);
			this.m_ButtonPlayFast.SetSelected(speedBindSlot == 1, false);
		}
		if (this.m_ButtonPlayFaster != null)
		{
			this.m_ButtonPlayFaster.Enable(flag2 || !flag, false);
			this.m_ButtonPlayFaster.SetSelected(speedBindSlot == 2, false);
		}
		if (this.m_UnderSpeed != null)
		{
			this.m_UnderSpeed.gameObject.SetActive(GameSpeed.IsSpeedUnderLowerBound(currentSpeed));
		}
		if (this.m_OverSpeed != null)
		{
			this.m_OverSpeed.gameObject.SetActive(GameSpeed.IsSpeedOverUpperBound(currentSpeed));
		}
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x0010A730 File Offset: 0x00108930
	private void ToggleOptions(BSGButton b)
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI)
		{
			baseUI.OnMenu();
			return;
		}
	}

	// Token: 0x06001C21 RID: 7201 RVA: 0x0010A754 File Offset: 0x00108954
	public bool HandleSpeedButtonTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt == Tooltip.Event.Fill || evt == Tooltip.Event.Update)
		{
			tooltip.vars.Set<bool>("speed_button_is_max", tooltip.gameObject == this.m_ButtonPlayFaster.gameObject);
			tooltip.vars.Set<float>("speed_key", this.GetSpeedAtButton(tooltip.gameObject));
		}
		return false;
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x0010A7AC File Offset: 0x001089AC
	private float GetSpeedAtButton(GameObject go)
	{
		if (go == this.m_ButtonPlay.gameObject)
		{
			return GameSpeed.GetSpeedAtBound(0);
		}
		if (go == this.m_ButtonPlayFast.gameObject)
		{
			return GameSpeed.GetSpeedAtBound(1);
		}
		if (go == this.m_ButtonPlayFaster.gameObject)
		{
			return GameSpeed.GetSpeedAtBound(2);
		}
		return 1f;
	}

	// Token: 0x06001C23 RID: 7203 RVA: 0x0010A80C File Offset: 0x00108A0C
	private void HanldePause(BSGButton b)
	{
		Game game = BattleMap.battle.game;
		if (game == null)
		{
			return;
		}
		Pause pause = game.pause;
		if (pause == null)
		{
			return;
		}
		pause.ToggleManualPause(-2);
	}

	// Token: 0x06001C24 RID: 7204 RVA: 0x0010A830 File Offset: 0x00108A30
	private void HandlePlay(BSGButton b)
	{
		Game game = BattleMap.battle.game;
		if (game == null)
		{
			return;
		}
		game.SetSpeed(GameSpeed.GetSpeedAtBound(0), -1);
	}

	// Token: 0x06001C25 RID: 7205 RVA: 0x0010A85C File Offset: 0x00108A5C
	private void HandlePlayFast(BSGButton b)
	{
		Game game = BattleMap.battle.game;
		if (game == null)
		{
			return;
		}
		game.SetSpeed(GameSpeed.GetSpeedAtBound(1), -1);
	}

	// Token: 0x06001C26 RID: 7206 RVA: 0x0010A888 File Offset: 0x00108A88
	private void HandlePlayFaster(BSGButton b)
	{
		Game game = BattleMap.battle.game;
		if (game == null)
		{
			return;
		}
		if (UICommon.GetKey(KeyCode.LeftShift, false) || UICommon.GetKey(KeyCode.RightShift, false))
		{
			game.SetSpeed(GameSpeed.GetMaxSpeed(), -1);
			return;
		}
		game.SetSpeed(GameSpeed.GetSpeedAtBound(2), -1);
	}

	// Token: 0x06001C27 RID: 7207 RVA: 0x0010A8D8 File Offset: 0x00108AD8
	private void HandleMute(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("volume_master_enabled");
		if (setting != null)
		{
			setting.ApplyValue(false);
		}
		this.UpdateMuteButtons();
	}

	// Token: 0x06001C28 RID: 7208 RVA: 0x0010A8FB File Offset: 0x00108AFB
	private void HandleUnmute(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("volume_master_enabled");
		if (setting != null)
		{
			setting.ApplyValue(true);
		}
		this.UpdateMuteButtons();
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x0010A920 File Offset: 0x00108B20
	public void OnMessage(object obj, string message, object param)
	{
		if (message == "stage_changed" && this.battleLogic.IsFinishing())
		{
			Logic.Battle battle = BattleMap.battle;
			Game game = (battle != null) ? battle.game : null;
			if (game != null)
			{
				game.SetSpeed(GameSpeed.GetSpeedAtBound(0), -1);
			}
		}
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x0010A969 File Offset: 0x00108B69
	private void OnDestroy()
	{
		GameSpeed.OnPaused -= this.OnPaused;
		GameSpeed.OnSpeedChange -= this.OnSpeedChange;
	}

	// Token: 0x06001C2B RID: 7211 RVA: 0x0010A990 File Offset: 0x00108B90
	public Value GetVar(string key, IVars vars = null, bool as_value = true)
	{
		if (key == "is_paused")
		{
			return GameSpeed.IsPaused();
		}
		if (!(key == "can_pause"))
		{
			if (!(key == "can_resume"))
			{
				if (key == "resume_cooldown_timer")
				{
					return GameSpeed.GetPauseResumeCooldown();
				}
				if (key == "cur_speed")
				{
					return GameSpeed.CurrentGameSpeed;
				}
				if (!(key == "max_speed"))
				{
					return Value.Unknown;
				}
				return GameSpeed.GetMaxSpeed();
			}
			else
			{
				Game game = GameLogic.Get(false);
				if (((game != null) ? game.pause : null) == null)
				{
					return true;
				}
				if (!game.IsPaused())
				{
					return false;
				}
				if (game.pause.CanUnpause(-2))
				{
					return true;
				}
				return false;
			}
		}
		else
		{
			Game game = GameLogic.Get(false);
			if (((game != null) ? game.pause : null) == null)
			{
				return true;
			}
			if (game.IsPaused())
			{
				return false;
			}
			if (game.pause.CanPause(-2))
			{
				return true;
			}
			return false;
		}
	}

	// Token: 0x04001248 RID: 4680
	[UIFieldTarget("id_MuteButton")]
	private BSGButton m_MuteButton;

	// Token: 0x04001249 RID: 4681
	[UIFieldTarget("id_UnmuteButton")]
	private BSGButton m_UnmuteButton;

	// Token: 0x0400124A RID: 4682
	[UIFieldTarget("id_ButtonSettings")]
	private BSGButton m_ButtonSettings;

	// Token: 0x0400124B RID: 4683
	[UIFieldTarget("id_ButtonPause")]
	private BSGButton m_ButtonPause;

	// Token: 0x0400124C RID: 4684
	[UIFieldTarget("id_ButtonPlay")]
	private BSGButton m_ButtonPlay;

	// Token: 0x0400124D RID: 4685
	[UIFieldTarget("id_ButtonPlayFast")]
	private BSGButton m_ButtonPlayFast;

	// Token: 0x0400124E RID: 4686
	[UIFieldTarget("id_ButtonPlayFaster")]
	private BSGButton m_ButtonPlayFaster;

	// Token: 0x0400124F RID: 4687
	[UIFieldTarget("id_UnderSpeed")]
	private GameObject m_UnderSpeed;

	// Token: 0x04001250 RID: 4688
	[UIFieldTarget("id_OverSpeed")]
	private GameObject m_OverSpeed;

	// Token: 0x04001251 RID: 4689
	[UIFieldTarget("id_ValueSpeed")]
	private TextMeshProUGUI m_ValueSpeed;

	// Token: 0x04001252 RID: 4690
	[UIFieldTarget("id_ValueTimePlayed")]
	private TextMeshProUGUI m_ValueTimePlayed;

	// Token: 0x04001253 RID: 4691
	private Logic.Kingdom Kingdom;

	// Token: 0x04001254 RID: 4692
	private bool m_Initalized;

	// Token: 0x04001255 RID: 4693
	private DT.Field m_WindowDef;

	// Token: 0x04001256 RID: 4694
	private Vars m_SpeedVars;

	// Token: 0x04001257 RID: 4695
	private Logic.Battle battleLogic;

	// Token: 0x04001258 RID: 4696
	private float m_NextGameTimeUpdate;

	// Token: 0x04001259 RID: 4697
	private float m_GameTimeUpdateInterval = 0.5f;

	// Token: 0x0400125A RID: 4698
	private float m_prevSpeed = -1f;

	// Token: 0x0400125B RID: 4699
	private Vars m_SpeedLabelVars = new Vars();
}
