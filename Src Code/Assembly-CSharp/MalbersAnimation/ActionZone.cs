using System;
using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
	// Token: 0x020003BD RID: 957
	[RequireComponent(typeof(BoxCollider))]
	public class ActionZone : MonoBehaviour, IWayPoint
	{
		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x060035C8 RID: 13768 RVA: 0x001AF455 File Offset: 0x001AD655
		// (set) Token: 0x060035C9 RID: 13769 RVA: 0x001AF45D File Offset: 0x001AD65D
		public List<Transform> NextTargets
		{
			get
			{
				return this.nextTargets;
			}
			set
			{
				this.nextTargets = value;
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x060035CA RID: 13770 RVA: 0x001AF466 File Offset: 0x001AD666
		public Transform NextTarget
		{
			get
			{
				if (this.NextTargets.Count > 0)
				{
					return this.NextTargets[Random.Range(0, this.NextTargets.Count)];
				}
				return null;
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x060035CB RID: 13771 RVA: 0x001AF494 File Offset: 0x001AD694
		public WayPointType PointType
		{
			get
			{
				return this.pointType;
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x060035CC RID: 13772 RVA: 0x001AF49C File Offset: 0x001AD69C
		public float WaitTime
		{
			get
			{
				return this.waitTime.RandomValue;
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x060035CD RID: 13773 RVA: 0x001AF4A9 File Offset: 0x001AD6A9
		// (set) Token: 0x060035CE RID: 13774 RVA: 0x001AF4B1 File Offset: 0x001AD6B1
		public float StoppingDistance
		{
			get
			{
				return this.stoppingDistance;
			}
			set
			{
				this.stoppingDistance = value;
			}
		}

		// Token: 0x060035CF RID: 13775 RVA: 0x001AF4BC File Offset: 0x001AD6BC
		private void OnTriggerEnter(Collider other)
		{
			if (!MalbersTools.CollidersLayer(other, LayerMask.GetMask(new string[]
			{
				"Animal"
			})))
			{
				return;
			}
			if (this.HeadOnly && !other.name.ToLowerInvariant().Contains("head"))
			{
				return;
			}
			Animal componentInParent = other.GetComponentInParent<Animal>();
			if (!componentInParent)
			{
				return;
			}
			componentInParent.ActionID = this.ID;
			if (this.animal_Colliders.Find((Collider coll) => coll == other) == null)
			{
				this.animal_Colliders.Add(other);
			}
			if (componentInParent == this.oldAnimal)
			{
				return;
			}
			if (this.oldAnimal)
			{
				this.oldAnimal.ActionID = -1;
				this.animal_Colliders = new List<Collider>();
			}
			this.oldAnimal = componentInParent;
			componentInParent.OnAction.AddListener(new UnityAction(this.OnActionListener));
			this.OnEnter.Invoke(componentInParent);
			if (this.automatic)
			{
				if (componentInParent.AnimState == AnimTag.Jump && !this.ActiveOnJump)
				{
					return;
				}
				componentInParent.SetAction(this.ID);
				base.StartCoroutine(this.ReEnable(componentInParent));
			}
		}

		// Token: 0x060035D0 RID: 13776 RVA: 0x001AF60C File Offset: 0x001AD80C
		private void OnTriggerExit(Collider other)
		{
			if (this.HeadOnly && !other.name.ToLowerInvariant().Contains("head"))
			{
				return;
			}
			Animal componentInParent = other.GetComponentInParent<Animal>();
			if (!componentInParent)
			{
				return;
			}
			if (componentInParent != this.oldAnimal)
			{
				return;
			}
			if (this.animal_Colliders.Find((Collider item) => item == other))
			{
				this.animal_Colliders.Remove(other);
			}
			if (this.animal_Colliders.Count == 0)
			{
				this.OnExit.Invoke(this.oldAnimal);
				if (this.oldAnimal.ActionID == this.ID)
				{
					this.oldAnimal.ActionID = -1;
				}
				this.oldAnimal = null;
			}
		}

		// Token: 0x060035D1 RID: 13777 RVA: 0x001AF6E8 File Offset: 0x001AD8E8
		private IEnumerator ReEnable(Animal animal)
		{
			if (this.AutomaticDisabled > 0f)
			{
				this.ZoneCollider.enabled = false;
				yield return null;
				yield return null;
				animal.ActionID = -1;
				yield return new WaitForSeconds(this.AutomaticDisabled);
				this.ZoneCollider.enabled = true;
			}
			this.oldAnimal = null;
			this.animal_Colliders = new List<Collider>();
			yield return null;
			yield break;
		}

		// Token: 0x060035D2 RID: 13778 RVA: 0x001AF6FE File Offset: 0x001AD8FE
		public virtual void _DestroyActionZone(float time)
		{
			Object.Destroy(base.gameObject, time);
		}

		// Token: 0x060035D3 RID: 13779 RVA: 0x001AF70C File Offset: 0x001AD90C
		private void OnActionListener()
		{
			if (!this.oldAnimal)
			{
				return;
			}
			base.StartCoroutine(this.OnActionDelay(this.ActionDelay, this.oldAnimal));
			if (this.Align && this.AlingPoint)
			{
				Vector3 newPosition = this.AlingPoint.position;
				if (this.AlingPoint2)
				{
					newPosition = MalbersTools.ClosestPointOnLine(this.AlingPoint.position, this.AlingPoint2.position, this.oldAnimal.transform.position);
				}
				if (this.AlignLookAt)
				{
					IEnumerator routine = MalbersTools.AlignLookAtTransform(this.oldAnimal.transform, this.AlingPoint, this.AlignTime, this.AlignCurve);
					base.StartCoroutine(routine);
				}
				else
				{
					if (this.AlignPos)
					{
						base.StartCoroutine(MalbersTools.AlignTransform_Position(this.oldAnimal.transform, newPosition, this.AlignTime, this.AlignCurve));
					}
					if (this.AlignRot)
					{
						base.StartCoroutine(MalbersTools.AlignTransform_Rotation(this.oldAnimal.transform, this.AlingPoint.rotation, this.AlignTime, this.AlignCurve));
					}
				}
			}
			base.StartCoroutine(this.CheckForCollidersOff());
		}

		// Token: 0x060035D4 RID: 13780 RVA: 0x001AF846 File Offset: 0x001ADA46
		private IEnumerator OnActionDelay(float time, Animal animal)
		{
			if (time > 0f)
			{
				yield return new WaitForSeconds(time);
			}
			this.OnAction.Invoke(animal);
			yield return null;
			yield break;
		}

		// Token: 0x060035D5 RID: 13781 RVA: 0x001AF863 File Offset: 0x001ADA63
		private IEnumerator CheckForCollidersOff()
		{
			yield return null;
			yield return null;
			if (this.oldAnimal && !this.oldAnimal.ActiveColliders)
			{
				this.oldAnimal.OnAction.RemoveListener(new UnityAction(this.OnActionListener));
				this.oldAnimal.ActionID = -1;
				this.oldAnimal = null;
				this.animal_Colliders = new List<Collider>();
			}
			yield break;
		}

		// Token: 0x060035D6 RID: 13782 RVA: 0x001AF872 File Offset: 0x001ADA72
		public virtual void _WakeAnimal(Animal animal)
		{
			if (animal)
			{
				animal.MovementAxis = Vector3.forward * 3f;
			}
		}

		// Token: 0x060035D7 RID: 13783 RVA: 0x001AF891 File Offset: 0x001ADA91
		private void OnEnable()
		{
			if (ActionZone.ActionZones == null)
			{
				ActionZone.ActionZones = new List<ActionZone>();
			}
			this.ZoneCollider = base.GetComponent<Collider>();
			ActionZone.ActionZones.Add(this);
		}

		// Token: 0x060035D8 RID: 13784 RVA: 0x001AF8BC File Offset: 0x001ADABC
		private void OnDisable()
		{
			ActionZone.ActionZones.Remove(this);
			if (this.oldAnimal)
			{
				this.oldAnimal.OnAction.RemoveListener(new UnityAction(this.OnActionListener));
				this.oldAnimal.ActionID = -1;
			}
		}

		// Token: 0x04002567 RID: 9575
		private static Keyframe[] K = new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		};

		// Token: 0x04002568 RID: 9576
		public Action ID;

		// Token: 0x04002569 RID: 9577
		public bool automatic;

		// Token: 0x0400256A RID: 9578
		public int index;

		// Token: 0x0400256B RID: 9579
		public float AutomaticDisabled = 10f;

		// Token: 0x0400256C RID: 9580
		public bool HeadOnly;

		// Token: 0x0400256D RID: 9581
		public bool ActiveOnJump;

		// Token: 0x0400256E RID: 9582
		public bool MoveToExitAction;

		// Token: 0x0400256F RID: 9583
		public bool Align;

		// Token: 0x04002570 RID: 9584
		public bool AlignWithWidth;

		// Token: 0x04002571 RID: 9585
		public Transform AlingPoint;

		// Token: 0x04002572 RID: 9586
		public Transform AlingPoint2;

		// Token: 0x04002573 RID: 9587
		public float AlignTime = 0.5f;

		// Token: 0x04002574 RID: 9588
		public AnimationCurve AlignCurve = new AnimationCurve(ActionZone.K);

		// Token: 0x04002575 RID: 9589
		public bool AlignPos = true;

		// Token: 0x04002576 RID: 9590
		public bool AlignRot = true;

		// Token: 0x04002577 RID: 9591
		public bool AlignLookAt;

		// Token: 0x04002578 RID: 9592
		protected List<Collider> animal_Colliders = new List<Collider>();

		// Token: 0x04002579 RID: 9593
		protected Animal oldAnimal;

		// Token: 0x0400257A RID: 9594
		public float ActionDelay;

		// Token: 0x0400257B RID: 9595
		public AnimalEvent OnEnter = new AnimalEvent();

		// Token: 0x0400257C RID: 9596
		public AnimalEvent OnExit = new AnimalEvent();

		// Token: 0x0400257D RID: 9597
		public AnimalEvent OnAction = new AnimalEvent();

		// Token: 0x0400257E RID: 9598
		[MinMaxRange(0f, 60f)]
		[SerializeField]
		private RangedFloat waitTime = new RangedFloat(0f, 5f);

		// Token: 0x0400257F RID: 9599
		public WayPointType pointType;

		// Token: 0x04002580 RID: 9600
		public static List<ActionZone> ActionZones;

		// Token: 0x04002581 RID: 9601
		[SerializeField]
		private List<Transform> nextTargets;

		// Token: 0x04002582 RID: 9602
		[SerializeField]
		private float stoppingDistance = 0.5f;

		// Token: 0x04002583 RID: 9603
		private Collider ZoneCollider;

		// Token: 0x04002584 RID: 9604
		[HideInInspector]
		public bool EditorShowEvents = true;

		// Token: 0x04002585 RID: 9605
		[HideInInspector]
		public bool EditorAI = true;
	}
}
