using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class UserHierarchyNode : KnetikModel {
        public int ID
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string FullName
        {
            get;
            set;
        }

        public string Language
        {
            get;
            set;
        }

        public string AvatarURL
        {
            get;
            set;
        }

        public string Gender
        {
            get;
            set;
        }

        public List<UserHierarchyNode> Descendants
        {
            get;
            set;
        }

        public UserHierarchyNode(KnetikClient client)
            : base(client) {
        }

        public override void Deserialize(KnetikJSONNode json)
        {
            base.Deserialize (json);
            ID = json ["id"].AsInt;
            Username = json ["username"].Value;
            DisplayName = json ["display_name"].Value;
            Email = json ["email"].Value;
            FirstName = json ["first_name"].Value;
            LastName = json ["last_name"].Value;
            FullName = json ["fullname"].Value;
            Language = json ["language"].Value;
            AvatarURL = json ["avatar_url"].Value;
            Gender = json ["gender"].Value;

            Descendants = new List<UserHierarchyNode>();
            if (json ["descendants"] != null && json ["descendants"].Count > 0)
            {
                foreach (KnetikJSONNode node in json["descendants"].Children)
                {
                    UserHierarchyNode descendant = new UserHierarchyNode(Client);
                    descendant.Deserialize(node);
                    Descendants.Add(descendant);
                }
            }
        }
    }
}
