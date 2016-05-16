﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IronWren.AutoMapper.StructureMapping
{
    /// <summary>
    /// Represents a Wren module that was generated by the <see cref="AutoMapper"/>.
    /// </summary>
    internal sealed class ForeignModule
    {
        private Dictionary<string, ForeignClass> classes = new Dictionary<string, ForeignClass>();

        /// <summary>
        /// Gets the string builder used for building the source code.
        /// </summary>
        private StringBuilder Source;

        /// <summary>
        /// Gets the <see cref="ForeignClass"/>es that are part of the module.
        /// </summary>
        public ReadOnlyDictionary<string, ForeignClass> Classes { get; }

        /// <summary>
        /// Gets whether the module was loaded.
        /// </summary>
        public bool Used { get; private set; }

        public ForeignModule()
        {
            Classes = new ReadOnlyDictionary<string, ForeignClass>(classes);
        }

        public void Add(Type type)
        {
            classes.Add(type.Name, new ForeignClass(type.GetTypeInfo()));
        }

        public string GetSource()
        {
            if (Source == null)
            {
                var classSources = Classes.Values.Select(foreignClass => foreignClass.GetSource()).ToArray();

                Source = new StringBuilder(classSources.Sum(source => source.Length));

                foreach (var classSource in classSources)
                    Source.AppendLine(classSource);
            }

            Used = true;

            return Source.ToString();
        }
    }
}