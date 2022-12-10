using System;
using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;
using MalbersAnimations.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000438 RID: 1080
	[RequireComponent(typeof(Rider3rdPerson))]
	public class RiderCombat : MonoBehaviour, IAnimatorListener
	{
		// Token: 0x06003A07 RID: 14855 RVA: 0x001C1A09 File Offset: 0x001BFC09
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
			if (this.ActiveAbility)
			{
				this.ActiveAbility.ListenAnimator(message, value);
			}
		}

		// Token: 0x06003A08 RID: 14856 RVA: 0x001C1A2D File Offset: 0x001BFC2D
		public virtual void AddAbility(RiderCombatAbility newAbility)
		{
			newAbility.StartAbility(this);
			this.CombatAbilities.Add(newAbility);
		}

		// Token: 0x06003A09 RID: 14857 RVA: 0x001C1A44 File Offset: 0x001BFC44
		public virtual void RightHand_is_Free(bool value)
		{
			if (this.rider.Montura != null)
			{
				IKReins component = this.rider.Montura.transform.GetComponent<IKReins>();
				if (component)
				{
					component.RightHand_is_Free(value);
				}
			}
		}

		// Token: 0x06003A0A RID: 14858 RVA: 0x001C1A8C File Offset: 0x001BFC8C
		public virtual void LeftHand_is_Free(bool value)
		{
			if (this.rider.Montura != null)
			{
				IKReins component = this.rider.Montura.transform.GetComponent<IKReins>();
				if (component)
				{
					component.LeftHand_is_Free(value);
				}
			}
		}

		// Token: 0x06003A0B RID: 14859 RVA: 0x001C1AD1 File Offset: 0x001BFCD1
		public virtual void SetActiveWeapon(GameObject weapon)
		{
			this.activeWeapon = weapon;
		}

		// Token: 0x06003A0C RID: 14860 RVA: 0x001C1ADA File Offset: 0x001BFCDA
		public virtual void ResetAiming()
		{
			if (this.IsAiming)
			{
				this.IsAiming = false;
				this.OnAimSide.Invoke(0);
				this.SetAction(this.CombatMode ? WeaponActions.Idle : WeaponActions.None);
			}
		}

		// Token: 0x06003A0D RID: 14861 RVA: 0x001C1B0A File Offset: 0x001BFD0A
		public virtual void ResetRiderCombat(bool storeWeapon)
		{
			this.ResetRiderCombat();
			this.SetAction(WeaponActions.None);
			if (storeWeapon)
			{
				this.Store_Weapon();
			}
			this.LinkAnimator();
		}

		// Token: 0x06003A0E RID: 14862 RVA: 0x001C1B28 File Offset: 0x001BFD28
		public virtual void ResetRiderCombat()
		{
			this.CombatMode = false;
			this.SetWeaponType(WeaponType.None);
			this.ResetActiveAbility();
			this.ResetAiming();
		}

		// Token: 0x06003A0F RID: 14863 RVA: 0x001C1B44 File Offset: 0x001BFD44
		public virtual void Draw_Weapon(WeaponHolder holder, WeaponType weaponType, bool isRightHand = true)
		{
			this.ResetRiderCombat();
			this.SetAction(isRightHand ? WeaponActions.DrawFromRight : WeaponActions.DrawFromLeft);
			this.SetWeaponIdleAnimState(weaponType, isRightHand);
			this._weaponType = weaponType;
			this.LinkAnimator();
			if (this.debug)
			{
				Debug.Log("Draw with No Active Weapon");
			}
		}

		// Token: 0x06003A10 RID: 14864 RVA: 0x001C1B81 File Offset: 0x001BFD81
		public virtual void Store_Weapon(WeaponHolder holder, bool isRightHand = true)
		{
			this._weaponType = WeaponType.None;
			this.ActiveHolderSide = holder;
			this.SetAction(isRightHand ? WeaponActions.StoreToRight : WeaponActions.StoreToLeft);
			this.LinkAnimator();
			this.ResetRiderCombat();
			if (this.debug)
			{
				Debug.Log("Store with No Active Weapon");
			}
		}

		// Token: 0x06003A11 RID: 14865 RVA: 0x001C1BC0 File Offset: 0x001BFDC0
		public virtual void SetWeaponIdleAnimState(bool IsRightHand)
		{
			string text = " Idle " + (IsRightHand ? "Right" : "Left") + " Hand";
			switch (this._weaponType)
			{
			case WeaponType.Melee:
				text = "Melee" + text;
				goto IL_88;
			case WeaponType.Bow:
				text = "Bow" + text;
				goto IL_88;
			case WeaponType.Pistol:
				text = "Pistol" + text;
				goto IL_88;
			case WeaponType.Rifle:
				text = "Rifle" + text;
				goto IL_88;
			}
			text = "Melee" + text;
			IL_88:
			this._anim.CrossFade(text, 0.25f, this.Active_IMWeapon.RightHand ? this.Layer_RiderArmRight : this.Layer_RiderArmLeft);
		}

		// Token: 0x06003A12 RID: 14866 RVA: 0x001C1C81 File Offset: 0x001BFE81
		public virtual void SetWeaponIdleAnimState(WeaponType weapon, bool isRightHand)
		{
			this.SetWeaponType(weapon);
			this.SetWeaponIdleAnimState(isRightHand);
		}

		// Token: 0x06003A13 RID: 14867 RVA: 0x001C1C91 File Offset: 0x001BFE91
		public virtual WeaponType GetWeaponType()
		{
			if (this.Active_IMWeapon != null)
			{
				if (this.Active_IMWeapon is IMelee)
				{
					return WeaponType.Melee;
				}
				if (this.Active_IMWeapon is IBow)
				{
					return WeaponType.Bow;
				}
				if (this.Active_IMWeapon is IGun)
				{
					return WeaponType.Pistol;
				}
			}
			return WeaponType.None;
		}

		// Token: 0x06003A14 RID: 14868 RVA: 0x001C1CC9 File Offset: 0x001BFEC9
		public virtual void SetWeaponType(WeaponType weapon)
		{
			this._weaponType = weapon;
			this._anim.SetInteger(RiderCombat.Hash_WeaponType, (int)this._weaponType);
		}

		// Token: 0x06003A15 RID: 14869 RVA: 0x001C1CE8 File Offset: 0x001BFEE8
		public virtual void WeaponSound(int SoundID)
		{
			if (this.Active_IMWeapon != null)
			{
				this.Active_IMWeapon.PlaySound(SoundID);
			}
		}

		// Token: 0x06003A16 RID: 14870 RVA: 0x001C1CFE File Offset: 0x001BFEFE
		public virtual void Action(int value)
		{
			this.SetAction((WeaponActions)value);
		}

		// Token: 0x06003A17 RID: 14871 RVA: 0x001C1D07 File Offset: 0x001BFF07
		public virtual void SetAction(WeaponActions action)
		{
			this._weaponAction = action;
			this._anim.SetInteger(RiderCombat.Hash_WeaponAction, (int)action);
			this.OnWeaponAction.Invoke(this._weaponAction);
		}

		// Token: 0x06003A18 RID: 14872 RVA: 0x001C1D32 File Offset: 0x001BFF32
		public void Attack1(bool v)
		{
			this._anim.SetBool(Hash.Attack1, v);
		}

		// Token: 0x06003A19 RID: 14873 RVA: 0x001C1D45 File Offset: 0x001BFF45
		public void Attack2(bool v)
		{
			this._anim.SetBool(Hash.Attack2, v);
		}

		// Token: 0x06003A1A RID: 14874 RVA: 0x001C1D58 File Offset: 0x001BFF58
		public virtual void EnableMountInput(string input)
		{
			MalbersInput component = this.rider.Montura.GetComponent<MalbersInput>();
			if (component)
			{
				component.EnableInput(input);
			}
		}

		// Token: 0x06003A1B RID: 14875 RVA: 0x001C1D88 File Offset: 0x001BFF88
		public virtual void DisableMountInput(string input)
		{
			MalbersInput component = this.rider.Montura.GetComponent<MalbersInput>();
			if (component)
			{
				component.DisableInput(input);
			}
		}

		// Token: 0x06003A1C RID: 14876 RVA: 0x000023FD File Offset: 0x000005FD
		private void Reset()
		{
		}

		// Token: 0x06003A1D RID: 14877 RVA: 0x001C1DB5 File Offset: 0x001BFFB5
		private void Start()
		{
			this.InitRiderCombat();
		}

		// Token: 0x06003A1E RID: 14878 RVA: 0x001C1DBD File Offset: 0x001BFFBD
		private void Update()
		{
			if (this.LockCombat)
			{
				return;
			}
			this.CombatLogicUpdate();
		}

		// Token: 0x06003A1F RID: 14879 RVA: 0x001C1DCE File Offset: 0x001BFFCE
		private void FixedUpdate()
		{
			if (this.LockCombat)
			{
				return;
			}
			if (this.ActiveAbility)
			{
				this.ActiveAbility.FixedUpdateAbility();
			}
		}

		// Token: 0x06003A20 RID: 14880 RVA: 0x001C1DF1 File Offset: 0x001BFFF1
		private void LateUpdate()
		{
			if (this.LockCombat)
			{
				return;
			}
			if (this.ActiveAbility)
			{
				this.ActiveAbility.LateUpdateAbility();
			}
			if (this.CombatMode)
			{
				this.Anim.SetInteger(Hash.IDInt, -1);
			}
		}

		// Token: 0x06003A21 RID: 14881 RVA: 0x001C1E30 File Offset: 0x001C0030
		protected virtual void InitRiderCombat()
		{
			this._transform = base.transform;
			this._anim = base.GetComponent<Animator>();
			this._Head = this._anim.GetBoneTransform(HumanBodyBones.Head);
			this._Chest = this._anim.GetBoneTransform(HumanBodyBones.Chest);
			this._RightHand = this._anim.GetBoneTransform(HumanBodyBones.RightHand);
			this._LeftHand = this._anim.GetBoneTransform(HumanBodyBones.LeftHand);
			this._RightShoulder = this._anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
			this._LeftShoulder = this._anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
			this.SetActiveHolder(this.ActiveHolderSide);
			this.Layer_RiderArmLeft = this._anim.GetLayerIndex("Rider Arm Left");
			this.Layer_RiderArmRight = this._anim.GetLayerIndex("Rider Arm Right");
			this.Layer_RiderCombat = this._anim.GetLayerIndex("Rider Combat");
			if (this.rider.MainCamera)
			{
				this._cam = this.rider.MainCamera.transform;
			}
			this.aimSent = true;
			if (this.AimDot)
			{
				this.AimDot.gameObject.SetActive(false);
			}
			foreach (RiderCombatAbility riderCombatAbility in this.CombatAbilities)
			{
				riderCombatAbility.StartAbility(this);
			}
			this.rider.OnStartMounting.AddListener(new UnityAction(this.OnStartMounting));
			this.InputAim.InputSystem = (this.InputWeapon.InputSystem = (this.InputAttack1.InputSystem = (this.InputAttack2.InputSystem = (this.HBack.InputSystem = (this.HLeft.InputSystem = (this.HRight.InputSystem = (this.Reload.InputSystem = this.rider.inputSystem)))))));
			this.InputAim.OnInputChanged.AddListener(new UnityAction<bool>(this.GetAimInput));
			this.InputAttack1.OnInputChanged.AddListener(new UnityAction<bool>(this.GetAttack1Input));
			this.InputAttack1.OnInputPressed.AddListener(delegate()
			{
				this.GetAttack1Input(true);
			});
			this.InputAttack2.OnInputChanged.AddListener(new UnityAction<bool>(this.GetAttack2Input));
			this.InputAttack2.OnInputPressed.AddListener(delegate()
			{
				this.GetAttack2Input(true);
			});
			this.Reload.OnInputChanged.AddListener(new UnityAction<bool>(this.GetReloadInput));
		}

		// Token: 0x06003A22 RID: 14882 RVA: 0x001C20EC File Offset: 0x001C02EC
		public virtual void OnStartMounting()
		{
			if (this.rider.AnimalControl)
			{
				this.DefaultMonturaInputCamBaseInput = this.rider.AnimalControl.CameraBaseInput;
			}
			this.rider.Montura.ActiveRiderCombat = this;
		}

		// Token: 0x06003A23 RID: 14883 RVA: 0x001C2128 File Offset: 0x001C0328
		protected virtual void CombatLogicUpdate()
		{
			if (!this.rider.IsRiding)
			{
				return;
			}
			if (!this.IsAiming)
			{
				this.MountStartRotation = this.MountPoint.localRotation;
			}
			this.CalculateCameraTargetSide();
			if (this.UseHolders)
			{
				this.Toogle_Weapon();
			}
			if (this.isInCombatMode)
			{
				this.Anim.speed = 1f;
				this.Anim.SetInteger(Hash.IDInt, -1);
				if (this.ActiveAbility)
				{
					if (this.Active_IMWeapon != null && this.Active_IMWeapon.Active && this.ActiveAbility.WeaponType() != this.GetWeaponType())
					{
						this.ActiveAbility = this.CombatAbilities.Find((RiderCombatAbility ability) => ability.WeaponType() == this.GetWeaponType());
					}
					if (this.ActiveAbility.CanAim())
					{
						this.AimMode();
					}
					bool getInput = this.InputAttack1.GetInput;
					bool getInput2 = this.InputAttack2.GetInput;
					bool getInput3 = this.Reload.GetInput;
					this.ActiveAbility.UpdateAbility();
				}
			}
			if (this.Target != this.lastTarget)
			{
				this.OnTarget.Invoke(this.Target);
				this.lastTarget = this.Target;
				if (this.rider.AnimalControl && this.StrafeOnTarget)
				{
					this.rider.AnimalControl.CameraBaseInput = (this.Target || this.DefaultMonturaInputCamBaseInput);
				}
			}
		}

		// Token: 0x06003A24 RID: 14884 RVA: 0x001C22A4 File Offset: 0x001C04A4
		private void CalculateCameraTargetSide()
		{
			float num = 0f;
			float num2 = 0f;
			if (this.Target)
			{
				num = Vector3.Dot((this._transform.position - this.Target.position).normalized, this._transform.right);
			}
			if (this._cam)
			{
				num2 = Vector3.Dot(this._cam.transform.right, this._transform.forward);
			}
			this.TargetSide = (num > 0f);
			this.CameraSide = (num2 > 0f);
		}

		// Token: 0x06003A25 RID: 14885 RVA: 0x001C2348 File Offset: 0x001C0548
		public virtual void MainAttack()
		{
			if (this.ActiveAbility)
			{
				this.ActiveAbility.PrimaryAttack();
			}
		}

		// Token: 0x06003A26 RID: 14886 RVA: 0x001C2362 File Offset: 0x001C0562
		public virtual void MainAttackReleased()
		{
			if (this.ActiveAbility)
			{
				this.ActiveAbility.PrimaryAttackReleased();
			}
		}

		// Token: 0x06003A27 RID: 14887 RVA: 0x001C237C File Offset: 0x001C057C
		public virtual void SecondAttack()
		{
			if (this.ActiveAbility)
			{
				this.ActiveAbility.SecondaryAttack();
			}
		}

		// Token: 0x06003A28 RID: 14888 RVA: 0x001C2396 File Offset: 0x001C0596
		public virtual void SecondAttackReleased()
		{
			if (this.ActiveAbility)
			{
				this.ActiveAbility.SecondaryAttackReleased();
			}
		}

		// Token: 0x06003A29 RID: 14889 RVA: 0x001C23B0 File Offset: 0x001C05B0
		public virtual void ReloadWeapon()
		{
			if (this.ActiveAbility)
			{
				this.ActiveAbility.ReloadWeapon();
			}
		}

		// Token: 0x06003A2A RID: 14890 RVA: 0x001C23CC File Offset: 0x001C05CC
		public virtual void LinkAnimator()
		{
			this._anim.SetInteger(RiderCombat.Hash_WeaponHolder, (int)this.ActiveHolderSide);
			this._anim.SetInteger(RiderCombat.Hash_WeaponType, (int)this._weaponType);
			this._anim.SetInteger(RiderCombat.Hash_WeaponAction, (int)this._weaponAction);
		}

		// Token: 0x06003A2B RID: 14891 RVA: 0x001C241C File Offset: 0x001C061C
		protected void GetAimInput(bool inputValue)
		{
			if (this.InputAim.GetPressed != InputButton.Press)
			{
				if (inputValue)
				{
					this.IsAiming = !this.IsAiming;
					this.aimSent = false;
					this.CurrentCameraSide = !this.CameraSide;
					return;
				}
			}
			else if (this.InputAim.GetPressed == InputButton.Press && this.IsAiming != inputValue)
			{
				this.IsAiming = inputValue;
				this.aimSent = false;
			}
		}

		// Token: 0x06003A2C RID: 14892 RVA: 0x001C2483 File Offset: 0x001C0683
		protected void GetAttack1Input(bool inputValue)
		{
			if (inputValue)
			{
				this.MainAttack();
				return;
			}
			this.MainAttackReleased();
		}

		// Token: 0x06003A2D RID: 14893 RVA: 0x001C2495 File Offset: 0x001C0695
		protected void GetAttack2Input(bool inputValue)
		{
			if (inputValue)
			{
				this.SecondAttack();
				return;
			}
			this.SecondAttackReleased();
		}

		// Token: 0x06003A2E RID: 14894 RVA: 0x001C24A7 File Offset: 0x001C06A7
		protected void GetReloadInput(bool inputValue)
		{
			if (inputValue)
			{
				this.ReloadWeapon();
			}
		}

		// Token: 0x06003A2F RID: 14895 RVA: 0x001C24B4 File Offset: 0x001C06B4
		public virtual void AimMode()
		{
			this.LookDirection();
			bool getInput = this.InputAim.GetInput;
			if (this.IsAiming)
			{
				if (this.AimDot)
				{
					this.AimDot.gameObject.SetActive(true);
				}
				this.SetAimDirection();
				if (this.CurrentCameraSide != this.CameraSide)
				{
					this.aimSent = false;
				}
				if (this.CameraSide && !this.aimSent)
				{
					if (this.ActiveAbility.ChangeAimCameraSide() || !this.Active_IMWeapon.RightHand)
					{
						this.SetAim(true);
					}
					else
					{
						this.SetAim(false);
					}
				}
				else if (!this.CameraSide && !this.aimSent)
				{
					if (this.ActiveAbility.ChangeAimCameraSide() || this.Active_IMWeapon.RightHand)
					{
						this.SetAim(false);
					}
					else
					{
						this.SetAim(true);
					}
				}
				if (this.rider.Montura != null)
				{
					this.rider.Montura.StraightAim(true);
					return;
				}
			}
			else if (!this.aimSent)
			{
				this.aimSent = true;
				this.CurrentCameraSide = !this.CameraSide;
				this.SetAction(WeaponActions.Idle);
				this.OnAimSide.Invoke(0);
				if (this.AimDot)
				{
					this.AimDot.gameObject.SetActive(false);
				}
				this.rider.Montura.StraightAim(false);
				if (this._weaponAction == WeaponActions.AimLeft || this._weaponAction == WeaponActions.AimRight || this.WeaponAction == WeaponActions.Hold || this.WeaponAction == WeaponActions.Fire_Proyectile)
				{
					this.SetAction(this.CombatMode ? WeaponActions.Idle : WeaponActions.None);
				}
			}
		}

		// Token: 0x06003A30 RID: 14896 RVA: 0x001C2654 File Offset: 0x001C0854
		public virtual void SetAimDirection()
		{
			Transform transform = this.ActiveAbility.AimRayOrigin();
			if (this.Target)
			{
				this.aimDirection = MalbersTools.DirectionTarget(transform, this.Target, true);
			}
			else
			{
				Vector3 screenPoint = (this.AimDot != null) ? this.AimDot.position : new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
				this.aimDirection = MalbersTools.DirectionFromCamera(transform, screenPoint, out this.aimRayHit, this.HitMask);
			}
			if (this.debug)
			{
				Debug.DrawLine(transform.position, this.AimRayCastHit.point, Color.red);
			}
		}

		// Token: 0x06003A31 RID: 14897 RVA: 0x001C2708 File Offset: 0x001C0908
		protected void SetAim(bool Side)
		{
			this.aimSent = true;
			this.CurrentCameraSide = this.CameraSide;
			if (this._weaponAction != WeaponActions.Hold && this._weaponAction != WeaponActions.ReloadLeft && this._weaponAction != WeaponActions.ReloadRight)
			{
				this.SetAction(this.Active_IMWeapon.RightHand ? WeaponActions.AimRight : WeaponActions.AimLeft);
			}
			this.OnAimSide.Invoke(Side ? 1 : -1);
		}

		// Token: 0x06003A32 RID: 14898 RVA: 0x001C2774 File Offset: 0x001C0974
		protected virtual void LookDirection()
		{
			Vector3 from = this.Target ? this.aimDirection : (this.AimDot ? MalbersTools.DirectionFromCameraNoRayCast(this.AimDot.position) : Camera.main.transform.forward);
			from.y = 0f;
			float b = Vector3.Angle(from, base.transform.root.forward) * (float)((this.Target ? this.TargetSide : this.CameraSide) ? 1 : -1) / 180f;
			this.horizontalAngle = Mathf.Lerp(this.HorizontalAngle, b, Time.deltaTime * 15f);
			this._anim.SetFloat(RiderCombat.Hash_AimSide, this.HorizontalAngle);
		}

		// Token: 0x06003A33 RID: 14899 RVA: 0x001C2844 File Offset: 0x001C0A44
		public virtual void SetActiveHolder(WeaponHolder holder)
		{
			this.ActiveHolderSide = holder;
			switch (this.ActiveHolderSide)
			{
			case WeaponHolder.None:
				this.ActiveHolderTransform = this.HolderBack;
				return;
			case WeaponHolder.Left:
				this.ActiveHolderTransform = (this.HolderLeft ? this.HolderLeft : this.HolderBack);
				return;
			case WeaponHolder.Right:
				this.ActiveHolderTransform = (this.HolderRight ? this.HolderRight : this.HolderBack);
				return;
			case WeaponHolder.Back:
				this.ActiveHolderTransform = this.HolderBack;
				return;
			default:
				return;
			}
		}

		// Token: 0x06003A34 RID: 14900 RVA: 0x001C28D4 File Offset: 0x001C0AD4
		public virtual void SetWeaponBeforeMounting(GameObject weapon)
		{
			if (weapon == null)
			{
				return;
			}
			if (weapon.GetComponent<IMWeapon>() == null)
			{
				return;
			}
			this.SetActiveWeapon(weapon);
			this.CombatMode = true;
			this.Active_IMWeapon.Owner = this.rider;
			this.Active_IMWeapon.IsEquiped = true;
			this.Active_IMWeapon.HitMask = this.HitMask;
			this.SetActiveHolder(this.Active_IMWeapon.Holder);
			this._weaponType = this.GetWeaponType();
			this.SetAction(WeaponActions.Idle);
			this.SetWeaponIdleAnimState(this.Active_IMWeapon.RightHand);
			this.ActiveAbility = this.CombatAbilities.Find((RiderCombatAbility ability) => ability.WeaponType() == this.GetWeaponType());
			if (this.ActiveAbility)
			{
				this.ActiveAbility.ActivateAbility();
			}
			else
			{
				Debug.LogWarning("The Weapon is combatible but there's no Combat Ability available for it, please Add the matching ability it on the list of Combat Abilities");
			}
			this.OnEquipWeapon.Invoke(this.ActiveWeapon);
			this.LinkAnimator();
		}

		// Token: 0x06003A35 RID: 14901 RVA: 0x001C29C0 File Offset: 0x001C0BC0
		protected virtual void Toogle_Weapon()
		{
			if (this._weaponAction == WeaponActions.None || this._weaponAction == WeaponActions.Idle || this._weaponAction == WeaponActions.AimLeft || this._weaponAction == WeaponActions.AimRight)
			{
				if (this.InputWeapon.GetInput)
				{
					this.ToggleActiveHolderWeapon();
				}
				if (this.UseHolders)
				{
					if (this.HBack.GetInput)
					{
						this.Change_Weapon_Holder_Inputs(WeaponHolder.Back);
						if (this.debug)
						{
							Debug.Log("Change Holder to 'Back'. ");
						}
					}
					if (this.HLeft.GetInput)
					{
						this.Change_Weapon_Holder_Inputs(WeaponHolder.Left);
						if (this.debug)
						{
							Debug.Log("Change Holder to 'Left'. ");
						}
					}
					if (this.HRight.GetInput)
					{
						this.Change_Weapon_Holder_Inputs(WeaponHolder.Right);
						if (this.debug)
						{
							Debug.Log("Change Holder to 'Right'. ");
						}
					}
				}
			}
		}

		// Token: 0x06003A36 RID: 14902 RVA: 0x001C2A83 File Offset: 0x001C0C83
		public virtual void ToggleActiveHolderWeapon()
		{
			if (this.ActiveWeapon)
			{
				if (this._weaponAction == WeaponActions.Idle)
				{
					this.Store_Weapon();
					return;
				}
			}
			else
			{
				this.Draw_Weapon();
			}
		}

		// Token: 0x06003A37 RID: 14903 RVA: 0x001C2AAC File Offset: 0x001C0CAC
		public virtual void Change_Weapon_Holder_Inputs(WeaponHolder holder)
		{
			if (this.ActiveHolderSide != holder && this._weaponAction == WeaponActions.Idle)
			{
				base.StartCoroutine(this.SwapWeaponsHolder(holder));
				return;
			}
			if (this.ActiveHolderSide != holder && this._weaponAction == WeaponActions.None)
			{
				this.SetActiveHolder(holder);
				this.Draw_Weapon();
				this.LinkAnimator();
				return;
			}
			if (!this.isInCombatMode)
			{
				if (this._weaponAction == WeaponActions.None)
				{
					this.Draw_Weapon();
				}
			}
			else if (this._weaponAction == WeaponActions.Idle)
			{
				this.Store_Weapon();
			}
			this.LinkAnimator();
		}

		// Token: 0x06003A38 RID: 14904 RVA: 0x001C2B2E File Offset: 0x001C0D2E
		private IEnumerator SwapWeaponsHolder(WeaponHolder HoldertoSwap)
		{
			this.Store_Weapon();
			this.LinkAnimator();
			while (this._weaponAction == WeaponActions.StoreToLeft || this._weaponAction == WeaponActions.StoreToRight)
			{
				yield return null;
			}
			this.SetActiveHolder(HoldertoSwap);
			this.Draw_Weapon();
			yield break;
		}

		// Token: 0x06003A39 RID: 14905 RVA: 0x001C2B44 File Offset: 0x001C0D44
		public virtual void SetWeaponByInventory(GameObject Next_Weapon)
		{
			base.StopAllCoroutines();
			if (!this.rider.IsRiding)
			{
				return;
			}
			if (Next_Weapon == null)
			{
				if (this.ActiveWeapon)
				{
					this.Store_Weapon();
				}
				return;
			}
			IMWeapon component = Next_Weapon.GetComponent<IMWeapon>();
			if (component == null)
			{
				if (this.ActiveWeapon)
				{
					this.Store_Weapon();
				}
				return;
			}
			if (this.Active_IMWeapon == null)
			{
				if (!this.AlreadyInstantiated)
				{
					Next_Weapon = Object.Instantiate<GameObject>(Next_Weapon, this.rider.transform);
					Next_Weapon.SetActive(false);
				}
				this.SetActiveWeapon(Next_Weapon);
				this.Draw_Weapon();
				return;
			}
			if (!this.Active_IMWeapon.Equals(component))
			{
				base.StartCoroutine(this.SwapWeaponsInventory(Next_Weapon));
				return;
			}
			if (!this.CombatMode)
			{
				this.Draw_Weapon();
				return;
			}
			this.Store_Weapon();
		}

		// Token: 0x06003A3A RID: 14906 RVA: 0x001C2C0A File Offset: 0x001C0E0A
		private IEnumerator SwapWeaponsInventory(GameObject nextWeapon)
		{
			this.Store_Weapon();
			while (this._weaponAction == WeaponActions.StoreToLeft || this._weaponAction == WeaponActions.StoreToRight)
			{
				yield return null;
			}
			if (!this.AlreadyInstantiated)
			{
				nextWeapon = Object.Instantiate<GameObject>(nextWeapon, this.rider.transform);
				nextWeapon.SetActive(false);
			}
			this.SetActiveWeapon(nextWeapon);
			this.SetActiveHolder(this.Active_IMWeapon.Holder);
			this.Draw_Weapon();
			yield break;
		}

		// Token: 0x06003A3B RID: 14907 RVA: 0x001C2C20 File Offset: 0x001C0E20
		public virtual void Equip_Weapon()
		{
			this.SetAction(WeaponActions.Equip);
			this.isInCombatMode = true;
			if (this.Active_IMWeapon == null)
			{
				return;
			}
			if (this.debug)
			{
				Debug.Log("Equip_Weapon");
			}
			this.Active_IMWeapon.HitMask = this.HitMask;
			if (this.UseHolders)
			{
				if (this.ActiveHolderTransform.transform.childCount > 0)
				{
					this.SetActiveWeapon(this.ActiveHolderTransform.GetChild(0).gameObject);
					this.ActiveWeapon.transform.parent = (this.Active_IMWeapon.RightHand ? this.RightHandEquipPoint : this.LeftHandEquipPoint);
					this.Active_IMWeapon.Holder = this.ActiveHolderSide;
					base.StartCoroutine(this.SmoothWeaponTransition(this.ActiveWeapon.transform, this.Active_IMWeapon.PositionOffset, this.Active_IMWeapon.RotationOffset, 0.3f));
				}
			}
			else if (this.UseInventory)
			{
				if (!this.AlreadyInstantiated)
				{
					this.ActiveWeapon.transform.parent = (this.Active_IMWeapon.RightHand ? this.RightHandEquipPoint : this.LeftHandEquipPoint);
					this.ActiveWeapon.transform.localPosition = this.Active_IMWeapon.PositionOffset;
					this.ActiveWeapon.transform.localEulerAngles = this.Active_IMWeapon.RotationOffset;
				}
				this.ActiveWeapon.gameObject.SetActive(true);
			}
			this.Active_IMWeapon.Owner = this.rider;
			this.Active_IMWeapon.IsEquiped = true;
			this.OnEquipWeapon.Invoke(this.ActiveWeapon);
			if (this.ActiveAbility)
			{
				this.ActiveAbility.ActivateAbility();
			}
		}

		// Token: 0x06003A3C RID: 14908 RVA: 0x001C2DDC File Offset: 0x001C0FDC
		public virtual void Unequip_Weapon()
		{
			this._weaponType = WeaponType.None;
			this.SetAction(WeaponActions.Unequip);
			this.LinkAnimator();
			if (this.Active_IMWeapon == null)
			{
				return;
			}
			if (this.debug)
			{
				Debug.Log("Unequip_Weapon");
			}
			this.Active_IMWeapon.IsEquiped = false;
			this.OnUnequipWeapon.Invoke(this.ActiveWeapon);
			if (this.UseHolders)
			{
				this.ActiveWeapon.transform.parent = this.ActiveHolderTransform.transform;
				base.StartCoroutine(this.SmoothWeaponTransition(this.ActiveWeapon.transform, Vector3.zero, Vector3.zero, 0.3f));
			}
			else if (this.UseInventory && !this.AlreadyInstantiated && this.ActiveWeapon)
			{
				Object.Destroy(this.ActiveWeapon);
			}
			this.activeWeapon = null;
		}

		// Token: 0x06003A3D RID: 14909 RVA: 0x001C2EB4 File Offset: 0x001C10B4
		public virtual void Draw_Weapon()
		{
			this.ResetRiderCombat();
			if (this.UseInventory)
			{
				if (this.Active_IMWeapon != null)
				{
					this.SetActiveHolder(this.Active_IMWeapon.Holder);
				}
			}
			else
			{
				if (this.ActiveHolderTransform.childCount == 0)
				{
					return;
				}
				if (this.ActiveHolderTransform.GetChild(0).GetComponent<IMWeapon>() == null)
				{
					return;
				}
				this.SetActiveWeapon(this.ActiveHolderTransform.GetChild(0).gameObject);
			}
			this._weaponType = this.GetWeaponType();
			this.SetAction(this.Active_IMWeapon.RightHand ? WeaponActions.DrawFromRight : WeaponActions.DrawFromLeft);
			this.SetWeaponIdleAnimState(this.Active_IMWeapon.RightHand);
			this.ActiveAbility = this.CombatAbilities.Find((RiderCombatAbility ability) => ability.TypeOfAbility(this.Active_IMWeapon));
			this.LinkAnimator();
			if (this.debug)
			{
				Debug.Log("Draw: " + this.ActiveWeapon.name);
			}
		}

		// Token: 0x06003A3E RID: 14910 RVA: 0x001C2FA0 File Offset: 0x001C11A0
		public virtual void Store_Weapon()
		{
			if (this.Active_IMWeapon == null || !this.isInCombatMode)
			{
				return;
			}
			this.ResetRiderCombat();
			this._weaponType = WeaponType.None;
			this.SetActiveHolder(this.Active_IMWeapon.Holder);
			this.SetAction(this.Active_IMWeapon.RightHand ? WeaponActions.StoreToRight : WeaponActions.StoreToLeft);
			this.LinkAnimator();
			if (this.debug)
			{
				Debug.Log("Store: " + this.ActiveWeapon.name);
			}
		}

		// Token: 0x06003A3F RID: 14911 RVA: 0x001C301D File Offset: 0x001C121D
		protected virtual void ResetActiveAbility()
		{
			if (this.ActiveAbility != null)
			{
				this.ActiveAbility.ResetAbility();
				this.ActiveAbility = null;
			}
		}

		// Token: 0x06003A40 RID: 14912 RVA: 0x001C303F File Offset: 0x001C123F
		private IEnumerator SmoothWeaponTransition(Transform obj, Vector3 posOfsset, Vector3 rotOffset, float time)
		{
			float elapsedtime = 0f;
			Vector3 startPos = obj.localPosition;
			Quaternion startRot = obj.localRotation;
			while (elapsedtime < time)
			{
				obj.localPosition = Vector3.Slerp(startPos, posOfsset, Mathf.SmoothStep(0f, 1f, elapsedtime / time));
				obj.localRotation = Quaternion.Slerp(startRot, Quaternion.Euler(rotOffset), elapsedtime / time);
				elapsedtime += Time.deltaTime;
				yield return null;
			}
			obj.localPosition = posOfsset;
			obj.localEulerAngles = rotOffset;
			yield break;
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06003A41 RID: 14913 RVA: 0x001C3064 File Offset: 0x001C1264
		// (set) Token: 0x06003A42 RID: 14914 RVA: 0x001C306C File Offset: 0x001C126C
		public bool LockCombat
		{
			get
			{
				return this.lockCombat;
			}
			set
			{
				this.lockCombat = value;
			}
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06003A43 RID: 14915 RVA: 0x001C3075 File Offset: 0x001C1275
		public Rider3rdPerson rider
		{
			get
			{
				if (this._rider == null)
				{
					this._rider = base.GetComponent<Rider3rdPerson>();
				}
				return this._rider;
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06003A44 RID: 14916 RVA: 0x001C3097 File Offset: 0x001C1297
		// (set) Token: 0x06003A45 RID: 14917 RVA: 0x001C309F File Offset: 0x001C129F
		public bool CombatMode
		{
			get
			{
				return this.isInCombatMode;
			}
			set
			{
				this.isInCombatMode = value;
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06003A46 RID: 14918 RVA: 0x001C3097 File Offset: 0x001C1297
		// (set) Token: 0x06003A47 RID: 14919 RVA: 0x001C309F File Offset: 0x001C129F
		public bool IsInCombatMode
		{
			get
			{
				return this.isInCombatMode;
			}
			set
			{
				this.isInCombatMode = value;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06003A48 RID: 14920 RVA: 0x001C30A8 File Offset: 0x001C12A8
		// (set) Token: 0x06003A49 RID: 14921 RVA: 0x001C30B0 File Offset: 0x001C12B0
		public WeaponActions WeaponAction
		{
			get
			{
				return this._weaponAction;
			}
			set
			{
				this._weaponAction = value;
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06003A4A RID: 14922 RVA: 0x001C30B9 File Offset: 0x001C12B9
		public IMWeapon Active_IMWeapon
		{
			get
			{
				if (this.ActiveWeapon)
				{
					return this.ActiveWeapon.GetComponent<IMWeapon>();
				}
				return null;
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06003A4B RID: 14923 RVA: 0x001C30D5 File Offset: 0x001C12D5
		public Transform MountPoint
		{
			get
			{
				if (this.mountPoint == null)
				{
					this.mountPoint = this.rider.Montura.MountPoint;
				}
				return this.mountPoint;
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06003A4C RID: 14924 RVA: 0x001C3101 File Offset: 0x001C1301
		public Animator Anim
		{
			get
			{
				return this._anim;
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06003A4E RID: 14926 RVA: 0x001C312D File Offset: 0x001C132D
		// (set) Token: 0x06003A4D RID: 14925 RVA: 0x001C3109 File Offset: 0x001C1309
		public bool IsAiming
		{
			get
			{
				return this.isAiming;
			}
			set
			{
				this.isAiming = value;
				if (!this.isAiming)
				{
					this.SetAction(this.CombatMode ? WeaponActions.Idle : WeaponActions.None);
				}
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06003A4F RID: 14927 RVA: 0x001C3135 File Offset: 0x001C1335
		public bool IsCamRightSide
		{
			get
			{
				return this.CameraSide;
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06003A50 RID: 14928 RVA: 0x001C313D File Offset: 0x001C133D
		public float HorizontalAngle
		{
			get
			{
				return this.horizontalAngle;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06003A51 RID: 14929 RVA: 0x001C3145 File Offset: 0x001C1345
		public Transform RightShoulder
		{
			get
			{
				return this._RightShoulder;
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06003A52 RID: 14930 RVA: 0x001C314D File Offset: 0x001C134D
		public Transform LeftShoulder
		{
			get
			{
				return this._LeftShoulder;
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06003A53 RID: 14931 RVA: 0x001C3155 File Offset: 0x001C1355
		public Transform RightHand
		{
			get
			{
				return this._RightHand;
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06003A54 RID: 14932 RVA: 0x001C315D File Offset: 0x001C135D
		public Transform LeftHand
		{
			get
			{
				return this._LeftHand;
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06003A55 RID: 14933 RVA: 0x001C3165 File Offset: 0x001C1365
		public Transform Head
		{
			get
			{
				return this._Head;
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06003A56 RID: 14934 RVA: 0x001C316D File Offset: 0x001C136D
		public Transform Chest
		{
			get
			{
				return this._Chest;
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06003A57 RID: 14935 RVA: 0x001C3175 File Offset: 0x001C1375
		public Vector3 AimDirection
		{
			get
			{
				return this.aimDirection;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06003A58 RID: 14936 RVA: 0x001C317D File Offset: 0x001C137D
		public RaycastHit AimRayCastHit
		{
			get
			{
				return this.aimRayHit;
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06003A59 RID: 14937 RVA: 0x001C3185 File Offset: 0x001C1385
		public GameObject ActiveWeapon
		{
			get
			{
				return this.activeWeapon;
			}
		}

		// Token: 0x04002A02 RID: 10754
		public LayerMask HitMask;

		// Token: 0x04002A03 RID: 10755
		public InputRow InputWeapon = new InputRow(KeyCode.E);

		// Token: 0x04002A04 RID: 10756
		public InputRow InputAttack1 = new InputRow("Fire1", KeyCode.Mouse0, InputButton.Press);

		// Token: 0x04002A05 RID: 10757
		public InputRow InputAttack2 = new InputRow("Fire2", KeyCode.Mouse1, InputButton.Press);

		// Token: 0x04002A06 RID: 10758
		public InputRow InputAim = new InputRow("Fire2", KeyCode.Mouse1, InputButton.Press);

		// Token: 0x04002A07 RID: 10759
		public bool UseInventory = true;

		// Token: 0x04002A08 RID: 10760
		public bool AlreadyInstantiated = true;

		// Token: 0x04002A09 RID: 10761
		public bool UseHolders;

		// Token: 0x04002A0A RID: 10762
		public bool StrafeOnTarget;

		// Token: 0x04002A0B RID: 10763
		public WeaponHolder ActiveHolderSide = WeaponHolder.Back;

		// Token: 0x04002A0C RID: 10764
		public Transform LeftHandEquipPoint;

		// Token: 0x04002A0D RID: 10765
		public Transform RightHandEquipPoint;

		// Token: 0x04002A0E RID: 10766
		public Transform HolderLeft;

		// Token: 0x04002A0F RID: 10767
		public Transform HolderRight;

		// Token: 0x04002A10 RID: 10768
		public Transform HolderBack;

		// Token: 0x04002A11 RID: 10769
		public Transform ActiveHolderTransform;

		// Token: 0x04002A12 RID: 10770
		public InputRow HBack = new InputRow(KeyCode.Alpha4);

		// Token: 0x04002A13 RID: 10771
		public InputRow HLeft = new InputRow(KeyCode.Alpha5);

		// Token: 0x04002A14 RID: 10772
		public InputRow HRight = new InputRow(KeyCode.Alpha6);

		// Token: 0x04002A15 RID: 10773
		public InputRow Reload = new InputRow("Reload", "Reload", KeyCode.R, InputButton.Down, InputType.Key);

		// Token: 0x04002A16 RID: 10774
		public List<RiderCombatAbility> CombatAbilities;

		// Token: 0x04002A17 RID: 10775
		public bool debug;

		// Token: 0x04002A18 RID: 10776
		public RiderCombatAbility ActiveAbility;

		// Token: 0x04002A19 RID: 10777
		protected Rider3rdPerson _rider;

		// Token: 0x04002A1A RID: 10778
		protected Animator _anim;

		// Token: 0x04002A1B RID: 10779
		protected GameObject activeWeapon;

		// Token: 0x04002A1C RID: 10780
		protected WeaponType _weaponType;

		// Token: 0x04002A1D RID: 10781
		protected WeaponActions _weaponAction;

		// Token: 0x04002A1E RID: 10782
		protected bool lockCombat;

		// Token: 0x04002A1F RID: 10783
		protected bool isInCombatMode;

		// Token: 0x04002A20 RID: 10784
		protected bool aimSent;

		// Token: 0x04002A21 RID: 10785
		protected bool TargetSide;

		// Token: 0x04002A22 RID: 10786
		protected bool CameraSide;

		// Token: 0x04002A23 RID: 10787
		protected bool CurrentCameraSide;

		// Token: 0x04002A24 RID: 10788
		protected int Layer_RiderArmRight;

		// Token: 0x04002A25 RID: 10789
		protected int Layer_RiderArmLeft;

		// Token: 0x04002A26 RID: 10790
		protected int Layer_RiderCombat;

		// Token: 0x04002A27 RID: 10791
		protected Transform _cam;

		// Token: 0x04002A28 RID: 10792
		public RectTransform AimDot;

		// Token: 0x04002A29 RID: 10793
		public Transform Target;

		// Token: 0x04002A2A RID: 10794
		internal Transform lastTarget;

		// Token: 0x04002A2B RID: 10795
		private float horizontalAngle;

		// Token: 0x04002A2C RID: 10796
		protected Vector3 aimDirection;

		// Token: 0x04002A2D RID: 10797
		protected RaycastHit aimRayHit;

		// Token: 0x04002A2E RID: 10798
		protected Quaternion MountStartRotation;

		// Token: 0x04002A2F RID: 10799
		public static readonly int Hash_AimSide = Animator.StringToHash("WeaponAim");

		// Token: 0x04002A30 RID: 10800
		public static readonly int Hash_WeaponType = Animator.StringToHash("WeaponType");

		// Token: 0x04002A31 RID: 10801
		public static readonly int Hash_WeaponHolder = Animator.StringToHash("WeaponHolder");

		// Token: 0x04002A32 RID: 10802
		public static readonly int Hash_WeaponAction = Animator.StringToHash("WeaponAction");

		// Token: 0x04002A33 RID: 10803
		public GameObjectEvent OnEquipWeapon = new GameObjectEvent();

		// Token: 0x04002A34 RID: 10804
		public GameObjectEvent OnUnequipWeapon = new GameObjectEvent();

		// Token: 0x04002A35 RID: 10805
		public WeaponActionEvent OnWeaponAction = new WeaponActionEvent();

		// Token: 0x04002A36 RID: 10806
		public WeaponEvent OnAttack = new WeaponEvent();

		// Token: 0x04002A37 RID: 10807
		public IntEvent OnAimSide = new IntEvent();

		// Token: 0x04002A38 RID: 10808
		public TransformEvent OnTarget = new TransformEvent();

		// Token: 0x04002A39 RID: 10809
		protected Transform _RightShoulder;

		// Token: 0x04002A3A RID: 10810
		protected Transform _LeftShoulder;

		// Token: 0x04002A3B RID: 10811
		protected Transform _RightHand;

		// Token: 0x04002A3C RID: 10812
		protected Transform _LeftHand;

		// Token: 0x04002A3D RID: 10813
		protected Transform _Head;

		// Token: 0x04002A3E RID: 10814
		protected Transform _Chest;

		// Token: 0x04002A3F RID: 10815
		protected Transform _transform;

		// Token: 0x04002A40 RID: 10816
		protected Transform mountPoint;

		// Token: 0x04002A41 RID: 10817
		private bool DefaultMonturaInputCamBaseInput;

		// Token: 0x04002A42 RID: 10818
		private bool isAiming;

		// Token: 0x04002A43 RID: 10819
		[HideInInspector]
		public bool Editor_ShowHolders;

		// Token: 0x04002A44 RID: 10820
		[HideInInspector]
		public bool Editor_ShowInputs;

		// Token: 0x04002A45 RID: 10821
		[HideInInspector]
		public bool Editor_ShowHoldersInput;

		// Token: 0x04002A46 RID: 10822
		[HideInInspector]
		public bool Editor_ShowAdvanced;

		// Token: 0x04002A47 RID: 10823
		[HideInInspector]
		public bool Editor_ShowEvents;

		// Token: 0x04002A48 RID: 10824
		[HideInInspector]
		public bool Editor_ShowEquipPoints;

		// Token: 0x04002A49 RID: 10825
		[HideInInspector]
		public bool Editor_ShowAbilities = true;
	}
}
