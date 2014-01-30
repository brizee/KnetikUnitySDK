using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

namespace Knetik
{
	public class UserInfoRequest : ApiRequest
	{
		public string id;
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

		public UserInfoRequest (string api_key)
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;
		}

		public UserInfoRequest (string api_key, int productId)
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;
			m_productId = productId;
		}

		public UserInfoRequest (string api_key, string userId, string avatarUrl, string lang)
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;
			m_userId = userId;
			m_avatarUrl = avatarUrl;
			m_lang = lang;
			m_method = "put";
			Debug.Log ("UserInfoRequest With Avatar URL " + avatarUrl + " And Lang " + lang);
		}
		
		public bool doGetInfo()
		{
			JSONNode jsonDict = null;
		
			m_url = ApiUtil.API_URL + "/rest/api/latest/user";

			if (sendSignedRequest(ref jsonDict) == false) {
				return false;
			}
			
		    if (jsonDict["result"] == null) {
				return false;
		    }

			Debug.Log("User Result: " + jsonDict["result"].ToString());
		    
			id = jsonDict["result"]["id"];
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
							user_options.Add(option_name, option_value);
						}
					}
				}

//				foreach (string key in user_options.Keys)
//				{
//					Debug.Log ("User Options Key: " + key + ", User Options Value: " + user_options[key]);
//				}
			}

			return true;
		}

		string setUserAvatar()
		{
			string user_request = "{";			
			user_request +=        "\"user_id\": " + m_userId + "";
			user_request +=        ",";
			user_request += 		"\"user_info\": ";
			user_request += 			"{";
			user_request +=					"\"avatar_url\": \"" + m_avatarUrl + "\"";
			user_request += 			"}";
			user_request +=     "}";	
			
			Debug.Log ("User Avatar Request Put: " + user_request);
			
			return user_request;    
		}

		string setUserLang()
		{
			string user_request = "{";			
			user_request +=        "\"user_id\": " + m_userId + "";
			user_request +=        ",";
			user_request += 		"\"user_info\": ";
			user_request += 			"{";
			user_request +=					"\"lang\": \"" + m_lang + "\"";
			user_request += 			"}";
			user_request +=     "}";	
			
			Debug.Log ("User Lang Request Put: " + user_request);
			
			return user_request;    
		}

		public bool putUserInfo(string mode)
		{
			JSONNode jsonDict = null;
			m_url = ApiUtil.API_URL + "/rest/api/latest/user";

			if (mode == "avatar" && m_avatarUrl != null)
			{
				Debug.Log("Setting user avatar to " + m_avatarUrl);
				if (sendSignedRequest(null, setUserAvatar(), ref jsonDict) == false) {
					Debug.Log("sendSignedRequest failed");
					return false;
				}
			}
			else if (mode == "lang" && m_lang != null)
			{
				if (sendSignedRequest(null, setUserLang(), ref jsonDict) == false) {
					Debug.Log("sendSignedRequest failed");
					return false;
				}
			}
			
			if (jsonDict["result"] == null) {
				Debug.Log("result is null");
				return false;
			}
			Debug.Log ("User Info Put Successful!");
			return true;
		}
	}	
}