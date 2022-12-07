using System;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200024F RID: 591
public class ChatChannel : Hotspot, IPoolable
{
	// Token: 0x06002442 RID: 9282 RVA: 0x00145ABD File Offset: 0x00143CBD
	private void Start()
	{
		this.Init();
		this.LocalizeTexts();
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x00145ACB File Offset: 0x00143CCB
	protected override void OnEnable()
	{
		this.UpdateHighlight();
		base.OnEnable();
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x00145ADC File Offset: 0x00143CDC
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.m_inGameChat = global::Common.GetParentComponent<UIInGameChat>(base.gameObject);
		UICommon.FindComponents(this, false);
		if (this.m_UnseenChannelIcon != null)
		{
			this.m_UnseenChannelIcon.SetActive(false);
		}
		this.inited = true;
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x00145B2B File Offset: 0x00143D2B
	public void UpdateState()
	{
		if (this.m_UnseenChannelIcon != null)
		{
			this.m_UnseenChannelIcon.SetActive(this.useend);
		}
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x00145B4C File Offset: 0x00143D4C
	public void Rebuild()
	{
		this.LocalizeTexts();
		this.UpdateState();
	}

	// Token: 0x06002447 RID: 9287 RVA: 0x00145B5C File Offset: 0x00143D5C
	private void Update()
	{
		if (UnityEngine.Time.unscaledTime < this.m_lastUpdate + this.m_UpdateInterval)
		{
			return;
		}
		this.m_lastUpdate = UnityEngine.Time.unscaledTime;
		if (this.m_Text != null)
		{
			Game game = GameLogic.Get(false);
			if (this.channel == Chat.Channel.Whisper && game != null)
			{
				game.teams.GetPlayerById(this.whisperTargetId);
				int playerIndex = game.campaign.GetPlayerIndex(this.whisperTargetId, true);
				if (game.campaign.GetPlayerRuntimeState(playerIndex) == Campaign.RuntimeState.Playing)
				{
					this.m_Text.color = global::Defs.GetColor(playerIndex, "InGameChat", "channels.Whisper.color_player");
					return;
				}
				this.m_Text.color = global::Defs.GetColor(playerIndex, "InGameChat", "channels.Whisper.color_player_offline");
			}
		}
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x00145C1C File Offset: 0x00143E1C
	private void LocalizeTexts()
	{
		if (this.m_Text != null)
		{
			Game game = GameLogic.Get(false);
			if (this.channel == Chat.Channel.Whisper && game != null)
			{
				Game.Player playerById = game.teams.GetPlayerById(this.whisperTargetId);
				int playerIndex = game.campaign.GetPlayerIndex(this.whisperTargetId, true);
				if (game.campaign.GetPlayerRuntimeState(playerIndex) == Campaign.RuntimeState.Playing)
				{
					this.m_Text.color = global::Defs.GetColor(playerIndex, "InGameChat", "channels.Whisper.color_player");
				}
				else
				{
					this.m_Text.color = global::Defs.GetColor(playerIndex, "InGameChat", "channels.Whisper.color_player_offline");
				}
				Vars vars = new Vars();
				vars.Set<string>("player_name", "#" + playerById.ExtractPlayerName());
				UIText.SetText(this.m_Text, global::Defs.Localize("InGameChat.channels.Whisper.description", vars, null, true, true));
				return;
			}
			UIText.SetTextKey(this.m_Text, string.Format("InGameChat.channels.{0}.description", this.channel), null, null);
			this.m_Text.color = global::Defs.GetColor(0, "InGameChat", string.Format("channels.{0}.color", this.channel));
		}
	}

	// Token: 0x06002449 RID: 9289 RVA: 0x00145D49 File Offset: 0x00143F49
	public override void OnPointerEnter(PointerEventData e)
	{
		base.OnPointerEnter(e);
		this.UpdateHighlight();
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x00145D58 File Offset: 0x00143F58
	public override void OnPointerExit(PointerEventData e)
	{
		base.OnPointerExit(e);
		this.UpdateHighlight();
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x00145D67 File Offset: 0x00143F67
	public override void OnClick(PointerEventData e)
	{
		base.OnClick(e);
		if (this.m_inGameChat != null)
		{
			this.m_inGameChat.SetChannel(this.channel, this.whisperTargetId);
		}
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x00145D98 File Offset: 0x00143F98
	private void UpdateHighlight()
	{
		if (this.m_Hover != null)
		{
			this.m_Hover.gameObject.SetActive(this.mouse_in);
		}
		if (this.m_Selected != null)
		{
			if (this.channel == Chat.Channel.Whisper)
			{
				this.m_Selected.gameObject.SetActive(this.m_inGameChat.whisperTargetId == this.whisperTargetId);
				return;
			}
			this.m_Selected.gameObject.SetActive(this.m_inGameChat.CurrentActiveChannel == this.channel);
		}
	}

	// Token: 0x0600244D RID: 9293 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolSpawned()
	{
	}

	// Token: 0x0600244E RID: 9294 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolActivated()
	{
	}

	// Token: 0x0600244F RID: 9295 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDeactivated()
	{
	}

	// Token: 0x06002450 RID: 9296 RVA: 0x000023FD File Offset: 0x000005FD
	public void OnPoolDestroyed()
	{
	}

	// Token: 0x04001887 RID: 6279
	[UIFieldTarget("id_Selected")]
	private GameObject m_Selected;

	// Token: 0x04001888 RID: 6280
	[UIFieldTarget("id_Hover")]
	private GameObject m_Hover;

	// Token: 0x04001889 RID: 6281
	[UIFieldTarget("id_Text")]
	protected TextMeshProUGUI m_Text;

	// Token: 0x0400188A RID: 6282
	[UIFieldTarget("id_UnseenChannelIcon")]
	protected GameObject m_UnseenChannelIcon;

	// Token: 0x0400188B RID: 6283
	public Chat.Channel channel;

	// Token: 0x0400188C RID: 6284
	[HideInInspector]
	public string whisperTargetId = string.Empty;

	// Token: 0x0400188D RID: 6285
	private UIInGameChat m_inGameChat;

	// Token: 0x0400188E RID: 6286
	public bool Selected;

	// Token: 0x0400188F RID: 6287
	private bool inited;

	// Token: 0x04001890 RID: 6288
	public bool useend;

	// Token: 0x04001891 RID: 6289
	private float m_lastUpdate;

	// Token: 0x04001892 RID: 6290
	private float m_UpdateInterval = 1f;
}
