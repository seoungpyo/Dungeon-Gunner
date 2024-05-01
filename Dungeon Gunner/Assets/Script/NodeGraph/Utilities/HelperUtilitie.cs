using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class HelperUtilitie
{
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if(stringToCheck == "")
        {
            Debug.Log("is empty and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool VaildateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if(enumerableObjectToCheck == null)
        {
            Debug.Log(fieldName + "is null in object " + thisObject.name.ToString());
            return true;
        }

        foreach(var item in enumerableObjectToCheck)
        {
            if(item == null)
            {
                Debug.Log(fieldName + "has null values in object" + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }
        }

        if(count == 0)
        {
            Debug.Log(fieldName + "has no values in object" + thisObject.name.ToString());
            error = true;
        }

        return error;
    }
    
}
