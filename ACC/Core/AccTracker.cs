#nullable enable
using System;
using System.Collections.Generic;
using ACC.Configuration;
using Zenject;

namespace ACC.Core
{
	class AccTracker : IInitializable, IDisposable, ISaberSwingRatingCounterDidChangeReceiver, ISaberSwingRatingCounterDidFinishReceiver
	{
		private const int MaxSwingScore = ScoreModel.kMaxBeforeCutSwingRawScore + ScoreModel.kMaxAfterCutSwingRawScore;

		private Dictionary<ISaberSwingRatingCounter, NoteCutInfo> swingCounterCutInfo = new Dictionary<ISaberSwingRatingCounter, NoteCutInfo>();

		private readonly ScoreController? scoreController;
		private readonly AccManager accManager;
		private readonly PluginConfig config;

		public AccTracker([InjectOptional] ScoreController scoreController, AccManager accManager)
		{
			this.scoreController = scoreController;
			this.accManager = accManager;
			config = PluginConfig.Instance;
		}

		public void Initialize()
		{
			// Reset the AccThresholdManager's stored values
			accManager.Reset();

			// Assign events
			if (scoreController != null)
			{
				scoreController.noteWasMissedEvent += ScoreController_noteWasMissedEvent;
				scoreController.noteWasCutEvent += ScoreController_noteWasCutEvent;
			}

			ScoreControllerWallCollisionDetectionPatch.wallCollisionEvent += WallCollisionDetector_wallCollisionEvent;
		}

		public void Dispose()
		{
			// Unassign events
			if (scoreController != null)
			{
				scoreController.noteWasMissedEvent -= ScoreController_noteWasMissedEvent;
				scoreController.noteWasCutEvent -= ScoreController_noteWasCutEvent;
			}

			ScoreControllerWallCollisionDetectionPatch.wallCollisionEvent -= WallCollisionDetector_wallCollisionEvent;
		}

		private void ScoreController_noteWasMissedEvent(NoteData noteData, int _) => EvaluateMiss(noteData);
		private void ScoreController_noteWasCutEvent(NoteData noteData, in NoteCutInfo noteCutInfo, int multiplier) => EvaluateCut(noteData, noteCutInfo);
		private void WallCollisionDetector_wallCollisionEvent() => accManager.BreakCombo(BrokenComboType.HeadWasInObstacle);

		private void EvaluateMiss(NoteData noteData)
		{
			if (IsBomb(noteData))
				return;

			accManager.BreakCombo(BrokenComboType.Miss);
		}

		private void EvaluateCut(NoteData noteData, NoteCutInfo noteCutInfo)
		{
			if (IsBomb(noteData))
				accManager.BreakCombo(BrokenComboType.BombCut);
			else if (IsCutBad(noteCutInfo))
				accManager.BreakCombo(BrokenComboType.BadCut);
			else if (IsCutUnableToExceedThreshold(noteCutInfo))
				accManager.BreakCombo(BrokenComboType.BelowThresholdOnCut);
			else if (IsCutAboveThreshold(noteCutInfo))
				accManager.IncreaseCombo(IncreaseComboType.OnCut);
			else
				TrackCut(noteCutInfo);
		}

		private bool IsBomb(NoteData noteData) => noteData.colorType == ColorType.None;
		private bool IsCutBad(NoteCutInfo noteCutInfo) => !noteCutInfo.allIsOK;
		private bool IsCutUnableToExceedThreshold(NoteCutInfo noteCutInfo) => MaxPotentialScore(noteCutInfo) < config.AccuracyThreshold;
		private bool IsCutAboveThreshold(NoteCutInfo noteCutInfo) => GetCutScore(noteCutInfo) >= config.AccuracyThreshold;
		private bool IsCutAboveThreshold(ISaberSwingRatingCounter saberSwingRatingCounter) => IsCutAboveThreshold(GetnoteCutInfo(saberSwingRatingCounter));

		private int MaxPotentialScore(NoteCutInfo noteCutInfo)
		{
			ScoreModel.RawScoreWithoutMultiplier(noteCutInfo.swingRatingCounter, noteCutInfo.cutDistanceToCenter, out int _, out int _, out int acc);
			return acc + MaxSwingScore;
		}

		private int GetCutScore(NoteCutInfo noteCutInfo)
		{
			ScoreModel.RawScoreWithoutMultiplier(noteCutInfo.swingRatingCounter, noteCutInfo.cutDistanceToCenter, out int preSwing, out int afterSwing, out int acc);
			return preSwing + afterSwing + acc;
		}

		private NoteCutInfo GetnoteCutInfo(ISaberSwingRatingCounter saberSwingRatingCounter)
		{
			NoteCutInfo noteCutInfo;
			if (!swingCounterCutInfo.TryGetValue(saberSwingRatingCounter, out noteCutInfo))
			{
				Plugin.Log.Error("AccThresholdTracker, GetnoteCutInfo : Failed to get NoteCutInfo from noteCutInfoData!");
				UnregisterSaberSwingRatingCounter(saberSwingRatingCounter);
			}

			return noteCutInfo;
		}

		private void TrackCut(NoteCutInfo noteCutInfo)
		{
			accManager.IncreaseCombo(IncreaseComboType.ProvisionalOnCut);

			swingCounterCutInfo.Add(noteCutInfo.swingRatingCounter, noteCutInfo);
			noteCutInfo.swingRatingCounter.RegisterDidChangeReceiver(this);
			noteCutInfo.swingRatingCounter.RegisterDidFinishReceiver(this);
		}

		public void HandleSaberSwingRatingCounterDidChange(ISaberSwingRatingCounter saberSwingRatingCounter, float rating)
		{
			if (IsCutAboveThreshold(saberSwingRatingCounter))
			{
				accManager.IncreaseCombo(IncreaseComboType.ProvisionalFinish);

				UnregisterSaberSwingRatingCounter(saberSwingRatingCounter);
			}
		}

		public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter)
		{
			if (IsCutAboveThreshold(saberSwingRatingCounter))
			{
				Plugin.Log.Warn("Please let ChirpyMisha know that \"the condition\" has been met. The world may end when this warning will be ignored . . .");
				accManager.IncreaseCombo(IncreaseComboType.ProvisionalFinish);
			}
			else
				accManager.BreakCombo(BrokenComboType.BelowThresholdOnFinish);

			UnregisterSaberSwingRatingCounter(saberSwingRatingCounter);
		}

		private void UnregisterSaberSwingRatingCounter(ISaberSwingRatingCounter saberSwingRatingCounter)
		{
			swingCounterCutInfo.Remove(saberSwingRatingCounter);
			saberSwingRatingCounter.UnregisterDidChangeReceiver(this);
			saberSwingRatingCounter.UnregisterDidFinishReceiver(this);
		}
	}
}