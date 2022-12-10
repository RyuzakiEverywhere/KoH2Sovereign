using System;
using UnityEngine;

namespace MalbersAnimations.HAP
{
	// Token: 0x0200042B RID: 1067
	public interface IMountAI
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x06003959 RID: 14681
		bool CanBeCalled { get; }

		// Token: 0x0600395A RID: 14682
		void CallAnimal(Transform target, bool call = true);
	}
}
