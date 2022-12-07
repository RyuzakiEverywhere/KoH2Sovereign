using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class StreamCollider : MonoBehaviour
{
	// Token: 0x0600022E RID: 558 RVA: 0x0002074F File Offset: 0x0001E94F
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, base.transform.lossyScale.x);
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0002077C File Offset: 0x0001E97C
	private void OnEnable()
	{
		RaycastHit[] array = Physics.RaycastAll(new Ray(base.transform.position + Vector3.up * 50f, Vector3.down));
		for (int i = 0; i < array.Length; i++)
		{
			StreamManager component = array[i].collider.GetComponent<StreamManager>();
			if (component != null)
			{
				this.streamMgr = component;
				component.Register(this);
				return;
			}
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x000207F0 File Offset: 0x0001E9F0
	private void OnDisable()
	{
		if (this.streamMgr != null)
		{
			this.streamMgr.Unregister(this);
			this.streamMgr = null;
		}
	}

	// Token: 0x04000340 RID: 832
	private StreamManager streamMgr;

	// Token: 0x04000341 RID: 833
	public StreamCollider.ColliderType colliderType = StreamCollider.ColliderType.Both;

	// Token: 0x02000511 RID: 1297
	public enum ColliderType
	{
		// Token: 0x04002EC2 RID: 11970
		Water,
		// Token: 0x04002EC3 RID: 11971
		Lava,
		// Token: 0x04002EC4 RID: 11972
		Both
	}
}
