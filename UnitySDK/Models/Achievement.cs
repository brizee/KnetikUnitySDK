using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Achievement : Item
	{
        public int Value {
            get;
            set;
        }

        public ItemAsset UnearnedAsset {
            get;
            set;
        }
        
        public ItemAsset EarnedAsset {
            get;
            set;
        }

        public Achievement (KnetikClient client)
            : base(client)
        {
        }

		public Achievement (KnetikClient client, int id)
            : base(client, id)
		{
		}

        public override void Deserialize (KnetikJSONNode json)
        {
            base.Deserialize (json);

            Value = json["value"].AsInt;
            
            int unearnedAssetId = json ["unearned_asset_id"].AsInt;
            int earnedAssetId = json ["earned_asset_id"].AsInt;
            
            foreach (ItemAsset asset in Assets) {
                if (asset.ID == unearnedAssetId) {
                    UnearnedAsset = asset;
                } else if (asset.ID == earnedAssetId) {
                    EarnedAsset = asset;
                }
            }
        }
	}
}

