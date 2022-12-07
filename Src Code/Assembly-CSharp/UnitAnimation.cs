using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000315 RID: 789
[ExecuteInEditMode]
public class UnitAnimation : MonoBehaviour
{
	// Token: 0x17000267 RID: 615
	// (get) Token: 0x0600315A RID: 12634 RVA: 0x0018EDA3 File Offset: 0x0018CFA3
	// (set) Token: 0x0600315B RID: 12635 RVA: 0x0018EDAB File Offset: 0x0018CFAB
	public UnitAnimation.State CurrentState { get; private set; }

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x0600315C RID: 12636 RVA: 0x0018EDB4 File Offset: 0x0018CFB4
	// (set) Token: 0x0600315D RID: 12637 RVA: 0x0018EDBC File Offset: 0x0018CFBC
	public UnitAnimation.Mode CurrentMode { get; private set; }

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x0600315E RID: 12638 RVA: 0x0018EDC5 File Offset: 0x0018CFC5
	// (set) Token: 0x0600315F RID: 12639 RVA: 0x0018EDCD File Offset: 0x0018CFCD
	public float CrossFadeDuration
	{
		get
		{
			return this.crossFadeDuration;
		}
		set
		{
			this.crossFadeDuration = Mathf.Clamp(value, 0f, 1f);
		}
	}

	// Token: 0x06003160 RID: 12640 RVA: 0x0018EDE8 File Offset: 0x0018CFE8
	private void OnEnable()
	{
		this.animatorMecanim = base.GetComponent<Animator>();
		if (this.animatorMecanim != null)
		{
			this.animatorMecanim.fireEvents = false;
			this.animatorMecanim.Play("Idle");
			this.animatorMecanim.Update(0.1f);
		}
	}

	// Token: 0x06003161 RID: 12641 RVA: 0x0018EE3C File Offset: 0x0018D03C
	private void Awake()
	{
		Component component = base.GetComponent<Animation>();
		if (component != null)
		{
			this.CurrentMode = UnitAnimation.Mode.Legacy;
			base.enabled = true;
			this.BuildForLegacy(component as Animation);
		}
		component = base.GetComponent<Animator>();
		if (component != null)
		{
			this.CurrentMode = UnitAnimation.Mode.Mecanim;
			if (Application.isPlaying)
			{
				base.enabled = false;
			}
			this.BuildForMechanim(component as Animator);
		}
		this.SetState(UnitAnimation.State.Idle, WrapMode.Loop, Random.value, false, -1f);
	}

	// Token: 0x06003162 RID: 12642 RVA: 0x0018EEB8 File Offset: 0x0018D0B8
	public void SetEditorModeState(UnitAnimation.State state)
	{
		if (Application.isPlaying)
		{
			return;
		}
		this.inEditorState = state;
		this.animatorMecanim = base.GetComponent<Animator>();
		if (this.animatorMecanim != null)
		{
			this.animatorMecanim.Play(this.ToMechanimState(this.inEditorState));
			this.animatorMecanim.Update(0.1f);
		}
	}

	// Token: 0x06003163 RID: 12643 RVA: 0x0018EF15 File Offset: 0x0018D115
	private void OnValidate()
	{
		this.SetEditorModeState(this.inEditorState);
	}

	// Token: 0x06003164 RID: 12644 RVA: 0x0018EF24 File Offset: 0x0018D124
	private void Update()
	{
		if (!Application.isPlaying)
		{
			if (this.animatorMecanim != null)
			{
				this.animatorMecanim.Update(Time.deltaTime);
				return;
			}
		}
		else
		{
			if (this.CurrentMode == UnitAnimation.Mode.None)
			{
				return;
			}
			if (this.CurrentMode == UnitAnimation.Mode.Legacy)
			{
				this.UpdateLegacy();
			}
			else if (this.CurrentMode == UnitAnimation.Mode.Mecanim)
			{
				this.UpdateFromMecanim();
			}
			if (this.force_refresh)
			{
				this.Refresh();
			}
		}
	}

