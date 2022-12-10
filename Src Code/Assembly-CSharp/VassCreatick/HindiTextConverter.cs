using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace VassCreatick
{
	// Token: 0x02000353 RID: 851
	public static class HindiTextConverter
	{
		// Token: 0x06003312 RID: 13074 RVA: 0x0019D167 File Offset: 0x0019B367
		static HindiTextConverter()
		{
			HindiTextConverter.LoadAttributesHindi();
		}

		// Token: 0x06003313 RID: 13075 RVA: 0x0019D17C File Offset: 0x0019B37C
		private static bool LoadAttributesHindi()
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Hindi/HindiReplacement");
			if (!(textAsset != null))
			{
				Debug.LogError("Failed to load HindiReplacement");
				return false;
			}
			HindiTextConverter.mCharacterAttributesHindi = JsonConvert.DeserializeObject<List<CharacterAttributesHindi>>(textAsset.text);
			if (HindiTextConverter.mCharacterAttributesHindi != null)
			{
				return true;
			}
			Debug.LogError("Failed to deserialize languague data");
			return false;
		}

		// Token: 0x06003314 RID: 13076 RVA: 0x0019D1D0 File Offset: 0x0019B3D0
		public static string Convert(string text)
		{
			if (HindiTextConverter.mCharacterAttributesHindi == null)
			{
				return text;
			}
			string text2 = text;
			for (int i = 0; i < HindiTextConverter.mCharacterAttributesHindi.Count; i++)
			{
				if (HindiTextConverter.mCharacterAttributesHindi[i].CharacterHexValue == "")
				{
					for (int j = 0; j < HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations.Count; j++)
					{
						if (text2.Contains(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j]))
						{
							if (HindiTextConverter.mCharacterAttributesHindi[i].Append)
							{
								string text3 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharacterHexValue2, 16)).ToString();
								text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], HindiTextConverter.mCharacterAttributesHindi[i].CharName + text3);
							}
							else if (HindiTextConverter.mCharacterAttributesHindi[i].AppendBefore)
							{
								string text3 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharacterHexValue2, 16)).ToString();
								text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], text3 + HindiTextConverter.mCharacterAttributesHindi[i].CharName);
							}
							else if (HindiTextConverter.mCharacterAttributesHindi[i].AppendFirst)
							{
								string text3 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharacterHexValue2, 16)).ToString();
								if (HindiTextConverter.mCharacterAttributesHindi[i].CharName == "006B")
								{
									string text4 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharName, 16)).ToString();
									text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], text3 + text4);
								}
								else
								{
									string text4 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharName, 16)).ToString();
									if (HindiTextConverter.mCharacterAttributesHindi[i].Character == "ndh" || HindiTextConverter.mCharacterAttributesHindi[i].Character == "ravo" || HindiTextConverter.mCharacterAttributesHindi[i].Character == "ravi" || HindiTextConverter.mCharacterAttributesHindi[i].Character == "rathi" || HindiTextConverter.mCharacterAttributesHindi[i].Character == "shki" || HindiTextConverter.mCharacterAttributesHindi[i].Character == "kriy" || HindiTextConverter.mCharacterAttributesHindi[i].Character == "iksii" || HindiTextConverter.mCharacterAttributesHindi[i].Character == "dhrii")
									{
										string text5 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharRetain2, 16)).ToString();
										if (HindiTextConverter.mCharacterAttributesHindi[i].Character == "iksii")
										{
											text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], text4 + text3 + HindiTextConverter.mCharacterAttributesHindi[i].CharRetain + text5);
										}
										else
										{
											text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], text4 + text3 + text5 + HindiTextConverter.mCharacterAttributesHindi[i].CharRetain);
										}
									}
									else
									{
										text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], text4 + text3 + HindiTextConverter.mCharacterAttributesHindi[i].CharRetain);
									}
								}
							}
							else if (HindiTextConverter.mCharacterAttributesHindi[i].AppendStart)
							{
								string text3 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharacterHexValue2, 16)).ToString();
								string text4 = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharName, 16)).ToString();
								text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], text4 + HindiTextConverter.mCharacterAttributesHindi[i].CharRetain + text3);
							}
							else
							{
								text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[j], HindiTextConverter.mCharacterAttributesHindi[i].CharName);
							}
						}
					}
				}
				else
				{
					string newValue = System.Convert.ToChar(System.Convert.ToInt32(HindiTextConverter.mCharacterAttributesHindi[i].CharacterHexValue, 16)).ToString();
					for (int k = 0; k < HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations.Count; k++)
					{
						text2 = text2.Replace(HindiTextConverter.mCharacterAttributesHindi[i].CharacterCombinations[k], newValue);
					}
				}
			}
			return text2;
		}

		// Token: 0x0400228F RID: 8847
		private static List<CharacterAttributesHindi> mCharacterAttributesHindi = new List<CharacterAttributesHindi>();
	}
}
