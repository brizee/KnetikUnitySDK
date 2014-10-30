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

        public bool IsForUser {
            get;
            set;
        }

		public AchievementsQuery (KnetikClient client, bool isForUser = false)
			: base(client)
		{
            IsForUser = isForUser;
            PageIndex = 1;
            PageSize = 25;
		}

        public void Load(Action<KnetikResult<AchievementsQuery>> cb)
        {
            if (IsForUser)
            {
                Client.ListUserAchievements(PageIndex, PageSize, HandleAchievementResponse(cb));
            } else
            {
                Client.ListAchievements(PageIndex, PageSize, HandleAchievementResponse(cb));
            }
        }

        public AchievementsQuery NextPage(Action<KnetikResult<AchievementsQuery>> cb = null) {
            var next = new AchievementsQuery (Client, IsForUser);
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

        protected Action<KnetikApiResponse> HandleAchievementResponse(Action<KnetikResult<AchievementsQuery>> cb) {
            return (KnetikApiResponse res) => {
                var result = new KnetikResult<AchievementsQuery> {
                    Response = res
                };
                if (!res.IsSuccess)
                {
                    cb(result);
                    return;
                }
                Response = res;
                
                if (res.Body["result"].Value != "null") {
                    this.Deserialize(res.Body ["result"]);
                } else {
                    Achievements = new List<Achievement> ();
                    HasMore = false;
                }
                
                result.Value = this;
                cb(result);
            };
        }
	}
}

