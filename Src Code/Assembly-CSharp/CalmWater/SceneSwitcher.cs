using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CalmWater
{
	// Token: 0x020004E2 RID: 1250
	public class SceneSwitcher : MonoBehaviour
	{
		// Token: 0x06004211 RID: 16913 RVA: 0x001F7D7A File Offset: 0x001F5F7A
		public void SwitchLevel(string level)
		{
			SceneManager.LoadScene(level);
		}
	}
}
