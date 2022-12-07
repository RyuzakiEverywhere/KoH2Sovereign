using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x02000058 RID: 88
public static class MemoryAnalyser
{
	// Token: 0x0600020B RID: 523 RVA: 0x0001FB06 File Offset: 0x0001DD06
	private static List<Object> GatherAllocatedObjects()
	{
		List<Object> list = new List<Object>();
		list.Clear();
		list.AddRange(from o in Resources.FindObjectsOfTypeAll<Object>()
		where o != null
		select o);
		return list;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0001FB44 File Offset: 0x0001DD44
	public static void MemorySnapshotToCSVFile(int min_weight, string file_name)
	{
		List<Object> list = MemoryAnalyser.GatherAllocatedObjects();
		list.RemoveAll((Object t) => t == null);
		string path = Path.Combine(Application.dataPath, file_name + ".csv");
		MemoryAnalyser.MemoryObjectDataCollection memoryObjectDataCollection = new MemoryAnalyser.MemoryObjectDataCollection(list);
		memoryObjectDataCollection.data.RemoveAll((MemoryAnalyser.MemoryObjectData o) => o.size < (long)min_weight);
		StringBuilder stringBuilder = new StringBuilder();
		MemoryAnalyser.MemoryObjectData.AppendCSVHeader(stringBuilder);
		foreach (MemoryAnalyser.MemoryObjectData memoryObjectData in memoryObjectDataCollection.data)
		{
			memoryObjectData.AppendCSVRow(stringBuilder);
		}
		File.WriteAllText(path, stringBuilder.ToString());
	}

	// Token: 0x0400031A RID: 794
	private const char SEPARATOR = ',';

	// Token: 0x0200050B RID: 1291
	[Serializable]
	private struct MemoryObjectData
	{
		// Token: 0x06004296 RID: 17046 RVA: 0x001F9358 File Offset: 0x001F7558
		public MemoryObjectData(Object obj)
		{
			this.name = "\"" + obj.name + "\"";
			this.size = Profiler.GetRuntimeMemorySizeLong(obj);
			this.type = obj.GetType().Name;
			this.description = "(no desc)";
			Texture2D texture2D;
			if ((texture2D = (obj as Texture2D)) != null)
			{
				this.description = string.Format("{0}x{1} {2} {3}", new object[]
				{
					texture2D.width,
					texture2D.height,
					texture2D.format,
					texture2D.graphicsFormat
				});
				return;
			}
			Texture texture;
			if ((texture = (obj as Texture)) != null)
			{
				this.description = string.Format("{0}x{1} {2}", texture.width, texture.height, texture.graphicsFormat);
				return;
			}
			Mesh mesh;
			if ((mesh = (obj as Mesh)) != null)
			{
				this.description = string.Format("Vertex count: {0}", mesh.vertexCount);
			}
		}

		// Token: 0x06004297 RID: 17047 RVA: 0x001F9464 File Offset: 0x001F7664
		public void AppendCSVRow(StringBuilder sb)
		{
			sb.AppendLine(string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
			{
				this.name,
				',',
				this.size,
				',',
				this.type,
				',',
				this.description
			}));
		}

		// Token: 0x06004298 RID: 17048 RVA: 0x001F94CF File Offset: 0x001F76CF
		public static void AppendCSVHeader(StringBuilder sb)
		{
			sb.AppendLine(string.Format("name{0}size{1}type{2}description", ',', ',', ','));
		}

		// Token: 0x04002EB8 RID: 11960
		public string name;

		// Token: 0x04002EB9 RID: 11961
		public long size;

		// Token: 0x04002EBA RID: 11962
		public string description;

		// Token: 0x04002EBB RID: 11963
		public string type;
	}

	// Token: 0x0200050C RID: 1292
	[Serializable]
	private struct MemoryObjectDataCollection
	{
		// Token: 0x06004299 RID: 17049 RVA: 0x001F94F7 File Offset: 0x001F76F7
		public MemoryObjectDataCollection(List<Object> objects)
		{
			this.data = (from t in objects
			select new MemoryAnalyser.MemoryObjectData(t)).ToList<MemoryAnalyser.MemoryObjectData>();
		}

		// Token: 0x04002EBC RID: 11964
		public List<MemoryAnalyser.MemoryObjectData> data;
	}
}
