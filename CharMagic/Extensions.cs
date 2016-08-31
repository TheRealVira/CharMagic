#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: CharMagic
// Filename: Extensions.cs
// Date - created:2016.08.31 - 12:40
// Date - current: 2016.08.31 - 15:08

#endregion

#region Usings

using System.Collections.Generic;

#endregion

namespace CharMagic
{
    internal static class Extensions
    {
        public static void TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key)) return;

            dictionary.Add(key, value);
        }
    }
}