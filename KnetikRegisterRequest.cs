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
		
		public KnetikRegisterRequest (string api_key, string username, string password, string email, string fullname)
		{
			m_clientId = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_username = username;
			m_password = password;
			m_email = email;
			m_fullname = fullname;
		}

		// Build JSON to register a new user
		string getRegisterData()
		{
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("username", m_username);
            j.AddField ("password", m_password);
            j.AddField ("email", m_email);
            j.AddField ("fullname", m_fullname);
            reg_request = j.Print ();
            Debug.Log ("REGISTER_REQUEST: " + reg_request);
            return reg_request;   
		}
		
		// Send JSON to register the new user with the server
		public bool doRegister()
		{
			KnetikJSONNode jsonDict = null;
            string request_str = getRegisterData();
            m_endpointUrl = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.REGISTER_ENDPOINT;

            if (sendApiRequest(request_str, ref jsonDict) == false) 
            {
                    Debug.LogError("Knetik Labs SDK - ERROR 303: Unable to send request for guest user login!");
                                Debug.LogError("Knetik Labs SDK: JSON Request: " + reg_request);
                    return false;
            }
            
            if (jsonDict["result"] == null) 
            {
                    Debug.LogError("Knetik Labs SDK - ERROR 304: Guest/New User could not be successfully logged in, server has no response!");
                                Debug.LogError("Knetik Labs SDK: JSON Request: " + reg_request);
                    return false;
            }
            
            if (jsonDict["result"]["session"] == null) 
            {
                    Debug.LogError("Knetik Labs SDK - ERROR 305: Guest/New User could not be successfully logged in, server did not return a session!");
                    Debug.LogError("Knetik Labs SDK: JSON Request: " + reg_request);
                    return false;
            }

            m_session = jsonDict["result"]["session"];
			return true;
		}		
	}
}

