using System;
using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    /// <summary>
    /// Allows multiselection to enum values within the inspector.
    /// </summary>
    public class BitMaskAttribute : PropertyAttribute
    {
        public Type propType;
        public BitMaskAttribute(Type aType)
        {
            propType = aType;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Checks if one or more enums match a specified value.
        /// </summary>
        /// <param name="actualValue">Enum(s) to check against.</param>
        /// <param name="containsValues">Values to check within actual value.</param>
        /// <returns>Returns true if actualValue has any of containsValues parameter.</returns>
        public static bool HasFlag(this Enum actualValue, Enum containsValues)
        {
            //If not the same type of Enum return false.
            if (actualValue.GetType() != containsValues.GetType())
                return false;

            /* Convert enum values to ulong. With so few
             * values a uint would be safe, but should
             * the options expand ulong is safer. */
            ulong containsNum = Convert.ToUInt64(containsValues);
            ulong targetNum = Convert.ToUInt64(actualValue);

            return (containsNum & targetNum) == targetNum;
        }

        ///// <summary>
        ///// Checks if one or more enums match a specified value.
        ///// </summary>
        ///// <param name="actualValue">int value for enums of object.</param>
        ///// <param name="targetValue">int value for enums to check against.</param>
        ///// <returns>Returns true if actualValue contains all enums within targetValue.</returns>
        //public static bool HasFlag(this int actualValue, int targetValue)
        //{
        //    /* Convert enum values to ulong. With so few
        //     * values a uint would be safe, but should
        //     * the options expand ulong is safer. */
        //    ulong actualNum = Convert.ToUInt64(actualValue);
        //    ulong targetNum = Convert.ToUInt64(targetValue);

        //    return (actualNum & targetNum) == targetNum;
        //}


        /// <summary>
        /// Convert a string to an enum value.
        /// </summary>
        /// <typeparam name="TEnum">Enum type.</typeparam>
        /// <param name="strEnumValue">String value</param>
        /// <param name="defaultValue">Default value for Enum type.</param>
        /// <returns></returns>
        public static TEnum StringToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
        {
            if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
                return defaultValue;

            return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
        }
        /// <summary>
        /// Determine an enum value from a given string. This can be an expensive function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">Text of string.</param>
        /// <param name="defaultValue">Default value if enum couldn't be found.</param>
        /// <returns>Enum found or default value if no enum is found.</returns>
        public static T FromString<T>(string text, T defaultValue)
        {
            //If string is empty or null return default value.
            if (string.IsNullOrEmpty(text))
                return defaultValue;
            //If enum isn't defined return default value.
            if (!Enum.IsDefined(typeof(T), (string)text))
                return defaultValue;
            //Return parsed value.
            return (T)Enum.Parse(typeof(T), text, true);
        }
    }


}