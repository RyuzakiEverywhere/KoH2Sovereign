using System;
using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x0200042F RID: 1071
	public abstract class GunCombat : RiderCombatAbility
	{
		// Token: 0x06003999 RID: 14745 RVA: 0x001BFDDC File Offset: 0x001BDFDC
		public override bool TypeOfAbility(IMWeapon weapon)
		{
			return weapon is IGun;
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x0600399A RID: 14746 RVA: 0x001BFDE7 File Offset: 0x001BDFE7
		public override WeaponType Type
		{
			get
			{
				return MalbersAnimations.WeaponType.Pistol;
			}
		}

		// Token: 0x0600399B RID: 14747 RVA: 0x001BFDE7 File Offset: 0x001BDFE7
		public override WeaponType WeaponType()
		{
			return MalbersAnimations.WeaponType.Pistol;
		}

		// Token: 0x0600399C RID: 14748 RVA: 0x001BFDEA File Offset: 0x001BDFEA
		public override void StartAbility(RiderCombat ridercombat)
		{
			base.StartAbility(ridercombat);
			this.DefaultInputType = this.RC.InputAttack1.GetPressed;
		}

		// Token: 0x0600399D RID: 14749 RVA: 0x001BFE0C File Offset: 0x001BE00C
		public override void ActivateAbility()
		{
			this.Gun = (this.RC.Active_IMWeapon as IGun);
			this.isReloading = false;
			if (this.Gun == null)
			{
				return;
			}
			if (!this.Gun.IsAutomatic)
			{
				this.RC.InputAttack1.GetPressed = InputButton.Down;
			}
			this.currentFireRateTime = 0f;
		}

		// Token: 0x0600399E RID: 14750 RVA: 0x001BFE68 File Offset: 0x001BE068
		public override void UpdateAbility()
		{
			if (this.Gun != null && this.Gun.IsAiming != this.RC.IsAiming)
			{
				this.Gun.IsAiming = this.RC.IsAiming;
			}
		}

		// Token: 0x0600399F RID: 14751 RVA: 0x001BFEA0 File Offset: 0x001BE0A0
		public override void ReloadWeapon()
		{
			this.PistolReload();
		}

		// Token: 0x060039A0 RID: 14752 RVA: 0x001BFEA8 File Offset: 0x001BE0A8
		public override void PrimaryAttack()
		{
			if (this.RC.IsAiming && this.RC.WeaponAction != WeaponActions.Fire_Proyectile)
			{
				this.PistolAttack();
			}
		}

		// Token: 0x060039A1 RID: 14753 RVA: 0x001BFECC File Offset: 0x001BE0CC
		protected virtual void PistolAttack()
		{
			if (this.isReloading)
			{
				return;
			}
			if (this.Gun.AmmoInChamber > 0)
			{
				WeaponActions weaponAction = this.RC.WeaponAction;
				this.RC.SetAction(WeaponActions.Fire_Proyectile);
				this.Gun.FireProyectile(this.RC.AimRayCastHit);
				this.RC.OnAttack.Invoke(this.RC.Active_IMWeapon);
				this.currentFireRateTime = Time.time;
				return;
			}
			if (Time.time - this.currentFireRateTime > 0.5f)
			{
				this.Gun.PlaySound(4);
				this.currentFireRateTime = Time.time;
			}
		}

		// Token: 0x060039A2 RID: 14754 RVA: 0x001BFF73 File Offset: 0x001BE173
		public virtual void PistolReload()
		{
			if (this.Gun.Reload())
			{
				this.RC.SetAction(this.RC.Active_IMWeapon.RightHand ? WeaponActions.ReloadRight : WeaponActions.ReloadLeft);
				return;
			}
			this.RC.SetAction(WeaponActions.Idle);
		}

		// Token: 0x060039A3 RID: 14755 RVA: 0x001BFFB4 File Offset: 0x001BE1B4
		public override void ResetAbility()
		{
			base.ResetAbility();
			if (this.Gun == null)
			{
				return;
			}
			this.isReloading = false;
			if (!this.Gun.IsAutomatic)
			{
				this.RC.InputAttack1.GetPressed = this.DefaultInputType;
			}
			this.EnableAimIKBehaviour(false);
			this.Gun = null;
		}

		// Token: 0x060039A4 RID: 14756 RVA: 0x001C0008 File Offset: 0x001BE208
		public virtual void FinishReload()
		{
			this.RC.SetAction(this.RC.IsAiming ? (this.RC.Active_IMWeapon.RightHand ? WeaponActions.AimRight : WeaponActions.AimLeft) : WeaponActions.Idle);
		}

		// Token: 0x060039A5 RID: 14757 RVA: 0x001C003E File Offset: 0x001BE23E
		public void IsReloading(bool value)
		{
			this.isReloading = value;
		}

		// Token: 0x060039A6 RID: 14758 RVA: 0x001C0047 File Offset: 0x001BE247
		public void ResetDoubleShoot()
		{
			this.Anim.SetInteger(Hash.IDInt, 0);
		}

		// Token: 0x060039A7 RID: 14759 RVA: 0x001C005C File Offset: 0x001BE25C
		protected void EnableAimIKBehaviour(bool value)
		{
			AimIKBehaviour[] behaviours = this.Anim.GetBehaviours<AimIKBehaviour>();
			for (int i = 0; i < behaviours.Length; i++)
			{
				behaviours[i].active = value;
			}
		}

		// Token: 0x040029B3 RID: 10675
		private IGun Gun;

		// Token: 0x040029B4 RID: 10676
		protected bool isReloading;

		// Token: 0x040029B5 RID: 10677
		protected float currentFireRateTime;

		// Token: 0x040029B6 RID: 10678
		protected InputButton DefaultInputType;
	}
}
