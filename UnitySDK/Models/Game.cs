using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Game : Item
    {
        public List<GameOption> Options {
            get;
            set;
        }

        public Game (KnetikClient client, int id)
            : base(client, id)
        {
        }

        public ValueMetric CreateValueMetric(string name)
        {
            return new ValueMetric (Client, this, name);
        }

        public ObjectMetric CreateObjectMetric(string name)
        {
            return new ObjectMetric (Client, this, name);
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);
            Options = new List<GameOption> ();
            foreach (KnetikJSONNode node in json["user_item_options"].Children) {
                GameOption option = new GameOption(Client, this);
                option.Deserialize(node);
                Options.Add(option);
            }
        }
    }
}

