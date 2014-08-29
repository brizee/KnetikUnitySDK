using System;

namespace Knetik
{
    public partial class KnetikClient
	{
        private UserInfo _userInfo;
        public UserInfo UserInfo {
            get {
                if (_userInfo == null) {
                    _userInfo = new UserInfo(this);
                }
                return _userInfo;
            }
        }

        public KnetikApiResponse GetUserInfo(Action<KnetikApiResponse> cb = null)
        {
            KnetikRequest req = CreateRequest(GetUserInfoEndpoint);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }

        public KnetikApiResponse GetUserInfoWithProduct(int productId, Action<KnetikApiResponse> cb = null)
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("productId", productId);
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
            
            KnetikRequest req = CreateRequest(PutUserInfoEndpoint, body);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }
	}
}

