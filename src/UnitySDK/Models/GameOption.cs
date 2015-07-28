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

        public KnetikApiResponse Refresh(Action<KnetikApiResponse> cb = null)
        {
            if (cb != null)
            {
                // async
                Client.GetGameOption(Game.ID, Key, (KnetikApiResponse res) => {
                    cb(OnRefresh(res));
                });
                return null;
            } else
            {
                // sync
                return OnRefresh(Client.GetGameOption(Game.ID, Key));
            }
        }

        private KnetikApiResponse OnRefresh(KnetikApiResponse res)
        {
            if (res.IsSuccess)
            {
                Value = res.Body ["result"] [Key].Value;
            }
            
            return res;
        }
	}
}

