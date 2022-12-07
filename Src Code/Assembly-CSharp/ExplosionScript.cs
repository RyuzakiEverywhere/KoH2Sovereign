using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class ExplosionScript : MonoBehaviour
{
	// Token: 0x0600018A RID: 394 RVA: 0x0000F919 File Offset: 0x0000DB19
	private void Start()
	{
		this.Explode();
	}

	// Token: 0x0600018B RID: 395 RVA: 0x0000F924 File Offset: 0x0000DB24
	public void SetupRubble(ExplosionScript.Rubble rubble, bool active)
	{
		if (((rubble != null) ? rubble.rubbleObj : null) == null)
		{
			return;
		}
		for (int i = 0; i < this.particles.Count; i++)
		{
			this.particles[i].counter = 0;
		}
		rubble.active = active;
		if (active)
		{
			rubble.startSinkTime = Time.time + this.destroyDelay;
			rubble.endSinkTime = Time.time + this.destroyDelay + this.sinkDuration;
			rubble.sinkSpeed = this.sinkSpeed;
		}
		float magnitude = base.transform.lossyScale.magnitude;
		rubble.bodies = rubble.rubbleObj.GetComponentsInChildren<Rigidbody>();
		for (int j = 0; j < rubble.bodies.Length; j++)
		{
			Rigidbody rigidbody = rubble.bodies[j];
			if (rigidbody != null)
			{
				rigidbody.isKinematic = !active;
				if (active)
				{
					rigidbody.mass *= magnitude;
					rigidbody.drag /= magnitude;
					rigidbody.AddExplosionForce(Random.Range(this.minForce, this.maxForce), base.transform.position, this.radius);
					for (int k = 0; k < this.particles.Count; k++)
					{
						ExplosionScript.Particles particles = this.particles[k];
						GameObject smoke = particles.smoke;
						if (smoke != null && particles.counter < particles.maximumSmokes && Random.Range(1, 4) == 1)
						{
							Object obj = Object.Instantiate<GameObject>(smoke, rigidbody.transform);
							particles.counter++;
							Object.Destroy(obj, this.destroyDelay);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600018C RID: 396 RVA: 0x0000FADC File Offset: 0x0000DCDC
	public void Explode()
	{
		if (this.explosion != null)
		{
			Object.Destroy(Object.Instantiate<GameObject>(this.explosion, base.transform.position + this.explosionOffset, Quaternion.identity), this.destroyDelay);
		}
		this.SetupRubble(this.center, true);
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000FB38 File Offset: 0x0000DD38
	private void LateUpdate()
	{
		if (this.center.active)
		{
			this.center.Sink();
		}
		if (this.left.active)
		{
			this.left.Sink();
		}
		if (this.right.active)
		{
			this.right.Sink();
		}
	}

	// Token: 0x040002A3 RID: 675
	public GameObject explosion;

	// Token: 0x040002A4 RID: 676
	public Vector3 explosionOffset;

	// Token: 0x040002A5 RID: 677
	public float destroyDelay = 20f;

	// Token: 0x040002A6 RID: 678
	public float sinkDuration = 8f;

	// Token: 0x040002A7 RID: 679
	public float sinkSpeed = 4f;

	// Token: 0x040002A8 RID: 680
	public float minForce;

	// Token: 0x040002A9 RID: 681
	public float maxForce;

	// Token: 0x040002AA RID: 682
	public float radius;

	// Token: 0x040002AB RID: 683
	public ExplosionScript.Rubble center;

	// Token: 0x040002AC RID: 684
	public ExplosionScript.Rubble left;

	// Token: 0x040002AD RID: 685
	public ExplosionScript.Rubble right;

	// Token: 0x040002AE RID: 686
	public List<ExplosionScript.Particles> particles = new List<ExplosionScript.Particles>();

	// Token: 0x020004FF RID: 1279
	[Serializable]
	public class Particles
	{
		// Token: 0x04002E75 RID: 11893
		public GameObject smoke;

		// Token: 0x04002E76 RID: 11894
		public int maximumSmokes;

		// Token: 0x04002E77 RID: 11895
		[NonSerialized]
		public int counter;
	}

	// Token: 0x02000500 RID: 1280
	[Serializable]
	public class Rubble
	{
		// Token: 0x06004268 RID: 17000 RVA: 0x001F8BE0 File Offset: 0x001F6DE0
		public void Sink()
		{
			if (this.bodies == null)
			{
				return;
			}
			if (Time.time > this.startSinkTime)
			{
				for (int i = 0; i < this.bodies.Length; i++)
				{
					Rigidbody rigidbody = this.bodies[i];
					if (!this.disabled)
					{
						rigidbody.isKinematic = true;
						Object.Destroy(rigidbody.GetComponent<Collider>());
					}
					Vector3 position = rigidbody.position;
					position.y -= Time.deltaTime * this.sinkSpeed;
					rigidbody.position = position;
				}
				this.disabled = true;
			}
			if (Time.time > this.endSinkTime)
			{
				for (int j = 0; j < this.bodies.Length; j++)
				{
					Object.Destroy(this.bodies[j].gameObject);
				}
				this.bodies = null;
			}
		}

		// Token: 0x04002E78 RID: 11896
		public GameObject rubbleObj;

		// Token: 0x04002E79 RID: 11897
		[NonSerialized]
		public float startSinkTime;

		// Token: 0x04002E7A RID: 11898
		[NonSerialized]
		public float endSinkTime;

		// Token: 0x04002E7B RID: 11899
		[NonSerialized]
		public bool active;

		// Token: 0x04002E7C RID: 11900
		[NonSerialized]
		public bool disabled;

		// Token: 0x04002E7D RID: 11901
		[NonSerialized]
		public float sinkSpeed;

		// Token: 0x04002E7E RID: 11902
		[NonSerialized]
		public Rigidbody[] bodies;
	}
}
