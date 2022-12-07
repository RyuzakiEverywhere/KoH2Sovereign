using System;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class Bird : MonoBehaviour
{
	// Token: 0x06000019 RID: 25 RVA: 0x000027D8 File Offset: 0x000009D8
	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		this.angleX = (float)Random.Range(0, 360);
		this.angleY = (float)Random.Range(0, 360);
		this.angleZ = (float)Random.Range(0, 360);
		this.lastPosition = this.GetNewPos();
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002834 File Offset: 0x00000A34
	private void OnAnimatorMove()
	{
		if (this.anim.GetCurrentAnimatorStateInfo(0).IsTag("NewAnim"))
		{
			if (this.canChangeAnim)
			{
				this.anim.SetInteger("AnimNum", Random.Range(0, this.animCount + 1));
				this.canChangeAnim = false;
				Debug.Log("Bird anim: " + this.anim.GetInteger("AnimNum"));
			}
		}
		else
		{
			this.canChangeAnim = true;
		}
		Vector3 newPos = this.GetNewPos();
		base.transform.position += newPos - this.lastPosition;
		this.lastPosition = newPos;
		this.angleX = Mathf.MoveTowardsAngle(this.angleX, this.angleX + this.speedX * Time.deltaTime, this.speedX * Time.deltaTime);
		this.angleY = Mathf.MoveTowardsAngle(this.angleY, this.angleY + this.speedY * Time.deltaTime, this.speedY * Time.deltaTime);
		this.angleZ = Mathf.MoveTowardsAngle(this.angleZ, this.angleZ + this.speedZ * Time.deltaTime, this.speedZ * Time.deltaTime);
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002978 File Offset: 0x00000B78
	private Vector3 GetNewPos()
	{
		Vector3 result;
		result.x = Mathf.Sin(this.angleX * 0.017453292f) * this.amplitudeX;
		result.y = Mathf.Sin(this.angleY * 0.017453292f) * this.amplitudeY;
		result.z = Mathf.Sin(this.angleZ * 0.017453292f) * this.amplitudeZ;
		return result;
	}

	// Token: 0x04000015 RID: 21
	public int animCount = 2;

	// Token: 0x04000016 RID: 22
	public float speedX;

	// Token: 0x04000017 RID: 23
	public float speedY;

	// Token: 0x04000018 RID: 24
	public float speedZ;

	// Token: 0x04000019 RID: 25
	public float amplitudeX;

	// Token: 0x0400001A RID: 26
	public float amplitudeY;

	// Token: 0x0400001B RID: 27
	public float amplitudeZ;

	// Token: 0x0400001C RID: 28
	private Animator anim;

	// Token: 0x0400001D RID: 29
	private bool canChangeAnim;

	// Token: 0x0400001E RID: 30
	private float angleX;

	// Token: 0x0400001F RID: 31
	private float angleY;

	// Token: 0x04000020 RID: 32
	private float angleZ;

	// Token: 0x04000021 RID: 33
	private Vector3 lastPosition;
}
