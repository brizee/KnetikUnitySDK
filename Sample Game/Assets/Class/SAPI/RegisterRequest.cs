using System;
using SimpleJSON;
using UnityEngine;

namespace Knetik
{
	public class RegisterRequest : ApiRequest
	{
		string m_username;
		string m_password;
		string m_email;
		string m_fullname;
		
		public RegisterRequest (string api_key, string u, string p, string e, string f)
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;

			m_username = u;
			m_password = p;
			m_email = e;
			m_fullname = f;
			
			m_method = "put";
		}

		string getRegisterData()
		{
			string reg_request = "{";
			
			reg_request += "\"user_info\": ";
			
			reg_request +=     "{";
			reg_request +=        "\"email\": \"" + m_email + "\"";
			reg_request +=        ",";
			reg_request +=        "\"password\": \"" + ApiUtil.sha1(m_password) + "\"";
			reg_request +=        ",";
			reg_request +=        "\"username\": \"" + m_username + "\"";
			reg_request +=        ",";
			reg_request +=        "\"fullname\": \"" + m_fullname + "\"";
			reg_request +=     "}";
			
			reg_request += "}";
		
			return reg_request;    
		}
		
		
		public bool doRegister()
		{
			JSONNode jsonDict = null;
		
			m_url = ApiUtil.API_URL + "/rest/api/latest/user";
			Debug.Log("doRegister 1, url: " + m_url);
			if (sendSignedRequest(null, getRegisterData(), ref jsonDict) == false) {
				return false;
			}
			
			Debug.Log("doRegister 2");
		    if (jsonDict["result"] == null) {
				return false;
		    }
		 
			return true;
		}		
	}
}

