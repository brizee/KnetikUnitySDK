using System;
using System.Collections.Generic;

namespace Knetik
{
	public partial class KnetikClient
	{
        public KnetikApiResponse FireEvent(
            string eventName,
            Dictionary<string, string> parameters,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("eventName", eventName);
            j.AddField ("params", JSONObject.Create(parameters));
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(FireEventEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
	}
}

