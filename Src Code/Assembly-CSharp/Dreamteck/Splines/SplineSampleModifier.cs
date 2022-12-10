using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004BC RID: 1212
	[Serializable]
	public class SplineSampleModifier
	{
		// Token: 0x06003FA4 RID: 16292 RVA: 0x001E6362 File Offset: 0x001E4562
		public virtual List<SplineSampleModifier.Key> GetKeys()
		{
			return new List<SplineSampleModifier.Key>();
		}

		// Token: 0x06003FA5 RID: 16293 RVA: 0x001E636C File Offset: 0x001E456C
		public virtual void SetKeys(List<SplineSampleModifier.Key> input)
		{
			for (int i = 0; i < input.Count; i++)
			{
				input[i].modifier = this;
			}
		}

		// Token: 0x06003FA6 RID: 16294 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void Apply(SplineSample result)
		{
		}

		// Token: 0x06003FA7 RID: 16295 RVA: 0x001E6397 File Offset: 0x001E4597
		public virtual void Apply(SplineSample source, SplineSample destination)
		{
			destination.CopyFrom(source);
			this.Apply(destination);
		}

		// Token: 0x04002CE9 RID: 11497
		public float blend = 1f;

		// Token: 0x02000998 RID: 2456
		[Serializable]
		public class Key
		{
			// Token: 0x170006FA RID: 1786
			// (get) Token: 0x06005441 RID: 21569 RVA: 0x00246116 File Offset: 0x00244316
			// (set) Token: 0x06005442 RID: 21570 RVA: 0x0024611E File Offset: 0x0024431E
			public double start
			{
				get
				{
					return this._featherStart;
				}
				set
				{
					if (value != this._featherStart)
					{
						this._featherStart = DMath.Clamp01(value);
					}
				}
			}

			// Token: 0x170006FB RID: 1787
			// (get) Token: 0x06005443 RID: 21571 RVA: 0x00246135 File Offset: 0x00244335
			// (set) Token: 0x06005444 RID: 21572 RVA: 0x0024613D File Offset: 0x0024433D
			public double end
			{
				get
				{
					return this._featherEnd;
				}
				set
				{
					if (value != this._featherEnd)
					{
						this._featherEnd = DMath.Clamp01(value);
					}
				}
			}

			// Token: 0x170006FC RID: 1788
			// (get) Token: 0x06005445 RID: 21573 RVA: 0x00246154 File Offset: 0x00244354
			// (set) Token: 0x06005446 RID: 21574 RVA: 0x0024615C File Offset: 0x0024435C
			public double centerStart
			{
				get
				{
					return this._centerStart;
				}
				set
				{
					if (value != this._centerStart)
					{
						this._centerStart = DMath.Clamp01(value);
						if (this._centerStart > this._centerEnd)
						{
							this._centerStart = this._centerEnd;
						}
					}
				}
			}

			// Token: 0x170006FD RID: 1789
			// (get) Token: 0x06005447 RID: 21575 RVA: 0x0024618D File Offset: 0x0024438D
			// (set) Token: 0x06005448 RID: 21576 RVA: 0x00246195 File Offset: 0x00244395
			public double centerEnd
			{
				get
				{
					return this._centerEnd;
				}
				set
				{
					if (value != this._centerEnd)
					{
						this._centerEnd = DMath.Clamp01(value);
						if (this._centerEnd < this._centerStart)
						{
							this._centerEnd = this._centerStart;
						}
					}
				}
			}

			// Token: 0x170006FE RID: 1790
			// (get) Token: 0x06005449 RID: 21577 RVA: 0x002461C6 File Offset: 0x002443C6
			// (set) Token: 0x0600544A RID: 21578 RVA: 0x002461D4 File Offset: 0x002443D4
			public double globalCenterStart
			{
				get
				{
					return this.LocalToGlobalPercent(this.centerStart);
				}
				set
				{
					this.centerStart = DMath.Clamp01(this.GlobalToLocalPercent(value));
				}
			}

			// Token: 0x170006FF RID: 1791
			// (get) Token: 0x0600544B RID: 21579 RVA: 0x002461E8 File Offset: 0x002443E8
			// (set) Token: 0x0600544C RID: 21580 RVA: 0x002461F6 File Offset: 0x002443F6
			public double globalCenterEnd
			{
				get
				{
					return this.LocalToGlobalPercent(this.centerEnd);
				}
				set
				{
					this.centerEnd = DMath.Clamp01(this.GlobalToLocalPercent(value));
				}
			}

			// Token: 0x17000700 RID: 1792
			// (get) Token: 0x0600544D RID: 21581 RVA: 0x0024620C File Offset: 0x0024440C
			// (set) Token: 0x0600544E RID: 21582 RVA: 0x002462A8 File Offset: 0x002444A8
			public double position
			{
				get
				{
					double num = DMath.Lerp(this._centerStart, this._centerEnd, 0.5);
					if (this.start > this.end)
					{
						double num2 = DMath.Lerp(this._featherStart, this._featherEnd, num);
						double num3 = 1.0 - this._featherStart;
						double num4 = num * (num3 + this._featherEnd);
						num2 = this._featherStart + num4;
						if (num2 > 1.0)
						{
							num2 -= 1.0;
						}
						return num2;
					}
					return DMath.Lerp(this._featherStart, this._featherEnd, num);
				}
				set
				{
					double num = value - this.position;
					this.start += num;
					this.end += num;
				}
			}

			// Token: 0x0600544F RID: 21583 RVA: 0x002462DC File Offset: 0x002444DC
			internal Key(double f, double t, SplineSampleModifier modifier)
			{
				this.modifier = modifier;
				this.start = f;
				this.end = t;
				this.interpolation = AnimationCurve.Linear(0f, 0f, 1f, 1f);
			}

			// Token: 0x06005450 RID: 21584 RVA: 0x0024634C File Offset: 0x0024454C
			private double GlobalToLocalPercent(double t)
			{
				if (this._featherStart <= this._featherEnd)
				{
					return DMath.InverseLerp(this._featherStart, this._featherEnd, t);
				}
				if (t > this._featherStart)
				{
					return DMath.InverseLerp(this._featherStart, this._featherStart + (1.0 - this._featherStart) + this._featherEnd, t);
				}
				if (t < this._featherEnd)
				{
					return DMath.InverseLerp(-(1.0 - this._featherStart), this._featherEnd, t);
				}
				return 0.0;
			}

			// Token: 0x06005451 RID: 21585 RVA: 0x002463E0 File Offset: 0x002445E0
			private double LocalToGlobalPercent(double t)
			{
				if (this._featherStart > this._featherEnd)
				{
					t = DMath.Lerp(this._featherStart, this._featherStart + (1.0 - this._featherStart) + this._featherEnd, t);
					if (t > 1.0)
					{
						t -= 1.0;
					}
					return t;
				}
				return DMath.Lerp(this._featherStart, this._featherEnd, t);
			}

			// Token: 0x06005452 RID: 21586 RVA: 0x00246454 File Offset: 0x00244654
			public float Evaluate(double t)
			{
				t = (double)((float)this.GlobalToLocalPercent(t));
				if (t < this._centerStart)
				{
					return this.interpolation.Evaluate((float)t / (float)this._centerStart) * this.blend;
				}
				if (t > this._centerEnd)
				{
					return this.interpolation.Evaluate(1f - (float)DMath.InverseLerp(this._centerEnd, 1.0, t)) * this.blend;
				}
				return this.interpolation.Evaluate(1f) * this.blend;
			}

			// Token: 0x06005453 RID: 21587 RVA: 0x002464E4 File Offset: 0x002446E4
			public virtual SplineSampleModifier.Key Duplicate()
			{
				return new SplineSampleModifier.Key(this.start, this.end, this.modifier)
				{
					_centerStart = this._centerStart,
					_centerEnd = this._centerEnd,
					blend = this.blend,
					interpolation = DuplicateUtility.DuplicateCurve(this.interpolation)
				};
			}

			// Token: 0x040044A1 RID: 17569
			[SerializeField]
			private double _featherStart;

			// Token: 0x040044A2 RID: 17570
			[SerializeField]
			private double _featherEnd;

			// Token: 0x040044A3 RID: 17571
			[SerializeField]
			private double _centerStart = 0.25;

			// Token: 0x040044A4 RID: 17572
			[SerializeField]
			private double _centerEnd = 0.75;

			// Token: 0x040044A5 RID: 17573
			[SerializeField]
			internal SplineSampleModifier modifier;

			// Token: 0x040044A6 RID: 17574
			public AnimationCurve interpolation;

			// Token: 0x040044A7 RID: 17575
			public float blend = 1f;
		}
	}
}
