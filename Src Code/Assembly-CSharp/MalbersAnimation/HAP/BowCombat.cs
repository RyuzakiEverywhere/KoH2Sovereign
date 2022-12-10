using System;
using MalbersAnimations.Utilities;
using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x0200042E RID: 1070
	[CreateAssetMenu(menuName = "Malbers Animations/HAP/Bow Combat")]
	public class BowCombat : RiderCombatAbility
	{
		// Token: 0x06003984 RID: 14724 RVA: 0x001BF6A2 File Offset: 0x001BD8A2
		public override bool TypeOfAbility(IMWeapon weapon)
		{
			return weapon is IBow;
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06003985 RID: 14725 RVA: 0x001BF6AD File Offset: 0x001BD8AD
		public override WeaponType Type
		{
			get
			{
				return MalbersAnimations.WeaponType.Bow;
			}
		}

		// Token: 0x06003986 RID: 14726 RVA: 0x001BF6AD File Offset: 0x001BD8AD
		public override WeaponType WeaponType()
		{
			return MalbersAnimations.WeaponType.Bow;
		}

		// Token: 0x06003987 RID: 14727 RVA: 0x001BF6B0 File Offset: 0x001BD8B0
		public override void StartAbility(RiderCombat ridercombat)
		{
			base.StartAbility(ridercombat);
			this.KnotToHand = false;
		}

		// Token: 0x06003988 RID: 14728 RVA: 0x001BF6C0 File Offset: 0x001BD8C0
		public override void ActivateAbility()
		{
			this.Bow = (this.RC.Active_IMWeapon as IBow);
		}

		// Token: 0x06003989 RID: 14729 RVA: 0x001BF6D8 File Offset: 0x001BD8D8
		public override void UpdateAbility()
		{
			if (!this.RC.IsAiming && this.isHolding)
			{
				this.isHolding = false;
				this.HoldTime = 0f;
				this.RC.Anim.SetFloat(Hash.IDFloat, 0f);
				this.Bow.BendBow(0f);
			}
			this.BowKnotInHand();
		}

		// Token: 0x0600398A RID: 14730 RVA: 0x001BF73C File Offset: 0x001BD93C
		public override void PrimaryAttack()
		{
			this.BowAttack();
		}

		// Token: 0x0600398B RID: 14731 RVA: 0x001BF744 File Offset: 0x001BD944
		public override void PrimaryAttackReleased()
		{
			this.ReleaseArrow();
		}

		// Token: 0x0600398C RID: 14732 RVA: 0x001BF74C File Offset: 0x001BD94C
		public override void LateUpdateAbility()
		{
			this.FixAimPoseBow();
		}

		// Token: 0x0600398D RID: 14733 RVA: 0x001BF754 File Offset: 0x001BD954
		protected virtual void BowAttack()
		{
			if (this.RC.IsAiming && this.RC.WeaponAction != WeaponActions.Fire_Proyectile)
			{
				if (!(this.RC.Active_IMWeapon.RightHand ? (this.RC.HorizontalAngle < 0.5f) : (this.RC.HorizontalAngle > -0.5f)))
				{
					this.isHolding = false;
					this.HoldTime = 0f;
					return;
				}
				if (!this.isHolding)
				{
					this.RC.SetAction(WeaponActions.Hold);
					this.isHolding = true;
					this.HoldTime = 0f;
					return;
				}
				this.HoldBow();
			}
		}

		// Token: 0x0600398E RID: 14734 RVA: 0x001BF7FC File Offset: 0x001BD9FC
		private void ReleaseArrow()
		{
			if (this.RC.IsAiming && this.RC.WeaponAction != WeaponActions.Fire_Proyectile && this.isHolding)
			{
				this.Bow.KNot.rotation = Quaternion.LookRotation(this.RC.AimDirection);
				this.RC.SetAction(WeaponActions.Fire_Proyectile);
				this.isHolding = false;
				this.HoldTime = 0f;
				this.RC.Anim.SetFloat(Hash.IDFloat, 0f);
				this.Bow.ReleaseArrow(this.RC.AimDirection);
				this.Bow.BendBow(0f);
				this.RC.OnAttack.Invoke(this.RC.Active_IMWeapon);
			}
		}

		// Token: 0x0600398F RID: 14735 RVA: 0x001BF8D0 File Offset: 0x001BDAD0
		private void HoldBow()
		{
			this.HoldTime += Time.deltaTime;
			if (this.HoldTime <= this.Bow.HoldTime + Time.deltaTime)
			{
				this.Bow.BendBow(this.HoldTime / this.Bow.HoldTime);
			}
			this.RC.Anim.SetFloat(Hash.IDFloat, this.HoldTime / this.Bow.HoldTime);
		}

		// Token: 0x06003990 RID: 14736 RVA: 0x001BF94C File Offset: 0x001BDB4C
		public virtual void EquipArrow()
		{
			this.Bow.EquipArrow();
		}

		// Token: 0x06003991 RID: 14737 RVA: 0x0002C538 File Offset: 0x0002A738
		public override bool ChangeAimCameraSide()
		{
			return false;
		}

		// Token: 0x06003992 RID: 14738 RVA: 0x001BF959 File Offset: 0x001BDB59
		public override void ResetAbility()
		{
			this.KnotToHand = false;
			this.Bow = null;
		}

		// Token: 0x06003993 RID: 14739 RVA: 0x001BF96C File Offset: 0x001BDB6C
		protected virtual void FixAimPoseBow()
		{
			if (this.RC.IsAiming)
			{
				float num = this.RC.Active_IMWeapon.RightHand ? this.AimWeight.Evaluate(1f + this.RC.HorizontalAngle) : this.AimWeight.Evaluate(1f - this.RC.HorizontalAngle);
				Vector3 vector = this.RC.Target ? this.RC.AimDirection : (this.RC.AimDot ? MalbersTools.DirectionFromCameraNoRayCast(this.RC.AimDot.position) : this.cam.forward);
				Quaternion lhs = Quaternion.LookRotation(vector, this.RC.Target ? Vector3.up : this.cam.up);
				Vector3 axis = this.RC.Target ? Vector3.Cross(Vector3.up, vector).normalized : this.cam.right;
				this.RC.Chest.RotateAround(this.RC.Chest.position, axis, (Vector3.Angle(Vector3.up, vector) - 90f) * num);
				if (this.RC.Active_IMWeapon.RightHand)
				{
					this.RC.Chest.rotation *= Quaternion.Euler(this.ChestRight);
					this.RC.RightHand.rotation *= Quaternion.Euler(this.HandRight);
					this.RC.RightShoulder.rotation = Quaternion.Lerp(this.RC.RightShoulder.rotation, lhs * Quaternion.Euler(this.ShoulderRight), num);
					return;
				}
				this.RC.Chest.rotation *= Quaternion.Euler(this.ChestLeft);
				this.RC.LeftHand.rotation *= Quaternion.Euler(this.HandLeft);
				this.RC.LeftShoulder.rotation = Quaternion.Lerp(this.RC.LeftShoulder.rotation, lhs * Quaternion.Euler(this.ShoulderLeft), num);
			}
		}

		// Token: 0x06003994 RID: 14740 RVA: 0x001BFBDC File Offset: 0x001BDDDC
		public virtual void BowKnotToHand(bool enabled)
		{
			this.KnotToHand = enabled;
			IBow bow = this.RC.Active_IMWeapon as IBow;
			if (!this.KnotToHand && bow != null)
			{
				bow.RestoreKnot();
			}
		}

		// Token: 0x06003995 RID: 14741 RVA: 0x001BFC14 File Offset: 0x001BDE14
		protected void BowKnotInHand()
		{
			if (this.KnotToHand)
			{
				IBow bow = this.RC.Active_IMWeapon as IBow;
				bow.KNot.position = (this.RC.Anim.GetBoneTransform(this.RC.Active_IMWeapon.RightHand ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal).position + this.RC.Anim.GetBoneTransform(this.RC.Active_IMWeapon.RightHand ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal).position) / 2f;
				bow.KNot.position = bow.KNot.position;
			}
		}

		// Token: 0x06003996 RID: 14742 RVA: 0x001BFCC7 File Offset: 0x001BDEC7
		public override Transform AimRayOrigin()
		{
			return (this.RC.Active_IMWeapon as IBow).KNot;
		}

		// Token: 0x040029A6 RID: 10662
		private bool isHolding;

		// Token: 0x040029A7 RID: 10663
		private float HoldTime;

		// Token: 0x040029A8 RID: 10664
		private static Keyframe[] KeyFrames = new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1.25f, 1f),
			new Keyframe(1.5f, 0f),
			new Keyframe(2f, 0f)
		};

		// Token: 0x040029A9 RID: 10665
		[Header("Right Handed Bow Offsets")]
		public Vector3 ChestRight = new Vector3(25f, 0f, 0f);

		// Token: 0x040029AA RID: 10666
		public Vector3 ShoulderRight = new Vector3(5f, 0f, 0f);

		// Token: 0x040029AB RID: 10667
		public Vector3 HandRight;

		// Token: 0x040029AC RID: 10668
		[Header("Left Handed Bow Offsets")]
		public Vector3 ChestLeft = new Vector3(-25f, 0f, 0f);

		// Token: 0x040029AD RID: 10669
		public Vector3 ShoulderLeft = new Vector3(-5f, 0f, 0f);

		// Token: 0x040029AE RID: 10670
		public Vector3 HandLeft;

		// Token: 0x040029AF RID: 10671
		[Space]
		[Tooltip("This Curve is for straightening the aiming Arm while is on the Aiming State")]
		public AnimationCurve AimWeight = new AnimationCurve(BowCombat.KeyFrames);

		// Token: 0x040029B0 RID: 10672
		protected bool KnotToHand;

		// Token: 0x040029B1 RID: 10673
		protected Quaternion Delta_Hand;

		// Token: 0x040029B2 RID: 10674
		private IBow Bow;
	}
}
