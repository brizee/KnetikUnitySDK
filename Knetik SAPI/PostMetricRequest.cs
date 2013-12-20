using System;
using SimpleJSON;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Knetik
{
	public class PostMetricRequest : ApiRequest
	{
		int m_metricId;
		int m_metricValue;
		
		public PostMetricRequest (string api_key, int metric_id, int metric_value)
		{
			m_Key = api_key;
			m_clientSecret = ApiUtil.API_CLIENT_SECRET;
			
			m_metricId = metric_id;
			m_metricValue = metric_value;
			
			m_method = "put";
		}

		string getMetricData()
		{
			string login_request = "{";
			
			login_request += "\"metrics\": ";
			login_request +=   "[";
			
			login_request +=     "{";
			login_request +=        "\"metric_id\": " + m_metricId + "";
			login_request +=        ",";
			login_request +=        "\"metric_data\": " + m_metricValue + "";
			login_request +=     "}";
			
			login_request +=    "]";
			login_request += "}";
		
			return login_request;    
		}
		
		
		public bool doPostMetric()
		{
			JSONNode jsonDict = null;
		
			m_url = ApiUtil.API_URL + "/rest/api/latest/metric";

			if (sendSignedRequest(null, getMetricData(), ref jsonDict) == false) {
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

