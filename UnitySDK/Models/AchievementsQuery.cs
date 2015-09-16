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

        public KnetikResult<AchievementsQuery> Load(Action<KnetikResult<AchievementsQuery>> cb = null)
        {
            if (cb != null)
            {
                // async
                if (IsForUser)
                {
                    Client.ListUserAchievements(PageIndex, PageSize, HandleAchievementResponse(cb));
                } else
                {
                    Client.ListAchievements(PageIndex, PageSize, HandleAchievementResponse(cb));
                }
                return null;
            } else
            {
                // sync
                if (IsForUser)
                {
                    return OnLoad(Client.ListUserAchievements(PageIndex, PageSize));
                } else
                {
                    return OnLoad(Client.ListAchievements(PageIndex, PageSize));
                }
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
				if(node != null)
				{
                	Achievement achievement = (Achievement)Item.Parse(Client, node);
                	Achievements.Add(achievement);
				}
            }

            HasMore = json ["hasMore"].AsBool;
		}

        private Action<KnetikApiResponse> HandleAchievementResponse(Action<KnetikResult<AchievementsQuery>> cb)
        {
            return (KnetikApiResponse res) => {
                cb(OnLoad(res));
            };
        }

        private KnetikResult<AchievementsQuery> OnLoad(KnetikApiResponse res)
        {
            var result = new KnetikResult<AchievementsQuery> {
                Response = res
            };
            if (!res.IsSuccess)
            {
                return result;
            }
            Response = res;
            
            if (res.Body["result"].Value != "null") {
                this.Deserialize(res.Body ["result"]);
            } else {
                Achievements = new List<Achievement> ();
                HasMore = false;
            }
            
            result.Value = this;
            return result;
        }
	}
}

