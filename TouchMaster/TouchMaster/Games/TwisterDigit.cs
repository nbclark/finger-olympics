using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileSrc.FantasticFingerFun.Games
{
    class TwisterDigit
    {
        public TwisterDigit(GamePlayer player, int fingerIndex)
        {
            this.Player = player;
            this.FingerIndex = fingerIndex;
        }
        public GamePlayer Player
        {
            get;
            set;
        }
        public int FingerIndex
        {
            get;
            set;
        }
        public int TouchId
        {
            get;
            set;
        }
        public TwisterCircle ActiveCircle
        {
            get;
            set;
        }
    }
}
