using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fulbo.Match
{
    public class Pitch : MonoBehaviour
    {
        #region Constants
        private const float PlaneMeshScaleMultiplier = 0.1f;

        private const float PitchY = 0f;
        private const float SquareY = 0.01f;
        private const float LineY = 0.02f;

        private const float RealGoalAreaWidth = 18.32f;
        private const float RealGoalAreaLength = 5.5f;
        private const float RealPenaltyAreaWidth = 42.32f;
        private const float RealPenaltyAreaLength = 16.5f;
        #endregion

        [Header("Pitch")]
        [SerializeField] private float length;

        [SerializeField] private MeshRenderer pitchMesh;

        [Header("Board")]
        [SerializeField] private Vector2Int squareCount;
        [SerializeField] private float squareSizeFactor;
        [SerializeField] private Board board;
        [SerializeField] private GameObject squarePrefab;

        [Header("Lines")]
        [SerializeField] private bool useRealAreaDimensions;
        [SerializeField] private float goalAreaWidthInSquares;
        [SerializeField] private float goalAreaLengthInSquares;
        [SerializeField] private float penaltyAreaWidthInSquares;
        [SerializeField] private float penaltyAreaLengthInSquares;
        [SerializeField] private float lineWidth;
        [SerializeField] private Transform lineContainer;
        [SerializeField] private GameObject linePrefab;

        [Header("Goals")]
        [SerializeField] private Goal leftGoal;
        [SerializeField] private Goal rightGoal;

        private float squareStep;
        private Vector3 topLeft;
        private Vector3 bottomRight;

        private Match match;
        
        private float GoalAreaWidth => useRealAreaDimensions ? RealGoalAreaWidth : goalAreaWidthInSquares * squareStep;
        private float GoalAreaLength => useRealAreaDimensions ? RealGoalAreaLength : goalAreaLengthInSquares * squareStep;
        private float PenaltyAreaWidth => useRealAreaDimensions ? RealPenaltyAreaWidth : penaltyAreaWidthInSquares * squareStep;
        private float PenaltyAreaLength => useRealAreaDimensions ? RealPenaltyAreaLength : penaltyAreaLengthInSquares * squareStep;

        public float Length => length;
        public float Width { get; private set; }

        public Board Board => board;

        public Goal LeftGoal => leftGoal;
        public Goal RightGoal => rightGoal;

        public void Initialize(Match match)
        {
            this.match = match;

            ScalePitch();
            SpawnSquares();
            SpawnLines();
            PlaceGoals();
        }

        private void ScalePitch()
        {
            Width = (length / squareCount.x) * squareCount.y;
            pitchMesh.transform.localScale = new Vector3(length * PlaneMeshScaleMultiplier, 0f, Width * PlaneMeshScaleMultiplier);

            topLeft = new Vector3(-length.Half(), PitchY, Width.Half());
            bottomRight = new Vector3(length.Half(), PitchY, -Width.Half());
        }

        private void SpawnSquares()
        {
            List<Transform> children = board.transform.Cast<Transform>().ToList();
            foreach (Transform child in children)
            {
                if (Application.isPlaying) Destroy(child.gameObject);
                else DestroyImmediate(child.gameObject);
            }

            Square[,] squareArray = new Square[squareCount.x, squareCount.y];

            squareStep = length / squareCount.x;
            float squareSize = squareStep * squareSizeFactor * PlaneMeshScaleMultiplier;

            float initialX = topLeft.x + squareStep.Half();
            float currentX = initialX;
            float currentZ = topLeft.z - squareStep.Half();
            for (int y = 0; y < squareCount.y; y++)
            {
                for (int x = 0; x < squareCount.x; x++)
                {
                    Square square = Instantiate(squarePrefab, new Vector3(currentX, SquareY, currentZ), Quaternion.identity, board.transform).GetComponent<Square>();
                    square.transform.localScale = Vector3.one * squareSize;
                    square.name = $"{x}, {y}";

                    (squareArray[x, y] = square).Initialize(x, y, Board);

                    currentX += squareStep;
                }

                currentX = initialX;
                currentZ -= squareStep;
            }

            board.Initialize(squareCount, squareArray, match);
        }

        private void SpawnLines()
        {
            List<Transform> children = lineContainer.Cast<Transform>().ToList();
            foreach (Transform child in children)
            {
                if (Application.isPlaying) Destroy(child.gameObject);
                else DestroyImmediate(child.gameObject);
            }

            // Sidelines
            Transform topSidelineGO =    Instantiate(linePrefab, new Vector3(0f, LineY, topLeft.z)    , Quaternion.identity, lineContainer).transform;
            Transform bottomSidelineGO = Instantiate(linePrefab, new Vector3(0f, LineY, bottomRight.z), Quaternion.identity, lineContainer).transform;
            topSidelineGO.localScale = bottomSidelineGO.localScale = 
                new Vector3((length + lineWidth) * PlaneMeshScaleMultiplier, 1f, lineWidth * PlaneMeshScaleMultiplier);

            // Goal lines
            Transform leftGoallineGO =  Instantiate(linePrefab, new Vector3(    topLeft.x, LineY, 0f), Quaternion.identity, lineContainer).transform;
            Transform rightGoallineGO = Instantiate(linePrefab, new Vector3(bottomRight.x, LineY, 0f), Quaternion.identity, lineContainer).transform;
            Transform halfwayLine =     Instantiate(linePrefab, new Vector3(           0f, LineY, 0f), Quaternion.identity, lineContainer).transform;
            leftGoallineGO.localScale = rightGoallineGO.localScale = halfwayLine.localScale = 
                new Vector3(lineWidth * PlaneMeshScaleMultiplier, 1f, (Width + lineWidth) * PlaneMeshScaleMultiplier);

            // Goal areas
            Transform leftGoalArea1 = Instantiate(linePrefab, new Vector3(topLeft.x + GoalAreaLength.Half(), LineY,  GoalAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            Transform leftGoalArea2 = Instantiate(linePrefab, new Vector3(topLeft.x + GoalAreaLength.Half(), LineY, -GoalAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            leftGoalArea1.localScale = leftGoalArea2.localScale = new Vector3((GoalAreaLength + lineWidth) * PlaneMeshScaleMultiplier, 1f, lineWidth * PlaneMeshScaleMultiplier);
            Transform leftGoalArea3 = Instantiate(linePrefab, new Vector3(topLeft.x + GoalAreaLength, LineY, 0f), Quaternion.identity, lineContainer).transform;
            leftGoalArea3.localScale = new Vector3(lineWidth * PlaneMeshScaleMultiplier, 1f, (GoalAreaWidth + lineWidth) * PlaneMeshScaleMultiplier);

            Transform rightGoalArea1 = Instantiate(linePrefab, new Vector3(bottomRight.x - GoalAreaLength.Half(), LineY,  GoalAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            Transform rightGoalArea2 = Instantiate(linePrefab, new Vector3(bottomRight.x - GoalAreaLength.Half(), LineY, -GoalAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            rightGoalArea1.localScale = rightGoalArea2.localScale = new Vector3((GoalAreaLength + lineWidth) * PlaneMeshScaleMultiplier, 1f, lineWidth * PlaneMeshScaleMultiplier);
            Transform rightGoalArea3 = Instantiate(linePrefab, new Vector3(bottomRight.x - GoalAreaLength, LineY, 0f), Quaternion.identity, lineContainer).transform;
            rightGoalArea3.localScale = new Vector3(lineWidth * PlaneMeshScaleMultiplier, 1f, (GoalAreaWidth + lineWidth) * PlaneMeshScaleMultiplier);

            // Penalty areas
            Transform leftPenaltyArea1 = Instantiate(linePrefab, new Vector3(topLeft.x + PenaltyAreaLength.Half(), LineY,  PenaltyAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            Transform leftPenaltyArea2 = Instantiate(linePrefab, new Vector3(topLeft.x + PenaltyAreaLength.Half(), LineY, -PenaltyAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            leftPenaltyArea1.localScale = leftPenaltyArea2.localScale = new Vector3((PenaltyAreaLength + lineWidth) * PlaneMeshScaleMultiplier, 1f, lineWidth * PlaneMeshScaleMultiplier);
            Transform leftPenaltyArea3 = Instantiate(linePrefab, new Vector3(topLeft.x + PenaltyAreaLength, LineY, 0f), Quaternion.identity, lineContainer).transform;
            leftPenaltyArea3.localScale = new Vector3(lineWidth * PlaneMeshScaleMultiplier, 1f, (PenaltyAreaWidth + lineWidth) * PlaneMeshScaleMultiplier);
            
            Transform rightPenaltyArea1 = Instantiate(linePrefab, new Vector3(bottomRight.x - PenaltyAreaLength.Half(), LineY,  PenaltyAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            Transform rightPenaltyArea2 = Instantiate(linePrefab, new Vector3(bottomRight.x - PenaltyAreaLength.Half(), LineY, -PenaltyAreaWidth.Half()), Quaternion.identity, lineContainer).transform;
            rightPenaltyArea1.localScale = rightPenaltyArea2.localScale = new Vector3((PenaltyAreaLength + lineWidth) * PlaneMeshScaleMultiplier, 1f, lineWidth * PlaneMeshScaleMultiplier);
            Transform rightPenaltyArea3 = Instantiate(linePrefab, new Vector3(bottomRight.x - PenaltyAreaLength, LineY, 0f), Quaternion.identity, lineContainer).transform;
            rightPenaltyArea3.localScale = new Vector3(lineWidth * PlaneMeshScaleMultiplier, 1f, (PenaltyAreaWidth + lineWidth) * PlaneMeshScaleMultiplier);
        }
        
        private void PlaceGoals()
        {
            leftGoal.transform.position = new Vector3(topLeft.x, PitchY, 0f);
            rightGoal.transform.position = new Vector3(bottomRight.x, PitchY, 0f);

            leftGoal.UpdateDimensions();
            rightGoal.UpdateDimensions();
        }
    }
}