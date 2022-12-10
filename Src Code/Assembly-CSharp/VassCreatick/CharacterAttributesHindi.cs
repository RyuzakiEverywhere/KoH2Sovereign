using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VassCreatick
{
	// Token: 0x02000354 RID: 852
	[Serializable]
	public class CharacterAttributesHindi
	{
		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06003315 RID: 13077 RVA: 0x0019D73E File Offset: 0x0019B93E
		// (set) Token: 0x06003316 RID: 13078 RVA: 0x0019D746 File Offset: 0x0019B946
		[JsonProperty]
		public string Character { get; private set; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06003317 RID: 13079 RVA: 0x0019D74F File Offset: 0x0019B94F
		// (set) Token: 0x06003318 RID: 13080 RVA: 0x0019D757 File Offset: 0x0019B957
		[JsonProperty]
		public bool Append { get; private set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06003319 RID: 13081 RVA: 0x0019D760 File Offset: 0x0019B960
		// (set) Token: 0x0600331A RID: 13082 RVA: 0x0019D768 File Offset: 0x0019B968
		[JsonProperty]
		public bool AppendBefore { get; private set; }

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x0600331B RID: 13083 RVA: 0x0019D771 File Offset: 0x0019B971
		// (set) Token: 0x0600331C RID: 13084 RVA: 0x0019D779 File Offset: 0x0019B979
		[JsonProperty]
		public bool AppendFirst { get; private set; }

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x0600331D RID: 13085 RVA: 0x0019D782 File Offset: 0x0019B982
		// (set) Token: 0x0600331E RID: 13086 RVA: 0x0019D78A File Offset: 0x0019B98A
		[JsonProperty]
		public bool AppendStart { get; private set; }

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x0600331F RID: 13087 RVA: 0x0019D793 File Offset: 0x0019B993
		// (set) Token: 0x06003320 RID: 13088 RVA: 0x0019D79B File Offset: 0x0019B99B
		[JsonProperty]
		public List<string> CharacterCombinations { get; private set; }

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06003321 RID: 13089 RVA: 0x0019D7A4 File Offset: 0x0019B9A4
		// (set) Token: 0x06003322 RID: 13090 RVA: 0x0019D7AC File Offset: 0x0019B9AC
		[JsonProperty]
		public string CharacterHexValue { get; private set; }

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06003323 RID: 13091 RVA: 0x0019D7B5 File Offset: 0x0019B9B5
		// (set) Token: 0x06003324 RID: 13092 RVA: 0x0019D7BD File Offset: 0x0019B9BD
		[JsonProperty]
		public string CharacterHexValue2 { get; private set; }

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06003325 RID: 13093 RVA: 0x0019D7C6 File Offset: 0x0019B9C6
		// (set) Token: 0x06003326 RID: 13094 RVA: 0x0019D7CE File Offset: 0x0019B9CE
		[JsonProperty]
		public string CharRetain { get; private set; }

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06003327 RID: 13095 RVA: 0x0019D7D7 File Offset: 0x0019B9D7
		// (set) Token: 0x06003328 RID: 13096 RVA: 0x0019D7DF File Offset: 0x0019B9DF
		[JsonProperty]
		public string CharRetain2 { get; private set; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06003329 RID: 13097 RVA: 0x0019D7E8 File Offset: 0x0019B9E8
		// (set) Token: 0x0600332A RID: 13098 RVA: 0x0019D7F0 File Offset: 0x0019B9F0
		[JsonProperty]
		public string CharName { get; private set; }
	}
}
