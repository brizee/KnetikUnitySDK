using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Item : KnetikModel
    {
        public int ID {
            get;
            set;
        }
        
        public string TypeHint {
            get;
            set;
        }
        
        public string Name {
            get;
            set;
        }
        
        public string ShortDescription  {
            get;
            set;
        }
        
        public string LongDescription {
            get;
            set;
        }

        public DateTime DeletedAt {
            get;
            set;
        }

        public DateTime DateCreated {
            get;
            set;
        }

        public DateTime DateUpdated {
            get;
            set;
        }
        
        public List<ItemAsset> Assets {
            get;
          	set;
	    }

        public Item (KnetikClient client, int id)
            : base(client)
        {
            ID = id;
            
            Assets = new List<ItemAsset>();
        }
        
        public override void Deserialize (KnetikJSONNode json)
        {
            ID = json ["id"].AsInt;
            TypeHint = json ["type_hint"].Value;
            Name = json ["name"].Value;
            ShortDescription = json ["short_description"].Value;
            LongDescription = json ["long_description"].Value;

            Assets.Clear ();
            foreach (KnetikJSONNode node in json["assets"].Children) {
                ItemAsset asset = new ItemAsset(Client);
                asset.Deserialize(node);
                Assets.Add(asset);
            }
            
            if (json ["deleted_at"] != null && json ["deleted_at"] != "null") {
                DeletedAt = new DateTime (json ["deleted_at"].AsInt);
            }

            if (json ["date_created"] != null && json ["date_created"] != "null") {
                DateCreated = new DateTime (json ["date_created"].AsInt);
            }

            if (json ["date_updated"] != null && json ["date_updated"] != "null") {
                DateUpdated = new DateTime (json ["date_updated"].AsInt);
            }
        }
    }
}

