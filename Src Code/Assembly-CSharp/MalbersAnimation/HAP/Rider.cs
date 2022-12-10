using System;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x02000436 RID: 1078
	public abstract class Rider : MonoBehaviour, IAnimatorListener
	{
		// Token: 0x170003BC RID: 956
		// (get) Token: 0x060039D2 RID: 14802 RVA: 0x001C0712 File Offset: 0x001BE912
		protected MonoBehaviour[] AllComponents
		{
			get
			{
				if (this.allComponents == null)
				{
					this.allComponents = base.GetComponents<MonoBehaviour>();
				}
				return this.allComponents;
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x060039D3 RID: 14803 RVA: 0x001C072E File Offset: 0x001BE92E
		// (set) Token: 0x060039D4 RID: 14804 RVA: 0x001C0736 File Offset: 0x001BE936
		public virtual Mountable Montura
		{
			get
			{
				return this.montura;
			}
			set
			{
				this.montura = value;
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x060039D5 RID: 14805 RVA: 0x001C073F File Offset: 0x001BE93F
		// (set) Token: 0x060039D6 RID: 14806 RVA: 0x001C0747 File Offset: 0x001BE947
		public MountTriggers MountTrigger
		{
			get
			{
				return this.mountTrigger;
			}
			set
			{
				this.mountTrigger = value;
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x060039D7 RID: 14807 RVA: 0x001C0750 File Offset: 0x001BE950
		public bool CanMount
		{
			get
			{
				return this.MountTrigger && !this.Mounted && !this.IsOnHorse;
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x060039D8 RID: 14808 RVA: 0x001C0772 File Offset: 0x001BE972
		public virtual bool CanDismount
		{
			get
			{
				return this.IsRiding;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x060039D9 RID: 14809 RVA: 0x001C077A File Offset: 0x001BE97A
		public virtual bool CanCallAnimal
		{
			get
			{
				return !this.MountTrigger && !this.Mounted && !this.IsOnHorse;
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x060039DA RID: 14810 RVA: 0x001C079C File Offset: 0x001BE99C
		// (set) Token: 0x060039DB RID: 14811 RVA: 0x001C07A4 File Offset: 0x001BE9A4
		public bool Mounted
		{
			get
			{
				return this.mounted;
			}
			set
			{
				this.mounted = value;
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x060039DC RID: 14812 RVA: 0x001C07AD File Offset: 0x001BE9AD
		public bool IsRiding
		{
			get
			{
				return this.IsOnHorse && this.mounted;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x060039DD RID: 14813 RVA: 0x001C07BF File Offset: 0x001BE9BF
		// (set) Token: 0x060039DE RID: 14814 RVA: 0x001C07C7 File Offset: 0x001BE9C7
		public bool IsOnHorse
		{
			get
			{
				return this.isOnHorse;
			}
			protected set
			{
				this.isOnHorse = value;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x060039DF RID: 14815 RVA: 0x001C07D0 File Offset: 0x001BE9D0
		// (set) Token: 0x060039E0 RID: 14816 RVA: 0x001C080E File Offset: 0x001BEA0E
		public virtual MalbersInput AnimalControl
		{
			get
			{
				if (this.animalControls == null && this.Montura.Animal)
				{
					this.animalControls = this.Montura.Animal.GetComponent<MalbersInput>();
				}
				return this.animalControls;
			}
			set
			{
				this.animalControls = value;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x060039E1 RID: 14817 RVA: 0x001C0817 File Offset: 0x001BEA17
		public Camera MainCamera
		{
			get
			{
				if (Camera.main != null)
				{
					this.cam = Camera.main;
				}
				else
				{
					this.cam = null;
				}
				return this.cam;
			}
		}

		// Token: 0x060039E2 RID: 14818 RVA: 0x001C0840 File Offset: 0x001BEA40
		public virtual void Start_Mounting()
		{
			this.Montura.ActiveRider = this;
			this.Montura.Mounted = (this.Mounted = true);
			if (this._rigidbody)
			{
				this._rigidbody.useGravity = false;
				this._rigidbody.isKinematic = true;
				this.DefaultConstraints = this._rigidbody.constraints;
				this._rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			}
			this.ToogleColliders(false);
			this.toogleCall = true;
			this.CallAnimal(false);
			if (this.Montura.Animal)
			{
				this.AnimalStored = this.Montura.Animal.GetComponent<Mountable>();
			}
			if (this.Parent)
			{
				base.transform.parent = this.Montura.MountPoint;
			}
		}

		// Token: 0x060039E3 RID: 14819 RVA: 0x001C090C File Offset: 0x001BEB0C
		public virtual void End_Mounting()
		{
			this.IsOnHorse = true;
			this.Montura.Mounted = (this.Mounted = true);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.Montura.EnableControls(true);
			if (this.CreateColliderMounted)
			{
				this.MountingCollider(true);
			}
		}

		// Token: 0x060039E4 RID: 14820 RVA: 0x001C0970 File Offset: 0x001BEB70
		public virtual void Start_Dismounting()
		{
			this.MountingCollider(false);
			if (this.CreateColliderMounted)
			{
				this.MountingCollider(false);
			}
			base.transform.parent = null;
			this.Montura.ActiveRider = null;
			this.Montura.ActiveRiderCombat = null;
			this.Montura.Mounted = (this.mounted = false);
			if (!this.Anim)
			{
				this.End_Dismounting();
			}
		}

		// Token: 0x060039E5 RID: 14821 RVA: 0x001C09E0 File Offset: 0x001BEBE0
		public virtual void End_Dismounting()
		{
			this.IsOnHorse = false;
			if (this.Montura)
			{
				this.Montura.EnableControls(false);
			}
			this.Montura = null;
			this.toogleCall = false;
			if (this._rigidbody)
			{
				this._rigidbody.useGravity = true;
				this._rigidbody.isKinematic = false;
				this._rigidbody.constraints = this.DefaultConstraints;
			}
			if (this.Anim)
			{
				this.Anim.speed = 1f;
			}
			this._transform.rotation = Quaternion.FromToRotation(this._transform.up, -Physics.gravity) * this._transform.rotation;
			this.ToogleColliders(true);
		}

		// Token: 0x060039E6 RID: 14822 RVA: 0x001C0AAA File Offset: 0x001BECAA
		public virtual void LinkRider()
		{
			if (this.IsRiding)
			{
				base.transform.position = this.Montura.MountPoint.position;
				base.transform.rotation = this.Montura.MountPoint.rotation;
			}
		}

		// Token: 0x060039E7 RID: 14823 RVA: 0x001C0AEC File Offset: 0x001BECEC
		public virtual void MountingCollider(bool create)
		{
			if (create)
			{
				this.mountedCollider = base.gameObject.AddComponent<CapsuleCollider>();
				this.mountedCollider.center = new Vector3(0f, this.Col_Center);
				this.mountedCollider.radius = this.Col_radius;
				this.mountedCollider.height = this.Col_height;
				this.mountedCollider.isTrigger = this.Col_Trigger;
				return;
			}
			Object.Destroy(this.mountedCollider);
		}

		// Token: 0x060039E8 RID: 14824 RVA: 0x001C0B67 File Offset: 0x001BED67
		public virtual void SetAnimalStored(Mountable MAnimal)
		{
			this.AnimalStored = MAnimal;
		}

		// Token: 0x060039E9 RID: 14825 RVA: 0x001C0B70 File Offset: 0x001BED70
		public virtual void KillMountura()
		{
			if (this.Montura)
			{
				this.Montura.Animal.Death = true;
			}
		}

		// Token: 0x060039EA RID: 14826 RVA: 0x001C0B90 File Offset: 0x001BED90
		public virtual void CallAnimal(bool playWistle = true)
		{
			if (!this.CanCallAnimal)
			{
				return;
			}
			if (this.AnimalStored)
			{
				IMountAI component = this.AnimalStored.GetComponent<IMountAI>();
				if (component != null)
				{
					this.toogleCall = !this.toogleCall;
					component.CallAnimal(this._transform, this.toogleCall);
					if (this.CallAnimalA && this.StopAnimalA && playWistle)
					{
						this.RiderAudio.PlayOneShot(this.toogleCall ? this.CallAnimalA : this.StopAnimalA);
					}
				}
			}
		}

		// Token: 0x060039EB RID: 14827 RVA: 0x001C0C24 File Offset: 0x001BEE24
		protected virtual void ToogleColliders(bool active)
		{
			if (this._collider.Length != 0)
			{
				Collider[] collider = this._collider;
				for (int i = 0; i < collider.Length; i++)
				{
					collider[i].enabled = active;
				}
			}
		}

		// Token: 0x060039EC RID: 14828 RVA: 0x001C0C58 File Offset: 0x001BEE58
		protected virtual void ToggleComponents(bool enabled)
		{
			if (this.DisableList.Length == 0)
			{
				foreach (MonoBehaviour monoBehaviour in this.AllComponents)
				{
					if (!(monoBehaviour is Rider) && !(monoBehaviour is RiderCombat))
					{
						monoBehaviour.enabled = enabled;
					}
				}
				return;
			}
			foreach (MonoBehaviour monoBehaviour2 in this.DisableList)
			{
				if (monoBehaviour2 != null)
				{
					monoBehaviour2.enabled = enabled;
				}
			}
		}

		// Token: 0x060039ED RID: 14829 RVA: 0x001AF9E6 File Offset: 0x001ADBE6
		public virtual void OnAnimatorBehaviourMessage(string message, object value)
		{
			this.InvokeWithParams(message, value);
		}

		// Token: 0x040029CA RID: 10698
		protected bool mounted;

		// Token: 0x040029CB RID: 10699
		protected bool isOnHorse;

		// Token: 0x040029CC RID: 10700
		protected bool isInMountTrigger;

		// Token: 0x040029CD RID: 10701
		protected Mountable montura;

		// Token: 0x040029CE RID: 10702
		protected bool toogleCall;

		// Token: 0x040029CF RID: 10703
		protected MalbersInput animalControls;

		// Token: 0x040029D0 RID: 10704
		protected MountTriggers mountTrigger;

		// Token: 0x040029D1 RID: 10705
		protected MonoBehaviour[] allComponents;

		// Token: 0x040029D2 RID: 10706
		public Animator Anim;

		// Token: 0x040029D3 RID: 10707
		protected Transform _transform;

		// Token: 0x040029D4 RID: 10708
		protected Rigidbody _rigidbody;

		// Token: 0x040029D5 RID: 10709
		protected Collider[] _collider;

		// Token: 0x040029D6 RID: 10710
		protected CapsuleCollider mountedCollider;

		// Token: 0x040029D7 RID: 10711
		public bool StartMounted;

		// Token: 0x040029D8 RID: 10712
		[Tooltip("Parent the Rider to the Mount Point")]
		public bool Parent;

		// Token: 0x040029D9 RID: 10713
		[Flag]
		public Rider.UpdateType LinkUpdate = (Rider.UpdateType)3;

		// Token: 0x040029DA RID: 10714
		public Mountable AnimalStored;

		// Token: 0x040029DB RID: 10715
		public InputRow MountInput = new InputRow("Mount", KeyCode.F, InputButton.Down);

		// Token: 0x040029DC RID: 10716
		public InputRow DismountInput = new InputRow("Mount", KeyCode.F, InputButton.LongPress);

		// Token: 0x040029DD RID: 10717
		public InputRow CallAnimalInput = new InputRow("Call", KeyCode.F, InputButton.Down);

		// Token: 0x040029DE RID: 10718
		internal IInputSystem inputSystem;

		// Token: 0x040029DF RID: 10719
		public bool CreateColliderMounted;

		// Token: 0x040029E0 RID: 10720
		public float Col_radius = 0.25f;

		// Token: 0x040029E1 RID: 10721
		public bool Col_Trigger = true;

		// Token: 0x040029E2 RID: 10722
		public float Col_height = 0.8f;

		// Token: 0x040029E3 RID: 10723
		public float Col_Center = 0.48f;

		// Token: 0x040029E4 RID: 10724
		public bool DisableComponents;

		// Token: 0x040029E5 RID: 10725
		public MonoBehaviour[] DisableList;

		// Token: 0x040029E6 RID: 10726
		public AudioClip CallAnimalA;

		// Token: 0x040029E7 RID: 10727
		public AudioClip StopAnimalA;

		// Token: 0x040029E8 RID: 10728
		public AudioSource RiderAudio;

		// Token: 0x040029E9 RID: 10729
		public string PlayerID = "Player0";

		// Token: 0x040029EA RID: 10730
		public GameObjectEvent OnFindMount = new GameObjectEvent();

		// Token: 0x040029EB RID: 10731
		public BoolEvent OnCanMount = new BoolEvent();

		// Token: 0x040029EC RID: 10732
		public BoolEvent OnCanDismount = new BoolEvent();

		// Token: 0x040029ED RID: 10733
		public BoolEvent OnCallMount = new BoolEvent();

		// Token: 0x040029EE RID: 10734
		protected Camera cam;

		// Token: 0x040029EF RID: 10735
		[HideInInspector]
		public bool Editor_RiderCallAnimal;

		// Token: 0x040029F0 RID: 10736
		[HideInInspector]
		public bool Editor_Events;

		// Token: 0x040029F1 RID: 10737
		[HideInInspector]
		public bool Editor_Inputs;

		// Token: 0x040029F2 RID: 10738
		private RigidbodyConstraints DefaultConstraints;

		// Token: 0x02000938 RID: 2360
		public enum UpdateType
		{
			// Token: 0x040042D5 RID: 17109
			Update = 1,
			// Token: 0x040042D6 RID: 17110
			FixedUpdate,
			// Token: 0x040042D7 RID: 17111
			LateUpdate = 4
		}
	}
}
