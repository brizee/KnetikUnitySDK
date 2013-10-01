using System;
using SimpleJSON;
using UnityEngine;

using System.Collections;

namespace Knetik
{
	public class LoginRequest : ApiRequest
	{
		private string username;
		private string password;
		private string client_key;
		
		private string m_key;
		private int m_userId;
		private string m_username;

		public LoginRequest ()
		{
			client_key = ApiUtil.API_CLIENT_KEY;
			username = null;
			password = null;
		}
		
		public LoginRequest (string u, string p)
		{
			client_key = ApiUtil.API_CLIENT_KEY;
			username = u;
			password = p;
		}

		string getLoginSessionRequest(bool isGuest)
		{
			string login_request = "{";
			
			login_request += "\"guest\": " + (isGuest ? "true" : "false");
			login_request += ",";
			login_request += "\"signature\": \"" + ApiUtil.getDeviceSignature() + "\"";
			login_request += ",";
			login_request += "\"serial\": \"" + ApiUtil.getDeviceSerial() + "\"";
			login_request += ",";
			login_request += "\"client_key\": \"" + client_key + "\"";
			
			if (!isGuest) {
				login_request += ",";
				login_request += "\"email\": \"" + username + "\"";
				login_request += ",";
				login_request += "\"password\": \"" + ApiUtil.sha1(password) + "\"";
			}
			login_request += "}";
	
			return login_request;    
		}
		
		public string getKey() {
			return m_key;
		}
		
		public int getUserId() {
			return m_userId;
		}
		
		public string getUsername() {
			return m_username;
		}

		public bool doLoginAsGuest()
		{
			string request_str = getLoginSessionRequest(true);
			JSONNode jsonDict = null;
		
			m_url = ApiUtil.API_URL + "/rest/auth/latest/session";

			if (sendApiRequest(request_str, ref jsonDict) == false) {
				return false;
			}
			
		    if (jsonDict["result"] == null) {
				return false;
		    }
		    
		    if (jsonDict["result"]["key"] == null) {
				return false;
		    }

			m_key = jsonDict["result"]["key"].Value;
			
			return true;
		}
		
		public bool doLogin()
		{
			string request_str = getLoginSessionRequest(false);
			JSONNode jsonDict = null;
		
			m_url = ApiUtil.API_URL + "/rest/auth/latest/session";

			if (sendApiRequest(request_str, ref jsonDict) == false) {
				return false;
			}
			
		    if (jsonDict["result"] == null) {
				return false;
		    }
		    
		    if (jsonDict["result"]["key"] == null) {
				return false;
		    }

			m_key = jsonDict["result"]["key"].Value;
			m_userId = jsonDict["result"]["user_id"].AsInt;
			m_username = username;
			
			return true;
		}
	}
}

