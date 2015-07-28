using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
	public abstract class KnetikModel
	{
		public KnetikClient Client {
			get;
			protected set;
		}

        public KnetikApiResponse Response {
            get;
            protected set;
        }

		public KnetikModel (KnetikClient client)
		{
			Client = client;
		}

        public virtual void Deserialize(KnetikJSONNode json) {}
	}
}

