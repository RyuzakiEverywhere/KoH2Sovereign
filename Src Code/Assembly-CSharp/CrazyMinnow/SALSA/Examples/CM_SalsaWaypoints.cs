using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200049C RID: 1180
	public class CM_SalsaWaypoints : MonoBehaviour
	{
		// Token: 0x06003E06 RID: 15878 RVA: 0x001DB3B7 File Offset: 0x001D95B7
		private void Start()
		{
			this.target.transform.position = this.waypoints[this.currentWaypoint].transform.position;
			this.currentWaypoint = this.startingWaypoint;
		}

		// Token: 0x06003E07 RID: 15879 RVA: 0x001DB3EC File Offset: 0x001D95EC
		private void Update()
		{
			this.target.transform.position = Vector3.MoveTowards(this.target.transform.position, this.waypoints[this.currentWaypoint].transform.position, Time.deltaTime * this.movementSpeed);
			if (this.matchWaypointRotation)
			{
				this.target.transform.rotation = Quaternion.RotateTowards(this.target.transform.rotation, this.waypoints[this.currentWaypoint].transform.rotation, Time.deltaTime * this.movementSpeed);
			}
		}

		// Token: 0x06003E08 RID: 15880 RVA: 0x001DB494 File Offset: 0x001D9694
		private void Salsa_OnTalkStatusChanged(SalsaStatus status)
		{
			for (int i = 0; i < this.triggers.Length; i++)
			{
				if (this.triggers[i].trigger == CM_SalsaWaypointTriggers.Trigger.Start && status.isTalking && this.triggers[i].audioClip.name == status.clipName)
				{
					this.movementSpeed = this.triggers[i].movementSpeed;
					this.SetSpeed(this.movementSpeed);
					this.SetWaypoint(this.triggers[i].waypointIndex);
				}
				if (this.triggers[i].trigger == CM_SalsaWaypointTriggers.Trigger.End && !status.isTalking && this.triggers[i].audioClip.name == status.clipName)
				{
					this.movementSpeed = this.triggers[i].movementSpeed;
					this.SetSpeed(this.movementSpeed);
					this.SetWaypoint(this.triggers[i].waypointIndex);
				}
			}
		}

		// Token: 0x06003E09 RID: 15881 RVA: 0x001DB58B File Offset: 0x001D978B
		public void SetWaypoint(int index)
		{
			if (index < this.waypoints.Length)
			{
				this.currentWaypoint = index;
			}
		}

		// Token: 0x06003E0A RID: 15882 RVA: 0x001DB59F File Offset: 0x001D979F
		public void SetSpeed(float speed)
		{
			this.movementSpeed = speed;
		}

		// Token: 0x06003E0B RID: 15883 RVA: 0x001DB5A8 File Offset: 0x001D97A8
		public void ResetSalsaWaypoints()
		{
			this.currentWaypoint = this.startingWaypoint;
			this.target.transform.position = this.waypoints[0].transform.position;
		}

		// Token: 0x04002C1D RID: 11293
		public GameObject target;

		// Token: 0x04002C1E RID: 11294
		public int startingWaypoint;

		// Token: 0x04002C1F RID: 11295
		public int currentWaypoint;

		// Token: 0x04002C20 RID: 11296
		public bool matchWaypointRotation;

		// Token: 0x04002C21 RID: 11297
		public CM_SalsaWaypointTriggers[] triggers;

		// Token: 0x04002C22 RID: 11298
		public GameObject[] waypoints;

		// Token: 0x04002C23 RID: 11299
		private float movementSpeed = 10f;

		// Token: 0x04002C24 RID: 11300
		private float startTime;

		// Token: 0x04002C25 RID: 11301
		private float journeyLength;
	}
}
