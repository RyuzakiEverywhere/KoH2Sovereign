using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02000088 RID: 136
[DebuggerTypeProxy(typeof(GrowBufferDebugView<>))]
public struct GrowBuffer<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000524 RID: 1316 RVA: 0x0003AD43 File Offset: 0x00038F43
	public bool IsCreated
	{
		get
		{
			return this.data != null;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000525 RID: 1317 RVA: 0x0003AD52 File Offset: 0x00038F52
	public unsafe T* Items
	{
		get
		{
			if (this.data != null)
			{
				return (T*)this.data->ptr;
			}
			return null;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000526 RID: 1318 RVA: 0x0003AD6C File Offset: 0x00038F6C
	public unsafe int Count
	{
		get
		{
			if (this.data != null)
			{
				return this.data->count;
			}
			return 0;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x06000527 RID: 1319 RVA: 0x0003AD85 File Offset: 0x00038F85
	public unsafe int Capacity
	{
		get
		{
			if (this.data != null)
			{
				return this.data->capacity;
			}
			return 0;
		}
	}

	// Token: 0x17000043 RID: 67
	public unsafe T this[int idx]
	{
		get
		{
			if (this.data == null)
			{
				throw new Exception("Attempt to index a non-created GrowBuffer");
			}
			if (idx < 0 || idx >= this.data->count)
			{
				this.data->panic = 200;
				this.data->panic_locked = this.data->locked;
				throw new Exception("GrowBuffer index out of range");
			}
			return ref this.Items[(IntPtr)idx * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0003AE15 File Offset: 0x00039015
	public unsafe GrowBuffer(Allocator allocator, int initial_capacity = 0)
	{
		this.data = (GrowBufferData*)UnsafeUtility.Malloc((long)UnsafeUtility.SizeOf<GrowBufferData>(), 64, allocator);
		GrowBuffer<T>.InitData(this.data, allocator, initial_capacity);
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0003AE38 File Offset: 0x00039038
	public unsafe static void InitData(GrowBufferData* data, Allocator allocator, int initial_capacity = 0)
	{
		data->ptr = null;
		data->count = 0;
		data->capacity = 0;
		data->allocator = allocator;
		data->locked = 0;
		data->panic = 0;
		data->panic_locked = 0;
		if (initial_capacity > 0)
		{
			GrowBuffer<T>.Realloc(data, initial_capacity);
		}
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0003AE77 File Offset: 0x00039077
	public unsafe GrowBuffer(IntPtr data)
	{
		this.data = (GrowBufferData*)((void*)data);
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0003AE85 File Offset: 0x00039085
	public unsafe GrowBuffer(void* data)
	{
		this.data = (GrowBufferData*)data;
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0003AE8E File Offset: 0x0003908E
	public unsafe void Clear()
	{
		this.data->count = 0;
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0003AE9C File Offset: 0x0003909C
	public void Add(T item)
	{
		this.Add(ref item);
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0003AEA8 File Offset: 0x000390A8
	private unsafe static bool Wait(ref long counter, int operaion, GrowBufferData* data)
	{
		if (data->panic != 0)
		{
			return false;
		}
		long num = counter + 1L;
		counter = num;
		if (num > 1000000000L)
		{
			data->panic = operaion;
			data->panic_locked = data->locked;
			return false;
		}
		return true;
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0003AEE7 File Offset: 0x000390E7
	public unsafe static void AddNonConcurrent(GrowBufferData* data, T item)
	{
		GrowBuffer<T>.AddNonConcurrent(data, ref item);
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0003AEF4 File Offset: 0x000390F4
	public unsafe static void AddNonConcurrent(GrowBufferData* data, ref T item)
	{
		if (data == null)
		{
			throw new Exception("Attempting to add to non-created GrowBuffer");
		}
		int count = data->count;
		data->count = count + 1;
		int num = count;
		if (num >= data->capacity)
		{
			int num2 = data->capacity;
			if (num2 < 4)
			{
				num2 = 4;
			}
			num2 <<= 2;
			GrowBuffer<T>.Realloc(data, num2);
		}
		*(T*)((byte*)data->ptr + (IntPtr)num * (IntPtr)sizeof(T)) = item;
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0003AF5C File Offset: 0x0003915C
	public unsafe void Add(ref T item)
	{
		if (this.data == null)
		{
			throw new Exception("Attempting to add to non-created GrowBuffer");
		}
		if (this.data->panic != 0)
		{
			return;
		}
		int i = Interlocked.Increment(ref this.data->count) - 1;
		while (i >= Volatile.Read(ref this.data->capacity))
		{
			if (Interlocked.Add(ref this.data->locked, 65536) < 131072)
			{
				long num = 0L;
				while ((Volatile.Read(ref this.data->locked) & 65535) != 0)
				{
					if (!GrowBuffer<T>.Wait(ref num, 1, this.data))
					{
						return;
					}
				}
				int num2 = Volatile.Read(ref this.data->capacity);
				if (num2 < 4)
				{
					num2 = 4;
				}
				num2 <<= 2;
				GrowBuffer<T>.Realloc(this.data, num2);
				Interlocked.Add(ref this.data->locked, -65536);
			}
			else
			{
				Interlocked.Add(ref this.data->locked, -65536);
				long num3 = 0L;
				while (Volatile.Read(ref this.data->locked) >= 65536)
				{
					if (!GrowBuffer<T>.Wait(ref num3, 2, this.data))
					{
						return;
					}
				}
			}
		}
		while (Interlocked.Increment(ref this.data->locked) >= 65536)
		{
			Interlocked.Decrement(ref this.data->locked);
			long num4 = 0L;
			while (Volatile.Read(ref this.data->locked) >= 65536)
			{
				if (!GrowBuffer<T>.Wait(ref num4, 3, this.data))
				{
					return;
				}
			}
		}
		*(T*)((byte*)this.data->ptr + (IntPtr)i * (IntPtr)sizeof(T)) = item;
		Interlocked.Decrement(ref this.data->locked);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0003B112 File Offset: 0x00039312
	public unsafe IntPtr ToIntPtr()
	{
		return (IntPtr)((void*)this.data);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0003B11F File Offset: 0x0003931F
	public unsafe NativeArray<T> ToNativeArray()
	{
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(this.data->ptr, this.data->count, Allocator.None);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0003B140 File Offset: 0x00039340
	[BurstDiscard]
	public unsafe T[] ToArray()
	{
		int count = this.Count;
		T[] array = new T[count];
		if (count <= 0 || this.data->ptr == null)
		{
			return array;
		}
		T[] array2;
		T* destination;
		if ((array2 = array) == null || array2.Length == 0)
		{
			destination = null;
		}
		else
		{
			destination = &array2[0];
		}
		UnsafeUtility.MemCpy((void*)destination, this.data->ptr, (long)(count * UnsafeUtility.SizeOf<T>()));
		array2 = null;
		return array;
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0003B1A4 File Offset: 0x000393A4
	private unsafe static void Realloc(GrowBufferData* data, int new_capacity)
	{
		if (data->allocator <= Allocator.None)
		{
			throw new Exception("Attempting to realloc disposed GrowBuffer");
		}
		int num = UnsafeUtility.SizeOf<T>();
		T* ptr = (T*)UnsafeUtility.Malloc((long)(new_capacity * num), 8, data->allocator);
		if (data->ptr != null)
		{
			UnsafeUtility.MemCpy((void*)ptr, data->ptr, (long)(data->capacity * num));
			UnsafeUtility.Free(data->ptr, data->allocator);
		}
		data->ptr = (void*)ptr;
		data->capacity = new_capacity;
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0003B21A File Offset: 0x0003941A
	public void Dispose()
	{
		GrowBuffer<T>.Dispose(this.data);
		this.data = null;
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0003B230 File Offset: 0x00039430
	public unsafe static void Dispose(GrowBufferData* data)
	{
		if (data == null)
		{
			return;
		}
		Allocator allocator = data->allocator;
		data->Dispose();
		UnsafeUtility.Free((void*)data, allocator);
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0003B258 File Offset: 0x00039458
	[BurstDiscard]
	public unsafe override string ToString()
	{
		string text = "GrowBuffer<" + typeof(T).Name + ">";
		if (this.data == null)
		{
			return text + "(null)";
		}
		return string.Format("{0}({1}/{2})\nLocked: {3:X}{4}{5}", new object[]
		{
			text,
			this.data->count,
			this.data->capacity,
			this.data->locked,
			this.PanicText("\n", "\n"),
			this.StatsText("\n", "\n")
		});
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0003B310 File Offset: 0x00039510
	[BurstDiscard]
	public unsafe string PanicText(string prefix = "", string delimiter = "\n")
	{
		if (this.data->panic == 0)
		{
			return "";
		}
		return string.Format("{0}Panic reason: {1}{2}Panic locked: {3:X}", new object[]
		{
			prefix,
			this.data->panic,
			delimiter,
			this.data->panic_locked
		});
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0003B36E File Offset: 0x0003956E
	[BurstDiscard]
	public string StatsText(string prefix = "", string delimiter = "\n")
	{
		return "";
	}

	// Token: 0x040004DD RID: 1245
	[NativeDisableUnsafePtrRestriction]
	public unsafe GrowBufferData* data;

	// Token: 0x040004DE RID: 1246
	private const int REALLOC_LOCK = 65536;
}
