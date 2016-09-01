#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: CharMagic
// Filename: Crucio.cs
// Date - created:2016.09.01 - 11:29
// Date - current: 2016.09.01 - 12:25

#endregion

#region Usings

using System;
using System.Linq;
using CharMagicLib;

#endregion

namespace CharMagic
{
    internal class Crucio : CharMagicAPI
    {
        private static Random _rand;

        public Crucio()
            : base(
                "Crucio /ˈkruːsioʊ/ krew-see-oh (Cruciatus Curse)",
                "Inflicts unbearable pain on the recipient of the curse. One of the irreversible curses.")
        {
            if (_rand == null)
            {
                _rand = new Random(DateTime.Now.Millisecond);
            }
        }

        public override string Curse(string input)
        {
            if (input.Length < 4) return input;

            return input.Substring(0, 1) +
                   string.Concat(input.Substring(1, input.Length - 2).ToCharArray().ToList().OrderBy(x => _rand.Next())) +
                   input.Substring(input.Length - 1);
        }

        public override string LiftCurse(string input)
        {
            // Irreversible
            return Curse(input);
        }
    }
}