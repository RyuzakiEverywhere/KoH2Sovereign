using System;
using System.Collections.Generic;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020002E5 RID: 741
public class UIBindings : RemoteVars.IListener
{
	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06002EF9 RID: 12025 RVA: 0x0018302C File Offset: 0x0018122C
	// (set) Token: 0x06002EFA RID: 12026 RVA: 0x00183034 File Offset: 0x00181234
	public RemoteVars vars { get; private set; }

	// Token: 0x06002EFB RID: 12027 RVA: 0x0018303D File Offset: 0x0018123D
	public UIBindings(RemoteVars vars)
	{
		this.vars = vars;
		if (vars != null)
		{
			vars.AddListener(this);
		}
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x00183064 File Offset: 0x00181264
	public void Destroy()
	{
		RemoteVars vars = this.vars;
		if (vars != null)
		{
			vars.DelListener(this);
		}
		foreach (KeyValuePair<string, UIBindings.Bond> keyValuePair in this.bindings)
		{
			UIBindings.Bond value = keyValuePair.Value;
			if (value.remove_listener != null)
			{
				value.remove_listener();
			}
		}
		this.bindings.Clear();
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x001830E8 File Offset: 0x001812E8
	public void BindAll(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		if (go.name.StartsWith("id_", StringComparison.Ordinal))
		{
			MonoBehaviour ctlComponent = UIBindings.GetCtlComponent(go);
			if (ctlComponent != null)
			{
				this.Bind(ctlComponent, null);
			}
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			Transform child = go.transform.GetChild(i);
			this.BindAll(child.gameObject);
		}
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x0018315C File Offset: 0x0018135C
	public void BindAll(GameObject go, string child_name)
	{
		GameObject go2 = global::Common.FindChildByName(go, child_name, true, true);
		this.BindAll(go2);
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x0018317C File Offset: 0x0018137C
	public void Bind(GameObject go, string child_name, string var_name = null)
	{
		MonoBehaviour ctlComponent = UIBindings.GetCtlComponent(global::Common.FindChildByName(go, child_name, true, true));
		if (ctlComponent == null)
		{
			Debug.LogError("Unknown UI control: " + go.name + "." + child_name);
			return;
		}
		this.Bind(ctlComponent, var_name);
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x001831C8 File Offset: 0x001813C8
	public void Bind(MonoBehaviour ctl, string var_name = null)
	{
		if (var_name == null)
		{
			var_name = ctl.name;
		}
		UIBindings.Bond bond = new UIBindings.Bond
		{
			var_name = var_name,
			ctl = ctl
		};
		if (ctl != null)
		{
			TMP_Text tmp_Text;
			if ((tmp_Text = (ctl as TMP_Text)) == null)
			{
				TMP_InputField tmp_InputField;
				if ((tmp_InputField = (ctl as TMP_InputField)) == null)
				{
					TMP_Dropdown tmp_Dropdown;
					if ((tmp_Dropdown = (ctl as TMP_Dropdown)) == null)
					{
						Toggle toggle;
						if ((toggle = (ctl as Toggle)) == null)
						{
							goto IL_8A;
						}
						Toggle ctl2 = toggle;
						this.AddListener(ctl2, bond);
					}
					else
					{
						TMP_Dropdown ctl3 = tmp_Dropdown;
						this.AddListener(ctl3, bond);
					}
				}
				else
				{
					TMP_InputField ctl4 = tmp_InputField;
					this.AddListener(ctl4, bond);
				}
			}
			else
			{
				TMP_Text ctl5 = tmp_Text;
				this.AddListener(ctl5, bond);
			}
			this.bindings.Add(var_name, bond);
			return;
		}
		IL_8A:
		Debug.LogError("Unknown control type: " + ctl.GetType().Name + " " + ctl.name);
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x00183294 File Offset: 0x00181494
	public void SetUserValidator(string var_name, Func<MonoBehaviour, Value, bool> validator)
	{
		UIBindings.Bond bond;
		if (!this.bindings.TryGetValue(var_name, out bond))
		{
			Debug.LogError("Unknown binding: " + var_name);
			return;
		}
		bond.user_validate = validator;
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x001832CC File Offset: 0x001814CC
	public void SetRemoteValidator(string var_name, Func<MonoBehaviour, Value, bool> validator)
	{
		UIBindings.Bond bond;
		if (!this.bindings.TryGetValue(var_name, out bond))
		{
			Debug.LogError("Unknown binding: " + var_name);
			return;
		}
		bond.remote_validate = validator;
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x00183304 File Offset: 0x00181504
	public void OnVarChanged(RemoteVars vars, string key, Value old_val, Value new_val)
	{
		if (key == null)
		{
			return;
		}
		UIBindings.Bond bond;
		if (!this.bindings.TryGetValue(key, out bond))
		{
			return;
		}
		if (bond.remote_validate != null && !bond.remote_validate(bond.ctl, new_val))
		{
			return;
		}
		this.ignore_field_changed = true;
		try
		{
			bond.set_value(new_val);
		}
		catch
		{
		}
		this.ignore_field_changed = false;
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x00183374 File Offset: 0x00181574
	private void OnFieldChanged(UIBindings.Bond bond, Value val)
	{
		if (this.ignore_field_changed)
		{
			return;
		}
		if (bond.user_validate != null && !bond.user_validate(bond.ctl, val))
		{
			return;
		}
		RemoteVars vars = this.vars;
		if (vars == null)
		{
			return;
		}
		vars.Set(bond.var_name, val, true);
	}

	// Token: 0x06002F05 RID: 12037 RVA: 0x001833B4 File Offset: 0x001815B4
	public static MonoBehaviour GetCtlComponent(GameObject go)
	{
		if (go == null)
		{
			return null;
		}
		foreach (UnityEngine.Component component in go.GetComponents(typeof(MonoBehaviour)))
		{
			if (component != null)
			{
				TMP_Text result;
				if ((result = (component as TMP_Text)) != null)
				{
					return result;
				}
				TMP_InputField result2;
				if ((result2 = (component as TMP_InputField)) != null)
				{
					return result2;
				}
				TMP_Dropdown result3;
				if ((result3 = (component as TMP_Dropdown)) != null)
				{
					return result3;
				}
				Toggle result4;
				if ((result4 = (component as Toggle)) != null)
				{
					return result4;
				}
			}
		}
		return null;
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x0018342C File Offset: 0x0018162C
	private void AddListener(TMP_Text ctl, UIBindings.Bond bond)
	{
		bond.set_value = delegate(Value val)
		{
			ctl.text = val.String(null);
		};
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x00183458 File Offset: 0x00181658
	private void AddListener(TMP_InputField ctl, UIBindings.Bond bond)
	{
		UnityAction<string> listener = delegate(string val)
		{
			this.OnFieldChanged(bond, val);
		};
		ctl.onSubmit.AddListener(listener);
		bond.set_value = delegate(Value val)
		{
			ctl.text = val.String(null);
		};
		bond.remove_listener = delegate()
		{
			ctl.onSubmit.RemoveListener(listener);
		};
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x001834D8 File Offset: 0x001816D8
	private void AddListener(TMP_Dropdown ctl, UIBindings.Bond bond)
	{
		UnityAction<int> listener = delegate(int val)
		{
			this.OnFieldChanged(bond, val);
		};
		ctl.onValueChanged.AddListener(listener);
		bond.set_value = delegate(Value val)
		{
			ctl.value = val.Int(0);
		};
		bond.remove_listener = delegate()
		{
			ctl.onValueChanged.RemoveListener(listener);
		};
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x00183558 File Offset: 0x00181758
	private void AddListener(Toggle ctl, UIBindings.Bond bond)
	{
		UnityAction<bool> listener = delegate(bool val)
		{
			this.OnFieldChanged(bond, val);
		};
		ctl.onValueChanged.AddListener(listener);
		bond.set_value = delegate(Value val)
		{
			ctl.isOn = val.Bool();
		};
		bond.remove_listener = delegate()
		{
			ctl.onValueChanged.RemoveListener(listener);
		};
	}

	// Token: 0x04001FBD RID: 8125
	public Dictionary<string, UIBindings.Bond> bindings = new Dictionary<string, UIBindings.Bond>();

	// Token: 0x04001FBE RID: 8126
	private bool ignore_field_changed;

	// Token: 0x0200085E RID: 2142
	public class Bond
	{
		// Token: 0x04003EEE RID: 16110
		public string var_name;

		// Token: 0x04003EEF RID: 16111
		public MonoBehaviour ctl;

		// Token: 0x04003EF0 RID: 16112
		public Func<MonoBehaviour, Value, bool> user_validate;

		// Token: 0x04003EF1 RID: 16113
		public Func<MonoBehaviour, Value, bool> remote_validate;

		// Token: 0x04003EF2 RID: 16114
		public Action<Value> set_value;

		// Token: 0x04003EF3 RID: 16115
		public Action remove_listener;
	}
}
