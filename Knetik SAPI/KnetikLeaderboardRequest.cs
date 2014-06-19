using System;
using KnetikSimpleJSON;
using UnityEngine;
using System.Collections.Generic;

// Retrieves Leaderboard information by the ID of the leaderboard, which are created in the Admin Panel

namespace Knetik
{
	public class KnetikLeaderboardRequest : KnetikApiRequest
	{
		private string leaderboard_request = null;

		public string leaderboard_id;
		public string copyright;
		public string date_created;
		public string date_updated;
		public string deleted;
		public string description;
		public string developer_id;
		public string image_url;
		public string lang;
		public string languages;
		public string metric_id;
		public string name;
		public string product_description;
		public string product_id;
		public string product_summary;
		public string product_title;
		public string product_translation_id;
		public string publisher_id;
		public string qualifying_value;
		public string rating_id;
		public string size;
		public string sort_style;
		public string update_date;
		public string level_id;
		public string active;
		public string create_date;
		public string level_name;
		public string metric_name;
		public Dictionary<string, string[]> user_results = new Dictionary<string, string[]>();

		public KnetikLeaderboardRequest (string api_key, long leaderboardId)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_leaderboardId = leaderboardId;
			m_method = "get";
		}

		// Build the Leaderboard JSON request
		string getLeaderboardRequest()
		{
			leaderboard_request = "{";
			leaderboard_request += "\"leaderboard_id\":\"" + m_leaderboardId.ToString() + "\"";
			leaderboard_request += "}";
			return leaderboard_request;    
		}

		// Retrieve Leaderboard values
		public bool doGetInfo()
		{
			string postBody = getLeaderboardRequest();
			KnetikJSONNode jsonDict = null;
			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.LEADERBOARD_ENDPOINT;

			if(sendSignedRequest(null, postBody, ref jsonDict) == false) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 200: Unable to send signed request for Leaderboard information!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + leaderboard_request);
				return false;
			}
			
			if (jsonDict["result"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 201: No result found for Leaderboard ID " + leaderboard_id);
				Debug.LogError("Knetik Labs SDK: JSON Request: " + leaderboard_request);
				return false;
			}
			
			leaderboard_id = jsonDict["result"]["leaderboard"]["id"];
			active = jsonDict["result"]["leaderboard"]["active"];
			copyright = jsonDict["result"]["leaderboard"]["copyright"];
			date_created = jsonDict["result"]["leaderboard"]["date_created"];
			date_updated = jsonDict["result"]["leaderboard"]["date_updated"];
			deleted = jsonDict["result"]["leaderboard"]["deleted"];
			developer_id = jsonDict["result"]["leaderboard"]["developer_id"];
			lang = jsonDict["result"]["leaderboard"]["lang"];
			languages = jsonDict["result"]["leaderboard"]["languages"];
			level_id = jsonDict["result"]["leaderboard"]["level_id"];
			level_name = jsonDict["result"]["leaderboard"]["level_name"];
			metric_id = jsonDict["result"]["leaderboard"]["metric_id"];
			metric_name = jsonDict["result"]["leaderboard"]["metric_name"];
			product_description = jsonDict["result"]["leaderboard"]["product_description"];
			product_id = jsonDict["result"]["leaderboard"]["product_id"];
			product_summary = jsonDict["result"]["leaderboard"]["product_summary"];
			product_title = jsonDict["result"]["leaderboard"]["product_title"];
			product_translation_id = jsonDict["result"]["leaderboard"]["product_translation_id"];
			publisher_id = jsonDict["result"]["leaderboard"]["publisher_id"];
			qualifying_value = jsonDict["result"]["leaderboard"]["qualifying_value"];
			rating_id = jsonDict["result"]["leaderboard"]["rating_id"];
			size = jsonDict["result"]["leaderboard"]["size"];
			sort_style = jsonDict["result"]["leaderboard"]["sort_style"];
			update_date = jsonDict["result"]["leaderboard"]["update_date"];
			create_date = jsonDict["result"]["leaderboard"]["create_date"];

			var gameLeaderboards = jsonDict["result"]["gameleaderboards"];
			int gameLeaderboardsCount = gameLeaderboards.Count;

			for(int i = 0; i < gameLeaderboardsCount; i++)
			{
				string userid = gameLeaderboards[i]["userid"];
				string current_score = gameLeaderboards[i]["current_score"];
				string username = gameLeaderboards[i]["username"];
				string avatar_url = gameLeaderboards[i]["avatar_url"];
				string[] results = {current_score, username, avatar_url};
				user_results.Add(userid, results);
			}

			return true;
		}
	}	
}