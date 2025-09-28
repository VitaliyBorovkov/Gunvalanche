using System;

[Serializable]
public struct WeaponUnlockEntry
{
    public GunsType gunsType;
    public int levelToUnlock;
    public bool spawnAllowed;
}
