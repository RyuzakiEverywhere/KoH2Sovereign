using System;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B0 RID: 1200
	public class MeshGenerator : SplineUser
	{
		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06003EC8 RID: 16072 RVA: 0x001E0EA0 File Offset: 0x001DF0A0
		// (set) Token: 0x06003EC9 RID: 16073 RVA: 0x001E0EA8 File Offset: 0x001DF0A8
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
					return;
				}
				this._size = value;
			}
		}

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06003ECA RID: 16074 RVA: 0x001E0EC8 File Offset: 0x001DF0C8
		// (set) Token: 0x06003ECB RID: 16075 RVA: 0x001E0ED0 File Offset: 0x001DF0D0
		public Color color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (value != this._color)
				{
					this._color = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06003ECC RID: 16076 RVA: 0x001E0EED File Offset: 0x001DF0ED
		// (set) Token: 0x06003ECD RID: 16077 RVA: 0x001E0EF5 File Offset: 0x001DF0F5
		public Vector3 offset
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

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06003ECE RID: 16078 RVA: 0x001E0F12 File Offset: 0x001DF112
		// (set) Token: 0x06003ECF RID: 16079 RVA: 0x001E0F1A File Offset: 0x001DF11A
		public MeshGenerator.NormalMethod normalMethod
		{
			get
			{
				return this._normalMethod;
			}
			set
			{
				if (value != this._normalMethod)
				{
					this._normalMethod = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06003ED0 RID: 16080 RVA: 0x001E0F32 File Offset: 0x001DF132
		// (set) Token: 0x06003ED1 RID: 16081 RVA: 0x001E0F3A File Offset: 0x001DF13A
		public bool calculateTangents
		{
			get
			{
				return this._calculateTangents;
			}
			set
			{
				if (value != this._calculateTangents)
				{
					this._calculateTangents = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06003ED2 RID: 16082 RVA: 0x001E0F52 File Offset: 0x001DF152
		// (set) Token: 0x06003ED3 RID: 16083 RVA: 0x001E0F5A File Offset: 0x001DF15A
		public float rotation
		{
			get
			{
				return this._rotation;
			}
			set
			{
				if (value != this._rotation)
				{
					this._rotation = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06003ED4 RID: 16084 RVA: 0x001E0F72 File Offset: 0x001DF172
		// (set) Token: 0x06003ED5 RID: 16085 RVA: 0x001E0F7A File Offset: 0x001DF17A
		public bool flipFaces
		{
			get
			{
				return this._flipFaces;
			}
			set
			{
				if (value != this._flipFaces)
				{
					this._flipFaces = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06003ED6 RID: 16086 RVA: 0x001E0F92 File Offset: 0x001DF192
		// (set) Token: 0x06003ED7 RID: 16087 RVA: 0x001E0F9A File Offset: 0x001DF19A
		public bool doubleSided
		{
			get
			{
				return this._doubleSided;
			}
			set
			{
				if (value != this._doubleSided)
				{
					this._doubleSided = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06003ED8 RID: 16088 RVA: 0x001E0FB2 File Offset: 0x001DF1B2
		// (set) Token: 0x06003ED9 RID: 16089 RVA: 0x001E0FBA File Offset: 0x001DF1BA
		public MeshGenerator.UVMode uvMode
		{
			get
			{
				return this._uvMode;
			}
			set
			{
				if (value != this._uvMode)
				{
					this._uvMode = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06003EDA RID: 16090 RVA: 0x001E0FD2 File Offset: 0x001DF1D2
		// (set) Token: 0x06003EDB RID: 16091 RVA: 0x001E0FDA File Offset: 0x001DF1DA
		public Vector2 uvScale
		{
			get
			{
				return this._uvScale;
			}
			set
			{
				if (value != this._uvScale)
				{
					this._uvScale = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06003EDC RID: 16092 RVA: 0x001E0FF7 File Offset: 0x001DF1F7
		// (set) Token: 0x06003EDD RID: 16093 RVA: 0x001E0FFF File Offset: 0x001DF1FF
		public Vector2 uvOffset
		{
			get
			{
				return this._uvOffset;
			}
			set
			{
				if (value != this._uvOffset)
				{
					this._uvOffset = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06003EDE RID: 16094 RVA: 0x001E101C File Offset: 0x001DF21C
		// (set) Token: 0x06003EDF RID: 16095 RVA: 0x001E1024 File Offset: 0x001DF224
		public float uvRotation
		{
			get
			{
				return this._uvRotation;
			}
			set
			{
				if (value != this._uvRotation)
				{
					this._uvRotation = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06003EE0 RID: 16096 RVA: 0x001E103C File Offset: 0x001DF23C
		public bool baked
		{
			get
			{
				return this._baked;
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06003EE1 RID: 16097 RVA: 0x001E1044 File Offset: 0x001DF244
		// (set) Token: 0x06003EE2 RID: 16098 RVA: 0x001E104C File Offset: 0x001DF24C
		public bool markDynamic
		{
			get
			{
				return this._markDynamic;
			}
			set
			{
				if (value != this._markDynamic)
				{
					this._markDynamic = value;
					if (!this._markDynamic)
					{
						Object.Destroy(this.mesh);
						this.mesh = new Mesh();
					}
					this.RebuildImmediate();
				}
			}
		}

		// Token: 0x06003EE3 RID: 16099 RVA: 0x001E1084 File Offset: 0x001DF284
		protected override void Awake()
		{
			if (this.mesh == null)
			{
				this.mesh = new Mesh();
			}
			base.Awake();
			this.filter = base.GetComponent<MeshFilter>();
			this.meshRenderer = base.GetComponent<MeshRenderer>();
			this.meshCollider = base.GetComponent<MeshCollider>();
		}

		// Token: 0x06003EE4 RID: 16100 RVA: 0x001E0B2E File Offset: 0x001DED2E
		protected override void Reset()
		{
			base.Reset();
		}

		// Token: 0x06003EE5 RID: 16101 RVA: 0x001E10D4 File Offset: 0x001DF2D4
		public void CloneMesh()
		{
			if (this.tsMesh != null)
			{
				this.tsMesh = TS_Mesh.Copy(this.tsMesh);
			}
			else
			{
				this.tsMesh = new TS_Mesh();
			}
			if (this.mesh != null)
			{
				this.mesh = Object.Instantiate<Mesh>(this.mesh);
				return;
			}
			this.mesh = new Mesh();
		}

		// Token: 0x06003EE6 RID: 16102 RVA: 0x001E1132 File Offset: 0x001DF332
		public override void Rebuild()
		{
			if (this._baked)
			{
				return;
			}
			base.Rebuild();
		}

		// Token: 0x06003EE7 RID: 16103 RVA: 0x001E1143 File Offset: 0x001DF343
		public override void RebuildImmediate()
		{
			if (this._baked)
			{
				return;
			}
			base.RebuildImmediate();
		}

		// Token: 0x06003EE8 RID: 16104 RVA: 0x001E0B36 File Offset: 0x001DED36
		protected override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x06003EE9 RID: 16105 RVA: 0x001E0B3E File Offset: 0x001DED3E
		protected override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x06003EEA RID: 16106 RVA: 0x001E1154 File Offset: 0x001DF354
		protected override void OnDestroy()
		{
			base.OnDestroy();
			MeshFilter component = base.GetComponent<MeshFilter>();
			MeshRenderer component2 = base.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.hideFlags = HideFlags.None;
			}
			if (component2 != null)
			{
				component2.hideFlags = HideFlags.None;
			}
		}

		// Token: 0x06003EEB RID: 16107 RVA: 0x001E1198 File Offset: 0x001DF398
		public void UpdateCollider()
		{
			this.meshCollider = base.GetComponent<MeshCollider>();
			if (this.meshCollider == null)
			{
				this.meshCollider = base.gameObject.AddComponent<MeshCollider>();
			}
			this.meshCollider.sharedMesh = this.filter.sharedMesh;
		}

		// Token: 0x06003EEC RID: 16108 RVA: 0x001E11E8 File Offset: 0x001DF3E8
		protected override void LateRun()
		{
			if (this._baked)
			{
				return;
			}
			base.LateRun();
			if (this.updateCollider && this.meshCollider != null && Time.time - this.lastUpdateTime >= this.colliderUpdateRate)
			{
				this.lastUpdateTime = Time.time;
				this.updateCollider = false;
				this.meshCollider.sharedMesh = this.filter.sharedMesh;
			}
		}

		// Token: 0x06003EED RID: 16109 RVA: 0x001E1256 File Offset: 0x001DF456
		protected override void Build()
		{
			base.Build();
			if (base.sampleCount > 0)
			{
				this.BuildMesh();
			}
		}

		// Token: 0x06003EEE RID: 16110 RVA: 0x001E126D File Offset: 0x001DF46D
		protected override void PostBuild()
		{
			base.PostBuild();
			this.WriteMesh();
		}

		// Token: 0x06003EEF RID: 16111 RVA: 0x000023FD File Offset: 0x000005FD
		protected virtual void BuildMesh()
		{
		}

		// Token: 0x06003EF0 RID: 16112 RVA: 0x001E127C File Offset: 0x001DF47C
		protected virtual void WriteMesh()
		{
			MeshUtility.InverseTransformMesh(this.tsMesh, this.trs);
			if (this._doubleSided)
			{
				MeshUtility.MakeDoublesidedHalf(this.tsMesh);
			}
			else if (this._flipFaces)
			{
				MeshUtility.FlipFaces(this.tsMesh);
			}
			if (this._calculateTangents)
			{
				MeshUtility.CalculateTangents(this.tsMesh);
			}
			this.mesh.MarkDynamic();
			this.tsMesh.WriteMesh(ref this.mesh);
			if (this._normalMethod == MeshGenerator.NormalMethod.Recalculate)
			{
				this.mesh.RecalculateNormals();
			}
			if (this.filter != null)
			{
				this.filter.sharedMesh = this.mesh;
			}
			this.updateCollider = true;
		}

		// Token: 0x06003EF1 RID: 16113 RVA: 0x001E132C File Offset: 0x001DF52C
		protected virtual void AllocateMesh(int vertexCount, int trisCount)
		{
			if (this._doubleSided)
			{
				vertexCount *= 2;
				trisCount *= 2;
			}
			if (this.tsMesh.vertexCount != vertexCount)
			{
				this.tsMesh.vertices = new Vector3[vertexCount];
				this.tsMesh.normals = new Vector3[vertexCount];
				this.tsMesh.tangents = new Vector4[vertexCount];
				this.tsMesh.colors = new Color[vertexCount];
				this.tsMesh.uv = new Vector2[vertexCount];
			}
			if (this.tsMesh.triangles.Length != trisCount)
			{
				this.tsMesh.triangles = new int[trisCount];
			}
		}

		// Token: 0x06003EF2 RID: 16114 RVA: 0x001E13CF File Offset: 0x001DF5CF
		protected void ResetUVDistance()
		{
			this.vDist = 0f;
			if (this.uvMode == MeshGenerator.UVMode.UniformClip)
			{
				this.vDist = base.spline.CalculateLength(0.0, base.GetSampleRaw(0).percent);
			}
		}

		// Token: 0x06003EF3 RID: 16115 RVA: 0x001E140B File Offset: 0x001DF60B
		protected void AddUVDistance(int sampleIndex)
		{
			if (sampleIndex == 0)
			{
				return;
			}
			this.vDist += Vector3.Distance(base.GetSampleRaw(sampleIndex).position, base.GetSampleRaw(sampleIndex - 1).position);
		}

		// Token: 0x06003EF4 RID: 16116 RVA: 0x001E1440 File Offset: 0x001DF640
		protected void CalculateUVs(double percent, float u)
		{
			MeshGenerator.uvs.x = u * this._uvScale.x - this._uvOffset.x;
			switch (this.uvMode)
			{
			case MeshGenerator.UVMode.Clip:
				MeshGenerator.uvs.y = (float)percent * this._uvScale.y - this._uvOffset.y;
				return;
			case MeshGenerator.UVMode.Clamp:
				MeshGenerator.uvs.y = (float)DMath.InverseLerp(base.clipFrom, base.clipTo, percent) * this._uvScale.y - this._uvOffset.y;
				return;
			case MeshGenerator.UVMode.UniformClamp:
				MeshGenerator.uvs.y = this.vDist * this._uvScale.y / (float)base.span - this._uvOffset.y;
				return;
			}
			MeshGenerator.uvs.y = this.vDist * this._uvScale.y - this._uvOffset.y;
		}

		// Token: 0x04002C84 RID: 11396
		[SerializeField]
		[HideInInspector]
		private bool _baked;

		// Token: 0x04002C85 RID: 11397
		[SerializeField]
		[HideInInspector]
		private bool _markDynamic = true;

		// Token: 0x04002C86 RID: 11398
		[SerializeField]
		[HideInInspector]
		private float _size = 1f;

		// Token: 0x04002C87 RID: 11399
		[SerializeField]
		[HideInInspector]
		private Color _color = Color.white;

		// Token: 0x04002C88 RID: 11400
		[SerializeField]
		[HideInInspector]
		private Vector3 _offset = Vector3.zero;

		// Token: 0x04002C89 RID: 11401
		[SerializeField]
		[HideInInspector]
		private MeshGenerator.NormalMethod _normalMethod = MeshGenerator.NormalMethod.SplineNormals;

		// Token: 0x04002C8A RID: 11402
		[SerializeField]
		[HideInInspector]
		private bool _calculateTangents = true;

		// Token: 0x04002C8B RID: 11403
		[SerializeField]
		[HideInInspector]
		[Range(-360f, 360f)]
		private float _rotation;

		// Token: 0x04002C8C RID: 11404
		[SerializeField]
		[HideInInspector]
		private bool _flipFaces;

		// Token: 0x04002C8D RID: 11405
		[SerializeField]
		[HideInInspector]
		private bool _doubleSided;

		// Token: 0x04002C8E RID: 11406
		[SerializeField]
		[HideInInspector]
		private MeshGenerator.UVMode _uvMode;

		// Token: 0x04002C8F RID: 11407
		[SerializeField]
		[HideInInspector]
		private Vector2 _uvScale = Vector2.one;

		// Token: 0x04002C90 RID: 11408
		[SerializeField]
		[HideInInspector]
		private Vector2 _uvOffset = Vector2.zero;

		// Token: 0x04002C91 RID: 11409
		[SerializeField]
		[HideInInspector]
		private float _uvRotation;

		// Token: 0x04002C92 RID: 11410
		[SerializeField]
		[HideInInspector]
		protected MeshCollider meshCollider;

		// Token: 0x04002C93 RID: 11411
		[SerializeField]
		[HideInInspector]
		protected MeshFilter filter;

		// Token: 0x04002C94 RID: 11412
		[SerializeField]
		[HideInInspector]
		protected MeshRenderer meshRenderer;

		// Token: 0x04002C95 RID: 11413
		[SerializeField]
		[HideInInspector]
		protected TS_Mesh tsMesh = new TS_Mesh();

		// Token: 0x04002C96 RID: 11414
		[SerializeField]
		[HideInInspector]
		protected Mesh mesh;

		// Token: 0x04002C97 RID: 11415
		[HideInInspector]
		public float colliderUpdateRate = 0.2f;

		// Token: 0x04002C98 RID: 11416
		protected bool updateCollider;

		// Token: 0x04002C99 RID: 11417
		protected float lastUpdateTime;

		// Token: 0x04002C9A RID: 11418
		private float vDist;

		// Token: 0x04002C9B RID: 11419
		protected static Vector2 uvs = Vector2.zero;

		// Token: 0x02000981 RID: 2433
		public enum UVMode
		{
			// Token: 0x04004430 RID: 17456
			Clip,
			// Token: 0x04004431 RID: 17457
			UniformClip,
			// Token: 0x04004432 RID: 17458
			Clamp,
			// Token: 0x04004433 RID: 17459
			UniformClamp
		}

		// Token: 0x02000982 RID: 2434
		public enum NormalMethod
		{
			// Token: 0x04004435 RID: 17461
			Recalculate,
			// Token: 0x04004436 RID: 17462
			SplineNormals
		}
	}
}
