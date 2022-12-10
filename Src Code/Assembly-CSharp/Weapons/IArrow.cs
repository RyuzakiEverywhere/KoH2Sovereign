using System;
using UnityEngine;

namespace MalbersAnimations.Weapons
{
	// Token: 0x02000419 RID: 1049
	public interface IArrow
	{
		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060038B2 RID: 14514
		// (set) Token: 0x060038B3 RID: 14515
		LayerMask HitMask { get; set; }

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060038B4 RID: 14516
		// (set) Token: 0x060038B5 RID: 14517
		float TailOffset { get; set; }

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060038B6 RID: 14518
		// (set) Token: 0x060038B7 RID: 14519
		float Damage { get; set; }

		// Token: 0x060038B8 RID: 14520
		void ShootArrow(float force, Vector3 Direction);
	}
}
