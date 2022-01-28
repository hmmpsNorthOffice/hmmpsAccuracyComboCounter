#nullable enable
#define RT_Visible_In_Settings
using CountersPlus.ConfigModels;
using CountersPlus.Counters.Interfaces;
using CountersPlus.Custom;
using CountersPlus.Utils;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using ACC.Core;
using ACC.Configuration;
using TMPro;
using UnityEngine;
using Zenject;
using ACC.Utils;

namespace ACC
{
	class AccuracyComboCounter : ICounter
	{
		private const float ExtraCounterPadding = 0.4f;

		private float[] ExtraCounterOffsets = { };
		private float ComboCounterVerticalOffset;

		public static FieldAccessor<ComboUIController, TextMeshProUGUI>.Accessor ComboUIText = FieldAccessor<ComboUIController, TextMeshProUGUI>.GetAccessor("_comboText");
		public static FieldAccessor<ComboUIController, Animator>.Accessor ComboBreakAnimator = FieldAccessor<ComboUIController, Animator>.GetAccessor("_animator");

		private TextMeshProUGUI? comboLabelText;
		private TextMeshProUGUI? comboCounterText;
		private TMP_Text? maxComboCounter;
		private TMP_Text? lowAccCutsCounter;

		private readonly CanvasUtility canvasUtility;
		private readonly CustomConfigModel settings;
		private readonly AccManager accManager;
		private readonly PluginConfig config;
		private ComboUIController accComboUIController;

		private bool doUpdateComboCounterTextOnCut;

		public AccuracyComboCounter(AccManager accManager, CanvasUtility canvasUtility, CustomConfigModel settings, ComboUIController comboUIController, [InjectOptional] GameplayCoreSceneSetupData sceneSetupData)
		{
			this.canvasUtility = canvasUtility;
			this.settings = settings;
			this.accManager = accManager;
			this.config = PluginConfig.Instance;
			this.accComboUIController = UnityEngine.Object.Instantiate(comboUIController.gameObject).GetComponent<ComboUIController>();
			SetFamilyActive(accComboUIController.transform, true);

			// If the environment type is not supported (or if the sceneSetupData is null), do not init counter.
			if (!EnvironmentUtils.IsSupportedEnvironmentType(sceneSetupData))
				return;

			// Set combo values from previous game to 0
			accManager.Reset();

			// Get the proper offset values for the current environment
			InitOffsetsForEnvironment(sceneSetupData);

			// Init Counters
			InitComboCounter();
			InitMaxComboCounter();
			InitMissesCounter();

			// Refresh text to starting values.
			RefreshCountersText();

			// Show init information
			//DebugCounterInit();
		}

		private void SetFamilyActive(Transform parent, bool beActive)
		{
			// Set parent object to active
			parent.gameObject.SetActive(beActive);

			// Set all children to active
			foreach (Transform child in parent)
				SetFamilyActive(child, beActive);
		}

		private void DebugCounterInit()
		{
			HUDCanvas? canvas = canvasUtility.GetCanvasSettingsFromID(settings.CanvasID);
			if (canvas != null)
			{
				Plugin.Log.Notice($"canvas.PositionScale = {canvas.PositionScale}");
			}

			if (maxComboCounter != null && lowAccCutsCounter != null)
			{
				Vector3 comboPos = accComboUIController.transform.position;
				Plugin.Log.Notice($"comboPos pos = x:{comboPos.x}, y:{comboPos.y}, z:{comboPos.z}");
				Vector3 maxComboPos = maxComboCounter.transform.position;
				Plugin.Log.Notice($"maxComboPos pos = x:{maxComboPos.x}, y:{maxComboPos.y}, z:{maxComboPos.z}");
				Vector3 lowAccCutsPos = lowAccCutsCounter.transform.position;
				Plugin.Log.Notice($"lowAccCutsPos pos = x:{lowAccCutsPos.x}, y:{lowAccCutsPos.y}, z:{lowAccCutsPos.z}");
			}
			else
			{
				Plugin.Log.Notice("maxComboCounter or lowAccCutsCounter == null");
			}

		}

		private void InitOffsetsForEnvironment(GameplayCoreSceneSetupData? sceneSetupData)
		{
			string envName = sceneSetupData == null ? "" : sceneSetupData.environmentInfo.serializedName;

			ComboCounterVerticalOffset = EnvironmentUtils.GetComboCounterOffset(envName);
			ExtraCounterOffsets = EnvironmentUtils.GetExtraCountersOffsets(envName);
		}

