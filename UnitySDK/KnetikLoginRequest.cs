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
		
        // User login
		public KnetikLoginRequest (string user = "", string pass = "")
		{
			m_username = user;
            m_password = pass;
		}

		// Build JSON for Login Request
        string getLoginSessionRequest()
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("serial", KnetikApiUtil.getDeviceSerial ());
            j.AddField ("mac_address", KnetikApiUtil.getMacAddress ());
            // Device Type is currently limited to 3 characters in the DB
            j.AddField ("device_type", SystemInfo.deviceType.ToString().Substring(0, 3));
            j.AddField ("signature", KnetikApiUtil.getDeviceSignature());
            login_request = j.Print ();
            Debug.Log ("LOGIN_REQUEST: " + login_request);
            return login_request;    
        }
		
		public string getKey() {
			return m_clientId;
		}
		
		public int getUserId() 
        {
			return m_userId;
		}
		
		public string getUsername() {
			return m_username;
		}

		// User login
		public bool doLogin(bool register = false)
		{
            m_register = register;
			string request_str = getLoginSessionRequest();
			KnetikJSONNode jsonDict = null;
		
			m_endpointUrl = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.SESSION_ENDPOINT;

			if (sendApiRequest(request_str, ref jsonDict) == false) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 303: Unable to send request for existing user login!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + login_request);
				return false;
			}
			
		    if (jsonDict["result"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 304: Existing User could not be successfully logged in, server has no response!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + login_request);
				return false;
		    }
		    
		    if (jsonDict["result"]["session"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 305: Existing User could not be successfully logged in, server did not return a session!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + login_request);
                Debug.LogError ("Result: " + jsonDict);
				return false;
		    }

			Debug.Log ("Existing User successfully logged in.");
            m_session = jsonDict["result"]["session"];
            m_userId = jsonDict["result"]["user_id"].AsInt;

            if(jsonDict["result"]["user_id"] == null && !m_register)
            {
                Debug.Log ("Guest Session has been established.");
            }
            
            else
            {
                Debug.Log ("Authenticated Session has been established.");
            }
            
            if(m_register)
            {
                m_register = false;
            }

			return true;
		}
	}
}

