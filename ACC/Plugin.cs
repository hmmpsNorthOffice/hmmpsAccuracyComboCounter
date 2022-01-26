using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System.Reflection;
using ACC.Core;
using ACC.ResultsView;
using Zenject;
using IPALogger = IPA.Logging.Logger;
using ACC.Configuration;

namespace ACC
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
#pragma warning disable CS8618
		public static Harmony harmony;
		public static AccManager AccManager { get; set; }

		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }
#pragma warning restore CS8618

		[Init]
		/// <summary>
		/// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
		/// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
		/// Only use [Init] with one Constructor.
		/// </summary>
		public Plugin(Config conf, IPALogger logger, Zenjector zenjector)
		{
			Instance = this;
			Plugin.Log = logger;
			PluginConfig.Instance = conf.Generated<PluginConfig>();

			zenjector.Install(Location.App, (DiContainer Container) =>
			{
				Container.BindInterfacesAndSelfTo<AccManager>().AsSingle();
			});
			zenjector.Install(Location.GameCore, (DiContainer Container) =>
			{
				Container.BindInterfacesAndSelfTo<AccTracker>().AsSingle();
			});
			zenjector.Install(Location.Menu, (DiContainer Container) =>
			{
				Container.BindInterfacesAndSelfTo<AccResultsViewController>().AsSingle();
			});
		}

		[OnEnable]
		public void OnApplicationStart()
		{
			harmony = new Harmony("com.ChirpyMisha.BeatSaber.ACC");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		[OnDisable]
		public void OnApplicationQuit()
		{
			harmony.UnpatchSelf();
		}
	}
}
