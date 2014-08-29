using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Metric : KnetikModel
    {
        public int ID {
            get;
            set;
        }

        public Game Game {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public string Value {
            get;
            set;
        }

        public Metric (KnetikClient client, int id)
            : base(client)
        {
            ID = id;
        }

        public Metric (KnetikClient client, Game game, string name)
            : base(client)
        {
            Game = game;
            Name = name;
        }

        public void Save(Action<KnetikApiResponse> res)
        {

        }
    }
}

