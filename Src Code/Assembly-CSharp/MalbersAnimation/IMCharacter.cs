using System;
using System.Collections.Generic;
using MalbersAnimations.Events;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003EE RID: 1006
	public interface IMCharacter
	{
		// Token: 0x060037E9 RID: 14313
		void Move(Vector3 move, bool direction = true);

		// Token: 0x060037EA RID: 14314
		void SetInput(string key, bool inputvalue);

		// Token: 0x060037EB RID: 14315
		void AddInput(string key, BoolEvent NewBool);

		// Token: 0x060037EC RID: 14316
		void InitializeInputs(Dictionary<string, BoolEvent> keys);
	}
}
