using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Weapons;
using System.Linq;

public class Character : MonoBehaviour
{
    [SerializeField] private bool enemy;
    [SerializeField] private Life life;
    [SerializeField] private int id;
    [SerializeField] private Weapon _weapon;
	private Animator _animator;


    public bool IsEnemy => enemy;
    public Weapon GetWeapon => _weapon;
    public Life Life => life;
    public int Id => id;
    public UnityEvent CharacterKilled;

    
    private void Awake()
    {
        id = Random.Range(0, 100000000);
        _weapon = GetComponent<Weapon>();
        life = GetComponent<Life>();
        life.onDeath.AddListener(DestroyCharacter);
		_animator = GetComponent<Animator>();

    }

    private void Start()
    {
        
    }
    
    private void DestroyCharacter()
    {
        _animator.SetTrigger("Die");
        CharacterKilled.Invoke();
        life.onDeath.RemoveListener(DestroyCharacter);
        Destroy(gameObject, 1);

    }
}