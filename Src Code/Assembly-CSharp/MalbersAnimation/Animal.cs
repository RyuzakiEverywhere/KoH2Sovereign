using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations.Events;
using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
	// Token: 0x020003BE RID: 958
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Rigidbody))]
	public class Animal : MonoBehaviour, IAnimatorListener, IMDamagable, IMCharacter, ICharacterMove
	{
		// Token: 0x060035DB RID: 13787 RVA: 0x001AF9E6 File Offset: 0x001ADBE6
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
		}

		// Token: 0x060035DC RID: 13788 RVA: 0x001AF9F0 File Offset: 0x001ADBF0
		public virtual void WakeAnimal()
		{
			this.MovementAxis = Vector3.forward * 3f;
			this.ActionID = -2;
		}

		// Token: 0x060035DD RID: 13789 RVA: 0x001AFA0F File Offset: 0x001ADC0F
		public virtual void ToggleStance(int NewStance)
		{
			this.Stance = ((this.Stance == NewStance) ? 0 : NewStance);
		}

		// Token: 0x060035DE RID: 13790 RVA: 0x001AFA24 File Offset: 0x001ADC24
		public virtual void ResetInputs()
		{
			this.Attack1 = false;
			this.Attack2 = false;
			this.Shift = false;
			this.Jump = false;
			this.Action = false;
			this.ActionID = 0;
			this.MovementAxis = Vector3.zero;
			this.RawDirection = Vector3.zero;
		}

		// Token: 0x060035DF RID: 13791 RVA: 0x001AFA71 File Offset: 0x001ADC71
		public virtual void ToggleStance(IntVar NewStance)
		{
			this.Stance = ((this.Stance == NewStance) ? 0 : NewStance);
		}

		// Token: 0x060035E0 RID: 13792 RVA: 0x001AFA90 File Offset: 0x001ADC90
		public virtual void ToggleTurnSpeed(float speeds)
		{
			if (this.walkSpeed.rotation != speeds)
			{
				this.runSpeed.rotation = speeds;
				this.trotSpeed.rotation = speeds;
				this.walkSpeed.rotation = speeds;
				return;
			}
			this.walkSpeed.rotation = (this.trotSpeed.rotation = (this.runSpeed.rotation = 0f));
		}

		// Token: 0x060035E1 RID: 13793 RVA: 0x001AF9F0 File Offset: 0x001ADBF0
		public virtual void InterruptAction()
		{
			this.MovementAxis = Vector3.forward * 3f;
			this.ActionID = -2;
		}

		// Token: 0x060035E2 RID: 13794 RVA: 0x001AFB00 File Offset: 0x001ADD00
		public virtual void getDamaged(DamageValues DV)
		{
			if (this.Death)
			{
				return;
			}
			if (this.isTakingDamage)
			{
				return;
			}
			if (this.inmune)
			{
				return;
			}
			float num = DV.Amount - this.defense;
			this.OnGetDamaged.Invoke(num);
			this.life -= num;
			this.ActionID = -2;
			if (this.life > 0f)
			{
				this.damaged = true;
				base.StartCoroutine(this.IsTakingDamageTime(this.damageDelay));
				this._hitDirection = DV.Direction;
				return;
			}
			this.Death = true;
		}

		// Token: 0x060035E3 RID: 13795 RVA: 0x001AFB93 File Offset: 0x001ADD93
		public virtual void Stop()
		{
			this.movementAxis = Vector3.zero;
			this.RawDirection = Vector3.zero;
		}

		// Token: 0x060035E4 RID: 13796 RVA: 0x001AFBAC File Offset: 0x001ADDAC
		public virtual void getDamaged(Vector3 Mycenter, Vector3 Theircenter, float Amount = 0f)
		{
			DamageValues dv = new DamageValues(Mycenter - Theircenter, Amount);
			this.getDamaged(dv);
		}

		// Token: 0x060035E5 RID: 13797 RVA: 0x001AFBCE File Offset: 0x001ADDCE
		private IEnumerator IsTakingDamageTime(float time)
		{
			this.isTakingDamage = true;
			yield return new WaitForSeconds(time);
			this.isTakingDamage = false;
			yield break;
		}

		// Token: 0x060035E6 RID: 13798 RVA: 0x001AFBE4 File Offset: 0x001ADDE4
		public virtual void AttackTrigger(int triggerIndex)
		{
			if (triggerIndex == -1)
			{
				foreach (AttackTrigger attackTrigger in this.Attack_Triggers)
				{
					attackTrigger.Collider.enabled = true;
					attackTrigger.gameObject.SetActive(true);
				}
				return;
			}
			if (triggerIndex == 0)
			{
				foreach (AttackTrigger attackTrigger2 in this.Attack_Triggers)
				{
					attackTrigger2.Collider.enabled = false;
					attackTrigger2.gameObject.SetActive(false);
				}
				return;
			}
			List<AttackTrigger> list = this.Attack_Triggers.FindAll((AttackTrigger item) => item.index == triggerIndex);
			if (list != null)
			{
				foreach (AttackTrigger attackTrigger3 in list)
				{
					attackTrigger3.Collider.enabled = true;
					attackTrigger3.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x060035E7 RID: 13799 RVA: 0x001AFD20 File Offset: 0x001ADF20
		public virtual void SetAttack()
		{
			this.activeAttack = -1;
			this.Attack1 = true;
		}

		// Token: 0x060035E8 RID: 13800 RVA: 0x001AFD30 File Offset: 0x001ADF30
		public virtual void SetLoop(int cycles)
		{
			this.Loops = cycles;
		}

		// Token: 0x060035E9 RID: 13801 RVA: 0x001AFD39 File Offset: 0x001ADF39
		public virtual void SetAttack(int attackID)
		{
			this.activeAttack = attackID;
			this.Attack1 = true;
		}

		// Token: 0x060035EA RID: 13802 RVA: 0x001AFD49 File Offset: 0x001ADF49
		public virtual void SetAttack(bool value)
		{
			this.Attack1 = value;
		}

		// Token: 0x060035EB RID: 13803 RVA: 0x001AFD52 File Offset: 0x001ADF52
		public virtual void SetSecondaryAttack()
		{
			if (this.hasAttack2)
			{
				base.StartCoroutine(this.ToogleAttack2());
			}
		}

		// Token: 0x060035EC RID: 13804 RVA: 0x001AFD69 File Offset: 0x001ADF69
		public virtual void RigidDrag(float amount)
		{
			this._RigidBody.drag = amount;
		}

		// Token: 0x060035ED RID: 13805 RVA: 0x001AFD77 File Offset: 0x001ADF77
		private IEnumerator ToogleAttack2()
		{
			int num;
			for (int i = 0; i < this.ToogleAmount; i = num + 1)
			{
				this.Attack2 = true;
				yield return null;
				num = i;
			}
			this.Attack2 = false;
			yield break;
		}

		// Token: 0x060035EE RID: 13806 RVA: 0x001AFD88 File Offset: 0x001ADF88
		public virtual bool CurrentAnimation(params int[] AnimsTags)
		{
			for (int i = 0; i < AnimsTags.Length; i++)
			{
				if (this.AnimState == AnimsTags[i])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060035EF RID: 13807 RVA: 0x001AFDB1 File Offset: 0x001ADFB1
		public void SetIntID(int value)
		{
			this.IDInt = value;
			this.Anim.SetInteger(Hash.IDInt, this.IDInt);
		}

		// Token: 0x060035F0 RID: 13808 RVA: 0x001AFDD0 File Offset: 0x001ADFD0
		public void SetFloatID(float value)
		{
			this.IDFloat = value;
			this.Anim.SetFloat(Hash.IDFloat, this.IDFloat);
		}

		// Token: 0x060035F1 RID: 13809 RVA: 0x001AFDEF File Offset: 0x001ADFEF
		protected void SetIntIDRandom(int range)
		{
			this.IDInt = Random.Range(1, range + 1);
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x060035F2 RID: 13810 RVA: 0x001AFE00 File Offset: 0x001AE000
		public bool IsJumping
		{
			get
			{
				return this.AnimState == AnimTag.Jump;
			}
		}

		// Token: 0x060035F3 RID: 13811 RVA: 0x001AFE0F File Offset: 0x001AE00F
		public virtual void StillConstraints(bool active)
		{
			this._RigidBody.constraints = (active ? Animal.Still_Constraints : RigidbodyConstraints.FreezeRotation);
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x060035F4 RID: 13812 RVA: 0x001AFE28 File Offset: 0x001AE028
		// (set) Token: 0x060035F5 RID: 13813 RVA: 0x001AFE30 File Offset: 0x001AE030
		public bool ActiveColliders { get; private set; }

		// Token: 0x060035F6 RID: 13814 RVA: 0x001AFE3C File Offset: 0x001AE03C
		public virtual void EnableColliders(bool active)
		{
			this.ActiveColliders = active;
			if (!active)
			{
				this._col_ = base.GetComponentsInChildren<Collider>(false).ToList<Collider>();
				List<Collider> list = new List<Collider>();
				foreach (Collider collider in this._col_)
				{
					if (!collider.isTrigger && collider.enabled)
					{
						list.Add(collider);
					}
				}
				this._col_ = list;
			}
			foreach (Collider collider2 in this._col_)
			{
				collider2.enabled = active;
			}
			if (active)
			{
				this._col_ = new List<Collider>();
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x060035F8 RID: 13816 RVA: 0x001AFF26 File Offset: 0x001AE126
		// (set) Token: 0x060035F7 RID: 13815 RVA: 0x001AFF18 File Offset: 0x001AE118
		public virtual bool Gravity
		{
			get
			{
				return this._RigidBody.useGravity;
			}
			set
			{
				this._RigidBody.useGravity = value;
			}
		}

		// Token: 0x060035F9 RID: 13817 RVA: 0x001AFF33 File Offset: 0x001AE133
		public virtual void InAir(bool active)
		{
			this.IsInAir = active;
			this.StillConstraints(!active);
		}

		// Token: 0x060035FA RID: 13818 RVA: 0x001AFF46 File Offset: 0x001AE146
		public virtual void SetJump()
		{
			base.StartCoroutine(this.ToggleJump());
		}

		// Token: 0x060035FB RID: 13819 RVA: 0x001AFF55 File Offset: 0x001AE155
		public virtual void SetAction(int ID)
		{
			this.ActionID = ID;
			this.Action = true;
		}

		// Token: 0x060035FC RID: 13820 RVA: 0x001AFF65 File Offset: 0x001AE165
		public virtual void SetAction(Action ID)
		{
			this.ActionID = ID;
			this.Action = true;
		}

		// Token: 0x060035FD RID: 13821 RVA: 0x001AFF7C File Offset: 0x001AE17C
		public virtual void SetAction(string actionName)
		{
			if (this.Anim.HasState(0, Animator.StringToHash(actionName)))
			{
				if (this.AnimState != AnimTag.Action && this.ActionID <= 0)
				{
					this.Anim.CrossFade(actionName, 0.1f, 0);
					return;
				}
			}
			else
			{
				Debug.LogWarning("The animal does not have an action called " + actionName);
			}
		}

		// Token: 0x060035FE RID: 13822 RVA: 0x001AFFD6 File Offset: 0x001AE1D6
		public virtual void ResetAnimal()
		{
			this.fly = false;
			this.swim = false;
			this.fall = false;
			this.action = false;
			this.attack1 = false;
			this.damaged = false;
			this.attack2 = false;
			this.anim.Rebind();
		}

		// Token: 0x060035FF RID: 13823 RVA: 0x001B0014 File Offset: 0x001AE214
		public virtual void SetStun(float time)
		{
			if (this.StunC != null)
			{
				base.StopCoroutine(this.StunC);
			}
			this.StunC = null;
			this.StunC = this.ToggleStun(time);
			base.StartCoroutine(this.StunC);
		}

		// Token: 0x06003600 RID: 13824 RVA: 0x001B004C File Offset: 0x001AE24C
		public virtual void DisableAnimal()
		{
			base.enabled = false;
			MalbersInput component = base.GetComponent<MalbersInput>();
			if (component)
			{
				component.enabled = false;
			}
		}

		// Token: 0x06003601 RID: 13825 RVA: 0x001B0076 File Offset: 0x001AE276
		public virtual void SetFly(bool value)
		{
			if (this.canFly && this.hasFly)
			{
				this.fly = !value;
				this.Fly = true;
			}
		}

		// Token: 0x06003602 RID: 13826 RVA: 0x001B0099 File Offset: 0x001AE299
		public virtual void SetToGlide(float value)
		{
			if (this.fly && this.fall)
			{
				base.StartCoroutine(this.GravityDrag(value));
			}
		}

		// Token: 0x06003603 RID: 13827 RVA: 0x001B00B9 File Offset: 0x001AE2B9
		internal IEnumerator GravityDrag(float value)
		{
			while (this.AnimState != AnimTag.Fly)
			{
				yield return null;
			}
			this.groundSpeed = 2f;
			if (this._RigidBody)
			{
				this._RigidBody.useGravity = true;
				this._RigidBody.drag = value;
			}
			yield break;
		}

		// Token: 0x06003604 RID: 13828 RVA: 0x001B00CF File Offset: 0x001AE2CF
		internal IEnumerator ToggleJump()
		{
			int num;
			for (int i = 0; i < this.ToogleAmount; i = num + 1)
			{
				this.Jump = true;
				yield return null;
				num = i;
			}
			this.Jump = false;
			yield break;
		}

		// Token: 0x06003605 RID: 13829 RVA: 0x001B00DE File Offset: 0x001AE2DE
		internal IEnumerator ToggleAction()
		{
			int num;
			for (int i = 0; i < this.ToogleAmount; i = num + 1)
			{
				this.action = true;
				if (this.AnimState == AnimTag.Action)
				{
					this.OnAction.Invoke();
					this.SetFloatID(-1f);
					break;
				}
				yield return null;
				num = i;
			}
			this.action = false;
			if (this.AnimState != AnimTag.Action)
			{
				this.ActionID = -1;
				this.SetFloatID(0f);
			}
			yield break;
		}

		// Token: 0x06003606 RID: 13830 RVA: 0x001B00ED File Offset: 0x001AE2ED
		internal IEnumerator ToggleStun(float time)
		{
			this.Stun = true;
			yield return new WaitForSeconds(time);
			this.stun = false;
			yield break;
		}

		// Token: 0x06003607 RID: 13831 RVA: 0x001B0103 File Offset: 0x001AE303
		public void InitializeInputs(Dictionary<string, BoolEvent> keys)
		{
			if (this.Inputs == null)
			{
				this.Inputs = new Dictionary<string, BoolEvent>();
			}
			this.Inputs = keys;
			this.CharacterConnect();
		}

		// Token: 0x06003608 RID: 13832 RVA: 0x001B0128 File Offset: 0x001AE328
		public void SetInput(string key, bool value)
		{
			BoolEvent boolEvent;
			if (this.Inputs.TryGetValue(key, out boolEvent))
			{
				boolEvent.Invoke(value);
			}
		}

		// Token: 0x06003609 RID: 13833 RVA: 0x001B014C File Offset: 0x001AE34C
		public void AddInput(string key, BoolEvent NewBool)
		{
			if (!this.Inputs.ContainsKey(key))
			{
				this.Inputs.Add(key, NewBool);
			}
		}

		// Token: 0x0600360A RID: 13834 RVA: 0x001B016C File Offset: 0x001AE36C
		private void CharacterConnect()
		{
			BoolEvent boolEvent;
			if (this.Inputs.TryGetValue("Attack1", out boolEvent))
			{
				boolEvent.AddListener(delegate(bool value)
				{
					this.Attack1 = value;
				});
			}
			BoolEvent boolEvent2;
			if (this.Inputs.TryGetValue("Attack2", out boolEvent2))
			{
				boolEvent2.AddListener(delegate(bool value)
				{
					this.Attack2 = value;
				});
			}
			BoolEvent boolEvent3;
			if (this.Inputs.TryGetValue("Action", out boolEvent3))
			{
				boolEvent3.AddListener(delegate(bool value)
				{
					this.Action = value;
				});
			}
			BoolEvent boolEvent4;
			if (this.Inputs.TryGetValue("Jump", out boolEvent4))
			{
				boolEvent4.AddListener(delegate(bool value)
				{
					this.Jump = value;
				});
			}
			BoolEvent boolEvent5;
			if (this.Inputs.TryGetValue("Shift", out boolEvent5))
			{
				boolEvent5.AddListener(delegate(bool value)
				{
					this.Shift = value;
				});
			}
			BoolEvent boolEvent6;
			if (this.Inputs.TryGetValue("Fly", out boolEvent6))
			{
				boolEvent6.AddListener(delegate(bool value)
				{
					this.Fly = value;
				});
			}
			BoolEvent boolEvent7;
			if (this.Inputs.TryGetValue("Down", out boolEvent7))
			{
				boolEvent7.AddListener(delegate(bool value)
				{
					this.Down = value;
				});
			}
			BoolEvent boolEvent8;
			if (this.Inputs.TryGetValue("Up", out boolEvent8))
			{
				boolEvent8.AddListener(delegate(bool value)
				{
					this.Up = value;
				});
			}
			BoolEvent boolEvent9;
			if (this.Inputs.TryGetValue("Dodge", out boolEvent9))
			{
				boolEvent9.AddListener(delegate(bool value)
				{
					this.Dodge = value;
				});
			}
			BoolEvent boolEvent10;
			if (this.Inputs.TryGetValue("Death", out boolEvent10))
			{
				boolEvent10.AddListener(delegate(bool value)
				{
					this.Death = value;
				});
			}
			BoolEvent boolEvent11;
			if (this.Inputs.TryGetValue("Stun", out boolEvent11))
			{
				boolEvent11.AddListener(delegate(bool value)
				{
					this.Stun = value;
				});
			}
			BoolEvent boolEvent12;
			if (this.Inputs.TryGetValue("Damaged", out boolEvent12))
			{
				boolEvent12.AddListener(delegate(bool value)
				{
					this.Damaged = value;
				});
			}
			BoolEvent boolEvent13;
			if (this.Inputs.TryGetValue("Speed1", out boolEvent13))
			{
				boolEvent13.AddListener(delegate(bool value)
				{
					this.Speed1 = value;
				});
			}
			BoolEvent boolEvent14;
			if (this.Inputs.TryGetValue("Speed2", out boolEvent14))
			{
				boolEvent14.AddListener(delegate(bool value)
				{
					this.Speed2 = value;
				});
			}
			BoolEvent boolEvent15;
			if (this.Inputs.TryGetValue("Speed3", out boolEvent15))
			{
				boolEvent15.AddListener(delegate(bool value)
				{
					this.Speed3 = value;
				});
			}
			BoolEvent boolEvent16;
			if (this.Inputs.TryGetValue("SpeedUp", out boolEvent16))
			{
				boolEvent16.AddListener(delegate(bool value)
				{
					this.SpeedUp = value;
				});
			}
			BoolEvent boolEvent17;
			if (this.Inputs.TryGetValue("SpeedDown", out boolEvent17))
			{
				boolEvent17.AddListener(delegate(bool value)
				{
					this.SpeedDown = value;
				});
			}
		}

		// Token: 0x0600360B RID: 13835 RVA: 0x001B040C File Offset: 0x001AE60C
		private void Reset()
		{
			MalbersTools.SetLayer(base.transform, 20);
			base.gameObject.tag = "Animal";
		}

		// Token: 0x0600360C RID: 13836 RVA: 0x001B042C File Offset: 0x001AE62C
		protected virtual void GetHashIDs()
		{
			this.hash_Vertical = Animator.StringToHash(this.m_Vertical);
			this.hash_Horizontal = Animator.StringToHash(this.m_Horizontal);
			this.hash_UpDown = Animator.StringToHash(this.m_UpDown);
			this.hash_Stand = Animator.StringToHash(this.m_Stand);
			this.hash_Jump = Animator.StringToHash(this.m_Jump);
			this.hash_Dodge = Animator.StringToHash(this.m_Dodge);
			this.hash_Fall = Animator.StringToHash(this.m_Fall);
			this.hash_Type = Animator.StringToHash(this.m_Type);
			this.hash_Slope = Animator.StringToHash(this.m_Slope);
			this.hash_Shift = Animator.StringToHash(this.m_Shift);
			this.hash_Fly = Animator.StringToHash(this.m_Fly);
			this.hash_Attack1 = Animator.StringToHash(this.m_Attack1);
			this.hash_Attack2 = Animator.StringToHash(this.m_Attack2);
			this.hash_Death = Animator.StringToHash(this.m_Death);
			this.hash_Damaged = Animator.StringToHash(this.m_Damaged);
			this.hash_Stunned = Animator.StringToHash(this.m_Stunned);
			this.hash_IDInt = Animator.StringToHash(this.m_IDInt);
			this.hash_IDFloat = Animator.StringToHash(this.m_IDFloat);
			this.hash_Swim = Animator.StringToHash(this.m_Swim);
			this.hash_Underwater = Animator.StringToHash(this.m_Underwater);
			this.hash_Action = Animator.StringToHash(this.m_Action);
			this.hash_IDAction = Animator.StringToHash(this.m_IDAction);
			this.hash_StateTime = Animator.StringToHash(this.m_StateTime);
			this.hash_Stance = Animator.StringToHash(this.m_Stance);
		}

		// Token: 0x0600360D RID: 13837 RVA: 0x001B05D4 File Offset: 0x001AE7D4
		private void Awake()
		{
			this.AnimalMesh = base.GetComponentInChildren<Renderer>();
			this.anim = base.GetComponent<Animator>();
			this.GetHashIDs();
			this._transform = base.transform;
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Int, this.hash_Type))
			{
				this.Anim.SetInteger(this.hash_Type, this.animalTypeID);
			}
			this.WaterLayer = LayerMask.GetMask(new string[]
			{
				"Water"
			});
			this.anim.applyRootMotion = true;
		}

		// Token: 0x0600360E RID: 13838 RVA: 0x001B065B File Offset: 0x001AE85B
		private void Start()
		{
			this.SetStart();
		}

		// Token: 0x0600360F RID: 13839 RVA: 0x001B0664 File Offset: 0x001AE864
		protected virtual void SetStart()
		{
			this.DeltaPosition = Vector3.zero;
			this._RigidBody.isKinematic = false;
			this.Anim.updateMode = AnimatorUpdateMode.Normal;
			this.isInAir = false;
			this.scaleFactor = this._transform.localScale.y;
			this.MovementReleased = true;
			this.SetPivots();
			this.ActiveColliders = true;
			switch (this.StartSpeed)
			{
			case Animal.Ground.walk:
				this.Speed1 = true;
				break;
			case Animal.Ground.trot:
				this.Speed2 = true;
				break;
			case Animal.Ground.run:
				this.Speed3 = true;
				break;
			}
			this.Attack_Triggers = base.GetComponentsInChildren<AttackTrigger>(true).ToList<AttackTrigger>();
			this.OptionalAnimatorParameters();
			this.Start_Flying();
			this.FrameCounter = Random.Range(0, 10000);
			this.OnAnimationChange.AddListener(new UnityAction<int>(this.OnAnimationStateEnter));
		}

		// Token: 0x06003610 RID: 13840 RVA: 0x001B0744 File Offset: 0x001AE944
		public virtual void SetPivots()
		{
			this.pivots = base.GetComponentsInChildren<Pivots>().ToList<Pivots>();
			if (this.pivots != null)
			{
				this.pivot_Hip = this.pivots.Find((Pivots p) => p.name.ToUpperInvariant().Contains("HIP"));
				this.pivot_Chest = this.pivots.Find((Pivots p) => p.name.ToUpperInvariant().Contains("CHEST"));
				this.pivot_Water = this.pivots.Find((Pivots p) => p.name.ToUpperInvariant().Contains("WATER"));
			}
		}

		// Token: 0x06003611 RID: 13841 RVA: 0x001B0800 File Offset: 0x001AEA00
		protected virtual void Start_Flying()
		{
			if (this.hasFly && this.StartFlying && this.canFly)
			{
				this.stand = false;
				this.Fly = true;
				this.Anim.Play("Fly", 0);
				this.IsInAir = true;
				this._RigidBody.useGravity = false;
			}
		}

		// Token: 0x06003612 RID: 13842 RVA: 0x001B0858 File Offset: 0x001AEA58
		protected void OptionalAnimatorParameters()
		{
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Bool, this.hash_Swim))
			{
				this.hasSwim = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Bool, this.hash_Dodge))
			{
				this.hasDodge = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Bool, this.hash_Fly))
			{
				this.hasFly = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Bool, this.hash_Attack2))
			{
				this.hasAttack2 = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Bool, this.hash_Stunned))
			{
				this.hasStun = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Bool, this.hash_Underwater))
			{
				this.hasUnderwater = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Float, this.hash_UpDown))
			{
				this.hasUpDown = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Float, this.hash_Slope))
			{
				this.hasSlope = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Float, this.hash_StateTime))
			{
				this.hasStateTime = true;
			}
			if (MalbersTools.FindAnimatorParameter(this.Anim, AnimatorControllerParameterType.Int, this.hash_Stance))
			{
				this.hasStance = true;
			}
		}

		// Token: 0x06003613 RID: 13843 RVA: 0x001B0974 File Offset: 0x001AEB74
		public virtual void LinkingAnimator()
		{
			if (!this.Death)
			{
				this.Anim.SetFloat(this.hash_Vertical, this.vertical);
				this.Anim.SetFloat(this.hash_Horizontal, this.horizontal);
				this.Anim.SetBool(this.hash_Stand, this.stand);
				this.Anim.SetBool(this.hash_Shift, this.Shift);
				this.Anim.SetBool(this.hash_Jump, this.jump);
				this.Anim.SetBool(this.hash_Attack1, this.attack1);
				this.Anim.SetBool(this.hash_Damaged, this.damaged);
				this.Anim.SetBool(this.hash_Action, this.action);
				this.Anim.SetInteger(this.hash_IDAction, this.ActionID);
				this.Anim.SetInteger(this.hash_IDInt, this.IDInt);
				if (this.hasSlope)
				{
					this.Anim.SetFloat(this.hash_Slope, this.Slope);
				}
				if (this.hasStun)
				{
					this.Anim.SetBool(this.hash_Stunned, this.stun);
				}
				if (this.hasAttack2)
				{
					this.Anim.SetBool(this.hash_Attack2, this.attack2);
				}
				if (this.hasUpDown)
				{
					this.Anim.SetFloat(this.hash_UpDown, this.movementAxis.y);
				}
				if (this.hasStateTime)
				{
					this.Anim.SetFloat(this.hash_StateTime, this.StateTime);
				}
				if (this.hasDodge)
				{
					this.Anim.SetBool(this.hash_Dodge, this.dodge);
				}
				if (this.hasFly && this.canFly)
				{
					this.Anim.SetBool(this.hash_Fly, this.Fly);
				}
				if (this.hasSwim && this.canSwim)
				{
					this.Anim.SetBool(this.hash_Swim, this.swim);
				}
				if (this.hasUnderwater && this.CanGoUnderWater)
				{
					this.Anim.SetBool(this.hash_Underwater, this.underwater);
				}
			}
			this.Anim.SetBool(this.hash_Fall, this.fall);
			this.OnSyncAnimator.Invoke();
		}

		// Token: 0x06003614 RID: 13844 RVA: 0x001B0BC8 File Offset: 0x001AEDC8
		public virtual void Move(Vector3 move, bool active = true)
		{
			this.MovementReleased = (move.x == 0f && move.z == 0f);
			this.directionalMovement = active;
			float deltaTime = Time.deltaTime;
			this.RawDirection = move.normalized;
			if (this.LockUp && move.y > 0f)
			{
				move.y = 0f;
			}
			if (active)
			{
				if (move.magnitude > 1f)
				{
					move.Normalize();
				}
				this.RawDirection = Vector3.Lerp(this.RawDirection, move, deltaTime * this.upDownSmoothness * 5f);
				move = this._transform.InverseTransformDirection(move);
				if (!this.Fly && !this.underwater)
				{
					move = Vector3.ProjectOnPlane(move, this.SurfaceNormal).normalized;
				}
				float x = Mathf.Atan2(move.x, move.z);
				float num = move.z;
				if (!this.SmoothVertical)
				{
					if (num > 0f)
					{
						num = 1f;
					}
					if (num < 0f)
					{
						num = -1f;
					}
				}
				this.movementAxis = new Vector3(x, this.IgnoreYDir ? this.movementAxis.y : this.RawDirection.y, Mathf.Abs(num));
				if ((this.Fly || this.underwater) && !this.Up && !this.Down && this.IgnoreYDir)
				{
					this.movementAxis.y = Mathf.Lerp(this.movementAxis.y, 0f, deltaTime * this.upDownSmoothness * 3f);
				}
				if (!this.stand && this.AnimState != AnimTag.Action && this.AnimState != AnimTag.Sleep)
				{
					this.DeltaRotation *= Quaternion.Euler(0f, this.movementAxis.x * deltaTime * this.TurnMultiplier, 0f);
					this.DeltaPosition += this._transform.DeltaPositionFromRotate(this.AnimalMesh.bounds.center, this.UpVector, this.movementAxis.x * deltaTime * this.TurnMultiplier);
				}
				if (this.AnimState == AnimTag.Action)
				{
					this.movementAxis = Vector3.zero;
					return;
				}
			}
			else
			{
				this.movementAxis = new Vector3(move.x, this.movementAxis.y, move.z);
			}
		}

		// Token: 0x06003615 RID: 13845 RVA: 0x001B0E48 File Offset: 0x001AF048
		protected virtual void AdditionalTurn(float time)
		{
			float rotation = this.currentSpeed.rotation;
			float num = Mathf.Clamp(this.horizontal, -1f, 1f) * (float)((this.movementAxis.z >= 0f) ? 1 : -1);
			Vector3 euler = this._transform.InverseTransformDirection(0f, rotation * 2f * num * time, 0f);
			this.DeltaRotation *= Quaternion.Euler(euler);
			if (this.Fly || this.swim || this.stun || this.AnimState == AnimTag.Action)
			{
				return;
			}
			if (this.AnimState == AnimTag.Jump || this.AnimState == AnimTag.Fall)
			{
				float num2 = this.airRotation * this.horizontal * time * (float)((this.movementAxis.z >= 0f) ? 1 : -1);
				this.DeltaRotation *= Quaternion.Euler(this._transform.InverseTransformDirection(0f, num2, 0f));
				this.DeltaPosition += this._transform.DeltaPositionFromRotate(this.AnimalMesh.bounds.center, this.T_Up, num2);
			}
		}

		// Token: 0x06003616 RID: 13846 RVA: 0x001B0F98 File Offset: 0x001AF198
		protected virtual void AdditionalSpeed(float time)
		{
			this.currentSpeed = new Speeds(1);
			if (this.hasUnderwater && this.underwater && this.CurrentAnimState == AnimTag.Underwater)
			{
				this.currentSpeed = this.underWaterSpeed;
			}
			else if (this.hasSwim && this.swim && this.CurrentAnimState == AnimTag.Swim)
			{
				this.currentSpeed = this.swimSpeed;
			}
			else if (this.hasFly && this.fly && this.CurrentAnimState == AnimTag.Fly)
			{
				this.currentSpeed = this.flySpeed;
			}
			else if (this.IsJumping || this.fall || this.CurrentAnimState == AnimTag.Fall)
			{
				this.currentSpeed = new Speeds(1);
			}
			else if (this.Speed3 || (this.Speed2 && this.Shift))
			{
				this.currentSpeed = this.runSpeed;
			}
			else if (this.Speed2 || (this.Speed1 && this.Shift))
			{
				this.currentSpeed = this.trotSpeed;
			}
			else if (this.Speed1)
			{
				this.currentSpeed = this.walkSpeed;
			}
			if (this.vertical < 0f)
			{
				this.currentSpeed.position = this.walkSpeed.position;
			}
			this.currentSpeed.position = this.currentSpeed.position * this.ScaleFactor;
			Vector3 a = this.T_Forward * this.vertical;
			if (a.magnitude > 1f)
			{
				a.Normalize();
			}
			this.DeltaPosition += a * this.currentSpeed.position / 5f * time;
			this.Anim.speed = Mathf.Lerp(this.Anim.speed, this.currentSpeed.animator * this.animatorSpeed, time * this.currentSpeed.lerpAnimator);
		}

		// Token: 0x06003617 RID: 13847 RVA: 0x001B119C File Offset: 0x001AF39C
		public virtual void YAxisMovement(float smoothness, float time)
		{
			if (this.Up)
			{
				this.Down = false;
			}
			float num = this.MovementAxis.y;
			if (this.Up)
			{
				num = Mathf.Lerp(num, this.LockUp ? 0f : ((this.MovementForward > 0f) ? 0.7f : 1f), time * smoothness);
			}
			else if (this.Down)
			{
				num = Mathf.Lerp(num, (this.MovementForward > 0f) ? -0.7f : -1f, time * smoothness);
			}
			else if (!this.DirectionalMovement)
			{
				num = Mathf.Lerp(num, 0f, time * smoothness);
			}
			if (Mathf.Abs(num) < 0.001f)
			{
				num = 0f;
			}
			this.movementAxis.y = num;
		}

		// Token: 0x06003618 RID: 13848 RVA: 0x001B1264 File Offset: 0x001AF464
		private void UpdatePlatformMovement(bool update)
		{
			if (this.platform == null)
			{
				return;
			}
			if (this.AnimState == AnimTag.Jump || this.AnimState == AnimTag.NoAlign || this.underwater || this.fly)
			{
				this.platform = null;
				return;
			}
			if (!update)
			{
				this.FixedDeltaPos = this.platform.position - this.platform_Pos;
				this.platform_Pos = this.platform.position;
				return;
			}
			float num = this.platform.eulerAngles.y - this.platform_formAngle;
			if (num == 0f)
			{
				return;
			}
			this.DeltaRotation *= Quaternion.Euler(0f, num, 0f);
			this.DeltaPosition += this._transform.DeltaPositionFromRotate(this.platform.position, Vector3.up, num);
			this.platform_formAngle = this.platform.eulerAngles.y;
		}

		// Token: 0x06003619 RID: 13849 RVA: 0x001B1368 File Offset: 0x001AF568
		protected void RayCasting()
		{
			if (this.AnimState != AnimTag.Jump && this.AnimState != AnimTag.JumpEnd && this.AnimState != AnimTag.Recover && this.AnimState != AnimTag.Fall && this.FrameCounter % this.PivotsRayInterval != 0)
			{
				return;
			}
			if (this.underwater)
			{
				return;
			}
			this.UpVector = -Physics.gravity;
			this.scaleFactor = this._transform.localScale.y;
			this._Height = this.height * this.scaleFactor;
			this.backray = (this.frontray = false);
			this.hit_Chest = Animal.NULLRayCast;
			this.hit_Hip = Animal.NULLRayCast;
			this.hit_Chest.distance = (this.hit_Hip.distance = this._Height);
			if (this.Pivot_Hip != null)
			{
				if (Physics.Raycast(this.Pivot_Hip.GetPivot, -this.T_Up, out this.hit_Hip, this.scaleFactor * this.Pivot_Hip.multiplier, this.GroundLayer))
				{
					if (this.debug)
					{
						Debug.DrawRay(this.hit_Hip.point, this.hit_Hip.normal * 0.2f, Color.blue);
					}
					this.backray = true;
					if (this.platform == null && this.AnimState != AnimTag.Jump)
					{
						this.platform = this.hit_Hip.transform;
						this.platform_Pos = this.platform.position;
						this.platform_formAngle = this.platform.eulerAngles.y;
					}
				}
				else
				{
					this.platform = null;
				}
			}
			if (Physics.Raycast(this.Main_Pivot_Point, -this.T_Up, out this.hit_Chest, this.Pivot_Multiplier, this.GroundLayer))
			{
				if (this.debug)
				{
					Debug.DrawRay(this.hit_Chest.point, this.hit_Chest.normal * 0.2f, Color.red);
				}
				if (Vector3.Angle(this.hit_Chest.normal, Vector3.up) < this.maxAngleSlope)
				{
					this.frontray = true;
				}
			}
			if (this.debug && this.frontray && this.backray)
			{
				Debug.DrawLine(this.hit_Hip.point, this.hit_Chest.point, Color.yellow);
			}
			if (!this.frontray && this.Stand)
			{
				this.fall = true;
				if (this.pivot_Hip && this.backray)
				{
					this.fall = false;
				}
			}
			this.FixDistance = this.hit_Hip.distance;
			if (!this.backray)
			{
				this.FixDistance = this.hit_Chest.distance;
			}
			if (!this.Pivot_Hip)
			{
				this.backray = this.frontray;
			}
			if (!this.Pivot_Chest)
			{
				this.frontray = this.backray;
			}
		}

		// Token: 0x0600361A RID: 13850 RVA: 0x001B1678 File Offset: 0x001AF878
		public virtual void AlignRotation(bool align, float time, float smoothness)
		{
			Quaternion rhs = Quaternion.FromToRotation(this.T_Up, this.SurfaceNormal) * this._transform.rotation;
			Quaternion lhs = Quaternion.Inverse(this._transform.rotation);
			Quaternion rhs2;
			if (align)
			{
				rhs2 = Quaternion.Inverse(this._transform.rotation) * rhs;
			}
			else
			{
				Quaternion rhs3 = Quaternion.FromToRotation(this.T_Up, this.UpVector) * this._transform.rotation;
				rhs2 = lhs * rhs3;
			}
			rhs2 = Quaternion.Slerp(this.DeltaRotation, this.DeltaRotation * rhs2, time * smoothness / 2f);
			this.DeltaRotation *= rhs2;
		}

		// Token: 0x0600361B RID: 13851 RVA: 0x001B1734 File Offset: 0x001AF934
		protected virtual void FixRotation(float time)
		{
			if (this.swim || this.fly || this.underwater)
			{
				return;
			}
			if (this.IsInAir || this.slope < -1f || this.AnimState == AnimTag.NoAlign || !this.backray || (this.backray && !this.frontray))
			{
				if (this.slope < 0f || this.AnimState == AnimTag.Fall)
				{
					this.AlignRotation(false, time, this.AlingToGround);
					return;
				}
			}
			else
			{
				this.AlignRotation(true, time, this.AlingToGround);
			}
		}

		// Token: 0x0600361C RID: 13852 RVA: 0x001B17CC File Offset: 0x001AF9CC
		internal virtual void RaycastWater()
		{
			if (!this.pivot_Water)
			{
				return;
			}
			if (Physics.Raycast(this.pivot_Water.transform.position, -this.T_Up, out this.WaterHitCenter, this.scaleFactor * this.pivot_Water.multiplier * 1.5f, this.WaterLayer))
			{
				this.waterLevel = this.WaterHitCenter.point.y;
				this.isInWater = true;
				return;
			}
			if (this.isInWater && this.AnimState != AnimTag.SwimJump)
			{
				this.isInWater = false;
			}
		}

		// Token: 0x0600361D RID: 13853 RVA: 0x001B1868 File Offset: 0x001AFA68
		protected virtual void Swimming(float time)
		{
			if (!this.hasSwim || !this.canSwim)
			{
				return;
			}
			if (this.underwater)
			{
				return;
			}
			if (this.Stand || !this.pivot_Water)
			{
				return;
			}
			if (this.FrameCounter % this.WaterRayInterval == 0)
			{
				this.RaycastWater();
			}
			if (this.isInWater)
			{
				if (((this.hit_Chest.distance < this._Height * 0.8f && this.movementAxis.z > 0f && this.hit_Chest.transform != null) || (this.hit_Hip.distance < this._Height * 0.8f && this.movementAxis.z < 0f && this.hit_Hip.transform != null)) && this.AnimState != AnimTag.Recover)
				{
					this.Swim = false;
					return;
				}
				if (!this.swim && this.Pivot_Chest.Y <= this.Waterlevel)
				{
					this.Swim = true;
				}
			}
			if (this.swim)
			{
				float num = Vector3.Angle(this.T_Up, this.WaterHitCenter.normal);
				Quaternion rhs = Quaternion.FromToRotation(this.T_Up, this.WaterHitCenter.normal) * this._transform.rotation;
				Quaternion rhs2 = Quaternion.Inverse(this._transform.rotation) * rhs;
				if (num > 0.5f)
				{
					rhs2 = Quaternion.Slerp(this.DeltaRotation, this.DeltaRotation * rhs2, time * 10f);
				}
				this.DeltaRotation *= rhs2;
				if (this.CanGoUnderWater && this.Down && !this.IsJumping && this.AnimState != AnimTag.SwimJump)
				{
					this.underwater = true;
				}
			}
		}

		// Token: 0x0600361E RID: 13854 RVA: 0x001B1A3C File Offset: 0x001AFC3C
		protected virtual void FixPosition(float time)
		{
			if (this.swim)
			{
				return;
			}
			float num = this._Height - this.FixDistance;
			if (this.FixDistance > this._Height)
			{
				if (!this.isInAir && !this.swim)
				{
					this.YFix += (((this.AnimState == AnimTag.Locomotion || this.Stand) && num > 0.01f) ? num : (num * time * this.SnapToGround));
				}
			}
			else if (!this.fall && !this.IsInAir)
			{
				this.YFix += ((num < 0.01f || this.Stand) ? num : (num * time * this.SnapToGround));
			}
			this.FixDistance += this.YFix;
		}

		// Token: 0x0600361F RID: 13855 RVA: 0x001B1B04 File Offset: 0x001AFD04
		protected virtual void Falling()
		{
			this.fall_Point = this.Main_Pivot_Point + this.T_Forward * (this.Shift ? (this.GroundSpeed + 1f) : this.GroundSpeed) * this.FallRayDistance * this.ScaleFactor;
			if (this.FrameCounter % this.FallRayInterval != 0)
			{
				return;
			}
			if (this.AnimState == AnimTag.Sleep || this.AnimState == AnimTag.Action || this.AnimState == AnimTag.Swim || this.AnimState == AnimTag.Idle || this.swim || this.underwater)
			{
				return;
			}
			float num = this.Pivot_Multiplier;
			if (this.AnimState == AnimTag.Jump || this.AnimState == AnimTag.Fall || this.AnimState == AnimTag.Fly)
			{
				num *= this.FallRayMultiplier;
			}
			if (Physics.Raycast(this.fall_Point, -this.T_Up, out this.FallHit, num, this.GroundLayer))
			{
				if (this.debug)
				{
					Debug.DrawRay(this.fall_Point, -this.T_Up * num, Color.magenta);
					MalbersTools.DebugPlane(this.FallHit.point, 0.1f, Color.magenta, true);
				}
				if (Vector3.Angle(this.FallHit.normal, Vector3.up) * (float)((Vector3.Dot(this.T_ForwardNoY, this.FallHit.normal) > 0f) ? 1 : -1) > this.maxAngleSlope || (!this.frontray && !this.backray))
				{
					this.fall = true;
					return;
				}
				this.fall = false;
				if (this.AnimState == AnimTag.Fly && this.Land)
				{
					this.SetFly(false);
					this.IsInAir = false;
					this.groundSpeed = this.LastGroundSpeed;
				}
				if (this.AnimState == AnimTag.SwimJump)
				{
					this.Swim = false;
					return;
				}
			}
			else
			{
				this.fall = true;
				if (this.debug)
				{
					MalbersTools.DebugPlane(this.fall_Point + -this.T_Up * num, 0.1f, Color.gray, true);
					Debug.DrawRay(this.fall_Point, -this.T_Up * num, Color.gray);
				}
			}
		}

		// Token: 0x06003620 RID: 13856 RVA: 0x001B1D5C File Offset: 0x001AFF5C
		protected virtual bool IsFallingBackwards(float ammount)
		{
			if (this.FrameCounter % this.FallRayInterval != 0)
			{
				return false;
			}
			RaycastHit raycastHit = default(RaycastHit);
			Vector3 a = this.Pivot_Hip ? this.Pivot_Hip.transform.position : (this._transform.position + new Vector3(0f, this._Height, 0f));
			float num = this.Pivot_Hip ? (this.Pivot_Hip.multiplier * this.FallRayMultiplier) : this.FallRayMultiplier;
			Vector3 vector = a + this.T_Forward * -1f * ammount;
			if (this.debug)
			{
				Debug.DrawRay(vector, -this.T_Up * num * this.scaleFactor, Color.white);
			}
			if (Physics.Raycast(vector, -this.T_Up, out raycastHit, this.scaleFactor * num, this.GroundLayer))
			{
				return (double)raycastHit.normal.y < 0.6;
			}
			return !this.swim && this.movementAxis.z < 0f;
		}

		// Token: 0x06003621 RID: 13857 RVA: 0x001B1E9C File Offset: 0x001B009C
		protected virtual void MovementSystem(float s1 = 1f, float s2 = 2f, float s3 = 3f)
		{
			float num = this.groundSpeed;
			float num2 = 1f + this.currentSpeed.lerpRotation;
			float num3 = 1f + this.currentSpeed.lerpPosition;
			num = ((this.swim || this.underwater) ? 1f : num);
			if (this.Shift && this.UseShift)
			{
				num += 1f;
			}
			if (!this.Fly && !this.Swim && !this.IsJumping)
			{
				if (this.SlowSlopes && (double)this.slope >= 0.5 && num > 1f)
				{
					num -= 1f;
				}
				if (this.slope >= 1f)
				{
					num = 0f;
					num3 = 10f;
				}
			}
			if (this.Fly || this.Underwater)
			{
				this.YAxisMovement(this.upDownSmoothness, Time.deltaTime);
			}
			if (this.movementAxis.z < 0f && !this.swim && !this.Fly && !this.fall && this.IsFallingBackwards(this.BackFallRayDistance))
			{
				num = 0f;
				num3 = 10f;
			}
			this.vertical = Mathf.Lerp(this.vertical, this.movementAxis.z * num, Time.deltaTime * num3);
			this.horizontal = Mathf.Lerp(this.horizontal, this.movementAxis.x * (float)((this.Shift && this.UseShift) ? 2 : 1), Time.deltaTime * num2);
			if (Mathf.Abs(this.horizontal) > 0.1f || Mathf.Abs(this.vertical) > 0.2f)
			{
				this.stand = false;
			}
			else
			{
				this.stand = true;
			}
			if (!this.MovementReleased)
			{
				this.stand = false;
			}
			if (this.jump || this.damaged || this.stun || this.fall || this.swim || this.fly || this.isInAir || (this.tired >= this.GotoSleep && this.GotoSleep != 0))
			{
				this.stand = false;
			}
			if (this.tired >= this.GotoSleep)
			{
				this.tired = 0;
			}
			if (!this.stand)
			{
				this.tired = 0;
			}
			if (!this.swim && !this.fly)
			{
				this.movementAxis.y = 0f;
			}
		}

		// Token: 0x06003622 RID: 13858 RVA: 0x001B2100 File Offset: 0x001B0300
		private void FixedUpdate()
		{
			if (this.fly || this.underwater)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (this.swim && this.AnimState != AnimTag.SwimJump)
			{
				this.YFix = (this.Waterlevel - this._Height + this.waterLine - this._transform.position.y) * fixedDeltaTime * 5f;
			}
			this.FixPosition(fixedDeltaTime);
			this.UpdatePlatformMovement(false);
			this.FixedDeltaPos.y = this.FixedDeltaPos.y + this.YFix;
			this._transform.position += this.FixedDeltaPos;
			this.YFix = 0f;
			this.FixedDeltaPos = Vector3.zero;
		}

		// Token: 0x06003623 RID: 13859 RVA: 0x001B21C4 File Offset: 0x001B03C4
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			this.UpdateSettings();
			this.AdditionalSpeed(deltaTime);
			this.AdditionalTurn(deltaTime);
			this.RayCasting();
			this.Swimming(deltaTime);
			this.FixRotation(deltaTime);
			this.UpdatePlatformMovement(true);
			this.Falling();
		}

		// Token: 0x06003624 RID: 13860 RVA: 0x001B220C File Offset: 0x001B040C
		public virtual void UpdateSettings()
		{
			this.CurrentAnimState = this.Anim.GetCurrentAnimatorStateInfo(0).tagHash;
			this.NextAnimState = this.Anim.GetNextAnimatorStateInfo(0).tagHash;
			this.StateTime = this.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if (this.LastAnimationTag != this.AnimState)
			{
				this.LastAnimationTag = this.AnimState;
			}
			this.T_Up = this._transform.up;
			this.T_Right = this._transform.right;
			this.T_Forward = this._transform.forward;
			this.FrameCounter++;
			this.FrameCounter %= 100000;
		}

		// Token: 0x06003625 RID: 13861 RVA: 0x001B22D4 File Offset: 0x001B04D4
		private void LateUpdate()
		{
			this.MovementSystem(this.movementS1, this.movementS2, this.movementS3);
			this.LinkingAnimator();
		}

		// Token: 0x06003626 RID: 13862 RVA: 0x001B22F4 File Offset: 0x001B04F4
		private void OnAnimatorMove()
		{
			if (Time.timeScale <= 1E-45f)
			{
				return;
			}
			if (this.Anim.applyRootMotion && Time.deltaTime > 0f)
			{
				this._RigidBody.velocity = (this.Anim.deltaPosition + this.DeltaPosition) / Time.deltaTime;
			}
			this._transform.rotation *= this.Anim.deltaRotation * this.DeltaRotation;
			this.DeltaPosition = Vector3.zero;
			this.DeltaRotation = Quaternion.identity;
			this.LastPosition = this._transform.position;
		}

		// Token: 0x06003627 RID: 13863 RVA: 0x001B23A5 File Offset: 0x001B05A5
		protected virtual void OnAnimationStateEnter(int animTag)
		{
			if (animTag == AnimTag.Locomotion || animTag == AnimTag.Idle)
			{
				this.IsInAir = false;
				this.Anim.applyRootMotion = true;
			}
			if (animTag == AnimTag.Swim)
			{
				this.Anim.applyRootMotion = true;
			}
		}

		// Token: 0x06003628 RID: 13864 RVA: 0x001B23DE File Offset: 0x001B05DE
		private void OnEnable()
		{
			if (Animal.Animals == null)
			{
				Animal.Animals = new List<Animal>();
			}
			Animal.Animals.Add(this);
		}

		// Token: 0x06003629 RID: 13865 RVA: 0x001B23FC File Offset: 0x001B05FC
		private void OnDisable()
		{
			Animal.Animals.Remove(this);
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x0600362A RID: 13866 RVA: 0x001B240C File Offset: 0x001B060C
		public Vector3 T_ForwardNoY
		{
			get
			{
				return new Vector3(this.T_Forward.x, 0f, this.T_Forward.z).normalized;
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x0600362B RID: 13867 RVA: 0x001B2441 File Offset: 0x001B0641
		public Rigidbody _RigidBody
		{
			get
			{
				if (this._rigidbody == null)
				{
					this._rigidbody = base.GetComponentInChildren<Rigidbody>();
				}
				return this._rigidbody;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600362D RID: 13869 RVA: 0x001B246C File Offset: 0x001B066C
		// (set) Token: 0x0600362C RID: 13868 RVA: 0x001B2463 File Offset: 0x001B0663
		public virtual float GroundSpeed
		{
			get
			{
				return this.groundSpeed;
			}
			set
			{
				this.groundSpeed = value;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x0600362F RID: 13871 RVA: 0x001B247D File Offset: 0x001B067D
		// (set) Token: 0x0600362E RID: 13870 RVA: 0x001B2474 File Offset: 0x001B0674
		public virtual float Speed
		{
			get
			{
				return this.vertical;
			}
			set
			{
				this.vertical = value;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06003630 RID: 13872 RVA: 0x001B2488 File Offset: 0x001B0688
		public float Slope
		{
			get
			{
				this.slope = 0f;
				if (this.pivot_Chest && this.pivot_Hip)
				{
					float num = Vector3.Angle(this.SurfaceNormal, this.UpVector);
					float num2 = (float)((this.pivot_Chest.Y > this.pivot_Hip.Y) ? 1 : -1);
					this.slope = num / this.maxAngleSlope * (float)((num2 <= 0f) ? -1 : 1);
					return this.slope;
				}
				return 0f;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06003632 RID: 13874 RVA: 0x001B2536 File Offset: 0x001B0736
		// (set) Token: 0x06003631 RID: 13873 RVA: 0x001B2518 File Offset: 0x001B0718
		public virtual bool MovementReleased
		{
			get
			{
				return this.movementReleased;
			}
			private set
			{
				if (this.movementReleased != value)
				{
					this.movementReleased = value;
					this.OnMovementReleased.Invoke(value);
				}
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06003634 RID: 13876 RVA: 0x001B25CF File Offset: 0x001B07CF
		// (set) Token: 0x06003633 RID: 13875 RVA: 0x001B2540 File Offset: 0x001B0740
		public virtual bool Swim
		{
			get
			{
				return this.swim;
			}
			set
			{
				if (this.swim != value && Time.time - this.swimChanged >= 0.8f)
				{
					this.swim = value;
					this.swimChanged = Time.time;
					this.currentSpeed = this.swimSpeed;
					if (this.swim)
					{
						this.fall = (this.isInAir = (this.fly = false));
						this.OnSwim.Invoke();
						this._RigidBody.constraints = Animal.Still_Constraints;
						this.currentSpeed = this.swimSpeed;
					}
				}
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06003635 RID: 13877 RVA: 0x001B25D7 File Offset: 0x001B07D7
		public float Direction
		{
			get
			{
				return this.horizontal;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06003637 RID: 13879 RVA: 0x001B25E8 File Offset: 0x001B07E8
		// (set) Token: 0x06003636 RID: 13878 RVA: 0x001B25DF File Offset: 0x001B07DF
		public int Loops
		{
			get
			{
				return this.loops;
			}
			set
			{
				this.loops = value;
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06003639 RID: 13881 RVA: 0x001B25F9 File Offset: 0x001B07F9
		// (set) Token: 0x06003638 RID: 13880 RVA: 0x001B25F0 File Offset: 0x001B07F0
		public int IDInt
		{
			get
			{
				return this.idInt;
			}
			set
			{
				this.idInt = value;
			}
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x0600363B RID: 13883 RVA: 0x001B260A File Offset: 0x001B080A
		// (set) Token: 0x0600363A RID: 13882 RVA: 0x001B2601 File Offset: 0x001B0801
		public float IDFloat
		{
			get
			{
				return this.idfloat;
			}
			set
			{
				this.idfloat = value;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x0600363D RID: 13885 RVA: 0x001B261B File Offset: 0x001B081B
		// (set) Token: 0x0600363C RID: 13884 RVA: 0x001B2612 File Offset: 0x001B0812
		public int Tired
		{
			get
			{
				return this.tired;
			}
			set
			{
				this.tired = value;
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x0600363E RID: 13886 RVA: 0x001B2623 File Offset: 0x001B0823
		public bool IsInWater
		{
			get
			{
				return this.isInWater;
			}
		}

		// Token: 0x170002DE RID: 734
		// (set) Token: 0x0600363F RID: 13887 RVA: 0x001B262B File Offset: 0x001B082B
		public bool SpeedUp
		{
			set
			{
				if (value)
				{
					if (this.groundSpeed == this.movementS1)
					{
						this.Speed2 = true;
						return;
					}
					if (this.groundSpeed == this.movementS2)
					{
						this.Speed3 = true;
					}
				}
			}
		}

		// Token: 0x170002DF RID: 735
		// (set) Token: 0x06003640 RID: 13888 RVA: 0x001B265B File Offset: 0x001B085B
		public bool SpeedDown
		{
			set
			{
				if (value)
				{
					if (this.groundSpeed == this.movementS3)
					{
						this.Speed2 = true;
						return;
					}
					if (this.groundSpeed == this.movementS2)
					{
						this.Speed1 = true;
					}
				}
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06003641 RID: 13889 RVA: 0x001B268B File Offset: 0x001B088B
		// (set) Token: 0x06003642 RID: 13890 RVA: 0x001B2694 File Offset: 0x001B0894
		public bool Speed1
		{
			get
			{
				return this.speed1;
			}
			set
			{
				if (value)
				{
					this.speed1 = value;
					this.speed2 = (this.speed3 = false);
					this.groundSpeed = this.movementS1;
				}
			}
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06003643 RID: 13891 RVA: 0x001B26C7 File Offset: 0x001B08C7
		// (set) Token: 0x06003644 RID: 13892 RVA: 0x001B26D0 File Offset: 0x001B08D0
		public bool Speed2
		{
			get
			{
				return this.speed2;
			}
			set
			{
				if (value)
				{
					this.speed2 = value;
					this.speed1 = (this.speed3 = false);
					this.groundSpeed = this.movementS2;
				}
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06003645 RID: 13893 RVA: 0x001B2703 File Offset: 0x001B0903
		// (set) Token: 0x06003646 RID: 13894 RVA: 0x001B270C File Offset: 0x001B090C
		public bool Speed3
		{
			get
			{
				return this.speed3;
			}
			set
			{
				if (value)
				{
					this.speed3 = value;
					this.speed2 = (this.speed1 = false);
					this.groundSpeed = this.movementS3;
				}
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06003647 RID: 13895 RVA: 0x001B273F File Offset: 0x001B093F
		// (set) Token: 0x06003648 RID: 13896 RVA: 0x001B2747 File Offset: 0x001B0947
		public bool Jump
		{
			get
			{
				return this.jump;
			}
			set
			{
				this.jump = value;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06003649 RID: 13897 RVA: 0x001B2750 File Offset: 0x001B0950
		// (set) Token: 0x0600364A RID: 13898 RVA: 0x001B2758 File Offset: 0x001B0958
		public bool Underwater
		{
			get
			{
				return this.underwater;
			}
			set
			{
				if (this.CanGoUnderWater)
				{
					this.underwater = value;
				}
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x0600364B RID: 13899 RVA: 0x001B2769 File Offset: 0x001B0969
		// (set) Token: 0x0600364C RID: 13900 RVA: 0x001B2771 File Offset: 0x001B0971
		public bool Shift
		{
			get
			{
				return this.shift;
			}
			set
			{
				this.shift = value;
			}
		}

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x0600364D RID: 13901 RVA: 0x001B277A File Offset: 0x001B097A
		// (set) Token: 0x0600364E RID: 13902 RVA: 0x001B2782 File Offset: 0x001B0982
		public bool Down
		{
			get
			{
				return this.down;
			}
			set
			{
				this.down = value;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x0600364F RID: 13903 RVA: 0x001B278B File Offset: 0x001B098B
		// (set) Token: 0x06003650 RID: 13904 RVA: 0x001B2793 File Offset: 0x001B0993
		public bool Up
		{
			get
			{
				return this.up;
			}
			set
			{
				this.up = value;
			}
		}

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06003651 RID: 13905 RVA: 0x001B279C File Offset: 0x001B099C
		// (set) Token: 0x06003652 RID: 13906 RVA: 0x001B27A4 File Offset: 0x001B09A4
		public bool Dodge
		{
			get
			{
				return this.dodge;
			}
			set
			{
				this.dodge = value;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06003653 RID: 13907 RVA: 0x001B27AD File Offset: 0x001B09AD
		// (set) Token: 0x06003654 RID: 13908 RVA: 0x001B27B5 File Offset: 0x001B09B5
		public bool Damaged
		{
			get
			{
				return this.damaged;
			}
			set
			{
				this.damaged = value;
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06003655 RID: 13909 RVA: 0x001B27BE File Offset: 0x001B09BE
		// (set) Token: 0x06003656 RID: 13910 RVA: 0x001B27D8 File Offset: 0x001B09D8
		public bool Fly
		{
			get
			{
				if (!this.canFly)
				{
					this.fly = false;
				}
				return this.fly;
			}
			set
			{
				if (!this.canFly)
				{
					return;
				}
				if (value)
				{
					this.fly = !this.fly;
					if (this.fly)
					{
						this._RigidBody.useGravity = false;
						this.LastGroundSpeed = this.groundSpeed;
						this.groundSpeed = 1f;
						this.IsInAir = true;
						this.currentSpeed = this.flySpeed;
						Quaternion rotation = Quaternion.FromToRotation(this.T_Up, this.UpVector) * this._transform.rotation;
						base.StartCoroutine(MalbersTools.AlignTransformsC(this._transform, rotation, 0.3f, null));
					}
					else
					{
						this.groundSpeed = this.LastGroundSpeed;
					}
					this.OnFly.Invoke(this.fly);
				}
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06003657 RID: 13911 RVA: 0x001B289B File Offset: 0x001B0A9B
		// (set) Token: 0x06003658 RID: 13912 RVA: 0x001B28A4 File Offset: 0x001B0AA4
		public bool Death
		{
			get
			{
				return this.death;
			}
			set
			{
				this.death = value;
				if (this.death)
				{
					this.Anim.SetTrigger(Hash.Death);
					this.Anim.SetBool(Hash.Attack1, false);
					if (this.hasAttack2)
					{
						this.Anim.SetBool(Hash.Attack2, false);
					}
					this.Anim.SetBool(Hash.Action, false);
					this.OnDeathE.Invoke();
					if (Animal.Animals.Count > 0)
					{
						Animal.Animals.Remove(this);
					}
				}
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06003659 RID: 13913 RVA: 0x001B292F File Offset: 0x001B0B2F
		// (set) Token: 0x0600365A RID: 13914 RVA: 0x001B2938 File Offset: 0x001B0B38
		public bool Attack1
		{
			get
			{
				return this.attack1;
			}
			set
			{
				if (!value)
				{
					this.attack1 = value;
				}
				if (this.death)
				{
					return;
				}
				if (this.AnimState == AnimTag.Action)
				{
					return;
				}
				if (!this.isAttacking && value)
				{
					this.attack1 = value;
					this.IDInt = this.activeAttack;
					if (this.IDInt <= 0)
					{
						this.SetIntIDRandom(this.TotalAttacks);
					}
					this.OnAttack.Invoke();
				}
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600365B RID: 13915 RVA: 0x001B29A4 File Offset: 0x001B0BA4
		// (set) Token: 0x0600365C RID: 13916 RVA: 0x001B29AC File Offset: 0x001B0BAC
		public bool Attack2
		{
			get
			{
				return this.attack2;
			}
			set
			{
				if (this.death)
				{
					return;
				}
				if (value && this.AnimState == AnimTag.Action)
				{
					return;
				}
				this.attack2 = value;
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x0600365D RID: 13917 RVA: 0x001B29CF File Offset: 0x001B0BCF
		// (set) Token: 0x0600365E RID: 13918 RVA: 0x001B29D7 File Offset: 0x001B0BD7
		public bool Stun
		{
			get
			{
				return this.stun;
			}
			set
			{
				this.stun = value;
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x0600365F RID: 13919 RVA: 0x001B29E0 File Offset: 0x001B0BE0
		// (set) Token: 0x06003660 RID: 13920 RVA: 0x001B29E8 File Offset: 0x001B0BE8
		public bool Action
		{
			get
			{
				return this.action;
			}
			set
			{
				if (this.ActionID == -1)
				{
					return;
				}
				if (this.death)
				{
					return;
				}
				if (this.action != value)
				{
					this.action = value;
					if (this.action)
					{
						base.StartCoroutine(this.ToggleAction());
					}
				}
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06003661 RID: 13921 RVA: 0x001B2A22 File Offset: 0x001B0C22
		// (set) Token: 0x06003662 RID: 13922 RVA: 0x001B2A2A File Offset: 0x001B0C2A
		public int ActionID
		{
			get
			{
				return this.actionID;
			}
			set
			{
				this.actionID = value;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06003663 RID: 13923 RVA: 0x001B2A33 File Offset: 0x001B0C33
		// (set) Token: 0x06003664 RID: 13924 RVA: 0x001B2A3B File Offset: 0x001B0C3B
		public bool IsAttacking
		{
			get
			{
				return this.isAttacking;
			}
			set
			{
				this.isAttacking = value;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06003666 RID: 13926 RVA: 0x001B2A52 File Offset: 0x001B0C52
		// (set) Token: 0x06003665 RID: 13925 RVA: 0x001B2A44 File Offset: 0x001B0C44
		public bool RootMotion
		{
			get
			{
				return this.Anim.applyRootMotion;
			}
			set
			{
				this.Anim.applyRootMotion = value;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06003667 RID: 13927 RVA: 0x001B2A5F File Offset: 0x001B0C5F
		// (set) Token: 0x06003668 RID: 13928 RVA: 0x001B2A67 File Offset: 0x001B0C67
		public bool IsInAir
		{
			get
			{
				return this.isInAir;
			}
			set
			{
				this.isInAir = value;
				this.StillConstraints(!this.IsInAir);
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06003669 RID: 13929 RVA: 0x001B2A7F File Offset: 0x001B0C7F
		public bool Stand
		{
			get
			{
				return this.stand;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x0600366A RID: 13930 RVA: 0x001B2A87 File Offset: 0x001B0C87
		// (set) Token: 0x0600366B RID: 13931 RVA: 0x001B2A8F File Offset: 0x001B0C8F
		public Vector3 HitDirection
		{
			get
			{
				return this._hitDirection;
			}
			set
			{
				this._hitDirection = value;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x0600366C RID: 13932 RVA: 0x001B2A98 File Offset: 0x001B0C98
		public float ScaleFactor
		{
			get
			{
				return this.scaleFactor;
			}
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x0600366D RID: 13933 RVA: 0x001B2AA0 File Offset: 0x001B0CA0
		public Pivots Pivot_Hip
		{
			get
			{
				return this.pivot_Hip;
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x0600366E RID: 13934 RVA: 0x001B2AA8 File Offset: 0x001B0CA8
		public Pivots Pivot_Chest
		{
			get
			{
				return this.pivot_Chest;
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x0600366F RID: 13935 RVA: 0x001B2AB0 File Offset: 0x001B0CB0
		public int AnimState
		{
			get
			{
				if (this.NextAnimState == 0)
				{
					return this.CurrentAnimState;
				}
				return this.NextAnimState;
			}
		}

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06003671 RID: 13937 RVA: 0x001B2ADC File Offset: 0x001B0CDC
		// (set) Token: 0x06003670 RID: 13936 RVA: 0x001B2AC7 File Offset: 0x001B0CC7
		public int LastAnimationTag
		{
			get
			{
				return this.lastAnimTag;
			}
			private set
			{
				this.lastAnimTag = value;
				this.OnAnimationChange.Invoke(value);
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06003672 RID: 13938 RVA: 0x001B2AE4 File Offset: 0x001B0CE4
		public Animator Anim
		{
			get
			{
				if (this.anim == null)
				{
					this.anim = base.GetComponent<Animator>();
				}
				return this.anim;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06003673 RID: 13939 RVA: 0x001B2B06 File Offset: 0x001B0D06
		public Vector3 Pivot_fall
		{
			get
			{
				return this.fall_Point;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06003674 RID: 13940 RVA: 0x001B2B10 File Offset: 0x001B0D10
		public float Pivot_Multiplier
		{
			get
			{
				return (this.Pivot_Chest ? this.Pivot_Chest.multiplier : (this.Pivot_Hip ? this.Pivot_Hip.multiplier : 1f)) * this.scaleFactor;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06003675 RID: 13941 RVA: 0x001B2B60 File Offset: 0x001B0D60
		public Vector3 Main_Pivot_Point
		{
			get
			{
				if (this.pivot_Chest)
				{
					return this.pivot_Chest.GetPivot;
				}
				if (this.pivot_Hip)
				{
					return this.pivot_Hip.GetPivot;
				}
				Vector3 position = this._transform.position;
				position.y += this.height;
				return position;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06003676 RID: 13942 RVA: 0x001B2BBD File Offset: 0x001B0DBD
		public static RigidbodyConstraints Still_Constraints
		{
			get
			{
				return (RigidbodyConstraints)116;
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06003677 RID: 13943 RVA: 0x001B2BC1 File Offset: 0x001B0DC1
		// (set) Token: 0x06003678 RID: 13944 RVA: 0x001B2BC9 File Offset: 0x001B0DC9
		public Vector3 MovementAxis
		{
			get
			{
				return this.movementAxis;
			}
			set
			{
				this.movementAxis = value;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06003679 RID: 13945 RVA: 0x001B2BD2 File Offset: 0x001B0DD2
		// (set) Token: 0x0600367A RID: 13946 RVA: 0x001B2BDF File Offset: 0x001B0DDF
		public float MovementForward
		{
			get
			{
				return this.movementAxis.z;
			}
			set
			{
				this.movementAxis.z = value;
				this.MovementReleased = (value == 0f);
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x0600367B RID: 13947 RVA: 0x001B2BFB File Offset: 0x001B0DFB
		// (set) Token: 0x0600367C RID: 13948 RVA: 0x001B2C08 File Offset: 0x001B0E08
		public float MovementRight
		{
			get
			{
				return this.movementAxis.x;
			}
			set
			{
				this.movementAxis.x = value;
				this.MovementReleased = (value == 0f);
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x0600367D RID: 13949 RVA: 0x001B2C24 File Offset: 0x001B0E24
		// (set) Token: 0x0600367E RID: 13950 RVA: 0x001B2C31 File Offset: 0x001B0E31
		public float MovementUp
		{
			get
			{
				return this.movementAxis.y;
			}
			set
			{
				this.movementAxis.y = value;
				this.MovementReleased = (value == 0f);
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x0600367F RID: 13951 RVA: 0x001B2C50 File Offset: 0x001B0E50
		public Vector3 SurfaceNormal
		{
			get
			{
				if (!this.pivot_Hip || !(this.hit_Hip.transform != null))
				{
					return Vector3.up;
				}
				if (this.Pivot_Chest && this.hit_Chest.transform != null)
				{
					Vector3 normalized = (this.hit_Chest.point - this.hit_Hip.point).normalized;
					Vector3 normalized2 = Vector3.Cross(this.UpVector, normalized).normalized;
					return Vector3.Cross(normalized, normalized2).normalized;
				}
				return this.hit_Hip.normal;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06003680 RID: 13952 RVA: 0x001B2CFA File Offset: 0x001B0EFA
		// (set) Token: 0x06003681 RID: 13953 RVA: 0x001B2D02 File Offset: 0x001B0F02
		public Renderer AnimalMesh
		{
			get
			{
				return this.animalMesh;
			}
			set
			{
				this.animalMesh = value;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06003682 RID: 13954 RVA: 0x001B2D0B File Offset: 0x001B0F0B
		// (set) Token: 0x06003683 RID: 13955 RVA: 0x001B2D13 File Offset: 0x001B0F13
		public float Waterlevel
		{
			get
			{
				return this.waterLevel;
			}
			set
			{
				this.waterLevel = value;
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06003684 RID: 13956 RVA: 0x001B2D1C File Offset: 0x001B0F1C
		public bool DirectionalMovement
		{
			get
			{
				return this.directionalMovement;
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06003685 RID: 13957 RVA: 0x001B2D24 File Offset: 0x001B0F24
		// (set) Token: 0x06003686 RID: 13958 RVA: 0x001B2D2C File Offset: 0x001B0F2C
		public Vector3 RawDirection
		{
			get
			{
				return this.rawDirection;
			}
			set
			{
				this.rawDirection = value;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06003687 RID: 13959 RVA: 0x001B2D35 File Offset: 0x001B0F35
		// (set) Token: 0x06003688 RID: 13960 RVA: 0x001B2D3D File Offset: 0x001B0F3D
		public bool Land
		{
			get
			{
				return this.land;
			}
			set
			{
				this.land = value;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06003689 RID: 13961 RVA: 0x001B2D46 File Offset: 0x001B0F46
		// (set) Token: 0x0600368A RID: 13962 RVA: 0x001B2D4E File Offset: 0x001B0F4E
		public float StateTime
		{
			get
			{
				return this.stateTime;
			}
			set
			{
				this.stateTime = value;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x0600368B RID: 13963 RVA: 0x001B2D57 File Offset: 0x001B0F57
		// (set) Token: 0x0600368C RID: 13964 RVA: 0x001B2D60 File Offset: 0x001B0F60
		public int Stance
		{
			get
			{
				return this.stance;
			}
			set
			{
				if (this.stance != value)
				{
					this.lastStance = this.stance;
					this.stance = value;
					this.OnStanceChange.Invoke(value);
				}
				if (this.hasStance)
				{
					this.Anim.SetInteger(Hash.Stance, this.stance);
				}
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x0600368D RID: 13965 RVA: 0x001B2DB3 File Offset: 0x001B0FB3
		public int LastStance
		{
			get
			{
				return this.lastStance;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x0600368E RID: 13966 RVA: 0x001B2DBB File Offset: 0x001B0FBB
		// (set) Token: 0x0600368F RID: 13967 RVA: 0x001B2DC3 File Offset: 0x001B0FC3
		public bool UseShift
		{
			get
			{
				return this.useShift;
			}
			set
			{
				this.useShift = value;
			}
		}

		// Token: 0x04002586 RID: 9606
		private int ToogleAmount = 4;

		// Token: 0x04002588 RID: 9608
		private IEnumerator StunC;

		// Token: 0x04002589 RID: 9609
		public Dictionary<string, BoolEvent> Inputs;

		// Token: 0x0400258A RID: 9610
		private float YFix;

		// Token: 0x0400258B RID: 9611
		protected float FixDistance;

		// Token: 0x0400258C RID: 9612
		public static List<Animal> Animals;

		// Token: 0x0400258D RID: 9613
		protected Animator anim;

		// Token: 0x0400258E RID: 9614
		protected Rigidbody _rigidbody;

		// Token: 0x0400258F RID: 9615
		private Renderer animalMesh;

		// Token: 0x04002590 RID: 9616
		protected Vector3 movementAxis;

		// Token: 0x04002591 RID: 9617
		protected Vector3 rawDirection;

		// Token: 0x04002592 RID: 9618
		[HideInInspector]
		internal Vector3 T_Up;

		// Token: 0x04002593 RID: 9619
		[HideInInspector]
		internal Vector3 T_Right;

		// Token: 0x04002594 RID: 9620
		[HideInInspector]
		internal Vector3 T_Forward;

		// Token: 0x04002595 RID: 9621
		public static readonly float LowWaterLevel = -1000f;

		// Token: 0x04002596 RID: 9622
		protected bool speed1;

		// Token: 0x04002597 RID: 9623
		protected bool speed2;

		// Token: 0x04002598 RID: 9624
		protected bool speed3;

		// Token: 0x04002599 RID: 9625
		protected bool movementReleased;

		// Token: 0x0400259A RID: 9626
		protected bool jump;

		// Token: 0x0400259B RID: 9627
		protected bool fly;

		// Token: 0x0400259C RID: 9628
		protected bool shift;

		// Token: 0x0400259D RID: 9629
		protected bool down;

		// Token: 0x0400259E RID: 9630
		protected bool up;

		// Token: 0x0400259F RID: 9631
		protected bool dodge;

		// Token: 0x040025A0 RID: 9632
		protected bool fall;

		// Token: 0x040025A1 RID: 9633
		protected bool fallback;

		// Token: 0x040025A2 RID: 9634
		protected bool isInWater;

		// Token: 0x040025A3 RID: 9635
		protected bool isInAir;

		// Token: 0x040025A4 RID: 9636
		protected bool swim;

		// Token: 0x040025A5 RID: 9637
		protected bool underwater;

		// Token: 0x040025A6 RID: 9638
		protected bool stun;

		// Token: 0x040025A7 RID: 9639
		protected bool action;

		// Token: 0x040025A8 RID: 9640
		protected bool stand = true;

		// Token: 0x040025A9 RID: 9641
		protected bool backray;

		// Token: 0x040025AA RID: 9642
		protected bool frontray;

		// Token: 0x040025AB RID: 9643
		private float waterLevel = -10f;

		// Token: 0x040025AC RID: 9644
		private bool directionalMovement;

		// Token: 0x040025AD RID: 9645
		protected float vertical;

		// Token: 0x040025AE RID: 9646
		protected float horizontal;

		// Token: 0x040025AF RID: 9647
		private float stateTime;

		// Token: 0x040025B0 RID: 9648
		protected float groundSpeed = 1f;

		// Token: 0x040025B1 RID: 9649
		protected float slope;

		// Token: 0x040025B2 RID: 9650
		protected float idfloat;

		// Token: 0x040025B3 RID: 9651
		protected float _Height;

		// Token: 0x040025B4 RID: 9652
		protected int idInt;

		// Token: 0x040025B5 RID: 9653
		protected int actionID = -1;

		// Token: 0x040025B6 RID: 9654
		protected int tired;

		// Token: 0x040025B7 RID: 9655
		protected int loops = 1;

		// Token: 0x040025B8 RID: 9656
		public int animalTypeID;

		// Token: 0x040025B9 RID: 9657
		[SerializeField]
		private int stance;

		// Token: 0x040025BA RID: 9658
		internal Vector3 FixedDeltaPos = Vector3.zero;

		// Token: 0x040025BB RID: 9659
		internal Vector3 DeltaPosition = Vector3.zero;

		// Token: 0x040025BC RID: 9660
		internal Vector3 LastPosition = Vector3.zero;

		// Token: 0x040025BD RID: 9661
		internal Quaternion DeltaRotation = Quaternion.identity;

		// Token: 0x040025BE RID: 9662
		public bool JumpPress;

		// Token: 0x040025BF RID: 9663
		public float JumpHeightMultiplier;

		// Token: 0x040025C0 RID: 9664
		public float AirForwardMultiplier;

		// Token: 0x040025C1 RID: 9665
		public LayerMask GroundLayer = 1;

		// Token: 0x040025C2 RID: 9666
		public Animal.Ground StartSpeed = Animal.Ground.walk;

		// Token: 0x040025C3 RID: 9667
		public float height = 1f;

		// Token: 0x040025C4 RID: 9668
		internal Speeds currentSpeed;

		// Token: 0x040025C5 RID: 9669
		public Speeds walkSpeed = new Speeds(8f, 4f, 6f);

		// Token: 0x040025C6 RID: 9670
		public Speeds trotSpeed = new Speeds(4f, 4f, 6f);

		// Token: 0x040025C7 RID: 9671
		public Speeds runSpeed = new Speeds(2f, 4f, 6f);

		// Token: 0x040025C8 RID: 9672
		protected float CurrentAnimatorSpeed = 1f;

		// Token: 0x040025C9 RID: 9673
		protected Transform platform;

		// Token: 0x040025CA RID: 9674
		protected Vector3 platform_Pos;

		// Token: 0x040025CB RID: 9675
		protected float platform_formAngle;

		// Token: 0x040025CC RID: 9676
		public string m_Vertical = "Vertical";

		// Token: 0x040025CD RID: 9677
		public string m_Horizontal = "Horizontal";

		// Token: 0x040025CE RID: 9678
		public string m_UpDown = "UpDown";

		// Token: 0x040025CF RID: 9679
		public string m_Stand = "Stand";

		// Token: 0x040025D0 RID: 9680
		public string m_Jump = "_Jump";

		// Token: 0x040025D1 RID: 9681
		public string m_Fly = "Fly";

		// Token: 0x040025D2 RID: 9682
		public string m_Fall = "Fall";

		// Token: 0x040025D3 RID: 9683
		public string m_Attack1 = "Attack1";

		// Token: 0x040025D4 RID: 9684
		public string m_Attack2 = "Attack2";

		// Token: 0x040025D5 RID: 9685
		public string m_Stunned = "Stunned";

		// Token: 0x040025D6 RID: 9686
		public string m_Damaged = "Damaged";

		// Token: 0x040025D7 RID: 9687
		public string m_Shift = "Shift";

		// Token: 0x040025D8 RID: 9688
		public string m_Death = "Death";

		// Token: 0x040025D9 RID: 9689
		public string m_Dodge = "Dodge";

		// Token: 0x040025DA RID: 9690
		public string m_Underwater = "Underwater";

		// Token: 0x040025DB RID: 9691
		public string m_Swim = "Swim";

		// Token: 0x040025DC RID: 9692
		public string m_Action = "Action";

		// Token: 0x040025DD RID: 9693
		public string m_IDAction = "IDAction";

		// Token: 0x040025DE RID: 9694
		public string m_IDFloat = "IDFloat";

		// Token: 0x040025DF RID: 9695
		public string m_IDInt = "IDInt";

		// Token: 0x040025E0 RID: 9696
		public string m_Slope = "Slope";

		// Token: 0x040025E1 RID: 9697
		public string m_Type = "Type";

		// Token: 0x040025E2 RID: 9698
		public string m_SpeedMultiplier = "SpeedMultiplier";

		// Token: 0x040025E3 RID: 9699
		public string m_StateTime = "StateTime";

		// Token: 0x040025E4 RID: 9700
		public string m_Stance = "Stance";

		// Token: 0x040025E5 RID: 9701
		internal int hash_Vertical;

		// Token: 0x040025E6 RID: 9702
		internal int hash_Horizontal;

		// Token: 0x040025E7 RID: 9703
		internal int hash_UpDown;

		// Token: 0x040025E8 RID: 9704
		internal int hash_Stand;

		// Token: 0x040025E9 RID: 9705
		internal int hash_Jump;

		// Token: 0x040025EA RID: 9706
		internal int hash_Dodge;

		// Token: 0x040025EB RID: 9707
		internal int hash_Fall;

		// Token: 0x040025EC RID: 9708
		internal int hash_Type;

		// Token: 0x040025ED RID: 9709
		internal int hash_Slope;

		// Token: 0x040025EE RID: 9710
		internal int hash_Shift;

		// Token: 0x040025EF RID: 9711
		internal int hash_Fly;

		// Token: 0x040025F0 RID: 9712
		internal int hash_Attack1;

		// Token: 0x040025F1 RID: 9713
		internal int hash_Attack2;

		// Token: 0x040025F2 RID: 9714
		internal int hash_Death;

		// Token: 0x040025F3 RID: 9715
		internal int hash_Damaged;

		// Token: 0x040025F4 RID: 9716
		internal int hash_Stunned;

		// Token: 0x040025F5 RID: 9717
		internal int hash_IDInt;

		// Token: 0x040025F6 RID: 9718
		internal int hash_IDFloat;

		// Token: 0x040025F7 RID: 9719
		internal int hash_Swim;

		// Token: 0x040025F8 RID: 9720
		internal int hash_Underwater;

		// Token: 0x040025F9 RID: 9721
		internal int hash_IDAction;

		// Token: 0x040025FA RID: 9722
		internal int hash_Action;

		// Token: 0x040025FB RID: 9723
		internal int hash_StateTime;

		// Token: 0x040025FC RID: 9724
		internal int hash_Stance;

		// Token: 0x040025FD RID: 9725
		[HideInInspector]
		private bool hasFly;

		// Token: 0x040025FE RID: 9726
		[HideInInspector]
		private bool hasDodge;

		// Token: 0x040025FF RID: 9727
		[HideInInspector]
		private bool hasSlope;

		// Token: 0x04002600 RID: 9728
		[HideInInspector]
		private bool hasStun;

		// Token: 0x04002601 RID: 9729
		[HideInInspector]
		private bool hasAttack2;

		// Token: 0x04002602 RID: 9730
		[HideInInspector]
		private bool hasUpDown;

		// Token: 0x04002603 RID: 9731
		[HideInInspector]
		private bool hasUnderwater;

		// Token: 0x04002604 RID: 9732
		[HideInInspector]
		private bool hasSwim;

		// Token: 0x04002605 RID: 9733
		[HideInInspector]
		private bool hasStateTime;

		// Token: 0x04002606 RID: 9734
		[HideInInspector]
		private bool hasStance;

		// Token: 0x04002607 RID: 9735
		public float airRotation = 100f;

		// Token: 0x04002608 RID: 9736
		public bool AirControl;

		// Token: 0x04002609 RID: 9737
		public float airMaxSpeed = 1f;

		// Token: 0x0400260A RID: 9738
		public float airSmoothness = 2f;

		// Token: 0x0400260B RID: 9739
		internal Vector3 AirControlDir;

		// Token: 0x0400260C RID: 9740
		public float movementS1 = 1f;

		// Token: 0x0400260D RID: 9741
		public float movementS2 = 2f;

		// Token: 0x0400260E RID: 9742
		public float movementS3 = 3f;

		// Token: 0x0400260F RID: 9743
		[Range(0f, 90f)]
		public float maxAngleSlope = 45f;

		// Token: 0x04002610 RID: 9744
		public bool SlowSlopes = true;

		// Token: 0x04002611 RID: 9745
		[Range(0f, 100f)]
		public int GotoSleep;

		// Token: 0x04002612 RID: 9746
		public float SnapToGround = 20f;

		// Token: 0x04002613 RID: 9747
		public float AlingToGround = 30f;

		// Token: 0x04002614 RID: 9748
		public float FallRayDistance = 0.1f;

		// Token: 0x04002615 RID: 9749
		public float BackFallRayDistance = 0.5f;

		// Token: 0x04002616 RID: 9750
		public float FallRayMultiplier = 1f;

		// Token: 0x04002617 RID: 9751
		public bool SmoothVertical = true;

		// Token: 0x04002618 RID: 9752
		public bool IgnoreYDir;

		// Token: 0x04002619 RID: 9753
		public float TurnMultiplier = 100f;

		// Token: 0x0400261A RID: 9754
		public float waterLine;

		// Token: 0x0400261B RID: 9755
		public Speeds swimSpeed = new Speeds(8f, 4f, 6f);

		// Token: 0x0400261C RID: 9756
		internal int WaterLayer;

		// Token: 0x0400261D RID: 9757
		public bool canSwim = true;

		// Token: 0x0400261E RID: 9758
		public bool CanGoUnderWater;

		// Token: 0x0400261F RID: 9759
		[Range(0f, 90f)]
		public float bank;

		// Token: 0x04002620 RID: 9760
		public Speeds underWaterSpeed = new Speeds(8f, 4f, 6f);

		// Token: 0x04002621 RID: 9761
		public Speeds flySpeed;

		// Token: 0x04002622 RID: 9762
		public bool StartFlying;

		// Token: 0x04002623 RID: 9763
		public bool canFly;

		// Token: 0x04002624 RID: 9764
		public bool land = true;

		// Token: 0x04002625 RID: 9765
		protected float LastGroundSpeed;

		// Token: 0x04002626 RID: 9766
		public bool LockUp;

		// Token: 0x04002627 RID: 9767
		public float life = 100f;

		// Token: 0x04002628 RID: 9768
		public float defense;

		// Token: 0x04002629 RID: 9769
		public float damageDelay = 0.5f;

		// Token: 0x0400262A RID: 9770
		public float damageInterrupt = 0.2f;

		// Token: 0x0400262B RID: 9771
		public int TotalAttacks = 3;

		// Token: 0x0400262C RID: 9772
		public int activeAttack = -1;

		// Token: 0x0400262D RID: 9773
		public float attackStrength = 10f;

		// Token: 0x0400262E RID: 9774
		public float attackDelay = 0.5f;

		// Token: 0x0400262F RID: 9775
		public bool inmune;

		// Token: 0x04002630 RID: 9776
		protected bool attack1;

		// Token: 0x04002631 RID: 9777
		protected bool attack2;

		// Token: 0x04002632 RID: 9778
		protected bool isAttacking;

		// Token: 0x04002633 RID: 9779
		protected bool isTakingDamage;

		// Token: 0x04002634 RID: 9780
		protected bool damaged;

		// Token: 0x04002635 RID: 9781
		protected bool death;

		// Token: 0x04002636 RID: 9782
		protected List<AttackTrigger> Attack_Triggers;

		// Token: 0x04002637 RID: 9783
		public float animatorSpeed = 1f;

		// Token: 0x04002638 RID: 9784
		public float upDownSmoothness = 2f;

		// Token: 0x04002639 RID: 9785
		public bool debug = true;

		// Token: 0x0400263A RID: 9786
		protected RaycastHit hit_Hip;

		// Token: 0x0400263B RID: 9787
		protected RaycastHit hit_Chest;

		// Token: 0x0400263C RID: 9788
		protected RaycastHit WaterHitCenter;

		// Token: 0x0400263D RID: 9789
		protected RaycastHit FallHit;

		// Token: 0x0400263E RID: 9790
		protected Vector3 fall_Point;

		// Token: 0x0400263F RID: 9791
		protected Vector3 _hitDirection;

		// Token: 0x04002640 RID: 9792
		protected Vector3 UpVector = Vector3.up;

		// Token: 0x04002641 RID: 9793
		protected float scaleFactor = 1f;

		// Token: 0x04002642 RID: 9794
		protected List<Pivots> pivots = new List<Pivots>();

		// Token: 0x04002643 RID: 9795
		protected Pivots pivot_Chest;

		// Token: 0x04002644 RID: 9796
		protected Pivots pivot_Hip;

		// Token: 0x04002645 RID: 9797
		protected Pivots pivot_Water;

		// Token: 0x04002646 RID: 9798
		public int PivotsRayInterval = 1;

		// Token: 0x04002647 RID: 9799
		public int FallRayInterval = 3;

		// Token: 0x04002648 RID: 9800
		public int WaterRayInterval = 5;

		// Token: 0x04002649 RID: 9801
		public UnityEvent OnJump;

		// Token: 0x0400264A RID: 9802
		public UnityEvent OnAttack;

		// Token: 0x0400264B RID: 9803
		public FloatEvent OnGetDamaged;

		// Token: 0x0400264C RID: 9804
		public UnityEvent OnDeathE;

		// Token: 0x0400264D RID: 9805
		public UnityEvent OnAction;

		// Token: 0x0400264E RID: 9806
		public UnityEvent OnSwim;

		// Token: 0x0400264F RID: 9807
		public BoolEvent OnFly;

		// Token: 0x04002650 RID: 9808
		public UnityEvent OnUnderWater;

		// Token: 0x04002651 RID: 9809
		public IntEvent OnAnimationChange;

		// Token: 0x04002652 RID: 9810
		public IntEvent OnStanceChange;

		// Token: 0x04002653 RID: 9811
		public UnityEvent OnSyncAnimator;

		// Token: 0x04002654 RID: 9812
		private static RaycastHit NULLRayCast = default(RaycastHit);

		// Token: 0x04002655 RID: 9813
		private List<Collider> _col_ = new List<Collider>();

		// Token: 0x04002656 RID: 9814
		[HideInInspector]
		public int FrameCounter;

		// Token: 0x04002657 RID: 9815
		public BoolEvent OnMovementReleased = new BoolEvent();

		// Token: 0x04002658 RID: 9816
		private float swimChanged;

		// Token: 0x04002659 RID: 9817
		[SerializeField]
		private bool useShift = true;

		// Token: 0x0400265A RID: 9818
		public int CurrentAnimState;

		// Token: 0x0400265B RID: 9819
		private int lastAnimTag;

		// Token: 0x0400265C RID: 9820
		private Transform _transform;

		// Token: 0x0400265D RID: 9821
		public int NextAnimState;

		// Token: 0x0400265E RID: 9822
		private int lastStance;

		// Token: 0x0400265F RID: 9823
		[HideInInspector]
		public bool EditorGeneral = true;

		// Token: 0x04002660 RID: 9824
		[HideInInspector]
		public bool EditorGround = true;

		// Token: 0x04002661 RID: 9825
		[HideInInspector]
		public bool EditorWater = true;

		// Token: 0x04002662 RID: 9826
		[HideInInspector]
		public bool EditorAir = true;

		// Token: 0x04002663 RID: 9827
		[HideInInspector]
		public bool EditorAdvanced = true;

		// Token: 0x04002664 RID: 9828
		[HideInInspector]
		public bool EditorAirControl = true;

		// Token: 0x04002665 RID: 9829
		[HideInInspector]
		public bool EditorAttributes = true;

		// Token: 0x04002666 RID: 9830
		[HideInInspector]
		public bool EditorEvents;

		// Token: 0x04002667 RID: 9831
		[HideInInspector]
		public bool EditorAnimatorParameters;

		// Token: 0x02000910 RID: 2320
		public enum Ground
		{
			// Token: 0x04004249 RID: 16969
			walk = 1,
			// Token: 0x0400424A RID: 16970
			trot,
			// Token: 0x0400424B RID: 16971
			run
		}
	}
}
