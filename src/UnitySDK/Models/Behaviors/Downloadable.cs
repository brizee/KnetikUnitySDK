﻿using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Downloadable : Behavior
    {
        public Downloadable(KnetikClient client)
            : base(client) 
        {
        }

        public Dictionary<string, ItemAsset> Assets
        {
            get;
            set;
        }

        public Dictionary<string, string> URLs
        {
            get;
            set;
        }
        
        public override void Deserialize(KnetikJSONNode json)
        {
            base.Deserialize(json);

            Assets = new Dictionary<string, ItemAsset>();
            URLs = new Dictionary<string, string>();

            if (json ["assets"].AsArray != null)
            {
                foreach (KnetikJSONNode node in json["assets"].Children)
                {
                    string slug = node ["slug"].Value;
                    ItemAsset asset = new ItemAsset(Client);
                    asset.Deserialize(node ["asset"]);
                    Assets.Add(slug, asset);
                    URLs.Add(slug, asset.URL);
                }
            } else if (json ["urls"].AsArray != null)
            {
                foreach (KnetikJSONNode node in json["urls"].Children)
                {
                    URLs.Add(node["name"].Value, node["url"].Value);
                }
            }
        }
    }
}