using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Leaderboard : Item
    {
        public int MetricID {
            get;
            set;
        }
        
        public string SortStyle {
            get;
            set;
        }
        
        public int QualifyingValue {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }

        public int PlayerCount
        {
            get;
            set;
        }

        public string Level
        {
            get;
            set;
        }

        public string MetricName
        {
            get;
            set;
        }

        public List<LeaderboardEntry> Entries
        {
            get;
            set;
        }
        
        public Leaderboard (KnetikClient client)
            : base(client)
        {
        }
        
        public Leaderboard (KnetikClient client, int id)
            : base(client, id)
        {
        }

        public void Load(Action<KnetikResult<Leaderboard>> cb)
        {
            string identifier;
            if (ID > 0)
            {
                identifier = ID.ToString();
            } else {
                identifier = UniqueKey;
            }
            Client.GetLeaderboard(identifier, Level, (res) => {
                var result = new KnetikResult<Leaderboard> {
                    Response = res
                };
                if (!res.IsSuccess) {
                    cb(result);
                    return;
                }
                Response = res;
                
                this.Deserialize(res.Body["result"]);
                
                result.Value = this;
                cb(result);
            });
        }
        
        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);
            
            MetricID = json ["metric_id"].AsInt;
            SortStyle = json ["sort_style"].Value;
            QualifyingValue = json["qualifying_value"].AsInt;
            Size = json["size"].AsInt;
            PlayerCount = json["player_count"].AsInt;
            Level = json["level"].Value;
            MetricName = json["metric_name"].Value;
            Entries = new List<LeaderboardEntry>();

            foreach (KnetikJSONNode node in json["leaderboard_data"].Children)
            {
                LeaderboardEntry entry = new LeaderboardEntry();
                entry.Deserialize(node);
                Entries.Add(entry);
            }

        }

        public class LeaderboardEntry
        {
            public int UserID
            {
                get;
                set;
            }

            public string DisplayName
            {
                get;
                set;
            }
            
            public string AvatarURL
            {
                get;
                set;
            }

            public double Score
            {
                get;
                set;
            }

            public DateTime Date
            {
                get;
                set;
            }

            public void Deserialize(KnetikJSONNode json)
            {
                UserID = json["user_id"].AsInt;
                DisplayName = json["display_name"].Value;
                AvatarURL = json["avatar_url"].Value;
                Score = json["score"].AsDouble;
                Date = DateTime.Parse(json["date"].Value);
            }
        }
    }
}

