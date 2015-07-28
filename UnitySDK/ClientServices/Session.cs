using System;
using UnityEngine;
using System.Text;

namespace Knetik
{
    public partial class KnetikClient
	{



        public event KnetikEventSuccessDelegate OnLoggedIn;
        public event KnetikEventSuccessDelegate OnGuestLogIn;
        public event KnetikEventSuccessDelegate OnRegistered;
        public event KnetikEventSuccessDelegate OnUserDataFetched;

        public event KnetikEventFailDelegate OnLoginFailed;
		public KnetikApiResponse Login(
			string username,
			string password,
			Action<KnetikApiResponse> cb = null
		) {
			int timestamp = GetTimestamp ();
            string body;
            string serviceBundle = null;

            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.AppendFormat(
				"grant_type=password&username={0}&password={1}&client_id={2}&client_secret={3}",
                System.Uri.EscapeDataString(username),
                System.Uri.EscapeDataString(password),
                System.Uri.EscapeDataString(ClientID),
				System.Uri.EscapeDataString(ClientSecret)
            );
            body = bodyBuilder.ToString();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body, "post", timestamp, serviceBundle, true);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}
		[Obsolete("JSAPI2 now uses GuestRegister and regular Login with user/pass combo returned", true)]
		public KnetikApiResponse GuestLogin(
			Action<KnetikApiResponse> cb = null
		) {
            KnetikApiResponse res = new KnetikApiResponse(KnetikClient.Instance, null);
            res.Status = KnetikApiResponse.StatusType.Error;
			if (cb != null)
            {
                cb(res);
            }
            return res;
		}

        // Attempts to resume a login without interrupting play
        public bool SilentLogin( Action<bool> cb = null)
        {
            // Retrieve any local information
            if(LoadSession())
            {
                Username = PlayerPrefs.GetString(UsernameKey);
                if(string.IsNullOrEmpty(Username))
                {
                    throw new Exception("Username wasn't loaded successfully");
                    if(isGuest)
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
            }
            if(isRegistered)
            {
                LoadUserProfile();
            }
            return isRegistered;

        }
	}
}

