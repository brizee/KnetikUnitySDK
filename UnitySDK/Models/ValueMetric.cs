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
        
        public override void Save(Action<KnetikApiResponse> cb)
        {
            if (ID != -1) {
                Client.RecordValueMetric(ID, Value, Level, cb);
            } else if (Game != null && Name != null) {
                Client.RecordValueMetric(Game.ID, Name, Value, Level, cb);
            }
        }
    }
}
