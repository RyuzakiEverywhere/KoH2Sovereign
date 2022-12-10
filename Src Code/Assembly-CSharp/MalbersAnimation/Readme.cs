using System;
using UnityEngine;

namespace MalbersAnimations
{
	// Token: 0x020003F5 RID: 1013
	public class Readme : ScriptableObject
	{
		// Token: 0x0400282F RID: 10287
		public Texture2D icon;

		// Token: 0x04002830 RID: 10288
		public string title;

		// Token: 0x04002831 RID: 10289
		public Readme.Section[] sections;

		// Token: 0x02000923 RID: 2339
		[Serializable]
		public class Section
		{
			// Token: 0x04004284 RID: 17028
			public string heading;

			// Token: 0x04004285 RID: 17029
			public string text;

			// Token: 0x04004286 RID: 17030
			public string linkText;

			// Token: 0x04004287 RID: 17031
			public string url;
		}
	}
}
