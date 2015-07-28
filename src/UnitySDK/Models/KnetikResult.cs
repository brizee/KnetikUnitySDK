using System;

namespace Knetik
{
	public class KnetikResult<T> where T : KnetikModel
	{
        public T Value {
            get;
            set;
        }

        public KnetikApiResponse Response {
            get;
            set;
        }
	}
}

