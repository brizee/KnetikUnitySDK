using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

namespace Knetik
{
	public class PostUserOptionsRequest : ApiRequest
	{
		string m_userId;
		string m_deleted = "0";
		public PostUserOptionsRequest (string api_key, string userId, string productId, string optionName, string optionValue, string deleted = "0")
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;
			m_userId = userId;
			m_productId = productId;
			m_optionName = optionName;
			m_optionValue = optionValue;
			m_method = "put";
			m_deleted = deleted;
		}
		
		string getUserData(string mode)
		{
			string user_request = "{";			
			user_request +=        "\"user_id\": " + m_userId + "";
			user_request +=        ",";
			user_request += 		"\"user_info\": ";
			user_request += 			"{\"" + mode + "\":{";
			user_request +=					"\"product_id\": \"" + m_productId + "\"";
			user_request +=        			",";
			user_request +=					"\"name\": \"" + m_optionName + "\"";
			user_request +=        			",";
			user_request +=					"\"value\": \"" + m_optionValue + "\"";
			user_request +=        			",";
			user_request +=					"\"deleted\": \"" + m_deleted + "\"";
			user_request += 			"}}";
			user_request +=     "}";	
			
			Debug.Log ("User Options Request Put: " + user_request);
			
			return user_request;    
		}
		
		public bool postUserInfo(string mode)
		{
			JSONNode jsonDict = null;
			
			m_url = ApiUtil.API_URL + "/rest/api/latest/user";
			string modeChoice = null;
			if (mode == "insert")
			{
				modeChoice = "insertUserGameOptions";
			}
			else if (mode == "update")
			{
				modeChoice = "updateUserGameOptions";
			}
			if (sendSignedRequest(null, getUserData(modeChoice), ref jsonDict) == false) {
				Debug.Log("sendSignedRequest failed");
				return false;
			}
			
			if (jsonDict["result"] == null) {
				Debug.Log("result is null");
				return false;
			}
			Debug.Log ("User Option Put Successful!");
			return true;
		}
	}
}
	
