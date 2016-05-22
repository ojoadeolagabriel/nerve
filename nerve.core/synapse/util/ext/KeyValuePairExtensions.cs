﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nerve.core.synapse.util.ext
{
    public static class KeyValuePairExtensions
    {
        public static bool IsNull<T, TU>(this KeyValuePair<T, TU> pair)
        {
            return pair.Equals(new KeyValuePair<T, TU>());
        }
    }
}
