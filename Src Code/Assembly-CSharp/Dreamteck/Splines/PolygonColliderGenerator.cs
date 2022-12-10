using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B6 RID: 1206
	[AddComponentMenu("Dreamteck/Splines/Users/Polygon Collider Generator")]
	[RequireComponent(typeof(PolygonCollider2D))]
	public class PolygonColliderGenerator : SplineUser
	{
		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06003F78 RID: 16248 RVA: 0x001E578E File Offset: 0x001E398E
		// (set) Token: 0x06003F79 RID: 16249 RVA: 0x001E5796 File Offset: 0x001E3996
		public PolygonColliderGenerator.Type type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06003F7A RID: 16250 RVA: 0x001E57AE File Offset: 0x001E39AE
		// (set) Token: 0x06003F7B RID: 16251 RVA: 0x001E57B6 File Offset: 0x001E39B6
		public float size
		{
			get
			{
				return this._size;
			}
			set
			{
				if (value != this._size)
				{
					this._size = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06003F7C RID: 16252 RVA: 0x001E57CE File Offset: 0x001E39CE
		// (set) Token: 0x06003F7D RID: 16253 RVA: 0x001E57D6 File Offset: 0x001E39D6
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

		// Token: 0x06003F7E RID: 16254 RVA: 0x001E57EE File Offset: 0x001E39EE
		protected override void Awake()
		{
			base.Awake();
			this.polygonCollider = base.GetComponent<PolygonCollider2D>();
		}

		// Token: 0x06003F7F RID: 16255 RVA: 0x001E0B2E File Offset: 0x001DED2E
		protected override void Reset()
		{
			base.Reset();
		}

		// Token: 0x06003F80 RID: 16256 RVA: 0x001E0B36 File Offset: 0x001DED36
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x06003F81 RID: 16257 RVA: 0x001E0B3E File Offset: 0x001DED3E
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x06003F82 RID: 16258 RVA: 0x001E0B46 File Offset: 0x001DED46
		protected override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x06003F83 RID: 16259 RVA: 0x001E5804 File Offset: 0x001E3A04
		protected override void LateRun()
		{
			base.LateRun();
			if (this.updateCollider && this.polygonCollider != null && Time.time - this.lastUpdateTime >= this.updateRate)
			{
				this.lastUpdateTime = Time.time;
				this.updateCollider = false;
				this.polygonCollider.SetPath(0, this.vertices);
			}
		}

		// Token: 0x06003F84 RID: 16260 RVA: 0x001E5868 File Offset: 0x001E3A68
		protected override void Build()
		{
			base.Build();
			if (base.sampleCount == 0)
			{
				return;
			}
			PolygonColliderGenerator.Type type = this.type;
			if (type == PolygonColliderGenerator.Type.Path)
			{
				this.GeneratePath();
				return;
			}
			if (type != PolygonColliderGenerator.Type.Shape)
			{
				return;
			}
			this.GenerateShape();
		}

		// Token: 0x06003F85 RID: 16261 RVA: 0x001E58A0 File Offset: 0x001E3AA0
		protected override void PostBuild()
		{
			base.PostBuild();
			if (this.polygonCollider == null)
			{
				return;
			}
			for (int i = 0; i < this.vertices.Length; i++)
			{
				this.vertices[i] = base.transform.InverseTransformPoint(this.vertices[i]);
			}
			if (this.updateRate == 0f)
			{
				this.polygonCollider.SetPath(0, this.vertices);
				return;
			}
			this.updateCollider = true;
		}

		// Token: 0x06003F86 RID: 16262 RVA: 0x001E592C File Offset: 0x001E3B2C
		private void GeneratePath()
		{
			int num = base.sampleCount * 2;
			if (this.vertices.Length != num)
			{
				this.vertices = new Vector2[num];
			}
			for (int i = 0; i < base.sampleCount; i++)
			{
				base.GetSample(i, this.evalResult);
				Vector2 a = new Vector2(-this.evalResult.forward.y, this.evalResult.forward.x).normalized * this.evalResult.size;
				this.vertices[i] = new Vector2(this.evalResult.position.x, this.evalResult.position.y) + a * this.size * 0.5f + a * this.offset;
				this.vertices[base.sampleCount + (base.sampleCount - 1) - i] = new Vector2(this.evalResult.position.x, this.evalResult.position.y) - a * this.size * 0.5f + a * this.offset;
			}
		}

		// Token: 0x06003F87 RID: 16263 RVA: 0x001E5A88 File Offset: 0x001E3C88
		private void GenerateShape()
		{
			if (this.vertices.Length != base.sampleCount)
			{
				this.vertices = new Vector2[base.sampleCount];
			}
			for (int i = 0; i < base.sampleCount; i++)
			{
				base.GetSample(i, this.evalResult);
				this.vertices[i] = this.evalResult.position;
				if (this.offset != 0f)
				{
					Vector2 a = new Vector2(-this.evalResult.forward.y, this.evalResult.forward.x).normalized * this.evalResult.size;
					this.vertices[i] += a * this.offset;
				}
			}
		}

		// Token: 0x04002CDC RID: 11484
		[SerializeField]
		[HideInInspector]
		private PolygonColliderGenerator.Type _type;

		// Token: 0x04002CDD RID: 11485
		[SerializeField]
		[HideInInspector]
		private float _size = 1f;

		// Token: 0x04002CDE RID: 11486
		[SerializeField]
		[HideInInspector]
		private float _offset;

		// Token: 0x04002CDF RID: 11487
		[SerializeField]
		[HideInInspector]
		protected PolygonCollider2D polygonCollider;

		// Token: 0x04002CE0 RID: 11488
		[SerializeField]
		[HideInInspector]
		protected Vector2[] vertices = new Vector2[0];

		// Token: 0x04002CE1 RID: 11489
		[HideInInspector]
		public float updateRate = 0.1f;

		// Token: 0x04002CE2 RID: 11490
		protected float lastUpdateTime;

		// Token: 0x04002CE3 RID: 11491
		private bool updateCollider;

		// Token: 0x02000992 RID: 2450
		public enum Type
		{
			// Token: 0x04004497 RID: 17559
			Path,
			// Token: 0x04004498 RID: 17560
			Shape
		}
	}
}
