using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class StreamEmitter : MonoBehaviour
{
	// Token: 0x06000232 RID: 562 RVA: 0x00020822 File Offset: 0x0001EA22
	private void OnDrawGizmos()
	{
		Gizmos.color = ((this.emitterType == StreamEmitter.EmitterType.Water) ? Color.blue : Color.red);
		Gizmos.DrawWireSphere(base.transform.position, base.transform.lossyScale.x);
	}

	// Token: 0x06000233 RID: 563 RVA: 0x00020860 File Offset: 0x0001EA60
	private void OnEnable()
	{
		RaycastHit[] array = Physics.RaycastAll(new Ray(base.transform.position + Vector3.up * 10f, Vector3.down));
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

	// Token: 0x06000234 RID: 564 RVA: 0x000208D4 File Offset: 0x0001EAD4
	private void OnDisable()
	{
		if (this.streamMgr != null)
		{
			this.streamMgr.Unregister(this);
			this.streamMgr = null;
		}
	}

	// Token: 0x04000342 RID: 834
	public StreamEmitter.EmitterType emitterType;

	// Token: 0x04000343 RID: 835
	[Range(0f, 1f)]
	public float strength = 1f;

	// Token: 0x04000344 RID: 836
	private StreamManager streamMgr;

	// Token: 0x02000512 RID: 1298
	public enum EmitterType
	{
		// Token: 0x04002EC6 RID: 11974
		Water,
		// Token: 0x04002EC7 RID: 11975
		Lava
	}
}
