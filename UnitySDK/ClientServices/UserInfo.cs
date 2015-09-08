using System;
using UnityEngine;

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
        public event KnetikEventSuccessDelegate OnUserInfoLoaded;
        public event KnetikEventFailDelegate OnUserInfoLoadFailure;
        public KnetikApiResponse GetUserInfo(Action<KnetikApiResponse> cb = null)
        {
            KnetikRequest req = CreateRequest(GetUserInfoEndpoint);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }

        public KnetikApiResponse GetUserInfoWithProduct(int productId, Action<KnetikApiResponse> cb = null)
        {
            return GetUserInfoWithProduct(productId.ToString(), cb);
        }

        public KnetikApiResponse GetUserInfoWithProduct(string productIdentifier, Action<KnetikApiResponse> cb = null)
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("productId", productIdentifier);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(GetUserInfoWithProductEndpoint, body);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }

        public void LoadUserProfile()
        {            
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                UserInfo.Load((res) =>
                {
                    if(res.Response.Status == KnetikApiResponse.StatusType.Success)
                    {
                        
                        if(Username == null || (Username.Length > 5 && Username.Substring(0, 5) == "Guest"))
                        {
                            Username = UserInfo.Username;
                            if (isGuest)
                            {
                                if (OnGuestLogIn != null)
                                    OnGuestLogIn();
                            }
                            else
                            {
                                if (OnLoggedIn != null)
                                    OnLoggedIn();
                            }
                        }
                        else
                        {
                            Username = UserInfo.Username;
                        }
                        SaveSession();
                        if (OnUserInfoLoaded != null)
                        {
                            OnUserInfoLoaded();
                            
                        }
                    }
                    else
                    {
                        if(OnUserInfoLoadFailure != null)
                        {
                            OnUserInfoLoadFailure(res.Response.ErrorMessage);
                        }
                    }
                });
            }
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

        public KnetikApiResponse GetRelationships(int ancestorDepth, int descendantDepth, bool includeSiblings, Action<KnetikApiResponse> cb = null) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("ancestorDepth", ancestorDepth);
            j.AddField ("descendantDepth", descendantDepth);
            j.AddField ("includeSiblings", includeSiblings);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(UserGetRelationshipsEndpoint, body);
            KnetikApiResponse res = new KnetikApiResponse(this, req, cb);
            return res;
        }
    }
}

