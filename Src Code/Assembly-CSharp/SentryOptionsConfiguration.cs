using System;
using Sentry;
using Sentry.Unity;
using UnityEngine;

// Token: 0x02000063 RID: 99
[CreateAssetMenu(fileName = "Assets/Resources/Sentry/SentryOptionsConfiguration.cs", menuName = "Sentry/SentryOptionsConfiguration", order = 999)]
public class SentryOptionsConfiguration : ScriptableOptionsConfiguration
{
	// Token: 0x0600024F RID: 591 RVA: 0x00021A1C File Offset: 0x0001FC1C
	public override void Configure(SentryUnityOptions options)
	{
		options.BeforeSend = new Func<SentryEvent, SentryEvent>(SentryOptionsConfiguration.BeforeSend);
	}

	// Token: 0x06000250 RID: 592 RVA: 0x00021A30 File Offset: 0x0001FC30
	private static SentryEvent BeforeSend(SentryEvent evt)
	{
		if (evt.Exception == null)
		{
			return null;
		}
		return evt;
	}
}
