using System;
using Logic;

// Token: 0x020001E3 RID: 483
public abstract class PresetRecipes
{
	// Token: 0x06001CE3 RID: 7395 RVA: 0x0011176F File Offset: 0x0010F96F
	public static PresetRecipes.Ages IntToAges(int i)
	{
		switch (i)
		{
		case 0:
			return PresetRecipes.Ages.Infant;
		case 1:
			return PresetRecipes.Ages.Child;
		case 2:
			return PresetRecipes.Ages.Young;
		case 3:
			return PresetRecipes.Ages.Adult;
		default:
			return PresetRecipes.Ages.Old;
		}
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x00111793 File Offset: 0x0010F993
	public static int CharacterToAges(Logic.Character character)
	{
		return PresetRecipes.AgeToInt(PresetRecipes.AgeToAges(character));
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x001117A0 File Offset: 0x0010F9A0
	public static int AgeToInt(PresetRecipes.Ages age)
	{
		int result = 0;
		switch (age)
		{
		case PresetRecipes.Ages.Infant:
			result = 0;
			break;
		case PresetRecipes.Ages.Child:
			result = 1;
			break;
		case (PresetRecipes.Ages)3:
			break;
		case PresetRecipes.Ages.Young:
			result = 2;
			break;
		default:
			if (age != PresetRecipes.Ages.Adult)
			{
				if (age == PresetRecipes.Ages.Old)
				{
					result = 4;
				}
			}
			else
			{
				result = 3;
			}
			break;
		}
		return result;
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x001117E8 File Offset: 0x0010F9E8
	public static PresetRecipes.Ages AgeToAges(Logic.Character character)
	{
		Logic.Character.Age age = character.age;
		if (character.sex == Logic.Character.Sex.Female)
		{
			if (age == Logic.Character.Age.Young)
			{
				age = Logic.Character.Age.Adult;
			}
			if (age >= Logic.Character.Age.Old)
			{
				age = Logic.Character.Age.Adult;
			}
		}
		return PresetRecipes.AgeToAges(age);
	}

	// Token: 0x06001CE7 RID: 7399 RVA: 0x00111817 File Offset: 0x0010FA17
	public static PresetRecipes.Ages AgeToAges(Logic.Character.Age age)
	{
		switch (age)
		{
		case Logic.Character.Age.Infant:
			return PresetRecipes.Ages.Infant;
		case Logic.Character.Age.Child:
		case Logic.Character.Age.Juvenile:
			return PresetRecipes.Ages.Child;
		case Logic.Character.Age.Young:
			return PresetRecipes.Ages.Young;
		case Logic.Character.Age.Adult:
			return PresetRecipes.Ages.Adult;
		default:
			return PresetRecipes.Ages.Old;
		}
	}

	// Token: 0x040012CD RID: 4813
	public const int AgeCount = 5;

	// Token: 0x02000724 RID: 1828
	public enum Titles
	{
		// Token: 0x04003879 RID: 14457
		None,
		// Token: 0x0400387A RID: 14458
		King,
		// Token: 0x0400387B RID: 14459
		Pope,
		// Token: 0x0400387C RID: 14460
		Queen = 4,
		// Token: 0x0400387D RID: 14461
		Caliph = 8,
		// Token: 0x0400387E RID: 14462
		Patriarch = 16,
		// Token: 0x0400387F RID: 14463
		Generic = 32,
		// Token: 0x04003880 RID: 14464
		Everything = -1
	}

	// Token: 0x02000725 RID: 1829
	public enum Classes
	{
		// Token: 0x04003882 RID: 14466
		None,
		// Token: 0x04003883 RID: 14467
		Marshal,
		// Token: 0x04003884 RID: 14468
		Cleric,
		// Token: 0x04003885 RID: 14469
		Spy = 4,
		// Token: 0x04003886 RID: 14470
		Diplomat = 8,
		// Token: 0x04003887 RID: 14471
		Merchant = 16,
		// Token: 0x04003888 RID: 14472
		Generic = 32,
		// Token: 0x04003889 RID: 14473
		Everything = -1
	}

	// Token: 0x02000726 RID: 1830
	public enum Religions
	{
		// Token: 0x0400388B RID: 14475
		None,
		// Token: 0x0400388C RID: 14476
		Muslim = 12,
		// Token: 0x0400388D RID: 14477
		Christian = 3,
		// Token: 0x0400388E RID: 14478
		Catholic = 1,
		// Token: 0x0400388F RID: 14479
		Orthodox,
		// Token: 0x04003890 RID: 14480
		Sunni = 4,
		// Token: 0x04003891 RID: 14481
		Shia = 8,
		// Token: 0x04003892 RID: 14482
		Pagan = 16,
		// Token: 0x04003893 RID: 14483
		Everything = -1
	}

	// Token: 0x02000727 RID: 1831
	public enum Cultures
	{
		// Token: 0x04003895 RID: 14485
		None,
		// Token: 0x04003896 RID: 14486
		EUROPEANGROUPS = 8191,
		// Token: 0x04003897 RID: 14487
		NORDICGROUPS = 57344,
		// Token: 0x04003898 RID: 14488
		SLAVICGROUPS = 458752,
		// Token: 0x04003899 RID: 14489
		ARABICGROUPS = 7864320,
		// Token: 0x0400389A RID: 14490
		AFRICANGROUPS = 8388608,
		// Token: 0x0400389B RID: 14491
		MONGOLGROUPS = 16777216,
		// Token: 0x0400389C RID: 14492
		Armenian = 1,
		// Token: 0x0400389D RID: 14493
		Arberian,
		// Token: 0x0400389E RID: 14494
		Caucasian = 4,
		// Token: 0x0400389F RID: 14495
		Celtic = 8,
		// Token: 0x040038A0 RID: 14496
		Gaelic = 16,
		// Token: 0x040038A1 RID: 14497
		Germanic = 32,
		// Token: 0x040038A2 RID: 14498
		WestGermanic = 64,
		// Token: 0x040038A3 RID: 14499
		Greek = 128,
		// Token: 0x040038A4 RID: 14500
		Iberian = 256,
		// Token: 0x040038A5 RID: 14501
		Latin = 512,
		// Token: 0x040038A6 RID: 14502
		Magyar = 1024,
		// Token: 0x040038A7 RID: 14503
		Wallachian = 2048,
		// Token: 0x040038A8 RID: 14504
		Semitic = 4096,
		// Token: 0x040038A9 RID: 14505
		FinnoUgric = 8192,
		// Token: 0x040038AA RID: 14506
		Nordic = 16384,
		// Token: 0x040038AB RID: 14507
		Baltic = 32768,
		// Token: 0x040038AC RID: 14508
		EastSlavic = 65536,
		// Token: 0x040038AD RID: 14509
		SouthSlavic = 131072,
		// Token: 0x040038AE RID: 14510
		WestSlavic = 262144,
		// Token: 0x040038AF RID: 14511
		Arab = 524288,
		// Token: 0x040038B0 RID: 14512
		Berber = 1048576,
		// Token: 0x040038B1 RID: 14513
		Iranic = 2097152,
		// Token: 0x040038B2 RID: 14514
		Turkic = 4194304,
		// Token: 0x040038B3 RID: 14515
		African = 8388608,
		// Token: 0x040038B4 RID: 14516
		Mongol = 16777216,
		// Token: 0x040038B5 RID: 14517
		Everything = -1
	}

	// Token: 0x02000728 RID: 1832
	public enum Ages
	{
		// Token: 0x040038B7 RID: 14519
		None,
		// Token: 0x040038B8 RID: 14520
		Infant,
		// Token: 0x040038B9 RID: 14521
		Child,
		// Token: 0x040038BA RID: 14522
		Young = 4,
		// Token: 0x040038BB RID: 14523
		Adult = 8,
		// Token: 0x040038BC RID: 14524
		Old = 16,
		// Token: 0x040038BD RID: 14525
		Everything = -1
	}

	// Token: 0x02000729 RID: 1833
	public enum Ethnicities
	{
		// Token: 0x040038BF RID: 14527
		None,
		// Token: 0x040038C0 RID: 14528
		Caucasian = 39,
		// Token: 0x040038C1 RID: 14529
		European = 1,
		// Token: 0x040038C2 RID: 14530
		Nordic,
		// Token: 0x040038C3 RID: 14531
		Slavic = 4,
		// Token: 0x040038C4 RID: 14532
		African = 8,
		// Token: 0x040038C5 RID: 14533
		Mongol = 16,
		// Token: 0x040038C6 RID: 14534
		Mediterranian = 32,
		// Token: 0x040038C7 RID: 14535
		Arabic = 64,
		// Token: 0x040038C8 RID: 14536
		Everything = -1
	}
}
