using System;
using System.Collections;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004B3 RID: 1203
	[AddComponentMenu("Dreamteck/Splines/Users/Object Controller")]
	public class ObjectController : SplineUser
	{
		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06003F31 RID: 16177 RVA: 0x001E36CA File Offset: 0x001E18CA
		// (set) Token: 0x06003F32 RID: 16178 RVA: 0x001E36D2 File Offset: 0x001E18D2
		public ObjectController.ObjectMethod objectMethod
		{
			get
			{
				return this._objectMethod;
			}
			set
			{
				if (value != this._objectMethod)
				{
					if (value == ObjectController.ObjectMethod.GetChildren)
					{
						this._objectMethod = value;
						this.Spawn();
						return;
					}
					this._objectMethod = value;
				}
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06003F33 RID: 16179 RVA: 0x001E36F6 File Offset: 0x001E18F6
		// (set) Token: 0x06003F34 RID: 16180 RVA: 0x001E3700 File Offset: 0x001E1900
		public int spawnCount
		{
			get
			{
				return this._spawnCount;
			}
			set
			{
				if (value != this._spawnCount)
				{
					if (value < 0)
					{
						value = 0;
					}
					if (this._objectMethod == ObjectController.ObjectMethod.Instantiate)
					{
						if (value < this._spawnCount)
						{
							this._spawnCount = value;
							this.Remove();
							return;
						}
						this._spawnCount = value;
						this.Spawn();
						return;
					}
					else
					{
						this._spawnCount = value;
					}
				}
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06003F35 RID: 16181 RVA: 0x001E3751 File Offset: 0x001E1951
		// (set) Token: 0x06003F36 RID: 16182 RVA: 0x001E3759 File Offset: 0x001E1959
		public ObjectController.Positioning objectPositioning
		{
			get
			{
				return this._objectPositioning;
			}
			set
			{
				if (value != this._objectPositioning)
				{
					this._objectPositioning = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06003F37 RID: 16183 RVA: 0x001E3771 File Offset: 0x001E1971
		// (set) Token: 0x06003F38 RID: 16184 RVA: 0x001E3779 File Offset: 0x001E1979
		public ObjectController.Iteration iteration
		{
			get
			{
				return this._iteration;
			}
			set
			{
				if (value != this._iteration)
				{
					this._iteration = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06003F39 RID: 16185 RVA: 0x001E3791 File Offset: 0x001E1991
		// (set) Token: 0x06003F3A RID: 16186 RVA: 0x001E3799 File Offset: 0x001E1999
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
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06003F3B RID: 16187 RVA: 0x001E37B1 File Offset: 0x001E19B1
		// (set) Token: 0x06003F3C RID: 16188 RVA: 0x001E37B9 File Offset: 0x001E19B9
		public Vector3 minOffset
		{
			get
			{
				return this._minOffset;
			}
			set
			{
				if (value != this._minOffset)
				{
					this._minOffset = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x06003F3D RID: 16189 RVA: 0x001E37D6 File Offset: 0x001E19D6
		// (set) Token: 0x06003F3E RID: 16190 RVA: 0x001E37DE File Offset: 0x001E19DE
		public Vector3 maxOffset
		{
			get
			{
				return this._maxOffset;
			}
			set
			{
				if (value != this._maxOffset)
				{
					this._maxOffset = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x06003F3F RID: 16191 RVA: 0x001E37FB File Offset: 0x001E19FB
		// (set) Token: 0x06003F40 RID: 16192 RVA: 0x001E3803 File Offset: 0x001E1A03
		public bool offsetUseWorldCoords
		{
			get
			{
				return this._offsetUseWorldCoords;
			}
			set
			{
				if (value != this._offsetUseWorldCoords)
				{
					this._offsetUseWorldCoords = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x06003F41 RID: 16193 RVA: 0x001E381B File Offset: 0x001E1A1B
		// (set) Token: 0x06003F42 RID: 16194 RVA: 0x001E3823 File Offset: 0x001E1A23
		public Vector3 minRotation
		{
			get
			{
				return this._minRotation;
			}
			set
			{
				if (value != this._minRotation)
				{
					this._minRotation = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x06003F43 RID: 16195 RVA: 0x001E3840 File Offset: 0x001E1A40
		// (set) Token: 0x06003F44 RID: 16196 RVA: 0x001E3848 File Offset: 0x001E1A48
		public Vector3 maxRotation
		{
			get
			{
				return this._maxRotation;
			}
			set
			{
				if (value != this._maxRotation)
				{
					this._maxRotation = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06003F45 RID: 16197 RVA: 0x001E3865 File Offset: 0x001E1A65
		// (set) Token: 0x06003F46 RID: 16198 RVA: 0x001E3884 File Offset: 0x001E1A84
		public Vector3 rotationOffset
		{
			get
			{
				return (this._maxRotation + this._minRotation) / 2f;
			}
			set
			{
				if (value != this._minRotation || value != this._maxRotation)
				{
					this._maxRotation = value;
					this._minRotation = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06003F47 RID: 16199 RVA: 0x001E38C3 File Offset: 0x001E1AC3
		// (set) Token: 0x06003F48 RID: 16200 RVA: 0x001E38CB File Offset: 0x001E1ACB
		public Vector3 minScaleMultiplier
		{
			get
			{
				return this._minScaleMultiplier;
			}
			set
			{
				if (value != this._minScaleMultiplier)
				{
					this._minScaleMultiplier = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06003F49 RID: 16201 RVA: 0x001E38E8 File Offset: 0x001E1AE8
		// (set) Token: 0x06003F4A RID: 16202 RVA: 0x001E38F0 File Offset: 0x001E1AF0
		public Vector3 maxScaleMultiplier
		{
			get
			{
				return this._maxScaleMultiplier;
			}
			set
			{
				if (value != this._maxScaleMultiplier)
				{
					this._maxScaleMultiplier = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06003F4B RID: 16203 RVA: 0x001E390D File Offset: 0x001E1B0D
		// (set) Token: 0x06003F4C RID: 16204 RVA: 0x001E3915 File Offset: 0x001E1B15
		public bool uniformScaleLerp
		{
			get
			{
				return this._uniformScaleLerp;
			}
			set
			{
				if (value != this._uniformScaleLerp)
				{
					this._uniformScaleLerp = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06003F4D RID: 16205 RVA: 0x001E392D File Offset: 0x001E1B2D
		// (set) Token: 0x06003F4E RID: 16206 RVA: 0x001E3935 File Offset: 0x001E1B35
		public bool shellOffset
		{
			get
			{
				return this._shellOffset;
			}
			set
			{
				if (value != this._shellOffset)
				{
					this._shellOffset = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06003F4F RID: 16207 RVA: 0x001E394D File Offset: 0x001E1B4D
		// (set) Token: 0x06003F50 RID: 16208 RVA: 0x001E3955 File Offset: 0x001E1B55
		public bool applyRotation
		{
			get
			{
				return this._applyRotation;
			}
			set
			{
				if (value != this._applyRotation)
				{
					this._applyRotation = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06003F51 RID: 16209 RVA: 0x001E396D File Offset: 0x001E1B6D
		// (set) Token: 0x06003F52 RID: 16210 RVA: 0x001E3975 File Offset: 0x001E1B75
		public bool rotateByOffset
		{
			get
			{
				return this._rotateByOffset;
			}
			set
			{
				if (value != this._rotateByOffset)
				{
					this._rotateByOffset = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06003F53 RID: 16211 RVA: 0x001E398D File Offset: 0x001E1B8D
		// (set) Token: 0x06003F54 RID: 16212 RVA: 0x001E3995 File Offset: 0x001E1B95
		public bool applyScale
		{
			get
			{
				return this._applyScale;
			}
			set
			{
				if (value != this._applyScale)
				{
					this._applyScale = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06003F55 RID: 16213 RVA: 0x001E39AD File Offset: 0x001E1BAD
		// (set) Token: 0x06003F56 RID: 16214 RVA: 0x001E39B5 File Offset: 0x001E1BB5
		public float evaluateOffset
		{
			get
			{
				return this._evaluateOffset;
			}
			set
			{
				if (value != this._evaluateOffset)
				{
					this._evaluateOffset = value;
					this.Rebuild();
				}
			}
		}

		// Token: 0x06003F57 RID: 16215 RVA: 0x001E39D0 File Offset: 0x001E1BD0
		public void Clear()
		{
			for (int i = 0; i < this.spawned.Length; i++)
			{
				if (this.spawned[i] != null && !(this.spawned[i].transform == null))
				{
					this.spawned[i].transform.localScale = this.spawned[i].baseScale;
					if (this._objectMethod == ObjectController.ObjectMethod.GetChildren)
					{
						this.spawned[i].gameObject.SetActive(false);
					}
					else
					{
						this.spawned[i].Destroy();
					}
				}
			}
			this.spawned = new ObjectController.ObjectControl[0];
		}

		// Token: 0x06003F58 RID: 16216 RVA: 0x001E3A65 File Offset: 0x001E1C65
		private void OnValidate()
		{
			if (this._spawnCount < 0)
			{
				this._spawnCount = 0;
			}
		}

		// Token: 0x06003F59 RID: 16217 RVA: 0x001E3A78 File Offset: 0x001E1C78
		private void Remove()
		{
			if (this._spawnCount >= this.spawned.Length)
			{
				return;
			}
			int num = this.spawned.Length - 1;
			while (num >= this._spawnCount && num < this.spawned.Length)
			{
				if (this.spawned[num] != null)
				{
					this.spawned[num].transform.localScale = this.spawned[num].baseScale;
					if (this._objectMethod == ObjectController.ObjectMethod.GetChildren)
					{
						this.spawned[num].gameObject.SetActive(false);
					}
					else if (Application.isEditor)
					{
						this.spawned[num].DestroyImmediate();
					}
					else
					{
						this.spawned[num].Destroy();
					}
				}
				num--;
			}
			ObjectController.ObjectControl[] array = new ObjectController.ObjectControl[this._spawnCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.spawned[i];
			}
			this.spawned = array;
			this.Rebuild();
		}

		// Token: 0x06003F5A RID: 16218 RVA: 0x001E3B5C File Offset: 0x001E1D5C
		public void GetAll()
		{
			ObjectController.ObjectControl[] array = new ObjectController.ObjectControl[base.transform.childCount];
			int num = 0;
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (array[num] == null)
				{
					array[num++] = new ObjectController.ObjectControl(transform.gameObject);
				}
				else
				{
					bool flag = false;
					for (int i = 0; i < this.spawned.Length; i++)
					{
						if (this.spawned[i].gameObject == transform.gameObject)
						{
							array[num++] = this.spawned[i];
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						array[num++] = new ObjectController.ObjectControl(transform.gameObject);
					}
				}
			}
			this.spawned = array;
		}

		// Token: 0x06003F5B RID: 16219 RVA: 0x001E3C4C File Offset: 0x001E1E4C
		public void Spawn()
		{
			if (this._objectMethod == ObjectController.ObjectMethod.Instantiate)
			{
				if (this.delayedSpawn && Application.isPlaying)
				{
					base.StopCoroutine("InstantiateAllWithDelay");
					base.StartCoroutine(this.InstantiateAllWithDelay());
				}
				else
				{
					this.InstantiateAll();
				}
			}
			else
			{
				this.GetAll();
			}
			this.Rebuild();
		}

		// Token: 0x06003F5C RID: 16220 RVA: 0x001E3C9E File Offset: 0x001E1E9E
		protected override void LateRun()
		{
			base.LateRun();
			if (this._objectMethod == ObjectController.ObjectMethod.GetChildren && this.lastChildCount != base.transform.childCount)
			{
				this.Spawn();
				this.lastChildCount = base.transform.childCount;
			}
		}

		// Token: 0x06003F5D RID: 16221 RVA: 0x001E3CD9 File Offset: 0x001E1ED9
		private IEnumerator InstantiateAllWithDelay()
		{
			if (base.spline == null)
			{
				yield break;
			}
			if (this.objects.Length == 0)
			{
				yield break;
			}
			int num;
			for (int i = this.spawned.Length; i <= this.spawnCount; i = num + 1)
			{
				this.InstantiateSingle();
				yield return new WaitForSeconds(this.spawnDelay);
				num = i;
			}
			yield break;
		}

		// Token: 0x06003F5E RID: 16222 RVA: 0x001E3CE8 File Offset: 0x001E1EE8
		private void InstantiateAll()
		{
			if (base.spline == null)
			{
				return;
			}
			if (this.objects.Length == 0)
			{
				return;
			}
			for (int i = this.spawned.Length; i < this.spawnCount; i++)
			{
				this.InstantiateSingle();
			}
		}

		// Token: 0x06003F5F RID: 16223 RVA: 0x001E3D2C File Offset: 0x001E1F2C
		private void InstantiateSingle()
		{
			if (this.objects.Length == 0)
			{
				return;
			}
			int num;
			if (this._iteration == ObjectController.Iteration.Ordered)
			{
				num = this.spawned.Length - Mathf.FloorToInt((float)(this.spawned.Length / this.objects.Length)) * this.objects.Length;
			}
			else
			{
				num = Random.Range(0, this.objects.Length);
			}
			if (this.objects[num] == null)
			{
				return;
			}
			ObjectController.ObjectControl[] array = new ObjectController.ObjectControl[this.spawned.Length + 1];
			this.spawned.CopyTo(array, 0);
			array[array.Length - 1] = new ObjectController.ObjectControl(Object.Instantiate<GameObject>(this.objects[num], base.transform.position, base.transform.rotation));
			array[array.Length - 1].transform.parent = base.transform;
			this.spawned = array;
		}

		// Token: 0x06003F60 RID: 16224 RVA: 0x001E3E04 File Offset: 0x001E2004
		protected override void Build()
		{
			base.Build();
			this.offsetRandomizer = new Random(this._randomSeed);
			if (this._shellOffset)
			{
				this.shellRandomizer = new Random(this._randomSeed + 1);
			}
			this.rotationRandomizer = new Random(this._randomSeed + 2);
			this.scaleRandomizer = new Random(this._randomSeed + 3);
			bool flag = this._minScaleMultiplier != this._maxScaleMultiplier;
			for (int i = 0; i < this.spawned.Length; i++)
			{
				if (this.spawned[i] == null)
				{
					this.Clear();
					this.Spawn();
					return;
				}
				float num = 0f;
				if (this.spawned.Length > 1)
				{
					if (base.spline.isClosed)
					{
						num = (float)i / (float)this.spawned.Length;
					}
					else
					{
						num = (float)i / (float)(this.spawned.Length - 1);
					}
				}
				num += this._evaluateOffset;
				if (num > 1f)
				{
					num -= 1f;
				}
				else if (num < 0f)
				{
					num += 1f;
				}
				if (this.objectPositioning == ObjectController.Positioning.Clip)
				{
					base.spline.Evaluate((double)num, this.evalResult);
				}
				else
				{
					base.Evaluate((double)num, this.evalResult);
				}
				base.ModifySample(this.evalResult);
				this.spawned[i].position = this.evalResult.position;
				if (this._applyScale)
				{
					Vector3 scale = this.spawned[i].baseScale * this.evalResult.size;
					Vector3 vector = this._minScaleMultiplier;
					if (flag)
					{
						if (this._uniformScaleLerp)
						{
							vector = Vector3.Lerp(new Vector3(this._minScaleMultiplier.x, this._minScaleMultiplier.y, this._minScaleMultiplier.x), new Vector3(this._maxScaleMultiplier.x, this._maxScaleMultiplier.y, this._maxScaleMultiplier.z), (float)this.scaleRandomizer.NextDouble());
						}
						else
						{
							vector.x = Mathf.Lerp(this._minScaleMultiplier.x, this._maxScaleMultiplier.x, (float)this.scaleRandomizer.NextDouble());
							vector.y = Mathf.Lerp(this._minScaleMultiplier.y, this._maxScaleMultiplier.y, (float)this.scaleRandomizer.NextDouble());
							vector.z = Mathf.Lerp(this._minScaleMultiplier.z, this._maxScaleMultiplier.z, (float)this.scaleRandomizer.NextDouble());
						}
					}
					scale.x *= vector.x;
					scale.y *= vector.y;
					scale.z *= vector.z;
					this.spawned[i].scale = scale;
				}
				else
				{
					this.spawned[i].scale = this.spawned[i].baseScale;
				}
				Vector3 normalized = Vector3.Cross(this.evalResult.forward, this.evalResult.up).normalized;
				Vector3 vector2 = this._minOffset;
				if (this._minOffset != this._maxOffset)
				{
					if (this._shellOffset)
					{
						float num2 = this._maxOffset.x - this._minOffset.x;
						float num3 = this._maxOffset.y - this._minOffset.y;
						float f = (float)this.shellRandomizer.NextDouble() * 360f * 0.017453292f;
						vector2 = new Vector2(0.5f * Mathf.Cos(f), 0.5f * Mathf.Sin(f));
						vector2.x *= num2;
						vector2.y *= num3;
					}
					else
					{
						float t = (float)this.offsetRandomizer.NextDouble();
						vector2.x = Mathf.Lerp(this._minOffset.x, this._maxOffset.x, t);
						t = (float)this.offsetRandomizer.NextDouble();
						vector2.y = Mathf.Lerp(this._minOffset.y, this._maxOffset.y, t);
						t = (float)this.offsetRandomizer.NextDouble();
						vector2.z = Mathf.Lerp(this._minOffset.z, this._maxOffset.z, t);
					}
				}
				if (this._offsetUseWorldCoords)
				{
					this.spawned[i].position += vector2;
				}
				else
				{
					this.spawned[i].position += normalized * vector2.x * this.evalResult.size + this.evalResult.up * vector2.y * this.evalResult.size;
				}
				if (this._applyRotation)
				{
					Quaternion rhs = Quaternion.Euler(Mathf.Lerp(this._minRotation.x, this._maxRotation.x, (float)this.rotationRandomizer.NextDouble()), Mathf.Lerp(this._minRotation.y, this._maxRotation.y, (float)this.rotationRandomizer.NextDouble()), Mathf.Lerp(this._minRotation.z, this._maxRotation.z, (float)this.rotationRandomizer.NextDouble()));
					if (this._rotateByOffset)
					{
						this.spawned[i].rotation = Quaternion.LookRotation(this.evalResult.forward, this.spawned[i].position - this.evalResult.position) * rhs;
					}
					else
					{
						this.spawned[i].rotation = this.evalResult.rotation * rhs;
					}
				}
				if (this._objectPositioning == ObjectController.Positioning.Clip)
				{
					if ((double)num < base.clipFrom || (double)num > base.clipTo)
					{
						this.spawned[i].active = false;
					}
					else
					{
						this.spawned[i].active = true;
					}
				}
			}
		}

		// Token: 0x06003F61 RID: 16225 RVA: 0x001E4418 File Offset: 0x001E2618
		protected override void PostBuild()
		{
			base.PostBuild();
			for (int i = 0; i < this.spawned.Length; i++)
			{
				this.spawned[i].Apply();
			}
		}

		// Token: 0x04002CAF RID: 11439
		[SerializeField]
		[HideInInspector]
		public GameObject[] objects = new GameObject[0];

		// Token: 0x04002CB0 RID: 11440
		[SerializeField]
		[HideInInspector]
		private float _evaluateOffset;

		// Token: 0x04002CB1 RID: 11441
		[SerializeField]
		[HideInInspector]
		private int _spawnCount;

		// Token: 0x04002CB2 RID: 11442
		[SerializeField]
		[HideInInspector]
		private ObjectController.Positioning _objectPositioning;

		// Token: 0x04002CB3 RID: 11443
		[SerializeField]
		[HideInInspector]
		private ObjectController.Iteration _iteration;

		// Token: 0x04002CB4 RID: 11444
		[SerializeField]
		[HideInInspector]
		private int _randomSeed = 1;

		// Token: 0x04002CB5 RID: 11445
		[SerializeField]
		[HideInInspector]
		private Vector3 _minOffset = Vector3.zero;

		// Token: 0x04002CB6 RID: 11446
		[SerializeField]
		[HideInInspector]
		private Vector3 _maxOffset = Vector3.zero;

		// Token: 0x04002CB7 RID: 11447
		[SerializeField]
		[HideInInspector]
		private bool _offsetUseWorldCoords;

		// Token: 0x04002CB8 RID: 11448
		[SerializeField]
		[HideInInspector]
		private Vector3 _minRotation = Vector3.zero;

		// Token: 0x04002CB9 RID: 11449
		[SerializeField]
		[HideInInspector]
		private Vector3 _maxRotation = Vector3.zero;

		// Token: 0x04002CBA RID: 11450
		[SerializeField]
		[HideInInspector]
		private bool _uniformScaleLerp = true;

		// Token: 0x04002CBB RID: 11451
		[SerializeField]
		[HideInInspector]
		private Vector3 _minScaleMultiplier = Vector3.one;

		// Token: 0x04002CBC RID: 11452
		[SerializeField]
		[HideInInspector]
		private Vector3 _maxScaleMultiplier = Vector3.one;

		// Token: 0x04002CBD RID: 11453
		[SerializeField]
		[HideInInspector]
		private bool _shellOffset;

		// Token: 0x04002CBE RID: 11454
		[SerializeField]
		[HideInInspector]
		private bool _applyRotation = true;

		// Token: 0x04002CBF RID: 11455
		[SerializeField]
		[HideInInspector]
		private bool _rotateByOffset;

		// Token: 0x04002CC0 RID: 11456
		[SerializeField]
		[HideInInspector]
		private bool _applyScale;

		// Token: 0x04002CC1 RID: 11457
		[SerializeField]
		[HideInInspector]
		private ObjectController.ObjectMethod _objectMethod;

		// Token: 0x04002CC2 RID: 11458
		[HideInInspector]
		public bool delayedSpawn;

		// Token: 0x04002CC3 RID: 11459
		[HideInInspector]
		public float spawnDelay = 0.1f;

		// Token: 0x04002CC4 RID: 11460
		[SerializeField]
		[HideInInspector]
		private int lastChildCount;

		// Token: 0x04002CC5 RID: 11461
		[SerializeField]
		[HideInInspector]
		private ObjectController.ObjectControl[] spawned = new ObjectController.ObjectControl[0];

		// Token: 0x04002CC6 RID: 11462
		private Random offsetRandomizer;

		// Token: 0x04002CC7 RID: 11463
		private Random shellRandomizer;

		// Token: 0x04002CC8 RID: 11464
		private Random rotationRandomizer;

		// Token: 0x04002CC9 RID: 11465
		private Random scaleRandomizer;

		// Token: 0x02000989 RID: 2441
		[Serializable]
		internal class ObjectControl
		{
			// Token: 0x170006F6 RID: 1782
			// (get) Token: 0x0600542D RID: 21549 RVA: 0x00245D77 File Offset: 0x00243F77
			public bool isNull
			{
				get
				{
					return this.gameObject == null;
				}
			}

			// Token: 0x170006F7 RID: 1783
			// (get) Token: 0x0600542E RID: 21550 RVA: 0x00245D85 File Offset: 0x00243F85
			public Transform transform
			{
				get
				{
					if (this.gameObject == null)
					{
						return null;
					}
					return this.gameObject.transform;
				}
			}

			// Token: 0x0600542F RID: 21551 RVA: 0x00245DA4 File Offset: 0x00243FA4
			public ObjectControl(GameObject input)
			{
				this.gameObject = input;
				this.baseScale = this.gameObject.transform.localScale;
			}

			// Token: 0x06005430 RID: 21552 RVA: 0x00245E07 File Offset: 0x00244007
			public void Destroy()
			{
				if (this.gameObject == null)
				{
					return;
				}
				Object.Destroy(this.gameObject);
			}

			// Token: 0x06005431 RID: 21553 RVA: 0x00245E23 File Offset: 0x00244023
			public void DestroyImmediate()
			{
				if (this.gameObject == null)
				{
					return;
				}
				Object.DestroyImmediate(this.gameObject);
			}

			// Token: 0x06005432 RID: 21554 RVA: 0x00245E40 File Offset: 0x00244040
			public void Apply()
			{
				if (this.gameObject == null)
				{
					return;
				}
				this.transform.position = this.position;
				this.transform.rotation = this.rotation;
				this.transform.localScale = this.scale;
				this.gameObject.SetActive(this.active);
			}

			// Token: 0x0400446C RID: 17516
			public GameObject gameObject;

			// Token: 0x0400446D RID: 17517
			public Vector3 position = Vector3.zero;

			// Token: 0x0400446E RID: 17518
			public Quaternion rotation = Quaternion.identity;

			// Token: 0x0400446F RID: 17519
			public Vector3 scale = Vector3.one;

			// Token: 0x04004470 RID: 17520
			public bool active = true;

			// Token: 0x04004471 RID: 17521
			public Vector3 baseScale = Vector3.one;
		}

		// Token: 0x0200098A RID: 2442
		public enum ObjectMethod
		{
			// Token: 0x04004473 RID: 17523
			Instantiate,
			// Token: 0x04004474 RID: 17524
			GetChildren
		}

		// Token: 0x0200098B RID: 2443
		public enum Positioning
		{
			// Token: 0x04004476 RID: 17526
			Stretch,
			// Token: 0x04004477 RID: 17527
			Clip
		}

		// Token: 0x0200098C RID: 2444
		public enum Iteration
		{
			// Token: 0x04004479 RID: 17529
			Ordered,
			// Token: 0x0400447A RID: 17530
			Random
		}
	}
}
