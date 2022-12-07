using System;
using UnityEngine;

// Token: 0x02000174 RID: 372
[ExecuteInEditMode]
public class RoadWaypoint : MonoBehaviour
{
	// Token: 0x0600130B RID: 4875 RVA: 0x000C6E02 File Offset: 0x000C5002
	private void OnEnable()
	{
		SettlementBV.waypoints.Add(this);
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x000C6E0F File Offset: 0x000C500F
	private void OnDisable()
	{
		SettlementBV.waypoints.Remove(this);
	}
}