	// Token: 0x06003165 RID: 12645 RVA: 0x0018EF8E File Offset: 0x0018D18E
	public void SetState(UnitAnimation.State state, WrapMode mode, float offset = -1f, bool force_refresh = false, float normalized_time = -1f)
	{
		if (this.CurrentState == state && !force_refresh)
		{
			return;
		}
		if (this.CurrentMode == UnitAnimation.Mode.Mecanim)
		{
			this.SetStateMechanim(state, mode, offset, normalized_time);
			return;
		}
		if (this.CurrentMode == UnitAnimation.Mode.Legacy)
		{
			this.SetStateLegacy(state, mode, offset);
		}
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x0018EFC8 File Offset: 0x0018D1C8
	public void Refresh()
	{
		if (!this.force_refresh)
		{
			base.enabled = true;
			this.force_refresh = true;
			return;
		}
		if (!base.gameObject.activeInHierarchy || this.animatorMecanim == null || !this.animatorMecanim.isActiveAndEnabled)
		{
			return;
		}
		base.enabled = false;
		this.force_refresh = false;
		float normalized_time = -1f;
		if (this.CurrentState == UnitAnimation.State.Death)
		{
			normalized_time = 1f;
		}
		this.SetState(this.CurrentState, this.currentWrapMode, -1f, true, normalized_time);
	}

	// Token: 0x06003167 RID: 12647 RVA: 0x0018F052 File Offset: 0x0018D252
	public void SetCrossFadeDuration(float duration)
	{
		if (duration < 0f || duration > 1f)
		{
			Debug.LogWarning("Un-reasonable duration detected. Please set duration between 0 and 1.");
			return;
		}
		this.crossFadeDuration = duration;
	}

	// Token: 0x06003168 RID: 12648 RVA: 0x0018F078 File Offset: 0x0018D278
	public void SetStateLegacy(UnitAnimation.State state, WrapMode mode, float offset = -1f)
	{
		if (this.stateData == null)
		{
			return;
		}
		if (!this.stateData.ContainsKey(state))
		{
			return;
		}
		if (!this.stateData[state].IsValid())
		{
			Debug.LogFormat("State {0} is not usable", new object[]
			{
				state
			});
			return;
		}
		if (mode == WrapMode.Once)
		{
			this.currentAnimationState = this.stateData[state].GetRandomAnimation(mode);
			this.currentAnimationState.time = ((offset != -1f) ? (offset * this.currentAnimationState.length) : 0f);
			this.animatorLegacy.CrossFade(this.currentAnimationState.name, this.crossFadeDuration);
		}
		else
		{
			this.currentAnimationState = this.stateData[state].GetRandomAnimation(mode);
			this.currentAnimationState.time = ((offset != -1f) ? (offset * this.currentAnimationState.length) : 0f);
			this.animatorLegacy.CrossFade(this.currentAnimationState.name, this.crossFadeDuration);
		}
		this.currentWrapMode = mode;
		this.CurrentState = state;
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x0018F194 File Offset: 0x0018D394
	public void SetSpeed(float speed)
	{
		this.globalSpeed = speed;
		if (this.CurrentMode == UnitAnimation.Mode.Legacy)
		{
			if (this.stateData == null)
			{
				return;
			}
			for (int i = 0; i < UnitAnimation.setupDataMap.Count; i++)
			{
				this.stateData[UnitAnimation.setupDataMap[i].state].SetSpeed(speed);
			}
		}
		if (this.CurrentMode == UnitAnimation.Mode.Mecanim)
		{
			int j = 1;
			int num = UnitAnimation.stateToSpeedModidiferMap.Length;
			while (j < num)
			{
				this.animatorMecanim.SetFloat(UnitAnimation.stateToSpeedModidiferMap[j], speed);
				j++;
			}
		}
	}

	// Token: 0x0600316A RID: 12650 RVA: 0x0018F220 File Offset: 0x0018D420
	public void SetSpeed(UnitAnimation.State state, float speed)
	{
		if (this.CurrentMode == UnitAnimation.Mode.Legacy)
		{
			if (this.stateData == null)
			{
				return;
			}
			if (!this.stateData.ContainsKey(state))
			{
				return;
			}
			this.stateData[state].SetSpeed(speed);
		}
		if (this.CurrentMode == UnitAnimation.Mode.Mecanim)
		{
			this.animatorMecanim.SetFloat(UnitAnimation.stateToSpeedModidiferMap[(int)state], speed);
		}
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x0018F27C File Offset: 0x0018D47C
	public float GetSpeed()
	{
		return this.globalSpeed;
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x0018F284 File Offset: 0x0018D484
	private void BuildForLegacy(Animation comp)
	{
		this.animatorLegacy = comp;
		this.stateData = new Dictionary<UnitAnimation.State, UnitAnimation.StateData>();
		foreach (UnitAnimation.SateSetupData sateSetupData in UnitAnimation.setupDataMap)
		{
			this.stateData.Add(sateSetupData.state, new UnitAnimation.StateData
			{
				name = sateSetupData.state,
				animations = new List<AnimationState>(),
				rotationMode = sateSetupData.rotationMode,
				speedModifer = 1f
			});
		}
		foreach (object obj in this.animatorLegacy)
		{
			AnimationState animationState = (AnimationState)obj;
			foreach (UnitAnimation.SateSetupData sateSetupData2 in UnitAnimation.setupDataMap)
			{
				if (!string.IsNullOrEmpty(sateSetupData2.keyword) && animationState.name.ToLowerInvariant().Contains(sateSetupData2.keyword))
				{
					this.stateData[sateSetupData2.state].animations.Add(animationState);
				}
				if (!string.IsNullOrEmpty(sateSetupData2.keywordDefaultState) && animationState.name.ToLowerInvariant().Contains(sateSetupData2.keywordDefaultState))
				{
					this.stateData[sateSetupData2.state].defaultAnimation = animationState;
				}
			}
		}
		this.SetState(UnitAnimation.State.Idle, WrapMode.Loop, -1f, false, -1f);
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x0018F448 File Offset: 0x0018D648
	private void UpdateLegacy()
	{
		if (this.currentWrapMode == WrapMode.Loop && this.currentAnimationState.normalizedTime > 0.95f)
		{
			this.currentAnimationState = this.stateData[this.CurrentState].GetRandomAnimation(WrapMode.Loop);
			this.currentAnimationState.time = 0f;
			this.animatorLegacy.CrossFade(this.currentAnimationState.name, this.crossFadeDuration);
		}
		if (this.currentWrapMode == WrapMode.Once && this.currentAnimationState.normalizedTime > 0.95f)
		{
			this.SetState(UnitAnimation.State.Ready, WrapMode.Loop, -1f, false, -1f);
		}
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x0018F4E7 File Offset: 0x0018D6E7
	private void BuildForMechanim(Animator animator)
	{
		this.animatorMecanim = animator;
		this.animatorMecanim.fireEvents = false;
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x000023FD File Offset: 0x000005FD
	private void UpdateFromMecanim()
	{
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x0018F4FC File Offset: 0x0018D6FC
	public void SetStateMechanim(UnitAnimation.State state, WrapMode mode, float offset = -1f, float normalized_time = -1f)
	{
		if (base.gameObject.activeInHierarchy)
		{
			int num = this.ToMechanimState(state);
			if (normalized_time < 0f)
			{
				this.animatorMecanim.CrossFadeInFixedTime(num, this.crossFadeDuration, 0, (offset != -1f) ? Mathf.Min(offset, 1f - this.crossFadeDuration) : 0f);
			}
			else
			{
				this.animatorMecanim.Play(num, 0, normalized_time);
			}
		}
		this.currentWrapMode = mode;
		this.CurrentState = state;
	}

	// Token: 0x06003171 RID: 12657 RVA: 0x0018F57C File Offset: 0x0018D77C
	private int ToMechanimState(UnitAnimation.State state)
	{
		int result = this.hashStateIdle;
		switch (state)
		{
		case UnitAnimation.State.None:
		case UnitAnimation.State.Idle:
			return this.hashStateIdle;
		case UnitAnimation.State.Ready:
			return this.hashStateReady;
		case UnitAnimation.State.Move:
			return this.hashStateWalk;
		case UnitAnimation.State.Trot:
		case UnitAnimation.State.Sprint:
			break;
		case UnitAnimation.State.Run:
			return this.hashStateRun;
		case UnitAnimation.State.Charge:
			return this.hashStateCharge;
		case UnitAnimation.State.Attack:
			return this.hashStateAttack;
		case UnitAnimation.State.SpecialAttack:
			return this.hashStateSpecialAttack;
		case UnitAnimation.State.Death:
			return this.hashStateDeath_01;
		case UnitAnimation.State.PushUps:
			return this.hashStatePushUps;
		default:
			if (state == UnitAnimation.State.Hit)
			{
				return this.hashStateHit;
			}
			if (state == UnitAnimation.State.HitSpecial)
			{
				return this.hashStateHitSpecial;
			}
			break;
		}
		Debug.LogFormat("State {0} is not impelemented", new object[]
		{
			state
		});
		return result;
	}

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06003172 RID: 12658 RVA: 0x0018F64F File Offset: 0x0018D84F
	public static int IndexBakedCount
	{
		get
		{
			return 43 - UnitAnimation.no_bake.Length;
		}
	}

	// Token: 0x06003173 RID: 12659 RVA: 0x0018F65C File Offset: 0x0018D85C
	public static UnitAnimation.State IdxToState(UnitAnimation.Index state)
	{
		switch (state)
		{
		case UnitAnimation.Index.Idle:
		case UnitAnimation.Index.IdleAlt_01:
		case UnitAnimation.Index.IdleAlt_02:
		case UnitAnimation.Index.IdleAlt_03:
			return UnitAnimation.State.Idle;
		case UnitAnimation.Index.Ready:
		case UnitAnimation.Index.ReadyAlt_01:
		case UnitAnimation.Index.ReadyAlt_02:
			return UnitAnimation.State.Ready;
		case UnitAnimation.Index.Attack:
		case UnitAnimation.Index.Attack_02:
			return UnitAnimation.State.Attack;
		case UnitAnimation.Index.Death:
		case UnitAnimation.Index.Death_02:
		case UnitAnimation.Index.Death_03:
			return UnitAnimation.State.Death;
		case UnitAnimation.Index.Ladder_Climb_Down:
			return UnitAnimation.State.LadderClimbDown;
		case UnitAnimation.Index.Ladder_Climb_Up:
			return UnitAnimation.State.LadderClimb;
		case UnitAnimation.Index.Ladder_Idle:
			return UnitAnimation.State.LadderIdle;
		case UnitAnimation.Index.SpecialReady:
		case UnitAnimation.Index.SpecialReadyAlt_01:
		case UnitAnimation.Index.SpecialReadyAlt_02:
			return UnitAnimation.State.SpecialReady;
		case UnitAnimation.Index.SpecialDeath_01:
		case UnitAnimation.Index.SpecialDeath_02:
			return UnitAnimation.State.SpecialDeath;
		case UnitAnimation.Index.Idle_Cheer:
		case UnitAnimation.Index.Idle_Cheer_01:
		case UnitAnimation.Index.Idle_Cheer_02:
			return UnitAnimation.State.Cheer;
		case UnitAnimation.Index.Ready_To_Surrender_01:
			return UnitAnimation.State.Ready_To_Surrender_01;
		case UnitAnimation.Index.Ready_To_Surrender_02:
			return UnitAnimation.State.Ready_To_Surrender_02;
		case UnitAnimation.Index.Idle_Surrender_01:
			return UnitAnimation.State.Idle_Surrender_01;
		case UnitAnimation.Index.Idle_Surrender_02:
			return UnitAnimation.State.Idle_Surrender_02;
		case UnitAnimation.Index.ShieldSetup:
			return UnitAnimation.State.ShieldSetup;
		case UnitAnimation.Index.ShieldPack:
			return UnitAnimation.State.ShieldPack;
		case UnitAnimation.Index.Hit:
			return UnitAnimation.State.Hit;
		case UnitAnimation.Index.HitSpecial:
			return UnitAnimation.State.HitSpecial;
		case UnitAnimation.Index.SpecialAttackMovingFront:
			return UnitAnimation.State.SpecialAttackMovingFront;
		case UnitAnimation.Index.SpecialAttackMovingLeft:
			return UnitAnimation.State.SpecialAttackMovingLeft;
		case UnitAnimation.Index.SpecialAttackMovingRight:
			return UnitAnimation.State.SpecialAttackMovingRight;
		case UnitAnimation.Index.SpecialAttackMovingRear:
			return UnitAnimation.State.SpecialAttackMovingRear;
		}
		return (UnitAnimation.State)state;
	}

	// Token: 0x040020F9 RID: 8441
	public bool force_refresh;

	// Token: 0x040020FA RID: 8442
	public UnitAnimation.State inEditorState = UnitAnimation.State.Idle;

	// Token: 0x040020FB RID: 8443
	private static List<UnitAnimation.SateSetupData> setupDataMap = new List<UnitAnimation.SateSetupData>
	{
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.Attack,
			keyword = "_attack_",
			rotationMode = UnitAnimation.ClipRotationMode.Random
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.SpecialAttack,
			keyword = "_special_",
			rotationMode = UnitAnimation.ClipRotationMode.Random
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.Move,
			keyword = "_walk",
			rotationMode = UnitAnimation.ClipRotationMode.None
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.Run,
			keyword = "_run",
			rotationMode = UnitAnimation.ClipRotationMode.None
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.Charge,
			keyword = "_charge",
			rotationMode = UnitAnimation.ClipRotationMode.None
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.Idle,
			keywordDefaultState = "_idle_",
			keyword = "_idlealt_",
			rotationMode = UnitAnimation.ClipRotationMode.RandomAlt
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.Ready,
			keywordDefaultState = "_ready_",
			keyword = "_readyalt_",
			rotationMode = UnitAnimation.ClipRotationMode.RandomAlt
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.Death,
			keyword = "_death_",
			rotationMode = UnitAnimation.ClipRotationMode.Random
		},
		new UnitAnimation.SateSetupData
		{
			state = UnitAnimation.State.PushUps,
			keyword = "_pushups",
			rotationMode = UnitAnimation.ClipRotationMode.Random
		}
	};

	// Token: 0x040020FC RID: 8444
	private static string[] stateToSpeedModidiferMap = new string[]
	{
		"NoneSpeedMod",
		"IdleSpeedMod",
		"ReadySpeedMod",
		"MoveSpeedMod",
		"RunSpeedMod",
		"ChargeSpeedMod",
		"AttackSpeedMod",
		"SpecialAttackSpeedMod",
		"DeathSpeedMod",
		"PushUpsSpeedMod"
	};

	// Token: 0x040020FD RID: 8445
	private const float ANIMSPEED_MIN = 0.25f;

	// Token: 0x040020FE RID: 8446
	private const float ANIMSPEED_MAX = 20f;

	// Token: 0x040020FF RID: 8447
	private const float DEFAULT_TRANSITION_DUR = 0.2f;

	// Token: 0x04002101 RID: 8449
	private Dictionary<UnitAnimation.State, UnitAnimation.StateData> stateData;

	// Token: 0x04002102 RID: 8450
	private WrapMode currentWrapMode;

	// Token: 0x04002103 RID: 8451
	private AnimationState currentAnimationState;

	// Token: 0x04002105 RID: 8453
	private float crossFadeDuration = 0.2f;

	// Token: 0x04002106 RID: 8454
	private float globalSpeed = 1f;

	// Token: 0x04002107 RID: 8455
	private Animation animatorLegacy;

	// Token: 0x04002108 RID: 8456
	private Animator animatorMecanim;

	// Token: 0x04002109 RID: 8457
	private int hashStateIdle = Animator.StringToHash("Idle");

	// Token: 0x0400210A RID: 8458
	private int hashStateReady = Animator.StringToHash("Ready");

	// Token: 0x0400210B RID: 8459
	private int hashStateWalk = Animator.StringToHash("Move");

	// Token: 0x0400210C RID: 8460
	private int hashStateRun = Animator.StringToHash("Run");

	// Token: 0x0400210D RID: 8461
	private int hashStateCharge = Animator.StringToHash("Charge");

	// Token: 0x0400210E RID: 8462
	private int hashStateAttack = Animator.StringToHash("Attack");

	// Token: 0x0400210F RID: 8463
	private int hashStateSpecialAttack = Animator.StringToHash("SpecialAttack");

	// Token: 0x04002110 RID: 8464
	private int hashStateDeath_01 = Animator.StringToHash("Death_01");

	// Token: 0x04002111 RID: 8465
	private int hashStatePushUps = Animator.StringToHash("PushUps");

	// Token: 0x04002112 RID: 8466
	private int hashStateHit = Animator.StringToHash("Hit");

	// Token: 0x04002113 RID: 8467
	private int hashStateHitSpecial = Animator.StringToHash("HitSpecial");

	// Token: 0x04002114 RID: 8468
	public static readonly UnitAnimation.Index[] no_bake = new UnitAnimation.Index[]
	{
		UnitAnimation.Index.Death_02,
		UnitAnimation.Index.Death_03,
		UnitAnimation.Index.SpecialDeath_01,
		UnitAnimation.Index.SpecialDeath_02,
		UnitAnimation.Index.Idle_Surrender_01,
		UnitAnimation.Index.Idle_Surrender_02,
		UnitAnimation.Index.Ready_To_Surrender_01,
		UnitAnimation.Index.Ready_To_Surrender_02,
		UnitAnimation.Index.ShieldPack,
		UnitAnimation.Index.ShieldSetup,
		UnitAnimation.Index.Ladder_Climb_Down,
		UnitAnimation.Index.Ladder_Idle
	};

	// Token: 0x02000878 RID: 2168
	private class StateData
	{
		// Token: 0x0600514E RID: 20814 RVA: 0x0023C71D File Offset: 0x0023A91D
		public bool IsValid()
		{
			return !(this.defaultAnimation == null) || this.animations.Count != 0;
		}

		// Token: 0x0600514F RID: 20815 RVA: 0x0023C740 File Offset: 0x0023A940
		public AnimationState GetRandomAnimation(WrapMode wrapMode)
		{
			AnimationState animationState = null;
			if (this.rotationMode == UnitAnimation.ClipRotationMode.None)
			{
				if (this.defaultAnimation != null)
				{
					animationState = this.defaultAnimation;
				}
				else
				{
					animationState = this.animations[0];
				}
			}
			if (this.rotationMode == UnitAnimation.ClipRotationMode.RandomAlt)
			{
				if (this.defaultAnimation != null && this.animations.Count == 0)
				{
					animationState = this.defaultAnimation;
				}
				else if (this.defaultAnimation != null && Random.value < 0.8f)
				{
					animationState = this.defaultAnimation;
				}
				else
				{
					animationState = this.animations[Random.Range(0, this.animations.Count)];
				}
			}
			if (this.rotationMode == UnitAnimation.ClipRotationMode.Random)
			{
				float num = (float)(this.animations.Count + (this.defaultAnimation ? 1 : 0));
				if (this.defaultAnimation != null && Random.value > (num - 1f) / num)
				{
					animationState = this.defaultAnimation;
				}
				else
				{
					animationState = this.animations[Random.Range(0, this.animations.Count)];
				}
			}
			animationState.wrapMode = wrapMode;
			return animationState;
		}

		// Token: 0x06005150 RID: 20816 RVA: 0x0023C85C File Offset: 0x0023AA5C
		public void SetSpeed(float speed)
		{
			this.speedModifer = speed;
			if (this.defaultAnimation != null)
			{
				this.defaultAnimation.speed = speed;
			}
			for (int i = 0; i < this.animations.Count; i++)
			{
				this.animations[i].speed = speed;
			}
		}

		// Token: 0x04003F4F RID: 16207
		public UnitAnimation.State name;

		// Token: 0x04003F50 RID: 16208
		public AnimationState defaultAnimation;

		// Token: 0x04003F51 RID: 16209
		public List<AnimationState> animations;

		// Token: 0x04003F52 RID: 16210
		public UnitAnimation.ClipRotationMode rotationMode;

		// Token: 0x04003F53 RID: 16211
		public float speedModifer;
	}

	// Token: 0x02000879 RID: 2169
	private enum ClipRotationMode
	{
		// Token: 0x04003F55 RID: 16213
		None,
		// Token: 0x04003F56 RID: 16214
		Random,
		// Token: 0x04003F57 RID: 16215
		RandomAlt
	}

	// Token: 0x0200087A RID: 2170
	private struct SateSetupData
	{
		// Token: 0x04003F58 RID: 16216
		public UnitAnimation.State state;

		// Token: 0x04003F59 RID: 16217
		public string keywordDefaultState;

		// Token: 0x04003F5A RID: 16218
		public string keyword;

		// Token: 0x04003F5B RID: 16219
		public UnitAnimation.ClipRotationMode rotationMode;
	}

	// Token: 0x0200087B RID: 2171
	public enum Mode
	{
		// Token: 0x04003F5D RID: 16221
		None,
		// Token: 0x04003F5E RID: 16222
		Legacy,
		// Token: 0x04003F5F RID: 16223
		Mecanim,
		// Token: 0x04003F60 RID: 16224
		Cumstom
	}

	// Token: 0x0200087C RID: 2172
	public enum State
	{
		// Token: 0x04003F62 RID: 16226
		None,
		// Token: 0x04003F63 RID: 16227
		Idle,
		// Token: 0x04003F64 RID: 16228
		Ready,
		// Token: 0x04003F65 RID: 16229
		Move,
		// Token: 0x04003F66 RID: 16230
		Trot,
		// Token: 0x04003F67 RID: 16231
		Run,
		// Token: 0x04003F68 RID: 16232
		Charge,
		// Token: 0x04003F69 RID: 16233
		Sprint,
		// Token: 0x04003F6A RID: 16234
		Attack,
		// Token: 0x04003F6B RID: 16235
		SpecialAttack,
		// Token: 0x04003F6C RID: 16236
		Death,
		// Token: 0x04003F6D RID: 16237
		PushUps,
		// Token: 0x04003F6E RID: 16238
		SpecialReady,
		// Token: 0x04003F6F RID: 16239
		SpecialDeath,
		// Token: 0x04003F70 RID: 16240
		LadderClimb,
		// Token: 0x04003F71 RID: 16241
		LadderIdle,
		// Token: 0x04003F72 RID: 16242
		LadderClimbDown,
		// Token: 0x04003F73 RID: 16243
		Ready_To_Surrender_01,
		// Token: 0x04003F74 RID: 16244
		Ready_To_Surrender_02,
		// Token: 0x04003F75 RID: 16245
		Idle_Surrender_01,
		// Token: 0x04003F76 RID: 16246
		Idle_Surrender_02,
		// Token: 0x04003F77 RID: 16247
		Cheer,
		// Token: 0x04003F78 RID: 16248
		ShieldSetup,
		// Token: 0x04003F79 RID: 16249
		ShieldPack,
		// Token: 0x04003F7A RID: 16250
		Hit,
		// Token: 0x04003F7B RID: 16251
		HitSpecial,
		// Token: 0x04003F7C RID: 16252
		SpecialAttackMovingFront,
		// Token: 0x04003F7D RID: 16253
		SpecialAttackMovingLeft,
		// Token: 0x04003F7E RID: 16254
		SpecialAttackMovingRight,
		// Token: 0x04003F7F RID: 16255
		SpecialAttackMovingRear,
		// Token: 0x04003F80 RID: 16256
		Count
	}

	// Token: 0x0200087D RID: 2173
	public enum Index
	{
		// Token: 0x04003F82 RID: 16258
		None,
		// Token: 0x04003F83 RID: 16259
		Idle,
		// Token: 0x04003F84 RID: 16260
		Ready,
		// Token: 0x04003F85 RID: 16261
		Move,
		// Token: 0x04003F86 RID: 16262
		Trot,
		// Token: 0x04003F87 RID: 16263
		Run,
		// Token: 0x04003F88 RID: 16264
		Charge,
		// Token: 0x04003F89 RID: 16265
		Sprint,
		// Token: 0x04003F8A RID: 16266
		Attack,
		// Token: 0x04003F8B RID: 16267
		SpecialAttack,
		// Token: 0x04003F8C RID: 16268
		Death,
		// Token: 0x04003F8D RID: 16269
		PushUps,
		// Token: 0x04003F8E RID: 16270
		IdleAlt_01,
		// Token: 0x04003F8F RID: 16271
		IdleAlt_02,
		// Token: 0x04003F90 RID: 16272
		IdleAlt_03,
		// Token: 0x04003F91 RID: 16273
		ReadyAlt_01,
		// Token: 0x04003F92 RID: 16274
		ReadyAlt_02,
		// Token: 0x04003F93 RID: 16275
		Attack_02,
		// Token: 0x04003F94 RID: 16276
		Death_02,
		// Token: 0x04003F95 RID: 16277
		Death_03,
		// Token: 0x04003F96 RID: 16278
		Ladder_Climb_Down,
		// Token: 0x04003F97 RID: 16279
		Ladder_Climb_Up,
		// Token: 0x04003F98 RID: 16280
		Ladder_Idle,
		// Token: 0x04003F99 RID: 16281
		SpecialReady,
		// Token: 0x04003F9A RID: 16282
		SpecialReadyAlt_01,
		// Token: 0x04003F9B RID: 16283
		SpecialReadyAlt_02,
		// Token: 0x04003F9C RID: 16284
		SpecialDeath_01,
		// Token: 0x04003F9D RID: 16285
		SpecialDeath_02,
		// Token: 0x04003F9E RID: 16286
		Idle_Cheer,
		// Token: 0x04003F9F RID: 16287
		Idle_Cheer_01,
		// Token: 0x04003FA0 RID: 16288
		Idle_Cheer_02,
		// Token: 0x04003FA1 RID: 16289
		Ready_To_Surrender_01,
		// Token: 0x04003FA2 RID: 16290
		Ready_To_Surrender_02,
		// Token: 0x04003FA3 RID: 16291
		Idle_Surrender_01,
		// Token: 0x04003FA4 RID: 16292
		Idle_Surrender_02,
		// Token: 0x04003FA5 RID: 16293
		ShieldSetup,
		// Token: 0x04003FA6 RID: 16294
		ShieldPack,
		// Token: 0x04003FA7 RID: 16295
		Hit,
		// Token: 0x04003FA8 RID: 16296
		HitSpecial,
		// Token: 0x04003FA9 RID: 16297
		SpecialAttackMovingFront,
		// Token: 0x04003FAA RID: 16298
		SpecialAttackMovingLeft,
		// Token: 0x04003FAB RID: 16299
		SpecialAttackMovingRight,
		// Token: 0x04003FAC RID: 16300
		SpecialAttackMovingRear,
		// Token: 0x04003FAD RID: 16301
		Count
	}
}
