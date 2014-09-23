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
                Client.CreateUserGameOption(Game.ID, Key, Value, (res) => {
                    if (res.IsSuccess) {
                        // API doesn't return an ID, but we're persisted now so
                        // we dont want to keep calling Create endpoint
                        ID = 0;
                    }
                    cb(res);
                });
            } else {
                Client.UpdateUserGameOption(Game.ID, Key, Value, cb);
            }
        }
	}
}

