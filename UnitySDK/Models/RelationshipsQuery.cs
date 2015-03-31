using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class RelationshipsQuery : KnetikModel
    {
        public int AncestorDepth
        {
            get;
            set;
        }

        public int DescendantDepth
        {
            get;
            set;
        }

        public bool IncludeSiblings
        {
            get;
            set;
        }

        public Dictionary<string, UserHierarchy> Relationships {
            get;
            set;
        }

        public RelationshipsQuery(KnetikClient client)
            : base(client)
        {
            AncestorDepth = 1;
            DescendantDepth = 1;
            IncludeSiblings = true;
        }

        public KnetikResult<RelationshipsQuery> Load(Action<KnetikResult<RelationshipsQuery>> cb = null)
        {
            if (cb != null)
            {
                // async

                Client.GetRelationships(AncestorDepth, DescendantDepth, IncludeSiblings, (res) => {
                    cb(OnLoad(res));
                });
                return null;
            } else
            {
                // sync
                return OnLoad(Client.GetRelationships(AncestorDepth, DescendantDepth, IncludeSiblings));
            }
        }

        private KnetikResult<RelationshipsQuery> OnLoad(KnetikApiResponse res)
        {
            var result = new KnetikResult<RelationshipsQuery> {
                Response = res
            };
            if (!res.IsSuccess)
            {
                return result;
            }
            Response = res;
            
            if (res.Body ["result"].Value != "null")
            {
                this.Deserialize(res.Body ["result"]);
            } else
            {
                Relationships = new Dictionary<String, UserHierarchy>();
            }
            
            result.Value = this;
            return result;
        }

        public override void Deserialize(KnetikJSONNode json)
        {
            base.Deserialize (json);
            
            Relationships = new Dictionary<String, UserHierarchy> ();
            foreach (KeyValuePair<string, KnetikJSONNode> kvp in (KnetikJSONClass)json) {
                UserHierarchy relationship = new UserHierarchy(Client);
                relationship.Deserialize(kvp.Value);
                Relationships.Add(kvp.Key, relationship);
            }
        }
    }
}

