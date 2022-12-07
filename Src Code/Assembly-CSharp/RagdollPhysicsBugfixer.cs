using System;
using UnityEngine;

// Token: 0x0200031F RID: 799
public class RagdollPhysicsBugfixer : MonoBehaviour
{
	// Token: 0x060031FD RID: 12797 RVA: 0x00195544 File Offset: 0x00193744
	private void Start()
	{
		this.rigs = base.GetComponentsInChildren<Rigidbody>();
		this.rend = base.GetComponentInChildren<SkinnedMeshRenderer>();
		if (this.rend != null)
		{
			this.bone_count = this.rend.bones.Length;
		}
		if (this.rend != null)
		{
			this.bones = new RagdollPhysicsBugfixer.BonePos[this.rend.bones.Length];
			for (int i = 0; i < this.rend.bones.Length; i++)
			{
				Transform transform = this.rend.bones[i];
				if (!(transform == null))
				{
					this.bones[i] = new RagdollPhysicsBugfixer.BonePos
					{
						pos = transform.localPosition,
						rot = transform.localRotation,
						scale = transform.localScale
					};
				}
			}
		}
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x00195620 File Offset: 0x00193820
	private void Update()
	{
		if (this.was_on != this.anim_on)
		{
			this.was_on = this.anim_on;
			for (int i = 0; i < this.rigs.Length; i++)
			{
				this.rigs[i].isKinematic = this.anim_on;
				if (!this.anim_on)
				{
					this.rigs[i].velocity = Vector3.zero;
				}
			}
			if (this.anim_on && this.rend != null)
			{
				for (int j = 0; j < this.rend.bones.Length; j++)
				{
					Transform transform = this.rend.bones[j];
					if (!(transform == null))
					{
						RagdollPhysicsBugfixer.BonePos bonePos = this.bones[j];
						transform.transform.localPosition = bonePos.pos;
						transform.transform.localRotation = bonePos.rot;
						transform.transform.localScale = bonePos.scale;
					}
				}
			}
		}
	}

	// Token: 0x04002161 RID: 8545
	private RagdollPhysicsBugfixer.BonePos[] bones;

	// Token: 0x04002162 RID: 8546
	private Rigidbody[] rigs;

	// Token: 0x04002163 RID: 8547
	public bool anim_on;

	// Token: 0x04002164 RID: 8548
	public int bone_count;

	// Token: 0x04002165 RID: 8549
	private SkinnedMeshRenderer rend;

	// Token: 0x04002166 RID: 8550
	private bool was_on;

	// Token: 0x02000883 RID: 2179
	public struct BonePos
	{
		// Token: 0x04003FD1 RID: 16337
		public Vector3 pos;

		// Token: 0x04003FD2 RID: 16338
		public Quaternion rot;

		// Token: 0x04003FD3 RID: 16339
		public Vector3 scale;
	}
}
