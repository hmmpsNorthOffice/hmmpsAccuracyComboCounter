#define RT_Visible_In_Settings
// To Easter Egg, globally replace '#define RT_Visible_In_Settings' with '#define RT_Invisible_In_Settings' 
//  .... then fix the above comment. :)
// The symbol is defined in four places. RollingThreshold.cs, ConfigController.cs, PluginConfig.cs, AccuracyComboCounter.cs
// MUST also arrange comment around checkbox bool in CounterSettings.bsml.


using ACC.Core;
using ACC.Configuration;

namespace ACC.Utils
{
	internal class RollingThreshold
	{
		// Accuracy Threshold    50,    60,   70,  80,  90, 100, 110 
		private static readonly int[] Good = { 50, 50, 50, 50, 50, 25, 5 };    // max combo target goals
		private static readonly int[] Better = { 100, 100, 100, 100, 100, 50, 10 };
		private static readonly int[] Best = { 300, 300, 200, 200, 200, 75, 25 };


		internal static void RollThreshold(AccManager accManager)
		{
			// use temp int rather than our persistent variable
			int tempAccuracyThreshold = PluginConfig.Instance.AccuracyThreshold;

			int RTListIndex = GetRTListIndex(PluginConfig.Instance.AccuracyThreshold);

			if (accManager.MaxCombo < RollingThreshold.Good[RTListIndex])
			{
				Plugin.Log.Notice("ACC Max Combo did not reach level1 (good) score ... (-1)");
				tempAccuracyThreshold--;
			}
			else if (accManager.MaxCombo < RollingThreshold.Better[RTListIndex])
			{
				Plugin.Log.Notice("ACC Max Combo reached level1 (good) score ... no action");
			}
			else if (accManager.MaxCombo < RollingThreshold.Best[RTListIndex])
			{
				Plugin.Log.Notice("ACC Max Combo reached level2 (better) score ... (+1)");
				tempAccuracyThreshold++;
			}
			else if (accManager.MaxCombo >= RollingThreshold.Best[RTListIndex])
			{
				Plugin.Log.Notice("ACC Max Combo reached level3 (best) score ... (+2)");
				tempAccuracyThreshold += 2;
			}

			// bounds check
			if (tempAccuracyThreshold < 0) { PluginConfig.Instance.AccuracyThreshold = 0; }
			else if (tempAccuracyThreshold > 115) { PluginConfig.Instance.AccuracyThreshold = 115; }
			// save the new threshold
			else PluginConfig.Instance.AccuracyThreshold = tempAccuracyThreshold;
		}

		internal static string RT_Info(AccManager accManager, int currentAccuracyThreshold)
		{
			int RTListIndex = GetRTListIndex(currentAccuracyThreshold);

			// feel free to improve color choices  https://www.december.com/html/spec/colorhex.html
			string color_red = "<color=#FF3300>";   // nectarine (Safe Hex3)	#FF3300
			string color_green = "<color=#4CBB17>"; // kelly	#4CBB17
			string color_white = "<color=#FFFFFF>";

			string font_large = "<size=120%>";
			string font_normal = "<size=100%>";

			if (accManager.MaxCombo < Good[RTListIndex])
				return " (" + color_red + font_large + Good[RTListIndex] + "</size></color>|" + font_normal + color_white + Better[RTListIndex] + "|" + Best[RTListIndex] + ")</size></color>";
			else if (accManager.MaxCombo < Better[RTListIndex])
				return " (" + color_green + font_large + Good[RTListIndex] + "</size></color>|" + font_normal + color_white + Better[RTListIndex] + "|" + Best[RTListIndex] + ")</size></color>";
			else if (accManager.MaxCombo < Best[RTListIndex])
				return " (" + Good[RTListIndex] + "|" + font_large + color_green + Better[RTListIndex] + "</size></color>|" + font_normal + color_white + Best[RTListIndex] + ")</size></color>";
			else // if (accManager.MaxCombo < Good[RTListIndex])
				return " (" + Good[RTListIndex] + "|" + Better[RTListIndex] + "|" + font_large + color_green + Best[RTListIndex] + "</size></color>" + font_normal + color_white + ")</size></color>";

			// ... previous boring version
			// return " (" + Good[RTListIndex] + "|" + Better[RTListIndex] + "|" + Best[RTListIndex] + ")";
		}

		private static int GetRTListIndex(int currentAccuracyThreshold)
		{
			// use math to convert AccuracyThreshold to list index integer
			int RTListIndex = (currentAccuracyThreshold - 50) / 10;

			// bounds check
			if (RTListIndex < 0)  // nicely formatted bounds check :)
			{
				RTListIndex = 0;
			}
			else if (RTListIndex > 6)
			{
				RTListIndex = 6;
			}

			return RTListIndex;
		}

		internal static bool RollingThresholdEnabled()
		{

#if RT_Visible_In_Settings

			return PluginConfig.Instance.EnableRollingThreshold;
#else
			// Easter Egg condition : If 'MaxComboCounterText' begins with "EE" then RollingThreshold is enabled.
			
			return PluginConfig.Instance.MaxComboCounterText.ToLower().StartsWith("ee"); // null check?
#endif
		}

		// Utility to remove "ee" (EasterEgg) from start of MaxComboCounterText
		internal static string RE(string EEstring)
		{
			if (EEstring.ToLower().StartsWith("ee"))
				EEstring = EEstring.Substring(2);

			return EEstring;
		}
	}

}

