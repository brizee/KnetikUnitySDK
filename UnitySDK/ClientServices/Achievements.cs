using System;

namespace Knetik
{
	public partial class KnetikClient
	{
		public KnetikApiResponse ListAchievements (
            int page = 1,
            int perPage = 25,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("pageIndex", page);
            j.AddField ("pageSize", perPage);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(ListAchievementsEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
		}
	}
}