		private void InitComboCounter()
		{
			if (canvasUtility != null)
			{
				accComboUIController.name = "AccComboUIController";

				// Set position
				float verticalOffset = ComboCounterVerticalOffset + GetExtraVerticalOffsetComboCounter();
				TMP_Text counterText = canvasUtility.CreateTextFromSettings(settings, new Vector3(0, verticalOffset, 0));
				Vector3 counterPos = counterText.transform.position;
				accComboUIController.transform.position = new Vector3(counterPos.x, counterPos.y, counterPos.z);
				GameObject.Destroy(counterText);

				// Set label text
				Transform comboLabelTextTrans = accComboUIController.transform.Find("ComboText");
				comboLabelText = comboLabelTextTrans.GetComponent<TextMeshProUGUI>();
				if (comboLabelText != null)
				{
					comboLabelText.text = accManager.InsertValuesInFormattedString(config.ComboLabelText);
					comboLabelText.richText = true;
				}
				doUpdateComboCounterTextOnCut = TextRequiresUpdating(config.ComboLabelText);

				// Set counter text
				comboCounterText = ComboUIText(ref accComboUIController);
				if (comboCounterText != null)
				{
					comboCounterText.text = accManager.InsertValuesInFormattedString(config.ComboCounterText);
					comboCounterText.enableWordWrapping = false;
					comboCounterText.richText = true;
				}

				// Disable animation if required
				if (config.HideComboBreakAnimation)
					ComboBreakAnimator(ref accComboUIController).speed = 69420f; // Thanks to Kinsi55's Tweaks55 for providing me with the proper animation speed value.
			}
		}

		private bool TextRequiresUpdating(string str)
		{
			int i = 0;
			while ((i = str.IndexOf('%', i)) >= 0)
			{
				if (++i != str.Length && str[i] != 't')
					return true;
			}
			return false;
		}

		private float GetExtraVerticalOffsetComboCounter()
		{
			// Get vertical offset
			float verticalOffset = 0;
			if (config.MaxComboPosition == ExtraCounterPositions.BelowCounterPosTwo ||
			config.LowAccCutsPosition == ExtraCounterPositions.BelowCounterPosTwo)
			{
				verticalOffset += (1.5f * ExtraCounterPadding);
			}
			else if (config.MaxComboPosition == ExtraCounterPositions.BelowCounterPosOne ||
			config.LowAccCutsPosition == ExtraCounterPositions.BelowCounterPosOne)
			{
				verticalOffset += ExtraCounterPadding;
			}

			return verticalOffset;
		}

		private float GetVerticalOffsetForPosition(ExtraCounterPositions position)
		{
			float verticalOffset = 0;
			if (position == ExtraCounterPositions.BelowCounterPosOne && !IsAnyInPosition(ExtraCounterPositions.BelowCounterPosTwo))
				verticalOffset = ExtraCounterOffsets[(int)ExtraCounterPositions.BelowCounterPosTwo];
			else
			{
				verticalOffset = ExtraCounterOffsets[(int)position];
				if (position == ExtraCounterPositions.AboveCounterPosOne || position == ExtraCounterPositions.AboveCounterPosTwo)
					verticalOffset += GetExtraVerticalOffsetComboCounter();
			}
			return verticalOffset;
		}

		private bool IsAnyInPosition(ExtraCounterPositions position) => config.MaxComboPosition == position || config.LowAccCutsPosition == position;

		private void InitMaxComboCounter()
		{
			if (config.MaxComboPosition != ExtraCounterPositions.Disabled)
			{
				float verticalOffset = GetVerticalOffsetForPosition(config.MaxComboPosition);
				maxComboCounter = canvasUtility.CreateTextFromSettings(settings, new Vector3(0, verticalOffset, 0));
				maxComboCounter.name = "ACCMaxComboText";
				maxComboCounter.fontSize = 2;
			}
		}

		private void InitMissesCounter()
		{
			if (config.LowAccCutsPosition != ExtraCounterPositions.Disabled)
			{
				float verticalOffset = GetVerticalOffsetForPosition(config.LowAccCutsPosition);
				lowAccCutsCounter = canvasUtility.CreateTextFromSettings(settings, new Vector3(0, verticalOffset, 0));
				lowAccCutsCounter.name = "ACCLowAccCutsCounter";
				lowAccCutsCounter.fontSize = 2;
			}
		}

		public void CounterInit()
		{
			accManager.OnComboUpdate += ATManager_OnComboUpdate;
			accManager.OnComboBroken += ATManager_OnComboBroken;
		}

		public void CounterDestroy()
		{
			accManager.OnComboUpdate -= ATManager_OnComboUpdate;
			accManager.OnComboBroken -= ATManager_OnComboBroken;
		}

		private void ATManager_OnComboUpdate(object sender, System.EventArgs e)
		{
			RefreshCountersText();
		}

		private void ATManager_OnComboBroken(object sender, EventArgs e)
		{
			accComboUIController.HandleComboBreakingEventHappened();
		}

		private void RefreshCountersText()
		{
			//accComboUIController.HandleComboDidChange(accManager.ProvisionalCombo);

			if (doUpdateComboCounterTextOnCut && comboLabelText != null)
				comboLabelText.text = accManager.InsertValuesInFormattedString(config.ComboLabelText);
			if (comboCounterText != null)
				comboCounterText.text = accManager.InsertValuesInFormattedString(config.ComboCounterText);
			if (maxComboCounter != null)

#if RT_Visible_In_Settings

				maxComboCounter.text = accManager.InsertValuesInFormattedString(config.MaxComboCounterText);
#else
				// remove Easter Egg text if is there
				maxComboCounter.text = accManager.InsertValuesInFormattedString(Utils.RollingThreshold.RE(config.MaxComboCounterText));
#endif

			if (lowAccCutsCounter != null)
				lowAccCutsCounter.text = accManager.InsertValuesInFormattedString(config.LowAccCutsCounterText);
		}
	}
}