using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003C5 RID: 965
	public class FollowTarget : MonoBehaviour
	{
		// Token: 0x0600372D RID: 14125 RVA: 0x001B4986 File Offset: 0x001B2B86
		private void Start()
		{
			this.animal = base.GetComponentInParent<Animal>();
		}

		// Token: 0x0600372E RID: 14126 RVA: 0x001B4994 File Offset: 0x001B2B94
		private void Update()
		{
			Vector3 vector = this.target.position - base.transform.position;
			float num = Vector3.Distance(base.transform.position, this.target.position);
			this.animal.Move((num > this.stopDistance) ? vector : Vector3.zero, true);
		}

		// Token: 0x0600372F RID: 14127 RVA: 0x001B49F6 File Offset: 0x001B2BF6
		private void OnDisable()
		{
			this.animal.Move(Vector3.zero, true);
		}

		// Token: 0x04002699 RID: 9881
		public Transform target;

		// Token: 0x0400269A RID: 9882
		public float stopDistance = 3f;

		// Token: 0x0400269B RID: 9883
		private Animal animal;
	}
}
