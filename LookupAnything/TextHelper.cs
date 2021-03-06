﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pathoschild.Stardew.LookupAnything
{
    /// <summary>A utility class for writing text output.</summary>
    internal class TextHelper
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Select the correct plural form for a word.</summary>
        /// <param name="count">The number.</param>
        /// <param name="single">The singular form.</param>
        /// <param name="plural">The plural form.</param>
        public static string Pluralise(int count, string single, string plural = null)
        {
            return count == 1 ? single : (plural ?? single + "s");
        }

        /// <summary>Get a human-readable list of values.</summary>
        /// <param name="values">The values to print.</param>
        public static string OrList(string[] values)
        {
            values = values ?? new string[0];
            switch (values.Length)
            {
                case 0:
                    return string.Empty;

                case 1:
                    return values[0];

                case 2:
                    return $"{values[0]} or {values[1]}";

                default:
                    StringBuilder list = new StringBuilder();
                    list.Append(values[0]);
                    for (int i = 1; i < values.Length - 1; i++)
                        list.Append($", {values[i]}");
                    if (values.Length != 1)
                        list.Append($", or {values[values.Length - 1]}");

                    return list.ToString();
            }
        }

        /// <summary>Get a human-readable representation of a value.</summary>
        /// <param name="value">The underlying value.</param>
        public static string Stringify(object value)
        {
            if (value == null)
                return null;

            // boolean
            if (value is bool)
                return (bool)value ? "yes" : "no";

            // time span
            if (value is TimeSpan)
            {
                TimeSpan span = (TimeSpan)value;
                List<string> parts = new List<string>();
                if (span.Days > 0)
                    parts.Add($"{span.Days} {TextHelper.Pluralise(span.Days, "day")}");
                if (span.Hours > 0)
                    parts.Add($"{span.Hours} {TextHelper.Pluralise(span.Hours, "hour")}");
                if (span.Minutes > 0)
                    parts.Add($"{span.Minutes} {TextHelper.Pluralise(span.Minutes, "minute")}");
                return string.Join(", ", parts);
            }

            // vector
            if (value is Vector2)
            {
                Vector2 vector = (Vector2)value;
                return $"({vector.X}, {vector.Y})";
            }

            // rectangle
            if (value is Rectangle)
            {
                Rectangle rect = (Rectangle)value;
                return $"(x:{rect.X}, y:{rect.Y}, width:{rect.Width}, height:{rect.Height})";
            }

            // array
            if (value is IEnumerable && !(value is string))
            {
                string[] values = (from val in ((IEnumerable)value).Cast<object>() select TextHelper.Stringify(val)).ToArray();
                return "(" + string.Join(", ", values) + ")";
            }

            // color
            if (value is Color)
            {
                Color color = (Color)value;
                return $"(r:{color.R} g:{color.G} b:{color.B} a:{color.A})";
            }

            // KeyValue<TKey, TValue>
            {
                Type type = value.GetType();
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(KeyValuePair<,>))
                    {
                        string k = TextHelper.Stringify(type.GetProperty(nameof(KeyValuePair<byte, byte>.Key)).GetValue(value));
                        string v = TextHelper.Stringify(type.GetProperty(nameof(KeyValuePair<byte, byte>.Value)).GetValue(value));
                        return $"({k}: {v})";
                    }
                }
            }

            // else
            return value.ToString();
        }
    }
}
