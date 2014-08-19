using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knetik
{
    public partial class KnetikClient
	{
        public KnetikApiResponse RecordValueMetric(int metricId, int value, string level = null, Action<KnetikApiResponse> cb = null)
		{
            Debug.Log ("Recording value metric " + metricId);
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("metric_id", metricId);
            j.AddField ("value", value);

            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(RecordMetricEndpoint, body);
            
            KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
            return  registerResponse;
		}

        public KnetikApiResponse RecordObjectMetric(int metricId, Dictionary<String, String> obj, string level = null, Action<KnetikApiResponse> cb = null)
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("metric_id", metricId);

            var jobj = JSONObject.Create (obj);
            j.AddField ("object", jobj.Print());
            
            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(RecordMetricEndpoint, body);
            
            KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
            return  registerResponse;
        }

        public KnetikApiResponse GetLeaderboard(int leaderboardId, string level = null, Action<KnetikApiResponse> cb = null)
        {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("leaderboard_id", leaderboardId);

            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(GetLeaderboardEndpoint, body);
            
            KnetikApiResponse registerResponse = new KnetikApiResponse(this, req, cb);
            return  registerResponse;
        }
	}
}

