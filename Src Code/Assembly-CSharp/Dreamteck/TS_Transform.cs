using System;
using UnityEngine;

namespace Dreamteck
{
	// Token: 0x020004AB RID: 1195
	[Serializable]
	public class TS_Transform
	{
		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06003E94 RID: 16020 RVA: 0x001E0346 File Offset: 0x001DE546
		// (set) Token: 0x06003E95 RID: 16021 RVA: 0x001E0365 File Offset: 0x001DE565
		public Vector3 position
		{
			get
			{
				return new Vector3(this.posX, this.posY, this.posZ);
			}
			set
			{
				this.setPosition = true;
				this.setLocalPosition = false;
				this.posX = value.x;
				this.posY = value.y;
				this.posZ = value.z;
			}
		}

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06003E96 RID: 16022 RVA: 0x001E039F File Offset: 0x001DE59F
		// (set) Token: 0x06003E97 RID: 16023 RVA: 0x001E03C8 File Offset: 0x001DE5C8
		public Quaternion rotation
		{
			get
			{
				return new Quaternion(this.rotX, this.rotY, this.rotZ, this.rotW);
			}
			set
			{
				this.setRotation = true;
				this.setLocalRotation = false;
				this.rotX = value.x;
				this.rotY = value.y;
				this.rotZ = value.z;
				this.rotW = value.w;
			}
		}

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06003E98 RID: 16024 RVA: 0x001E041B File Offset: 0x001DE61B
		// (set) Token: 0x06003E99 RID: 16025 RVA: 0x001E043A File Offset: 0x001DE63A
		public Vector3 scale
		{
			get
			{
				return new Vector3(this.scaleX, this.scaleY, this.scaleZ);
			}
			set
			{
				this.setScale = true;
				this.scaleX = value.x;
				this.scaleY = value.y;
				this.scaleZ = value.z;
			}
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06003E9A RID: 16026 RVA: 0x001E046D File Offset: 0x001DE66D
		// (set) Token: 0x06003E9B RID: 16027 RVA: 0x001E048C File Offset: 0x001DE68C
		public Vector3 lossyScale
		{
			get
			{
				return new Vector3(this.lossyScaleX, this.lossyScaleY, this.lossyScaleZ);
			}
			set
			{
				this.setScale = true;
				this.lossyScaleX = value.x;
				this.lossyScaleY = value.y;
				this.lossyScaleZ = value.z;
			}
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06003E9C RID: 16028 RVA: 0x001E04BF File Offset: 0x001DE6BF
		// (set) Token: 0x06003E9D RID: 16029 RVA: 0x001E04DE File Offset: 0x001DE6DE
		public Vector3 localPosition
		{
			get
			{
				return new Vector3(this.lposX, this.lposY, this.lposZ);
			}
			set
			{
				this.setLocalPosition = true;
				this.setPosition = false;
				this.lposX = value.x;
				this.lposY = value.y;
				this.lposZ = value.z;
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06003E9E RID: 16030 RVA: 0x001E0518 File Offset: 0x001DE718
		// (set) Token: 0x06003E9F RID: 16031 RVA: 0x001E0540 File Offset: 0x001DE740
		public Quaternion localRotation
		{
			get
			{
				return new Quaternion(this.lrotX, this.lrotY, this.lrotZ, this.lrotW);
			}
			set
			{
				this.setLocalRotation = true;
				this.setRotation = false;
				this.lrotX = value.x;
				this.lrotY = value.y;
				this.lrotZ = value.z;
				this.lrotW = value.w;
			}
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06003EA0 RID: 16032 RVA: 0x001E0593 File Offset: 0x001DE793
		public Transform transform
		{
			get
			{
				return this._transform;
			}
		}

		// Token: 0x06003EA1 RID: 16033 RVA: 0x001E059C File Offset: 0x001DE79C
		public TS_Transform(Transform input)
		{
			this.SetTransform(input);
		}

		// Token: 0x06003EA2 RID: 16034 RVA: 0x001E0604 File Offset: 0x001DE804
		public void Update()
		{
			if (this.transform == null)
			{
				return;
			}
			if (this.setPosition)
			{
				this._transform.position = this.position;
			}
			else if (this.setLocalPosition)
			{
				this._transform.localPosition = this.localPosition;
			}
			else
			{
				this.position = this._transform.position;
				this.localPosition = this._transform.localPosition;
			}
			if (this.setScale)
			{
				this._transform.localScale = this.scale;
			}
			else
			{
				this.scale = this._transform.localScale;
			}
			this.lossyScale = this._transform.lossyScale;
			if (this.setRotation)
			{
				this._transform.rotation = this.rotation;
			}
			else if (this.setLocalRotation)
			{
				this._transform.localRotation = this.localRotation;
			}
			else
			{
				this.rotation = this._transform.rotation;
				this.localRotation = this._transform.localRotation;
			}
			this.setPosition = (this.setLocalPosition = (this.setRotation = (this.setLocalRotation = (this.setScale = false))));
		}

		// Token: 0x06003EA3 RID: 16035 RVA: 0x001E0738 File Offset: 0x001DE938
		public void SetTransform(Transform input)
		{
			this._transform = input;
			this.setPosition = (this.setLocalPosition = (this.setRotation = (this.setLocalRotation = (this.setScale = false))));
			this.Update();
		}

		// Token: 0x06003EA4 RID: 16036 RVA: 0x001E077D File Offset: 0x001DE97D
		public bool HasChange()
		{
			return this.HasPositionChange() || this.HasRotationChange() || this.HasScaleChange();
		}

		// Token: 0x06003EA5 RID: 16037 RVA: 0x001E0798 File Offset: 0x001DE998
		public bool HasPositionChange()
		{
			return this.posX != this._transform.position.x || this.posY != this._transform.position.y || this.posZ != this._transform.position.z;
		}

		// Token: 0x06003EA6 RID: 16038 RVA: 0x001E07F8 File Offset: 0x001DE9F8
		public bool HasRotationChange()
		{
			return this.rotX != this._transform.rotation.x || this.rotY != this._transform.rotation.y || this.rotZ != this._transform.rotation.z || this.rotW != this._transform.rotation.w;
		}

		// Token: 0x06003EA7 RID: 16039 RVA: 0x001E0874 File Offset: 0x001DEA74
		public bool HasScaleChange()
		{
			return this.lossyScaleX != this._transform.lossyScale.x || this.lossyScaleY != this._transform.lossyScale.y || this.lossyScaleZ != this._transform.lossyScale.z;
		}

		// Token: 0x06003EA8 RID: 16040 RVA: 0x001E08D4 File Offset: 0x001DEAD4
		public Vector3 TransformPoint(Vector3 point)
		{
			Vector3 point2 = new Vector3(point.x * this.lossyScaleX, point.y * this.lossyScaleY, point.z * this.lossyScaleZ);
			Vector3 b = this.rotation * point2;
			return this.position + b;
		}

		// Token: 0x06003EA9 RID: 16041 RVA: 0x001E092E File Offset: 0x001DEB2E
		public Vector3 TransformDirection(Vector3 direction)
		{
			return this.TransformPoint(direction) - this.position;
		}

		// Token: 0x06003EAA RID: 16042 RVA: 0x001E0942 File Offset: 0x001DEB42
		public Vector3 InverseTransformPoint(Vector3 point)
		{
			return this.InverseTransformDirection(point - this.position);
		}

		// Token: 0x06003EAB RID: 16043 RVA: 0x001E0958 File Offset: 0x001DEB58
		public Vector3 InverseTransformDirection(Vector3 direction)
		{
			Vector3 vector = Quaternion.Inverse(this.rotation) * direction;
			return new Vector3(vector.x / this.lossyScaleX, vector.y / this.lossyScaleY, vector.z / this.lossyScaleZ);
		}

		// Token: 0x06003EAC RID: 16044 RVA: 0x001E09A9 File Offset: 0x001DEBA9
		public T GetComponent<T>()
		{
			return this._transform.GetComponent<T>();
		}

		// Token: 0x04002C60 RID: 11360
		private bool setPosition;

		// Token: 0x04002C61 RID: 11361
		private bool setRotation;

		// Token: 0x04002C62 RID: 11362
		private bool setScale;

		// Token: 0x04002C63 RID: 11363
		private bool setLocalPosition;

		// Token: 0x04002C64 RID: 11364
		private bool setLocalRotation;

		// Token: 0x04002C65 RID: 11365
		[SerializeField]
		[HideInInspector]
		private Transform _transform;

		// Token: 0x04002C66 RID: 11366
		[SerializeField]
		[HideInInspector]
		private volatile float posX;

		// Token: 0x04002C67 RID: 11367
		[SerializeField]
		[HideInInspector]
		private volatile float posY;

		// Token: 0x04002C68 RID: 11368
		[SerializeField]
		[HideInInspector]
		private volatile float posZ;

		// Token: 0x04002C69 RID: 11369
		[SerializeField]
		[HideInInspector]
		private volatile float scaleX = 1f;

		// Token: 0x04002C6A RID: 11370
		[SerializeField]
		[HideInInspector]
		private volatile float scaleY = 1f;

		// Token: 0x04002C6B RID: 11371
		[SerializeField]
		[HideInInspector]
		private volatile float scaleZ = 1f;

		// Token: 0x04002C6C RID: 11372
		[SerializeField]
		[HideInInspector]
		private volatile float lossyScaleX = 1f;

		// Token: 0x04002C6D RID: 11373
		[SerializeField]
		[HideInInspector]
		private volatile float lossyScaleY = 1f;

		// Token: 0x04002C6E RID: 11374
		[SerializeField]
		[HideInInspector]
		private volatile float lossyScaleZ = 1f;

		// Token: 0x04002C6F RID: 11375
		[SerializeField]
		[HideInInspector]
		private volatile float rotX;

		// Token: 0x04002C70 RID: 11376
		[SerializeField]
		[HideInInspector]
		private volatile float rotY;

		// Token: 0x04002C71 RID: 11377
		[SerializeField]
		[HideInInspector]
		private volatile float rotZ;

		// Token: 0x04002C72 RID: 11378
		[SerializeField]
		[HideInInspector]
		private volatile float rotW;

		// Token: 0x04002C73 RID: 11379
		[SerializeField]
		[HideInInspector]
		private volatile float lposX;

		// Token: 0x04002C74 RID: 11380
		[SerializeField]
		[HideInInspector]
		private volatile float lposY;

		// Token: 0x04002C75 RID: 11381
		[SerializeField]
		[HideInInspector]
		private volatile float lposZ;

		// Token: 0x04002C76 RID: 11382
		[SerializeField]
		[HideInInspector]
		private volatile float lrotX;

		// Token: 0x04002C77 RID: 11383
		[SerializeField]
		[HideInInspector]
		private volatile float lrotY;

		// Token: 0x04002C78 RID: 11384
		[SerializeField]
		[HideInInspector]
		private volatile float lrotZ;

		// Token: 0x04002C79 RID: 11385
		[SerializeField]
		[HideInInspector]
		private volatile float lrotW;
	}
}
