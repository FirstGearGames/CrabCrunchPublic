using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    /// <summary>
    /// Various utility classes relating to ints.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Pads a string with text to the left until the string exceeds a specified value. C# default String.PadLeft doesn't seem to be working; bug perhaps?
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static string PadLeft(string text, int length, string padding)
        {
            while (text.Length < length)
            {
                text = padding + text;
            }
            return text;
        }
        
    }

}