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
            List<string> related = null,
            bool useCatalog = true,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("page", page);
            j.AddField ("limit", limit);
            if (terms != null) {
                j.AddField ("terms", JSONObject.Create(terms));
            }
            if (related != null) {
                j.AddField ("related", JSONObject.Create(related));
            }
            j.AddField("useCatalog", useCatalog);

            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(ListStorePageEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        public KnetikApiResponse UseItem(
            int itemID,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("item_id", itemID);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(UseItemEndpoint, body);
            
            return new KnetikApiResponse(this, req, cb);
        }
    }
}

