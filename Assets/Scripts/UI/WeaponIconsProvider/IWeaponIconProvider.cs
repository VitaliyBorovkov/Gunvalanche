using System;

using UnityEngine;

public interface IWeaponIconProvider
{
    void GetIcon(string iconKey, Action<Sprite> onLoaded);
    void ReleaseIcon(string iconKey);
    //void PreloadAllIcons();
}
