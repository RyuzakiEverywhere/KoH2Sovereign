using System;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Events
{
	// Token: 0x0200044F RID: 1103
	[CreateAssetMenu(menuName = "Malbers Animations/Event", fileName = "New Event Asset")]
	public class MEvent : ScriptableObject
	{
		// Token: 0x06003AAF RID: 15023 RVA: 0x001C3BD0 File Offset: 0x001C1DD0
		public virtual void Invoke()
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked();
			}
		}

		// Token: 0x06003AB0 RID: 15024 RVA: 0x001C3C08 File Offset: 0x001C1E08
		public virtual void Invoke(float value)
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked(value);
			}
		}

		// Token: 0x06003AB1 RID: 15025 RVA: 0x001C3C40 File Offset: 0x001C1E40
		public virtual void Invoke(bool value)
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked(value);
			}
		}

		// Token: 0x06003AB2 RID: 15026 RVA: 0x001C3C78 File Offset: 0x001C1E78
		public virtual void Invoke(string value)
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked(value);
			}
		}

		// Token: 0x06003AB3 RID: 15027 RVA: 0x001C3CB0 File Offset: 0x001C1EB0
		public virtual void Invoke(int value)
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked(value);
			}
		}

		// Token: 0x06003AB4 RID: 15028 RVA: 0x001C3CE8 File Offset: 0x001C1EE8
		public virtual void Invoke(GameObject value)
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked(value);
			}
		}

		// Token: 0x06003AB5 RID: 15029 RVA: 0x001C3D20 File Offset: 0x001C1F20
		public virtual void Invoke(Transform value)
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked(value);
			}
		}

		// Token: 0x06003AB6 RID: 15030 RVA: 0x001C3D58 File Offset: 0x001C1F58
		public virtual void Invoke(Vector3 value)
		{
			for (int i = this.eventListeners.Count - 1; i >= 0; i--)
			{
				this.eventListeners[i].OnEventInvoked(value);
			}
		}

		// Token: 0x06003AB7 RID: 15031 RVA: 0x001C3D8F File Offset: 0x001C1F8F
		public virtual void RegisterListener(MEventItemListener listener)
		{
			if (!this.eventListeners.Contains(listener))
			{
				this.eventListeners.Add(listener);
			}
		}

		// Token: 0x06003AB8 RID: 15032 RVA: 0x001C3DAB File Offset: 0x001C1FAB
		public virtual void UnregisterListener(MEventItemListener listener)
		{
			if (this.eventListeners.Contains(listener))
			{
				this.eventListeners.Remove(listener);
			}
		}

		// Token: 0x06003AB9 RID: 15033 RVA: 0x0000AEA5 File Offset: 0x000090A5
		public virtual void DebugLog(string text)
		{
			Debug.Log(text);
		}

		// Token: 0x04002A6E RID: 10862
		private readonly List<MEventItemListener> eventListeners = new List<MEventItemListener>();
	}
}
