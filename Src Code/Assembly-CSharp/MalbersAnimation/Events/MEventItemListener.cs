using System;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Events
{
	// Token: 0x02000450 RID: 1104
	[Serializable]
	public class MEventItemListener
	{
		// Token: 0x06003ABB RID: 15035 RVA: 0x001C3DDB File Offset: 0x001C1FDB
		public virtual void OnEventInvoked()
		{
			this.Response.Invoke();
		}

		// Token: 0x06003ABC RID: 15036 RVA: 0x001C3DE8 File Offset: 0x001C1FE8
		public virtual void OnEventInvoked(string value)
		{
			this.ResponseString.Invoke(value);
		}

		// Token: 0x06003ABD RID: 15037 RVA: 0x001C3DF6 File Offset: 0x001C1FF6
		public virtual void OnEventInvoked(float value)
		{
			this.ResponseFloat.Invoke(value);
		}

		// Token: 0x06003ABE RID: 15038 RVA: 0x001C3E04 File Offset: 0x001C2004
		public virtual void OnEventInvoked(int value)
		{
			this.ResponseInt.Invoke(value);
		}

		// Token: 0x06003ABF RID: 15039 RVA: 0x001C3E12 File Offset: 0x001C2012
		public virtual void OnEventInvoked(bool value)
		{
			this.ResponseBool.Invoke(value);
		}

		// Token: 0x06003AC0 RID: 15040 RVA: 0x001C3E20 File Offset: 0x001C2020
		public virtual void OnEventInvoked(GameObject value)
		{
			this.ResponseGO.Invoke(value);
		}

		// Token: 0x06003AC1 RID: 15041 RVA: 0x001C3E2E File Offset: 0x001C202E
		public virtual void OnEventInvoked(Transform value)
		{
			this.ResponseTransform.Invoke(value);
		}

		// Token: 0x06003AC2 RID: 15042 RVA: 0x001C3E3C File Offset: 0x001C203C
		public virtual void OnEventInvoked(Vector3 value)
		{
			this.ResponseVector.Invoke(value);
		}

		// Token: 0x06003AC3 RID: 15043 RVA: 0x001C3E4C File Offset: 0x001C204C
		public MEventItemListener()
		{
			this.useVoid = true;
			this.useInt = (this.useFloat = (this.useString = (this.useBool = (this.useGO = (this.useTransform = (this.useVector = false))))));
		}

		// Token: 0x04002A6F RID: 10863
		public MEvent Event;

		// Token: 0x04002A70 RID: 10864
		[HideInInspector]
		public bool useInt;

		// Token: 0x04002A71 RID: 10865
		[HideInInspector]
		public bool useFloat;

		// Token: 0x04002A72 RID: 10866
		[HideInInspector]
		public bool useVoid = true;

		// Token: 0x04002A73 RID: 10867
		[HideInInspector]
		public bool useString;

		// Token: 0x04002A74 RID: 10868
		[HideInInspector]
		public bool useBool;

		// Token: 0x04002A75 RID: 10869
		[HideInInspector]
		public bool useGO;

		// Token: 0x04002A76 RID: 10870
		[HideInInspector]
		public bool useTransform;

		// Token: 0x04002A77 RID: 10871
		[HideInInspector]
		public bool useVector;

		// Token: 0x04002A78 RID: 10872
		public UnityEvent Response = new UnityEvent();

		// Token: 0x04002A79 RID: 10873
		public FloatEvent ResponseFloat = new FloatEvent();

		// Token: 0x04002A7A RID: 10874
		public IntEvent ResponseInt = new IntEvent();

		// Token: 0x04002A7B RID: 10875
		public BoolEvent ResponseBool = new BoolEvent();

		// Token: 0x04002A7C RID: 10876
		public StringEvent ResponseString = new StringEvent();

		// Token: 0x04002A7D RID: 10877
		public GameObjectEvent ResponseGO = new GameObjectEvent();

		// Token: 0x04002A7E RID: 10878
		public TransformEvent ResponseTransform = new TransformEvent();

		// Token: 0x04002A7F RID: 10879
		public Vector3Event ResponseVector = new Vector3Event();
	}
}
