using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004CB RID: 1227
	[Serializable]
	public class ObjectSequence<T>
	{
		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06004129 RID: 16681 RVA: 0x001EF5B8 File Offset: 0x001ED7B8
		// (set) Token: 0x0600412A RID: 16682 RVA: 0x001EF5C0 File Offset: 0x001ED7C0
		public int randomSeed
		{
			get
			{
				return this._randomSeed;
			}
			set
			{
				if (value != this._randomSeed)
				{
					this._randomSeed = value;
					this.randomizer = new Random(this._randomSeed);
				}
			}
		}

		// Token: 0x0600412B RID: 16683 RVA: 0x001EF5E3 File Offset: 0x001ED7E3
		public ObjectSequence()
		{
			this.randomizer = new Random(this._randomSeed);
		}

		// Token: 0x0600412C RID: 16684 RVA: 0x001EF603 File Offset: 0x001ED803
		public T GetFirst()
		{
			if (this.startObject != null)
			{
				return this.startObject;
			}
			return this.Next();
		}

		// Token: 0x0600412D RID: 16685 RVA: 0x001EF61F File Offset: 0x001ED81F
		public T GetLast()
		{
			if (this.endObject != null)
			{
				return this.endObject;
			}
			return this.Next();
		}

		// Token: 0x0600412E RID: 16686 RVA: 0x001EF63C File Offset: 0x001ED83C
		public T Next()
		{
			if (this.iteration == ObjectSequence<T>.Iteration.Ordered)
			{
				if (this.index >= this.objects.Length)
				{
					this.index = 0;
				}
				T[] array = this.objects;
				int num = this.index;
				this.index = num + 1;
				return array[num];
			}
			int num2 = this.randomizer.Next(this.objects.Length - 1);
			return this.objects[num2];
		}

		// Token: 0x04002D80 RID: 11648
		public T startObject;

		// Token: 0x04002D81 RID: 11649
		public T endObject;

		// Token: 0x04002D82 RID: 11650
		public T[] objects;

		// Token: 0x04002D83 RID: 11651
		public ObjectSequence<T>.Iteration iteration;

		// Token: 0x04002D84 RID: 11652
		[SerializeField]
		[HideInInspector]
		private int _randomSeed = 1;

		// Token: 0x04002D85 RID: 11653
		[SerializeField]
		[HideInInspector]
		private int index;

		// Token: 0x04002D86 RID: 11654
		[SerializeField]
		[HideInInspector]
		private Random randomizer;

		// Token: 0x020009AE RID: 2478
		public enum Iteration
		{
			// Token: 0x04004518 RID: 17688
			Ordered,
			// Token: 0x04004519 RID: 17689
			Random
		}
	}
}
