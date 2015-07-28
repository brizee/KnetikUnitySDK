using System;
using System.Collections;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Consumable : Behavior
    {
        public Consumable(KnetikClient client)
            : base(client) 
        {
        }

        public int MaxUse
        {
            get;
            set;
        }

        public override void Deserialize(KnetikJSONNode json)
        {
            base.Deserialize(json);
            MaxUse = json ["max_use"].AsInt;
        }
    }
}
