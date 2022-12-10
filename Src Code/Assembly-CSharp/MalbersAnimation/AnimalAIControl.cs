using System;
using System.Collections;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace MalbersAnimations
{
	// Token: 0x020003C2 RID: 962
	[RequireComponent(typeof(Animal))]
	public class AnimalAIControl : MonoBehaviour
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x060036A7 RID: 13991 RVA: 0x001B32FE File Offset: 0x001B14FE
		public NavMeshAgent Agent
		{
			get
			{
				if (this.agent == null)
				{
					this.agent = base.GetComponentInChildren<NavMeshAgent>();
				}
				return this.agent;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x060036A8 RID: 13992 RVA: 0x001B3320 File Offset: 0x001B1520
		// (set) Token: 0x060036A9 RID: 13993 RVA: 0x001B3328 File Offset: 0x001B1528
		public float StoppingDistance
		{
			get
			{
				return this.stoppingDistance;
			}
			set
			{
				NavMeshAgent navMeshAgent = this.Agent;
				this.stoppingDistance = value;
				navMeshAgent.stoppingDistance = value;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060036AA RID: 13994 RVA: 0x001B334C File Offset: 0x001B154C
		public bool TargetisMoving
		{
			get
			{
				return this.target != null && (this.target.position - this.TargetLastPosition).magnitude > 0.001f;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060036AB RID: 13995 RVA: 0x001B338E File Offset: 0x001B158E
		public bool AgentActive
		{
			get
			{
				return this.Agent.isOnNavMesh && this.Agent.enabled;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060036AC RID: 13996 RVA: 0x001B33AA File Offset: 0x001B15AA
		// (set) Token: 0x060036AD RID: 13997 RVA: 0x001B33B2 File Offset: 0x001B15B2
		public bool IsWaiting { get; protected set; }

		// Token: 0x060036AE RID: 13998 RVA: 0x001B33BB File Offset: 0x001B15BB
		private void Start()
		{
			this.StartAgent();
		}

		// Token: 0x060036AF RID: 13999 RVA: 0x001B33C4 File Offset: 0x001B15C4
		protected virtual void StartAgent()
		{
			this.animal = base.GetComponent<Animal>();
			this.animal.OnAnimationChange.AddListener(new UnityAction<int>(this.OnAnimationChanged));
			this.DoingAnAction = (this.isFlyingOffMesh = false);
			this.Agent.updateRotation = false;
			this.Agent.updatePosition = false;
			this.DefaultStopDistance = this.StoppingDistance;
			this.Agent.stoppingDistance = this.StoppingDistance;
			this.SetTarget(this.target);
			this.IsWaiting = false;
		}

		// Token: 0x060036B0 RID: 14000 RVA: 0x001B3454 File Offset: 0x001B1654
		private void Update()
		{
			if (this.isFlyingOffMesh)
			{
				return;
			}
			if (this.Stopped)
			{
				return;
			}
			if (this.animal.Fly || this.animal.Swim)
			{
				this.FreeMovement();
			}
			else if (this.AgentActive)
			{
				this.Agent.nextPosition = this.agent.transform.position;
				if (this.IsWaiting)
				{
					return;
				}
				if (this.targetPosition == AnimalAIControl.NullVector)
				{
					this.StopAnimal();
				}
				else
				{
					this.UpdateAgent();
				}
			}
			if (this.target)
			{
				if (this.TargetisMoving)
				{
					this.UpdateTargetTransform();
				}
				this.TargetLastPosition = this.target.position;
			}
		}

		// Token: 0x060036B1 RID: 14001 RVA: 0x001B3510 File Offset: 0x001B1710
		private void FreeMovement()
		{
			if (this.IsWaiting)
			{
				return;
			}
			if (this.target == null || this.targetPosition == AnimalAIControl.NullVector)
			{
				return;
			}
			this.RemainingDistance = (this.target ? Vector3.Distance(this.animal.transform.position, this.target.position) : 0f);
			Vector3 move = this.target.position - this.animal.transform.position;
			this.animal.Move(move, true);
			Debug.DrawRay(this.animal.transform.position, move.normalized, Color.white);
			if (this.RemainingDistance < this.StoppingDistance)
			{
				if (this.NextWayPoint != null && this.NextWayPoint.PointType != WayPointType.Air && this.animal.Fly)
				{
					this.animal.SetFly(false);
				}
				this.CheckNextTarget();
			}
		}

		// Token: 0x060036B2 RID: 14002 RVA: 0x001B3614 File Offset: 0x001B1814
		private void CheckNextTarget()
		{
			if (this.isActionZone && !this.DoingAnAction)
			{
				this.animal.Action = true;
				this.animal.Stop();
				if (this.isActionZone.MoveToExitAction)
				{
					float waitTime = this.isActionZone.WaitTime;
					this.Debuging(string.Concat(new object[]
					{
						base.name,
						"is Waiting ",
						waitTime,
						" seconds to finish a 'Move to Exit' Action"
					}));
					this.animal.Invoke("WakeAnimal", waitTime);
					return;
				}
			}
			else
			{
				this.SetNextTarget();
			}
		}

		// Token: 0x060036B3 RID: 14003 RVA: 0x001B36B4 File Offset: 0x001B18B4
		protected virtual void OnAnimationChanged(int animTag)
		{
			bool flag = animTag == AnimTag.Action;
			if (flag != this.DoingAnAction)
			{
				this.DoingAnAction = flag;
				if (this.DoingAnAction)
				{
					this.OnActionStart.Invoke();
					this.Debuging(base.name + " has started an ACTION");
					this.IsWaiting = true;
				}
				else
				{
					this.OnActionEnd.Invoke();
					this.Debuging(base.name + " has ended an ACTION");
					if (!this.EnterOFFMESH)
					{
						this.SetNextTarget();
					}
					else
					{
						this.IsWaiting = false;
					}
				}
			}
			if (animTag == AnimTag.Jump)
			{
				this.animal.MovementRight = 0f;
			}
			if (animTag == AnimTag.Locomotion || animTag == AnimTag.Idle)
			{
				if (this.animal.canFly && this.flyPending && !this.animal.Fly && this.NextWayPoint.PointType == WayPointType.Air)
				{
					this.animal.SetFly(true);
					this.flyPending = false;
					return;
				}
				if (!this.Agent.enabled)
				{
					this.Agent.enabled = true;
					this.Agent.ResetPath();
					this.EnterOFFMESH = false;
					if (this.targetPosition != AnimalAIControl.NullVector)
					{
						this.Agent.SetDestination(this.targetPosition);
						this.Agent.isStopped = false;
						return;
					}
				}
			}
			else if (this.Agent.enabled)
			{
				this.Agent.enabled = false;
				string str = "not on Locomotion or Idle";
				if (animTag == AnimTag.Action)
				{
					str = "doing an Action";
				}
				if (animTag == AnimTag.Jump)
				{
					str = "Jumping";
				}
				if (animTag == AnimTag.Fall)
				{
					str = "Falling";
				}
				if (animTag == AnimTag.Recover)
				{
					str = "Recovering";
				}
				this.Debuging("Disable Agent. " + base.name + " is " + str);
			}
		}

		// Token: 0x060036B4 RID: 14004 RVA: 0x001B3888 File Offset: 0x001B1A88
		private void SetNextTarget()
		{
			if (this.WaitToNextTargetC != null)
			{
				base.StopCoroutine(this.WaitToNextTargetC);
			}
			if (this.NextWayPoint != null)
			{
				this.WaitToNextTargetC = this.WaitToNextTarget(this.NextWayPoint.WaitTime, this.NextWayPoint.NextTarget);
				base.StartCoroutine(this.WaitToNextTargetC);
			}
		}

		// Token: 0x060036B5 RID: 14005 RVA: 0x001B38E0 File Offset: 0x001B1AE0
		protected virtual void UpdateAgent()
		{
			Vector3 move = Vector3.zero;
			this.RemainingDistance = this.Agent.remainingDistance;
			if (this.Agent.pathPending || Mathf.Abs(this.RemainingDistance) <= 0.1f)
			{
				this.RemainingDistance = float.PositiveInfinity;
				this.UpdateTargetTransform();
			}
			if (this.RemainingDistance > this.StoppingDistance)
			{
				move = this.Agent.desiredVelocity;
				this.DoingAnAction = false;
			}
			else
			{
				this.OnTargetPositionArrived.Invoke(this.targetPosition);
				if (this.target)
				{
					this.OnTargetArrived.Invoke(this.target);
					if (this.isWayPoint)
					{
						this.isWayPoint.TargetArrived(this);
					}
				}
				this.targetPosition = AnimalAIControl.NullVector;
				this.agent.isStopped = true;
				this.CheckNextTarget();
			}
			this.animal.Move(move, true);
			if (this.AutoSpeed)
			{
				this.AutomaticSpeed();
			}
			this.CheckOffMeshLinks();
		}

		// Token: 0x060036B6 RID: 14006 RVA: 0x001B39DD File Offset: 0x001B1BDD
		protected virtual void WakeAnimal()
		{
			this.animal.WakeAnimal();
			this.IsWaiting = false;
		}

		// Token: 0x060036B7 RID: 14007 RVA: 0x001B39F4 File Offset: 0x001B1BF4
		protected virtual void CheckOffMeshLinks()
		{
			if (this.Agent.isOnOffMeshLink && !this.EnterOFFMESH)
			{
				this.EnterOFFMESH = true;
				OffMeshLinkData currentOffMeshLinkData = this.Agent.currentOffMeshLinkData;
				if (currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeManual)
				{
					OffMeshLink offMeshLink = currentOffMeshLinkData.offMeshLink;
					if (offMeshLink.GetComponentInParent<ActionZone>() && !this.DoingAnAction)
					{
						this.animal.Action = (this.DoingAnAction = true);
						return;
					}
					float sqrMagnitude = (base.transform.position - offMeshLink.endTransform.position).sqrMagnitude;
					float sqrMagnitude2 = (base.transform.position - offMeshLink.startTransform.position).sqrMagnitude;
					Transform transform = (sqrMagnitude < sqrMagnitude2) ? offMeshLink.endTransform : offMeshLink.startTransform;
					Transform transform2 = (sqrMagnitude > sqrMagnitude2) ? offMeshLink.endTransform : offMeshLink.startTransform;
					base.StartCoroutine(MalbersTools.AlignTransform_Rotation(base.transform, transform.rotation, 0.15f, null));
					if (this.animal.canFly && offMeshLink.CompareTag("Fly"))
					{
						this.Debuging(base.name + ": Fly OffMesh");
						base.StartCoroutine(this.CFlyOffMesh(transform2));
						return;
					}
					if (offMeshLink.area == 2)
					{
						this.animal.SetJump();
						return;
					}
				}
				else if (currentOffMeshLinkData.linkType == OffMeshLinkType.LinkTypeJumpAcross)
				{
					this.animal.SetJump();
				}
			}
		}

		// Token: 0x060036B8 RID: 14008 RVA: 0x001B3B67 File Offset: 0x001B1D67
		protected virtual IEnumerator WaitToNextTarget(float time, Transform NextTarget)
		{
			if (this.isActionZone && this.isActionZone.MoveToExitAction)
			{
				time = 0f;
			}
			if (time > 0f)
			{
				this.IsWaiting = true;
				this.Debuging(base.name + " is waiting " + time.ToString("F2") + " seconds");
				this.animal.Move(Vector3.zero, true);
				yield return new WaitForSeconds(time);
			}
			this.IsWaiting = false;
			this.SetTarget(NextTarget);
			yield return null;
			yield break;
		}

		// Token: 0x060036B9 RID: 14009 RVA: 0x001B3B84 File Offset: 0x001B1D84
		protected virtual void AutomaticSpeed()
		{
			if (this.RemainingDistance < this.ToTrot)
			{
				this.animal.Speed1 = true;
				return;
			}
			if (this.RemainingDistance < this.ToRun)
			{
				this.animal.Speed2 = true;
				return;
			}
			if (this.RemainingDistance > this.ToRun)
			{
				this.animal.Speed3 = true;
			}
		}

		// Token: 0x060036BA RID: 14010 RVA: 0x001B3BE4 File Offset: 0x001B1DE4
		public virtual void SetTarget(Transform target)
		{
			if (target == null)
			{
				this.StopAnimal();
				return;
			}
			this.target = target;
			this.targetPosition = target.position;
			this.isActionZone = target.GetComponent<ActionZone>();
			this.isWayPoint = target.GetComponent<MWayPoint>();
			this.NextWayPoint = target.GetComponent<IWayPoint>();
			this.Stopped = false;
			this.StoppingDistance = ((this.NextWayPoint != null) ? this.NextWayPoint.StoppingDistance : this.DefaultStopDistance);
			this.CheckAirTarget();
			this.Debuging(base.name + " is travelling to : " + target.name);
			if (!this.Agent.isOnNavMesh)
			{
				return;
			}
			this.Agent.enabled = true;
			this.Agent.SetDestination(this.targetPosition);
			this.Agent.isStopped = false;
		}

		// Token: 0x060036BB RID: 14011 RVA: 0x001B3CBC File Offset: 0x001B1EBC
		private void CheckAirTarget()
		{
			if (this.NextWayPoint != null && this.NextWayPoint.PointType == WayPointType.Air && this.animal.canFly)
			{
				int currentAnimState = this.animal.CurrentAnimState;
				if (currentAnimState == AnimTag.Locomotion || currentAnimState == AnimTag.Idle)
				{
					this.animal.SetFly(true);
					this.flyPending = false;
					return;
				}
				this.flyPending = true;
			}
		}

		// Token: 0x060036BC RID: 14012 RVA: 0x001B3D24 File Offset: 0x001B1F24
		public virtual void UpdateTargetTransform()
		{
			if (!this.Agent.isOnNavMesh)
			{
				return;
			}
			if (this.target == null)
			{
				return;
			}
			this.targetPosition = this.target.position;
			this.Agent.SetDestination(this.targetPosition);
			if (this.Agent.isStopped)
			{
				this.Agent.isStopped = false;
			}
		}

		// Token: 0x060036BD RID: 14013 RVA: 0x001B3D8C File Offset: 0x001B1F8C
		public virtual void StopAnimal()
		{
			if (this.Agent && this.Agent.isOnNavMesh)
			{
				this.Agent.isStopped = true;
			}
			this.targetPosition = AnimalAIControl.NullVector;
			base.StopAllCoroutines();
			this.DoingAnAction = false;
			this.animal.InterruptAction();
			if (this.animal)
			{
				this.animal.Stop();
			}
			this.IsWaiting = (this.isFlyingOffMesh = false);
			this.Stopped = true;
		}

		// Token: 0x060036BE RID: 14014 RVA: 0x001B3E14 File Offset: 0x001B2014
		public virtual void SetDestination(Vector3 point)
		{
			this.targetPosition = point;
			this.target = null;
			this.StoppingDistance = this.DefaultStopDistance;
			if (!this.Agent.isOnNavMesh || !this.Agent.enabled)
			{
				return;
			}
			this.Agent.SetDestination(this.targetPosition);
			this.Agent.isStopped = false;
			this.Stopped = false;
			this.Debuging(base.name + " is travelling to : " + point);
		}

		// Token: 0x060036BF RID: 14015 RVA: 0x001B3E97 File Offset: 0x001B2097
		protected void Debuging(string Log)
		{
			if (this.debug)
			{
				Debug.Log(Log);
			}
			this.OnDebug.Invoke(Log);
		}

		// Token: 0x060036C0 RID: 14016 RVA: 0x001B3EB3 File Offset: 0x001B20B3
		internal IEnumerator CFlyOffMesh(Transform target)
		{
			this.animal.SetFly(true);
			this.flyPending = false;
			this.isFlyingOffMesh = true;
			float distance = float.MaxValue;
			this.agent.enabled = false;
			while (distance > this.agent.stoppingDistance)
			{
				this.animal.Move(target.position - this.animal.transform.position, true);
				distance = Vector3.Distance(this.animal.transform.position, target.position);
				yield return null;
			}
			this.animal.Stop();
			this.animal.SetFly(false);
			this.isFlyingOffMesh = false;
			yield break;
		}

		// Token: 0x04002671 RID: 9841
		private NavMeshAgent agent;

		// Token: 0x04002672 RID: 9842
		protected Animal animal;

		// Token: 0x04002673 RID: 9843
		protected ActionZone isActionZone;

		// Token: 0x04002674 RID: 9844
		protected MWayPoint isWayPoint;

		// Token: 0x04002675 RID: 9845
		protected static Vector3 NullVector = MalbersTools.NullVector;

		// Token: 0x04002676 RID: 9846
		protected Vector3 targetPosition = AnimalAIControl.NullVector;

		// Token: 0x04002677 RID: 9847
		protected Vector3 TargetLastPosition = AnimalAIControl.NullVector;

		// Token: 0x04002678 RID: 9848
		protected float RemainingDistance;

		// Token: 0x04002679 RID: 9849
		protected float DefaultStopDistance;

		// Token: 0x0400267A RID: 9850
		protected bool EnterOFFMESH;

		// Token: 0x0400267B RID: 9851
		protected bool DoingAnAction;

		// Token: 0x0400267C RID: 9852
		protected bool EnterAction;

		// Token: 0x0400267D RID: 9853
		protected bool Stopped;

		// Token: 0x0400267E RID: 9854
		private bool isFlyingOffMesh;

		// Token: 0x0400267F RID: 9855
		internal IWayPoint NextWayPoint;

		// Token: 0x04002680 RID: 9856
		protected bool flyPending;

		// Token: 0x04002681 RID: 9857
		[SerializeField]
		protected float stoppingDistance = 0.6f;

		// Token: 0x04002682 RID: 9858
		[SerializeField]
		protected Transform target;

		// Token: 0x04002683 RID: 9859
		public bool AutoSpeed = true;

		// Token: 0x04002684 RID: 9860
		public float ToTrot = 6f;

		// Token: 0x04002685 RID: 9861
		public float ToRun = 8f;

		// Token: 0x04002686 RID: 9862
		public bool debug;

		// Token: 0x04002687 RID: 9863
		[Space]
		public Vector3Event OnTargetPositionArrived = new Vector3Event();

		// Token: 0x04002688 RID: 9864
		public TransformEvent OnTargetArrived = new TransformEvent();

		// Token: 0x04002689 RID: 9865
		public UnityEvent OnActionStart = new UnityEvent();

		// Token: 0x0400268A RID: 9866
		public UnityEvent OnActionEnd = new UnityEvent();

		// Token: 0x0400268B RID: 9867
		public StringEvent OnDebug = new StringEvent();

		// Token: 0x0400268D RID: 9869
		private IEnumerator WaitToNextTargetC;

		// Token: 0x0400268E RID: 9870
		[HideInInspector]
		public bool showevents;
	}
}
