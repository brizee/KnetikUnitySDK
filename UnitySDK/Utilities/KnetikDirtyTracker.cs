using System;
using System.Collections.Generic;

namespace Knetik
{
	public class KnetikDirtyTracker
	{
        private HashSet<string> dirtyProperties;

		public KnetikDirtyTracker ()
		{
            dirtyProperties = new HashSet<string> ();
		}

        public bool MarkDirty(string field, bool isDirty = true)
        {
            if (isDirty) {
                return dirtyProperties.Add (field);
            } else {
                return dirtyProperties.Remove (field);
            }
        }

        public bool IsDirty(string field)
        {
            return dirtyProperties.Contains (field);
        }

        public void Reset()
        {
            dirtyProperties.Clear ();
        }
	}
}

