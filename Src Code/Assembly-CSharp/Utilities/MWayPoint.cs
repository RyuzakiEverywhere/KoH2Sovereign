using System;
using System.Collections.Generic;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000473 RID: 1139
	public class MWayPoint : MonoBehaviour, IWayPoint
	{
		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06003B7F RID: 15231 RVA: 0x001C7455 File Offset: 0x001C5655
		// (set) Token: 0x06003B80 RID: 15232 RVA: 0x001C745D File Offset: 0x001C565D
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

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06003B81 RID: 15233 RVA: 0x001C7466 File Offset: 0x001C5666
		public float WaitTime
		{
			get
			{
				return this.waitTime.RandomValue;
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06003B82 RID: 15234 RVA: 0x001C7473 File Offset: 0x001C5673
		// (set) Token: 0x06003B83 RID: 15235 RVA: 0x001C747B File Offset: 0x001C567B
		public List<Transform> NextTargets
		{
			get
			{
				return this.nextWayPoints;
			}
			set
			{
				this.nextWayPoints = value;
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06003B84 RID: 15236 RVA: 0x001C7484 File Offset: 0x001C5684
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

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06003B85 RID: 15237 RVA: 0x001C74B2 File Offset: 0x001C56B2
		public WayPointType PointType
		{
			get
			{
				return this.pointType;
			}
		}

		// Token: 0x06003B86 RID: 15238 RVA: 0x001C74BA File Offset: 0x001C56BA
		private void OnEnable()
		{
			if (MWayPoint.WayPoints == null)
			{
				MWayPoint.WayPoints = new List<MWayPoint>();
			}
			MWayPoint.WayPoints.Add(this);
		}

		// Token: 0x06003B87 RID: 15239 RVA: 0x001C74D8 File Offset: 0x001C56D8
		private void OnDisable()
		{
			MWayPoint.WayPoints.Remove(this);
		}

		// Token: 0x06003B88 RID: 15240 RVA: 0x001C74E6 File Offset: 0x001C56E6
		public void TargetArrived(Component target)
		{
			this.OnTargetArrived.Invoke(target);
		}

		// Token: 0x06003B89 RID: 15241 RVA: 0x001C74F4 File Offset: 0x001C56F4
		public static Transform GetWaypoint()
		{
			if (MWayPoint.WayPoints != null && MWayPoint.WayPoints.Count > 1)
			{
				return MWayPoint.WayPoints[Random.Range(0, MWayPoint.WayPoints.Count)].transform;
			}
			return null;
		}

		// Token: 0x06003B8A RID: 15242 RVA: 0x001C752C File Offset: 0x001C572C
		public static Transform GetWaypoint(WayPointType pointType)
		{
			if (MWayPoint.WayPoints == null || MWayPoint.WayPoints.Count <= 1)
			{
				return null;
			}
			MWayPoint mwayPoint = MWayPoint.WayPoints.Find((MWayPoint item) => item.pointType == pointType);
			if (!mwayPoint)
			{
				return null;
			}
			return mwayPoint.transform;
		}

		// Token: 0x04002B4A RID: 11082
		public static List<MWayPoint> WayPoints;

		// Token: 0x04002B4B RID: 11083
		[SerializeField]
		private float stoppingDistance = 1f;

		// Token: 0x04002B4C RID: 11084
		[MinMaxRange(0f, 60f)]
		public RangedFloat waitTime = new RangedFloat(0f, 15f);

		// Token: 0x04002B4D RID: 11085
		public WayPointType pointType;

		// Token: 0x04002B4E RID: 11086
		[SerializeField]
		private List<Transform> nextWayPoints;

		// Token: 0x04002B4F RID: 11087
		[Space]
		[Space]
		public ComponentEvent OnTargetArrived = new ComponentEvent();

		// Token: 0x04002B50 RID: 11088
		public bool debug = true;
	}
}
