using SwipeSort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        [SerializeField] public GameMode currentGameMode = GameMode.Shape;

        GameRule leftRule;
        GameRule rightRule;

        BlockStackManager blockStackManager;
        Score score;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            blockStackManager = GetComponent<BlockStackManager>();
            currentGameMode = GameMode.Shape;
            SetGameRule();
        }
        private void SetGameRule()
        {
            leftRule = new ShapeRule(ShapeType.Circle, SwipeDirection.Left);
            rightRule = new ShapeRule(ShapeType.Square, SwipeDirection.Right);
            score = new Score();
        }

        public void GetRandomMode()
        {
            GameRule rule = null;

            Array modes = Enum.GetValues(typeof(GameMode));
            GameMode randomMode = (GameMode)modes.GetValue(UnityEngine.Random.Range(0, modes.Length));
            currentGameMode = randomMode;

            switch (randomMode)
            {
                case GameMode.Shape:
                    ShapeType leftShape = (ShapeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ShapeType)).Length);
                    ShapeType rightShape;
                    do
                    {
                        rightShape = (ShapeType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ShapeType)).Length);
                    } while (rightShape == leftShape);
                    leftRule = new ShapeRule(leftShape, SwipeDirection.Left);
                    rightRule = new ShapeRule(rightShape, SwipeDirection.Right);
                    rule = leftRule;
                    break;
                case GameMode.Color:
                    BlockColor leftColor = (BlockColor)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BlockColor)).Length);
                    BlockColor rightColor;
                    do
                    {
                        rightColor = (BlockColor)UnityEngine.Random.Range(0, Enum.GetValues(typeof(BlockColor)).Length);
                    } while (rightColor == leftColor);
                    leftRule = new ColorRule(leftColor, SwipeDirection.Left);
                    rightRule = new ColorRule(rightColor, SwipeDirection.Right);
                    rule = leftRule;
                    break;
                case GameMode.Number:
                    int leftNumber = UnityEngine.Random.Range(1, 7);
                    int rightNumber;
                    do
                    {
                        rightNumber = UnityEngine.Random.Range(1, 7);
                    } while (rightNumber == leftNumber);
                    leftRule = new NumberRule(leftNumber, SwipeDirection.Left);
                    rightRule = new NumberRule(rightNumber, SwipeDirection.Right);
                    rule = leftRule;
                    break;

            }
            Debug.Log($"<color=green>[GameManager] GameMode changed to {currentGameMode}, LeftRule: {leftRule}, RightRule: {rightRule}</color>");

        }

        /// <summary>
        /// 정답 체크 로직
        /// </summary>
        /// <remarks> 모양,색깔,숫자 </remarks>
        /// <param name="swipe"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool CheckCorrect(SwipeData swipe, Block block)
        {
            bool isCorrect = false;
            //block.data.number
            //block.data.color

            switch (swipe.direction)
            {
                case SwipeDirection.Left:
                    if (leftRule.IsBlockMatch(block.data))
                        isCorrect = true;
                    break;
                case SwipeDirection.Right:
                    if (rightRule.IsBlockMatch(block.data))
                        isCorrect = true;
                    break;
            }

            if (score.Value > 50)
            {
                GetRandomMode();
            }
            return isCorrect;
        }

        internal void Swipe(SwipeData data)
        {
            var block = blockStackManager.HandleSwipeDetected(data);
            bool result = CheckCorrect(data, block);
            if (result)
                score.Add(10);
            else
                score.Subtract(5);
        }

        private class SwipeArea
        {
            public SwipeDirection direction;
            public GameRule rule;

            public SwipeArea(SwipeDirection dir, GameRule gameRule)
            {
                direction = dir;
                rule = gameRule;
            }
        }
    }


}

public enum GameMode
{
    Shape,
    Color,
    Number
}
