#nullable enable
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using IPA.Utilities;
using System;
using System.Reflection;
using ACC.Core;
using ACC.Configuration;
using TMPro;
using Zenject;

namespace ACC.ResultsView
{
	class AccResultsViewController : IInitializable, IDisposable
	{
		private static readonly string ResourceNameAccThreshold = "ACC.ResultsView.UI.Views.ResultsComboResult.bsml";

		private static FieldAccessor<ResultsViewController, LevelCompletionResults>.Accessor LevelCompletionResults = FieldAccessor<ResultsViewController, LevelCompletionResults>.GetAccessor("_levelCompletionResults");

		// Text fields in the bsml
		[UIComponent("AccResultText")]
		private TextMeshProUGUI? accResultText = null!;
		[UIComponent("AccMaxComboText")]
		private TextMeshProUGUI? accMaxComboText = null!;

		private readonly AccManager accManager;
		private ResultsViewController resultsViewController;
		private LevelCompletionResults levelCompletionResults = null!;
		private readonly PluginConfig config;

		public AccResultsViewController(AccManager accManager, ResultsViewController resultsViewController)
		{
			this.accManager = accManager;
			this.resultsViewController = resultsViewController;
			this.config = PluginConfig.Instance;
		}

		public void Initialize()
		{
			if (resultsViewController != null)
				resultsViewController.didActivateEvent += ResultsViewController_OnActivateEvent;
		}

		public void Dispose()
		{
			if (resultsViewController != null)
				resultsViewController.didActivateEvent -= ResultsViewController_OnActivateEvent;
		}

		private void ResultsViewController_OnActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{
			levelCompletionResults = LevelCompletionResults(ref resultsViewController);

			ParseAllBSML();

			if (levelCompletionResults.levelEndStateType == global::LevelCompletionResults.LevelEndStateType.Cleared)
				SetResultsViewText();
			else
				EmptyResultsViewText();
		}

		private void ParseAllBSML()
		{
			if (accResultText == null || accMaxComboText == null)
			{
				ParseBSML(ResourceNameAccThreshold);

				if (accResultText != null && accMaxComboText != null)
				{
					accResultText.fontSize *= 1.2f;
					accMaxComboText.fontSize *= 0.60f;
				}
				else
					Plugin.Log.Error($"Parsing BSML ({ResourceNameAccThreshold}) has failed.");
			}
		}

		private void ParseBSML(string bsmlPath)
		{
			BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), bsmlPath), resultsViewController.gameObject, this);
		}

		private void SetResultsViewText()
		{
			// Empty the text fields so they can be filled with new information
			EmptyResultsViewText();

			// Conditionally launch MiniGame (hmmps)
			if (PluginConfig.Instance.EnableComboMiniGame) MaxComboMiniGame(); 

			// Set the text
			if (PluginConfig.Instance.ShowOnResultsScreen && accResultText != null && accMaxComboText != null)
			{
				accResultText.text = accManager.InsertValuesInFormattedString(config.ResultText);
				string maxComboText = accManager.LowAccCuts > 0 ? config.MaxComboText : config.FullComboText;
				accMaxComboText.text = accManager.InsertValuesInFormattedString(maxComboText);
			}
		}

		// hmmps minigame logic ... feel free to adjust.
		private class MiniGameValues
		{
			                        // PrecisionLevel    50,  60,  70,  80,  90,  100,  110 
			internal static readonly int[] NotGood =  {  50,  50,  50,  50,  50,  25,    5 };
			internal static readonly int[] Good =     { 100, 100, 100, 100, 100,  50,   10 };
			internal static readonly int[] VeryGood = { 300, 300, 200, 200, 200,  75,   25 };
		}
		private void MaxComboMiniGame()
		{
			// use temp int rather than our persistent variable
			int tempAccuracyThreshold = PluginConfig.Instance.AccuracyThreshold;

			// use math to convert AccuracyThreshold to list index integer ( similar to enum value )
			int PrecisionEnum = (tempAccuracyThreshold - 50) / 10;

			// bounds check just to be safe
			if (PrecisionEnum < 0) PrecisionEnum = 0; else if (PrecisionEnum > 6) PrecisionEnum = 6;

			if (accManager.MaxCombo < MiniGameValues.NotGood[PrecisionEnum])
			{
				Plugin.Log.Notice("Really Bad ... (-1)");
				tempAccuracyThreshold--; 
			}
			else if (accManager.MaxCombo < MiniGameValues.Good[PrecisionEnum])
			{
				Plugin.Log.Notice("Could do better ... but ok ... no penalty");
				// tempAccuracyThreshold--; removing penalty!
			}
			else if (accManager.MaxCombo < MiniGameValues.VeryGood[PrecisionEnum])
			{
				Plugin.Log.Notice("Pretty Good ... (+1)");
				tempAccuracyThreshold++; 
			}
			else if (accManager.MaxCombo >= MiniGameValues.VeryGood[PrecisionEnum])
			{
				Plugin.Log.Notice("Very Good! ... (+2)");
				tempAccuracyThreshold += 2; 
			}
			else { Plugin.Log.Debug("Mini Game conditional flawed"); }

			// bounds check
			if(tempAccuracyThreshold < 0) { PluginConfig.Instance.AccuracyThreshold = 0; }
			else if (tempAccuracyThreshold > 115) { PluginConfig.Instance.AccuracyThreshold = 115; }
			// save the new threshold
			else PluginConfig.Instance.AccuracyThreshold = tempAccuracyThreshold;
		}

		private void EmptyResultsViewText()
		{
			if (accResultText != null && accMaxComboText != null)
			{
				accResultText.text = "";
				accMaxComboText.text = "";
			}
		}
	}
}