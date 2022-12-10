using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003F2 RID: 1010
	public interface IAnimatorBehaviour
	{
		// Token: 0x060037FB RID: 14331
		void OnStateEnter(int ID, AnimatorStateInfo stateInfo, int layerIndex);

		// Token: 0x060037FC RID: 14332
		void OnStateExit(int ID, AnimatorStateInfo stateInfo, int layerIndex);

		// Token: 0x060037FD RID: 14333
		void OnStateMove(int ID, AnimatorStateInfo stateInfo, int layerIndex);

		// Token: 0x060037FE RID: 14334
		void OnStateUpdate(int ID, AnimatorStateInfo stateInfo, int layerIndex);
	}
}
