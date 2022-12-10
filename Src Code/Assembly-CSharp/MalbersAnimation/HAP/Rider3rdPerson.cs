using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000437 RID: 1079
	public class Rider3rdPerson : Rider
	{
		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x060039EF RID: 14831 RVA: 0x001C072E File Offset: 0x001BE92E
		// (set) Token: 0x060039F0 RID: 14832 RVA: 0x001C0D7A File Offset: 0x001BEF7A
		public override Mountable Montura
		{
			get
			{
				return this.montura;
			}
			set
			{
				this.montura = value;
				this.MountLayerIndex = ((value != null) ? this.Anim.GetLayerIndex(this.Montura.MountLayer) : -1);
			}
		}

		// Token: 0x060039F1 RID: 14833 RVA: 0x001C0DAB File Offset: 0x001BEFAB
		private void Reset()
		{
			this.DismountInput.GetPressed = InputButton.LongPress;
		}

		// Token: 0x060039F2 RID: 14834 RVA: 0x001C0DBC File Offset: 0x001BEFBC
		private void Awake()
		{
			this._transform = base.transform;
			if (!this.Anim)
			{
				this.Anim = base.GetComponentInChildren<Animator>();
			}
			this._rigidbody = base.GetComponent<Rigidbody>();
			this._collider = base.GetComponents<Collider>();
			this.MountLayerIndex = -1;
		}

		// Token: 0x060039F3 RID: 14835 RVA: 0x001C0E10 File Offset: 0x001BF010
		private void Start()
		{
			this.inputSystem = DefaultInput.GetInputSystem(this.PlayerID);
			this.MountInput.InputSystem = (this.DismountInput.InputSystem = (this.CallAnimalInput.InputSystem = this.inputSystem));
			base.IsOnHorse = (base.Mounted = false);
			if (this.Anim)
			{
				this.Initial_UpdateMode = this.Anim.updateMode;
			}
			if (this.StartMounted)
			{
				this.AlreadyMounted();
			}
		}

		// Token: 0x060039F4 RID: 14836 RVA: 0x001C0E97 File Offset: 0x001BF097
		public void AlreadyMounted()
		{
			base.StartCoroutine(this.AlreadyMountedC());
		}

		// Token: 0x060039F5 RID: 14837 RVA: 0x001C0EA6 File Offset: 0x001BF0A6
		private IEnumerator AlreadyMountedC()
		{
			yield return null;
			if (this.AnimalStored != null && this.StartMounted)
			{
				this.Montura = this.AnimalStored;
				this.Montura.ActiveRider = this;
				if (base.MountTrigger == null)
				{
					this.Montura.transform.GetComponentInChildren<MountTriggers>();
				}
				this.Start_Mounting();
				this.End_Mounting();
				if (this.Anim)
				{
					this.Anim.Play(this.Montura.MountIdle, this.MountLayerIndex);
				}
				this.toogleCall = true;
				this.Montura.Mounted = (base.Mounted = true);
				this.Anim.SetBool(Hash.Mount, this.mounted);
			}
			this.Montura.Animal.OnSyncAnimator.AddListener(new UnityAction(this.SyncAnimator));
			this.OnAlreadyMounted.Invoke();
			yield break;
		}

		// Token: 0x060039F6 RID: 14838 RVA: 0x001C0EB8 File Offset: 0x001BF0B8
		public override void Start_Mounting()
		{
			if (this.Anim)
			{
				this.Anim.SetLayerWeight(this.MountLayerIndex, 1f);
				this.Anim.SetBool(Hash.Mount, base.Mounted);
				Vector3 position = base.transform.position;
				position.y = this.Anim.GetBoneTransform(HumanBodyBones.Hips).position.y;
				base.transform.position = position;
			}
			if (!base.MountTrigger)
			{
				base.MountTrigger = this.Montura.GetComponentInChildren<MountTriggers>();
			}
			if (this.DisableComponents)
			{
				this.ToggleComponents(false);
			}
			base.Start_Mounting();
			this.OnStartMounting.Invoke();
			if (!this.Anim)
			{
				this.End_Dismounting();
			}
		}

		// Token: 0x060039F7 RID: 14839 RVA: 0x001C0F84 File Offset: 0x001BF184
		public override void End_Mounting()
		{
			base.End_Mounting();
			if (this.Anim)
			{
				this.Anim.updateMode = AnimatorUpdateMode.Normal;
			}
			this.OnEndMounting.Invoke();
		}

		// Token: 0x060039F8 RID: 14840 RVA: 0x001C0FB0 File Offset: 0x001BF1B0
		public override void Start_Dismounting()
		{
			base.Start_Dismounting();
			if (this.Anim)
			{
				this.Anim.updateMode = this.Initial_UpdateMode;
			}
			this.OnStartDismounting.Invoke();
		}

		// Token: 0x060039F9 RID: 14841 RVA: 0x001C0FE4 File Offset: 0x001BF1E4
		public override void End_Dismounting()
		{
			if (this.Anim && this.MountLayerIndex != -1)
			{
				this.Anim.SetLayerWeight(this.MountLayerIndex, 0f);
			}
			base.End_Dismounting();
			if (this.DisableComponents)
			{
				this.ToggleComponents(true);
			}
			this.OnEndDismounting.Invoke();
		}

		// Token: 0x060039FA RID: 14842 RVA: 0x001C1040 File Offset: 0x001BF240
		protected virtual void Animators_ReSync()
		{
			if (!this.Anim)
			{
				return;
			}
			if (!this.Montura.syncAnimators)
			{
				return;
			}
			if (this.Montura.Animal.AnimState == AnimTag.Locomotion)
			{
				this.RiderNormalizedTime = this.Anim.GetCurrentAnimatorStateInfo(this.MountLayerIndex).normalizedTime;
				this.HorseNormalizedTime = this.Montura.Animal.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
				this.syncronize = true;
				if (Mathf.Abs(this.RiderNormalizedTime - this.HorseNormalizedTime) > 0.1f && Time.time - this.LastSyncTime > 1f)
				{
					this.Anim.CrossFade(AnimTag.Locomotion, 0.2f, this.MountLayerIndex, this.HorseNormalizedTime);
					this.LastSyncTime = Time.time;
					return;
				}
			}
			else
			{
				this.syncronize = false;
				this.RiderNormalizedTime = (this.HorseNormalizedTime = 0f);
			}
		}

		// Token: 0x060039FB RID: 14843 RVA: 0x001C1141 File Offset: 0x001BF341
		private void Update()
		{
			this.SetMounting();
			if (base.IsRiding && this.Montura)
			{
				this.WhileIsMounted();
			}
			if ((this.LinkUpdate & Rider.UpdateType.Update) == Rider.UpdateType.Update)
			{
				this.LinkRider();
			}
		}

		// Token: 0x060039FC RID: 14844 RVA: 0x001C1175 File Offset: 0x001BF375
		private void LateUpdate()
		{
			if ((this.LinkUpdate & Rider.UpdateType.LateUpdate) == Rider.UpdateType.LateUpdate)
			{
				this.LinkRider();
			}
		}

		// Token: 0x060039FD RID: 14845 RVA: 0x001C1188 File Offset: 0x001BF388
		private void FixedUpdate()
		{
			if ((this.LinkUpdate & Rider.UpdateType.FixedUpdate) == Rider.UpdateType.FixedUpdate)
			{
				this.LinkRider();
			}
		}

		// Token: 0x060039FE RID: 14846 RVA: 0x001C119C File Offset: 0x001BF39C
		public virtual void SetMounting()
		{
			if (base.CanMount)
			{
				if (this.MountInput.GetInput)
				{
					this.MountAnimal();
					return;
				}
			}
			else if (this.CanDismount)
			{
				if (this.DismountInput.GetInput)
				{
					this.DismountAnimal();
					return;
				}
			}
			else if (this.CanCallAnimal && this.CallAnimalInput.GetInput)
			{
				this.CallAnimal(true);
			}
		}

		// Token: 0x060039FF RID: 14847 RVA: 0x001C11FD File Offset: 0x001BF3FD
		public virtual void CheckMountDismount()
		{
			if (base.CanMount)
			{
				this.MountAnimal();
				return;
			}
			if (this.CanDismount)
			{
				this.DismountAnimal();
				return;
			}
			if (this.CanCallAnimal)
			{
				this.CallAnimal(true);
			}
		}

		// Token: 0x06003A00 RID: 14848 RVA: 0x001C122C File Offset: 0x001BF42C
		protected virtual void WhileIsMounted()
		{
			this.Animators_ReSync();
		}

		// Token: 0x06003A01 RID: 14849 RVA: 0x001C1234 File Offset: 0x001BF434
		protected virtual void SyncAnimator()
		{
			Animal animal = this.Montura.Animal;
			float num = animal.Speed;
			if (animal.Fly)
			{
				num = Mathf.Clamp(num * 2f, 0f, 2f);
			}
			this.Anim.SetFloat(animal.hash_Vertical, num);
			this.Anim.SetFloat(animal.hash_Horizontal, this.Montura.Animal.Direction);
			this.Anim.SetFloat(animal.hash_Slope, this.Montura.Animal.Slope);
			this.Anim.SetBool(animal.hash_Stand, this.Montura.Animal.Stand);
			this.Anim.SetBool(animal.hash_Jump, !this.Montura.Animal.Fly && this.Montura.Animal.Jump);
			this.Anim.SetBool(animal.hash_Attack1, this.Montura.Animal.Attack1);
			this.Anim.SetBool(animal.hash_Shift, !this.Montura.Animal.Attack2 && this.Montura.Animal.Shift);
			this.Anim.SetBool(animal.hash_Damaged, this.Montura.Animal.Damaged);
			this.Anim.SetBool(animal.hash_Stunned, this.Montura.Animal.Stun);
			this.Anim.SetBool(animal.hash_Action, this.Montura.Animal.Action);
			this.Anim.SetInteger(animal.hash_IDAction, this.Montura.Animal.ActionID);
			this.Anim.SetInteger(animal.hash_IDInt, this.Montura.Animal.IDInt);
			this.Anim.SetFloat(animal.hash_IDFloat, this.Montura.Animal.IDFloat);
			if (this.Montura.Animal.canSwim)
			{
				this.Anim.SetBool(animal.hash_Swim, this.Montura.Animal.Swim);
			}
		}

		// Token: 0x06003A02 RID: 14850 RVA: 0x001C1470 File Offset: 0x001BF670
		public virtual void MountAnimal()
		{
			if (!base.CanMount)
			{
				return;
			}
			if (this.Montura == null)
			{
				return;
			}
			this.Montura.Animal.OnSyncAnimator.AddListener(new UnityAction(this.SyncAnimator));
			if (this.Anim)
			{
				this.Anim.SetLayerWeight(this.MountLayerIndex, 1f);
				this.Anim.SetBool(Hash.Mount, base.Mounted);
			}
			if (!this.Montura.InstantMount)
			{
				if (this.Anim)
				{
					this.Anim.Play(base.MountTrigger.MountAnimation, this.MountLayerIndex);
					return;
				}
			}
			else
			{
				this.Start_Mounting();
				this.End_Mounting();
				if (this.Anim)
				{
					this.Anim.Play(this.Montura.MountIdle, this.MountLayerIndex);
				}
			}
		}

		// Token: 0x06003A03 RID: 14851 RVA: 0x001C155C File Offset: 0x001BF75C
		public virtual void DismountAnimal()
		{
			if (!this.CanDismount)
			{
				return;
			}
			this.Montura.Mounted = (base.Mounted = false);
			this.Montura.Animal.OnSyncAnimator.RemoveListener(new UnityAction(this.SyncAnimator));
			if (this.Anim)
			{
				this.Anim.SetBool(Hash.Mount, base.Mounted);
				this.Anim.SetInteger(Hash.MountSide, base.MountTrigger.DismountID);
			}
			if (this.Montura.InstantMount)
			{
				this.Anim.Play(Hash.Null, this.MountLayerIndex);
				this.Anim.SetInteger(Hash.MountSide, 0);
				this.Start_Dismounting();
				this.End_Dismounting();
				this._transform.position = base.MountTrigger.transform.position + base.MountTrigger.transform.forward * -0.2f;
			}
		}

		// Token: 0x06003A04 RID: 14852 RVA: 0x001C1664 File Offset: 0x001BF864
		private void OnAnimatorIK()
		{
			if (this.Anim == null)
			{
				return;
			}
			if (this.Montura != null)
			{
				if (this.Montura.FootLeftIK == null || this.Montura.FootRightIK == null || this.Montura.KneeLeftIK == null || this.Montura.KneeRightIK == null)
				{
					return;
				}
				if (base.Mounted || base.IsOnHorse)
				{
					this.L_IKFootWeight = 1f;
					this.R_IKFootWeight = 1f;
					int tagHash = this.Anim.GetCurrentAnimatorStateInfo(this.MountLayerIndex).tagHash;
					if (tagHash == Hash.Tag_Mounting || tagHash == Hash.Tag_Unmounting)
					{
						this.L_IKFootWeight = this.Anim.GetFloat(Hash.IKLeftFoot);
						this.R_IKFootWeight = this.Anim.GetFloat(Hash.IKRightFoot);
					}
					this.Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, this.L_IKFootWeight);
					this.Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, this.R_IKFootWeight);
					this.Anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, this.L_IKFootWeight);
					this.Anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, this.R_IKFootWeight);
					this.Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, this.L_IKFootWeight);
					this.Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, this.R_IKFootWeight);
					this.Anim.SetIKPosition(AvatarIKGoal.LeftFoot, this.Montura.FootLeftIK.position);
					this.Anim.SetIKPosition(AvatarIKGoal.RightFoot, this.Montura.FootRightIK.position);
					this.Anim.SetIKHintPosition(AvatarIKHint.LeftKnee, this.Montura.KneeLeftIK.position);
					this.Anim.SetIKHintPosition(AvatarIKHint.RightKnee, this.Montura.KneeRightIK.position);
					this.Anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, this.L_IKFootWeight);
					this.Anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, this.R_IKFootWeight);
					this.Anim.SetIKRotation(AvatarIKGoal.LeftFoot, this.Montura.FootLeftIK.rotation);
					this.Anim.SetIKRotation(AvatarIKGoal.RightFoot, this.Montura.FootRightIK.rotation);
					return;
				}
				this.Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
				this.Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
				this.Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
				this.Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
			}
		}

		// Token: 0x06003A05 RID: 14853 RVA: 0x001C18DC File Offset: 0x001BFADC
		private void OnDrawGizmos()
		{
			if (!this.debug)
			{
				return;
			}
			if (!this.Anim)
			{
				return;
			}
			if (this.syncronize)
			{
				Transform boneTransform = this.Anim.GetBoneTransform(HumanBodyBones.Head);
				if ((int)this.RiderNormalizedTime % 2 == 0)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.white;
				}
				Gizmos.DrawSphere(boneTransform.position - base.transform.root.right * 0.2f, 0.05f);
				if ((int)this.HorseNormalizedTime % 2 == 0)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.white;
				}
				Gizmos.DrawSphere(boneTransform.position + base.transform.root.right * 0.2f, 0.05f);
			}
		}

		// Token: 0x040029F3 RID: 10739
		protected float L_IKFootWeight;

		// Token: 0x040029F4 RID: 10740
		protected float R_IKFootWeight;

		// Token: 0x040029F5 RID: 10741
		public UnityEvent OnStartMounting = new UnityEvent();

		// Token: 0x040029F6 RID: 10742
		public UnityEvent OnEndMounting = new UnityEvent();

		// Token: 0x040029F7 RID: 10743
		public UnityEvent OnStartDismounting = new UnityEvent();

		// Token: 0x040029F8 RID: 10744
		public UnityEvent OnEndDismounting = new UnityEvent();

		// Token: 0x040029F9 RID: 10745
		public UnityEvent OnAlreadyMounted = new UnityEvent();

		// Token: 0x040029FA RID: 10746
		public int MountLayerIndex = -1;

		// Token: 0x040029FB RID: 10747
		protected AnimatorUpdateMode Initial_UpdateMode;

		// Token: 0x040029FC RID: 10748
		private float RiderNormalizedTime;

		// Token: 0x040029FD RID: 10749
		private float HorseNormalizedTime;

		// Token: 0x040029FE RID: 10750
		private float LastSyncTime;

		// Token: 0x040029FF RID: 10751
		private bool syncronize;

		// Token: 0x04002A00 RID: 10752
		[SerializeField]
		public bool Editor_Advanced;

		// Token: 0x04002A01 RID: 10753
		public bool debug;
	}
}
