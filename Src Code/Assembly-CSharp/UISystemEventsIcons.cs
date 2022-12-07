using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002C4 RID: 708
public class UISystemEventsIcons : MonoBehaviour, IListener
{
	// Token: 0x06002C6B RID: 11371 RVA: 0x001735F0 File Offset: 0x001717F0
	private void OnEnable()
	{
		UISystemEventsIcons.instances.Add(this);
		this.Init();
	}

	// Token: 0x06002C6C RID: 11372 RVA: 0x00173603 File Offset: 0x00171803
	private void OnDisable()
	{
		UISystemEventsIcons.instances.Remove(this);
	}

	// Token: 0x06002C6D RID: 11373 RVA: 0x00149C87 File Offset: 0x00147E87
	private void OnDestroy()
	{
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		game.DelListener(this);
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x00173614 File Offset: 0x00171814
	private void Init()
	{
		if (this.m_Initialzied)
		{
			return;
		}
		this.m_Initialzied = true;
		this.icons.Clear();
		if (base.transform.childCount < 1)
		{
			return;
		}
		GameObject gameObject = base.transform.GetChild(0).gameObject;
		gameObject.SetActive(false);
		while (base.transform.childCount > 1)
		{
			global::Common.DestroyObj(base.transform.GetChild(1));
		}
		DT.Field defField = global::Defs.GetDefField("SystemEventsIcons", null);
		if (((defField != null) ? defField.children : null) == null)
		{
			return;
		}
		for (int i = 0; i < defField.children.Count; i++)
		{
			DT.Field field = defField.children[i];
			if (!string.IsNullOrEmpty(field.key))
			{
				GameObject gameObject2 = global::Common.Spawn(gameObject, base.transform, false, "");
				if (!(gameObject2 == null))
				{
					Image component = gameObject2.GetComponent<Image>();
					if (component == null)
					{
						global::Common.DestroyObj(gameObject2);
						break;
					}
					gameObject2.name = field.key;
					UISystemEventsIcons.Icon icon = new UISystemEventsIcons.Icon
					{
						def = field,
						image = component,
						keep_visible = false,
						time_shown = -1f,
						min_visible_time = field.GetFloat("min_visible_time", null, 1f, true, true, true, '.'),
						vars = null
					};
					this.icons.Add(icon);
					Tooltip.Get(icon.image.gameObject, true).SetText("SystemEventsIcons." + field.key + ".tooltip", "SystemEventsIcons." + field.key + ".caption", new Vars(icon));
				}
			}
		}
		Game game = GameLogic.Get(false);
		if (game == null)
		{
			return;
		}
		game.AddListener(this);
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x001737D8 File Offset: 0x001719D8
	private static List<UISystemEventsIcons.Icon> FindIcons(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		if (UISystemEventsIcons.instances == null || UISystemEventsIcons.instances.Count == 0)
		{
			return null;
		}
		List<UISystemEventsIcons.Icon> list = new List<UISystemEventsIcons.Icon>();
		for (int i = 0; i < UISystemEventsIcons.instances.Count; i++)
		{
			UISystemEventsIcons uisystemEventsIcons = UISystemEventsIcons.instances[i];
			if (!(uisystemEventsIcons == null) && !(uisystemEventsIcons.gameObject == null))
			{
				UISystemEventsIcons.Icon icon = uisystemEventsIcons.FindIcon(id);
				if (icon != null)
				{
					list.Add(icon);
					break;
				}
			}
		}
		return list;
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x00173858 File Offset: 0x00171A58
	private static void FindIconsNonAlloc(string id, List<UISystemEventsIcons.Icon> result)
	{
		if (string.IsNullOrEmpty(id))
		{
			return;
		}
		if (UISystemEventsIcons.instances == null || UISystemEventsIcons.instances.Count == 0)
		{
			return;
		}
		if (result == null)
		{
			result = new List<UISystemEventsIcons.Icon>();
		}
		for (int i = 0; i < UISystemEventsIcons.instances.Count; i++)
		{
			UISystemEventsIcons uisystemEventsIcons = UISystemEventsIcons.instances[i];
			if (!(uisystemEventsIcons == null) && !(uisystemEventsIcons.gameObject == null))
			{
				UISystemEventsIcons.Icon icon = uisystemEventsIcons.FindIcon(id);
				if (icon != null)
				{
					result.Add(icon);
					return;
				}
			}
		}
	}

	// Token: 0x06002C71 RID: 11377 RVA: 0x001738D8 File Offset: 0x00171AD8
	private UISystemEventsIcons.Icon FindIcon(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		if (this.icons == null)
		{
			return null;
		}
		for (int i = 0; i < this.icons.Count; i++)
		{
			UISystemEventsIcons.Icon icon = this.icons[i];
			if (icon.def.key == id)
			{
				return icon;
			}
		}
		return null;
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x00173934 File Offset: 0x00171B34
	public static bool Show(string id, bool keep_visible = false, string icon_key = "icon", IVars vars = null)
	{
		UISystemEventsIcons.tmp_IconsList.Clear();
		UISystemEventsIcons.FindIconsNonAlloc(id, UISystemEventsIcons.tmp_IconsList);
		if (UISystemEventsIcons.tmp_IconsList == null || UISystemEventsIcons.tmp_IconsList.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < UISystemEventsIcons.tmp_IconsList.Count; i++)
		{
			UISystemEventsIcons.Icon icon = UISystemEventsIcons.tmp_IconsList[i];
			if (icon != null)
			{
				icon.Show(keep_visible, icon_key, vars);
			}
		}
		return true;
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x0017399C File Offset: 0x00171B9C
	public static bool Hide(string id)
	{
		UISystemEventsIcons.tmp_IconsList.Clear();
		UISystemEventsIcons.FindIconsNonAlloc(id, UISystemEventsIcons.tmp_IconsList);
		if (UISystemEventsIcons.tmp_IconsList == null || UISystemEventsIcons.tmp_IconsList.Count == 0)
		{
			return false;
		}
		bool result = true;
		for (int i = 0; i < UISystemEventsIcons.tmp_IconsList.Count; i++)
		{
			UISystemEventsIcons.Icon icon = UISystemEventsIcons.tmp_IconsList[i];
			if (icon != null)
			{
				if (icon.time_shown < 0f)
				{
					result = false;
				}
				else
				{
					icon.keep_visible = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x00173A14 File Offset: 0x00171C14
	private void Update()
	{
		this.UpdateConnectionProblems();
		for (int i = 0; i < this.icons.Count; i++)
		{
			UISystemEventsIcons.Icon icon = this.icons[i];
			if (icon.time_shown >= 0f && !icon.keep_visible && icon.time_shown + icon.min_visible_time <= UnityEngine.Time.unscaledTime)
			{
				icon.time_shown = -1f;
				icon.image.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x00173A90 File Offset: 0x00171C90
	private void UpdateConnectionProblems()
	{
		if (this.HasConnectionProblemsWithHost())
		{
			if (this.host_connection_problems)
			{
				return;
			}
			this.host_connection_problems = true;
			UISystemEventsIcons.Icon icon = this.FindIcon("ConnectionProblemsWithHost");
			if (icon != null)
			{
				icon.Show(true, "icon", null);
				return;
			}
		}
		else
		{
			if (!this.host_connection_problems)
			{
				return;
			}
			this.host_connection_problems = false;
			UISystemEventsIcons.Icon icon2 = this.FindIcon("ConnectionProblemsWithHost");
			if (icon2 != null)
			{
				icon2.Show(false, "icon", null);
			}
		}
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x00173B00 File Offset: 0x00171D00
	private bool HasConnectionProblemsWithHost()
	{
		Game game = GameLogic.Get(false);
		return ((game != null) ? game.pings : null) != null && game.multiplayer != null && !game.IsAuthority() && Logic.Multiplayer.multiplayer_settings != null && game.pings.TimeSinceLastPong() >= (long)Logic.Multiplayer.multiplayer_settings.connection_issues_timeout;
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x00173B5C File Offset: 0x00171D5C
	private void UpdateGameSpeed(Game game, int pid)
	{
		int num;
		if (game.IsPaused())
		{
			num = 0;
		}
		else
		{
			num = (int)game.speed;
			if (num > 4)
			{
				num = 4;
			}
			else if (num < 1)
			{
				num = 1;
			}
			else if (num == 3)
			{
				num = 2;
			}
		}
		if (num == this.last_game_speed)
		{
			return;
		}
		this.last_game_speed = num;
		this.speed_vars.Set<int>("new_speed", num);
		UISystemEventsIcons.Icon icon = this.FindIcon("GameSpeedChanged");
		if (icon != null)
		{
			icon.Show(num == 0, string.Format("icon{0}", num), this.speed_vars);
		}
	}

	// Token: 0x06002C78 RID: 11384 RVA: 0x00173BE4 File Offset: 0x00171DE4
	public void OnMessage(object obj, string message, object param)
	{
		Game game = obj as Game;
		if (message == "game_pause_changed")
		{
			this.UpdateGameSpeed(game, -1);
			return;
		}
		if (!(message == "game_speed_changed"))
		{
			return;
		}
		this.UpdateGameSpeed(game, game.last_speed_control_pid);
	}

	// Token: 0x04001E4E RID: 7758
	private List<UISystemEventsIcons.Icon> icons = new List<UISystemEventsIcons.Icon>();

	// Token: 0x04001E4F RID: 7759
	private static List<UISystemEventsIcons> instances = new List<UISystemEventsIcons>();

	// Token: 0x04001E50 RID: 7760
	private bool m_Initialzied;

	// Token: 0x04001E51 RID: 7761
	private static List<UISystemEventsIcons.Icon> tmp_IconsList = new List<UISystemEventsIcons.Icon>();

	// Token: 0x04001E52 RID: 7762
	private bool host_connection_problems;

	// Token: 0x04001E53 RID: 7763
	private int last_game_speed = 1;

	// Token: 0x04001E54 RID: 7764
	private Vars speed_vars = new Vars();

	// Token: 0x02000818 RID: 2072
	public class Icon : IVars
	{
		// Token: 0x06004F8A RID: 20362 RVA: 0x00235D30 File Offset: 0x00233F30
		public void Show(bool keep_visible = false, string icon_key = "icon", IVars vars = null)
		{
			if (!string.IsNullOrEmpty(icon_key))
			{
				Sprite obj = global::Defs.GetObj<Sprite>(this.def, icon_key, null);
				this.image.overrideSprite = obj;
			}
			this.time_shown = UnityEngine.Time.unscaledTime;
			this.keep_visible = keep_visible;
			this.vars = vars;
			this.image.gameObject.SetActive(true);
		}

		// Token: 0x06004F8B RID: 20363 RVA: 0x00235D8C File Offset: 0x00233F8C
		public Value GetVar(string key, IVars vars = null, bool as_value = true)
		{
			if (this.vars != null)
			{
				Value var = this.vars.GetVar(key, vars, as_value);
				if (!var.is_unknown)
				{
					return var;
				}
			}
			if (key == "is_saving")
			{
				return SaveGame.IsSaving();
			}
			return Value.Unknown;
		}

		// Token: 0x04003DB8 RID: 15800
		public DT.Field def;

		// Token: 0x04003DB9 RID: 15801
		public Image image;

		// Token: 0x04003DBA RID: 15802
		public bool keep_visible;

		// Token: 0x04003DBB RID: 15803
		public float time_shown;

		// Token: 0x04003DBC RID: 15804
		public float min_visible_time;

		// Token: 0x04003DBD RID: 15805
		public IVars vars;
	}
}
