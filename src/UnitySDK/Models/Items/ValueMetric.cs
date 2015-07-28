using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class ValueMetric : Metric
    {
        public float Value {
            get;
            set;
        }

        public ValueMetric (KnetikClient client, int id)
        : base(client, id)
        {
        }
        
        public ValueMetric (KnetikClient client, Game game, string name)
        : base(client, game, name)
        {
        }
        
        public override KnetikApiResponse Save(Action<KnetikApiResponse> cb = null)
        {
            if (ID != -1)
            {
                return Client.RecordValueMetric(ID, Value, Level, cb);
            } else if (Game != null && Name != null)
            {
                return Client.RecordValueMetric(Game.ID, Name, Value, Level, cb);
            }
            return null;
        }
    }
}
