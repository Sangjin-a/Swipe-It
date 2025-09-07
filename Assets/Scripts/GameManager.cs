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
        GameRule upRule;

        BlockStackManager blockStackManager;
        Score score;
        int level = 1;
        int scoreForNextLevel = 0;
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
            upRule = new ShapeRule(ShapeType.Triangle, SwipeDirection.Up);
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

                    var randomArr = RandomHelper.GetUniqueRandomValues<ShapeType>(3);
                    leftRule = new ShapeRule(randomArr[0], SwipeDirection.Left);
                    rightRule = new ShapeRule(randomArr[1], SwipeDirection.Right);
                    upRule = new ShapeRule(randomArr[2], SwipeDirection.Up);
                    rule = leftRule;
                    break;
                case GameMode.Color:
                    var colorArr = RandomHelper.GetUniqueRandomValues<BlockColor>(3);
                    leftRule = new ColorRule(colorArr[0], SwipeDirection.Left);
                    rightRule = new ColorRule(colorArr[1], SwipeDirection.Right);
                    upRule = new ColorRule(colorArr[2], SwipeDirection.Up);
                    rule = leftRule;
                    break;
                    /* case GameMode.Number:
                         int leftNumber = UnityEngine.Random.Range(1, 7);
                         int rightNumber;
                         do
                         {
                             rightNumber = UnityEngine.Random.Range(1, 7);
                         } while (rightNumber == leftNumber);
                         leftRule = new NumberRule(leftNumber, SwipeDirection.Left);
                         rightRule = new NumberRule(rightNumber, SwipeDirection.Right);
                         rule = leftRule;
                         break;*/

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
                case SwipeDirection.Up:
                    if (upRule.IsBlockMatch(block.data))
                        isCorrect = true;
                    break;
            }
            scoreForNextLevel += 10;
            if (scoreForNextLevel > 30)
            {
                scoreForNextLevel = 0;
                GetRandomMode();
                UIManager.instance.UpdateAreaInfo(leftRule, UIManager.instance.leftInfo);
                UIManager.instance.UpdateAreaInfo(rightRule, UIManager.instance.rightInfo);
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

// 방법 1: 리스트 셔플 방식 (가장 추천!)
public static class RandomHelper
{
    public static T[] GetUniqueRandomValues<T>(int count) where T : struct, Enum
    {
        var allValues = ((T[])Enum.GetValues(typeof(T))).ToList();

        // 셔플
        for (int i = allValues.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = allValues[i];
            allValues[i] = allValues[randomIndex];
            allValues[randomIndex] = temp;
        }

        return allValues.Take(count).ToArray();
    }
}
public enum GameMode
{
    Shape,
    Color,
    //Number
}
