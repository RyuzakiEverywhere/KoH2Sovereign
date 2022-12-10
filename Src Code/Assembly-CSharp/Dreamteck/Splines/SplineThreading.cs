using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Dreamteck.Splines
{
	// Token: 0x020004D0 RID: 1232
	public static class SplineThreading
	{
		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x0600418B RID: 16779 RVA: 0x001F2CBE File Offset: 0x001F0EBE
		// (set) Token: 0x0600418C RID: 16780 RVA: 0x001F2CC8 File Offset: 0x001F0EC8
		public static int threadCount
		{
			get
			{
				return SplineThreading.threads.Length;
			}
			set
			{
				if (value > SplineThreading.threads.Length)
				{
					while (SplineThreading.threads.Length < value)
					{
						SplineThreading.ThreadDef threadDef = new SplineThreading.ThreadDef();
						threadDef.Restart();
						ArrayUtility.Add<SplineThreading.ThreadDef>(ref SplineThreading.threads, threadDef);
					}
				}
			}
		}

		// Token: 0x0600418D RID: 16781 RVA: 0x001F2D04 File Offset: 0x001F0F04
		static SplineThreading()
		{
			for (int i = 0; i < SplineThreading.threads.Length; i++)
			{
				SplineThreading.threads[i] = new SplineThreading.ThreadDef();
			}
		}

		// Token: 0x0600418E RID: 16782 RVA: 0x001F2D44 File Offset: 0x001F0F44
		private static void Quitting()
		{
			SplineThreading.Stop();
		}

		// Token: 0x0600418F RID: 16783 RVA: 0x001F2D4C File Offset: 0x001F0F4C
		private static void RunThread(object o)
		{
			SplineThreading.ThreadDef.Worker worker = (SplineThreading.ThreadDef.Worker)o;
			for (;;)
			{
				try
				{
					worker.computing = false;
					Thread.Sleep(-1);
					continue;
				}
				catch (ThreadInterruptedException)
				{
					worker.computing = true;
					object obj = SplineThreading.locker;
					lock (obj)
					{
						while (worker.instructions.Count > 0)
						{
							SplineThreading.EmptyHandler emptyHandler = worker.instructions.Dequeue();
							if (emptyHandler != null)
							{
								emptyHandler();
							}
						}
					}
					continue;
				}
				catch (Exception ex)
				{
					if (ex.Message != "")
					{
						Debug.Log("THREAD EXCEPTION " + ex.Message);
					}
				}
				break;
			}
			Debug.Log("Thread stopped");
			worker.computing = false;
		}

		// Token: 0x06004190 RID: 16784 RVA: 0x001F2E24 File Offset: 0x001F1024
		public static void Run(SplineThreading.EmptyHandler handler)
		{
			int i = 0;
			while (i < SplineThreading.threads.Length)
			{
				if (!SplineThreading.threads[i].isAlive)
				{
					SplineThreading.threads[i].Restart();
				}
				if (!SplineThreading.threads[i].computing || i == SplineThreading.threads.Length - 1)
				{
					SplineThreading.threads[i].Queue(handler);
					if (!SplineThreading.threads[i].computing)
					{
						SplineThreading.threads[i].Interrupt();
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x06004191 RID: 16785 RVA: 0x001F2EA0 File Offset: 0x001F10A0
		public static void PrewarmThreads()
		{
			for (int i = 0; i < SplineThreading.threads.Length; i++)
			{
				if (!SplineThreading.threads[i].isAlive)
				{
					SplineThreading.threads[i].Restart();
				}
			}
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x001F2EDC File Offset: 0x001F10DC
		public static void Stop()
		{
			for (int i = 0; i < SplineThreading.threads.Length; i++)
			{
				SplineThreading.threads[i].Abort();
			}
		}

		// Token: 0x04002DA2 RID: 11682
		internal static SplineThreading.ThreadDef[] threads = new SplineThreading.ThreadDef[2];

		// Token: 0x04002DA3 RID: 11683
		internal static readonly object locker = new object();

		// Token: 0x020009B2 RID: 2482
		// (Invoke) Token: 0x060054B1 RID: 21681
		public delegate void EmptyHandler();

		// Token: 0x020009B3 RID: 2483
		internal class ThreadDef
		{
			// Token: 0x17000722 RID: 1826
			// (get) Token: 0x060054B4 RID: 21684 RVA: 0x0024732E File Offset: 0x0024552E
			internal bool isAlive
			{
				get
				{
					return this.thread != null && this.thread.IsAlive;
				}
			}

			// Token: 0x17000723 RID: 1827
			// (get) Token: 0x060054B5 RID: 21685 RVA: 0x00247345 File Offset: 0x00245545
			internal bool computing
			{
				get
				{
					return this.worker.computing;
				}
			}

			// Token: 0x060054B6 RID: 21686 RVA: 0x00247352 File Offset: 0x00245552
			internal ThreadDef()
			{
				this.start = new ParameterizedThreadStart(SplineThreading.RunThread);
			}

			// Token: 0x060054B7 RID: 21687 RVA: 0x00247377 File Offset: 0x00245577
			internal void Queue(SplineThreading.EmptyHandler handler)
			{
				this.worker.instructions.Enqueue(handler);
			}

			// Token: 0x060054B8 RID: 21688 RVA: 0x0024738A File Offset: 0x0024558A
			internal void Interrupt()
			{
				this.thread.Interrupt();
			}

			// Token: 0x060054B9 RID: 21689 RVA: 0x00247397 File Offset: 0x00245597
			internal void Restart()
			{
				this.thread = new Thread(this.start);
				this.thread.Start(this.worker);
				Debug.Log("Starting Thread");
			}

			// Token: 0x060054BA RID: 21690 RVA: 0x002473C5 File Offset: 0x002455C5
			internal void Abort()
			{
				if (this.isAlive)
				{
					this.thread.Abort();
				}
				Debug.Log("Stopping Thread");
			}

			// Token: 0x04004526 RID: 17702
			private ParameterizedThreadStart start;

			// Token: 0x04004527 RID: 17703
			internal Thread thread;

			// Token: 0x04004528 RID: 17704
			private SplineThreading.ThreadDef.Worker worker = new SplineThreading.ThreadDef.Worker();

			// Token: 0x02000A4B RID: 2635
			internal class Worker
			{
				// Token: 0x04004745 RID: 18245
				internal bool computing;

				// Token: 0x04004746 RID: 18246
				internal Queue<SplineThreading.EmptyHandler> instructions = new Queue<SplineThreading.EmptyHandler>();
			}

			// Token: 0x02000A4C RID: 2636
			// (Invoke) Token: 0x0600562D RID: 22061
			internal delegate void BoolHandler(bool flag);
		}
	}
}
