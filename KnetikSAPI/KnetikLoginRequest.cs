using System;
using KnetikSimpleJSON;
using UnityEngine;
using System.Collections;

// Builds and Sends request to login to server

namespace Knetik
{
	public class KnetikLoginRequest : KnetikApiRequest
	{
		private string login_request = null;

		// Build JSON for Login Request
		string getLoginSessionRequest(bool isGuest)
		{
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("guest", isGuest);
            j.AddField ("signature", KnetikApiUtil.getDeviceSignature());
            j.AddField ("serial", KnetikApiUtil.getDeviceSerial ());
            j.AddField ("client_key", KnetikApiUtil.API_CLIENT_KEY);   
            
            if (!isGuest) 
            {
                j.AddField("email", m_username);
                j.AddField("password", KnetikApiUtil.sha1(m_password));
            }
            
            login_request = j.Print ();
            Debug.Log ("LOGIN_REQUEST: " + login_request);
            return login_request;   
		}
		
		public string getKey() {
			return m_key;
		}
		
		public long getUserId() {
			return m_userId;
		}
		
		public string getUsername() {
			return m_username;
		}

		// User login (defaults to guest login)
		public bool doLogin(string user = null, string pass = null)
		{
            string request_str = null;
            bool isGuest = false;
            
            m_username = user;
            m_password = pass;
            m_key = KnetikApiUtil.API_CLIENT_KEY;
            m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
            
            if (user == null && pass == null)
            {
                // Guest login
                isGuest = true;
            }
            
            request_str = getLoginSessionRequest(isGuest);
            
			KnetikJSONNode jsonDict = null;
		
			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.AUTH_PREFIX + KnetikApiUtil.SESSION_ENDPOINT;

			if (sendApiRequest(request_str, ref jsonDict) == false) 
			{
                if (isGuest)
                {
                    Debug.LogError("Knetik Labs SDK - ERROR 300: Unable to send request for new user login!");
                }
                
                else
                {
				    Debug.LogError("Knetik Labs SDK - ERROR 303: Unable to send request for existing user login!");
                }
                
				Debug.LogError("Knetik Labs SDK: JSON Request: " + login_request);
				return false;
			}
			
		    if (jsonDict["result"] == null) 
			{
                if (isGuest)
                {
                    Debug.LogError("Knetik Labs SDK - ERROR 301: New User could not be successfully logged in, server has no response!");
                }
                
                else
                {
				    Debug.LogError("Knetik Labs SDK - ERROR 304: Existing User could not be successfully logged in, server has no response!");
                }
                
				Debug.LogError("Knetik Labs SDK: JSON Request: " + login_request);
				return false;
		    }
		    
		    if (jsonDict["result"]["key"] == null) 
			{
                if (isGuest)
                {
                    Debug.LogError("Knetik Labs SDK - ERROR 302: New User could not be successfully logged in, server has no key!");
                }
                
                else
                {
				    Debug.LogError("Knetik Labs SDK - ERROR 305: Existing User could not be successfully logged in, server has no key!");
                }
                
				Debug.LogError("Knetik Labs SDK: JSON Request: " + login_request);
				return false;
		    }

			Debug.Log ("Existing User successfully logged in.");
			m_userId = jsonDict["result"]["user_id"].AsInt;
            m_key = jsonDict["result"]["key"];
			return true;
		}
	}
}

