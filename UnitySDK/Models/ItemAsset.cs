using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class ItemAsset : KnetikModel
    {
        public int ID {
            get;
            set;
        }

        public string Description {
            get;
            set;
        }

        public string Type {
            get;
            set;
        }

        public string Path {
            get;
            set;
        }

		public string url {
			get;
			set;
		}
        
		public int itemId {
			get;
			set;
		}
        public ItemAsset (KnetikClient client)
            : base(client)
        {
        }
        
        public override void Deserialize (KnetikJSONNode json)
        {
            ID = json ["id"].AsInt;
            Description = json ["description"].Value;
            Type = json ["type"].Value;
            Path = json ["path"].Value;
			url = json ["url"].Value;
			itemId = json ["item_id"].AsInt;

        }
    }
}
