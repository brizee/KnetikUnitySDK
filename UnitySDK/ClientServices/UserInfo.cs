using System;

namespace Knetik
{
    public partial class KnetikClient
	{
        public KnetikApiResponse GetUserInfo(Action<KnetikApiResponse> cb = null)
        {
            KnetikRequest req = CreateRequest(GetUserInfoEndpoint);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }

        public KnetikApiResponse GetUserInfoWithProduct(int productId, Action<KnetikApiResponse> cb = null)
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("product_id", productId);
            String body = j.Print ();

            KnetikRequest req = CreateRequest(GetUserInfoWithProductEndpoint, body);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }

        public KnetikApiResponse PutUserInfo(string name, string value, Action<KnetikApiResponse> cb = null)
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("configName", name);
            j.AddField ("configValue", value);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(PutUserInfoWithProductEndpoint, body);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }
	}
}

