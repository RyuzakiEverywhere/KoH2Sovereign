using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class Smear : MonoBehaviour
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000009 RID: 9 RVA: 0x000022D3 File Offset: 0x000004D3
	// (set) Token: 0x0600000A RID: 10 RVA: 0x000022DB File Offset: 0x000004DB
	private Material InstancedMaterial
	{
		get
		{
			return this.m_instancedMaterial;
		}
		set
		{
			this.m_instancedMaterial = value;
		}
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000022E4 File Offset: 0x000004E4
	private void Start()
	{
		this.InstancedMaterial = this.Renderer.material;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000022F8 File Offset: 0x000004F8
	private void LateUpdate()
	{
		if (this.m_recentPositions.Count > this.FramesBufferSize)
		{
			this.InstancedMaterial.SetVector("_PrevPosition", this.m_recentPositions.Dequeue());
		}
		this.InstancedMaterial.SetVector("_Position", base.transform.position);
		this.m_recentPositions.Enqueue(base.transform.position);
	}

	// Token: 0x04000008 RID: 8
	private Queue<Vector3> m_recentPositions = new Queue<Vector3>();

	// Token: 0x04000009 RID: 9
	public int FramesBufferSize;

	// Token: 0x0400000A RID: 10
	public Renderer Renderer;

	// Token: 0x0400000B RID: 11
	private Material m_instancedMaterial;
}
