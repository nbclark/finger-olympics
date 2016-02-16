using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileSrc.FantasticFingerFun
{
    class GamePlayer
    {
        public GamePlayer(int index, string name)
        {
            this.Index = index;
            this.Name = string.Concat("Player #", index + 1);
        }

        public int Index
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public double Score
        {
            get;
            set;
        }
        public double OverallScore
        {
            get;
            set;
        }
    }
}
