using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System;

public static class Utils {

    public static byte[] ToBytesArray(this object objectToConvert)
    {


        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        bf.Serialize(ms, objectToConvert);

        return ms.ToArray();
    }

    public static object ToObject(this byte[] arrayToConvert)
    {
        if(arrayToConvert.Length == 0)
        {
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        ms.Write(arrayToConvert, 0, arrayToConvert.Length);
        ms.Seek(0, SeekOrigin.Begin);

        object convertedObject = bf.Deserialize(ms);

        return convertedObject;
    }


    public static T[] Slice<T>(this T[] arrayToSlice, int indexToStart)
    {

        T[] slicedArray = new T[arrayToSlice.Length - indexToStart];

        Array.Copy(arrayToSlice, indexToStart, slicedArray, 0, slicedArray.Length);

        return slicedArray;
    }
}
