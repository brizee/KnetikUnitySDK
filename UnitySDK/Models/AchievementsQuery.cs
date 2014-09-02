using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
	public class AchievementsQuery : KnetikModel
	{
        public int PageIndex {
            get;
            set;
        }

        public int PageSize {
            get;
            set;
        }

        public bool HasMore {
            get;
            set;
        }

        public List<Achievement> Achievements {
            get;
            set;
        }

		public AchievementsQuery (KnetikClient client)
			: base(client)
		{
            PageIndex = 1;
            PageSize = 25;
		}

        public void Load(Action<KnetikResult<AchievementsQuery>> cb)
        {
            Client.ListAchievements (PageIndex, PageSize, (res) => {
                var result = new KnetikResult<AchievementsQuery> {
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

        public AchievementsQuery NextPage(Action<KnetikResult<AchievementsQuery>> cb = null) {
            var next = new AchievementsQuery (Client);
            next.PageIndex = PageIndex + 1;
            next.PageSize = PageSize;
            if (cb != null) {
                next.Load (cb);
            }
            return next;
        }
		
		public override void Deserialize (KnetikJSONNode json)
		{
			base.Deserialize (json);
			
            Achievements = new List<Achievement> ();
            foreach (KnetikJSONNode node in json["achievement"].Children) {
                Achievement achievement = new Achievement(Client, json["id"].AsInt);
                achievement.Deserialize(node);
                Achievements.Add(achievement);
            }

            HasMore = json ["hasMore"].AsBool;
		}
	}
}

