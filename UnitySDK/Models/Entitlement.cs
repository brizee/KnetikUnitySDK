using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Entitlement : Item
    {
        public Entitlement (KnetikClient client, int id)
            : base(client, id)
        {
        }
    }
}
