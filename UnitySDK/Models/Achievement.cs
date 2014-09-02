using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Achievement : Item
	{
        public int Value {
            get;
            set;
        }

		public Achievement (KnetikClient client, int id)
            : base(client, id)
		{
		}

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);

            Value = json["value"].AsInt;
        }
	}
}

