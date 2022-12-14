using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] Player _player;

    //PlayerGun _playerGun;
    TMP_Text _ammoText;
    
    void Awake()
    {
        _ammoText = GetComponent<TMP_Text>();
    }
    
    void OnEnable()
    {
        _player.transform.GetChild(4).GetComponent<PlayerGun>().OnAmmoChange += UpdateAmmo;
    }
    
    void UpdateAmmo(Projectile type, int count)
    {
        string ammoType = ProjectileToString(type);

        if (ammoType != null)
        {
            _ammoText.text = ammoType + "\t\t" + count;
        }
        else
        {
            _ammoText.text = "";
        }
    }

    string ProjectileToString(Projectile type)
    {
        if (type is ExplosiveBullet)
        {
            return "GRENADE";
        }
        else if (type is TrackerBullet)
        {
            return "TRACKER";
        }
        else if (type is TripleBullet)
        {
            return "TRISHOT";
        }
        else
        {
            return null;
        }
    }

    void OnDisable()
    {
        _player.MyGun.OnAmmoChange -= UpdateAmmo;
    }
}
