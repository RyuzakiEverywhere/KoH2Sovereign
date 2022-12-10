using System;
using MalbersAnimations.Weapons;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000432 RID: 1074
	[CreateAssetMenu(menuName = "Malbers Animations/HAP/MeleeCombat")]
	public class MeleeCombat : RiderCombatAbility
	{
		// Token: 0x060039B0 RID: 14768 RVA: 0x001C04E6 File Offset: 0x001BE6E6
		public override bool TypeOfAbility(IMWeapon weapon)
		{
			return weapon is IMelee;
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x060039B1 RID: 14769 RVA: 0x0002C53B File Offset: 0x0002A73B
		public override WeaponType Type
		{
			get
			{
				return MalbersAnimations.WeaponType.Melee;
			}
		}

		// Token: 0x060039B2 RID: 14770 RVA: 0x0002C53B File Offset: 0x0002A73B
		public override WeaponType WeaponType()
		{
			return MalbersAnimations.WeaponType.Melee;
		}

		// Token: 0x060039B3 RID: 14771 RVA: 0x001C04F1 File Offset: 0x001BE6F1
		public override void ActivateAbility()
		{
			this.timeOnAttack = 0f;
			this.isAttacking = false;
		}

		// Token: 0x060039B4 RID: 14772 RVA: 0x001C0505 File Offset: 0x001BE705
		private void CheckAttacking()
		{
			if (this.isAttacking && Time.time - this.timeOnAttack > this.meleeAttackDelay)
			{
				this.isAttacking = false;
			}
		}

		// Token: 0x060039B5 RID: 14773 RVA: 0x001C052A File Offset: 0x001BE72A
		public override void PrimaryAttack()
		{
			this.CheckAttacking();
			if (!this.isAttacking)
			{
				this.RiderMeleeAttack(false);
			}
		}

		// Token: 0x060039B6 RID: 14774 RVA: 0x001C0541 File Offset: 0x001BE741
		public override void SecondaryAttack()
		{
			this.CheckAttacking();
			if (!this.isAttacking)
			{
				this.RiderMeleeAttack(true);
			}
		}

		// Token: 0x060039B7 RID: 14775 RVA: 0x001C0558 File Offset: 0x001BE758
		protected virtual void RiderMeleeAttack(bool rightSide)
		{
			this.RC.Anim.SetInteger(Hash.IDInt, -99);
			if (rightSide)
			{
				this.RC.Anim.SetBool(Hash.Attack2, true);
			}
			else
			{
				this.RC.Anim.SetBool(Hash.Attack1, true);
			}
			int value;
			if (this.RC.Active_IMWeapon.RightHand)
			{
				if (rightSide)
				{
					value = Random.Range(1, 3);
				}
				else
				{
					value = Random.Range(3, 5);
				}
			}
			else if (rightSide)
			{
				value = Random.Range(7, 9);
			}
			else
			{
				value = Random.Range(5, 7);
			}
			this.RC.Anim.SetInteger(RiderCombat.Hash_WeaponAction, value);
			this.isAttacking = true;
			this.timeOnAttack = Time.time;
			this.RC.OnAttack.Invoke(this.RC.Active_IMWeapon);
		}

		// Token: 0x060039B8 RID: 14776 RVA: 0x001C062F File Offset: 0x001BE82F
		public virtual void OnCauseDamage(bool value)
		{
			(this.RC.Active_IMWeapon as IMelee).CanDoDamage(value);
		}

		// Token: 0x060039B9 RID: 14777 RVA: 0x0002C538 File Offset: 0x0002A738
		public override bool CanAim()
		{
			return false;
		}

		// Token: 0x040029C3 RID: 10691
		[Tooltip("Time before attacking again with melee")]
		public float meleeAttackDelay = 0.5f;

		// Token: 0x040029C4 RID: 10692
		private bool isAttacking;

		// Token: 0x040029C5 RID: 10693
		private float timeOnAttack;
	}
}
