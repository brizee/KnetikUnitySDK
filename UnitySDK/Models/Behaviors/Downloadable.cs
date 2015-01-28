using System;
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
        
        public Dictionary<string, string> URLs
        {
            get;
            set;
        }
        
        public override void Deserialize(KnetikJSONNode json)
        {
            base.Deserialize(json);

            URLs = new Dictionary<string, string>();
            foreach (KnetikJSONNode node in json["urls"].Children)
            {
                URLs.Add(node["name"].Value, node["url"].Value);
            }
        }
    }
}
