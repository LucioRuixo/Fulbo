using System;

namespace Fulbo.Match
{
    using Settings;
    using UI;

    public class MPA_Shoot : MPAction
    {
        private ShotAttempt shotAttempt;
        private SkillCheckPopUp shotAttemptPopUp;

        private Shot shot;
        private DuelPopUp shotPopUp;

        public override bool RequiresFeed => false;

        public event Action<MatchPlayer, RollResult> ShotAttemptEvent;
        public event Action<MatchPlayer, RollResult> ShotEvent;

        public override void OnChosen(bool completeUI)
        {
            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, player.AttackedGoal.BottomCenter);

            shotAttempt = new ShotAttempt(player, MatchSettings.GetShotDifficulty(player));
            SkillCheckResult shotAttempResult = new SkillCheckResult();
            shotAttempResult.Initialize(shotAttempt.Data);
            shotAttemptPopUp = shotAttempt.PopUp;

            MatchPlayer gk = player.Rival.GK;
            shot = new Shot(player, gk);
            DuelResult shotResult = new DuelResult();
            shotResult.Initialize(shot.Data);
            shotPopUp = shot.PopUp;
        }

        public override void OnConfirmed()
        {
            shotAttempt = SkillCheck.RollSC(shotAttempt);
            shotAttemptPopUp.UpdateContent(shotAttempt.Result);

            if (shotAttempt.Result.Failed) return;

            shot = DuelRoll.RollDuel(shot, new DuelData() { Roll = shotAttempt.Result.Roll });
            shotPopUp.UpdateContent(shot.Result);
        }

        public override void Execute()
        {
            ShotAttemptEvent?.Invoke(player, shotAttempt.Result);
            if (shotAttempt.Result.Succeeded) ShotEvent?.Invoke(player, shot.Result);
        }

        public override void OnExit()
        {
            hud.Arrow.Hide();
            if (shotAttempt != null) shotAttempt.SetDisplayed(false);
            if (shot != null) shot.SetDisplayed(false);

            shotAttempt = null;
            shotAttemptPopUp = null;
        }

        public void OnShotAttempt(MatchPlayer kicker, RollResult result) { }
    }
}