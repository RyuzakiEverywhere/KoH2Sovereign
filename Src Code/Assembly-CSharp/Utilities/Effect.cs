using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000461 RID: 1121
	[Serializable]
	public class Effect
	{
		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06003B1E RID: 15134 RVA: 0x001C603D File Offset: 0x001C423D
		// (set) Token: 0x06003B1F RID: 15135 RVA: 0x001C6045 File Offset: 0x001C4245
		public Transform Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06003B20 RID: 15136 RVA: 0x001C604E File Offset: 0x001C424E
		// (set) Token: 0x06003B21 RID: 15137 RVA: 0x001C6056 File Offset: 0x001C4256
		public GameObject Instance
		{
			get
			{
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}

		// Token: 0x04002AF6 RID: 10998
		public string Name = "EffectName";

		// Token: 0x04002AF7 RID: 10999
		public int ID;

		// Token: 0x04002AF8 RID: 11000
		public bool active = true;

		// Token: 0x04002AF9 RID: 11001
		public Transform root;

		// Token: 0x04002AFA RID: 11002
		public bool isChild;

		// Token: 0x04002AFB RID: 11003
		public bool useRootRotation = true;

		// Token: 0x04002AFC RID: 11004
		public GameObject effect;

		// Token: 0x04002AFD RID: 11005
		public Vector3 RotationOffset;

		// Token: 0x04002AFE RID: 11006
		public Vector3 PositionOffset;

		// Token: 0x04002AFF RID: 11007
		public Vector3 ScaleMultiplier = Vector3.one;

		// Token: 0x04002B00 RID: 11008
		public float life = 10f;

		// Token: 0x04002B01 RID: 11009
		public float delay;

		// Token: 0x04002B02 RID: 11010
		public bool instantiate = true;

		// Token: 0x04002B03 RID: 11011
		public bool toggleable;

		// Token: 0x04002B04 RID: 11012
		public bool On;

		// Token: 0x04002B05 RID: 11013
		public EffectModifier Modifier;

		// Token: 0x04002B06 RID: 11014
		public UnityEvent OnPlay;

		// Token: 0x04002B07 RID: 11015
		public UnityEvent OnStop;

		// Token: 0x04002B08 RID: 11016
		protected Transform owner;

		// Token: 0x04002B09 RID: 11017
		protected GameObject instance;
	}
}
