using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private string nombre;
        public string Id => nombre;
        private float _lastShootTime;
        
        [SerializeField]
        [Tooltip("punto desde el que se dispara")]
        private GameObject shootingPoint;
        [SerializeField] private WeaponValues weaponValues;
        private string _layerName;

        public WeaponValues WeaponValues
        {
            get => weaponValues;
            set => weaponValues = value;
        }

        /// <summary>
        /// Produce el efecto de disparo si el tiempo que ha 
        /// pasado desde el último disparo es menor que la
        /// cadencia del arma
        /// </summary> 
        /// <param name=layerName>La capa a la que pertenece el proyectil</param> 
        public bool Shoot(string layerName, int delay, Character target, Character shooter)
        {
            var timeSinceLastShoot = Time.time - _lastShootTime;
            if (target != null)
            {
                var directionLook = Vector3.Normalize(target.transform.position - transform.position);
                shootingPoint.transform.forward = directionLook;
            }
            if (timeSinceLastShoot < weaponValues.FireRate / 10)
                return false;
            _lastShootTime = Time.time;
            _layerName = layerName;
            StartCoroutine(Fire(shooter, delay));
            return true;
        }

        private IEnumerator Fire(Character shooter, int delay)
        {
            yield return new WaitForSeconds(delay);
            Projectile bullet = ObjectPool.SharedInstance.getFirstPooledObject().GetComponent<Projectile>();
            bullet.Shooter = shooter;
            bullet.Damage = weaponValues.Damage;
            bullet.gameObject.layer = LayerMask.NameToLayer(_layerName);
            bullet.transform.position = shootingPoint.transform.position;
            bullet.transform.rotation = shootingPoint.transform.rotation;
            bullet.gameObject.SetActive(true);
            //TODO Activar efectos de disparo


        }
    }
}
