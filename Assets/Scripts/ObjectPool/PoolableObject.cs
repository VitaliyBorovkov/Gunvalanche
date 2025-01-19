using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    private string PoolID;

    public void SetPoolID(string poolID)
    {
        PoolID = poolID;
    }

    public string GetPoolID()
    {
        return PoolID;
    }
}
