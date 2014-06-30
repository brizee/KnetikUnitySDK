using System;
using KnetikSimpleJSON;
using UnityEngine;

// Sends request to server to register a new user

namespace Knetik
{
	public class KnetikRegisterRequest : KnetikApiRequest
	{
		private string reg_request = null;
		string m_username;
		string m_password;
		string m_email;
		string m_fullname;
		
		public KnetikRegisterRequest (string api_key, string username, string password, string email, string fullname)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_username = username;
			m_password = password;
			m_email = email;
			m_fullname = fullname;
			
			m_method = "put";
		}

		// Build JSON to register a new user
		string getRegisterData()
		{
			reg_request = "{";
			reg_request += "\"user_info\": ";
			reg_request +=     "{";
			reg_request +=        "\"email\": \"" + m_email + "\"";
			reg_request +=        ",";
			reg_request +=        "\"password\": \"" + KnetikApiUtil.sha1(m_password) + "\"";
			reg_request +=        ",";
			reg_request +=        "\"username\": \"" + m_username + "\"";
			reg_request +=        ",";
			reg_request +=        "\"fullname\": \"" + m_fullname + "\"";
			reg_request +=     "}";
			reg_request += "}";
			return reg_request;    
		}
		
		// Send JSON to register the new user with the server
		public bool doRegister()
		{
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

