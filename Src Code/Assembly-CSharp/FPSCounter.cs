using System;
using System.Collections.Generic;
using System.Text;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;

// Token: 0x02000201 RID: 513
public class FPSCounter : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06001F40 RID: 8000 RVA: 0x00121630 File Offset: 0x0011F830
	private void Start()
	{
		this.txt = base.GetComponent<Text>();
		this.txt.raycastTarget = true;
		Tooltip.Get(base.gameObject, true).handler = new Tooltip.Handler(this.HandleTooltip);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x00121680 File Offset: 0x0011F880
	private void Sample()
	{
		Game game = GameLogic.Get(true);
		this.fps.Add(1f / UnityEngine.Time.unscaledDeltaTime);
		this.updated.Add(game.scheduler.iUpdated);
		this.expressions.Add(Expression.num_calcs);
		Logic.Multiplayer multiplayer = game.multiplayer;
		if (((multiplayer != null) ? multiplayer.tx_profiler : null) != null)
		{
			long sum = game.multiplayer.tx_profiler.packets.sum;
			this.tx.Add((float)(sum - this.last_tx_bytes));
			this.last_tx_bytes = sum;
		}
		Logic.Multiplayer multiplayer2 = game.multiplayer;
		if (((multiplayer2 != null) ? multiplayer2.rx_profiler : null) != null)
		{
			long sum2 = game.multiplayer.rx_profiler.packets.sum;
			this.rx.Add((float)(sum2 - this.last_rx_bytes));
			this.last_rx_bytes = sum2;
		}
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x0012175C File Offset: 0x0011F95C
	private void Update()
	{
		this.Sample();
		this.sb.Clear();
		FPSCounter.AddFPSText(this.sb, this.fps);
		this.txt.text = this.sb.ToString();
		if (this.details_rect != null && this.canvas != null && this.details_rect.gameObject.activeSelf)
		{
			this.fpsPlot.DrawGraph(UnityEngine.Time.unscaledDeltaTime);
			this.updatedPlot.DrawGraph(UnityEngine.Time.unscaledDeltaTime);
			this.expressionsPlot.DrawGraph(UnityEngine.Time.unscaledDeltaTime);
			this.txPlot.DrawGraph(UnityEngine.Time.unscaledDeltaTime);
			this.rxPlot.DrawGraph(UnityEngine.Time.unscaledDeltaTime);
			float num = 0.6f * (1f / this.canvas.scaleFactor);
			this.details_rect.position = new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f);
			this.details_rect.sizeDelta = new Vector2((float)Screen.width * num, (float)Screen.height * num);
			int num2 = (int)((float)Screen.width * num / 250f);
			for (int i = 0; i < this.details_texts.Length; i++)
			{
				UIText.SetText(this.details_texts[i], "");
			}
			if (num2 >= 3)
			{
				UIText.SetText(this.details_texts[0], this.GetObjectsInfoDetails());
				UIText.SetText(this.details_texts[1], this.GetTimeInfoDetails());
				this.sb.Clear();
				FPSCounter.AddMemoryInfoDetails(this.sb);
				this.sb.Append("\n\n");
				FPSCounter.AddNetworkInfoDetails(this.sb);
				UIText.SetText(this.details_texts[2], this.sb.ToString());
			}
			else
			{
				this.sb.Clear();
				FPSCounter.AddTimeInfoDetails(this.sb);
				this.sb.Append("\n\n");
				FPSCounter.AddMemoryInfoDetails(this.sb);
				this.sb.Append("\n\n");
				FPSCounter.AddObjectsInfoDetails(this.sb);
				UIText.SetText(this.details_texts[0], this.sb.ToString());
			}
			if (num2 >= 4)
			{
				if (UICommon.GetKey(KeyCode.RightControl, false))
				{
					DBGOffersData dbgoffersData = Offer.dbg_offers_data["CounterOffer"];
					DBGOffersData dbgoffersData2 = Offer.dbg_offers_data["SweetenOffer"];
					string text = "Counter:\n";
					string text2 = "Sweeten:\n";
					foreach (KeyValuePair<string, DBGOffersData.ArgData> keyValuePair in dbgoffersData.argsData)
					{
						DBGOffersData.ArgData value = keyValuePair.Value;
						text = string.Concat(new object[]
						{
							text,
							"\n",
							keyValuePair.Key,
							": (",
							keyValuePair.Value.total_times,
							") "
						});
						foreach (KeyValuePair<int, int> keyValuePair2 in value.times_on_argument)
						{
							text = string.Concat(new object[]
							{
								text,
								keyValuePair2.Key,
								":",
								keyValuePair2.Value,
								"-"
							});
						}
						text.Remove(text.Length - 1, 1);
					}
					foreach (KeyValuePair<string, DBGOffersData.ArgData> keyValuePair3 in dbgoffersData2.argsData)
					{
						DBGOffersData.ArgData value2 = keyValuePair3.Value;
						text2 = string.Concat(new object[]
						{
							text2,
							"\n",
							keyValuePair3.Key,
							": (",
							keyValuePair3.Value.total_times,
							") "
						});
						foreach (KeyValuePair<int, int> keyValuePair4 in value2.times_on_argument)
						{
							text2 = string.Concat(new object[]
							{
								text2,
								keyValuePair4.Key,
								":",
								keyValuePair4.Value,
								"-"
							});
						}
						text2.Remove(text2.Length - 1, 1);
					}
					UIText.SetText(this.details_texts[0], text + "\n\n\n\n\n\n\n\n\n\n");
					UIText.SetText(this.details_texts[1], text2 + "\n\n\n\n\n\n\n\n\n\n");
					UIText.SetText(this.details_texts[2], "");
					UIText.SetText(this.details_texts[3], "");
				}
				if (UICommon.GetKey(KeyCode.RightShift, false))
				{
					string text3 = "Offers:\n";
					foreach (KeyValuePair<string, DBGOffersData> keyValuePair5 in Offer.dbg_offers_data)
					{
						text3 = string.Concat(new object[]
						{
							text3,
							"\n",
							keyValuePair5.Key,
							": ",
							keyValuePair5.Value.sent,
							" - ",
							keyValuePair5.Value.validateWithoutArgs_totalTime / Mathf.Max((float)keyValuePair5.Value.validateWithoutArgs_count, 1f),
							"(",
							Mathf.Min(keyValuePair5.Value.validateWithoutArgs_maxTime, keyValuePair5.Value.validateWithoutArgs_minTime),
							"/",
							keyValuePair5.Value.validateWithoutArgs_maxTime,
							")ms - ",
							keyValuePair5.Value.fillOfferArgs_totalTime / Mathf.Max((float)keyValuePair5.Value.fillOfferArgs_count, 1f),
							"(",
							Mathf.Min(keyValuePair5.Value.fillOfferArgs_maxTime, keyValuePair5.Value.fillOfferArgs_minTime),
							"/",
							keyValuePair5.Value.fillOfferArgs_maxTime,
							")ms"
						});
					}
					UIText.SetText(this.details_texts[0], text3);
					UIText.SetText(this.details_texts[1], "");
					UIText.SetText(this.details_texts[2], "");
					UIText.SetText(this.details_texts[3], "");
				}
			}
		}
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x00121EA4 File Offset: 0x001200A4
	public static void AddFPSText(StringBuilder txt, global::Sampler fps)
	{
		if (SaveGame.save_thread != null)
		{
			txt.Append("SAVING ");
		}
		if (!Mathf.Approximately(UnityEngine.Time.timeScale, 1f))
		{
			if (UnityEngine.Time.timeScale == 0f)
			{
				txt.Append("Paused ");
			}
			else
			{
				txt.Append("Speed: ");
				txt.Append(UnityEngine.Time.timeScale.ToString("F1"));
				txt.Append(" ");
			}
		}
		txt.Append("FPS:");
		txt.Append(fps.avg.ToString("F1"));
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x00121F44 File Offset: 0x00120144
	private string GetTooltipText()
	{
		this.sb.Clear();
		this.sb.Append("#<align=left>");
		this.sb.Append("FPS: " + this.fps.ToString());
		this.sb.Append("\n--------------------------------------------------------\n");
		string text = Title.Version(false);
		if (!string.IsNullOrEmpty(text))
		{
			this.sb.Append("Build " + text);
			this.sb.Append("\n--------------------------------------------------------\n");
		}
		FPSCounter.AddTimeInfoDetails(this.sb);
		this.sb.Append("\n--------------------------------------------------------\n");
		FPSCounter.AddMemoryInfoDetails(this.sb);
		Game game = GameLogic.Get(true);
		bool flag;
		if (game == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Multiplayer multiplayer = game.multiplayer;
			flag = (((multiplayer != null) ? multiplayer.tx_profiler : null) != null);
		}
		if (!flag)
		{
			bool flag2;
			if (game == null)
			{
				flag2 = (null != null);
			}
			else
			{
				Logic.Multiplayer multiplayer2 = game.multiplayer;
				flag2 = (((multiplayer2 != null) ? multiplayer2.rx_profiler : null) != null);
			}
			if (!flag2)
			{
				goto IL_1A0;
			}
		}
		this.sb.Append("\n--------------------------------------------------------");
		if (game.multiplayer.tx_profiler != null)
		{
			int num;
			int num2;
			game.multiplayer.tx_profiler.CalcRates(out num, out num2);
			this.sb.Append("\nTX: " + ((float)num / 1000f).ToString("F1") + " K/s");
		}
		if (game.multiplayer.rx_profiler != null)
		{
			int num;
			int num2;
			game.multiplayer.rx_profiler.CalcRates(out num, out num2);
			this.sb.Append("\nRX: " + ((float)num / 1000f).ToString("F1") + " K/s");
		}
		IL_1A0:
		this.sb.Append("\n--------------------------------------------------------");
		this.sb.Append("\nObjects: " + game.num_objects);
		this.sb.Append(string.Concat(new object[]
		{
			"\nUpdates: ",
			this.updated.ToString(),
			" / ",
			game.scheduler.iTotal
		}));
		this.sb.Append("\nExpressions: " + this.expressions.ToString());
		return this.sb.ToString();
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x00122198 File Offset: 0x00120398
	public bool HandleTooltip(BaseUI ui, Tooltip tooltip, Tooltip.Event evt)
	{
		if (evt == Tooltip.Event.Update)
		{
			Profile.BeginSection("FPSCounter.GetTooltipText");
			string tooltipText = this.GetTooltipText();
			tooltip.SetText(tooltipText, null, null);
			Profile.EndSection("FPSCounter.GetTooltipText");
			return true;
		}
		return false;
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x001221D0 File Offset: 0x001203D0
	private string GetTimeInfoDetails()
	{
		this.sb.Clear();
		FPSCounter.AddTimeInfoDetails(this.sb);
		return this.sb.ToString();
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x001221F4 File Offset: 0x001203F4
	public static void AddTimeInfoDetails(StringBuilder txt)
	{
		int num = (int)(GameLogic.Get(true).session_time.milliseconds / 1000L);
		int num2 = num % 60;
		int num3 = num / 60;
		int num4 = num3 % 60;
		int num5 = num3 / 60;
		txt.Append("Speed:");
		txt.Append(string.Concat(new string[]
		{
			"\n    Time: ",
			num5.ToString(),
			":",
			num4.ToString("D2"),
			":",
			num2.ToString("D2")
		}));
		txt.Append("\n    Speed: " + UnityEngine.Time.timeScale.ToString("F1"));
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x001222AC File Offset: 0x001204AC
	public static void AddNetworkInfoDetails(StringBuilder txt, NetworkProfiler profiler, string prefix, string nl = "\n")
	{
		if (profiler == null)
		{
			return;
		}
		txt.Append(prefix);
		txt.Append(nl);
		object @lock = profiler.Lock;
		lock (@lock)
		{
			int num;
			int num2;
			profiler.CalcRates(out num, out num2);
			int num3;
			int num4;
			profiler.CalcTotalRates(out num3, out num4);
			txt.Append("Packets: " + profiler.packets.ToString());
			txt.Append(nl + "Messages: " + profiler.messages.ToString());
			txt.Append(nl + "Total PPS: " + num4);
			txt.Append(nl + "Total Rate: " + ((float)num3 / 1000f).ToString("F1") + " K/s");
			txt.Append(nl + "PPS: " + num2);
			txt.Append(nl + "Rate: " + ((float)num / 1000f).ToString("F1") + " K/s");
		}
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x001223D4 File Offset: 0x001205D4
	public static void AddNetworkInfoDetails(StringBuilder txt)
	{
		Game game = GameLogic.Get(true);
		bool flag;
		if (game == null)
		{
			flag = (null != null);
		}
		else
		{
			Logic.Multiplayer multiplayer = game.multiplayer;
			flag = (((multiplayer != null) ? multiplayer.tx_profiler : null) != null);
		}
		if (!flag)
		{
			bool flag2;
			if (game == null)
			{
				flag2 = (null != null);
			}
			else
			{
				Logic.Multiplayer multiplayer2 = game.multiplayer;
				flag2 = (((multiplayer2 != null) ? multiplayer2.rx_profiler : null) != null);
			}
			if (!flag2)
			{
				return;
			}
		}
		txt.Append("Network:");
		string nl = "\n    ";
		FPSCounter.AddNetworkInfoDetails(txt, game.multiplayer.tx_profiler, "\nTX:", nl);
		FPSCounter.AddNetworkInfoDetails(txt, game.multiplayer.rx_profiler, "\nRX:", nl);
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x0012245D File Offset: 0x0012065D
	private string GetMemoryInfoDetails()
	{
		this.sb.Clear();
		FPSCounter.AddMemoryInfoDetails(this.sb);
		return this.sb.ToString();
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x00122484 File Offset: 0x00120684
	public static void AddMemoryInfoDetails(StringBuilder txt)
	{
		int num = 1048576;
		txt.Append("Memory:");
		txt.Append("\n    Unity: " + Profiler.GetTotalAllocatedMemoryLong() / (long)num + " MB");
		txt.Append("\n    Mono: " + Profiler.GetMonoUsedSizeLong() / (long)num + " MB");
		txt.Append("\n    Textures: " + Texture.currentTextureMemory / (ulong)num + " MB");
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x0012250C File Offset: 0x0012070C
	private string GetObjectsInfoDetails()
	{
		this.sb.Clear();
		FPSCounter.AddObjectsInfoDetails(this.sb);
		return this.sb.ToString();
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x00122530 File Offset: 0x00120730
	public static void AddObjectsInfoDetails(StringBuilder txt)
	{
		Game game = GameLogic.Get(true);
		txt.Append("Updates: " + game.scheduler.iTotal);
		txt.Append(string.Concat(new object[]
		{
			"\nRelations: ",
			KingdomAndKingdomRelation.created - KingdomAndKingdomRelation.destroyed,
			" / ",
			KingdomAndKingdomRelation.created
		}));
		txt.Append(string.Concat(new object[]
		{
			"\nObjects: ",
			game.num_objects,
			" / ",
			game.num_created_objects
		}));
		foreach (KeyValuePair<Type, int> keyValuePair in game.num_objects_by_type)
		{
			txt.Append(string.Concat(new object[]
			{
				"\n    ",
				keyValuePair.Key.Name,
				": ",
				keyValuePair.Value
			}));
		}
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x00122664 File Offset: 0x00120864
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if (this.details_rect == null)
		{
			this.createDetails();
		}
		if (this.details_rect.gameObject.activeSelf)
		{
			this.details_rect.gameObject.SetActive(false);
			return;
		}
		this.details_rect.gameObject.SetActive(true);
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x001226BC File Offset: 0x001208BC
	public void createDetails()
	{
		this.details_rect = global::Common.FindChildByName(base.gameObject, "Details", true, true).GetComponent<RectTransform>();
		this.details_texts = this.details_rect.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
		this.canvas = base.gameObject.GetComponentInParent<Canvas>();
		this.fpsPlot = new BarPlot(this.fps, global::Common.FindChildByName(this.details_rect.gameObject, "FpsPlot", true, true));
		this.updatedPlot = new BarPlot(this.updated, global::Common.FindChildByName(this.details_rect.gameObject, "UpdatedPlot", true, true));
		this.expressionsPlot = new BarPlot(this.expressions, global::Common.FindChildByName(this.details_rect.gameObject, "ExpressionsPlot", true, true));
		this.txPlot = new BarPlot(this.tx, global::Common.FindChildByName(this.details_rect.gameObject, "TxPlot", true, true));
		this.rxPlot = new BarPlot(this.rx, global::Common.FindChildByName(this.details_rect.gameObject, "RxPlot", true, true));
	}

	// Token: 0x040014BE RID: 5310
	private global::Sampler fps = new global::Sampler(25);

	// Token: 0x040014BF RID: 5311
	private global::Sampler updated = new global::Sampler(60);

	// Token: 0x040014C0 RID: 5312
	private global::Sampler expressions = new global::Sampler(60);

	// Token: 0x040014C1 RID: 5313
	private global::Sampler tx = new global::Sampler(60);

	// Token: 0x040014C2 RID: 5314
	private long last_tx_bytes;

	// Token: 0x040014C3 RID: 5315
	private global::Sampler rx = new global::Sampler(60);

	// Token: 0x040014C4 RID: 5316
	private long last_rx_bytes;

	// Token: 0x040014C5 RID: 5317
	private Canvas canvas;

	// Token: 0x040014C6 RID: 5318
	private RectTransform details_rect;

	// Token: 0x040014C7 RID: 5319
	private Text txt;

	// Token: 0x040014C8 RID: 5320
	private TextMeshProUGUI[] details_texts;

	// Token: 0x040014C9 RID: 5321
	private BarPlot fpsPlot;

	// Token: 0x040014CA RID: 5322
	private BarPlot updatedPlot;

	// Token: 0x040014CB RID: 5323
	private BarPlot expressionsPlot;

	// Token: 0x040014CC RID: 5324
	private BarPlot txPlot;

	// Token: 0x040014CD RID: 5325
	private BarPlot rxPlot;

	// Token: 0x040014CE RID: 5326
	private StringBuilder sb = new StringBuilder(16384);
}
