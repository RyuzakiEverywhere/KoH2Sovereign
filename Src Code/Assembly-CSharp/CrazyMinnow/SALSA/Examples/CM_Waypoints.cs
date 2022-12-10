using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x0200049E RID: 1182
	public class CM_Waypoints : MonoBehaviour
	{
		// Token: 0x06003E0E RID: 15886 RVA: 0x001DB5EC File Offset: 0x001D97EC
		private void Start()
		{
			if (this.waypoints != null)
			{
				this.curDestIndex = this.StartingWaypoint;
				this.curDest = this.waypoints[this.curDestIndex].waypoint.transform.position;
				this.target.transform.position = this.waypoints[this.curDestIndex].waypoint.transform.position;
			}
			else
			{
				Debug.LogError("Add waypoints to the waypoints array");
			}
			this.timer = Time.time;
		}

		// Token: 0x06003E0F RID: 15887 RVA: 0x001DB674 File Offset: 0x001D9874
		private void Update()
		{
			this.target.transform.position = Vector3.MoveTowards(this.target.transform.position, this.curDest, Time.deltaTime * this.speed);
			if (this.waypoints[this.curDestIndex].direction == CM_WaypointItems.Direction.Left)
			{
				this.target.transform.eulerAngles = new Vector3(this.target.transform.eulerAngles.x, 0f, this.target.transform.eulerAngles.z);
			}
			if (this.waypoints[this.curDestIndex].direction == CM_WaypointItems.Direction.Right)
			{
				this.target.transform.eulerAngles = new Vector3(this.target.transform.eulerAngles.x, 180f, this.target.transform.eulerAngles.z);
			}
			this.WaypointCheck();
		}

		// Token: 0x06003E10 RID: 15888 RVA: 0x001DB770 File Offset: 0x001D9970
		private void WaypointCheck()
		{
			switch (this.animationType)
			{
			case CM_Waypoints.AnimationType.Once:
				if (this.target.transform.position == this.curDest)
				{
					if (this.getTime)
					{
						this.timer = Time.time;
						this.getTime = false;
					}
					if (Time.time > this.timer + (float)this.waypoints[this.curDestIndex].delay)
					{
						if (this.countUp && this.curDestIndex < this.waypoints.Length - 1)
						{
							this.curDestIndex++;
						}
						this.curDest = this.waypoints[this.curDestIndex].waypoint.transform.position;
						this.getTime = true;
						return;
					}
				}
				break;
			case CM_Waypoints.AnimationType.Repeat:
				if (this.target.transform.position == this.curDest)
				{
					if (this.getTime)
					{
						this.timer = Time.time;
						this.getTime = false;
					}
					if (Time.time > this.timer + (float)this.waypoints[this.curDestIndex].delay)
					{
						if (this.countUp)
						{
							if (this.curDestIndex < this.waypoints.Length - 1)
							{
								this.curDestIndex++;
							}
							else
							{
								this.curDestIndex = 0;
								this.target.transform.position = this.waypoints[this.curDestIndex].waypoint.transform.position;
							}
						}
						this.curDest = this.waypoints[this.curDestIndex].waypoint.transform.position;
						this.getTime = true;
						return;
					}
				}
				break;
			case CM_Waypoints.AnimationType.PingPong_Once:
				if (this.target.transform.position == this.curDest)
				{
					if (this.getTime)
					{
						this.timer = Time.time;
						this.getTime = false;
					}
					if (Time.time > this.timer + (float)this.waypoints[this.curDestIndex].delay)
					{
						if (this.countUp)
						{
							if (this.curDestIndex < this.waypoints.Length - 1)
							{
								this.curDestIndex++;
							}
							else
							{
								this.countUp = false;
							}
						}
						if (!this.countUp && this.curDestIndex > 0)
						{
							this.curDestIndex--;
						}
						this.curDest = this.waypoints[this.curDestIndex].waypoint.transform.position;
						this.getTime = true;
						return;
					}
				}
				break;
			case CM_Waypoints.AnimationType.PingPong_Repeat:
				if (this.target.transform.position == this.curDest)
				{
					if (this.getTime)
					{
						this.timer = Time.time;
						this.getTime = false;
					}
					if (Time.time > this.timer + (float)this.waypoints[this.curDestIndex].delay)
					{
						if (this.countUp)
						{
							if (this.curDestIndex < this.waypoints.Length - 1)
							{
								this.curDestIndex++;
							}
							else
							{
								this.countUp = false;
							}
						}
						if (!this.countUp)
						{
							if (this.curDestIndex > 0)
							{
								this.curDestIndex--;
							}
							else
							{
								this.countUp = true;
								this.curDestIndex++;
							}
						}
						this.curDest = this.waypoints[this.curDestIndex].waypoint.transform.position;
						this.getTime = true;
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06003E11 RID: 15889 RVA: 0x001DBAF6 File Offset: 0x001D9CF6
		public void SetAnimationType(CM_Waypoints.AnimationType animType)
		{
			this.animationType = animType;
		}

		// Token: 0x04002C29 RID: 11305
		public GameObject target;

		// Token: 0x04002C2A RID: 11306
		public CM_Waypoints.AnimationType animationType = CM_Waypoints.AnimationType.PingPong_Repeat;

		// Token: 0x04002C2B RID: 11307
		public int StartingWaypoint;

		// Token: 0x04002C2C RID: 11308
		public float speed = 3f;

		// Token: 0x04002C2D RID: 11309
		public CM_WaypointItems[] waypoints;

		// Token: 0x04002C2E RID: 11310
		private int curDestIndex;

		// Token: 0x04002C2F RID: 11311
		private Vector3 curDest;

		// Token: 0x04002C30 RID: 11312
		private bool countUp = true;

		// Token: 0x04002C31 RID: 11313
		private float timer;

		// Token: 0x04002C32 RID: 11314
		private bool getTime = true;

		// Token: 0x0200097C RID: 2428
		public enum AnimationType
		{
			// Token: 0x04004416 RID: 17430
			Once,
			// Token: 0x04004417 RID: 17431
			Repeat,
			// Token: 0x04004418 RID: 17432
			PingPong_Once,
			// Token: 0x04004419 RID: 17433
			PingPong_Repeat
		}
	}
}
