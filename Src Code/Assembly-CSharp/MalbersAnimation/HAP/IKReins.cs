using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000429 RID: 1065
	[RequireComponent(typeof(Mountable))]
	public class IKReins : MonoBehaviour
	{
		// Token: 0x0600393A RID: 14650 RVA: 0x001BEBBB File Offset: 0x001BCDBB
		private void Awake()
		{
			this.Montura = base.GetComponent<Mountable>();
		}

		// Token: 0x0600393B RID: 14651 RVA: 0x001BEBCC File Offset: 0x001BCDCC
		private void Start()
		{
			if (this.ReinLeftHand && this.ReinRightHand)
			{
				this.LocalStride_L = this.ReinLeftHand.localPosition;
				this.LocalStride_R = this.ReinRightHand.localPosition;
				return;
			}
			Debug.LogWarning("Some of the Reins has not been set on the inspector. Please fill the values");
		}

		// Token: 0x0600393C RID: 14652 RVA: 0x001BEC20 File Offset: 0x001BCE20
		private void OnEnable()
		{
			this.Montura.OnMounted.AddListener(new UnityAction(this.OnRiderMounted));
			this.Montura.OnDismounted.AddListener(new UnityAction(this.OnRiderDismounted));
		}

		// Token: 0x0600393D RID: 14653 RVA: 0x001BEC5A File Offset: 0x001BCE5A
		private void OnDisable()
		{
			this.Montura.OnMounted.RemoveListener(new UnityAction(this.OnRiderMounted));
			this.Montura.OnDismounted.RemoveListener(new UnityAction(this.OnRiderDismounted));
		}

		// Token: 0x0600393E RID: 14654 RVA: 0x001BEC94 File Offset: 0x001BCE94
		public void RightHand_is_Free(bool value)
		{
			this.freeRightHand = value;
			if (!value && this.ReinRightHand)
			{
				this.ReinRightHand.localPosition = this.LocalStride_R;
			}
		}

		// Token: 0x0600393F RID: 14655 RVA: 0x001BECBE File Offset: 0x001BCEBE
		public void LeftHand_is_Free(bool value)
		{
			this.freeLeftHand = value;
			if (!value && this.ReinLeftHand)
			{
				this.ReinLeftHand.localPosition = this.LocalStride_L;
			}
		}

		// Token: 0x06003940 RID: 14656 RVA: 0x001BECE8 File Offset: 0x001BCEE8
		private void OnRiderMounted()
		{
			Animator anim = this.Montura.ActiveRider.Anim;
			this.riderHand_L = anim.GetBoneTransform(HumanBodyBones.LeftHand);
			this.riderHand_R = anim.GetBoneTransform(HumanBodyBones.RightHand);
		}

		// Token: 0x06003941 RID: 14657 RVA: 0x001BED22 File Offset: 0x001BCF22
		private void OnRiderDismounted()
		{
			this.riderHand_L = null;
			this.riderHand_R = null;
		}

		// Token: 0x06003942 RID: 14658 RVA: 0x001BED34 File Offset: 0x001BCF34
		private void LateUpdate()
		{
			if (!this.ReinLeftHand || !this.ReinRightHand)
			{
				return;
			}
			if (this.Montura.ActiveRider && this.Montura.ActiveRider.IsRiding)
			{
				if (this.freeLeftHand)
				{
					this.ReinLeftHand.position = Vector3.Lerp(this.riderHand_L.position, this.riderHand_L.GetChild(1).position, 0.5f);
				}
				else if (this.freeRightHand)
				{
					this.ReinLeftHand.position = Vector3.Lerp(this.riderHand_R.position, this.riderHand_R.GetChild(1).position, 0.5f);
				}
				if (this.freeRightHand)
				{
					this.ReinRightHand.position = Vector3.Lerp(this.riderHand_R.position, this.riderHand_R.GetChild(1).position, 0.5f);
					return;
				}
				if (this.freeLeftHand)
				{
					this.ReinRightHand.position = Vector3.Lerp(this.riderHand_L.position, this.riderHand_L.GetChild(1).position, 0.5f);
					return;
				}
			}
			else
			{
				this.ReinLeftHand.localPosition = this.LocalStride_L;
				this.ReinRightHand.localPosition = this.LocalStride_R;
			}
		}

		// Token: 0x0400296D RID: 10605
		public Transform ReinLeftHand;

		// Token: 0x0400296E RID: 10606
		public Transform ReinRightHand;

		// Token: 0x0400296F RID: 10607
		protected Vector3 LocalStride_L;

		// Token: 0x04002970 RID: 10608
		protected Vector3 LocalStride_R;

		// Token: 0x04002971 RID: 10609
		protected Transform riderHand_L;

		// Token: 0x04002972 RID: 10610
		protected Transform riderHand_R;

		// Token: 0x04002973 RID: 10611
		protected Mountable Montura;

		// Token: 0x04002974 RID: 10612
		protected bool freeRightHand = true;

		// Token: 0x04002975 RID: 10613
		protected bool freeLeftHand = true;
	}
}
