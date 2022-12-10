using System;
using MalbersAnimations.Events;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace MalbersAnimations.Utilities
{
	// Token: 0x02000478 RID: 1144
	public class PointClick : MonoBehaviour
	{
		// Token: 0x06003BCF RID: 15311 RVA: 0x001C8CA0 File Offset: 0x001C6EA0
		public void OnGroundClick(BaseEventData data)
		{
			PointerEventData pointerEventData = (PointerEventData)data;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(pointerEventData.pointerCurrentRaycast.worldPosition, out navMeshHit, 4f, -1))
			{
				this.destinationPosition = navMeshHit.position;
			}
			else
			{
				this.destinationPosition = pointerEventData.pointerCurrentRaycast.worldPosition;
			}
			this.OnPointClick.Invoke(this.destinationPosition);
		}

		// Token: 0x06003BD0 RID: 15312 RVA: 0x001C8CFF File Offset: 0x001C6EFF
		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(this.destinationPosition, 0.1f);
				Gizmos.DrawSphere(this.destinationPosition, 0.1f);
			}
		}

		// Token: 0x04002B63 RID: 11107
		private const float navMeshSampleDistance = 4f;

		// Token: 0x04002B64 RID: 11108
		public Vector3Event OnPointClick = new Vector3Event();

		// Token: 0x04002B65 RID: 11109
		public GameObjectEvent OnInteractableClick = new GameObjectEvent();

		// Token: 0x04002B66 RID: 11110
		private Vector3 destinationPosition;
	}
}
