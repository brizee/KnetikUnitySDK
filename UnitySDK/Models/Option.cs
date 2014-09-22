using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Option : KnetikModel
    {
        public Game Game {
            get;
            set;
        }

        public int ID {
            get;
            set;
        }

        public string Key {
            get;
            set;
        }

        public string Value {
            get;
            set;
        }

        public Option (KnetikClient client, Game game)
            : base(client)
        {
            Game = game;
            ID = -1;
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            ID = json ["id"].AsInt;
            Key = json ["keye"].Value;
            Value = json ["value"].Value;
        }
    }
}

