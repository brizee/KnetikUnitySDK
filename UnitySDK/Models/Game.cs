using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Game : Item
    {
        public Dictionary<string, GameOption> GameOptions {
            get;
            set;
        }

        public Dictionary<string, UserOption> UserOptions {
            get;
            set;
        }

        public Game (KnetikClient client, int id)
            : base(client, id)
        {
            GameOptions = new Dictionary<string, GameOption> ();
            UserOptions = new Dictionary<string, UserOption> ();
        }

        public ValueMetric CreateValueMetric(string name)
        {
            return new ValueMetric (Client, this, name);
        }

        public ObjectMetric CreateObjectMetric(string name)
        {
            return new ObjectMetric (Client, this, name);
        }

        public UserOption CreateUserOption(string key, string value = null)
        {
            UserOption option = new UserOption (Client, this);
            option.Key = key;
            option.Value = value;
            UserOptions [key] = option;
            return option;
        }

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);
            UserOptions.Clear ();
            foreach (KnetikJSONNode node in json["userItemOptions"].Children) {
                UserOption option = new UserOption(Client, this);
                option.Deserialize(node);
                UserOptions.Add(option.Key, option);
            }

            GameOptions.Clear ();
            foreach (KnetikJSONNode node in json["itemOptions"].Children) {
                GameOption option = new GameOption(Client, this);
                option.Deserialize(node);
                GameOptions.Add(option.Key, option);
            }
        }
    }
}

