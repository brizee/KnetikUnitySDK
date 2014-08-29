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

        public void CreateMetric(string name)
        {
            return new Metric (Client, this, name);
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);
            Options = new List<GameOption> ();
            foreach (KnetikJSONNode node in json["user_item_options"]) {
                GameOption option = new GameOption(Client, this);
                option.Deserialize(node);
                Options.Add(option);
            }
        }
    }
}

