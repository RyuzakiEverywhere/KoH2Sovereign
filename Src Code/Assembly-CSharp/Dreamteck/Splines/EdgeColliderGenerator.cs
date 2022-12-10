using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004AE RID: 1198
	[AddComponentMenu("Dreamteck/Splines/Users/Edge Collider Generator")]
	[RequireComponent(typeof(EdgeCollider2D))]
	public class EdgeColliderGenerator : SplineUser
	{
		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06003EB8 RID: 16056 RVA: 0x001E0AFA File Offset: 0x001DECFA
		// (set) Token: 0x06003EB9 RID: 16057 RVA: 0x001E0B02 File Offset: 0x001DED02
		public float offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				if (value != this._offset)
				{
					this._offset = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x06003EBA RID: 16058 RVA: 0x001E0B1A File Offset: 0x001DED1A
		protected override void Awake()
		{
			base.Awake();
			this.edgeCollider = base.GetComponent<EdgeCollider2D>();
		}

		// Token: 0x06003EBB RID: 16059 RVA: 0x001E0B2E File Offset: 0x001DED2E
		protected override void Reset()
		{
			base.Reset();
		}

		// Token: 0x06003EBC RID: 16060 RVA: 0x001E0B36 File Offset: 0x001DED36
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x06003EBD RID: 16061 RVA: 0x001E0B3E File Offset: 0x001DED3E
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x06003EBE RID: 16062 RVA: 0x001E0B46 File Offset: 0x001DED46
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x06003EBF RID: 16063 RVA: 0x001E0B50 File Offset: 0x001DED50
		protected override void LateRun()
		{
			base.LateRun();
			if (this.updateCollider && this.edgeCollider != null && Time.time - this.lastUpdateTime >= this.updateRate)
			{
				this.lastUpdateTime = Time.time;
				this.updateCollider = false;
				this.edgeCollider.points = this.vertices;
			}
		}

		// Token: 0x06003EC0 RID: 16064 RVA: 0x001E0BB0 File Offset: 0x001DEDB0
		protected override void Build()
		{
			base.Build();
			if (base.sampleCount == 0)
			{
				return;
			}
			if (this.vertices.Length != base.sampleCount)
			{
				this.vertices = new Vector2[base.sampleCount];
			}
			bool flag = this.offset != 0f;
			for (int i = 0; i < base.sampleCount; i++)
			{
				base.GetSample(i, this.evalResult);
				this.vertices[i] = this.evalResult.position;
				if (flag)
				{
					Vector2 a = new Vector2(-this.evalResult.forward.y, this.evalResult.forward.x).normalized * this.evalResult.size;
					this.vertices[i] += a * this.offset;
				}
			}
		}

		// Token: 0x06003EC1 RID: 16065 RVA: 0x001E0CA8 File Offset: 0x001DEEA8
		protected override void PostBuild()
		{
			base.PostBuild();
			if (this.edgeCollider == null)
			{
				return;
			}
			for (int i = 0; i < this.vertices.Length; i++)
			{
				this.vertices[i] = base.transform.InverseTransformPoint(this.vertices[i]);
			}
			if (this.updateRate == 0f)
			{
				this.edgeCollider.points = this.vertices;
				return;
			}
			this.updateCollider = true;
		}

		// Token: 0x04002C7A RID: 11386
		[SerializeField]
		[HideInInspector]
		private float _offset;

		// Token: 0x04002C7B RID: 11387
		[SerializeField]
		[HideInInspector]
		protected EdgeCollider2D edgeCollider;

		// Token: 0x04002C7C RID: 11388
		[SerializeField]
		[HideInInspector]
		protected Vector2[] vertices = new Vector2[0];

		// Token: 0x04002C7D RID: 11389
		[HideInInspector]
		public float updateRate = 0.1f;

		// Token: 0x04002C7E RID: 11390
		protected float lastUpdateTime;

		// Token: 0x04002C7F RID: 11391
		private bool updateCollider;
	}
}
