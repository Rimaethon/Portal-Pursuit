using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public List<Gun> guns = new List<Gun>();
    public int equippedGunIndex;

    private void Start()
    {
        SetUpGuns();
    }

    private void SetUpGuns()
    {
        foreach (Gun gun in guns)
        {
            if (gun == null) continue;

            if (gun.available && guns[equippedGunIndex] == gun)
                gun.gameObject.SetActive(true);
            else
                gun.gameObject.SetActive(false);
        }
    }

    public void FireEquippedGun()
    {
        if (guns[equippedGunIndex].available && guns[equippedGunIndex] != null) guns[equippedGunIndex].Fire();
    }

}
