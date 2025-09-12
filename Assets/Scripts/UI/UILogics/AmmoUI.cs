using TMPro;

using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    public void UpdateAmmo(int currentInClip, int totalAmmo)
    {
        ammoText.text = $"{currentInClip}/{totalAmmo}";
    }
}
