using System;
using System.Collections.Generic;
using MalbersAnimations.Scriptables;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003F6 RID: 1014
	public class Stats : MonoBehaviour
	{
		// Token: 0x06003805 RID: 14341 RVA: 0x001BAC18 File Offset: 0x001B8E18
		private void Start()
		{
			base.StopAllCoroutines();
			foreach (Stat stat in this.stats)
			{
				stat.InitializeStat(this);
			}
		}

		// Token: 0x06003806 RID: 14342 RVA: 0x001BAC70 File Offset: 0x001B8E70
		private void OnDisable()
		{
			base.StopAllCoroutines();
			foreach (Stat stat in this.stats)
			{
				stat.Clean();
			}
		}

		// Token: 0x06003807 RID: 14343 RVA: 0x001BACC8 File Offset: 0x001B8EC8
		public virtual void _PinStat(string name)
		{
			this.PinnedStat = this.GetStat(name);
		}

		// Token: 0x06003808 RID: 14344 RVA: 0x001BACD7 File Offset: 0x001B8ED7
		public virtual void _PinStat(int ID)
		{
			this.PinnedStat = this.GetStat(ID);
		}

		// Token: 0x06003809 RID: 14345 RVA: 0x001BACE6 File Offset: 0x001B8EE6
		public virtual void _PinStat(IntVar ID)
		{
			this.PinnedStat = this.GetStat(ID);
		}

		// Token: 0x0600380A RID: 14346 RVA: 0x001BACF8 File Offset: 0x001B8EF8
		public virtual Stat GetStat(string name)
		{
			this.PinnedStat = this.stats.Find((Stat item) => item.name == name);
			return this.PinnedStat;
		}

		// Token: 0x0600380B RID: 14347 RVA: 0x001BAD38 File Offset: 0x001B8F38
		public virtual Stat GetStat(int ID)
		{
			this.PinnedStat = this.stats.Find((Stat item) => item.ID == ID);
			return this.PinnedStat;
		}

		// Token: 0x0600380C RID: 14348 RVA: 0x001BAD78 File Offset: 0x001B8F78
		public virtual Stat GetStat(IntVar ID)
		{
			this.PinnedStat = this.stats.Find((Stat item) => item.ID == ID);
			return this.PinnedStat;
		}

		// Token: 0x0600380D RID: 14349 RVA: 0x001BADB5 File Offset: 0x001B8FB5
		public virtual void _PinStatModifyValue(float value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Modify(value);
				return;
			}
			Debug.Log("There's no Pinned Stat");
		}

		// Token: 0x0600380E RID: 14350 RVA: 0x001BADD6 File Offset: 0x001B8FD6
		public virtual void _PinStatModifyValue(float value, float time)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Modify(value, time);
				return;
			}
			Debug.Log("There's no Pinned Stat");
		}

		// Token: 0x0600380F RID: 14351 RVA: 0x001BADF8 File Offset: 0x001B8FF8
		public virtual void _PinStatModifyValue1Sec(float value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Modify(value, 1f);
				return;
			}
			Debug.Log("There's no Pinned Stat");
		}

		// Token: 0x06003810 RID: 14352 RVA: 0x001BAE1E File Offset: 0x001B901E
		public virtual void _PinStatSetValue(float value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Value = value;
				return;
			}
			Debug.Log("There's no Pinned Stat");
		}

		// Token: 0x06003811 RID: 14353 RVA: 0x001BAE3F File Offset: 0x001B903F
		public virtual void _PinStatModifyMaxValue(float value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.ModifyMAX(value);
				return;
			}
			Debug.Log("There's no Pinned Stat");
		}

		// Token: 0x06003812 RID: 14354 RVA: 0x001BAE60 File Offset: 0x001B9060
		public virtual void _PinStatSetMaxValue(float value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.MaxValue = value;
				return;
			}
			Debug.Log("There's no Pinned Stat");
		}

		// Token: 0x06003813 RID: 14355 RVA: 0x001BAE81 File Offset: 0x001B9081
		public virtual void _PinStatModifyRegenerationRate(float value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.ModifyRegenerationRate(value);
				return;
			}
			Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
		}

		// Token: 0x06003814 RID: 14356 RVA: 0x001BAEA2 File Offset: 0x001B90A2
		public virtual void _PinStatDegenerate(bool value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Degenerate = value;
				return;
			}
			Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
		}

		// Token: 0x06003815 RID: 14357 RVA: 0x001BAEC3 File Offset: 0x001B90C3
		public virtual void _PinStatRegenerate(bool value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Regenerate = value;
				return;
			}
			Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
		}

		// Token: 0x06003816 RID: 14358 RVA: 0x001BAEE4 File Offset: 0x001B90E4
		public virtual void _PinStatEnable(bool value)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Active = value;
				return;
			}
			Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
		}

		// Token: 0x06003817 RID: 14359 RVA: 0x001BAF05 File Offset: 0x001B9105
		public virtual void _PinStatModifyValue(float newValue, int ticks, float timeBetweenTicks)
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Modify(newValue, ticks, timeBetweenTicks);
				return;
			}
			Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
		}

		// Token: 0x06003818 RID: 14360 RVA: 0x001BAF28 File Offset: 0x001B9128
		public virtual void _PinStatCLEAN()
		{
			if (this.PinnedStat != null)
			{
				this.PinnedStat.Clean();
				return;
			}
			Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
		}

		// Token: 0x04002832 RID: 10290
		public List<Stat> stats = new List<Stat>();

		// Token: 0x04002833 RID: 10291
		[SerializeField]
		private Stat PinnedStat;
	}
}
