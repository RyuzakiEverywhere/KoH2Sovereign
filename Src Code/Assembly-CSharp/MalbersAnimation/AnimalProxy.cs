using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003C3 RID: 963
	[CreateAssetMenu(menuName = "Malbers Animations/Animal Proxy")]
	public class AnimalProxy : ScriptableObject
	{
		// Token: 0x060036C3 RID: 14019 RVA: 0x001B3F60 File Offset: 0x001B2160
		private void OnEnable()
		{
			if (this.CleanAnimalOnEnable)
			{
				this.animal = null;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060036C4 RID: 14020 RVA: 0x001B3F71 File Offset: 0x001B2171
		public virtual Animal Animal
		{
			get
			{
				return this.animal;
			}
		}

		// Token: 0x17000314 RID: 788
		// (set) Token: 0x060036C5 RID: 14021 RVA: 0x001B3F79 File Offset: 0x001B2179
		public virtual float GroundSpeed
		{
			set
			{
				this.animal.GroundSpeed = value;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060036C7 RID: 14023 RVA: 0x001B3F95 File Offset: 0x001B2195
		// (set) Token: 0x060036C6 RID: 14022 RVA: 0x001B3F87 File Offset: 0x001B2187
		public virtual float Vertical
		{
			get
			{
				return this.animal.Speed;
			}
			set
			{
				this.animal.Speed = value;
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060036C9 RID: 14025 RVA: 0x001B3FB0 File Offset: 0x001B21B0
		// (set) Token: 0x060036C8 RID: 14024 RVA: 0x001B3FA2 File Offset: 0x001B21A2
		public int Loops
		{
			get
			{
				return this.animal.Loops;
			}
			set
			{
				this.animal.Loops = value;
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060036CB RID: 14027 RVA: 0x001B3FCB File Offset: 0x001B21CB
		// (set) Token: 0x060036CA RID: 14026 RVA: 0x001B3FBD File Offset: 0x001B21BD
		public int IDInt
		{
			get
			{
				return this.animal.IDInt;
			}
			set
			{
				this.animal.IDInt = value;
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060036CD RID: 14029 RVA: 0x001B3FE6 File Offset: 0x001B21E6
		// (set) Token: 0x060036CC RID: 14028 RVA: 0x001B3FD8 File Offset: 0x001B21D8
		public float IDFloat
		{
			get
			{
				return this.animal.IDFloat;
			}
			set
			{
				this.animal.IDFloat = value;
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060036CF RID: 14031 RVA: 0x001B4001 File Offset: 0x001B2201
		// (set) Token: 0x060036CE RID: 14030 RVA: 0x001B3FF3 File Offset: 0x001B21F3
		public int Tired
		{
			get
			{
				return this.animal.Tired;
			}
			set
			{
				this.animal.Tired = value;
			}
		}

		// Token: 0x1700031A RID: 794
		// (set) Token: 0x060036D0 RID: 14032 RVA: 0x001B400E File Offset: 0x001B220E
		public bool SpeedUp
		{
			set
			{
				this.animal.SpeedUp = value;
			}
		}

		// Token: 0x1700031B RID: 795
		// (set) Token: 0x060036D1 RID: 14033 RVA: 0x001B401C File Offset: 0x001B221C
		public bool SpeedDown
		{
			set
			{
				this.animal.SpeedDown = value;
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060036D3 RID: 14035 RVA: 0x001B4038 File Offset: 0x001B2238
		// (set) Token: 0x060036D2 RID: 14034 RVA: 0x001B402A File Offset: 0x001B222A
		public bool Speed1
		{
			get
			{
				return this.animal.Speed1;
			}
			set
			{
				this.animal.Speed1 = value;
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060036D5 RID: 14037 RVA: 0x001B4053 File Offset: 0x001B2253
		// (set) Token: 0x060036D4 RID: 14036 RVA: 0x001B4045 File Offset: 0x001B2245
		public bool Speed2
		{
			get
			{
				return this.animal.Speed2;
			}
			set
			{
				this.animal.Speed2 = value;
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060036D7 RID: 14039 RVA: 0x001B406E File Offset: 0x001B226E
		// (set) Token: 0x060036D6 RID: 14038 RVA: 0x001B4060 File Offset: 0x001B2260
		public bool Speed3
		{
			get
			{
				return this.animal.Speed3;
			}
			set
			{
				this.animal.Speed3 = value;
			}
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060036D9 RID: 14041 RVA: 0x001B4089 File Offset: 0x001B2289
		// (set) Token: 0x060036D8 RID: 14040 RVA: 0x001B407B File Offset: 0x001B227B
		public bool Jump
		{
			get
			{
				return this.animal.Jump;
			}
			set
			{
				this.animal.Jump = value;
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060036DB RID: 14043 RVA: 0x001B40A4 File Offset: 0x001B22A4
		// (set) Token: 0x060036DA RID: 14042 RVA: 0x001B4096 File Offset: 0x001B2296
		public bool Underwater
		{
			get
			{
				return this.animal.Underwater;
			}
			set
			{
				this.animal.Underwater = value;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060036DD RID: 14045 RVA: 0x001B40BF File Offset: 0x001B22BF
		// (set) Token: 0x060036DC RID: 14044 RVA: 0x001B40B1 File Offset: 0x001B22B1
		public bool UseShift
		{
			get
			{
				return this.animal.UseShift;
			}
			set
			{
				this.animal.UseShift = value;
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060036DF RID: 14047 RVA: 0x001B40DA File Offset: 0x001B22DA
		// (set) Token: 0x060036DE RID: 14046 RVA: 0x001B40CC File Offset: 0x001B22CC
		public bool Shift
		{
			get
			{
				return this.animal.Shift;
			}
			set
			{
				this.animal.Shift = value;
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060036E1 RID: 14049 RVA: 0x001B40F5 File Offset: 0x001B22F5
		// (set) Token: 0x060036E0 RID: 14048 RVA: 0x001B40E7 File Offset: 0x001B22E7
		public bool Down
		{
			get
			{
				return this.animal.Down;
			}
			set
			{
				this.animal.Down = value;
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060036E3 RID: 14051 RVA: 0x001B4110 File Offset: 0x001B2310
		// (set) Token: 0x060036E2 RID: 14050 RVA: 0x001B4102 File Offset: 0x001B2302
		public bool Up
		{
			get
			{
				return this.animal.Up;
			}
			set
			{
				this.animal.Up = value;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060036E5 RID: 14053 RVA: 0x001B412B File Offset: 0x001B232B
		// (set) Token: 0x060036E4 RID: 14052 RVA: 0x001B411D File Offset: 0x001B231D
		public bool Dodge
		{
			get
			{
				return this.animal.Dodge;
			}
			set
			{
				this.animal.Dodge = value;
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060036E7 RID: 14055 RVA: 0x001B412B File Offset: 0x001B232B
		// (set) Token: 0x060036E6 RID: 14054 RVA: 0x001B4138 File Offset: 0x001B2338
		public bool Damaged
		{
			get
			{
				return this.animal.Dodge;
			}
			set
			{
				this.animal.Damaged = value;
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060036E9 RID: 14057 RVA: 0x001B4154 File Offset: 0x001B2354
		// (set) Token: 0x060036E8 RID: 14056 RVA: 0x001B4146 File Offset: 0x001B2346
		public bool Fly
		{
			get
			{
				return this.animal.Fly;
			}
			set
			{
				this.animal.SetFly(value);
			}
		}

		// Token: 0x060036EA RID: 14058 RVA: 0x001B4161 File Offset: 0x001B2361
		public void ToogleFly()
		{
			this.animal.Fly = true;
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x060036EC RID: 14060 RVA: 0x001B417D File Offset: 0x001B237D
		// (set) Token: 0x060036EB RID: 14059 RVA: 0x001B416F File Offset: 0x001B236F
		public bool Death
		{
			get
			{
				return this.animal.Death;
			}
			set
			{
				this.animal.Death = value;
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x060036EE RID: 14062 RVA: 0x001B4198 File Offset: 0x001B2398
		// (set) Token: 0x060036ED RID: 14061 RVA: 0x001B418A File Offset: 0x001B238A
		public bool Attack1
		{
			get
			{
				return this.animal && this.animal.Attack1;
			}
			set
			{
				this.animal.Attack1 = value;
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x060036F0 RID: 14064 RVA: 0x001B41C2 File Offset: 0x001B23C2
		// (set) Token: 0x060036EF RID: 14063 RVA: 0x001B41B4 File Offset: 0x001B23B4
		public bool Attack2
		{
			get
			{
				return this.animal && this.animal.Attack2;
			}
			set
			{
				this.animal.Attack2 = value;
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x060036F2 RID: 14066 RVA: 0x001B41EC File Offset: 0x001B23EC
		// (set) Token: 0x060036F1 RID: 14065 RVA: 0x001B41DE File Offset: 0x001B23DE
		public bool Stun
		{
			get
			{
				return this.animal.Stun;
			}
			set
			{
				this.animal.Stun = value;
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x060036F4 RID: 14068 RVA: 0x001B4207 File Offset: 0x001B2407
		// (set) Token: 0x060036F3 RID: 14067 RVA: 0x001B41F9 File Offset: 0x001B23F9
		public bool Action
		{
			get
			{
				return this.animal.Action;
			}
			set
			{
				this.animal.Action = value;
			}
		}

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x060036F6 RID: 14070 RVA: 0x001B4222 File Offset: 0x001B2422
		// (set) Token: 0x060036F5 RID: 14069 RVA: 0x001B4214 File Offset: 0x001B2414
		public int ActionID
		{
			get
			{
				return this.animal.ActionID;
			}
			set
			{
				this.animal.ActionID = value;
			}
		}

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x060036F8 RID: 14072 RVA: 0x001B423D File Offset: 0x001B243D
		// (set) Token: 0x060036F7 RID: 14071 RVA: 0x001B422F File Offset: 0x001B242F
		public bool IsInAir
		{
			get
			{
				return this.animal.IsInAir;
			}
			set
			{
				this.animal.IsInAir = value;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x060036F9 RID: 14073 RVA: 0x001B424A File Offset: 0x001B244A
		public bool Stand
		{
			get
			{
				return this.animal.Stand;
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x060036FA RID: 14074 RVA: 0x001B4257 File Offset: 0x001B2457
		// (set) Token: 0x060036FB RID: 14075 RVA: 0x001B4264 File Offset: 0x001B2464
		public Vector3 HitDirection
		{
			get
			{
				return this.animal.HitDirection;
			}
			set
			{
				this.animal.HitDirection = value;
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x060036FC RID: 14076 RVA: 0x001B4272 File Offset: 0x001B2472
		public float ScaleFactor
		{
			get
			{
				return this.animal.ScaleFactor;
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x060036FD RID: 14077 RVA: 0x001B427F File Offset: 0x001B247F
		public Pivots Pivot_Hip
		{
			get
			{
				return this.animal.Pivot_Hip;
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x060036FE RID: 14078 RVA: 0x001B428C File Offset: 0x001B248C
		public Pivots Pivot_Chest
		{
			get
			{
				return this.animal.Pivot_Chest;
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x060036FF RID: 14079 RVA: 0x001B4299 File Offset: 0x001B2499
		public int AnimState
		{
			get
			{
				return this.animal.AnimState;
			}
		}

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06003700 RID: 14080 RVA: 0x001B42A6 File Offset: 0x001B24A6
		public int LastAnimationTag
		{
			get
			{
				return this.animal.LastAnimationTag;
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06003701 RID: 14081 RVA: 0x001B42B3 File Offset: 0x001B24B3
		// (set) Token: 0x06003702 RID: 14082 RVA: 0x001B42C0 File Offset: 0x001B24C0
		public Vector3 MovementAxis
		{
			get
			{
				return this.animal.MovementAxis;
			}
			set
			{
				this.animal.MovementAxis = value;
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06003703 RID: 14083 RVA: 0x001B42CE File Offset: 0x001B24CE
		// (set) Token: 0x06003704 RID: 14084 RVA: 0x001B42DB File Offset: 0x001B24DB
		public float MovementForward
		{
			get
			{
				return this.animal.MovementForward;
			}
			set
			{
				this.animal.MovementForward = value;
			}
		}

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06003705 RID: 14085 RVA: 0x001B42E9 File Offset: 0x001B24E9
		// (set) Token: 0x06003706 RID: 14086 RVA: 0x001B42F6 File Offset: 0x001B24F6
		public float MovementRight
		{
			get
			{
				return this.animal.MovementRight;
			}
			set
			{
				this.animal.MovementRight = value;
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06003707 RID: 14087 RVA: 0x001B4304 File Offset: 0x001B2504
		// (set) Token: 0x06003708 RID: 14088 RVA: 0x001B4311 File Offset: 0x001B2511
		public float MovementUp
		{
			get
			{
				return this.animal.MovementUp;
			}
			set
			{
				this.animal.MovementUp = value;
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06003709 RID: 14089 RVA: 0x001B431F File Offset: 0x001B251F
		public Vector3 SurfaceNormal
		{
			get
			{
				return this.animal.SurfaceNormal;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x0600370A RID: 14090 RVA: 0x001B432C File Offset: 0x001B252C
		// (set) Token: 0x0600370B RID: 14091 RVA: 0x001B4339 File Offset: 0x001B2539
		public float Waterlevel
		{
			get
			{
				return this.animal.Waterlevel;
			}
			set
			{
				this.animal.Waterlevel = value;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x0600370C RID: 14092 RVA: 0x001B4347 File Offset: 0x001B2547
		// (set) Token: 0x0600370D RID: 14093 RVA: 0x001B4354 File Offset: 0x001B2554
		public bool Land
		{
			get
			{
				return this.animal.Land;
			}
			set
			{
				this.animal.Land = value;
			}
		}

		// Token: 0x0600370E RID: 14094 RVA: 0x001B4362 File Offset: 0x001B2562
		public virtual void SetAnimal(Animal newAnimal)
		{
			this.animal = newAnimal;
		}

		// Token: 0x0600370F RID: 14095 RVA: 0x001B436B File Offset: 0x001B256B
		public virtual void getDamaged(DamageValues DV)
		{
			if (this.animal)
			{
				this.animal.getDamaged(DV);
			}
		}

		// Token: 0x06003710 RID: 14096 RVA: 0x001B4386 File Offset: 0x001B2586
		public virtual void Stop()
		{
			if (this.animal)
			{
				this.animal.Stop();
			}
		}

		// Token: 0x06003711 RID: 14097 RVA: 0x001B43A0 File Offset: 0x001B25A0
		public virtual void getDamaged(Vector3 Mycenter, Vector3 Theircenter, float Amount = 0f)
		{
			DamageValues dv = new DamageValues(Mycenter - Theircenter, Amount);
			this.getDamaged(dv);
		}

		// Token: 0x06003712 RID: 14098 RVA: 0x001B43C2 File Offset: 0x001B25C2
		public virtual void AttackTrigger(int triggerIndex)
		{
			if (this.animal)
			{
				this.animal.AttackTrigger(triggerIndex);
			}
		}

		// Token: 0x06003713 RID: 14099 RVA: 0x001B43DD File Offset: 0x001B25DD
		public virtual void SetAttack()
		{
			if (this.animal)
			{
				this.animal.SetAttack();
			}
		}

		// Token: 0x06003714 RID: 14100 RVA: 0x001B43F7 File Offset: 0x001B25F7
		public virtual void SetLoop(int cycles)
		{
			if (this.animal)
			{
				this.animal.Loops = cycles;
			}
		}

		// Token: 0x06003715 RID: 14101 RVA: 0x001B4412 File Offset: 0x001B2612
		public virtual void SetAttack(int attackID)
		{
			if (this.animal)
			{
				this.animal.SetAttack(attackID);
			}
		}

		// Token: 0x06003716 RID: 14102 RVA: 0x001B442D File Offset: 0x001B262D
		public virtual void SetAttack(bool value)
		{
			this.Attack1 = value;
		}

		// Token: 0x06003717 RID: 14103 RVA: 0x001B4436 File Offset: 0x001B2636
		public virtual void SetSecondaryAttack()
		{
			if (this.animal)
			{
				this.animal.SetSecondaryAttack();
			}
		}

		// Token: 0x06003718 RID: 14104 RVA: 0x001B4450 File Offset: 0x001B2650
		public virtual void RigidDrag(float amount)
		{
			if (this.animal)
			{
				this.animal.RigidDrag(amount);
			}
		}

		// Token: 0x06003719 RID: 14105 RVA: 0x001B446B File Offset: 0x001B266B
		public void SetIntID(int value)
		{
			if (this.animal)
			{
				this.animal.SetIntID(value);
			}
		}

		// Token: 0x0600371A RID: 14106 RVA: 0x001B4486 File Offset: 0x001B2686
		public void SetFloatID(float value)
		{
			if (this.animal)
			{
				this.animal.SetFloatID(value);
			}
		}

		// Token: 0x0600371B RID: 14107 RVA: 0x001B44A1 File Offset: 0x001B26A1
		public virtual void StillConstraints(bool active)
		{
			if (this.animal)
			{
				this.animal.StillConstraints(active);
			}
		}

		// Token: 0x0600371C RID: 14108 RVA: 0x001B44BC File Offset: 0x001B26BC
		public virtual void EnableColliders(bool active)
		{
			if (this.animal)
			{
				this.animal.EnableColliders(active);
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x0600371E RID: 14110 RVA: 0x001B44F2 File Offset: 0x001B26F2
		// (set) Token: 0x0600371D RID: 14109 RVA: 0x001B44D7 File Offset: 0x001B26D7
		public virtual bool Gravity
		{
			get
			{
				return this.animal && this.animal.Gravity;
			}
			set
			{
				if (this.animal)
				{
					this.animal.Gravity = value;
				}
			}
		}

		// Token: 0x0600371F RID: 14111 RVA: 0x001B450E File Offset: 0x001B270E
		public virtual void InAir(bool active)
		{
			if (this.animal)
			{
				this.animal.InAir(active);
			}
		}

		// Token: 0x06003720 RID: 14112 RVA: 0x001B4529 File Offset: 0x001B2729
		public virtual void SetJump()
		{
			if (this.animal)
			{
				this.animal.SetJump();
			}
		}

		// Token: 0x06003721 RID: 14113 RVA: 0x001B4543 File Offset: 0x001B2743
		public virtual void SetAction(int ID)
		{
			if (this.animal)
			{
				this.animal.SetAction(ID);
			}
		}

		// Token: 0x06003722 RID: 14114 RVA: 0x001B455E File Offset: 0x001B275E
		public virtual void SetAction(string actionName)
		{
			if (this.animal)
			{
				this.animal.SetAction(actionName);
			}
		}

		// Token: 0x06003723 RID: 14115 RVA: 0x001B4579 File Offset: 0x001B2779
		public virtual void ResetAnimal()
		{
			if (this.animal)
			{
				this.animal.ResetAnimal();
			}
		}

		// Token: 0x06003724 RID: 14116 RVA: 0x001B4593 File Offset: 0x001B2793
		public virtual void SetStun(float time)
		{
			if (this.animal)
			{
				this.animal.SetStun(time);
			}
		}

		// Token: 0x06003725 RID: 14117 RVA: 0x001B45AE File Offset: 0x001B27AE
		public virtual void DisableAnimal()
		{
			if (this.animal)
			{
				this.animal.DisableAnimal();
			}
		}

		// Token: 0x06003726 RID: 14118 RVA: 0x001B45C8 File Offset: 0x001B27C8
		public virtual void SetToGlide(float value)
		{
			if (this.animal)
			{
				this.animal.SetToGlide(value);
			}
		}

		// Token: 0x0400268F RID: 9871
		protected Animal animal;

		// Token: 0x04002690 RID: 9872
		[Tooltip("Since the Animal Proxy is a Scriptable Object... After Stoping the Editor an animal can get stored.. you can clean the reference on the next Editor Play enabling this option ")]
		public bool CleanAnimalOnEnable;
	}
}
