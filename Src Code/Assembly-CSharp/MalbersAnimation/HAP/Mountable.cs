using System;
using System.Collections;
using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.HAP
{
	// Token: 0x0200042D RID: 1069
	public class Mountable : MonoBehaviour, IAnimatorListener
	{
		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x06003960 RID: 14688 RVA: 0x001BF0A0 File Offset: 0x001BD2A0
		public Transform MountPoint
		{
			get
			{
				return this.ridersLink;
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x06003961 RID: 14689 RVA: 0x001BF0A8 File Offset: 0x001BD2A8
		public Transform FootLeftIK
		{
			get
			{
				return this.leftIK;
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x06003962 RID: 14690 RVA: 0x001BF0B0 File Offset: 0x001BD2B0
		public Transform FootRightIK
		{
			get
			{
				return this.rightIK;
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x06003963 RID: 14691 RVA: 0x001BF0B8 File Offset: 0x001BD2B8
		public Transform KneeLeftIK
		{
			get
			{
				return this.leftKnee;
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x06003964 RID: 14692 RVA: 0x001BF0C0 File Offset: 0x001BD2C0
		public Transform KneeRightIK
		{
			get
			{
				return this.rightKnee;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x06003965 RID: 14693 RVA: 0x001BF0C8 File Offset: 0x001BD2C8
		public bool StraightSpine
		{
			get
			{
				return this.straightSpine;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06003966 RID: 14694 RVA: 0x001BF0D0 File Offset: 0x001BD2D0
		public Quaternion PointOffset
		{
			get
			{
				return Quaternion.Euler(this.pointOffset);
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06003968 RID: 14696 RVA: 0x001BF10E File Offset: 0x001BD30E
		// (set) Token: 0x06003967 RID: 14695 RVA: 0x001BF0DD File Offset: 0x001BD2DD
		public bool Mounted
		{
			get
			{
				return this.mounted;
			}
			set
			{
				if (value != this.mounted)
				{
					this.mounted = value;
					if (this.mounted)
					{
						this.OnMounted.Invoke();
						return;
					}
					this.OnDismounted.Invoke();
				}
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06003969 RID: 14697 RVA: 0x001BF116 File Offset: 0x001BD316
		public virtual Animal Animal
		{
			get
			{
				if (this._animal == null)
				{
					this._animal = base.GetComponent<Animal>();
				}
				return this._animal;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x0600396A RID: 14698 RVA: 0x001BF138 File Offset: 0x001BD338
		public virtual bool CanDismount
		{
			get
			{
				return this.Mounted;
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x0600396B RID: 14699 RVA: 0x001BF140 File Offset: 0x001BD340
		// (set) Token: 0x0600396C RID: 14700 RVA: 0x001BF148 File Offset: 0x001BD348
		public virtual string MountLayer
		{
			get
			{
				return this.mountLayer;
			}
			set
			{
				this.mountLayer = value;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x0600396D RID: 14701 RVA: 0x001BF151 File Offset: 0x001BD351
		// (set) Token: 0x0600396E RID: 14702 RVA: 0x001BF159 File Offset: 0x001BD359
		public virtual string MountIdle
		{
			get
			{
				return this.mountIdle;
			}
			set
			{
				this.mountIdle = value;
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x0600396F RID: 14703 RVA: 0x001BF162 File Offset: 0x001BD362
		// (set) Token: 0x06003970 RID: 14704 RVA: 0x001BF16A File Offset: 0x001BD36A
		public virtual bool CanBeMounted
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06003971 RID: 14705 RVA: 0x001BF173 File Offset: 0x001BD373
		// (set) Token: 0x06003972 RID: 14706 RVA: 0x001BF17B File Offset: 0x001BD37B
		public Rider ActiveRider
		{
			get
			{
				return this._rider;
			}
			set
			{
				this._rider = value;
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06003973 RID: 14707 RVA: 0x001BF184 File Offset: 0x001BD384
		// (set) Token: 0x06003974 RID: 14708 RVA: 0x001BF18C File Offset: 0x001BD38C
		public RiderCombat ActiveRiderCombat
		{
			get
			{
				return this._ridercombat;
			}
			set
			{
				this._ridercombat = value;
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06003975 RID: 14709 RVA: 0x001BF195 File Offset: 0x001BD395
		// (set) Token: 0x06003976 RID: 14710 RVA: 0x001BF19D File Offset: 0x001BD39D
		public bool InstantMount
		{
			get
			{
				return this.instantMount;
			}
			set
			{
				this.instantMount = value;
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06003977 RID: 14711 RVA: 0x001BF1A6 File Offset: 0x001BD3A6
		public Quaternion StraightRotation
		{
			get
			{
				return this.straightRotation;
			}
		}

		// Token: 0x06003978 RID: 14712 RVA: 0x001BF1B0 File Offset: 0x001BD3B0
		public virtual void EnableControls(bool value)
		{
			if (this.Animal)
			{
				MalbersInput component = this.Animal.GetComponent<MalbersInput>();
				if (component)
				{
					component.enabled = value;
				}
				this.Animal.ResetInputs();
			}
		}

		// Token: 0x06003979 RID: 14713 RVA: 0x000023FD File Offset: 0x000005FD
		private void OnValidate()
		{
		}

		// Token: 0x0600397A RID: 14714 RVA: 0x001BF1F0 File Offset: 0x001BD3F0
		private void Start()
		{
			this.InitialRotation = this.MountPoint.localRotation;
		}

		// Token: 0x0600397B RID: 14715 RVA: 0x001BF203 File Offset: 0x001BD403
		private void Update()
		{
			if (!this.ActiveRider)
			{
				return;
			}
			if (!this.ActiveRider.IsRiding)
			{
				return;
			}
			if (this.syncAnimators)
			{
				this.SetAnimatorSpeed(Time.deltaTime * 2f);
			}
			this.SolveStraightMount();
		}

		// Token: 0x0600397C RID: 14716 RVA: 0x001BF240 File Offset: 0x001BD440
		private void SolveStraightMount()
		{
			this.currentRotation = this.MountPoint.localRotation;
			this.MountPoint.localRotation = this.InitialRotation;
			this.straightRotation = Quaternion.FromToRotation(this.MountPoint.up, Vector3.up) * this.MountPoint.rotation;
			float num = Vector3.Angle(Vector3.up, this.MountPoint.forward);
			if (num < this.LowLimit)
			{
				this.straightRotation *= Quaternion.Euler(new Vector3(num - this.LowLimit, 0f));
			}
			else if (num > this.HighLimit)
			{
				this.straightRotation *= Quaternion.Euler(new Vector3(num - this.HighLimit, 0f));
			}
			if (this.pointOffset != Vector3.zero)
			{
				this.straightRotation *= this.PointOffset;
			}
			this.MountPoint.localRotation = this.currentRotation;
			if (this.straightAim)
			{
				this.MountPoint.rotation = this.straightRotation;
				return;
			}
			if (this.straightSpine && !this.changed)
			{
				this.changed = true;
				base.StopAllCoroutines();
				base.StartCoroutine(this.I_to_StraightMount(1f));
			}
			if (!this.straightSpine && this.changed)
			{
				this.changed = false;
				base.StopAllCoroutines();
				base.StartCoroutine(this.I_from_StraightMount(1f));
			}
			if (!this.isOnCoroutine)
			{
				if (this.straightSpine)
				{
					this.MountPoint.rotation = this.straightRotation;
					return;
				}
				this.MountPoint.localRotation = this.InitialRotation;
			}
		}

		// Token: 0x0600397D RID: 14717 RVA: 0x001BF3F9 File Offset: 0x001BD5F9
		private IEnumerator I_to_StraightMount(float time)
		{
			float currentTime = 0f;
			Quaternion startRotation = this.MountPoint.rotation;
			this.isOnCoroutine = true;
			while (currentTime <= time)
			{
				currentTime += Time.deltaTime;
				this.MountPoint.rotation = Quaternion.Slerp(startRotation, this.straightRotation, currentTime / time);
				yield return null;
			}
			this.MountPoint.rotation = this.straightRotation;
			this.isOnCoroutine = false;
			yield break;
		}

		// Token: 0x0600397E RID: 14718 RVA: 0x001BF40F File Offset: 0x001BD60F
		private IEnumerator I_from_StraightMount(float time)
		{
			float currentTime = 0f;
			Quaternion startRotation = this.MountPoint.localRotation;
			this.isOnCoroutine = true;
			while (currentTime <= time)
			{
				currentTime += Time.deltaTime;
				this.MountPoint.localRotation = Quaternion.Slerp(startRotation, this.InitialRotation, currentTime / time);
				yield return null;
			}
			this.MountPoint.localRotation = this.InitialRotation;
			this.isOnCoroutine = false;
			yield break;
		}

		// Token: 0x0600397F RID: 14719 RVA: 0x001BF428 File Offset: 0x001BD628
		private void SetAnimatorSpeed(float time)
		{
			this.AnimatorSpeed = 1f;
			if (this.Animal.AnimState == AnimTag.Locomotion)
			{
				if (this.Animal.Speed1)
				{
					this.AnimatorSpeed = this.WalkASpeed * this.Animal.walkSpeed.animator;
				}
				else if (this.Animal.Speed2)
				{
					this.AnimatorSpeed = this.TrotASpeed * this.Animal.trotSpeed.animator;
				}
				else if (this.Animal.Speed3)
				{
					this.AnimatorSpeed = this.RunASpeed * this.Animal.runSpeed.animator;
				}
			}
			else if (this.Animal.canSwim && this.Animal.Swim)
			{
				this.AnimatorSpeed = this.SwimASpeed * this.Animal.swimSpeed.animator;
			}
			else if (this.Animal.canFly && this.Animal.Fly)
			{
				this.AnimatorSpeed = this.FlyASpeed * this.Animal.flySpeed.animator;
			}
			this.AnimatorSpeed *= this.Animal.animatorSpeed;
			if (this.ActiveRider is Rider3rdPerson)
			{
				this.ActiveRider.Anim.speed = Mathf.Lerp(this.ActiveRider.Anim.speed, this.AnimatorSpeed, time);
			}
		}

		// Token: 0x06003980 RID: 14720 RVA: 0x001BF5A3 File Offset: 0x001BD7A3
		public virtual void StraightMount(bool value)
		{
			this.straightSpine = value;
		}

		// Token: 0x06003981 RID: 14721 RVA: 0x001BF5AC File Offset: 0x001BD7AC
		public virtual void StraightAim(bool value)
		{
			this.straightAim = value;
		}

		// Token: 0x06003982 RID: 14722 RVA: 0x001AF9E6 File Offset: 0x001ADBE6
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
		}

		// Token: 0x0400297B RID: 10619
		protected Rider _rider;

		// Token: 0x0400297C RID: 10620
		protected RiderCombat _ridercombat;

		// Token: 0x0400297D RID: 10621
		protected Animal _animal;

		// Token: 0x0400297E RID: 10622
		public bool active = true;

		// Token: 0x0400297F RID: 10623
		public string mountLayer = "Mounted";

		// Token: 0x04002980 RID: 10624
		public bool instantMount;

		// Token: 0x04002981 RID: 10625
		public string mountIdle = "Idle01";

		// Token: 0x04002982 RID: 10626
		protected bool mounted;

		// Token: 0x04002983 RID: 10627
		private bool isOnCoroutine;

		// Token: 0x04002984 RID: 10628
		internal bool NearbyRider;

		// Token: 0x04002985 RID: 10629
		public bool straightSpine;

		// Token: 0x04002986 RID: 10630
		public Vector3 pointOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x04002987 RID: 10631
		public float LowLimit = 45f;

		// Token: 0x04002988 RID: 10632
		public float HighLimit = 135f;

		// Token: 0x04002989 RID: 10633
		public float smoothSM = 0.5f;

		// Token: 0x0400298A RID: 10634
		protected Quaternion InitialRotation;

		// Token: 0x0400298B RID: 10635
		private Quaternion straightRotation;

		// Token: 0x0400298C RID: 10636
		private Quaternion currentRotation;

		// Token: 0x0400298D RID: 10637
		public bool syncAnimators = true;

		// Token: 0x0400298E RID: 10638
		public float WalkASpeed = 1f;

		// Token: 0x0400298F RID: 10639
		public float TrotASpeed = 1f;

		// Token: 0x04002990 RID: 10640
		public float RunASpeed = 1f;

		// Token: 0x04002991 RID: 10641
		public float FlyASpeed = 1f;

		// Token: 0x04002992 RID: 10642
		public float SwimASpeed = 1f;

		// Token: 0x04002993 RID: 10643
		protected float AnimatorSpeed = 1f;

		// Token: 0x04002994 RID: 10644
		public bool DebugSync;

		// Token: 0x04002995 RID: 10645
		public Transform ridersLink;

		// Token: 0x04002996 RID: 10646
		public Transform leftIK;

		// Token: 0x04002997 RID: 10647
		public Transform rightIK;

		// Token: 0x04002998 RID: 10648
		public Transform leftKnee;

		// Token: 0x04002999 RID: 10649
		public Transform rightKnee;

		// Token: 0x0400299A RID: 10650
		protected Vector3 LocalStride_L;

		// Token: 0x0400299B RID: 10651
		protected Vector3 LocalStride_R;

		// Token: 0x0400299C RID: 10652
		protected bool freeRightHand = true;

		// Token: 0x0400299D RID: 10653
		protected bool freeLeftHand = true;

		// Token: 0x0400299E RID: 10654
		public UnityEvent OnMounted = new UnityEvent();

		// Token: 0x0400299F RID: 10655
		public UnityEvent OnDismounted = new UnityEvent();

		// Token: 0x040029A0 RID: 10656
		public BoolEvent OnCanBeMounted = new BoolEvent();

		// Token: 0x040029A1 RID: 10657
		private bool changed;

		// Token: 0x040029A2 RID: 10658
		private bool straightAim;

		// Token: 0x040029A3 RID: 10659
		[HideInInspector]
		public bool ShowLinks = true;

		// Token: 0x040029A4 RID: 10660
		[HideInInspector]
		public bool ShowAnimatorSpeeds;

		// Token: 0x040029A5 RID: 10661
		[HideInInspector]
		public bool ShowEvents;
	}
}
