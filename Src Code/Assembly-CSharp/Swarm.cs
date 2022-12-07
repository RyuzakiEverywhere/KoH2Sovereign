using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class Swarm : MonoBehaviour
{
	// Token: 0x0600001D RID: 29 RVA: 0x000029F4 File Offset: 0x00000BF4
	private void Start()
	{
		this.angle = Random.Range(0f, 360f);
		this.lastPosition = this.GetNewPos();
		float max = this.swarmRadius / this.birdsDistance;
		for (int i = 0; i < this.birdsCount; i++)
		{
			Vector3 vector = new Vector3(Random.Range(0f, max) * this.birdsDistance, Random.Range(0f, max) * this.birdsDistance, Random.Range(0f, max) * this.birdsDistance);
			vector += base.transform.position;
			Object.Instantiate<GameObject>(this.bird, vector, base.transform.rotation).transform.parent = base.transform;
		}
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002ABC File Offset: 0x00000CBC
	private void FixedUpdate()
	{
		Vector3 newPos = this.GetNewPos();
		base.transform.position += newPos - this.lastPosition;
		this.lastPosition = newPos;
		this.angle = Mathf.MoveTowardsAngle(this.angle, this.angle + this.speed * Time.deltaTime, this.speed * Time.deltaTime);
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002B2C File Offset: 0x00000D2C
	private Vector3 GetNewPos()
	{
		Vector3 result;
		result.x = 0f;
		result.y = Mathf.Sin(this.angle * 0.017453292f) * this.amplitude;
		result.z = 0f;
		return result;
	}

	// Token: 0x04000022 RID: 34
	public GameObject bird;

	// Token: 0x04000023 RID: 35
	public int birdsCount;

	// Token: 0x04000024 RID: 36
	public float swarmRadius;

	// Token: 0x04000025 RID: 37
	public float birdsDistance;

	// Token: 0x04000026 RID: 38
	public float amplitude;

	// Token: 0x04000027 RID: 39
	public float speed;

	// Token: 0x04000028 RID: 40
	private float angle;

	// Token: 0x04000029 RID: 41
	private Vector3 lastPosition;
}
