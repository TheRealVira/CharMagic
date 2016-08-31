#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: ExamplePlugin
// Filename: Colloportus.cs
// Date - created:2016.08.31 - 12:20
// Date - current: 2016.08.31 - 18:18

#endregion

#region Usings

using System;
using System.Linq;
using CharMagicLib;

#endregion

namespace ExamplePlugin
{
    public class Colloportus : CharMagicAPI
    {
        public Colloportus()
            : base(
                "Colloportus /kɒloʊˈpɔːrtəs/ kol-o-por-təs",
                "Magically locks a string, preventing it from being opened by Muggle means.")
        {
        }

        public override string Curse(string input)
        {
            return input.Aggregate(string.Empty, (current, c) => current + Convert.ToChar(c + 1));
        }

        public override string LiftCurse(string input)
        {
            return input.Aggregate(string.Empty, (current, c) => current + Convert.ToChar(c - 1));
        }
    }
}