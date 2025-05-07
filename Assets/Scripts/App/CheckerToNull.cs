using UnityEngine;

public class CheckerToNull
{
    public static bool CheckObjectNotNull(object obj, string objectName)
    {
        if (obj == null)
        {
            Debug.Log($"CheckerToNull: Объект '{objectName}' равен null!");
            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool CheckArrayNotEmpty<T>(T[] array, string arrayName)
    {
        if (array == null)
        {
            Debug.Log($"CheckerToNull: Массив '{arrayName}' равен null!");
            return false;
        }
        if (array.Length == 0)
        {
            Debug.Log($"CheckerToNull: Массив '{arrayName}' пуст!");
            return false;
        }
       
        return true;
    }
}
