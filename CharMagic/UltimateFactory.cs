﻿#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: CharMagic
// Project: CharMagic
// Filename: UltimateFactory.cs
// Date - created:2016.08.31 - 12:06
// Date - current: 2016.08.31 - 18:18

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using CharMagicLib;

#endregion

namespace CharMagic
{
    /// <summary>
    ///     Copy past of my UltimateFactory class in my FileSearch project:
    ///     https://github.com/TheRealVira/FileSearch/blob/master/FileSearch/Algorithms/UltimateFactory.cs
    ///     NOTE: May create a lib out of it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class UltimateFactory<T>
    {
#if (DEBUG)
        private const bool DEBUG = true;
#else
        private const bool DEBUG = false;
#endif

        public static Dictionary<string, T> Compute(AppDomain domain)
        {
            var toRet = new Dictionary<string, T>();

            // The oneliner of hell explained:
            // 1) Get all Assemblies in the current domain
            // 2) Flatt out the resulting sequneces int one sequence
            // 3) Select only those Types, which are assigned from T
            // 3.1) And they shouldn't be an interface or an abstract class
            // 4) Convert it to a list, so we are able to use Linq to iterate through (via the "Foreach" method") without using any more lines of code
            // 5) Now we need to create an instance of all types, we got
            // 6) Add [the name of the type, the created instance] to our toRet dictionary
            // 7) Hope you'll don't get an error :3

            // Note: Well - it kind looks quite like a spell which summons satan if you wait long enough, but if you look closer...... hmmm yeah you're probably right. It'll summon satan.
            domain.GetAssemblies()
                .AsParallel()
                .ForAll(assms => Compute(assms).AsParallel().ForAll(items => toRet.Add(items.Key, items.Value)));

            return toRet;
        }

        public static Dictionary<string, T> Compute(Assembly assm)
        {
            var myType = typeof(T);
            var toRet = new Dictionary<string, T>();

            try
            {
                assm.GetTypes()
                    .Where(x => !x.IsInterface && !x.IsAbstract && myType.IsAssignableFrom(x) &&
                                (DEBUG
                                    ? true
                                    : !Attribute.IsDefined(x, typeof(TestingPurpose))))
                    .ToList()
                    .ForEach(x => { toRet.Add(x.Name, (T) Activator.CreateInstance(x)); });
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var item in ex.LoaderExceptions)
                {
                    MessageBox.Show(item.Message);
                }
            }

            return toRet;
        }
    }
}