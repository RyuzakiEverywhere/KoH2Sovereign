using System;

// Token: 0x020000A9 RID: 169
public struct AlphamapPixelData
{
	// Token: 0x06000601 RID: 1537 RVA: 0x000417B3 File Offset: 0x0003F9B3
	public AlphamapPixelData(byte value, int alphamap_index, int channel, int texture_layer_index)
	{
		this.value = value;
		this.alphamap_index = alphamap_index;
		this.channel = channel;
		this.texture_layer_index = texture_layer_index;
	}

	// Token: 0x04000590 RID: 1424
	public byte value;

	// Token: 0x04000591 RID: 1425
	public int alphamap_index;

	// Token: 0x04000592 RID: 1426
	public int channel;

	// Token: 0x04000593 RID: 1427
	public int texture_layer_index;
}
