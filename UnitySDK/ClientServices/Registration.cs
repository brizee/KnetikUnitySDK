using System;
using UnityEngine;

namespace Knetik
{
    public partial class KnetikClient
	{
        public event KnetikEventSuccessDelegate OnRegisterComplete;
        public event KnetikEventFailDelegate OnRegisterFailed;
		public KnetikApiResponse Register(
			string username,
			string password,
			string email,
			string fullname,
			Action<KnetikApiResponse> cb = null
		) {

			JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
			j.AddField ("username", username);
			j.AddField ("password", password);
			j.AddField ("email", email);
			j.AddField ("fullname", fullname);
			
			String body = j.Print ();
			
			KnetikRequest req = CreateRequest(RegisterEndpoint, body);
			
			KnetikApiResponse res;
            if(cb != null)
            {
                res = new KnetikApiResponse(this, req, (resp) =>
                {
                    Debug.Log(resp.Body);
                    if(resp.Status == KnetikApiResponse.StatusType.Success)
                    {
                        Login(username, password, cb);
                    }
                    else
                    {
                        if (OnRegisterFailed != null)
                        {
                            OnRegisterFailed(resp.ErrorMessage);
                        }
                    }
                    cb(resp);
                });
            }
            else
            {
                res = new KnetikApiResponse(this, req, null);
                Debug.Log(res.Body);
                if(res.Status == KnetikApiResponse.StatusType.Success)
                {
                    Debug.Log(res.Body);
                    res = Login(username, password, null);
                }
                else
                {
                    if (OnRegisterFailed != null)
                    {
                        OnRegisterFailed(res.ErrorMessage);
                    }
                }
            }
			return  res;
		}

        public KnetikApiResponse GuestRegister(
            Action<KnetikApiResponse> cb = null
            ) {

            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(GuestRegisterEndpoint, body);
            
            KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
            return  registerResponse;
        }

        public KnetikApiResponse UpgradeFromRegisteredGuest(
            string username,
            string password,
            string email,
            string fullname,
            Action<KnetikApiResponse> cb = null
            ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("username", username);
            j.AddField ("password", password);
            j.AddField ("email", email);
            j.AddField ("fullname", fullname);
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(UpgradeFromRegisteredGuestEndpoint, body);
            
            KnetikApiResponse res;
            if (cb != null)
            {
                res = new KnetikApiResponse(this, req, (resp) =>
                {
                    Debug.Log(resp.Body);
                    if (resp.Status == KnetikApiResponse.StatusType.Success)
                    {
                        if (OnRegistered != null)
                        {
                            OnRegistered();
                        }
                    }
                    else
                    {
                        if (OnRegisterFailed != null)
                        {
                            OnRegisterFailed(resp.Body["error"]["message"]);
                        }
                    }
                    cb(resp);
                });
            }
            else
            {
                res = new KnetikApiResponse(this, req, null);
                Debug.Log(res.Body);
                if (res.Status == KnetikApiResponse.StatusType.Success)
                {
                    if (OnRegistered != null)
                    {
                        OnRegistered();
                    }
                }
                else
                {
                    if (OnRegisterFailed != null)
                    {
                        OnRegisterFailed(res.Body["error"]["message"]);
                    }
                }
            }
            return  res;
        }
	}
}

