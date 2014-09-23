using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
	public class GameOption : Option
	{
        public GameOption (KnetikClient client, Game game)
            : base(client, game)
        {
        }

        public void Refresh(Action<KnetikApiResponse> cb)
        {
            Client.GetGameOption (Game.ID, Key, (KnetikApiResponse res) => {
                if (res.IsSuccess) {
                    Value = res.Body["result"][Key].Value;
                }

                cb(res);
            });
        }
	}
}

