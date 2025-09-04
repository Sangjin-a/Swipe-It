using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{

    public class Score
    {
        private int val;

        public int Value
        {
            get { return val; }
            set
            {
                val = value;
                CoreManager.instance.scoreTMP.text = val.ToString();
                
            }
        }

        public Score(int initialValue = 0)
        {
            Value = initialValue;
        }
        public void Add(int amount)
        {
            Value += amount;
        }
        public void Subtract(int amount)
        {
            Value -= amount;
            if (val < 0) val = 0;
        }
        public void Reset()
        {
            Value = 0;
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public void Check()
        {

        }
    }
}
