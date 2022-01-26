using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACC.Utils
{
	public static class EnvironmentUtils
	{
		private const float ComboCounterOffsetDefault = 1.41f;

		private static readonly float[] ExtraCounterOffsetsDefault = { 0.36f, 0.12f, 0.0f, -0.34f, -0.57f };

		private static readonly float[] OffsetsOriginsOffset = { 0.10f, 0.10f, 0.0f, -0.10f, -0.10f };
		private static readonly float[] OffsetsBTSOffset = { 0.60f, 0.60f, 0.0f, 0.37f, 0.37f };
		private static readonly float[] OffsetsBillieOffset = { -0.20f, -0.20f, 0.0f, 0.20f, 0.20f };
		private static readonly float[] OffsetsSpookyOffset = { -0.10f, -0.10f, 0.0f, -0.13f, -0.12f };



		internal static bool IsSupportedEnvironmentType(GameplayCoreSceneSetupData sceneSetupData)
		{
			if (sceneSetupData != null)
			{
				//Plugin.Log.Notice($"environmentType.typeNameLocalizationKey = {sceneSetupData.environmentInfo.environmentType.typeNameLocalizationKey}");
				if (sceneSetupData.environmentInfo.environmentType.typeNameLocalizationKey == "NORMAL_ENVIRONMENT_TYPE")
					return true;

				Plugin.Log.Error("This environment is currently not supported. AccuracyComboCounter won't be initialized.");
			}
			return false;
		}

		internal static float GetComboCounterOffset(string envName)
		{
			float offset = ComboCounterOffsetDefault;

			if (IsOriginsOffset(envName) || IsBTSOffset(envName))
				offset -= 0.254f;
			else if (IsBillieOffset(envName))
				offset += 0.346f;
			else if (IsSpookyOffset(envName))
				offset -= 0.054f;

			return offset;				
		}

		internal static float[] GetExtraCountersOffsets(string envName)
		{
			float[] offsets = { 0f, 0f, 0f, 0f, 0f };
			ApplyOffsets(ref offsets, ExtraCounterOffsetsDefault);

			if (IsOriginsOffset(envName))
				ApplyOffsets(ref offsets, OffsetsOriginsOffset);
			else if (IsBTSOffset(envName))
				ApplyOffsets(ref offsets, OffsetsBTSOffset);
			else if (IsBillieOffset(envName))
				ApplyOffsets(ref offsets, OffsetsBillieOffset);
			else if (IsSpookyOffset(envName))
				ApplyOffsets(ref offsets, OffsetsSpookyOffset);

			return offsets;
		}

		private static void ApplyOffsets(ref float[] arr, float[] offsets)
		{
			if (offsets.Length >= arr.Length)
			{
				for (int i = 0; i < arr.Length; i++)
				{
					arr[i] += offsets[i];
				}
			}
			else
				Plugin.Log.Error("EnvironmentUtils: Could not apply offset to array - Not enough offsets in float[] offsets");
		}

		private static readonly List<string> originsOffsetEnvironments = new List<string>()
		{
			"OriginsEnvironment",
			"RocketEnvironment",
			"GreenDayGrenadeEnvironment",
			"GreenDayEnvironment",
			"FitBeatEnvironment",
			"LinkinParkEnvironment",
			"KaleidoscopeEnvironment",
			"InterscopeEnvironment"
		};

		private static readonly List<string> BTSOffsetEnvironments = new List<string>()
		{
			"BTSEnvironment"
		};
		private static readonly List<string> billieOffsetEnvironments = new List<string>()
		{
			"BillieEnvironment"
		};
		private static readonly List<string> spookyOffsetEnvironments = new List<string>()
		{
			"HalloweenEnvironment",
			"GagaEnvironment"
		};

		internal static bool IsOriginsOffset(string envName)
		{
			return originsOffsetEnvironments.Contains(envName);
		}

		internal static bool IsBTSOffset(string envName)
		{
			return BTSOffsetEnvironments.Contains(envName);
		}

		internal static bool IsBillieOffset(string envName)
		{
			return billieOffsetEnvironments.Contains(envName);
		}

		internal static bool IsSpookyOffset(string envName)
		{
			return spookyOffsetEnvironments.Contains(envName);
		}
	}
}
