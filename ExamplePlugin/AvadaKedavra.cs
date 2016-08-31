#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: ExamplePlugin
// Filename: AvadaKedavra.cs
// Date - created:2016.08.31 - 12:23
// Date - current: 2016.08.31 - 18:18

#endregion

#region Usings

using System;
using System.Linq;
using CharMagicLib;

#endregion

namespace ExamplePlugin
{
    public class AvadaKedavra : CharMagicAPI
    {
        private static Random _rand;

        public AvadaKedavra()
            : base(
                "Avada Kedavra /əˈvɑːdə kəˈdɑːvrə/ ə-vah-də kə-dah-vrə (Killing Curse)",
                "Causes instant, painless death to whomever the curse hits. There is no countercurse or method of blocking this spell;"
                )
        {
            if (_rand == null)
            {
                _rand = new Random();
            }
        }

        public override string Curse(string input)
        {
            return input.Aggregate(string.Empty,
                (current, t) => current + Convert.ToChar(_rand.Next(char.MinValue, char.MaxValue + 1)));
        }

        public override string LiftCurse(string input)
        {
            return Curse(input);
        }
    }
}