using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;
	
	namespace Knetik
	{
		public class ProductInfoRequest : ApiRequest
		{
			public Dictionary<string, string> game_options = new Dictionary<string, string>();
			
			public ProductInfoRequest (string api_key, string productId)
			{
				m_Key = api_key;
				m_clientSecret = ApiUtil.API_CLIENT_SECRET;
				m_productId = productId;
			}

			string getProductRequest()
			{
				string product_request = "{";
				product_request += "\"search\": [{\"product_id\":\"" + m_productId + "\"}]";
				product_request += ",";
				product_request += "\"game_options\": \"true\"";
				product_request += ",";
				product_request += "\"fields\": ";
				product_request += "[\"game_options\"";
				product_request += "]";
				product_request += "}";

				Debug.Log ("Product Request String: " + product_request);
				return product_request;    
			}
			
			public bool doGetInfo()
			{
				string postBody = getProductRequest();

				JSONNode jsonDict = null;
				
				m_url = ApiUtil.API_URL + "/rest/api/latest/product";
				
				if(sendSignedRequest(null, postBody, ref jsonDict) == false) {
					return false;
				}

				if (jsonDict["result"] == null) {
					return false;
				}
				
				//Debug.Log("Product Result: " + jsonDict["result"].ToString());

				/* If passing productId string, check for game_options in json
				* Note that game_options will be returned as a Dictionary<string, string>
				* for the purposes of lookups and parsing option name/option value pairs
				*/
				if (m_productId != null)
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
	//			foreach (string key in game_options.Keys)
	//			{
	//				Debug.Log ("Game Options Key: " + key + ", Game Options Value: " + game_options[key]);
	//			}
				
				return true;
			}
		}
	}
	
	