#define RT_Visible_In_Settings

using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ACC.Configuration
{
	class ConfigController : INotifyPropertyChanged
	{
		private static string enabledTextColor = "#" + ColorUtility.ToHtmlStringRGB(Color.white);
		private static string disabledTextColor = "#" + ColorUtility.ToHtmlStringRGB(Color.grey);
		private string GetInteractabilityColor(bool isEnabled) => isEnabled ? enabledTextColor : disabledTextColor;

#pragma warning disable CS8618
		public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8618

		private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		// Settings getters/setters
		[UIValue("AccuracyThreshold")]
		public virtual int AccuracyThreshold
		{
			get { return PluginConfig.Instance.AccuracyThreshold; }
			set { PluginConfig.Instance.AccuracyThreshold = value; }
		}

		[UIValue("ShowOnResultsScreen")]
		public virtual bool ShowOnResultsScreen
		{
			get { return PluginConfig.Instance.ShowOnResultsScreen; }
			set { PluginConfig.Instance.ShowOnResultsScreen = value; }
		}

		[UIValue("MaxComboPosition")]
		public virtual ExtraCounterPositions MaxComboPosition
		{
			get { return PluginConfig.Instance.MaxComboPosition; }
			set 
			{
				PluginConfig.Instance.MaxComboPosition = value;
				RaisePropertyChanged();
				UpdateInteractibilityMaxComboCounter();
			}
		}

		[UIValue("LowAccCutsPosition")]
		public virtual ExtraCounterPositions LowAccCutsPosition
		{
			get { return PluginConfig.Instance.LowAccCutsPosition; }
			set 
			{ 
				PluginConfig.Instance.LowAccCutsPosition = value;
				RaisePropertyChanged();
				UpdateInteractibilityLowAccCutsCounter();
			}
		}

		[UIValue("BreakOnMiss")]
		public virtual bool BreakOnMiss
		{
			get { return PluginConfig.Instance.BreakOnMiss; }
			set { PluginConfig.Instance.BreakOnMiss = value; }
		}

		[UIValue("BreakOnBadCut")]
		public virtual bool BreakOnBadCut
		{
			get { return PluginConfig.Instance.BreakOnBadCut; }
			set { PluginConfig.Instance.BreakOnBadCut = value; }
		}

		[UIValue("BreakOnBomb")]
		public virtual bool BreakOnBomb
		{
			get { return PluginConfig.Instance.BreakOnBomb; }
			set { PluginConfig.Instance.BreakOnBomb = value; }
		}

		[UIValue("BreakOnWall")]
		public virtual bool BreakOnWall
		{
			get { return PluginConfig.Instance.BreakOnWall; }
			set { PluginConfig.Instance.BreakOnWall = value; }
		}

		[UIValue("HideComboBreakAnimation")]
		public virtual bool HideComboBreakAnimation
		{
			get { return PluginConfig.Instance.HideComboBreakAnimation; }
			set { PluginConfig.Instance.HideComboBreakAnimation = value; }
		}

#if RT_Visible_In_Settings
		[UIValue("EnableRollingThreshold")] 
		public virtual bool EnableRollingThreshold
		{
			get { return PluginConfig.Instance.EnableRollingThreshold; }
			set { PluginConfig.Instance.EnableRollingThreshold = value; }
		} 
#endif

		[UIValue("ComboLabelText")]
		public virtual string ComboLabelText
		{
			get { return PluginConfig.Instance.ComboLabelText; }
			set 
			{ 
				PluginConfig.Instance.ComboLabelText = value;
				RaisePropertyChanged();
			}
		}

		[UIValue("ComboCounterText")]
		public virtual string ComboCounterText
		{
			get { return PluginConfig.Instance.ComboCounterText; }
			set 
			{ 
				PluginConfig.Instance.ComboCounterText = value;
				RaisePropertyChanged();
			}
		}

		[UIValue("MaxComboCounterText")]
		public virtual string MaxComboCounterText
		{
			get { return PluginConfig.Instance.MaxComboCounterText; }
			set 
			{ 
				PluginConfig.Instance.MaxComboCounterText = value;
				RaisePropertyChanged();
			}
		}

		[UIValue("LowAccCutsCounterText")]
		public virtual string LowAccCutsCounterText
		{
			get { return PluginConfig.Instance.LowAccCutsCounterText; }
			set 
			{ 
				PluginConfig.Instance.LowAccCutsCounterText = value;
				RaisePropertyChanged();
			}
		}


		// Reset to default buttons click events
		[UIAction("#reset-combo-label-text")]
		public void OnResetComboLabelText()
		{
			ComboLabelText = PluginConfig.DefaultComboLabelText;
			RaisePropertyChanged(nameof(ComboLabelText));
		}

		[UIAction("#reset-combo-counter-text")]
		public void OnResetComboCounterText()
		{
			ComboCounterText = PluginConfig.DefaultComboCounterText;
			RaisePropertyChanged(nameof(ComboCounterText));
		}

		[UIAction("#reset-max-combo-counter-text")]
		public void OnResetMaxComboCounterText() 
		{
			MaxComboCounterText = PluginConfig.DefaultMaxComboCounterText;
			RaisePropertyChanged(nameof(MaxComboCounterText));
		}

		[UIAction("#reset-low-acc-cuts-counter-text")]
		public void OnResetLowAccCutsCounterText() 
		{
			LowAccCutsCounterText = PluginConfig.DefaultLowAccCutsCounterText;
			RaisePropertyChanged(nameof(LowAccCutsCounterText));
		}



		// Enable/Disable event handlers
		[UIValue("is-max-combo-counter-enabled")]
		private bool IsMaxComboCounterEnabled => IsDisabled(PluginConfig.Instance.MaxComboPosition);
		[UIValue("is-max-combo-counter-enabled-color")]
		private string IsMaxComboCounterEnabledColor => GetInteractabilityColor(IsMaxComboCounterEnabled);
		
		[UIValue("is-low-acc-cuts-counter-enabled")]
		private bool IsLowAccCutsCounterEnabled => IsDisabled(PluginConfig.Instance.LowAccCutsPosition);
		[UIValue("is-low-acc-cuts-counter-enabled-color")]
		private string IsLowAccCutsCounterEnabledColor => GetInteractabilityColor(IsLowAccCutsCounterEnabled);
		private bool IsDisabled(ExtraCounterPositions counterPos) => counterPos != ExtraCounterPositions.Disabled;


		// Update interactibility
		private void UpdateInteractibilityMaxComboCounter()
		{
			RaisePropertyChanged(nameof(IsMaxComboCounterEnabled));
			RaisePropertyChanged(nameof(IsMaxComboCounterEnabledColor));
		}

		private void UpdateInteractibilityLowAccCutsCounter()
		{
			RaisePropertyChanged(nameof(IsLowAccCutsCounterEnabled));
			RaisePropertyChanged(nameof(IsLowAccCutsCounterEnabledColor));
		}



		[UIValue(nameof(ExtraCounterPositionsList))]
		public List<object> ExtraCounterPositionsList => ExtraCounterPositionsToNames.Keys.Cast<object>().ToList();

		[UIAction(nameof(ExtraCounterPositionsFormat))]
		public string ExtraCounterPositionsFormat(ExtraCounterPositions position) => ExtraCounterPositionsToNames[position];

		private static Dictionary<ExtraCounterPositions, string> ExtraCounterPositionsToNames = new Dictionary<ExtraCounterPositions, string>()
		{
			{ExtraCounterPositions.AboveCounterPosTwo, "Above +2" },
			{ExtraCounterPositions.AboveCounterPosOne, "Above +1" },
			{ExtraCounterPositions.Disabled, "Disabled" },
			{ExtraCounterPositions.BelowCounterPosOne, "Below -1" },
			{ExtraCounterPositions.BelowCounterPosTwo, "Below -2" }
		};
	}
}
