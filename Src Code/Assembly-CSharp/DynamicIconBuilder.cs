using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Logic;
using UnityEngine;

// Token: 0x020001E2 RID: 482
public class DynamicIconBuilder : MonoBehaviour
{
	// Token: 0x06001CC7 RID: 7367 RVA: 0x0000210D File Offset: 0x0000030D
	private DynamicIconBuilder()
	{
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06001CC8 RID: 7368 RVA: 0x00110A84 File Offset: 0x0010EC84
	public static DynamicIconBuilder Instance
	{
		get
		{
			if (DynamicIconBuilder._instance == null)
			{
				DynamicIconBuilder._instance = (DynamicIconBuilder)UnityEngine.Object.FindObjectOfType(typeof(DynamicIconBuilder));
				if (DynamicIconBuilder._instance == null)
				{
					DynamicIconBuilder._instance = new GameObject().AddComponent<DynamicIconBuilder>();
					DynamicIconBuilder._instance.name = "DynamicIconBuilder";
					if (Application.isPlaying)
					{
						UnityEngine.Object.DontDestroyOnLoad(DynamicIconBuilder._instance);
					}
				}
			}
			return DynamicIconBuilder._instance;
		}
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x00110AF8 File Offset: 0x0010ECF8
	private void OnEnable()
	{
		DynamicIconBuilder._instance = this;
	}

	// Token: 0x06001CCA RID: 7370 RVA: 0x00110B00 File Offset: 0x0010ED00
	private void OnDestroy()
	{
		this.ClearFaceMap();
		if (DynamicIconBuilder.archives == null)
		{
			return;
		}
		for (int i = 0; i < DynamicIconBuilder.archives.Length; i++)
		{
			ZipArchive zipArchive = DynamicIconBuilder.archives[i];
			if (zipArchive != null)
			{
				zipArchive.Dispose();
			}
		}
		DynamicIconBuilder.archives = null;
	}

	// Token: 0x06001CCB RID: 7371 RVA: 0x00110B44 File Offset: 0x0010ED44
	public DynamicIconBuilder.CharacterData GetCD(Logic.Character character)
	{
		return this.GetCD(character.portraitID);
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x00110B54 File Offset: 0x0010ED54
	public DynamicIconBuilder.CharacterData GetCD(int id)
	{
		DynamicIconBuilder.CharacterData result = null;
		if (id == -1)
		{
			Game game = GameLogic.Get(true);
			if (game != null && !game.IsAuthority())
			{
				return null;
			}
		}
		if (!DynamicIconBuilder.sm_FaceMap.TryGetValue(id, out result))
		{
			Debug.LogWarning("Missing portrait at id " + id);
			return null;
		}
		return result;
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x00110BA4 File Offset: 0x0010EDA4
	public DynamicIconBuilder.CharacterData.VariantData GetVariant(DynamicIconBuilder.CharacterData characterData, Logic.Character.Age age)
	{
		int age2 = PresetRecipes.AgeToInt(PresetRecipes.AgeToAges(age));
		return this.GetVariant(characterData.id, age2, 0);
	}

	// Token: 0x06001CCE RID: 7374 RVA: 0x00110BCC File Offset: 0x0010EDCC
	public DynamicIconBuilder.CharacterData.VariantData GetVariant(Logic.Character character)
	{
		int age = PresetRecipes.CharacterToAges(character);
		return this.GetVariant(character.portraitID, age, character.portrait_variantID);
	}

	// Token: 0x06001CCF RID: 7375 RVA: 0x00110BF4 File Offset: 0x0010EDF4
	public DynamicIconBuilder.CharacterData.VariantData GetVariant(int id, int age, int variant)
	{
		DynamicIconBuilder.CharacterData cd = this.GetCD(id);
		if (cd == null)
		{
			return null;
		}
		List<DynamicIconBuilder.CharacterData.VariantData> list = cd.variants[age];
		if (list == null)
		{
			return null;
		}
		if (list.Count <= variant)
		{
			return null;
		}
		return list[variant];
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x00110C30 File Offset: 0x0010EE30
	public DynamicIconBuilder.CharacterData.VariantData GetLastVariant(Logic.Character character)
	{
		DynamicIconBuilder.CharacterData characterData;
		if (!DynamicIconBuilder.sm_FaceMap.TryGetValue(character.last_portraitID, out characterData))
		{
			return null;
		}
		if (characterData.variants.Length <= character.last_portrait_age)
		{
			return null;
		}
		List<DynamicIconBuilder.CharacterData.VariantData> list = characterData.variants[character.last_portrait_age];
		if (list.Count <= character.last_variantID)
		{
			return null;
		}
		return list[character.last_variantID];
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x00110C90 File Offset: 0x0010EE90
	public static ZipArchive GetArchive(int id)
	{
		if (DynamicIconBuilder.archives == null)
		{
			return null;
		}
		if (DynamicIconBuilder.archives.Length <= id)
		{
			ZipArchive[] array = new ZipArchive[id + 1];
			for (int i = 0; i < DynamicIconBuilder.archives.Length; i++)
			{
				array[i] = DynamicIconBuilder.archives[i];
			}
			for (int j = DynamicIconBuilder.archives.Length; j <= id; j++)
			{
				string archiveFileName = "Portraits/" + string.Format("{0}-{1}.zip", j * 25, (j + 1) * 25);
				array[j] = ZipFile.Open(archiveFileName, ZipArchiveMode.Update);
			}
			DynamicIconBuilder.archives = array;
		}
		return DynamicIconBuilder.archives[id];
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x00110D27 File Offset: 0x0010EF27
	public void AddCharacterData(DynamicIconBuilder.CharacterData data, int id)
	{
		DynamicIconBuilder.sm_FaceMap[id] = data;
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x00110D35 File Offset: 0x0010EF35
	public static void InitValidIDs()
	{
		DynamicIconBuilder.valid_p_ids = new List<int>(DynamicIconBuilder.sm_FaceMap.Count);
		DynamicIconBuilder.valid_v_ids = new List<int>();
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x00110D55 File Offset: 0x0010EF55
	public void ClearFaceMap()
	{
		Dictionary<int, DynamicIconBuilder.CharacterData> dictionary = DynamicIconBuilder.sm_FaceMap;
		if (dictionary == null)
		{
			return;
		}
		dictionary.Clear();
	}

	// Token: 0x06001CD5 RID: 7381 RVA: 0x00110D68 File Offset: 0x0010EF68
	public static int FindDefaultVariants(Logic.Character character, bool check_ethnicity = true)
	{
		Profile.BeginSection("Choose default portrait");
		int result = -1;
		DynamicIconBuilder.valid_p_ids.Clear();
		Profile.BeginSection("Validate default portraits");
		foreach (KeyValuePair<int, DynamicIconBuilder.CharacterData> keyValuePair in DynamicIconBuilder.sm_FaceMap)
		{
			int num = PresetRecipes.CharacterToAges(character);
			int id = keyValuePair.Value.id;
			List<DynamicIconBuilder.CharacterData.VariantData> list = keyValuePair.Value.variants[num];
			if (list.Count != 0 && list[0].Validate(character, check_ethnicity))
			{
				DynamicIconBuilder.valid_p_ids.Add(id);
			}
		}
		Profile.EndSection("Validate default portraits");
		if (DynamicIconBuilder.valid_p_ids.Count > 0)
		{
			Logic.Kingdom kingdom = character.GetKingdom();
			if (kingdom != null && !character.historical_figure)
			{
				Profile.BeginSection("Avoid duplicate portraits");
				List<int> list2 = new List<int>(DynamicIconBuilder.valid_p_ids);
				if (kingdom.court != null)
				{
					for (int i = 0; i < kingdom.court.Count; i++)
					{
						Logic.Character character2 = kingdom.court[i];
						if (character2 != null)
						{
							list2.Remove(character2.portraitID);
						}
					}
				}
				if (kingdom.prisoners != null)
				{
					for (int j = 0; j < kingdom.prisoners.Count; j++)
					{
						Logic.Character character3 = kingdom.prisoners[j];
						if (character3 != null)
						{
							list2.Remove(character3.portraitID);
						}
					}
				}
				if (kingdom.royalFamily != null)
				{
					if (kingdom.royalFamily.Sovereign != null)
					{
						list2.Remove(kingdom.royalFamily.Sovereign.portraitID);
					}
					if (kingdom.royalFamily.Spouse != null)
					{
						list2.Remove(kingdom.royalFamily.Spouse.portraitID);
					}
					for (int k = 0; k < kingdom.royalFamily.Children.Count; k++)
					{
						Logic.Character character4 = kingdom.royalFamily.Children[k];
						if (character4 != null)
						{
							list2.Remove(character4.portraitID);
						}
					}
				}
				if (list2.Count > 0)
				{
					result = list2[Random.Range(0, list2.Count)];
				}
				else
				{
					result = DynamicIconBuilder.valid_p_ids[Random.Range(0, DynamicIconBuilder.valid_p_ids.Count)];
				}
				Profile.EndSection("Avoid duplicate portraits");
			}
			else if (kingdom != null && character.historical_figure)
			{
				Random random = new Random(CharacterFactory.GetKingdomSeed(kingdom));
				result = DynamicIconBuilder.valid_p_ids[random.Next(0, DynamicIconBuilder.valid_p_ids.Count)];
			}
			else
			{
				result = DynamicIconBuilder.valid_p_ids[Random.Range(0, DynamicIconBuilder.valid_p_ids.Count)];
			}
		}
		Profile.EndSection("Choose default portrait");
		return result;
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x00111040 File Offset: 0x0010F240
	public static int FindVariant(Logic.Character character)
	{
		Profile.BeginSection("Choose variant portrait");
		int portraitID = character.portraitID;
		if (portraitID == -1)
		{
			return -1;
		}
		DynamicIconBuilder.valid_v_ids.Clear();
		DynamicIconBuilder.CharacterData characterData = DynamicIconBuilder.sm_FaceMap[portraitID];
		int num = PresetRecipes.CharacterToAges(character);
		List<DynamicIconBuilder.CharacterData.VariantData> list = characterData.variants[num];
		if (list.Count == 0)
		{
			return -1;
		}
		if (list.Count > DynamicIconBuilder.valid_v_ids.Capacity)
		{
			DynamicIconBuilder.valid_v_ids.Capacity = list.Count;
		}
		Profile.BeginSection("Validate variants");
		for (int i = 1; i < list.Count; i++)
		{
			if (list[i].Validate(character, false))
			{
				DynamicIconBuilder.valid_v_ids.Add(i);
			}
		}
		Profile.EndSection("Validate variants");
		int result;
		if (DynamicIconBuilder.valid_v_ids.Count > 0)
		{
			result = DynamicIconBuilder.valid_v_ids[Random.Range(0, DynamicIconBuilder.valid_v_ids.Count)];
		}
		else
		{
			result = 0;
		}
		Profile.EndSection("Choose variant portrait");
		return result;
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x00111134 File Offset: 0x0010F334
	public Sprite GenerateFace(Logic.Character character, float size = 64f)
	{
		DynamicIconBuilder.CharacterData.VariantData variant = this.GetVariant(character);
		if (variant == null)
		{
			return null;
		}
		character.game.NotifyListeners("clear_portrait", character);
		if (character.portrait_variantID != character.last_variantID || character.portraitID != character.last_portraitID || character.portrait_age != character.last_portrait_age)
		{
			variant.used_by++;
			character.last_portraitID = character.portraitID;
			character.last_portrait_age = character.portrait_age;
			character.last_variantID = character.portrait_variantID;
		}
		return this.GenerateFace(character, variant, size);
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x001111C4 File Offset: 0x0010F3C4
	public void LoadPortraitZip()
	{
		if (DynamicIconBuilder.archives == null)
		{
			try
			{
				if (!Directory.Exists("Portraits/"))
				{
					throw new Exception("Missing portrait zip files");
				}
				int num = (int)Math.Ceiling((double)((float)DynamicIconBuilder.sm_FaceMap.Last<KeyValuePair<int, DynamicIconBuilder.CharacterData>>().Key / 25f));
				DynamicIconBuilder.archives = new ZipArchive[num];
				for (int i = 0; i < num; i++)
				{
					ZipArchive zipArchive = DynamicIconBuilder.archives[i];
					zipArchive = ZipFile.Open("Portraits/" + string.Format("{0}-{1}.zip", i * 25, (i + 1) * 25), ZipArchiveMode.Read);
					DynamicIconBuilder.archives[i] = zipArchive;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Sharing violation : " + ex.Message);
				return;
			}
		}
		for (int j = 0; j < DynamicIconBuilder.archives.Length; j++)
		{
			ReadOnlyCollection<ZipArchiveEntry> entries = DynamicIconBuilder.archives[j].Entries;
			for (int k = entries.Count - 1; k >= 0; k--)
			{
				ZipArchiveEntry zipArchiveEntry = entries[k];
				string[] array = zipArchiveEntry.Name.Split(new char[]
				{
					'-'
				});
				if (array.Length == 4)
				{
					int id = int.Parse(array[0]);
					int age = int.Parse(array[1]);
					int variant = int.Parse(array[2]);
					DynamicIconBuilder.CharacterData.VariantData variant2 = this.GetVariant(id, age, variant);
					if (variant2 != null)
					{
						int key = int.Parse(array[3].Split(new char[]
						{
							'.'
						})[0]);
						variant2.zip_entries[key] = zipArchiveEntry;
					}
				}
			}
		}
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x0011136C File Offset: 0x0010F56C
	public static void LoadZipArchive(DynamicIconBuilder.CharacterData.VariantData variant, ZipArchiveEntry entry, int s, bool replace = false)
	{
		if (variant == null || entry == null)
		{
			return;
		}
		if (variant.GetSprite((float)s, true) != null && !replace)
		{
			return;
		}
		MemoryStream memoryStream = new MemoryStream();
		Stream stream = entry.Open();
		stream.CopyTo(memoryStream);
		stream.Close();
		byte[] array = memoryStream.ToArray();
		if (array != null)
		{
			Texture2D texture2D = new Texture2D(s, s, TextureFormat.ARGB32, false);
			texture2D.LoadImage(array);
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
			variant.SetSprite(sprite, (float)s);
		}
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x00111408 File Offset: 0x0010F608
	public Sprite GenerateFace(Logic.Character character, DynamicIconBuilder.CharacterData.VariantData variant, float size = 64f)
	{
		Sprite sprite = variant.GetSprite(size, false);
		if (sprite != null)
		{
			return sprite;
		}
		float num = float.MaxValue;
		for (int i = 0; i < DynamicIconBuilder.sprite_sizes.Length; i++)
		{
			int num2 = DynamicIconBuilder.sprite_sizes[i];
			ZipArchiveEntry entry;
			if (variant.zip_entries.TryGetValue(num2, out entry))
			{
				DynamicIconBuilder.LoadZipArchive(variant, entry, num2, false);
				Sprite sprite2 = variant.GetSprite((float)num2, true);
				if (sprite2 != null)
				{
					float num3 = Math.Abs((float)num2 - size);
					if (num3 < num)
					{
						num = num3;
						sprite = sprite2;
					}
				}
			}
		}
		return sprite;
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x00111490 File Offset: 0x0010F690
	public Texture2D GetCharacterTexture(int portraitID, int age, int variantID = 0)
	{
		DynamicIconBuilder.CharacterData characterData;
		DynamicIconBuilder.sm_FaceMap.TryGetValue(portraitID, out characterData);
		if (characterData != null)
		{
			List<DynamicIconBuilder.CharacterData.VariantData> list = characterData.variants[age];
			if (list.Count > variantID)
			{
				return list[variantID].fullTexture;
			}
		}
		return null;
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x001114D0 File Offset: 0x0010F6D0
	public Sprite GetSprite(Logic.Character character, float size = 64f)
	{
		float num = 1f;
		if (BaseUI.Get() != null)
		{
			num = Mathf.Max(1f, BaseUI.Get().canvas.scaleFactor);
		}
		size *= num;
		if (DynamicIconBuilder.sm_FaceMap == null)
		{
			return null;
		}
		DynamicIconBuilder.CharacterData.VariantData variant = this.GetVariant(character);
		if (variant != null)
		{
			Sprite sprite = variant.GetSprite(size, false);
			if (sprite != null)
			{
				return sprite;
			}
		}
		return this.GenerateFace(character, size);
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x0011153F File Offset: 0x0010F73F
	public void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		DynamicIconBuilder.InitValidIDs();
		this.ClearFaceMap();
		this.LoadRecipes();
		this.LoadPortraitZip();
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x00111568 File Offset: 0x0010F768
	private void LoadRecipes()
	{
		byte[] array = File.ReadAllBytes("Portraits/KingsRecipes.binary");
		int num = 4;
		int num2 = array.Length / num;
		this.values = new int[num2];
		for (int i = 0; i < num2; i++)
		{
			this.values[i] = this.ReadSignedInt(i, array);
		}
		this.c = 0;
		while (this.c < this.values.Length)
		{
			DynamicIconBuilder.CharacterData characterData = new DynamicIconBuilder.CharacterData();
			characterData.id = this.GetVal();
			characterData.ethnicities = (PresetRecipes.Ethnicities)this.GetVal();
			characterData.sex = (Logic.Character.Sex)this.GetVal();
			for (int j = 0; j < 5; j++)
			{
				characterData.variants[j] = new List<DynamicIconBuilder.CharacterData.VariantData>();
				int val = this.GetVal();
				for (int k = 0; k < val; k++)
				{
					DynamicIconBuilder.CharacterData.VariantData variantData = new DynamicIconBuilder.CharacterData.VariantData();
					variantData.id = this.GetVal();
					variantData.supported_ages = (PresetRecipes.Ages)this.GetVal();
					variantData.supported_classes = (PresetRecipes.Classes)this.GetVal();
					variantData.supported_cultures = (PresetRecipes.Cultures)this.GetVal();
					variantData.supported_religions = (PresetRecipes.Religions)this.GetVal();
					variantData.supported_titles = (PresetRecipes.Titles)this.GetVal();
					variantData.cd = characterData;
					characterData.variants[j].Add(variantData);
				}
			}
			DynamicIconBuilder.Instance.AddCharacterData(characterData, characterData.id);
		}
		this.values = null;
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x001116C4 File Offset: 0x0010F8C4
	private int GetVal()
	{
		int result = this.values[this.c];
		this.c++;
		return result;
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x001116E4 File Offset: 0x0010F8E4
	private int Read7BitUINT(int i, byte[] bytes)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 4;
		int num4 = 0;
		for (;;)
		{
			int num5 = (int)bytes[i * num3 + num4];
			num |= (num5 & 127) << num2;
			if ((num5 & 128) == 0)
			{
				break;
			}
			num2 += 7;
			num4++;
		}
		return num;
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x00111724 File Offset: 0x0010F924
	private int ReadSignedInt(int i, byte[] bytes)
	{
		int num = this.Read7BitUINT(i, bytes);
		int num2 = num >> 1;
		if ((num & 1) != 0)
		{
			num2 = -num2;
		}
		return num2;
	}

	// Token: 0x040012C2 RID: 4802
	private static DynamicIconBuilder _instance = null;

	// Token: 0x040012C3 RID: 4803
	private bool init;

	// Token: 0x040012C4 RID: 4804
	public const int portraits_per_archive = 25;

	// Token: 0x040012C5 RID: 4805
	private const string portraits_dir = "Portraits/";

	// Token: 0x040012C6 RID: 4806
	public static ZipArchive[] archives;

	// Token: 0x040012C7 RID: 4807
	public static Dictionary<int, DynamicIconBuilder.CharacterData> sm_FaceMap = new Dictionary<int, DynamicIconBuilder.CharacterData>();

	// Token: 0x040012C8 RID: 4808
	private static List<int> valid_p_ids;

	// Token: 0x040012C9 RID: 4809
	private static List<int> valid_v_ids;

	// Token: 0x040012CA RID: 4810
	public static readonly int[] sprite_sizes = new int[]
	{
		64,
		128
	};

	// Token: 0x040012CB RID: 4811
	private int c;

	// Token: 0x040012CC RID: 4812
	private int[] values;

	// Token: 0x02000723 RID: 1827
	public class CharacterData
	{
		// Token: 0x060049E2 RID: 18914 RVA: 0x0021E85A File Offset: 0x0021CA5A
		public bool ValidateSex(Logic.Character character)
		{
			return character.sex == this.sex;
		}

		// Token: 0x060049E3 RID: 18915 RVA: 0x0021E86C File Offset: 0x0021CA6C
		public bool ValidateEthnicity(Logic.Character character)
		{
			PresetRecipes.Ethnicities ethnicities;
			switch (character.ethnicity)
			{
			case Logic.Character.Ethnicity.European:
				ethnicities = PresetRecipes.Ethnicities.European;
				break;
			case Logic.Character.Ethnicity.Nordic:
				ethnicities = PresetRecipes.Ethnicities.Nordic;
				break;
			case Logic.Character.Ethnicity.Slavic:
				ethnicities = PresetRecipes.Ethnicities.Slavic;
				break;
			case Logic.Character.Ethnicity.African:
				ethnicities = PresetRecipes.Ethnicities.African;
				break;
			case Logic.Character.Ethnicity.Mongol:
				ethnicities = PresetRecipes.Ethnicities.Mongol;
				break;
			case Logic.Character.Ethnicity.Mediterranian:
				ethnicities = PresetRecipes.Ethnicities.Mediterranian;
				break;
			case Logic.Character.Ethnicity.Arabic:
				ethnicities = PresetRecipes.Ethnicities.Arabic;
				break;
			default:
				ethnicities = PresetRecipes.Ethnicities.Caucasian;
				break;
			}
			return (this.ethnicities & ethnicities) > PresetRecipes.Ethnicities.None;
		}

		// Token: 0x060049E4 RID: 18916 RVA: 0x0021E8D4 File Offset: 0x0021CAD4
		public bool ValidateEthnicity(Logic.Character.Ethnicity e)
		{
			PresetRecipes.Ethnicities ethnicities;
			switch (e)
			{
			case Logic.Character.Ethnicity.European:
				ethnicities = PresetRecipes.Ethnicities.European;
				break;
			case Logic.Character.Ethnicity.Nordic:
				ethnicities = PresetRecipes.Ethnicities.Nordic;
				break;
			case Logic.Character.Ethnicity.Slavic:
				ethnicities = PresetRecipes.Ethnicities.Slavic;
				break;
			case Logic.Character.Ethnicity.African:
				ethnicities = PresetRecipes.Ethnicities.African;
				break;
			case Logic.Character.Ethnicity.Mongol:
				ethnicities = PresetRecipes.Ethnicities.Mongol;
				break;
			case Logic.Character.Ethnicity.Mediterranian:
				ethnicities = PresetRecipes.Ethnicities.Mediterranian;
				break;
			case Logic.Character.Ethnicity.Arabic:
				ethnicities = PresetRecipes.Ethnicities.Arabic;
				break;
			default:
				ethnicities = PresetRecipes.Ethnicities.Caucasian;
				break;
			}
			return (this.ethnicities & ethnicities) > PresetRecipes.Ethnicities.None;
		}

		// Token: 0x060049E5 RID: 18917 RVA: 0x0021E932 File Offset: 0x0021CB32
		public bool Validate(Logic.Character character, bool check_ethnicity = true)
		{
			return this.ValidateSex(character) && (!check_ethnicity || this.ValidateEthnicity(character));
		}

		// Token: 0x04003874 RID: 14452
		public Logic.Character.Sex sex;

		// Token: 0x04003875 RID: 14453
		public PresetRecipes.Ethnicities ethnicities;

		// Token: 0x04003876 RID: 14454
		public int id;

		// Token: 0x04003877 RID: 14455
		public List<DynamicIconBuilder.CharacterData.VariantData>[] variants = new List<DynamicIconBuilder.CharacterData.VariantData>[5];

		// Token: 0x02000A0C RID: 2572
		public class VariantData
		{
			// Token: 0x06005543 RID: 21827 RVA: 0x00248BA1 File Offset: 0x00246DA1
			public void ClearSprites()
			{
				Dictionary<float, Sprite> dictionary = this.sprites;
				if (dictionary != null)
				{
					dictionary.Clear();
				}
				this.sprites = null;
				this.fullTexture = null;
			}

			// Token: 0x06005544 RID: 21828 RVA: 0x00248BC4 File Offset: 0x00246DC4
			public Sprite GetSprite(float size = 64f, bool exact = false)
			{
				if (this.sprites == null)
				{
					return null;
				}
				if (exact)
				{
					Sprite result = null;
					this.sprites.TryGetValue(size, out result);
					return result;
				}
				float num = float.MaxValue;
				Sprite result2 = null;
				foreach (KeyValuePair<float, Sprite> keyValuePair in this.sprites)
				{
					float num2 = Math.Abs(keyValuePair.Key - size);
					if (num2 < num)
					{
						num = num2;
						result2 = keyValuePair.Value;
					}
				}
				return result2;
			}

			// Token: 0x06005545 RID: 21829 RVA: 0x00248C5C File Offset: 0x00246E5C
			public void LoadZipArchives(bool replace = false)
			{
				foreach (KeyValuePair<int, ZipArchiveEntry> keyValuePair in this.zip_entries)
				{
					DynamicIconBuilder.LoadZipArchive(this, keyValuePair.Value, keyValuePair.Key, replace);
				}
			}

			// Token: 0x06005546 RID: 21830 RVA: 0x00248CC0 File Offset: 0x00246EC0
			public Texture GetTexture(float size = 64f)
			{
				if (this.textures == null)
				{
					return null;
				}
				float num = float.MaxValue;
				Texture result = null;
				foreach (KeyValuePair<float, Texture> keyValuePair in this.textures)
				{
					float num2 = Math.Abs(keyValuePair.Key - size);
					if (num2 < num)
					{
						num = num2;
						result = keyValuePair.Value;
					}
				}
				return result;
			}

			// Token: 0x06005547 RID: 21831 RVA: 0x00248D40 File Offset: 0x00246F40
			public void SetSprite(Sprite sprite, float size = 64f)
			{
				if (this.sprites == null)
				{
					this.sprites = new Dictionary<float, Sprite>();
				}
				this.sprites[size] = sprite;
			}

			// Token: 0x06005548 RID: 21832 RVA: 0x00248D62 File Offset: 0x00246F62
			public void SetTexture(Texture sprite, float size = 64f)
			{
				if (this.textures == null)
				{
					this.textures = new Dictionary<float, Texture>();
				}
				this.textures[size] = sprite;
			}

			// Token: 0x06005549 RID: 21833 RVA: 0x00248D84 File Offset: 0x00246F84
			public bool ValidateClass(Logic.Character character)
			{
				string class_name = character.class_name;
				PresetRecipes.Classes classes;
				if (!(class_name == "Marshal"))
				{
					if (!(class_name == "Merchant"))
					{
						if (!(class_name == "Cleric"))
						{
							if (!(class_name == "Spy"))
							{
								if (!(class_name == "Diplomat"))
								{
									classes = PresetRecipes.Classes.Generic;
								}
								else
								{
									classes = PresetRecipes.Classes.Diplomat;
								}
							}
							else
							{
								classes = PresetRecipes.Classes.Spy;
							}
						}
						else
						{
							classes = PresetRecipes.Classes.Cleric;
						}
					}
					else
					{
						classes = PresetRecipes.Classes.Merchant;
					}
				}
				else
				{
					classes = PresetRecipes.Classes.Marshal;
				}
				return (classes & this.supported_classes) > PresetRecipes.Classes.None;
			}

			// Token: 0x0600554A RID: 21834 RVA: 0x00248E00 File Offset: 0x00247000
			public bool ValidateReligion(Logic.Character character)
			{
				Religion religion;
				if (character == null)
				{
					religion = null;
				}
				else
				{
					Logic.Kingdom kingdom = character.GetKingdom();
					religion = ((kingdom != null) ? kingdom.religion : null);
				}
				Religion religion2 = religion;
				PresetRecipes.Religions religions = PresetRecipes.Religions.Catholic;
				if (religion2 != null)
				{
					string name = religion2.name;
					if (!(name == "Catholic"))
					{
						if (!(name == "Orthodox"))
						{
							if (!(name == "Sunni"))
							{
								if (!(name == "Shia"))
								{
									if (name == "Pagan")
									{
										religions = PresetRecipes.Religions.Pagan;
									}
								}
								else
								{
									religions = PresetRecipes.Religions.Shia;
								}
							}
							else
							{
								religions = PresetRecipes.Religions.Sunni;
							}
						}
						else
						{
							religions = PresetRecipes.Religions.Orthodox;
						}
					}
					else
					{
						religions = PresetRecipes.Religions.Catholic;
					}
				}
				return (religions & this.supported_religions) > PresetRecipes.Religions.None;
			}

			// Token: 0x0600554B RID: 21835 RVA: 0x00248E94 File Offset: 0x00247094
			public bool ValidateTitle(Logic.Character character)
			{
				string title = character.title;
				PresetRecipes.Titles titles;
				if (!(title == "King"))
				{
					if (!(title == "Pope"))
					{
						if (!(title == "Queen"))
						{
							if (!(title == "Caliph"))
							{
								if (!(title == "Patriarch"))
								{
									titles = PresetRecipes.Titles.Generic;
								}
								else
								{
									titles = PresetRecipes.Titles.Patriarch;
								}
							}
							else
							{
								titles = PresetRecipes.Titles.Caliph;
							}
						}
						else
						{
							titles = PresetRecipes.Titles.Queen;
						}
					}
					else
					{
						titles = PresetRecipes.Titles.Pope;
					}
				}
				else
				{
					titles = PresetRecipes.Titles.King;
				}
				return (titles & this.supported_titles) > PresetRecipes.Titles.None;
			}

			// Token: 0x0600554C RID: 21836 RVA: 0x00248F0E File Offset: 0x0024710E
			public bool ValidateAge(Logic.Character character)
			{
				return (PresetRecipes.AgeToAges(character) & this.supported_ages) > PresetRecipes.Ages.None;
			}

			// Token: 0x0600554D RID: 21837 RVA: 0x00248F20 File Offset: 0x00247120
			public bool ValidateCulture(Logic.Character character)
			{
				Logic.Kingdom originalKingdom = character.GetOriginalKingdom();
				if (originalKingdom == null)
				{
					return true;
				}
				string culture = originalKingdom.culture;
				string text = character.game.cultures.GetGroup(culture);
				if (string.IsNullOrEmpty(text))
				{
					text = culture;
				}
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				PresetRecipes.Cultures cultures;
				if (num <= 1366458497U)
				{
					if (num <= 475091125U)
					{
						if (num <= 299966229U)
						{
							if (num != 82837899U)
							{
								if (num != 144161276U)
								{
									if (num == 299966229U)
									{
										if (text == "Arberian")
										{
											cultures = PresetRecipes.Cultures.Arberian;
											goto IL_4AE;
										}
									}
								}
								else if (text == "Gaelic")
								{
									cultures = PresetRecipes.Cultures.Gaelic;
									goto IL_4AE;
								}
							}
							else if (text == "African")
							{
								cultures = PresetRecipes.Cultures.AFRICANGROUPS;
								goto IL_4AE;
							}
						}
						else if (num != 334219777U)
						{
							if (num != 337007010U)
							{
								if (num == 475091125U)
								{
									if (text == "Wallachian")
									{
										cultures = PresetRecipes.Cultures.Wallachian;
										goto IL_4AE;
									}
								}
							}
							else if (text == "Armenian")
							{
								cultures = PresetRecipes.Cultures.Armenian;
								goto IL_4AE;
							}
						}
						else if (text == "Latin")
						{
							cultures = PresetRecipes.Cultures.Latin;
							goto IL_4AE;
						}
					}
					else if (num <= 820546001U)
					{
						if (num != 498922913U)
						{
							if (num != 792887735U)
							{
								if (num == 820546001U)
								{
									if (text == "Mongolic")
									{
										cultures = PresetRecipes.Cultures.MONGOLGROUPS;
										goto IL_4AE;
									}
								}
							}
							else if (text == "Germanic")
							{
								cultures = PresetRecipes.Cultures.Germanic;
								goto IL_4AE;
							}
						}
						else if (text == "Turkic")
						{
							cultures = PresetRecipes.Cultures.Turkic;
							goto IL_4AE;
						}
					}
					else if (num != 1259462800U)
					{
						if (num != 1313277548U)
						{
							if (num == 1366458497U)
							{
								if (text == "FinnoUgric")
								{
									cultures = PresetRecipes.Cultures.FinnoUgric;
									goto IL_4AE;
								}
							}
						}
						else if (text == "Nordic")
						{
							cultures = PresetRecipes.Cultures.Nordic;
							goto IL_4AE;
						}
					}
					else if (text == "Magyar")
					{
						cultures = PresetRecipes.Cultures.Magyar;
						goto IL_4AE;
					}
				}
				else if (num <= 2790507171U)
				{
					if (num <= 1987938472U)
					{
						if (num != 1532612597U)
						{
							if (num != 1971889810U)
							{
								if (num == 1987938472U)
								{
									if (text == "EastSlavic")
									{
										cultures = PresetRecipes.Cultures.EastSlavic;
										goto IL_4AE;
									}
								}
							}
							else if (text == "SouthSlavic")
							{
								cultures = PresetRecipes.Cultures.SouthSlavic;
								goto IL_4AE;
							}
						}
						else if (text == "Semitic")
						{
							cultures = PresetRecipes.Cultures.Semitic;
							goto IL_4AE;
						}
					}
					else if (num != 2624879012U)
					{
						if (num != 2656823083U)
						{
							if (num == 2790507171U)
							{
								if (text == "Greek")
								{
									cultures = PresetRecipes.Cultures.Greek;
									goto IL_4AE;
								}
							}
						}
						else if (text == "Berber")
						{
							cultures = PresetRecipes.Cultures.Berber;
							goto IL_4AE;
						}
					}
					else if (text == "Baltic")
					{
						cultures = PresetRecipes.Cultures.Baltic;
						goto IL_4AE;
					}
				}
				else if (num <= 3378020390U)
				{
					if (num != 3089066645U)
					{
						if (num != 3221088397U)
						{
							if (num == 3378020390U)
							{
								if (text == "WestSlavic")
								{
									cultures = PresetRecipes.Cultures.WestSlavic;
									goto IL_4AE;
								}
							}
						}
						else if (text == "Celtic")
						{
							cultures = PresetRecipes.Cultures.Celtic;
							goto IL_4AE;
						}
					}
					else if (text == "Iranic")
					{
						cultures = PresetRecipes.Cultures.Iranic;
						goto IL_4AE;
					}
				}
				else if (num <= 3496570627U)
				{
					if (num != 3482029931U)
					{
						if (num == 3496570627U)
						{
							if (text == "Iberian")
							{
								cultures = PresetRecipes.Cultures.Iberian;
								goto IL_4AE;
							}
						}
					}
					else if (text == "Caucasian")
					{
						cultures = PresetRecipes.Cultures.Caucasian;
						goto IL_4AE;
					}
				}
				else if (num != 3621870633U)
				{
					if (num == 3851292132U)
					{
						if (text == "WestGermanic")
						{
							cultures = PresetRecipes.Cultures.WestGermanic;
							goto IL_4AE;
						}
					}
				}
				else if (text == "Arabic")
				{
					cultures = PresetRecipes.Cultures.Arab;
					goto IL_4AE;
				}
				cultures = PresetRecipes.Cultures.Caucasian;
				IL_4AE:
				return (cultures & this.supported_cultures) > PresetRecipes.Cultures.None;
			}

			// Token: 0x0600554E RID: 21838 RVA: 0x002493E8 File Offset: 0x002475E8
			public bool Validate(Logic.Character character, bool check_ethnicity = true)
			{
				return character != null && this.cd.Validate(character, check_ethnicity) && this.ValidateAge(character) && this.ValidateReligion(character) && this.ValidateTitle(character) && this.ValidateClass(character) && this.ValidateCulture(character);
			}

			// Token: 0x0400464D RID: 17997
			public int id;

			// Token: 0x0400464E RID: 17998
			public int used_by;

			// Token: 0x0400464F RID: 17999
			public DynamicIconBuilder.CharacterData cd;

			// Token: 0x04004650 RID: 18000
			public Dictionary<float, Sprite> sprites;

			// Token: 0x04004651 RID: 18001
			public Dictionary<float, Texture> textures;

			// Token: 0x04004652 RID: 18002
			public Dictionary<int, ZipArchiveEntry> zip_entries = new Dictionary<int, ZipArchiveEntry>();

			// Token: 0x04004653 RID: 18003
			public PresetRecipes.Titles supported_titles;

			// Token: 0x04004654 RID: 18004
			public PresetRecipes.Classes supported_classes;

			// Token: 0x04004655 RID: 18005
			public PresetRecipes.Religions supported_religions;

			// Token: 0x04004656 RID: 18006
			public PresetRecipes.Cultures supported_cultures;

			// Token: 0x04004657 RID: 18007
			public PresetRecipes.Ages supported_ages;

			// Token: 0x04004658 RID: 18008
			public Texture2D fullTexture;
		}
	}
}
