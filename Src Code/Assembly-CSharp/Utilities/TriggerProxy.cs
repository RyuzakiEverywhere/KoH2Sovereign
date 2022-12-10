using System;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
	// Token: 0x0200047B RID: 1147
	public class TriggerProxy : MonoBehaviour
	{
		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06003BDA RID: 15322 RVA: 0x001C8E9A File Offset: 0x001C709A
		// (set) Token: 0x06003BDB RID: 15323 RVA: 0x001C8EA2 File Offset: 0x001C70A2
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}

		// Token: 0x06003BDC RID: 15324 RVA: 0x001C8EAB File Offset: 0x001C70AB
		private void OnTriggerStay(Collider other)
		{
			if (!this.active)
			{
				return;
			}
			if (MalbersTools.Layer_in_LayerMask(other.gameObject.layer, this.Ignore))
			{
				return;
			}
			this.OnTrigger_Stay.Invoke(other);
		}

		// Token: 0x06003BDD RID: 15325 RVA: 0x001C8EDB File Offset: 0x001C70DB
		private void OnTriggerEnter(Collider other)
		{
			if (!this.active)
			{
				return;
			}
			if (MalbersTools.Layer_in_LayerMask(other.gameObject.layer, this.Ignore))
			{
				return;
			}
			this.OnTrigger_Enter.Invoke(other);
		}

		// Token: 0x06003BDE RID: 15326 RVA: 0x001C8F0B File Offset: 0x001C710B
		private void OnTriggerExit(Collider other)
		{
			if (!this.active)
			{
				return;
			}
			if (MalbersTools.Layer_in_LayerMask(other.gameObject.layer, this.Ignore))
			{
				return;
			}
			this.OnTrigger_Exit.Invoke(other);
		}

		// Token: 0x06003BDF RID: 15327 RVA: 0x001C8F3C File Offset: 0x001C713C
		private void Reset()
		{
			Collider component = base.GetComponent<Collider>();
			this.Active = true;
			if (component)
			{
				component.isTrigger = true;
				return;
			}
			Debug.LogError("This Script requires a Collider, please add any type of collider");
		}

		// Token: 0x04002B6C RID: 11116
		[Tooltip("Ignore this Objects with this layers")]
		public LayerMask Ignore;

		// Token: 0x04002B6D RID: 11117
		[SerializeField]
		private bool active = true;

		// Token: 0x04002B6E RID: 11118
		public ColliderEvent OnTrigger_Enter = new ColliderEvent();

		// Token: 0x04002B6F RID: 11119
		public ColliderEvent OnTrigger_Stay = new ColliderEvent();

		// Token: 0x04002B70 RID: 11120
		public ColliderEvent OnTrigger_Exit = new ColliderEvent();

		// Token: 0x04002B71 RID: 11121
		public CollisionEvent OnCollision_Enter = new CollisionEvent();
	}
}
