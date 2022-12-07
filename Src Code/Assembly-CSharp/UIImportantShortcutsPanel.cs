using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002F7 RID: 759
public class UIImportantShortcutsPanel : MonoBehaviour, IListener, IVars
{
	// Token: 0x06002FAF RID: 12207 RVA: 0x001852E0 File Offset: 0x001834E0
	private void Init()
	{
		if (this.m_Initalized)
		{
			return;
		}
		UICommon.FindComponents(this, false);
		this.m_SpeedVars = new Vars(this);
		if (this.m_ButtonWars != null)
		{
			this.m_ButtonWars.onClick = new BSGButton.OnClick(this.ToggleWars);
			this.m_ButtonWars.AllowSelection(true);
		}
		if (this.m_ButtonRoyalFamily != null)
		{
			this.m_ButtonRoyalFamily.onClick = new BSGButton.OnClick(this.ToggleRoyalFamily);
			this.m_ButtonRoyalFamily.AllowSelection(true);
		}
		if (this.m_ButtonGamepedia != null)
		{
			this.m_ButtonGamepedia.onClick = new BSGButton.OnClick(this.ToggleGamepedia);
			this.m_ButtonGamepedia.AllowSelection(true);
		}
		if (this.m_PrestigeVictory != null)
		{
			this.m_PrestigeVictory.onClick = new BSGButton.OnClick(this.ToggleGreatpowers);
			this.m_PrestigeVictory.AllowSelection(true);
		}
		if (this.m_EconomyVictory != null)
		{
			this.m_EconomyVictory.onClick = new BSGButton.OnClick(this.ToggleAdvantages);
			this.m_EconomyVictory.AllowSelection(true);
		}
		if (this.n_ButtonProvinces != null)
		{
			this.n_ButtonProvinces.onClick = new BSGButton.OnClick(this.ToggleProvinces);
			this.n_ButtonProvinces.AllowSelection(true);
		}
		if (this.m_ButtonSettings != null)
		{
			this.m_ButtonSettings.onClick = new BSGButton.OnClick(this.ToggleOptions);
		}
		if (this.m_ButtonPause != null)
		{
			this.m_ButtonPause.onClick = new BSGButton.OnClick(this.HandlePause);
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
		UserSettings.OnSettingChange += this.SetupTooltips;
		this.m_WindowDef = global::Defs.GetDefField("ImportantShortcutsPanel", null);
		this.m_Initalized = true;
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x001856DE File Offset: 0x001838DE
	private void OnSpeedChange(float obj)
	{
		this.UpdateSpeedSettings(true);
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x001856DE File Offset: 0x001838DE
	private void OnPaused(bool obj)
	{
		this.UpdateSpeedSettings(true);
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x001856E7 File Offset: 0x001838E7
	private IEnumerator Start()
	{
		yield return null;
		bool flag = true;
		while (flag)
		{
			WorldUI ui = WorldUI.Get();
			if (ui == null)
			{
				yield return null;
			}
			if (ui.kingdom == 0)
			{
				yield return null;
			}
			flag = false;
			ui = null;
		}
		UICommon.FindComponents(this, false);
		yield break;
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x001856F8 File Offset: 0x001838F8
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
		this.m_SpeedLabelVars.obj = k;
		this.SetupTooltips();
		this.UpdateSpeedSettings(true);
		this.UpdateOpenPannels();
		this.UpdateMuteButtons();
		this.UpdateGameTime();
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x00185768 File Offset: 0x00183968
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
		this.UpdatePrestigeProgress();
		this.EconomyPrestigeProgress();
		this.UpdateOpenPannels();
		this.UpdateSpeedSettings(false);
		this.UpdateMuteButtons();
		this.UpdateSpeedButtons();
		this.UpdatePrestigeButton();
		this.UpdateEconomyButton();
		if (this.m_NextGameTimeUpdate < UnityEngine.Time.unscaledTime && !GameSpeed.IsPaused())
		{
			this.UpdateGameTime();
			this.m_NextGameTimeUpdate += this.m_GameTimeUpdateInterval;
		}
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x001857FC File Offset: 0x001839FC
	private void SetupTooltips()
	{
		Vars vars = new Vars(this.Kingdom);
		if (this.m_ButtonWars != null)
		{
			Tooltip.Get(this.m_ButtonWars.gameObject, true).SetDef("KingdomWarsTooltip", vars);
		}
		if (this.m_PrestigeVictory != null)
		{
			Tooltip.Get(this.m_PrestigeVictory.gameObject, true).SetDef("GreatPowersTooltip", vars);
		}
		if (this.m_EconomyVictory != null)
		{
			Tooltip.Get(this.m_EconomyVictory.gameObject, true).SetDef("GoodsVictoryTooltip", vars);
		}
		if (this.m_ButtonRoyalFamily != null)
		{
			Tooltip.Get(this.m_ButtonRoyalFamily.gameObject, true).SetDef("RoyalFamilyAndDiplomacyTooltip", vars);
		}
		if (this.m_ButtonGamepedia != null)
		{
			Tooltip.Get(this.m_ButtonGamepedia.gameObject, true).SetDef("GamePediaTooltip", vars);
		}
		if (this.n_ButtonProvinces != null)
		{
			Tooltip.Get(this.n_ButtonProvinces.gameObject, true).SetDef("PorvinceOverview", vars);
		}
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

	// Token: 0x06002FB6 RID: 12214 RVA: 0x00185994 File Offset: 0x00183B94
	private void UpdatePrestigeButton()
	{
		if (this.m_WindowDef == null)
		{
			return;
		}
		if (this.m_PrestigeVictory == null)
		{
			return;
		}
		BSGButtonImage component = this.m_PrestigeVictory.GetComponent<BSGButtonImage>();
		if (component == null)
		{
			return;
		}
		List<DT.Field> list = this.m_WindowDef.FindChild("prestige_victory_buttons", null, true, true, true, '.').Children();
		if (list == null)
		{
			return;
		}
		int num = (int)(Mathf.Clamp01(this.Kingdom.fame / this.Kingdom.required_fame_victory) * (float)(list.Count - 1));
		if (num == list.Count - 1 && !this.Kingdom.game.emperorOfTheWorld.ValidateVote(this.Kingdom))
		{
			num--;
		}
		if (this.m_CurrentPrestigeStage == num)
		{
			return;
		}
		this.m_CurrentPrestigeStage = num;
		Sprite obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentPrestigeStage], "normal", null);
		if (obj != null)
		{
			component.normalImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentPrestigeStage], "over", null);
		if (obj != null)
		{
			component.rolloverImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentPrestigeStage], "pressed", null);
		if (obj != null)
		{
			component.pressedImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentPrestigeStage], "disabled", null);
		if (obj != null)
		{
			component.disabledImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentPrestigeStage], "selected", null);
		if (obj != null)
		{
			component.selectedImage = obj;
		}
		this.m_PrestigeVictory.UpdateState(true);
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x00185B28 File Offset: 0x00183D28
	private void UpdatePrestigeProgress()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		float num = Mathf.Clamp01(this.Kingdom.fame / this.Kingdom.required_fame_victory);
		if (this.m_PrestigeProgress != null)
		{
			this.m_PrestigeProgress.fillAmount = num;
		}
		if (this.m_PrestigeThumb != null)
		{
			float width = (this.m_PrestigeThumb.transform.parent.transform as RectTransform).rect.width;
			Vector3 localPosition = this.m_PrestigeThumb.transform.localPosition;
			localPosition.x = -width / 2f + width * num;
			this.m_PrestigeThumb.transform.localPosition = localPosition;
		}
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x00185BE0 File Offset: 0x00183DE0
	private void EconomyPrestigeProgress()
	{
		if (this.Kingdom == null)
		{
			return;
		}
		float num = Mathf.Clamp01((float)(this.Kingdom.goods_produced.Count + this.Kingdom.goods_imported.Count) / (float)Resource.Def.total);
		if (this.m_EconomyProgress != null)
		{
			this.m_EconomyProgress.fillAmount = num;
		}
		if (this.m_EconomyThumb != null)
		{
			float width = (this.m_EconomyThumb.transform.parent.transform as RectTransform).rect.width;
			Vector3 localPosition = this.m_EconomyThumb.transform.localPosition;
			localPosition.x = -width / 2f + width * num;
			this.m_EconomyThumb.transform.localPosition = localPosition;
		}
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x00185CAC File Offset: 0x00183EAC
	private void UpdateEconomyButton()
	{
		if (this.m_WindowDef == null)
		{
			return;
		}
		if (this.m_EconomyVictory == null)
		{
			return;
		}
		BSGButtonImage component = this.m_EconomyVictory.GetComponent<BSGButtonImage>();
		if (component == null)
		{
			return;
		}
		List<DT.Field> list = this.m_WindowDef.FindChild("economy_victory_buttons", null, true, true, true, '.').Children();
		if (list == null)
		{
			return;
		}
		int num = (int)(Mathf.Clamp01((float)((this.Kingdom.goods_produced.Count + this.Kingdom.goods_imported.Count) / Resource.Def.total)) * (float)(list.Count - 1));
		if (this.m_CurrentEconomyStage == num)
		{
			return;
		}
		this.m_CurrentEconomyStage = num;
		Sprite obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentEconomyStage], "normal", null);
		if (obj != null)
		{
			component.normalImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentEconomyStage], "over", null);
		if (obj != null)
		{
			component.rolloverImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentEconomyStage], "pressed", null);
		if (obj != null)
		{
			component.pressedImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentEconomyStage], "disabled", null);
		if (obj != null)
		{
			component.disabledImage = obj;
		}
		obj = global::Defs.GetObj<Sprite>(list[this.m_CurrentEconomyStage], "selected", null);
		if (obj != null)
		{
			component.selectedImage = obj;
		}
		this.m_EconomyVictory.UpdateState(true);
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x00185E25 File Offset: 0x00184025
	private void UpdateMuteButtons()
	{
		this.m_MuteButton.gameObject.SetActive(UserSettings.MasterOn);
		this.m_UnmuteButton.gameObject.SetActive(!UserSettings.MasterOn);
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x00185E54 File Offset: 0x00184054
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

	// Token: 0x06002FBC RID: 12220 RVA: 0x00185F00 File Offset: 0x00184100
	private void UpdateGameTime()
	{
		int num = (int)(GameLogic.Get(true).session_time.milliseconds / 1000L);
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

	// Token: 0x06002FBD RID: 12221 RVA: 0x00185FF4 File Offset: 0x001841F4
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

	// Token: 0x06002FBE RID: 12222 RVA: 0x00186154 File Offset: 0x00184354
	private void UpdateOpenPannels()
	{
		if (this.m_ButtonWars != null)
		{
			this.m_ButtonWars.SetSelected(UIWarsOverviewWindow.IsActive(), false);
		}
		if (this.m_ButtonRoyalFamily != null)
		{
			this.m_ButtonRoyalFamily.SetSelected(UIRoyalFamily.IsActive(), false);
		}
		if (this.m_ButtonGamepedia != null)
		{
			this.m_ButtonGamepedia.SetSelected(UIWikiWindow.IsActive(), false);
		}
		if (this.m_PrestigeVictory != null)
		{
			this.m_PrestigeVictory.SetSelected(UIGreatPowersWindow.IsActive(), false);
		}
		if (this.n_ButtonProvinces != null)
		{
			this.n_ButtonProvinces.SetSelected(UIProvinceSelectorWindow.IsActive(), false);
		}
		if (this.m_EconomyVictory != null)
		{
			this.m_EconomyVictory.SetSelected(UIKingdomAdvantagesWindow.IsActive(), false);
		}
	}

	// Token: 0x06002FBF RID: 12223 RVA: 0x0018621B File Offset: 0x0018441B
	private void ToggleWars(BSGButton b)
	{
		UIWarsOverviewWindow.ToggleOpen(this.Kingdom, null, null);
	}

	// Token: 0x06002FC0 RID: 12224 RVA: 0x0018622A File Offset: 0x0018442A
	private void ToggleRoyalFamily(BSGButton b)
	{
		UIRoyalFamily.ToggleOpen(this.Kingdom);
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x00186237 File Offset: 0x00184437
	private void ToggleGamepedia(BSGButton b)
	{
		UIWikiWindow.ToggleOpen("", null);
	}

	// Token: 0x06002FC2 RID: 12226 RVA: 0x00186244 File Offset: 0x00184444
	private void ToggleGreatpowers(BSGButton b)
	{
		Logic.Kingdom kingdom = this.Kingdom;
		GreatPowers great_powers;
		if (kingdom == null)
		{
			great_powers = null;
		}
		else
		{
			Game game = kingdom.game;
			great_powers = ((game != null) ? game.great_powers : null);
		}
		UIGreatPowersWindow.ToggleOpen(great_powers);
	}

	// Token: 0x06002FC3 RID: 12227 RVA: 0x00186269 File Offset: 0x00184469
	private void ToggleAdvantages(BSGButton b)
	{
		UIKingdomAdvantagesWindow.ToggleOpen(this.Kingdom);
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x00186276 File Offset: 0x00184476
	private void ToggleProvinces(BSGButton b)
	{
		UIProvinceSelectorWindow.ToggleOpen(this.Kingdom);
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x00186284 File Offset: 0x00184484
	private void ToggleOptions(BSGButton b)
	{
		BaseUI baseUI = BaseUI.Get();
		if (baseUI)
		{
			baseUI.OnMenu();
			return;
		}
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x001862A8 File Offset: 0x001844A8
	public bool HandleSpeedButtonTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt == Tooltip.Event.Fill || evt == Tooltip.Event.Update)
		{
			tooltip.vars.Set<bool>("speed_button_is_max", tooltip.gameObject == this.m_ButtonPlayFaster.gameObject);
			tooltip.vars.Set<float>("speed_key", this.GetSpeedAtButton(tooltip.gameObject));
		}
		return false;
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x00186300 File Offset: 0x00184500
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

	// Token: 0x06002FC8 RID: 12232 RVA: 0x00186360 File Offset: 0x00184560
	private void HandlePause(BSGButton b)
	{
		Logic.Kingdom kingdom = this.Kingdom;
		Game game = (kingdom != null) ? kingdom.game : null;
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

	// Token: 0x06002FC9 RID: 12233 RVA: 0x0018638C File Offset: 0x0018458C
	private void HandlePlay(BSGButton b)
	{
		Logic.Kingdom kingdom = this.Kingdom;
		Game game = (kingdom != null) ? kingdom.game : null;
		if (game == null)
		{
			return;
		}
		game.SetSpeed(GameSpeed.GetSpeedAtBound(0), -1);
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x001863C0 File Offset: 0x001845C0
	private void HandlePlayFast(BSGButton b)
	{
		Logic.Kingdom kingdom = this.Kingdom;
		Game game = (kingdom != null) ? kingdom.game : null;
		if (game == null)
		{
			return;
		}
		game.SetSpeed(GameSpeed.GetSpeedAtBound(1), -1);
	}

	// Token: 0x06002FCB RID: 12235 RVA: 0x001863F4 File Offset: 0x001845F4
	private void HandlePlayFaster(BSGButton b)
	{
		Logic.Kingdom kingdom = this.Kingdom;
		Game game = (kingdom != null) ? kingdom.game : null;
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

	// Token: 0x06002FCC RID: 12236 RVA: 0x0018644C File Offset: 0x0018464C
	private void HandleMute(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("volume_master_enabled");
		if (setting != null)
		{
			setting.ApplyValue(false);
		}
		this.UpdateMuteButtons();
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x0018646F File Offset: 0x0018466F
	private void HandleUnmute(BSGButton b)
	{
		UserSettings.SettingData setting = UserSettings.GetSetting("volume_master_enabled");
		if (setting != null)
		{
			setting.ApplyValue(true);
		}
		this.UpdateMuteButtons();
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnMessage(object obj, string message, object param)
	{
	}

	// Token: 0x06002FCF RID: 12239 RVA: 0x00186492 File Offset: 0x00184692
	private void OnDestroy()
	{
		GameSpeed.OnPaused -= this.OnPaused;
		GameSpeed.OnSpeedChange -= this.OnSpeedChange;
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x001864B8 File Offset: 0x001846B8
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

	// Token: 0x0400200F RID: 8207
	[UIFieldTarget("id_MuteButton")]
	private BSGButton m_MuteButton;

	// Token: 0x04002010 RID: 8208
	[UIFieldTarget("id_UnmuteButton")]
	private BSGButton m_UnmuteButton;

	// Token: 0x04002011 RID: 8209
	[UIFieldTarget("id_ButtonWars")]
	private BSGButton m_ButtonWars;

	// Token: 0x04002012 RID: 8210
	[UIFieldTarget("id_ButtonRoyalFamily")]
	private BSGButton m_ButtonRoyalFamily;

	// Token: 0x04002013 RID: 8211
	[UIFieldTarget("id_ButtonGamepedia")]
	private BSGButton m_ButtonGamepedia;

	// Token: 0x04002014 RID: 8212
	[UIFieldTarget("id_PrestigeVictory")]
	private BSGButton m_PrestigeVictory;

	// Token: 0x04002015 RID: 8213
	[UIFieldTarget("id_PrestigeProgress")]
	private Image m_PrestigeProgress;

	// Token: 0x04002016 RID: 8214
	[UIFieldTarget("id_PrestigeThumb")]
	private GameObject m_PrestigeThumb;

	// Token: 0x04002017 RID: 8215
	[UIFieldTarget("id_EconomyVictory")]
	private BSGButton m_EconomyVictory;

	// Token: 0x04002018 RID: 8216
	[UIFieldTarget("id_EconomyProgress")]
	private Image m_EconomyProgress;

	// Token: 0x04002019 RID: 8217
	[UIFieldTarget("id_EconomyThumb")]
	private GameObject m_EconomyThumb;

	// Token: 0x0400201A RID: 8218
	[UIFieldTarget("id_ButtonProvinces")]
	private BSGButton n_ButtonProvinces;

	// Token: 0x0400201B RID: 8219
	[UIFieldTarget("id_ButtonSettings")]
	private BSGButton m_ButtonSettings;

	// Token: 0x0400201C RID: 8220
	[UIFieldTarget("id_ButtonPause")]
	private BSGButton m_ButtonPause;

	// Token: 0x0400201D RID: 8221
	[UIFieldTarget("id_ButtonPlay")]
	private BSGButton m_ButtonPlay;

	// Token: 0x0400201E RID: 8222
	[UIFieldTarget("id_ButtonPlayFast")]
	private BSGButton m_ButtonPlayFast;

	// Token: 0x0400201F RID: 8223
	[UIFieldTarget("id_ButtonPlayFaster")]
	private BSGButton m_ButtonPlayFaster;

	// Token: 0x04002020 RID: 8224
	[UIFieldTarget("id_UnderSpeed")]
	private GameObject m_UnderSpeed;

	// Token: 0x04002021 RID: 8225
	[UIFieldTarget("id_OverSpeed")]
	private GameObject m_OverSpeed;

	// Token: 0x04002022 RID: 8226
	[UIFieldTarget("id_ValueSpeed")]
	private TextMeshProUGUI m_ValueSpeed;

	// Token: 0x04002023 RID: 8227
	[UIFieldTarget("id_ValueTimePlayed")]
	private TextMeshProUGUI m_ValueTimePlayed;

	// Token: 0x04002024 RID: 8228
	private Logic.Kingdom Kingdom;

	// Token: 0x04002025 RID: 8229
	private bool m_Initalized;

	// Token: 0x04002026 RID: 8230
	private DT.Field m_WindowDef;

	// Token: 0x04002027 RID: 8231
	private Vars m_SpeedVars;

	// Token: 0x04002028 RID: 8232
	private float m_NextGameTimeUpdate;

	// Token: 0x04002029 RID: 8233
	private float m_GameTimeUpdateInterval = 0.3f;

	// Token: 0x0400202A RID: 8234
	private int m_CurrentPrestigeStage = -1;

	// Token: 0x0400202B RID: 8235
	private int m_CurrentEconomyStage = -1;

	// Token: 0x0400202C RID: 8236
	private float m_prevSpeed = -1f;

	// Token: 0x0400202D RID: 8237
	private Vars m_SpeedLabelVars = new Vars();
}
