using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobileSrc.FantasticFingerFun
{
    class ScoreCenterManager
    {
        static ScoreCenter.ScoreServiceSoapClient _client = new ScoreCenter.ScoreServiceSoapClient();

        private static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static byte[] GetHashCode(string str)
        {
            using (System.Security.Cryptography.SHA1Managed hasher = new System.Security.Cryptography.SHA1Managed())
            {
                return hasher.ComputeHash(Encoding.Unicode.GetBytes(str));
            }
        }

        private static byte[] GenerateHash(string anonymousId, string user, string gameName, string gameVariation, float score)
        {
            return GetHashCode(ReverseString(string.Format("{0}{1}{2}{3}{4:0.00}", anonymousId, user, gameName, gameVariation, score)));
        }

        public static void RegisterScore(string gameName, string gameVariation, float score, ScoreCenter.SortDirection direction, EventHandler<ScoreCenter.RegisterScoreCompletedEventArgs> completedEvent)
        {
            ScoreCenter.ScoreServiceSoapClient client = new ScoreCenter.ScoreServiceSoapClient();
            client.RegisterScoreAsync(FingerGames.Instance.GameManager.AnonymousId, Settings.Instance.GamerTag, gameName, gameVariation, 0, 0, score, direction, GenerateHash(FingerGames.Instance.GameManager.AnonymousId, Settings.Instance.GamerTag, gameName, gameVariation, score));
            client.RegisterScoreCompleted += delegate(object sender, ScoreCenter.RegisterScoreCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    completedEvent(sender, null);
                }
                else
                {
                    completedEvent(sender, e);
                }
            };
        }
    }
}
