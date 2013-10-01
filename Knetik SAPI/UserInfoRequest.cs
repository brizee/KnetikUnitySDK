using System;
using SimpleJSON;

namespace Knetik
{
	public class UserInfoRequest : ApiRequest
	{
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

		public UserInfoRequest (string api_key)
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;
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
				
			return true;
		}
	}
}

