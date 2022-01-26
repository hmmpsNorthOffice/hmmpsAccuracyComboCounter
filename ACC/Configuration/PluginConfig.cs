using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ACC.Configuration
{
	internal class PluginConfig
	{
		public static PluginConfig Instance { get; set; } = null!;

		public virtual int AccuracyThreshold { get; set; } = 0;
		public virtual bool ShowOnResultsScreen { get; set; } = true;
		public virtual bool HideComboBreakAnimation { get; set; } = false;
		public virtual bool EnableComboMiniGame { get; set; } = false;
		public ExtraCounterPositions MaxComboPosition { get; set; } = ExtraCounterPositions.Disabled;
		public ExtraCounterPositions LowAccCutsPosition { get; set; } = ExtraCounterPositions.Disabled;
		public virtual bool BreakOnMiss { get; set; } = true;
		public virtual bool BreakOnBadCut { get; set; } = true;
		public virtual bool BreakOnBomb { get; set; } = true;
		public virtual bool BreakOnWall { get; set; } = true;
		public virtual string ComboLabelText { get; set; } = DefaultComboLabelText;
		public virtual string ComboCounterText { get; set; } = DefaultComboCounterText;
		public virtual string MaxComboCounterText { get; set; } = DefaultMaxComboCounterText;
		public virtual string LowAccCutsCounterText { get; set; } = DefaultLowAccCutsCounterText;
		public virtual string ResultText { get; set; } = DefaultResultText;
		public virtual string MaxComboText { get; set; } = DefaultMaxComboText;
		public virtual string FullComboText { get; set; } = DefaultFullComboText;

		internal const string DefaultComboLabelText = "Combo > %t";
		internal const string DefaultComboCounterText = "%c";
		internal const string DefaultMaxComboCounterText = "Max Combo : %m";
		internal const string DefaultLowAccCutsCounterText = "Cuts Below %t : %l";
		internal const string DefaultResultText = "%h<size=70%> / %n</size>";
		internal const string DefaultMaxComboText = "MAX ACC COMBO %m";
		internal const string DefaultFullComboText = "FULL ACC COMBO";



		/// <summary>
		/// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
		/// </summary>
		public virtual void OnReload()
		{
			// Do stuff after config is read from disk.
		}

		/// <summary>
		/// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
		/// </summary>
		public virtual void Changed()
		{
			// Do stuff when the config is changed.
		}

		/// <summary>
		/// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
		/// </summary>
		public virtual void CopyFrom(PluginConfig other)
		{
			// This instance's members populated from other
		}
	}

	public enum ExtraCounterPositions
	{
		AboveCounterPosTwo,
		AboveCounterPosOne,
		Disabled,
		BelowCounterPosOne,
		BelowCounterPosTwo
	}
}