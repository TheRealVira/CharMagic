#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: CharMagicLib
// Filename: CustomAttributes.cs
// Date - created:2016.08.31 - 11:54
// Date - current: 2016.08.31 - 18:18

#endregion

#region Usings

using System;

#endregion

namespace CharMagicLib
{
    /// <summary>
    ///     Algorithms which have the "TestingPurpose" as attribute, won't show up in Release-Mode
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TestingPurpose : Attribute
    {
    }
}