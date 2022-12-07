using System;
using System.Collections.Generic;
using Logic;

// Token: 0x020002E2 RID: 738
public class TargetPickerData
{
	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06002EC7 RID: 11975 RVA: 0x0002C53B File Offset: 0x0002A73B
	public bool isLogicObject
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x00181B64 File Offset: 0x0017FD64
	public static TargetPickerData Create(Value obj, Vars vars, Func<Value, bool> validate = null)
	{
		return new TargetPickerData
		{
			Target = obj,
			Vars = vars,
			Validate = validate
		};
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x00181B80 File Offset: 0x0017FD80
	public static List<TargetPickerData> Create(List<Value> valsList, List<Vars> varsList = null, Func<Value, bool> validate = null)
	{
		if (valsList == null)
		{
			return null;
		}
		if (valsList.Count == 0)
		{
			return null;
		}
		List<TargetPickerData> list = new List<TargetPickerData>(valsList.Count);
		for (int i = 0; i < valsList.Count; i++)
		{
			list.Add(new TargetPickerData
			{
				Target = valsList[i],
				Vars = ((varsList != null && varsList.Count > i) ? varsList[i] : null),
				Validate = validate
			});
		}
		list.Sort(new TargetPickerData.TargetListComparer());
		return list;
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x00181C00 File Offset: 0x0017FE00
	public static List<TargetPickerData> Create(List<Value> valsList, Vars vars = null, Func<Value, bool> validate = null)
	{
		if (valsList == null)
		{
			return null;
		}
		if (valsList.Count == 0)
		{
			return null;
		}
		List<TargetPickerData> list = new List<TargetPickerData>(valsList.Count);
		for (int i = 0; i < valsList.Count; i++)
		{
			list.Add(new TargetPickerData
			{
				Target = valsList[i],
				Vars = vars,
				Validate = validate
			});
		}
		list.Sort(new TargetPickerData.TargetListComparer());
		return list;
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x00181C6C File Offset: 0x0017FE6C
	public static List<TargetPickerData> Create(List<Object> objList, List<Vars> varsList = null, Func<Value, bool> validate = null)
	{
		if (objList == null)
		{
			return null;
		}
		if (objList.Count == 0)
		{
			return null;
		}
		List<TargetPickerData> list = new List<TargetPickerData>(objList.Count);
		for (int i = 0; i < objList.Count; i++)
		{
			list.Add(new TargetPickerData
			{
				Target = objList[i],
				Vars = ((varsList != null && varsList.Count > i) ? varsList[i] : null),
				Validate = validate
			});
		}
		list.Sort(new TargetPickerData.TargetListComparer());
		return list;
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x00181CF4 File Offset: 0x0017FEF4
	public static List<TargetPickerData> Create(List<Object> objList, Vars vars, Func<Value, bool> validate = null)
	{
		if (objList == null)
		{
			return null;
		}
		if (objList.Count == 0)
		{
			return null;
		}
		List<TargetPickerData> list = new List<TargetPickerData>(objList.Count);
		for (int i = 0; i < objList.Count; i++)
		{
			list.Add(new TargetPickerData
			{
				Target = objList[i],
				Vars = vars,
				Validate = validate
			});
		}
		list.Sort(new TargetPickerData.TargetListComparer());
		return list;
	}

	// Token: 0x04001FA7 RID: 8103
	public Value Target;

	// Token: 0x04001FA8 RID: 8104
	public Vars Vars;

	// Token: 0x04001FA9 RID: 8105
	public Func<Value, bool> Validate;

	// Token: 0x02000858 RID: 2136
	public class TargetListComparer : IComparer<TargetPickerData>
	{
		// Token: 0x060050C2 RID: 20674 RVA: 0x0023AB88 File Offset: 0x00238D88
		public int Compare(TargetPickerData x, TargetPickerData y)
		{
			if (x.Target.is_object)
			{
				object obj_val = x.Target.obj_val;
				if (obj_val != null)
				{
					Logic.Kingdom kingdom;
					if ((kingdom = (obj_val as Logic.Kingdom)) != null)
					{
						Logic.Kingdom kingdom2 = kingdom;
						if (kingdom2 != null)
						{
							if (y.Target.is_object)
							{
								Logic.Kingdom kingdom3 = y.Target.obj_val as Logic.Kingdom;
								if (kingdom3 == null)
								{
									return 0;
								}
								return string.Compare(kingdom2.Name, kingdom3.Name);
							}
							else
							{
								if (y.Target.is_string)
								{
									return string.Compare(kingdom2.Name, y.Target.String(null));
								}
								if (y.Target.is_number)
								{
									return string.Compare(kingdom2.Name, y.Target.Float(0f).ToString());
								}
								return 0;
							}
						}
					}
					Logic.Realm realm;
					if ((realm = (obj_val as Logic.Realm)) != null)
					{
						Logic.Realm realm2 = realm;
						if (realm2 != null)
						{
							if (y.Target.is_object)
							{
								Logic.Realm realm3 = y.Target.obj_val as Logic.Realm;
								if (realm3 == null)
								{
									return 0;
								}
								return string.Compare(realm2.name, realm3.name);
							}
							else
							{
								if (y.Target.is_string)
								{
									return string.Compare(realm2.name, y.Target.String(null));
								}
								if (y.Target.is_number)
								{
									return string.Compare(realm2.name, y.Target.Float(0f).ToString());
								}
								return 0;
							}
						}
					}
					Logic.Character character;
					if ((character = (obj_val as Logic.Character)) != null)
					{
						Logic.Character character2 = character;
						if (character2 != null)
						{
							if (y.Target.is_object)
							{
								Logic.Character character3 = y.Target.obj_val as Logic.Character;
								if (character3 == null)
								{
									return 0;
								}
								return string.Compare(character2.Name, character3.Name);
							}
							else
							{
								if (y.Target.is_string)
								{
									return string.Compare(character2.Name, y.Target.String(null));
								}
								if (y.Target.is_number)
								{
									return string.Compare(character2.Name, y.Target.Float(0f).ToString());
								}
								return 0;
							}
						}
					}
				}
			}
			bool is_object = x.Target.is_object;
			return 0;
		}
	}
}
