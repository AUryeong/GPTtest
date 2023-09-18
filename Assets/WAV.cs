using System.IO;

public class WAV
	{
		// Token: 0x060095AA RID: 38314 RVA: 0x002C22A0 File Offset: 0x002C04A0
		private static float bytesToFloat(byte firstByte, byte secondByte)
		{
			short num = (short)((int)secondByte << 8 | (int)firstByte);
			return (float)num / 32768f;
		}

		// Token: 0x060095AB RID: 38315 RVA: 0x002C22C4 File Offset: 0x002C04C4
		private static int bytesToInt(byte[] bytes, int offset = 0)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num |= (int)bytes[offset + i] << i * 8;
			}
			return num;
		}

		// Token: 0x060095AC RID: 38316 RVA: 0x002C22FC File Offset: 0x002C04FC
		private static byte[] GetBytes(string filename)
		{
			return File.ReadAllBytes(filename);
		}

		// Token: 0x1700105B RID: 4187
		// (get) Token: 0x060095AD RID: 38317 RVA: 0x002C2314 File Offset: 0x002C0514
		// (set) Token: 0x060095AE RID: 38318 RVA: 0x002C231C File Offset: 0x002C051C
		public float[] LeftChannel { get; internal set; }

		// Token: 0x1700105C RID: 4188
		// (get) Token: 0x060095AF RID: 38319 RVA: 0x002C2328 File Offset: 0x002C0528
		// (set) Token: 0x060095B0 RID: 38320 RVA: 0x002C2330 File Offset: 0x002C0530
		public float[] RightChannel { get; internal set; }

		// Token: 0x1700105D RID: 4189
		// (get) Token: 0x060095B1 RID: 38321 RVA: 0x002C233C File Offset: 0x002C053C
		// (set) Token: 0x060095B2 RID: 38322 RVA: 0x002C2344 File Offset: 0x002C0544
		public int ChannelCount { get; internal set; }

		// Token: 0x1700105E RID: 4190
		// (get) Token: 0x060095B3 RID: 38323 RVA: 0x002C2350 File Offset: 0x002C0550
		// (set) Token: 0x060095B4 RID: 38324 RVA: 0x002C2358 File Offset: 0x002C0558
		public int SampleCount { get; internal set; }

		// Token: 0x1700105F RID: 4191
		// (get) Token: 0x060095B5 RID: 38325 RVA: 0x002C2364 File Offset: 0x002C0564
		// (set) Token: 0x060095B6 RID: 38326 RVA: 0x002C236C File Offset: 0x002C056C
		public int Frequency { get; internal set; }

		// Token: 0x060095B7 RID: 38327 RVA: 0x002C2378 File Offset: 0x002C0578
		public WAV(string filename) : this(WAV.GetBytes(filename))
		{
		}

		// Token: 0x060095B8 RID: 38328 RVA: 0x002C2388 File Offset: 0x002C0588
		public WAV(byte[] wav)
		{
			this.ChannelCount = (int)wav[22];
			this.Frequency = WAV.bytesToInt(wav, 24);
			int i = 12;
			while (wav[i] != 100 || wav[i + 1] != 97 || wav[i + 2] != 116 || wav[i + 3] != 97)
			{
				i += 4;
				int num = (int)wav[i] + (int)wav[i + 1] * 256 + (int)wav[i + 2] * 65536 + (int)wav[i + 3] * 16777216;
				i += 4 + num;
			}
			i += 8;
			this.SampleCount = (wav.Length - i) / 2;
			bool flag = this.ChannelCount == 2;
			if (flag)
			{
				this.SampleCount /= 2;
			}
			this.LeftChannel = new float[this.SampleCount];
			bool flag2 = this.ChannelCount == 2;
			if (flag2)
			{
				this.RightChannel = new float[this.SampleCount];
			}
			else
			{
				this.RightChannel = null;
			}
			int num2 = 0;
			while (i < wav.Length)
			{
				this.LeftChannel[num2] = WAV.bytesToFloat(wav[i], wav[i + 1]);
				i += 2;
				bool flag3 = this.ChannelCount == 2;
				if (flag3)
				{
					this.RightChannel[num2] = WAV.bytesToFloat(wav[i], wav[i + 1]);
					i += 2;
				}
				num2++;
			}
		}

		// Token: 0x060095B9 RID: 38329 RVA: 0x002C24E4 File Offset: 0x002C06E4
		public override string ToString()
		{
			return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", new object[]
			{
				this.LeftChannel,
				this.RightChannel,
				this.ChannelCount,
				this.SampleCount,
				this.Frequency
			});
		}
	}