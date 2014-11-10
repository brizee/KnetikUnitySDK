using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public partial class KnetikClient
    {
        private StoreQuery _store;
        public StoreQuery Store {
            get {
                if (_store == null) {
                    _store = new StoreQuery(this);
                }
                return _store;
            }
        }

        public KnetikApiResponse ListStorePage(
            int page = 1,
            int limit = 10,
            List<string> terms = null,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("page", page);
            j.AddField ("limit", limit);
            if (terms != null) {
                j.AddField ("terms", JSONObject.Create(terms));
            } else {
                j.AddField ("terms", JSONObject.Create(new List<string> {"T-Shirts"}));
            }
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(ListStorePageEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
    }
}

