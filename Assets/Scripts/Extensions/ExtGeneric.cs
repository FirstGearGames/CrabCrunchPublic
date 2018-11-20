
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Global.Structures
{

    public static class Generics
    {

        /// <summary>
        /// Shuffles a generic array.
        /// </summary>
        /// <param name="array">Array to shuffle.</param>
        public static void Shuffle<T>(ref T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                T tmp = array[i];
                int r = Random.Range(i, array.Length);
                array[i] = array[r];
                array[r] = tmp;
            }
        }



        /// <summary>
        /// Shuffles a generic list.
        /// </summary>
        /// <param name="list">List to shuffle.</param>
        public static void Shuffle<T>(ref List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T tmp = list[i];
                int r = Random.Range(i, list.Count);
                list[i] = list[r];
                list[r] = tmp;
            }
        }
    }


}