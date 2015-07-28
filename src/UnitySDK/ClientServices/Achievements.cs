using System;

namespace Knetik
{
	public partial class KnetikClient
	{
        private AchievementsQuery _achievements;
        public AchievementsQuery Achievements {
            get {
                if (_achievements == null) {
                    _achievements = new AchievementsQuery(this);
                }
                return _achievements;
            }
        }

        public KnetikApiResponse ListAchievements (
            int page = 1,
            int perPage = 25,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("page_index", page);
            j.AddField ("page_size", perPage);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(ListAchievementsEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
		}

        public KnetikApiResponse ListUserAchievements (
            int page = 1,
            int perPage = 25,
            Action<KnetikApiResponse> cb = null
            ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("page_index", page);
            j.AddField ("page_size", perPage);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(ListUserAchievementsEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
	}
}

