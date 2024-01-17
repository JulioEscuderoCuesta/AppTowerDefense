using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Custom/Weapon Values")]
    public class WeaponValues : ScriptableObject
    {
        [SerializeField] private Class type;
        [SerializeField] private int damage;
        [SerializeField] private int range;
        [SerializeField] private int firingRate;
        [SerializeField] private int accuracy;
        [SerializeField] private bool availableForDefense;
        [SerializeField] private int speed;

        public Class Type
        {
            get => type;
            set => type = value;
        }

        public int Damage
        {
            get => damage;
            set => damage = value;
        }
        public int Range
        {
            get => range;
            set => range = value;
        }
        public int FireRate
        {
            get => firingRate;
            set => firingRate = value;
        }
        public int Accuracy
        {
            get => accuracy;
            set => accuracy = value;
        }
        public bool AvailableForDefense
        {
            get => availableForDefense;
            set => availableForDefense = value;
        }
        public int Speed
        {
            get => speed;
            set => speed = value;
        }
        

    }
}


