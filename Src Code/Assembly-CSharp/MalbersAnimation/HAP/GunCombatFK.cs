using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000430 RID: 1072
	[CreateAssetMenu(menuName = "Malbers Animations/HAP/Gun Combat FK")]
	public class GunCombatFK : GunCombat
	{
		// Token: 0x060039A9 RID: 14761 RVA: 0x001C0094 File Offset: 0x001BE294
		public override void ActivateAbility()
		{
			base.ActivateAbility();
			base.EnableAimIKBehaviour(false);
		}

		// Token: 0x060039AA RID: 14762 RVA: 0x001C00A3 File Offset: 0x001BE2A3
		public override void LateUpdateAbility()
		{
			this.FixAimPoseFK();
		}

		// Token: 0x060039AB RID: 14763 RVA: 0x001C00AC File Offset: 0x001BE2AC
		protected virtual void FixAimPoseFK()
		{
			if (this.RC.IsAiming && this.RC.IsAiming && this.RC.WeaponAction != WeaponActions.ReloadRight && this.RC.WeaponAction != WeaponActions.ReloadLeft)
			{
				Quaternion b = Quaternion.LookRotation(this.RC.AimDirection, Vector3.up);
				Quaternion lhs = Quaternion.LookRotation(this.RC.AimDirection);
				Vector3 normalized = Vector3.Cross(Vector3.up, this.RC.AimDirection).normalized;
				float angle = Vector3.Angle(Vector3.up, this.RC.AimDirection) - 90f;
				Debug.DrawRay(this.RC.Active_IMWeapon.RightHand ? this.RC.RightShoulder.position : this.RC.LeftShoulder.position, normalized, Color.green);
				if (this.RC.Active_IMWeapon.RightHand)
				{
					this.RC.RightShoulder.RotateAround(this.RC.RightShoulder.position, normalized, angle);
					this.RC.RightShoulder.rotation *= Quaternion.Euler(this.RightShoulderOffset);
					if (!this.RC.Target)
					{
						this.RC.RightShoulder.RotateAround(this.RC.RightShoulder.position, Vector3.up, this.RC.IsCamRightSide ? 0f : (-this.AimHorizontalOffset));
					}
				}
				else
				{
					this.RC.LeftShoulder.RotateAround(this.RC.LeftShoulder.position, normalized, angle);
					this.RC.LeftShoulder.rotation *= Quaternion.Euler(this.LeftShoulderOffset);
					if (!this.RC.Target)
					{
						this.RC.LeftShoulder.RotateAround(this.RC.LeftShoulder.position, Vector3.up, this.RC.IsCamRightSide ? this.AimHorizontalOffset : 0f);
					}
				}
				this.RC.Head.rotation = Quaternion.Slerp(this.RC.Head.rotation, lhs * Quaternion.Euler(this.HeadOffset), this.headLookWeight);
				if (this.RC.WeaponAction != WeaponActions.Fire_Proyectile)
				{
					if (this.RC.Active_IMWeapon.RightHand)
					{
						this.RC.RightHand.rotation = this.Delta_Rotation * Quaternion.Euler(this.RightHandOffset);
					}
					else
					{
						this.RC.LeftHand.rotation = this.Delta_Rotation * Quaternion.Euler(this.LeftHandOffset);
					}
					this.Delta_Rotation = Quaternion.Lerp(this.Delta_Rotation, b, Time.deltaTime * 20f);
				}
			}
		}

		// Token: 0x040029B7 RID: 10679
		public float AimHorizontalOffset = 20f;

		// Token: 0x040029B8 RID: 10680
		[Header("Right Offsets")]
		public Vector3 RightShoulderOffset = new Vector3(-90f, 90f, 0f);

		// Token: 0x040029B9 RID: 10681
		public Vector3 RightHandOffset = new Vector3(-90f, 90f, 0f);

		// Token: 0x040029BA RID: 10682
		[Header("Right Offsets")]
		public Vector3 LeftShoulderOffset = new Vector3(90f, 90f, 0f);

		// Token: 0x040029BB RID: 10683
		public Vector3 LeftHandOffset = new Vector3(90f, 90f, 0f);

		// Token: 0x040029BC RID: 10684
		[Space]
		public Vector3 HeadOffset = new Vector3(0f, -90f, -90f);

		// Token: 0x040029BD RID: 10685
		[Range(0f, 1f)]
		public float headLookWeight = 0.7f;

		// Token: 0x040029BE RID: 10686
		protected Quaternion Delta_Rotation;
	}
}
