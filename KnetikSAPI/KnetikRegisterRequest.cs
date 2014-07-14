using System;
using KnetikSimpleJSON;
using UnityEngine;

// Sends request to server to register a new user

namespace Knetik
{
	public class KnetikRegisterRequest : KnetikApiRequest
	{
		private string reg_request = null;
		string m_email;
		string m_fullname;
		
		public KnetikRegisterRequest ()
		{
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_method = "put";
		}

		// Build JSON to register a new user
		string getRegisterData()
		{
            JSONObject userInfo = new JSONObject (JSONObject.Type.OBJECT);
            userInfo.AddField ("email", m_email);
            userInfo.AddField ("password", KnetikApiUtil.sha1(m_password));
            userInfo.AddField ("username", m_username);
            userInfo.AddField ("fullname", m_fullname);
            
            JSONObject register = new JSONObject (JSONObject.Type.OBJECT);
            register.AddField ("user_info", userInfo);            
            reg_request = register.Print ();
            Debug.Log ("REGISTER_REQUEST: " + reg_request);
            return reg_request;     
		}
		
		// Send JSON to register the new user with the server
        public bool doRegister(string apiKey, string username, string password, string email, string fullname)
		{
            m_key = apiKey;
            m_username = username;
            m_password = password;
            m_email = email;
            m_fullname = fullname;
            KnetikJSONNode jsonDict = null;
			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.USER_ENDPOINT;

			if (sendSignedRequest(null, getRegisterData(), ref jsonDict) == false) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 700: Unable to send signed request to register new user with server!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + reg_request);
				return false;
			}
			
		    if (jsonDict["result"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 701: No response from server for registering a new user!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + reg_request);
				return false;
		    }
		 
			Debug.Log ("Successfully registered new user");
			return true;
		}		
	}
}

