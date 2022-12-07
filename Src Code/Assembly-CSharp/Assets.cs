using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Logic;
using UnityEngine;

// Token: 0x02000068 RID: 104
[ExecuteInEditMode]
public class Assets : MonoBehaviour
{
	// Token: 0x06000297 RID: 663 RVA: 0x00024BF8 File Offset: 0x00022DF8
	public static Assets.DirInfo ResolveRelativePath(Assets.DirInfo dir, ref string rel_path, bool create = false)
	{
		int num = 0;
		for (;;)
		{
			int num2 = rel_path.IndexOf('/', num);
			if (num2 < 0)
			{
				goto IL_4E;
			}
			string text = rel_path.Substring(num, num2 - num);
			Assets.DirInfo subDir;
			if (dir == null)
			{
				if (text != "assets")
				{
					break;
				}
				subDir = Assets.root;
			}
			else
			{
				subDir = dir.GetSubDir(text, create);
			}
			if (subDir == null)
			{
				goto Block_4;
			}
			dir = subDir;
			num = num2 + 1;
		}
		return null;
		Block_4:
		return null;
		IL_4E:
		if (num > 0)
		{
			rel_path = rel_path.Substring(num);
		}
		return dir;
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00024C64 File Offset: 0x00022E64
	public static Assets.DirInfo GetDir(string path, bool create = false)
	{
		path = path.ToLowerInvariant();
		Assets.DirInfo dirInfo = Assets.ResolveRelativePath(null, ref path, create);
		if (dirInfo == null)
		{
			return null;
		}
		return dirInfo.GetSubDir(path, create);
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00024C94 File Offset: 0x00022E94
	public static Assets.AssetInfo GetAsset(string path, bool create = false)
	{
		path = path.ToLowerInvariant();
		Assets.DirInfo dirInfo = Assets.ResolveRelativePath(null, ref path, create);
		if (dirInfo == null)
		{
			return null;
		}
		return dirInfo.GetAsset(path, create);
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00024CC0 File Offset: 0x00022EC0
	public static UnityEngine.Object GetObject(string path, string subasset_name = null, int subasset_index = 1)
	{
		UnityEngine.Object result;
		using (Game.Profile("Assets.GetObject", false, 0f, null))
		{
			Assets.AssetInfo asset = Assets.GetAsset(path, false);
			if (asset == null)
			{
				result = null;
			}
			else if (subasset_name == null)
			{
				result = asset.GetAsset();
			}
			else
			{
				UnityEngine.Object[] assets = asset.GetAssets();
				if (assets == null)
				{
					UnityEngine.Object asset2 = asset.GetAsset();
					if (asset2 != null && (subasset_index < 1 || (asset2.name != null && asset2.name.Equals(subasset_name, StringComparison.OrdinalIgnoreCase))))
					{
						result = asset2;
					}
					else
					{
						result = null;
					}
				}
				else
				{
					UnityEngine.Object asset3 = asset.GetAsset();
					UnityEngine.Object @object = null;
					foreach (UnityEngine.Object object2 in assets)
					{
						if (object2.name.Equals(subasset_name, StringComparison.OrdinalIgnoreCase) && !(object2 == asset3))
						{
							@object = object2;
							if (--subasset_index <= 0)
							{
								return object2;
							}
						}
					}
					if (@object != null)
					{
						result = @object;
					}
					else if (asset3 != null && asset3.name.Equals(subasset_name, StringComparison.OrdinalIgnoreCase))
					{
						result = asset3;
					}
					else
					{
						result = null;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00024DF0 File Offset: 0x00022FF0
	public static T Get<T>(string path) where T : UnityEngine.Object
	{
		return Assets.GetObject(path, null, 1) as T;
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00024E04 File Offset: 0x00023004
	public static void LoadManifest()
	{
		try
		{
			Assets.SetStatus("Title.loading_progress.manifest");
			string[] array = File.ReadAllLines("AssetBundles/manifest.txt");
			Assets.BundleInfo bundleInfo = null;
			foreach (string text in array)
			{
				int num = text.IndexOf('|');
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
				text = text.TrimEnd(Array.Empty<char>());
				if (text.Length > 0)
				{
					if (text[0] != ' ')
					{
						bundleInfo = new Assets.BundleInfo(text);
						Assets.bundles.Add(bundleInfo);
						if (bundleInfo.name != "resources" && bundleInfo.name != "battleview")
						{
							Assets.total_assets++;
						}
					}
					else if (bundleInfo != null)
					{
						string text2 = text.TrimStart(Array.Empty<char>());
						if (text2.EndsWith(".unity", StringComparison.Ordinal))
						{
							if (!bundleInfo.name.StartsWith("maps_", StringComparison.Ordinal))
							{
								goto IL_1C8;
							}
						}
						else if (bundleInfo.name != "resources" && bundleInfo.name != "battleview")
						{
							Assets.total_assets++;
						}
						Assets.AssetInfo asset = Assets.GetAsset(text2, true);
						if (asset == null)
						{
							Debug.LogError(string.Concat(new string[]
							{
								"Invalid asset path: '",
								text2,
								"' in bundle '",
								bundleInfo.name,
								"'"
							}));
						}
						else if (asset.bundle != null)
						{
							Debug.LogError(string.Concat(new string[]
							{
								"Asset '",
								text2,
								"' is listed in bundles '",
								bundleInfo.name,
								"' and '",
								asset.bundle.name,
								"'"
							}));
						}
						else
						{
							asset.bundle = bundleInfo;
							bundleInfo.assets.Add(asset);
						}
					}
				}
				IL_1C8:;
			}
		}
		catch (Exception arg)
		{
			Debug.LogError("Error loading bundles manifest: " + arg);
		}
	}

	// Token: 0x0600029D RID: 669 RVA: 0x000023FD File Offset: 0x000005FD
	public static void LoadResources()
	{
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0002501C File Offset: 0x0002321C
	public static bool IsLoadingWholeBundles()
	{
		return Assets.load_whole_bundles && Assets.load_asset_request != null && Assets.cur_loading_bundle >= 0;
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00025039 File Offset: 0x00023239
	private static void SetStatus(string s)
	{
		Assets.status = s;
		if (Assets.status_changed != null)
		{
			Assets.status_changed(s);
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x00025053 File Offset: 0x00023253
	public static void BeginLoadingBundles()
	{
		Assets.cur_loading_bundle = -1;
		Assets.cur_loading_asset = -1;
		Assets.LoadResources();
		if (Assets.instance != null)
		{
			Assets.instance.enabled = true;
		}
		Assets.StartTimer("Load bundles");
		Assets.BeginLoadingNextBundle();
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00025090 File Offset: 0x00023290
	private static void BeginLoadingNextBundle()
	{
		Assets.BundleInfo bundleInfo;
		for (;;)
		{
			Assets.cur_loading_bundle++;
			if (Assets.cur_loading_bundle >= Assets.bundles.Count)
			{
				break;
			}
			bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
			if (bundleInfo.name != "resources")
			{
				goto Block_1;
			}
		}
		Assets.cur_loading_bundle = -1;
		Assets.StopTimer();
		Assets.OnAllBundlesLoaded();
		return;
		Block_1:
		Assets.SetStatus("Title.loading_progress." + bundleInfo.name);
		string path = "AssetBundles/" + bundleInfo.name;
		if (Assets.async_load_bundles)
		{
			Assets.load_bundle_request = AssetBundle.LoadFromFileAsync(path);
			Assets.load_bundle_request.completed += Assets.OnBundleLoaded;
			return;
		}
		bundleInfo.bundle = AssetBundle.LoadFromFile(path);
		Assets.AsyncCompleted(new Action<AsyncOperation>(Assets.OnBundleLoaded));
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0002515C File Offset: 0x0002335C
	private static void OnBundleLoaded(AsyncOperation obj)
	{
		Assets.BundleInfo bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
		Assets.num_loaded_assets++;
		Assets.CalcProgress();
		if (Assets.load_bundle_request != null)
		{
			bundleInfo.bundle = Assets.load_bundle_request.assetBundle;
			Assets.load_bundle_request = null;
		}
		if (bundleInfo.bundle == null)
		{
			Debug.LogError("Failed to load asset bundle " + bundleInfo.name);
		}
		Assets.BeginLoadingNextBundle();
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x000251CF File Offset: 0x000233CF
	private static void OnAllBundlesLoaded()
	{
		Assets.SetStatus("Title.loading_progress.bundles_loaded");
		Assets.StartTimer("Load assets");
		Assets.BeginLoadingAssets();
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x000251EA File Offset: 0x000233EA
	private static void BeginLoadingAssets()
	{
		Assets.cur_loading_bundle = -1;
		Assets.BeginLoadingNextAssets();
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x000251F8 File Offset: 0x000233F8
	private static void BeginLoadingNextAssets()
	{
		Assets.BundleInfo bundleInfo;
		for (;;)
		{
			Assets.cur_loading_bundle++;
			if (Assets.cur_loading_bundle >= Assets.bundles.Count)
			{
				break;
			}
			bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
			if (!(bundleInfo.bundle == null) && bundleInfo.assets.Count > 0 && !bundleInfo.name.StartsWith("maps_", StringComparison.Ordinal) && !(bundleInfo.name == "battleview"))
			{
				goto Block_4;
			}
		}
		Assets.cur_loading_bundle = -1;
		Assets.cur_loading_asset = -1;
		Assets.StopTimer();
		Assets.OnAllAssetsLoaded();
		return;
		Block_4:
		Assets.SetStatus("Title.loading_progress." + bundleInfo.name);
		Assets.cur_loading_asset = -1;
		if (Assets.load_whole_bundles)
		{
			Assets.load_asset_request = bundleInfo.bundle.LoadAllAssetsAsync();
			Assets.load_asset_request.completed += Assets.OnAllAssetsInBundleLoaded;
			return;
		}
		if (Assets.use_paraller_async_loading_assets)
		{
			Assets.BeginLoadingAssetParallel();
			return;
		}
		Assets.BeginLoadingNextAsset();
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x000252E8 File Offset: 0x000234E8
	private static void OnAllAssetsInBundleLoaded(AsyncOperation obj)
	{
		Assets.BundleInfo bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
		Assets.num_loaded_assets += bundleInfo.assets.Count;
		Assets.load_asset_request = null;
		Assets.CalcProgress();
		Assets.BeginLoadingNextAssets();
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0002532C File Offset: 0x0002352C
	private static void BeginLoadingAssetParallel()
	{
		if (Assets.cur_loading_bundle >= Assets.bundles.Count)
		{
			return;
		}
		Assets.BundleInfo bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
		if (Assets.cur_loading_asset >= bundleInfo.assets.Count)
		{
			if (Assets.activeJobs <= 0)
			{
				Assets.BeginLoadingNextAssets();
			}
			return;
		}
		while (Assets.activeJobs < Assets.max_async_loading_assets && Assets.cur_loading_asset < bundleInfo.assets.Count)
		{
			Assets.cur_loading_asset++;
			if (Assets.cur_loading_asset >= bundleInfo.assets.Count)
			{
				break;
			}
			Assets.activeJobs++;
			Assets.AssetInfo assetInfo = bundleInfo.assets[Assets.cur_loading_asset];
			new Assets.AssetLoaderJob(bundleInfo, assetInfo, new Action<Assets.AssetLoaderJob>(Assets.OnAssetLoadJobDone));
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x000253E8 File Offset: 0x000235E8
	private static void OnAssetLoadJobDone(Assets.AssetLoaderJob job)
	{
		Assets.activeJobs--;
		Assets.num_loaded_assets++;
		Assets.CalcProgress();
		if (Assets.activeJobs <= Assets.min_async_loading_assets)
		{
			Assets.BeginLoadingAssetParallel();
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00025418 File Offset: 0x00023618
	private static void BeginLoadingNextAsset()
	{
		Assets.BundleInfo bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
		Assets.cur_loading_asset++;
		if (Assets.cur_loading_asset >= bundleInfo.assets.Count)
		{
			Assets.BeginLoadingNextAssets();
			return;
		}
		Assets.AssetInfo assetInfo = bundleInfo.assets[Assets.cur_loading_asset];
		if (Assets.async_load_assets)
		{
			Assets.load_asset_request = bundleInfo.bundle.LoadAssetWithSubAssetsAsync(assetInfo.path);
			Assets.load_asset_request.completed += Assets.OnAssetLoaded;
			return;
		}
		assetInfo.SetAsset(bundleInfo.bundle.LoadAsset(assetInfo.path));
		assetInfo.SetAssets(bundleInfo.bundle.LoadAssetWithSubAssets(assetInfo.path));
		Assets.AsyncCompleted(new Action<AsyncOperation>(Assets.OnAssetLoaded));
	}

	// Token: 0x060002AA RID: 682 RVA: 0x000254E0 File Offset: 0x000236E0
	private static void OnAssetLoaded(AsyncOperation obj)
	{
		Assets.BundleInfo bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
		Assets.AssetInfo assetInfo = bundleInfo.assets[Assets.cur_loading_asset];
		Assets.num_loaded_assets++;
		Assets.CalcProgress();
		if (Assets.load_asset_request != null)
		{
			if (Assets.load_asset_request.asset == null)
			{
				Debug.LogError("Failed to load '" + assetInfo.path + "' from bundle " + bundleInfo.name);
			}
			assetInfo.SetAsset(Assets.load_asset_request.asset);
			assetInfo.SetAssets(Assets.load_asset_request.allAssets);
			Assets.load_asset_request = null;
		}
		Assets.BeginLoadingNextAsset();
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00025583 File Offset: 0x00023783
	private static void OnAllAssetsLoaded()
	{
		Assets.available = true;
		Assets.DestroyInstance();
		Assets.CalcProgress();
		Assets.SetStatus("Done");
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0002559F File Offset: 0x0002379F
	private static void StartTimer(string operation)
	{
		Assets.timer_op = operation;
		if (Assets.timer == null)
		{
			Assets.timer = Stopwatch.StartNew();
			return;
		}
		Assets.timer.Restart();
	}

	// Token: 0x060002AD RID: 685 RVA: 0x000255C4 File Offset: 0x000237C4
	private static void StopTimer()
	{
		if (Assets.timer_op == null)
		{
			return;
		}
		float num = (float)Assets.timer.ElapsedMilliseconds / 1000f;
		Debug.Log(string.Format("{0}: {1}s", Assets.timer_op, num));
		Assets.timer_op = null;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x0002560C File Offset: 0x0002380C
	public static void CalcProgress()
	{
		if (Assets.total_assets <= 0)
		{
			Assets.progress = 0f;
		}
		else
		{
			int num = Assets.num_loaded_assets;
			if (Assets.IsLoadingWholeBundles())
			{
				Assets.BundleInfo bundleInfo = Assets.bundles[Assets.cur_loading_bundle];
				num += (int)((float)bundleInfo.assets.Count * Assets.load_asset_request.progress);
			}
			Assets.progress = (float)num / (float)Assets.total_assets;
		}
		if (Assets.progress_changed != null)
		{
			Assets.progress_changed(Assets.progress);
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00025689 File Offset: 0x00023889
	private static void CreateInstance()
	{
		if (Assets.instance != null)
		{
			return;
		}
		Assets.instance = new GameObject("Assets").AddComponent<Assets>();
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x000256AD File Offset: 0x000238AD
	private static void DestroyInstance()
	{
		if (Assets.instance == null)
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(Assets.instance.gameObject);
		Assets.instance = null;
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x000256D2 File Offset: 0x000238D2
	private static void AsyncCompleted(Action<AsyncOperation> handler)
	{
		Assets.async_completed = handler;
		Assets.CreateInstance();
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x000256E0 File Offset: 0x000238E0
	public static bool Init(bool force_reset = false)
	{
		if (force_reset || !(Assets.status != "Off"))
		{
			AssetBundle.UnloadAllAssetBundles(true);
			Assets.bundles = new List<Assets.BundleInfo>();
			Assets.root = new Assets.DirInfo();
			Assets.root.path = (Assets.root.name = "assets");
			Assets.num_loaded_assets = (Assets.total_assets = 0);
			Assets.available = false;
			Assets.LoadManifest();
			Assets.BeginLoadingBundles();
			return Assets.status == "Done";
		}
		if (Assets.status == "Done")
		{
			Assets.OnAllAssetsLoaded();
			return true;
		}
		return false;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0002577C File Offset: 0x0002397C
	public static void Shutdown()
	{
		Assets.status = "Off";
		Assets.progress = 0f;
		Assets.num_loaded_assets = (Assets.total_assets = 0);
		Assets.DestroyInstance();
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x000257A3 File Offset: 0x000239A3
	private void OnEnable()
	{
		if (Assets.instance == null)
		{
			Assets.instance = this;
		}
		base.gameObject.hideFlags = HideFlags.DontSave;
		if (Application.isPlaying)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x000257D7 File Offset: 0x000239D7
	private void Update()
	{
		if (Assets.async_completed != null)
		{
			Action<AsyncOperation> action = Assets.async_completed;
			Assets.async_completed = null;
			action(null);
		}
	}

	// Token: 0x040003C4 RID: 964
	public const string bundles_dir = "AssetBundles";

	// Token: 0x040003C5 RID: 965
	public const string scenes_prefix = "maps_";

	// Token: 0x040003C6 RID: 966
	public const string manifest_name = "manifest.txt";

	// Token: 0x040003C7 RID: 967
	public static bool async_load_bundles = true;

	// Token: 0x040003C8 RID: 968
	public static bool async_load_assets = true;

	// Token: 0x040003C9 RID: 969
	public static bool load_whole_bundles = true;

	// Token: 0x040003CA RID: 970
	public static bool use_paraller_async_loading_assets = true;

	// Token: 0x040003CB RID: 971
	public static int min_async_loading_assets = 20;

	// Token: 0x040003CC RID: 972
	public static int max_async_loading_assets = 30;

	// Token: 0x040003CD RID: 973
	public static bool available = false;

	// Token: 0x040003CE RID: 974
	public static string status = "Off";

	// Token: 0x040003CF RID: 975
	public static Assets.StatusChanged status_changed = null;

	// Token: 0x040003D0 RID: 976
	private static Stopwatch timer = null;

	// Token: 0x040003D1 RID: 977
	private static string timer_op = null;

	// Token: 0x040003D2 RID: 978
	public static float progress = 0f;

	// Token: 0x040003D3 RID: 979
	public static Assets.ProgressChanged progress_changed = null;

	// Token: 0x040003D4 RID: 980
	private static List<Assets.BundleInfo> bundles = null;

	// Token: 0x040003D5 RID: 981
	private static Assets.DirInfo root = null;

	// Token: 0x040003D6 RID: 982
	private static int num_loaded_assets = 0;

	// Token: 0x040003D7 RID: 983
	private static int total_assets = 0;

	// Token: 0x040003D8 RID: 984
	private static int cur_loading_bundle = -1;

	// Token: 0x040003D9 RID: 985
	private static AssetBundleCreateRequest load_bundle_request = null;

	// Token: 0x040003DA RID: 986
	private static int cur_loading_asset = -1;

	// Token: 0x040003DB RID: 987
	private static AssetBundleRequest load_asset_request = null;

	// Token: 0x040003DC RID: 988
	private static Action<AsyncOperation> async_completed = null;

	// Token: 0x040003DD RID: 989
	private static int activeJobs = 0;

	// Token: 0x040003DE RID: 990
	private static Assets instance = null;

	// Token: 0x02000520 RID: 1312
	// (Invoke) Token: 0x060042CA RID: 17098
	public delegate void StatusChanged(string status);

	// Token: 0x02000521 RID: 1313
	// (Invoke) Token: 0x060042CE RID: 17102
	public delegate void ProgressChanged(float progress);

	// Token: 0x02000522 RID: 1314
	public class BundleInfo
	{
		// Token: 0x060042D1 RID: 17105 RVA: 0x001F9E5B File Offset: 0x001F805B
		public BundleInfo(string name)
		{
			this.name = name;
		}

		// Token: 0x060042D2 RID: 17106 RVA: 0x001F9E75 File Offset: 0x001F8075
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x04002F06 RID: 12038
		public string name;

		// Token: 0x04002F07 RID: 12039
		public List<Assets.AssetInfo> assets = new List<Assets.AssetInfo>();

		// Token: 0x04002F08 RID: 12040
		public AssetBundle bundle;
	}

	// Token: 0x02000523 RID: 1315
	public class AssetInfo
	{
		// Token: 0x060042D3 RID: 17107 RVA: 0x001F9E7D File Offset: 0x001F807D
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x060042D4 RID: 17108 RVA: 0x001F9E88 File Offset: 0x001F8088
		private void Load()
		{
			if (this.loaded)
			{
				return;
			}
			this.loaded = true;
			Assets.BundleInfo bundleInfo = this.bundle;
			if (((bundleInfo != null) ? bundleInfo.bundle : null) == null)
			{
				string str = "Unable to load asset from bundle '";
				Assets.BundleInfo bundleInfo2 = this.bundle;
				Debug.LogWarning(str + ((bundleInfo2 != null) ? bundleInfo2.name : null) + "': " + this.path);
				return;
			}
			this.all_assets = this.bundle.bundle.LoadAssetWithSubAssets(this.path);
			this.asset = this.bundle.bundle.LoadAsset(this.path);
			if (this.all_assets != null && this.all_assets.Length == 0)
			{
				this.all_assets = null;
			}
		}

		// Token: 0x060042D5 RID: 17109 RVA: 0x001F9F3C File Offset: 0x001F813C
		public void SetAsset(UnityEngine.Object obj)
		{
			this.loaded = true;
			this.asset = obj;
		}

		// Token: 0x060042D6 RID: 17110 RVA: 0x001F9F4C File Offset: 0x001F814C
		public void SetAssets(UnityEngine.Object[] obj)
		{
			this.loaded = true;
			this.all_assets = obj;
			if (this.all_assets != null && this.all_assets.Length == 0)
			{
				this.all_assets = null;
			}
		}

		// Token: 0x060042D7 RID: 17111 RVA: 0x001F9F74 File Offset: 0x001F8174
		public UnityEngine.Object GetAsset()
		{
			this.Load();
			return this.asset;
		}

		// Token: 0x060042D8 RID: 17112 RVA: 0x001F9F82 File Offset: 0x001F8182
		public UnityEngine.Object[] GetAssets()
		{
			this.Load();
			return this.all_assets;
		}

		// Token: 0x04002F09 RID: 12041
		public string name = "";

		// Token: 0x04002F0A RID: 12042
		public string path = "";

		// Token: 0x04002F0B RID: 12043
		public Assets.DirInfo dir;

		// Token: 0x04002F0C RID: 12044
		private UnityEngine.Object asset;

		// Token: 0x04002F0D RID: 12045
		private UnityEngine.Object[] all_assets;

		// Token: 0x04002F0E RID: 12046
		public Assets.BundleInfo bundle;

		// Token: 0x04002F0F RID: 12047
		private bool loaded;
	}

	// Token: 0x02000524 RID: 1316
	public class DirInfo
	{
		// Token: 0x060042DA RID: 17114 RVA: 0x001F9FAE File Offset: 0x001F81AE
		public override string ToString()
		{
			return this.path;
		}

		// Token: 0x060042DB RID: 17115 RVA: 0x001F9FB8 File Offset: 0x001F81B8
		public void Unload()
		{
			if (this.assets == null)
			{
				return;
			}
			for (int i = 0; i < this.assets.Count; i++)
			{
				UnityEngine.Object[] array = this.assets[i].GetAssets();
				if (array != null)
				{
					foreach (UnityEngine.Object @object in array)
					{
						if (!(@object is GameObject) && !(@object is UnityEngine.Component))
						{
							Resources.UnloadAsset(@object);
						}
					}
				}
			}
			this.assets = null;
		}

		// Token: 0x060042DC RID: 17116 RVA: 0x001FA028 File Offset: 0x001F8228
		public List<Assets.DirInfo> GetSubdirs()
		{
			if (this.subdirs != null)
			{
				return this.subdirs;
			}
			this.subdirs = new List<Assets.DirInfo>();
			return this.subdirs;
		}

		// Token: 0x060042DD RID: 17117 RVA: 0x001FA04A File Offset: 0x001F824A
		public List<Assets.AssetInfo> GetAssets()
		{
			if (this.assets != null)
			{
				return this.assets;
			}
			this.assets = new List<Assets.AssetInfo>();
			return this.assets;
		}

		// Token: 0x060042DE RID: 17118 RVA: 0x001FA06C File Offset: 0x001F826C
		public Assets.DirInfo GetSubDir(string name, bool create = false)
		{
			this.GetSubdirs();
			Assets.DirInfo dirInfo;
			for (int i = 0; i < this.subdirs.Count; i++)
			{
				dirInfo = this.subdirs[i];
				if (dirInfo.name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return dirInfo;
				}
			}
			if (!create)
			{
				return null;
			}
			dirInfo = new Assets.DirInfo();
			dirInfo.name = name;
			dirInfo.path = this.path + "/" + dirInfo.name;
			dirInfo.parent = this;
			this.subdirs.Add(dirInfo);
			return dirInfo;
		}

		// Token: 0x060042DF RID: 17119 RVA: 0x001FA0F8 File Offset: 0x001F82F8
		public Assets.AssetInfo GetAsset(string name, bool create = false)
		{
			this.GetAssets();
			Assets.AssetInfo assetInfo;
			for (int i = 0; i < this.assets.Count; i++)
			{
				assetInfo = this.assets[i];
				if (assetInfo.name == name)
				{
					return assetInfo;
				}
			}
			if (!create)
			{
				return null;
			}
			assetInfo = new Assets.AssetInfo();
			assetInfo.name = name;
			assetInfo.path = this.path + "/" + assetInfo.name;
			assetInfo.dir = this;
			this.assets.Add(assetInfo);
			return assetInfo;
		}

		// Token: 0x04002F10 RID: 12048
		public string name = "";

		// Token: 0x04002F11 RID: 12049
		public string path = "";

		// Token: 0x04002F12 RID: 12050
		public Assets.DirInfo parent;

		// Token: 0x04002F13 RID: 12051
		private List<Assets.DirInfo> subdirs;

		// Token: 0x04002F14 RID: 12052
		private List<Assets.AssetInfo> assets;
	}

	// Token: 0x02000525 RID: 1317
	private class AssetLoaderJob
	{
		// Token: 0x060042E1 RID: 17121 RVA: 0x001FA1A0 File Offset: 0x001F83A0
		public AssetLoaderJob(Assets.BundleInfo bundleInfo, Assets.AssetInfo assetInfo, Action<Assets.AssetLoaderJob> onCompleteCallback)
		{
			if (bundleInfo == null)
			{
				return;
			}
			if (bundleInfo.bundle == null)
			{
				return;
			}
			if (assetInfo == null)
			{
				return;
			}
			this.bundleInfo = bundleInfo;
			this.assetInfo = assetInfo;
			this.onCompleteCallback = onCompleteCallback;
			this.request = bundleInfo.bundle.LoadAssetWithSubAssetsAsync(assetInfo.path);
			this.request.completed += this.OnAssetLoaded;
		}

		// Token: 0x060042E2 RID: 17122 RVA: 0x001FA210 File Offset: 0x001F8410
		private void OnAssetLoaded(AsyncOperation obj)
		{
			Assets.CalcProgress();
			if (this.request != null)
			{
				this.assetInfo.SetAsset(this.request.asset);
				this.assetInfo.SetAssets(this.request.allAssets);
				this.request = null;
			}
			if (this.assetInfo.GetAsset() == null)
			{
				Debug.LogError("Failed to load '" + this.assetInfo.path + "' from bundle " + this.bundleInfo.name);
			}
			this.isCompleted = true;
			Action<Assets.AssetLoaderJob> action = this.onCompleteCallback;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x04002F15 RID: 12053
		private Assets.BundleInfo bundleInfo;

		// Token: 0x04002F16 RID: 12054
		private Assets.AssetInfo assetInfo;

		// Token: 0x04002F17 RID: 12055
		private AssetBundleRequest request;

		// Token: 0x04002F18 RID: 12056
		public bool isCompleted;

		// Token: 0x04002F19 RID: 12057
		public Action<Assets.AssetLoaderJob> onCompleteCallback;
	}
}
