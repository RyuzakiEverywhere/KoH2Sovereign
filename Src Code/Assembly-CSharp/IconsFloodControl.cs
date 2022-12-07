using System;
using System.Collections.Generic;
using Logic;

// Token: 0x02000208 RID: 520
public class IconsFloodControl
{
	// Token: 0x06001FAB RID: 8107 RVA: 0x00124698 File Offset: 0x00122898
	public void OnUpdate()
	{
		this.time = GameLogic.Get(true).time;
		for (int i = 0; i < this.futureTimeline.Count; i++)
		{
			if (this.futureTimeline[i].time <= this.time.milliseconds)
			{
				this.futureTimeline[i].OnTimeComes(this.futureTimeline[i].iconCache);
				this.pastTimeline.Add(this.futureTimeline[i]);
				this.futureTimeline.RemoveAt(i);
				int num = this.pastTimeline.Count;
				for (int j = 0; j < num; j++)
				{
					if (this.time.milliseconds - this.pastTimeline[j].time > 15000L)
					{
						this.pastTimeline.RemoveAt(j);
						num--;
						j--;
					}
				}
				return;
			}
		}
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x00124790 File Offset: 0x00122990
	public MessageIcon Push(IconsFloodControl.IconCache cache, IconsFloodControl.Callback cb)
	{
		IconsFloodControl.TimelineEvent timelineEvent = new IconsFloodControl.TimelineEvent(cache, cb);
		MessageIcon result = null;
		int @int = timelineEvent.iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.');
		float weight = timelineEvent.iconCache.weight;
		this.time = GameLogic.Get(true).time;
		if (@int == 0)
		{
			timelineEvent.time = this.time.milliseconds;
			result = timelineEvent.OnTimeComes(timelineEvent.iconCache);
			this.pastTimeline.Add(timelineEvent);
			this.RecalculateFutureTimes();
		}
		else
		{
			int num = this.FindBestIndex(@int, weight);
			timelineEvent.time = this.GetTimeOfFutureIndex(num);
			if (timelineEvent.time - this.time.milliseconds > (long)(timelineEvent.iconCache.def_field.GetInt("max_delay", null, 0, true, true, true, '.') * 1000))
			{
				return null;
			}
			this.futureTimeline.Insert(num, timelineEvent);
			this.RecalculateFutureTimes();
		}
		return result;
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x00124884 File Offset: 0x00122A84
	private void RecalculateFutureTimes()
	{
		if (this.futureTimeline.Count == 0)
		{
			return;
		}
		float num = 0f;
		if (this.pastTimeline.Count > 0)
		{
			float num2 = (float)(this.futureTimeline[0].time - this.pastTimeline[this.pastTimeline.Count - 1].time);
			if (num2 < 1000f)
			{
				num = 1000f - num2;
			}
		}
		for (int i = 0; i < this.futureTimeline.Count; i++)
		{
			this.futureTimeline[i].time += (long)num;
			if (i > 0 && this.futureTimeline[i].time <= this.futureTimeline[i - 1].time)
			{
				float num3 = (float)(this.futureTimeline[i - 1].time - this.futureTimeline[i].time);
				if (num3 < 1000f)
				{
					num3 = 1000f - num3;
				}
				for (int j = i; j < this.futureTimeline.Count; j++)
				{
					this.futureTimeline[j].time += (long)num3;
				}
			}
		}
		for (int k = this.futureTimeline.Count - 1; k >= 0; k--)
		{
			if (this.futureTimeline[k].time - this.time.milliseconds > (long)(this.futureTimeline[k].iconCache.def_field.GetInt("max_delay", null, 0, true, true, true, '.') * 1000))
			{
				long num4;
				if (k > 0)
				{
					num4 = this.futureTimeline[k].time - this.futureTimeline[k - 1].time;
				}
				else
				{
					num4 = this.futureTimeline[0].time - this.pastTimeline[this.pastTimeline.Count - 1].time;
				}
				this.futureTimeline.RemoveAt(k);
				for (int l = k; l < this.futureTimeline.Count; l++)
				{
					this.futureTimeline[l].time -= num4;
				}
			}
		}
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x00124AE0 File Offset: 0x00122CE0
	private long GetTimeOfFutureIndex(int idx)
	{
		if (this.futureTimeline.Count == 0)
		{
			return GameLogic.Get(true).time.milliseconds;
		}
		if (idx >= this.futureTimeline.Count)
		{
			return this.futureTimeline[this.futureTimeline.Count - 1].time;
		}
		if (idx <= 0)
		{
			return this.futureTimeline[0].time;
		}
		return this.futureTimeline[idx].time;
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x00124B60 File Offset: 0x00122D60
	private int GetSameAndHigherPriorityCountInPast(int pr, long backTimeLimit)
	{
		int num = 0;
		int num2 = this.pastTimeline.Count - 1;
		while (num2 >= 0 && this.pastTimeline[num2].time > this.time.milliseconds - backTimeLimit)
		{
			if (this.pastTimeline[num2].iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.') <= pr)
			{
				num++;
			}
			num2--;
		}
		return num;
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x00124BD8 File Offset: 0x00122DD8
	private int GetSameAndHigherPriorityCountInFuture(int pr)
	{
		int num = 0;
		int num2 = 0;
		while (num2 < this.futureTimeline.Count && this.futureTimeline[num2].iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.') <= pr)
		{
			num++;
			num2++;
		}
		return num;
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x00124C2C File Offset: 0x00122E2C
	private float GetAvgFuturePriority()
	{
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.futureTimeline.Count; i++)
		{
			num += (float)this.futureTimeline[i].iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.');
		}
		return num / (float)this.futureTimeline.Count;
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x00124C90 File Offset: 0x00122E90
	private int FindBestIndex(int priority, float weight = 0f)
	{
		int num = 0;
		int num2 = 0;
		while (num2 < this.futureTimeline.Count && this.futureTimeline[num2].iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.') < priority)
		{
			num = num2 + 1;
			num2++;
		}
		int num3 = num;
		while (num3 < this.futureTimeline.Count && this.futureTimeline[num3].iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.') <= priority && this.futureTimeline[num3].iconCache.weight >= weight)
		{
			num++;
			num3++;
		}
		return num;
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x00124D44 File Offset: 0x00122F44
	public override string ToString()
	{
		string text = "Future Timeline priority_weight time:";
		for (int i = 0; i < this.futureTimeline.Count; i++)
		{
			text = string.Concat(new object[]
			{
				text,
				"\n",
				i,
				":\t",
				this.futureTimeline[i].iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.'),
				"_",
				this.futureTimeline[i].iconCache.weight,
				"\t",
				this.futureTimeline[i].time
			});
		}
		text += "\n\nPast Timeline priority_weight_time:";
		for (int j = 0; j < this.pastTimeline.Count; j++)
		{
			text = string.Concat(new object[]
			{
				text,
				"\n",
				j,
				":\t",
				this.pastTimeline[j].iconCache.def_field.GetInt("priority", null, 0, true, true, true, '.'),
				"_",
				this.pastTimeline[j].iconCache.weight,
				"\t",
				this.pastTimeline[j].time
			});
		}
		return text;
	}

	// Token: 0x040014FF RID: 5375
	private List<IconsFloodControl.TimelineEvent> futureTimeline = new List<IconsFloodControl.TimelineEvent>();

	// Token: 0x04001500 RID: 5376
	private List<IconsFloodControl.TimelineEvent> pastTimeline = new List<IconsFloodControl.TimelineEvent>();

	// Token: 0x04001501 RID: 5377
	private Time time;

	// Token: 0x02000744 RID: 1860
	public struct IconCache
	{
		// Token: 0x06004A59 RID: 19033 RVA: 0x00220763 File Offset: 0x0021E963
		public IconCache(DT.Field argInDef_field, Vars argInVars, IconsBar argInBar, MessageIcon.Type argInType, float argInWeight, bool playSound)
		{
			this.def_field = argInDef_field;
			this.weight = argInWeight;
			this.vars = argInVars;
			this.bar = argInBar;
			this.type = argInType;
			this.playSound = playSound;
		}

		// Token: 0x0400395A RID: 14682
		public DT.Field def_field;

		// Token: 0x0400395B RID: 14683
		public float weight;

		// Token: 0x0400395C RID: 14684
		public Vars vars;

		// Token: 0x0400395D RID: 14685
		public IconsBar bar;

		// Token: 0x0400395E RID: 14686
		public MessageIcon.Type type;

		// Token: 0x0400395F RID: 14687
		public bool playSound;
	}

	// Token: 0x02000745 RID: 1861
	// (Invoke) Token: 0x06004A5B RID: 19035
	public delegate MessageIcon Callback(IconsFloodControl.IconCache cache);

	// Token: 0x02000746 RID: 1862
	private class TimelineEvent
	{
		// Token: 0x06004A5E RID: 19038 RVA: 0x00220792 File Offset: 0x0021E992
		public TimelineEvent(IconsFloodControl.IconCache parameters, IconsFloodControl.Callback func)
		{
			this.iconCache = parameters;
			this.time = 0L;
			this.OnTimeComes = func;
		}

		// Token: 0x04003960 RID: 14688
		public IconsFloodControl.IconCache iconCache;

		// Token: 0x04003961 RID: 14689
		public long time;

		// Token: 0x04003962 RID: 14690
		public IconsFloodControl.Callback OnTimeComes;
	}
}
