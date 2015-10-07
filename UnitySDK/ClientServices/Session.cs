using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace Knetik
{
    public partial class KnetikClient
	{



        public event KnetikEventSuccessDelegate OnLoggedIn;
        public event KnetikEventSuccessDelegate OnGuestLogIn;
        public event KnetikEventSuccessDelegate OnRegistered;

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
            KnetikApiResponse res;
            if(cb != null)
            {
                res = new KnetikLoginResponse(this, req, (resp) => {
                    if(resp.Status == KnetikApiResponse.StatusType.Success)
                    {
                        PlayerPrefs.SetString(UsernameKey, username);
                        PlayerPrefs.SetString(PasswordKey, password);
                        LoadUserProfile();
                    }
                    else
                    {
                        if(OnLoginFailed != null)
                        {
                            if (resp.Request.response != null && !string.IsNullOrEmpty(resp.Request.response.Text))
                            {
                                JSONObject j = new JSONObject(resp.Request.response.Text);
			                    string error = j["error_description"].str;
                                OnLoginFailed(error);
                            }
                            else
                            {
                                OnLoginFailed(resp.ErrorMessage);
                            }
                        }
                    }
                    cb(resp);
                });
            }
            else
            {
                res = new KnetikLoginResponse(this, req, null);
                if(res.Status == KnetikApiResponse.StatusType.Success)
                {
                    PlayerPrefs.SetString(UsernameKey, username);
                    PlayerPrefs.SetString(PasswordKey, password);
                    LoadUserProfile();
                }
                else
                {
                    if (OnLoginFailed != null)
                    {
                        if (res.Body != null && !string.IsNullOrEmpty(res.Body["error"]["message"]))
                        {
                            OnLoginFailed(res.Body["error"]["message"]);
                        }
                        else
                        {
                            OnLoginFailed(res.ErrorMessage);
                        }
                    }
                }
            }
			return res;
		}
		
		/*
			Login With custome Grant Type 
			@param Dictionary<string,string>
		 */

		public KnetikApiResponse Login(Dictionary<string,string> parameters,
			Action<KnetikApiResponse> cb = null
			) {
			int timestamp = GetTimestamp ();
			string body;
			string serviceBundle = null;

			body = KnetikApiUtil.buildStringRequestFromDictionary (parameters);

			KnetikRequest req = CreateRequest(SessionEndpoint, body, "post", timestamp, serviceBundle, true);

			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);

			return res;
		}



		public KnetikApiResponse refreshSession(
			Action<KnetikApiResponse> cb = null
			) {
			int timestamp = GetTimestamp ();
			string body;
			string serviceBundle = null;
			
			StringBuilder bodyBuilder = new StringBuilder();
			bodyBuilder.AppendFormat(
				"grant_type=refresh_token&refresh_token={0}&client_id={1}&client_secret={2}",
				System.Uri.EscapeDataString(KnetikClient.Instance.RefreshToken),
				System.Uri.EscapeDataString(ClientID),
				System.Uri.EscapeDataString(ClientSecret)
				);
			body = bodyBuilder.ToString();
			
			KnetikRequest req = CreateRequest(SessionEndpoint, body, "post", timestamp, serviceBundle, true);
			KnetikApiResponse res = new KnetikLoginResponse(this, req, cb);
			return res;
		}
		

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
            if(PlayerPrefs.HasKey(UsernameKey) && PlayerPrefs.HasKey(PasswordKey))
            {
                Username = PlayerPrefs.GetString(UsernameKey);
                Password = PlayerPrefs.GetString(PasswordKey);
                if(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    throw new Exception("Username/Password wasn't loaded successfully");
                    return false;  
                }
                else
                {
                    if (cb != null)
                        Login(Username, Password, res => {
                            cb(res.IsSuccess);
                        });
                    else
                        return Login(Username, Password).IsSuccess;
                }
            }
            return false;

        }
	}


}

