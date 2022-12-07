using System;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class MouseOrbit : MonoBehaviour
{
	// Token: 0x06000036 RID: 54 RVA: 0x00003188 File Offset: 0x00001388
	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.rotationYAxis = eulerAngles.y;
		this.rotationXAxis = eulerAngles.x;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x000031BC File Offset: 0x000013BC
	private void LateUpdate()
	{
		if (this.target)
		{
			if (Input.GetMouseButton(1))
			{
				this.start = false;
				this.velocityX += this.xSpeed * Input.GetAxis("Mouse X") * 0.02f;
				this.velocityY += this.ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
			}
			if (this.start)
			{
				return;
			}
			this.rotationYAxis += this.velocityX;
			this.rotationXAxis -= this.velocityY;
			this.rotationXAxis = MouseOrbit.ClampAngle(this.rotationXAxis, this.yMinLimit, this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.rotationXAxis, this.rotationYAxis, 0f);
			Vector3 point = new Vector3(0f, 0f, -this.distance);
			Vector3 position = rotation * point + this.target.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
			this.velocityX = Mathf.Lerp(this.velocityX, 0f, Time.deltaTime * this.smoothTime);
			this.velocityY = Mathf.Lerp(this.velocityY, 0f, Time.deltaTime * this.smoothTime);
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis < 0f && this.distance < this.distanceMax)
			{
				this.distance += this.scrollSpeed;
				return;
			}
			if (axis > 0f && this.distance > this.distanceMin)
			{
				this.distance -= this.scrollSpeed;
			}
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002EA7 File Offset: 0x000010A7
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x0400005B RID: 91
	public Transform target;

	// Token: 0x0400005C RID: 92
	public float distance = 5f;

	// Token: 0x0400005D RID: 93
	public float xSpeed = 120f;

	// Token: 0x0400005E RID: 94
	public float ySpeed = 120f;

	// Token: 0x0400005F RID: 95
	public float scrollSpeed = 1f;

	// Token: 0x04000060 RID: 96
	public float yMinLimit = -20f;

	// Token: 0x04000061 RID: 97
	public float yMaxLimit = 80f;

	// Token: 0x04000062 RID: 98
	public float distanceMin = 0.5f;

	// Token: 0x04000063 RID: 99
	public float distanceMax = 15f;

	// Token: 0x04000064 RID: 100
	public float smoothTime = 2f;

	// Token: 0x04000065 RID: 101
	private float rotationYAxis;

	// Token: 0x04000066 RID: 102
	private float rotationXAxis;

	// Token: 0x04000067 RID: 103
	private float velocityX;

	// Token: 0x04000068 RID: 104
	private float velocityY;

	// Token: 0x04000069 RID: 105
	private bool start = true;
}
