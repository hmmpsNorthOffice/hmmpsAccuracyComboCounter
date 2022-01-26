using ACC.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace ACC
{
	// Ah. The art of the bodge.
	// The Start function of the ComboUIController sets the comboText to "0". But I want it to be something else.
	// And since the Start method is called after the Counter's Init I have to find a way to set the text after the Start method sets it to "0". Hence the HarmonyPatch :D

	[HarmonyPatch(typeof(ComboUIController))]
	[HarmonyPatch("Start")]
	class ComboUIControllerPatch
	{
		internal static void Postfix(TextMeshProUGUI ____comboText, ComboUIController __instance)
		{
			if (__instance.name == "AccComboUIController")
			{
				____comboText.text = Plugin.AccManager?.InsertValuesInFormattedString(PluginConfig.Instance.ComboCounterText);
			}
		}
	}
}
