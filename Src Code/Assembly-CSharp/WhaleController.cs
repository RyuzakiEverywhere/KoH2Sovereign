using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class WhaleController : MonoBehaviour
{
	// Token: 0x06000245 RID: 581 RVA: 0x00021578 File Offset: 0x0001F778
	private void Start()
	{
		this.waterLevel = MapData.GetWaterLevel();
		base.transform.position = new Vector3(0f, -this.waterLevel, 0f);
		this.ui = WorldUI.Get();
		this.SetAnimation();
		if (Camera.main == null)
		{
			this.active = false;
			Debug.Log("No camera");
		}
		if (this.model == null)
		{
			this.active = false;
			Debug.Log("No model");
			return;
		}
		this.size = this.model.GetComponent<SkinnedMeshRenderer>().localBounds.size;
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00021620 File Offset: 0x0001F820
	private void SetAnimation()
	{
		this.animator = base.GetComponent<Animation>();
		if (this.animator == null)
		{
			this.active = false;
			Debug.Log("No animation component");
			return;
		}
		foreach (object obj in this.animator)
		{
			((AnimationState)obj).speed = this.speed;
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x000216A8 File Offset: 0x0001F8A8
	private IEnumerator Move(Vector3 direction, float moveDuration = 10f)
	{
		this.isMoving = true;
		float time = Time.time;
		if (this.animator != null && !this.animator.isPlaying)
		{
			this.animator.Play(this.swim.name);
		}
		float distanceTravelled = 0f;
		do
		{
			float num = Time.time - time;
			time = Time.time;
			float num2 = Mathf.Min(num / moveDuration, 1f - distanceTravelled);
			distanceTravelled += num2;
			base.transform.position = new Vector3(base.transform.position.x + num2 * direction.x, base.transform.position.y + num2 * direction.y, base.transform.position.z + num2 * direction.z);
			yield return null;
		}
		while (distanceTravelled < 1f);
		this.isMoving = false;
		yield break;
	}

	// Token: 0x06000248 RID: 584 RVA: 0x000216C5 File Offset: 0x0001F8C5
	private IEnumerator PlayAnimation(string name)
	{
		if (this.animator[name] != null)
		{
			if (this.animationDelay != 0f)
			{
				yield return new WaitForSeconds(this.animationDelay);
			}
			this.isPlayingAnimation = true;
			this.animator[name].speed = this.speed;
			this.animator.Play(name);
			yield return base.StartCoroutine(this.Move(base.transform.forward * this.distanceToTravel, this.animator[name].length));
			this.isPlayingAnimation = false;
		}
		yield break;
	}

	// Token: 0x06000249 RID: 585 RVA: 0x000216DB File Offset: 0x0001F8DB
	private IEnumerator AppearSequence()
	{
		Vector3 vector = base.transform.forward * (this.waterLevel + this.size.x) / (3f * this.speed);
		if (this.animations.Count != 0)
		{
			int index = Random.Range(0, this.animations.Count);
			if (this.animations[index] != null)
			{
				string animationToPlay = this.animations[index].name;
				Vector3 direction = new Vector3(vector.x * this.distanceToTravel, this.waterLevel + this.offset, vector.z * this.distanceToTravel);
				Vector3 down = new Vector3(vector.x * this.distanceToTravel, -this.waterLevel - this.offset, vector.z * this.distanceToTravel);
				yield return base.StartCoroutine(this.Move(direction, 10f));
				yield return base.StartCoroutine(this.PlayAnimation(animationToPlay));
				yield return base.StartCoroutine(this.Move(down, 10f));
				animationToPlay = null;
				down = default(Vector3);
			}
		}
		this.isMoving = false;
		this.isWaiting = true;
		this.elapsedTime = 0f;
		yield break;
	}

	// Token: 0x0600024A RID: 586 RVA: 0x000216EC File Offset: 0x0001F8EC
	private Vector3[] GetPointsInRadius(Vector3 pt, float radius, int numOfPoints)
	{
		Vector3[] array = new Vector3[numOfPoints];
		for (int i = 0; i < numOfPoints; i++)
		{
			Vector3 vector = pt;
			float num = (float)i / (float)numOfPoints * 360f;
			vector.x += (radius + this.waterLevel) * Mathf.Cos(3.1415927f * num / 180f) + this.size.y;
			vector.z += (radius + this.waterLevel) * Mathf.Sin(3.1415927f * num / 180f) + this.size.y;
			array[i] = vector;
		}
		return array;
	}

	// Token: 0x0600024B RID: 587 RVA: 0x0002178C File Offset: 0x0001F98C
	private void Update()
	{
		if (this.isPlayingAnimation && !this.animator.isPlaying)
		{
			this.isPlayingAnimation = this.animator.isPlaying;
		}
		if (this.isPlayingAnimation || this.isMoving || !this.active)
		{
			return;
		}
		if (this.ui == null)
		{
			return;
		}
		this.elapsedTime += Time.deltaTime;
		if (this.elapsedTime < this.spawnDelay && this.isWaiting)
		{
			return;
		}
		this.isWaiting = false;
		Vector3 ptLookAt = this.ui.ptLookAt;
		ptLookAt.x += Random.Range(0f, (float)this.radius);
		ptLookAt.z += Random.Range(0f, (float)this.radius);
		if (Common.GetTerrainHeight(ptLookAt, null, false) < this.waterLevel)
		{
			int numOfPoints = 12;
			float num = 5f;
			int num2 = (int)((float)this.radius / num);
			for (int i = 0; i < num2; i++)
			{
				Vector3[] pointsInRadius = this.GetPointsInRadius(ptLookAt, (float)this.radius - (float)i * num, numOfPoints);
				for (int j = 0; j < pointsInRadius.Length; j++)
				{
					if (Common.GetTerrainHeight(pointsInRadius[j], null, false) > this.waterLevel)
					{
						return;
					}
				}
			}
			base.transform.position = new Vector3(ptLookAt.x, 0f, ptLookAt.z);
			base.transform.Rotate(0f, Random.Range(0f, 360f), 0f);
			base.StartCoroutine(this.AppearSequence());
		}
	}

	// Token: 0x0400036B RID: 875
	public float speed = 1f;

	// Token: 0x0400036C RID: 876
	public GameObject model;

	// Token: 0x0400036D RID: 877
	public float offset;

	// Token: 0x0400036E RID: 878
	[Range(0f, 10f)]
	public float animationDelay;

	// Token: 0x0400036F RID: 879
	public float spawnDelay = 5f;

	// Token: 0x04000370 RID: 880
	public int radius = 50;

	// Token: 0x04000371 RID: 881
	public AnimationClip swim;

	// Token: 0x04000372 RID: 882
	public List<AnimationClip> animations = new List<AnimationClip>();

	// Token: 0x04000373 RID: 883
	public float distanceToTravel = 10f;

	// Token: 0x04000374 RID: 884
	private bool isWaiting;

	// Token: 0x04000375 RID: 885
	private float elapsedTime;

	// Token: 0x04000376 RID: 886
	private float waterLevel;

	// Token: 0x04000377 RID: 887
	private bool active = true;

	// Token: 0x04000378 RID: 888
	private bool isPlayingAnimation;

	// Token: 0x04000379 RID: 889
	private bool isMoving;

	// Token: 0x0400037A RID: 890
	private Vector3 size = Vector3.one;

	// Token: 0x0400037B RID: 891
	private Animation animator;

	// Token: 0x0400037C RID: 892
	private WorldUI ui;
}
