using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class ObjectMetric : Metric
    {
        public Dictionary<string, string> Value {
            get;
            set;
        }

        public ObjectMetric (KnetikClient client, int id)
        : base(client, id)
        {
        }
        
        public ObjectMetric (KnetikClient client, Game game, string name)
        : base(client, game, name)
        {
        }
        
        public override KnetikApiResponse Save(Action<KnetikApiResponse> cb = null)
        {
            if (ID != -1)
            {
                return Client.RecordObjectMetric(ID, Value, Level, cb);
            } else if (Game != null && Name != null)
            {
                return Client.RecordObjectMetric(Game.ID, Value, Level, cb);
            }
            return null;
        }
    }
}
