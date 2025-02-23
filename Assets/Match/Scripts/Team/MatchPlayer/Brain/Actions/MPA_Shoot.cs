using System;

namespace Fulbo.Match
{
    public class MPA_Shoot : MPAction
    {
        public override bool RequiresFeed => false;

        public event Action<MatchPlayer> ShotEvent;

        public override void OnChosen(bool completeUI)
        {
            hud.Arrow.Show();
            hud.Arrow.Point(player.Position, player.AttackedGoal.BottomCenter);
        }

        public override void OnExecuted() => ShotEvent?.Invoke(player);

        public override void OnExit() => hud.Arrow.Hide();
    }
}