using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

namespace Knetik
{
	public class GameEventRequest : ApiRequest
	{
		public GameEventRequest (string api_key, string hashId, string score)
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;
			m_hashId = hashId;
			m_score = score;
		}
		
		string getGameEventData()
		{
			string game_request = "{";			
			game_request +=        "\"hashId\": \"" + m_hashId + "\"";
			game_request +=        ",";
			game_request += 		"\"score\": \"" + m_score + "\"";
			game_request +=       "}";	
			
			Debug.Log ("Game Event Request Put: " + game_request);
			
			return game_request;    
		}
		
		public bool postGameEvent(string mode)
		{
			JSONNode jsonDict = null;
			string endPoint;
			if (mode == "event")
			{
				m_method = "put";
				endPoint = "gameevent";
			}
			else if (mode == "result")
			{
				m_method = "post";
				endPoint = "gameresult";
			}
			else
			{
				Debug.Log ("Mode: " + mode + " is not valid.");
				return false;
			}

			m_url = ApiUtil.API_URL + "/rest/api/latest/" + endPoint;
			if (sendSignedRequest(null, getGameEventData(), ref jsonDict) == false) {
				Debug.Log("sendSignedRequest failed");
				return false;
			}
			
			if (jsonDict["result"] == null) {
				Debug.Log("result is null");
				return false;
			}
			return true;
		}
	}
}

