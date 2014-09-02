using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knetik
{
    public partial class KnetikClient
	{
        public ValueMetric CreateValueMetric(int metricId)
        {
            return new ValueMetric (this, metricId);
        }

        public ObjectMetric CreateObjectMetric(int metricId)
        {
            return new ObjectMetric (this, metricId);
        }

        public KnetikApiResponse RecordValueMetric(
            int gameId,
            string metricName,
            float value,
            string level = null,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("item_id", gameId);
            j.AddField ("metric_name", metricName);
            j.AddField ("value", value);
            
            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(RecordMetricEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        public KnetikApiResponse RecordValueMetric(
            int metricId,
            float value,
            string level = null,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("metric_id", metricId);
            j.AddField ("value", value);

            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(RecordMetricEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
		}

        public KnetikApiResponse RecordObjectMetric(
            int gameId,
            string metricName,
            Dictionary<String, String> obj,
            string level = null,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("item_id", gameId);
            j.AddField ("metric_name", metricName);

            j.AddField ("object", JSONObject.Create (obj));
            
            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(RecordMetricEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        public KnetikApiResponse RecordObjectMetric(
            int metricId,
            Dictionary<String, String> obj,
            string level = null,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("metric_id", metricId);

            var jobj = JSONObject.Create (obj);
            j.AddField ("object", jobj.Print());
            
            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(RecordMetricEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }

        public KnetikApiResponse GetLeaderboard(
            int leaderboardId,
            string level = null,
            Action<KnetikApiResponse> cb = null
        ) {
            JSONObject j = new JSONObject (JSONObject.Type.OBJECT);
            j.AddField ("leaderboard_id", leaderboardId);

            if (level != null) {
                j.AddField ("level", level);
            }
            
            String body = j.Print ();
            
            KnetikRequest req = CreateRequest(GetLeaderboardEndpoint, body);
            
            KnetikApiResponse response = new KnetikApiResponse(this, req, cb);
            return  response;
        }
	}
}

