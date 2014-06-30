using System;
using KnetikSimpleJSON;
using UnityEngine;
using System.Collections.Generic;

namespace Knetik
{
	public class KnetikUserInfoRequest : KnetikApiRequest
	{
		private string user_request = null;

		public int id;
		public string email;
		public string username;
		public string fullname;
		public string mobile_number;
		public string money_balance;
		public string coin_balance;
		public string avatar_url;
		public string level;
		public string experience;
		public string first_name;
		public string last_name;
		public string token;
		public string gender;
		public string lang;
		public string country;
		public Dictionary<string, string> user_options = new Dictionary<string, string>();

		public KnetikUserInfoRequest (string api_key)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
		}

		public KnetikUserInfoRequest (string api_key, long productId)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_productId = productId;
		}

		public KnetikUserInfoRequest (string api_key, int userId, string avatarUrl, string lang)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_userId = userId;
			m_avatarUrl = avatarUrl;
			m_lang = lang;
			m_method = "put";
		}
		
		public bool doGetInfo()
		{
			KnetikJSONNode jsonDict = null;
		
			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.USER_ENDPOINT;

			if (sendSignedRequest(ref jsonDict) == false) 
			{
				Debug.Log("Knetik Labs SDK - ERROR 800: Unable to send a signed request for user information");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + user_request);
				return false;
			}
			
		    if (jsonDict["result"] == null) 
			{
				Debug.Log("Knetik Labs SDK - ERROR 801: Response from SAPI to get user information is null");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + user_request);
				return false;
		    }

			Debug.Log("Knetik Labs SDK - User information successfully retrieved.");
			id = jsonDict["result"]["id"].AsInt;
			email = jsonDict["result"]["email"];
			username = jsonDict["result"]["username"];
			fullname = jsonDict["result"]["fullname"];
			mobile_number = jsonDict["result"]["mobile_number"];
			money_balance = jsonDict["result"]["money_balance"];
			coin_balance = jsonDict["result"]["coin_balance"];
			avatar_url = jsonDict["result"]["avatar_url"];
			level = jsonDict["result"]["level"];
			experience = jsonDict["result"]["experience"];
			first_name = jsonDict["result"]["first_name"];
			last_name = jsonDict["result"]["last_name"];
			token = jsonDict["result"]["token"];
			gender = jsonDict["result"]["gender"];
			lang = jsonDict["result"]["lang"];
			country = jsonDict["result"]["country"];

			/* If passing productId string, check for userGameOptions in json
			* Note that user_options will be returned as a Dictionary<string, string>
			* for the purposes of lookups and parsing option name/option value pairs
			*/
			if(m_productId != 0)
			{
				var purchase_history = jsonDict["result"]["purchase_history"];

				int purchase_history_count = purchase_history.Count;
				for(int j = 0; j < purchase_history_count; j++)
				{
					string product_id = purchase_history[j]["product_id"];

					if (product_id.Equals(m_productId.ToString()))
					{
						var options = purchase_history[j]["userGameOptions"];
						int user_game_option_count = options.Count;
						for(int i = 0; i < user_game_option_count; i++)
						{
							string option_name = options[i]["name"];
							string option_value = options[i]["value"];
							if (!user_options.ContainsKey(option_name))
							{
								user_options.Add(option_name, option_value);
							}
						}
					}
				}

			}

			return true;
		}

		string setUserAvatar()
		{
			user_request = "{";			
			user_request +=        "\"user_id\": " + m_userId + "";
			user_request +=        ",";
			user_request += 		"\"user_info\": ";
			user_request += 			"{";
			user_request +=					"\"avatar_url\": \"" + m_avatarUrl + "\"";
			user_request += 			"}";
			user_request +=     "}";	
			return user_request;    
		}

		string setUserLang()
		{
			user_request = "{";			
			user_request +=        "\"user_id\": " + m_userId + "";
			user_request +=        ",";
			user_request += 		"\"user_info\": ";
			user_request += 			"{";
			user_request +=					"\"lang\": \"" + m_lang + "\"";
			user_request += 			"}";
			user_request +=     "}";	
			return user_request;    
		}

		public bool putUserInfo(string mode)
		{
			KnetikJSONNode jsonDict = null;
			m_url = KnetikApiUtil.API_URL + "/rest/api/latest/user";

			if (mode == "avatar" && m_avatarUrl != null)
			{
				if (sendSignedRequest(null, setUserAvatar(), ref jsonDict) == false) {
					Debug.Log("Knetik Labs SDK - ERROR 802: Unable to send a signed request for user avatar update");
					Debug.LogError("Knetik Labs SDK: JSON Request: " + user_request);
					return false;
				}
			}

			else if (mode == "lang" && m_lang != null)
			{
				if (sendSignedRequest(null, setUserLang(), ref jsonDict) == false) {
					Debug.Log("Knetik Labs SDK - ERROR 803: Unable to send a signed request for user language update");
					Debug.LogError("Knetik Labs SDK: JSON Request: " + user_request);
					return false;
				}
			}

			else if (mode != "avatar" && mode != "lang")
			{
				Debug.Log("Knetik Labs SDK - ERROR 806: Invalid mode " + mode + " for user update");
				return false;
			}
			
			if (jsonDict["result"] == null) 
			{
				Debug.Log("Knetik Labs SDK - ERROR 805: User update response from SAPI is null");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + user_request);
				return false;
			}

			Debug.Log ("Knetik Labs SDK - User Info Put Successful!");
			return true;
		}
	}	
}