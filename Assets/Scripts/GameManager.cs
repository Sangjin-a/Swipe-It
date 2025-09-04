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

        SwipeArea leftArea;
        SwipeArea rightArea;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            currentGameMode = GameMode.Shape;

        }

        /// <summary>
        /// 정답 체크 로직
        /// </summary>
        /// <remarks> 모양,색깔,숫자 </remarks>
        /// <param name="swipe"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        private bool CheckCorrect(SwipeData swipe, Block block)
        {
            bool isCorrect = false;
            //block.data.number
            //block.data.color

            switch (swipe.direction)
            {
                case SwipeDirection.Left:
                    if (leftArea.rule.IsBlockMatch(block.data))
                        isCorrect = true;
                    break;
                case SwipeDirection.Right:
                    if (rightArea.rule.IsBlockMatch(block.data))
                        isCorrect = true;
                    break;
            }
            return isCorrect;
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
