using System;
using UnityEngine;
using UnityEngine.Events;

namespace Dreamteck.Splines
{
	// Token: 0x020004AF RID: 1199
	[AddComponentMenu("Dreamteck/Splines/Users/Length Calculator")]
	public class LengthCalculator : SplineUser
	{
		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06003EC3 RID: 16067 RVA: 0x001E0D4F File Offset: 0x001DEF4F
		public float length
		{
			get
			{
				return this._length;
			}
		}

		// Token: 0x06003EC4 RID: 16068 RVA: 0x001E0D58 File Offset: 0x001DEF58
		protected override void Awake()
		{
			base.Awake();
			this._length = base.CalculateLength(0.0, 1.0);
			this.lastLength = this._length;
			for (int i = 0; i < this.lengthEvents.Length; i++)
			{
				if (this.lengthEvents[i].targetLength == this._length)
				{
					this.lengthEvents[i].onChange.Invoke();
				}
			}
		}

		// Token: 0x06003EC5 RID: 16069 RVA: 0x001E0DD0 File Offset: 0x001DEFD0
		protected override void Build()
		{
			base.Build();
			this._length = base.CalculateLength(0.0, 1.0);
			if (this.lastLength != this._length)
			{
				for (int i = 0; i < this.lengthEvents.Length; i++)
				{
					this.lengthEvents[i].Check(this.lastLength, this._length);
				}
				this.lastLength = this._length;
			}
		}

		// Token: 0x06003EC6 RID: 16070 RVA: 0x001E0E48 File Offset: 0x001DF048
		private void AddEvent(LengthCalculator.LengthEvent lengthEvent)
		{
			LengthCalculator.LengthEvent[] array = new LengthCalculator.LengthEvent[this.lengthEvents.Length + 1];
			this.lengthEvents.CopyTo(array, 0);
			array[array.Length - 1] = lengthEvent;
			this.lengthEvents = array;
		}

		// Token: 0x04002C80 RID: 11392
		[HideInInspector]
		public LengthCalculator.LengthEvent[] lengthEvents = new LengthCalculator.LengthEvent[0];

		// Token: 0x04002C81 RID: 11393
		[HideInInspector]
		public float idealLength = 1f;

		// Token: 0x04002C82 RID: 11394
		private float _length;

		// Token: 0x04002C83 RID: 11395
		private float lastLength;

		// Token: 0x02000980 RID: 2432
		[Serializable]
		public class LengthEvent
		{
			// Token: 0x06005412 RID: 21522 RVA: 0x0024539E File Offset: 0x0024359E
			public LengthEvent()
			{
			}

			// Token: 0x06005413 RID: 21523 RVA: 0x002453BF File Offset: 0x002435BF
			public LengthEvent(LengthCalculator.LengthEvent.Type t)
			{
				this.type = t;
			}

			// Token: 0x06005414 RID: 21524 RVA: 0x002453E8 File Offset: 0x002435E8
			public void Check(float fromLength, float toLength)
			{
				if (!this.enabled)
				{
					return;
				}
				bool flag = false;
				switch (this.type)
				{
				case LengthCalculator.LengthEvent.Type.Growing:
					flag = (toLength >= this.targetLength && fromLength < this.targetLength);
					break;
				case LengthCalculator.LengthEvent.Type.Shrinking:
					flag = (toLength <= this.targetLength && fromLength > this.targetLength);
					break;
				case LengthCalculator.LengthEvent.Type.Both:
					flag = ((toLength >= this.targetLength && fromLength < this.targetLength) || (toLength <= this.targetLength && fromLength > this.targetLength));
					break;
				}
				if (flag)
				{
					this.onChange.Invoke();
				}
			}

			// Token: 0x0400442B RID: 17451
			public bool enabled = true;

			// Token: 0x0400442C RID: 17452
			public float targetLength;

			// Token: 0x0400442D RID: 17453
			public UnityEvent onChange = new UnityEvent();

			// Token: 0x0400442E RID: 17454
			public LengthCalculator.LengthEvent.Type type = LengthCalculator.LengthEvent.Type.Both;

			// Token: 0x02000A41 RID: 2625
			public enum Type
			{
				// Token: 0x04004719 RID: 18201
				Growing,
				// Token: 0x0400471A RID: 18202
				Shrinking,
				// Token: 0x0400471B RID: 18203
				Both
			}
		}
	}
}
