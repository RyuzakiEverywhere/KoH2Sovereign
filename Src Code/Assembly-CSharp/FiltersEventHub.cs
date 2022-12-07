using System;

// Token: 0x02000204 RID: 516
public class FiltersEventHub
{
	// Token: 0x06001F62 RID: 8034 RVA: 0x00122CD6 File Offset: 0x00120ED6
	public void AddListener(FiltersEventHub.FilterChanged listener)
	{
		this.filterChanged = (FiltersEventHub.FilterChanged)Delegate.Combine(this.filterChanged, listener);
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x00122CEF File Offset: 0x00120EEF
	public void RemoveListener(FiltersEventHub.FilterChanged listener)
	{
		this.filterChanged = (FiltersEventHub.FilterChanged)Delegate.Remove(this.filterChanged, listener);
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x00122D08 File Offset: 0x00120F08
	public void CallEvent(string filter, bool isOn)
	{
		FiltersEventHub.FilterChanged filterChanged = this.filterChanged;
		if (filterChanged == null)
		{
			return;
		}
		filterChanged(filter, isOn);
	}

	// Token: 0x040014D7 RID: 5335
	public FiltersEventHub.FilterChanged filterChanged;

	// Token: 0x02000741 RID: 1857
	// (Invoke) Token: 0x06004A55 RID: 19029
	public delegate void FilterChanged(string filter, bool isOn);
}
