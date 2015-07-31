using System;
using System.Collections.Generic;
using KnetikSimpleJSON;
using System.Text;

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

			StringBuilder storeBuilder = new StringBuilder();
			storeBuilder.Append (ListStorePageEndpoint);
			storeBuilder.Append ("?");
			storeBuilder.Append ("page="+page);
			storeBuilder.Append ("&limit="+limit);


	//		j.AddField ("limit", limit);
			if (terms != null) {
				storeBuilder.Append ("&terms="+string.Join(",", terms.ToArray()));
		//		j.AddField ("terms", string.Join(",", terms.ToArray()));
            }
			if (related != null) {
				storeBuilder.Append ("&related="+string.Join(",", related.ToArray()));
		//		j.AddField ("related", string.Join(",",related.ToArray()));
            }
       //     j.AddField("useCatalog", useCatalog);
			storeBuilder.Append ("&useCatalog="+useCatalog);

            String body = j.Print ();
            
			KnetikRequest req = CreateRequest(storeBuilder.ToString(), body,"GET");
            
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

