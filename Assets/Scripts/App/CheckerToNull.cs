using UnityEngine;

public class CheckerToNull
{
    public static bool CheckObjectNotNull(object obj, string objectName)
    {
        if (obj == null)
        {
            Debug.Log($"CheckerToNull: ������ '{objectName}' ����� null!");
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
            Debug.Log($":CheckerToNull ������ '{arrayName}' ����� null!");
            return false;
        }
        if (array.Length == 0)
        {
            Debug.Log($":CheckerToNull ������ '{arrayName}' ����!");
            return false;
        }
       
        return true;
    }
}
