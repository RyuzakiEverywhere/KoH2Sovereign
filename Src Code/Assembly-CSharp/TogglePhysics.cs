using System;
using UnityEngine;

// Token: 0x02000320 RID: 800
public class TogglePhysics : MonoBehaviour
{
	// Token: 0x06003200 RID: 12800 RVA: 0x0019570F File Offset: 0x0019390F
	private void Start()
	{
		Physics.autoSimulation = true;
	}
}
