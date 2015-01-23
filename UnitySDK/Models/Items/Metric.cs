using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public abstract class Metric : KnetikModel
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

        public string Level {
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
            ID = -1;
            Game = game;
            Name = name;
        }

        public abstract void Save(Action<KnetikApiResponse> cb);
    }
}

