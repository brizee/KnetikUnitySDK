using System;
using System.Collections;
using KnetikSimpleJSON;

namespace Knetik {
    public class Behavior : KnetikModel {
        public static Behavior Parse(KnetikClient client, KnetikJSONNode json)
        {
            Behavior behavior;

            string typeHint = json ["type_hint"].Value;

            switch (typeHint)
            {
                case "consumable":
                    behavior = new Consumable(client);
                    break;
                default:
                    behavior = new Behavior(client);
                    break;
            }

            behavior.Deserialize(json);

            return behavior;
        }

        public string TypeHint
        {
            get;
            set;
        }

        public Behavior(KnetikClient client)
            : base(client) 
        {
        }

        public override void Deserialize(KnetikJSONNode json)
        {
            TypeHint = json ["type_hint"].Value;
        }
    }
}
