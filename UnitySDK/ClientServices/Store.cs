using System;
using System.Collections.Generic;

namespace Knetik
{
    public partial class KnetikClient
    {
        public KnetikApiResponse ListStorePage(
            int page = 1,
            int limit = 10,
            List<string> terms = null,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("page", page);
            j.AddField ("limit", limit);
            j.AddField ("params", JSONArray.Create(terms));
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(ListStorePageEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
    }
}

