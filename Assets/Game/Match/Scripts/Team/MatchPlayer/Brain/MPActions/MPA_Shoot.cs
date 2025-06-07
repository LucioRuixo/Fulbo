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

        public override MPActions Type => MPActions.Shoot;
        public override int APCost => 2;
        public override bool RequiresFeed => false;

        public static event Action<MatchPlayer, RollResult> ShotAttemptEvent;
        public static event Action<MatchPlayer, RollResult> ShotEvent;

        public override void OnChosen(bool completeUI)
        {
            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, player.AttackedGoal.BottomCenter);

            shotAttempt = new ShotAttempt(player, MatchSettings.GetShotDifficulty(player));
            shotAttemptPopUp = shotAttempt.PopUp;

            MatchPlayer gk = player.Rival.GK;
            shot = new Shot(player, gk);
            shotPopUp = shot.PopUp;
        }

        public override void OnConfirmed()
        {
            shotAttempt = SkillCheck.RollSC(shotAttempt);
            shotAttemptPopUp.UpdateContent(shotAttempt.Result);

            if (shotAttempt.Result.Failed) return;

            shot = DuelRoll.RollDuel(shot);
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

            shotAttempt?.SetDisplayed(false);
            shot?.SetDisplayed(false);

            shotAttempt = null;
            shot = null;
            shotAttemptPopUp = null;
            shotPopUp = null;
        }
    }
}