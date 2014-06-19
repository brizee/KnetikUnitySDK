using System;
using KnetikSimpleJSON;
using UnityEngine;
using System.Collections.Generic;

// Creates/Updates user options on a per game/product basis, must know the Product ID from the Admin Panel

namespace Knetik
{
	public class KnetikPostUserOptionsRequest : KnetikApiRequest
	{
		private string user_request = null;
		string m_deleted = "0";

		public KnetikPostUserOptionsRequest (string api_key, long userId, long productId, string optionName, string optionValue, string deleted = "0")
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_userId = userId;
			m_productId = productId;
			m_gameOptionName = optionName;
			m_gameOptionValue = optionValue;
			m_method = "put";
			m_deleted = deleted;
		}

		// Builds the JSON to send user options on a product, for either creation or update
		string buildUserData(string mode)
		{
			user_request = "{";			
			user_request +=        "\"user_id\": " + m_userId + "";
			user_request +=        ",";
			user_request += 		"\"user_info\": ";
			user_request += 			"{\"" + mode + "\":{";
			user_request +=					"\"product_id\": \"" + m_productId.ToString() + "\"";
			user_request +=        			",";
			user_request +=					"\"name\": \"" + m_gameOptionName + "\"";
			user_request +=        			",";
			user_request +=					"\"value\": \"" + m_gameOptionValue + "\"";
			user_request +=        			",";
			user_request +=					"\"deleted\": \"" + m_deleted + "\"";
			user_request += 			"}}";
			user_request +=     "}";			
			return user_request;    
		}

		// Sends the JSON to the server to create or update a user option
		public bool postUserInfo(string mode)
		{
			KnetikJSONNode jsonDict = null;
			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.USER_ENDPOINT;
			string modeChoice = null;

			if (mode == "insert")
			{
				modeChoice = "insertUserGameOptions";
			}

			else if (mode == "update")
			{
				modeChoice = "updateUserGameOptions";
			}
			else
			{
				Debug.LogError("Knetik Labs SDK - ERROR 502: Mode " + modeChoice + " is not a valid method for user option");
				return false;
			}

			if (sendSignedRequest(null, buildUserData(modeChoice), ref jsonDict) == false) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 500: Unable to send signed request for user option!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + user_request);
				return false;
			}
			
			if (jsonDict["result"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 501: No response from server to send user option!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + user_request);
				return false;
			}

			Debug.Log ("Successfully sent user option creation/update to server");
			return true;
		}
	}
}
	
