using System;
using UnityEngine;

namespace TFHC_ForceShield_Shader_Sample
{
	// Token: 0x020004E7 RID: 1255
	public class ForceShieldShootBall : MonoBehaviour
	{
		// Token: 0x06004221 RID: 16929 RVA: 0x001F7FB8 File Offset: 0x001F61B8
		private void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.distance);
				vector = Camera.main.ScreenToWorldPoint(vector);
				Rigidbody rigidbody = Object.Instantiate<Rigidbody>(this.bullet, base.transform.position, Quaternion.identity);
				rigidbody.transform.LookAt(vector);
				rigidbody.AddForce(rigidbody.transform.forward * this.speed);
			}
		}

		// Token: 0x04002E0A RID: 11786
		public Rigidbody bullet;

		// Token: 0x04002E0B RID: 11787
		public Transform origshoot;

		// Token: 0x04002E0C RID: 11788
		public float speed = 1000f;

		// Token: 0x04002E0D RID: 11789
		private float distance = 10f;
	}
}
