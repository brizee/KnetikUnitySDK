using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class UserHierarchy : KnetikModel
    {
        public List<UserHierarchyNode> Ancestors { get; private set; }
        public List<UserHierarchyNode> Siblings { get; private set; }
        public List<UserHierarchyNode> Descendants { get; private set; }

        public UserHierarchy(KnetikClient client)
            : base(client) {
        }

        public override void Deserialize(KnetikJSONNode json)
        {
            base.Deserialize (json);

            Ancestors = new List<UserHierarchyNode>();
            Siblings = new List<UserHierarchyNode>();
            Descendants = new List<UserHierarchyNode>();

            foreach (KnetikJSONNode node in json["ancestors"].Children) {
                UserHierarchyNode ancestor = new UserHierarchyNode(Client);
                ancestor.Deserialize(node);
                Ancestors.Add(ancestor);
            }

            foreach (KnetikJSONNode node in json["siblings"].Children) {
                UserHierarchyNode siblings = new UserHierarchyNode(Client);
                siblings.Deserialize(node);
                Siblings.Add(siblings);
            }

            foreach (KnetikJSONNode node in json["descendants"].Children) {
                UserHierarchyNode descendant = new UserHierarchyNode(Client);
                descendant.Deserialize(node);
                Descendants.Add(descendant);
            }
        }
    }
}