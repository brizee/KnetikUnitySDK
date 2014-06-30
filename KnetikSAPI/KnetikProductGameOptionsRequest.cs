using System;
using KnetikSimpleJSON;
using UnityEngine;
using System.Collections.Generic;

// Retrieves game options information about a particular product (game) with known Product ID, found on Admin Panel
	
namespace Knetik
{
	public class KnetikProductGameOptionsRequest : KnetikApiRequest
	{
		private string product_request = null;
		public Dictionary<string, string> game_options = new Dictionary<string, string>();
		
		public KnetikProductGameOptionsRequest (string api_key, long productId)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_productId = productId;
		}

		// Build the JSON to search for a particular product and return its game options
		string getProductRequest()
		{
			product_request = "{";
			product_request += "\"search\": [{\"product_id\":\"" + m_productId.ToString() + "\"}]";
			product_request += ",";
			product_request += "\"game_options\": \"true\"";
			product_request += ",";
			product_request += "\"fields\": ";
			product_request += "[\"game_options\"";
			product_request += "]";
			product_request += "}";
			return product_request;    
		}

		// Retrieves the product's game options
		public bool doGetInfo()
		{
			string postBody = getProductRequest();

			KnetikJSONNode jsonDict = null;
			
			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.PRODUCT_ENDPOINT;
			
			if(sendSignedRequest(null, postBody, ref jsonDict) == false) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 600: Unable to send signed request to server for Product Game Options!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + product_request);
				return false;
			}

			if (jsonDict["result"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 601: No response from server for Product Game Options request!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + product_request);
				return false;
			}
			
			/* If passing productId string, check for game_options in json
			* Note that game_options will be returned as a Dictionary<string, string>
			* for the purposes of lookups and parsing option name/option value pairs
			*/
			if (m_productId != 0)
			{
				var items = jsonDict["result"]["items"];
				var options = items[0]["game_options"];
				
				int item_count = items.Count;

				// Only 1 product should return
				if(item_count == 1)
				{
					int game_option_count = options.Count;

					for(int i = 0; i < game_option_count; i++)
					{
						string option_name = options[i]["name"];
						string option_value = options[i]["value"];
						game_options.Add(option_name, option_value);
					}

				}
			}

			return true;
		}
	}
}

	