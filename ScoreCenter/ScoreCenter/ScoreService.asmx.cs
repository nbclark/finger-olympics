using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace ScoreCenter
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class ScoreService : System.Web.Services.WebService
    {
        public enum SortDirection
        {
            Ascending,
            Descending
        }

        private static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        private static byte[] GenerateHash(string anonymousId, string user, string gameName, string gameVariation, float score)
        {
            return GetHashCode(ReverseString(string.Format("{0}{1}{2}{3}{4:0.00}", anonymousId, user, gameName, gameVariation, score)));
        }

        public static byte[] GetHashCode(string str)
        {
            using (System.Security.Cryptography.SHA1Managed hasher = new System.Security.Cryptography.SHA1Managed())
            {
                return hasher.ComputeHash(Encoding.Unicode.GetBytes(str));
            }
        }

        [WebMethod]
        public List<ScoreItem> RegisterScore(string anonymousId, string user, string gameName, string gameVariation, float latitude, float longitude, float score, SortDirection direction, byte[] securityCode)
        {
            byte[] hash = GenerateHash(anonymousId, user, gameName, gameVariation, score);

            bool isValid = false;
            if (hash.Length == securityCode.Length)
            {
                isValid = true;
                for (int i = 0; i < hash.Length; ++i)
                {
                    if (hash[i] != securityCode[i])
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            Int64 pkScore = 0;
            if (isValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(@"Data Source=wpcommuter.db.3448251.hostedresource.com; Initial Catalog=wpcommuter; User ID=wpcommuter; Password='Ce86944';"))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("AddScore", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("AnonymousId", anonymousId);
                        command.Parameters.AddWithValue("User", user);
                        command.Parameters.AddWithValue("Latitude", latitude);
                        command.Parameters.AddWithValue("Longitude", longitude);
                        command.Parameters.AddWithValue("Score", score);
                        command.Parameters.AddWithValue("gameName", gameName);
                        command.Parameters.AddWithValue("gameVariation", gameVariation);

                        pkScore = Convert.ToInt64(command.ExecuteScalar());
                    }
                }
                catch
                {
                }
            }
            else
            {
                throw new Exception(string.Format("{0} != {1}, {2}", hash, securityCode, ReverseString(string.Format("{0}{1}{2}{3}{4:0.00}", anonymousId, user, gameName, gameVariation, score))));
            }

            return GetScoresWithCurrent(gameName, gameVariation, pkScore, direction);
        }

        [WebMethod]
        public List<ScoreItem> GetScores(string gameName, string gameVariation, SortDirection direction)
        {
            return GetScoresWithCurrent(gameName, gameVariation, 0, direction);
        }

        [WebMethod]
        public List<ScoreItem> GetScoresWithCurrent(string gameName, string gameVariation, Int64 lastScore, SortDirection direction)
        {
            List<ScoreItem> scores = new List<ScoreItem>();
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=wpcommuter.db.3448251.hostedresource.com; Initial Catalog=wpcommuter; User ID=wpcommuter; Password='Ce86944';"))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("GetScores", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("idScore", lastScore);
                    command.Parameters.AddWithValue("gameName", gameName);
                    command.Parameters.AddWithValue("gameVariation", gameVariation);
                    command.Parameters.AddWithValue("Ascending", (direction == SortDirection.Ascending));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ScoreItem item = new ScoreItem();
                            item.Index = Convert.ToInt32(reader["RowNumber"]);
                            item.User = Convert.ToString(reader["User"]);
                            item.Score = (float)(double)reader["Score"];
                            item.IsLast = Convert.ToBoolean(reader["MyScore"]);
                            item.Date = (DateTime)reader["Date"];

                            scores.Add(item);
                        }
                    }
                }
            }
            catch
            {
            }
            return scores;
        }
    }

    public class ScoreItem
    {
        public int Index;
        public string User;
        public float Score;
        public bool IsLast;
        public DateTime Date;
    }
}
