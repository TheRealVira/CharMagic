#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: ExamplePlugin
// Filename: FirstCurse.cs
// Date - created:2016.08.31 - 11:56
// Date - current: 2016.08.31 - 15:08

#endregion

#region Usings

using CharMagicLib;

#endregion

namespace ExamplePlugin
{
    public class FirstCurse : CharMagicAPI
    {
        public FirstCurse()
            : base(
                "TheThirstyOne",
                "It will take the first and last letter and put them back interchanged. The curse itself MAY also lift it`......"
                )
        {
        }

        public override string Curse(string input)
        {
            if (input.Length < 2) return input;

            return input.Substring(input.Length - 1) + input.Substring(1, input.Length - 2) + input.Substring(0, 1);
        }

        public override string LiftCurse(string input)
        {
            // Some basic reuse :P
            return Curse(input);
        }
    }
}