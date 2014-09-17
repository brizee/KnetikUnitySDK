using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class GameOption : KnetikModel
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

        public GameOption (KnetikClient client, Game game)
            : base(client)
        {
            Game = game;
            ID = -1;
        }

        public void Save(Action<KnetikApiResponse> cb)
        {
            if (ID == -1) {
                Client.CreateGameOption(Game.ID, Key, Value, cb);
            } else {
                Client.UpdateGameOption(Game.ID, Key, Value, cb);
            }
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            ID = json ["id"].AsInt;
            Key = json ["keye"].Value;
            Value = json ["value"].Value;
        }
    }
}

