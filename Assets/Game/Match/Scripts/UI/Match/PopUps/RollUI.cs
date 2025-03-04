using System.Collections.Generic;
using UnityEngine;

namespace Fulbo.Match.UI
{
    public class RollUI : MonoBehaviour
    {
        [SerializeField] private GameObject skillCheckPrefab;
        [SerializeField] private GameObject duelPrefab;

        private Canvas canvas;
        private Camera camera;
        private Match match;

        List<PopUp> popUps;

        private void OnDestroy()
        {
            SkillCheck.SetDisplayedEvent -= OnSkillCheckSetDisplayed;
            DuelRoll.SetDisplayedEvent -= OnDuelSetDisplayed;
        }

        public void Initialize(Canvas canvas, Camera camera, Match match)
        {
            this.canvas = canvas;
            this.camera = camera;
            this.match = match;

            popUps = new List<PopUp>();

            SkillCheck.SetDisplayedEvent += OnSkillCheckSetDisplayed;
            DuelRoll.SetDisplayedEvent += OnDuelSetDisplayed;
        }

        private PopUp CreatePopUp<PopUp>(GameObject prefab, Canvas canvas) where PopUp : RollPopUp
        {
            PopUp popUp = Instantiate(prefab, transform).GetComponent<PopUp>();
            popUp.Canvas = canvas;
            popUp.Camera = camera;
            return popUp;
        }

        #region Handlers
        private void OnSkillCheckSetDisplayed(SkillCheck skillCheck, bool displayed) => OnPopUpDisplayed<SkillCheck, SkillCheckPopUp>(skillCheck, skillCheckPrefab, displayed);

        private void OnDuelSetDisplayed(DuelRoll duel, bool displayed) => OnPopUpDisplayed<DuelRoll, DuelPopUp>(duel, duelPrefab, displayed);

        private void OnPopUpDisplayed<Roll, PopUp>(Roll roll, GameObject popUpPrefab, bool displayed)
            where Roll : RollActionBase
            where PopUp : RollPopUp
        {
            if (displayed)
            {
                PopUp popUp = CreatePopUp<PopUp>(popUpPrefab, canvas);
                popUps.Add(popUp);
                roll.SetPopUp(popUp);

                Canvas.ForceUpdateCanvases();
            }
            else if (popUps.Contains(roll.BasePopUp))
            {
                popUps.Remove(roll.BasePopUp);
                Destroy(roll.BasePopUp.gameObject);
            }
        }
        #endregion
    }
}