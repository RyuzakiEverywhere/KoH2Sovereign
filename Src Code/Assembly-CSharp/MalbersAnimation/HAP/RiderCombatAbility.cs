using System;
using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000433 RID: 1075
	public abstract class RiderCombatAbility : ScriptableObject
	{
		// Token: 0x170003BB RID: 955
		// (get) Token: 0x060039BB RID: 14779
		public abstract WeaponType Type { get; }

		// Token: 0x060039BC RID: 14780
		public abstract bool TypeOfAbility(IMWeapon weapon);

		// Token: 0x060039BD RID: 14781
		public abstract WeaponType WeaponType();

		// Token: 0x060039BE RID: 14782 RVA: 0x001C065C File Offset: 0x001BE85C
		public virtual void StartAbility(RiderCombat ridercombat)
		{
			this.RC = ridercombat;
			Camera mainCamera = this.RC.rider.MainCamera;
			if (mainCamera)
			{
				this.cam = mainCamera.transform;
			}
			this.Anim = this.RC.Anim;
		}

		// Token: 0x060039BF RID: 14783 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void ActivateAbility()
		{
		}

		// Token: 0x060039C0 RID: 14784 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void FixedUpdateAbility()
		{
		}

		// Token: 0x060039C1 RID: 14785 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void PrimaryAttack()
		{
		}

		// Token: 0x060039C2 RID: 14786 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void PrimaryAttackReleased()
		{
		}

		// Token: 0x060039C3 RID: 14787 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void SecondaryAttack()
		{
		}

		// Token: 0x060039C4 RID: 14788 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void SecondaryAttackReleased()
		{
		}

		// Token: 0x060039C5 RID: 14789 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void ReloadWeapon()
		{
		}

		// Token: 0x060039C6 RID: 14790 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void UpdateAbility()
		{
		}

		// Token: 0x060039C7 RID: 14791 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void LateUpdateAbility()
		{
		}

		// Token: 0x060039C8 RID: 14792 RVA: 0x001C06A6 File Offset: 0x001BE8A6
		public virtual void ResetAbility()
		{
			if (this.RC.Active_IMWeapon == null)
			{
				return;
			}
			if (this.RC.debug)
			{
				Debug.Log("Ability Reseted");
			}
		}

		// Token: 0x060039C9 RID: 14793 RVA: 0x001C06CD File Offset: 0x001BE8CD
		public virtual void ListenAnimator(string Method, object value)
		{
			this.Invoke(Method, value);
		}

		// Token: 0x060039CA RID: 14794 RVA: 0x0002C53B File Offset: 0x0002A73B
		public virtual bool ChangeAimCameraSide()
		{
			return true;
		}

		// Token: 0x060039CB RID: 14795 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void IK()
		{
		}

		// Token: 0x060039CC RID: 14796 RVA: 0x0002C53B File Offset: 0x0002A73B
		public virtual bool CanAim()
		{
			return true;
		}

		// Token: 0x060039CD RID: 14797 RVA: 0x001C06D7 File Offset: 0x001BE8D7
		public virtual Transform AimRayOrigin()
		{
			if (!this.RC.Active_IMWeapon.RightHand)
			{
				return this.RC.LeftShoulder;
			}
			return this.RC.RightShoulder;
		}

		// Token: 0x060039CE RID: 14798 RVA: 0x000023FD File Offset: 0x000005FD
		public virtual void OnActionChange()
		{
		}

		// Token: 0x040029C6 RID: 10694
		protected RiderCombat RC;

		// Token: 0x040029C7 RID: 10695
		protected Transform cam;

		// Token: 0x040029C8 RID: 10696
		protected Animator Anim;

		// Token: 0x040029C9 RID: 10697
		protected IMWeapon weapon;
	}
}
