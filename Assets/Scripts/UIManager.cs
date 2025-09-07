using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {

        public static UIManager instance;
        [SerializeField] TextMeshProUGUI scoreText;
        public AreaInfo leftInfo;
        public AreaInfo rightInfo;

        public void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

   
        public void UpdateAreaInfo(GameRule rule,AreaInfo info)
        {
            switch (rule)
            {
                case ColorRule colorRule:
                    //leftRule.icon.sprite = Resources.Load<Sprite>($"Icons/{colorRule.targetColor}");
                    info.text.text = colorRule.targetColor.ToString();
                  
                    break;
                case ShapeRule shapeRule:
                    //leftRule.icon.sprite = Resources.Load<Sprite>($"Icons/{shapeRule.targetShape}");
                    info.text.text = shapeRule.targetShape.ToString();
                  
                    break;
                case NumberRule numberRule:
                    //leftRule.icon.sprite = Resources.Load<Sprite>($"Icons/{numberRule.targetNumber}");
                    info.text.text = numberRule.targetNumber.ToString();
                  
                    break;
            }
        }
    }
}
