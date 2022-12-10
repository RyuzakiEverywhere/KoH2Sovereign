using System;
using System.Collections;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
	// Token: 0x020003F7 RID: 1015
	[Serializable]
	public class Stat
	{
		// Token: 0x17000351 RID: 849
		// (get) Token: 0x0600381A RID: 14362 RVA: 0x001BAF5B File Offset: 0x001B915B
		// (set) Token: 0x0600381B RID: 14363 RVA: 0x001BAF63 File Offset: 0x001B9163
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
				if (value)
				{
					this.StartRegeneration();
					return;
				}
				this.StopRegeneration();
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x0600381C RID: 14364 RVA: 0x001BAF7C File Offset: 0x001B917C
		// (set) Token: 0x0600381D RID: 14365 RVA: 0x001BAF8C File Offset: 0x001B918C
		public float Value
		{
			get
			{
				return this.value.Value;
			}
			set
			{
				if (!this.Active)
				{
					return;
				}
				if (value < 0f)
				{
					value = 0f;
				}
				if (this.value.Value != value)
				{
					this.value.Value = value;
					if (value == 0f)
					{
						this.OnStatEmpty.Invoke();
					}
					this.OnValueChangeNormalized.Invoke(value / this.MaxValue);
					this.OnValueChange.Invoke(value);
					if (value > this.Above && !this.isAbove)
					{
						this.OnStatAbove.Invoke();
						this.isAbove = true;
						this.isBelow = false;
						return;
					}
					if (value < this.Below && !this.isBelow)
					{
						this.OnStatBelow.Invoke();
						this.isBelow = true;
						this.isAbove = false;
					}
				}
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x0600381E RID: 14366 RVA: 0x001BB055 File Offset: 0x001B9255
		// (set) Token: 0x0600381F RID: 14367 RVA: 0x001BB062 File Offset: 0x001B9262
		public float MaxValue
		{
			get
			{
				return this.maxValue;
			}
			set
			{
				this.maxValue.Value = value;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06003820 RID: 14368 RVA: 0x001BB070 File Offset: 0x001B9270
		// (set) Token: 0x06003821 RID: 14369 RVA: 0x001BB07D File Offset: 0x001B927D
		public float MinValue
		{
			get
			{
				return this.minValue;
			}
			set
			{
				this.minValue.Value = value;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x06003822 RID: 14370 RVA: 0x001BB08B File Offset: 0x001B928B
		// (set) Token: 0x06003823 RID: 14371 RVA: 0x001BB093 File Offset: 0x001B9293
		public bool Regenerate
		{
			get
			{
				return this.regenerate;
			}
			set
			{
				this.regenerate = value;
				this.Regenerate_OldValue = this.regenerate;
				this.StartRegeneration();
			}
		}

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x06003824 RID: 14372 RVA: 0x001BB0AE File Offset: 0x001B92AE
		// (set) Token: 0x06003825 RID: 14373 RVA: 0x001BB0B8 File Offset: 0x001B92B8
		public bool Degenerate
		{
			get
			{
				return this.degenerate;
			}
			set
			{
				if (this.degenerate != value)
				{
					this.degenerate = value;
					this.OnDegenereate.Invoke(value);
					if (this.degenerate)
					{
						this.regenerate = false;
						this.StartDegeneration();
						this.StopRegeneration();
						return;
					}
					this.regenerate = this.Regenerate_OldValue;
					this.StopDegeneration();
					this.StartRegeneration();
				}
			}
		}

		// Token: 0x06003826 RID: 14374 RVA: 0x001BB118 File Offset: 0x001B9318
		internal void InitializeStat(MonoBehaviour holder)
		{
			this.isAbove = (this.isBelow = false);
			this.Coroutine = holder;
			if (this.value.Value > this.Above)
			{
				this.isAbove = true;
			}
			else if (this.value.Value < this.Below)
			{
				this.isBelow = true;
			}
			this.Regenerate_OldValue = this.Regenerate;
			if (this.MaxValue < this.Value)
			{
				this.MaxValue = this.Value;
			}
			this.Regeneration = null;
			this.Degeneration = null;
			this.ModifyPerTicks = null;
			this.StartRegeneration();
		}

		// Token: 0x06003827 RID: 14375 RVA: 0x001BB1B3 File Offset: 0x001B93B3
		public virtual void Modify(float newValue)
		{
			if (!this.Active)
			{
				return;
			}
			this.Value += newValue;
			this.StartRegeneration();
		}

		// Token: 0x06003828 RID: 14376 RVA: 0x001BB1D2 File Offset: 0x001B93D2
		public virtual void Modify(float newValue, float time)
		{
			if (!this.Active)
			{
				return;
			}
			this.StopSlowModification();
			this.ModifySlow = this.C_SmoothChangeValue(newValue, time);
			this.Coroutine.StartCoroutine(this.ModifySlow);
		}

		// Token: 0x06003829 RID: 14377 RVA: 0x001BB204 File Offset: 0x001B9404
		public virtual void Modify(float newValue, int ticks, float timeBetweenTicks)
		{
			if (!this.Active)
			{
				return;
			}
			if (this.ModifyPerTicks != null)
			{
				this.Coroutine.StopCoroutine(this.ModifyPerTicks);
			}
			this.ModifyPerTicks = this.C_ModifyTicksValue(newValue, ticks, timeBetweenTicks);
			this.Coroutine.StartCoroutine(this.ModifyPerTicks);
		}

		// Token: 0x0600382A RID: 14378 RVA: 0x001BB254 File Offset: 0x001B9454
		public virtual void ModifyMAX(float newValue)
		{
			if (!this.Active)
			{
				return;
			}
			this.MaxValue += newValue;
			this.StartRegeneration();
		}

		// Token: 0x0600382B RID: 14379 RVA: 0x001BB273 File Offset: 0x001B9473
		public virtual void ModifyRegenerationRate(float newValue)
		{
			if (!this.Active)
			{
				return;
			}
			this.RegenRate.Value += newValue;
			this.StartRegeneration();
		}

		// Token: 0x0600382C RID: 14380 RVA: 0x001BB297 File Offset: 0x001B9497
		public virtual void ModifyRegenerationWait(float newValue)
		{
			if (!this.Active)
			{
				return;
			}
			this.RegenWaitTime.Value += newValue;
			if (this.RegenWaitTime < 0f)
			{
				this.RegenWaitTime.Value = 0f;
			}
		}

		// Token: 0x0600382D RID: 14381 RVA: 0x001BB2D7 File Offset: 0x001B94D7
		public virtual void SetRegenerationRate(float newValue)
		{
			if (!this.Active)
			{
				return;
			}
			this.RegenRate.Value = newValue;
		}

		// Token: 0x0600382E RID: 14382 RVA: 0x001BB2EE File Offset: 0x001B94EE
		public virtual void Reset()
		{
			this.Value = this.MaxValue;
		}

		// Token: 0x0600382F RID: 14383 RVA: 0x001BB2FC File Offset: 0x001B94FC
		public virtual void Clean()
		{
			this.StopDegeneration();
			this.StopRegeneration();
			this.StopTickDamage();
			this.StopSlowModification();
		}

		// Token: 0x06003830 RID: 14384 RVA: 0x001BB318 File Offset: 0x001B9518
		protected virtual void StartRegeneration()
		{
			this.StopRegeneration();
			if (this.RegenRate == 0f || !this.Regenerate)
			{
				return;
			}
			this.Regeneration = this.C_Regenerate();
			this.Coroutine.StartCoroutine(this.Regeneration);
		}

		// Token: 0x06003831 RID: 14385 RVA: 0x001BB364 File Offset: 0x001B9564
		protected virtual void StartDegeneration()
		{
			if (this.DegenRate == 0f)
			{
				return;
			}
			this.StopDegeneration();
			this.Degeneration = this.C_Degenerate();
			this.Coroutine.StartCoroutine(this.Degeneration);
		}

		// Token: 0x06003832 RID: 14386 RVA: 0x001BB39D File Offset: 0x001B959D
		protected virtual void StopRegeneration()
		{
			if (this.Regeneration != null)
			{
				this.Coroutine.StopCoroutine(this.Regeneration);
			}
			this.Regeneration = null;
		}

		// Token: 0x06003833 RID: 14387 RVA: 0x001BB3BF File Offset: 0x001B95BF
		protected virtual void StopDegeneration()
		{
			if (this.Degeneration != null)
			{
				this.Coroutine.StopCoroutine(this.Degeneration);
			}
			this.Degeneration = null;
		}

		// Token: 0x06003834 RID: 14388 RVA: 0x001BB3E1 File Offset: 0x001B95E1
		protected virtual void StopTickDamage()
		{
			if (this.ModifyPerTicks != null)
			{
				this.Coroutine.StopCoroutine(this.ModifyPerTicks);
			}
			this.ModifyPerTicks = null;
		}

		// Token: 0x06003835 RID: 14389 RVA: 0x001BB403 File Offset: 0x001B9603
		protected virtual void StopSlowModification()
		{
			if (this.ModifySlow != null)
			{
				this.Coroutine.StopCoroutine(this.ModifySlow);
			}
			this.ModifySlow = null;
		}

		// Token: 0x06003836 RID: 14390 RVA: 0x001BB425 File Offset: 0x001B9625
		protected virtual IEnumerator C_Regenerate()
		{
			if (this.RegenWaitTime > 0f)
			{
				yield return new WaitForSeconds(this.RegenWaitTime);
			}
			float ReachValue = (this.RegenRate > 0f) ? this.MaxValue : 0f;
			bool Positive = this.RegenRate > 0f;
			while (this.Value != ReachValue)
			{
				this.Value += this.RegenRate * Time.deltaTime;
				if (Positive && this.Value > this.MaxValue)
				{
					this.Reset();
					this.OnStatFull.Invoke();
				}
				else if (!Positive && this.Value < 0f)
				{
					this.Value = this.MinValue;
					this.OnStatEmpty.Invoke();
				}
				yield return null;
			}
			yield return null;
			yield break;
		}

		// Token: 0x06003837 RID: 14391 RVA: 0x001BB434 File Offset: 0x001B9634
		protected virtual IEnumerator C_Degenerate()
		{
			while (this.Degenerate || this.Value <= this.MinValue)
			{
				this.Value -= this.DegenRate * Time.deltaTime;
				yield return null;
			}
			yield return null;
			yield break;
		}

		// Token: 0x06003838 RID: 14392 RVA: 0x001BB443 File Offset: 0x001B9643
		protected virtual IEnumerator C_ModifyTicksValue(float value, int Ticks, float time)
		{
			WaitForSeconds WaitForTicks = new WaitForSeconds(time);
			int num;
			for (int i = 0; i < Ticks; i = num + 1)
			{
				this.Value += value;
				if (this.Value <= this.MinValue)
				{
					this.Value = this.MinValue;
					break;
				}
				yield return WaitForTicks;
				num = i;
			}
			yield return null;
			this.StartRegeneration();
			yield break;
		}

		// Token: 0x06003839 RID: 14393 RVA: 0x001BB467 File Offset: 0x001B9667
		protected virtual IEnumerator C_SmoothChangeValue(float newvalue, float smoothChangeValueTime)
		{
			this.StopRegeneration();
			Debug.Log(newvalue);
			float currentTime = 0f;
			float currentValue = this.Value;
			newvalue = this.Value + newvalue;
			while (currentTime <= smoothChangeValueTime)
			{
				this.Value = Mathf.Lerp(currentValue, newvalue, currentTime / smoothChangeValueTime);
				currentTime += Time.deltaTime;
				yield return null;
			}
			this.Value = newvalue;
			yield return null;
			this.StartRegeneration();
			yield break;
		}

		// Token: 0x04002834 RID: 10292
		public string name;

		// Token: 0x04002835 RID: 10293
		[SerializeField]
		private bool active = true;

		// Token: 0x04002836 RID: 10294
		public IntReference ID;

		// Token: 0x04002837 RID: 10295
		[SerializeField]
		private FloatReference value;

		// Token: 0x04002838 RID: 10296
		[SerializeField]
		private FloatReference maxValue;

		// Token: 0x04002839 RID: 10297
		[SerializeField]
		private FloatReference minValue;

		// Token: 0x0400283A RID: 10298
		[SerializeField]
		private bool regenerate;

		// Token: 0x0400283B RID: 10299
		public FloatReference RegenRate;

		// Token: 0x0400283C RID: 10300
		public FloatReference RegenWaitTime;

		// Token: 0x0400283D RID: 10301
		[SerializeField]
		private bool degenerate;

		// Token: 0x0400283E RID: 10302
		public FloatReference DegenRate;

		// Token: 0x0400283F RID: 10303
		private bool isBelow;

		// Token: 0x04002840 RID: 10304
		private bool isAbove;

		// Token: 0x04002841 RID: 10305
		public bool ShowEvents;

		// Token: 0x04002842 RID: 10306
		public UnityEvent OnStatFull = new UnityEvent();

		// Token: 0x04002843 RID: 10307
		public UnityEvent OnStatEmpty = new UnityEvent();

		// Token: 0x04002844 RID: 10308
		[SerializeField]
		public float Below;

		// Token: 0x04002845 RID: 10309
		[SerializeField]
		public float Above;

		// Token: 0x04002846 RID: 10310
		public UnityEvent OnStatBelow = new UnityEvent();

		// Token: 0x04002847 RID: 10311
		public UnityEvent OnStatAbove = new UnityEvent();

		// Token: 0x04002848 RID: 10312
		public FloatEvent OnValueChangeNormalized = new FloatEvent();

		// Token: 0x04002849 RID: 10313
		public FloatEvent OnValueChange = new FloatEvent();

		// Token: 0x0400284A RID: 10314
		public BoolEvent OnDegenereate = new BoolEvent();

		// Token: 0x0400284B RID: 10315
		private bool Regenerate_OldValue;

		// Token: 0x0400284C RID: 10316
		internal MonoBehaviour Coroutine;

		// Token: 0x0400284D RID: 10317
		public IEnumerator Regeneration;

		// Token: 0x0400284E RID: 10318
		public IEnumerator Degeneration;

		// Token: 0x0400284F RID: 10319
		public IEnumerator ModifyPerTicks;

		// Token: 0x04002850 RID: 10320
		public IEnumerator ModifySlow;
	}
}
