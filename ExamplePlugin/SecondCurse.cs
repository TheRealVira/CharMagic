#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: ExamplePlugin
// Filename: SecondCurse.cs
// Date - created:2016.08.31 - 12:09
// Date - current: 2016.08.31 - 18:18

#endregion

#region Usings

using CharMagicLib;

#endregion

namespace ExamplePlugin
{
    public class SecondCurse : CharMagicAPI
    {
        public SecondCurse()
            : base(
                "SadSecondOne", "It only works with the big winners, which have four or more medals around their neck.")
        {
        }

        public override string Curse(string input)
        {
            if (input.Length < 4) return input;

            return input.Substring(0, 1) + input.Substring(input.Length - 2, 1) + input.Substring(3, input.Length - 4) +
                   input.Substring(1, 1) + input.Substring(input.Length - 1);
        }

        public override string LiftCurse(string input)
        {
            return Curse(input);
        }
    }
}