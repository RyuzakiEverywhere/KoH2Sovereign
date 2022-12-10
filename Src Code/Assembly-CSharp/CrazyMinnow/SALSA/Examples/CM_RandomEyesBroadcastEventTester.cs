using System;
using UnityEngine;

namespace CrazyMinnow.SALSA.Examples
{
	// Token: 0x02000495 RID: 1173
	public class CM_RandomEyesBroadcastEventTester : MonoBehaviour
	{
		// Token: 0x06003DF3 RID: 15859 RVA: 0x001DAB5C File Offset: 0x001D8D5C
		private void RandomEyes_OnLookStatusChanged(RandomEyesLookStatus status)
		{
			Debug.Log(string.Concat(new object[]
			{
				"RandomEyes_OnLookStatusChanged: instance(",
				status.instance.GetType(),
				"), name(",
				status.instance.name,
				"), blendSpeed(",
				status.blendSpeed,
				"), rangeOfMotion(",
				status.rangeOfMotion,
				")"
			}));
		}

		// Token: 0x06003DF4 RID: 15860 RVA: 0x001DABDC File Offset: 0x001D8DDC
		private void RandomEyes_OnCustomShapeChanged(RandomEyesCustomShapeStatus status)
		{
			Debug.Log(string.Concat(new object[]
			{
				"RandomEyes_OnCustomShapeChanged: instance(",
				status.instance.GetType(),
				"), name(",
				status.instance.name,
				"), shapeIndex(",
				status.shapeIndex,
				"), shapeName(",
				status.shapeName,
				"), overrideOn(",
				status.overrideOn.ToString(),
				"), isOn(",
				status.isOn.ToString(),
				"), blendSpeed(",
				status.blendSpeed,
				"), rangeOfMotion(",
				status.rangeOfMotion,
				")"
			}));
		}
	}
}
