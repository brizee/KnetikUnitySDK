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

        public KnetikApiResponse Save(Action<KnetikApiResponse> cb = null)
        {
            if (ID == -1)
            {
                if (cb != null)
                {
                    Client.CreateUserGameOption(Game.ID, Key, Value, (res) => {
                        cb(OnSave(res));
                    });
                    return null;
                } else {
                    return OnSave(Client.CreateUserGameOption(Game.ID, Key, Value));
                }
            } else {
                return Client.UpdateUserGameOption(Game.ID, Key, Value, cb);
            }
        }

        private KnetikApiResponse OnSave(KnetikApiResponse res)
        {
            if (res.IsSuccess) {
                // API doesn't return an ID, but we're persisted now so
                // we dont want to keep calling Create endpoint
                ID = 0;
            }
            return res;
        }
	}
}

