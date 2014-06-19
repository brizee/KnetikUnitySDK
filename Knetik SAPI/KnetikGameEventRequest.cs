using System;
using KnetikSimpleJSON;
using UnityEngine;
using System.Collections.Generic;

// Builds and Sends Game Event data to server to be stored in database against a user (denoted by their hash ID)

namespace Knetik
{
	public class KnetikGameEventRequest : KnetikApiRequest
	{
		private string game_request = null;

		public KnetikGameEventRequest (string api_key, string hashId, long score)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			// Note that the hashId is the Game Session ID
			// TODO: The Game Session ID needs to be generated within the Unity SDK (previously done in XCode)
			m_hashId = hashId;
			m_score = score;
		}

		// Build JSON to send Game Event to server
		string getGameEventData()
		{
			game_request = "{";			
			game_request +=        "\"hashId\": \"" + m_hashId + "\"";
			game_request +=        ",";
			game_request += 		"\"score\": \"" + m_score + "\"";
			game_request +=       "}";	
			return game_request;    
		}

		// Send the Game Event Data to the server
		public bool postGameEvent(string mode)
		{
			KnetikJSONNode jsonDict = null;
			string endPoint;

			// Singular game event, e.g. not the end of the game/final result, can be used for authentication/prevent cheating
			if (mode == "event")
			{
				m_method = "put";
				endPoint = "gameevent";
			}

			// Final game result
			else if (mode == "result")
			{
				m_method = "post";
				endPoint = "gameresult";
			}

			else
			{
				Debug.LogError("Knetik Labs SDK - ERROR 100: Mode: " + mode + " is not valid.");
				return false;
			}

			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + endPoint;
			if (sendSignedRequest(null, getGameEventData(), ref jsonDict) == false) {
				Debug.LogError("Knetik Labs SDK - ERROR 101: Unable to send signed request for game event/result!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + game_request);
				return false;
			}
			
			if (jsonDict["result"] == null) {
				Debug.LogError("Knetik Labs SDK - ERROR 102: Game event/result response is null");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + game_request);
				return false;
			}
			Debug.Log ("Send Game Event " + m_method + " Successful!");
			return true;
		}
	}
}

