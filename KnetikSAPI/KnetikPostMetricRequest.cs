using System;
using KnetikSimpleJSON;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

// NOTE: Metrics (metric_id) must first be created in the Admin Panel before posting values

namespace Knetik
{
	public class KnetikPostMetricRequest : KnetikApiRequest
	{
		private string metric_request = null;
		int m_metricId;
		long m_metricValue;

		// Set up a metric with no level
		public KnetikPostMetricRequest (string api_key, int metric_id, long metric_value)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_metricId = metric_id;
			m_metricValue = metric_value;
			m_method = "put";
		}

		// Metric with level
		public KnetikPostMetricRequest (string api_key, int metric_id, long metric_value, int level_id)
		{
			m_key = api_key;
			m_clientSecret = KnetikApiUtil.API_CLIENT_SECRET;
			m_metricId = metric_id;
			m_metricValue = metric_value;
			m_levelId = level_id;
			m_method = "put";
		}

		// Build JSON for non-level metric
		string setMetricData()
		{
			metric_request = "{";
			metric_request += "\"metrics\": ";
			metric_request +=   "[";
			metric_request +=     "{";
			metric_request +=        "\"metric_id\": " + m_metricId + "";
			metric_request +=        ",";
			metric_request +=        "\"metric_data\": " + m_metricValue + "";
			metric_request +=     "}";
			metric_request +=    "]";
			metric_request += "}";
			return metric_request;    
		}

		// Build JSON for level-based metric
		string setMetricLevelData()
		{
			metric_request = "{";
			metric_request += "\"metrics\": ";
			metric_request +=   "[";
			metric_request +=     "{";
			metric_request +=        "\"metric_id\": " + m_metricId + "";
			metric_request +=        ",";
			metric_request +=        "\"metric_data\": " + m_metricValue + "";
			metric_request +=		",";
			metric_request +=		"\"level_id\": " + m_levelId + "";
			metric_request +=     "}";
			metric_request +=    "]";
			metric_request += "}";
			return metric_request;    
		}
		
		// Send metric information to server
		public bool doPostMetric()
		{
			KnetikJSONNode jsonDict = null;
		
			m_url = KnetikApiUtil.API_URL + KnetikApiUtil.ENDPOINT_PREFIX + KnetikApiUtil.METRIC_ENDPOINT;

			if (m_levelId == 0)
			{

				if (sendSignedRequest(null, setMetricData(), ref jsonDict) == false) 
				{
					Debug.LogError("Knetik Labs SDK - ERROR 400: Unable to send signed request for metric without level!");
					Debug.LogError("Knetik Labs SDK: JSON Request: " + metric_request);
					return false;
				}

			}

			else if (m_levelId != 0)
			{

				if (sendSignedRequest(null, setMetricLevelData(), ref jsonDict) == false) 
				{
					Debug.LogError("Knetik Labs SDK - ERROR 401: Unable to send signed request for metric with level!");
					Debug.LogError("Knetik Labs SDK: JSON Request: " + metric_request);
					return false;
				}

			}
			
		    if (jsonDict["result"] == null) 
			{
				Debug.LogError("Knetik Labs SDK - ERROR 402: No response from server for metric posting!");
				Debug.LogError("Knetik Labs SDK: JSON Request: " + metric_request);
				return false;
		    }
		 
			Debug.Log ("Metric posted successfully");
			return true;
		}		
	}
}

