using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
	public class UserOption : Option
	{
        public UserOption (KnetikClient client, Game game)
            : base(client, game)
        {
        }

        public void Save(Action<KnetikApiResponse> cb)
        {
            if (ID == -1) {
                Client.CreateUserGameOption(Game.ID, Key, Value, cb);
            } else {
                Client.UpdateUserGameOption(Game.ID, Key, Value, cb);
            }
        }
	}
}

