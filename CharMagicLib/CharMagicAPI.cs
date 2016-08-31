#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: CharMagicLib
// Filename: CharMagicAPI.cs
// Date - created:2016.08.31 - 11:42
// Date - current: 2016.08.31 - 15:08

#endregion

namespace CharMagicLib
{
    public abstract class CharMagicAPI
    {
        /// <summary>
        ///     The description of curse.
        /// </summary>
        public readonly string DescriptionOfCurse;

        /// <summary>
        ///     The kind of curse.
        /// </summary>
        public readonly string TypeOfCurse;

        protected CharMagicAPI(string typeOfCurse, string descriptionOfCurse)
        {
            TypeOfCurse = typeOfCurse;
            DescriptionOfCurse = descriptionOfCurse;
        }

        /// <summary>
        ///     This method will curse your string to look different
        /// </summary>
        /// <param name="input">The text you want to curse.</param>
        /// <returns>The cursed text.</returns>
        public abstract string Curse(string input);

        /// <summary>
        ///     This method will lift the applied curse.
        /// </summary>
        /// <param name="input">The cursed text.</param>
        /// <returns>It'll return your virgin string ;)</returns>
        public abstract string LiftCurse(string input);

        public override string ToString()
        {
            return $"{TypeOfCurse} is a curse which does cruel things as followed:\n\n{DescriptionOfCurse}";
        }
    }
}