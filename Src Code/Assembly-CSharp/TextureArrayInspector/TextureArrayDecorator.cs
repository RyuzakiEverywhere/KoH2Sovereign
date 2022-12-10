using System;
using UnityEngine;

namespace TextureArrayInspector
{
	// Token: 0x02000484 RID: 1156
	public class TextureArrayDecorator
	{
		// Token: 0x06003CC9 RID: 15561 RVA: 0x001D038E File Offset: 0x001CE58E
		public TextureArrayDecorator(Texture2DArray texArr)
		{
			this.texArr = texArr;
			this.LoadSources();
		}

		// Token: 0x06003CCA RID: 15562 RVA: 0x000023FD File Offset: 0x000005FD
		public void SetSource(Texture2D tex, int ch, bool isAlpha = false, bool saveSources = true)
		{
		}

		// Token: 0x06003CCB RID: 15563 RVA: 0x000023FD File Offset: 0x000005FD
		public void FindAndSetSource(Texture2D tex, string texPath, bool isAlpha = false)
		{
		}

		// Token: 0x06003CCC RID: 15564 RVA: 0x000448AF File Offset: 0x00042AAF
		public Texture2D GetSource(int ch, bool isAlpha = false)
		{
			return null;
		}

		// Token: 0x06003CCD RID: 15565 RVA: 0x000448AF File Offset: 0x00042AAF
		public Texture2D GetPreview(int ch)
		{
			return null;
		}

		// Token: 0x06003CCE RID: 15566 RVA: 0x000023FD File Offset: 0x000005FD
		public void ApplySource(int ch)
		{
		}

		// Token: 0x06003CCF RID: 15567 RVA: 0x000023FD File Offset: 0x000005FD
		public void ApplyAllSources()
		{
		}

		// Token: 0x06003CD0 RID: 15568 RVA: 0x000023FD File Offset: 0x000005FD
		public void Add(Texture2D tex, Texture2D al)
		{
		}

		// Token: 0x06003CD1 RID: 15569 RVA: 0x000023FD File Offset: 0x000005FD
		public void Insert(Texture2D tex, Texture2D al, int index)
		{
		}

		// Token: 0x06003CD2 RID: 15570 RVA: 0x000023FD File Offset: 0x000005FD
		public void Switch(int ch1, int ch2)
		{
		}

		// Token: 0x06003CD3 RID: 15571 RVA: 0x000023FD File Offset: 0x000005FD
		public void RemoveAt(int index)
		{
		}

		// Token: 0x06003CD4 RID: 15572 RVA: 0x001D03A4 File Offset: 0x001CE5A4
		public void Resize(int width, int height)
		{
			Texture2DArray oldArr = this.texArr;
			this.texArr = this.texArr.ResizedClone(width, height);
			this.ApplyAllSources();
			this.Rewrite(oldArr, this.texArr);
		}

		// Token: 0x06003CD5 RID: 15573 RVA: 0x001D03E0 File Offset: 0x001CE5E0
		public void Format(TextureFormat format)
		{
			Texture2DArray oldArr = this.texArr;
			this.texArr = this.texArr.FormattedClone(format);
			this.ApplyAllSources();
			this.Rewrite(oldArr, this.texArr);
		}

		// Token: 0x06003CD6 RID: 15574 RVA: 0x001D0419 File Offset: 0x001CE619
		public void ReadWrite(bool readWrite)
		{
			if (readWrite)
			{
				this.Format(this.texArr.format);
				return;
			}
			this.texArr.Apply(false, true);
		}

		// Token: 0x06003CD7 RID: 15575 RVA: 0x001D0440 File Offset: 0x001CE640
		public void Linear(bool linear)
		{
			Texture2DArray oldArr = this.texArr;
			this.texArr = this.texArr.LinearClone(linear);
			this.ApplyAllSources();
			this.Rewrite(oldArr, this.texArr);
		}

		// Token: 0x06003CD8 RID: 15576 RVA: 0x000023FD File Offset: 0x000005FD
		public void Rewrite(Texture2DArray oldArr, Texture2DArray newArr)
		{
		}

		// Token: 0x06003CD9 RID: 15577 RVA: 0x000023FD File Offset: 0x000005FD
		public void LoadSources()
		{
		}

		// Token: 0x06003CDA RID: 15578 RVA: 0x000023FD File Offset: 0x000005FD
		public void SaveSources()
		{
		}

		// Token: 0x06003CDB RID: 15579 RVA: 0x000023FD File Offset: 0x000005FD
		public static void UnlinkTexture(string texGuid, string arrGuid, bool isAlpha = false)
		{
		}

		// Token: 0x06003CDC RID: 15580 RVA: 0x000023FD File Offset: 0x000005FD
		public static void LinkTexture(string texGuid, string arrGuid, bool isAlpha = false)
		{
		}

		// Token: 0x04002BB7 RID: 11191
		public Texture2DArray texArr;
	}
}
