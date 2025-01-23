using System;
using System.Collections.Generic;
using System.Text;

namespace JBTrackIR.Helpers
{
    [Serializable]
    public class Limit
    {
        public Limit()
        {
            lower = -180;
            upper = 180;
        }
        public Limit(float low, float up)
        {
            lower = low;
            upper = up;
        }
        public float lower, upper;
    }
}
