using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Weapons;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] public enum EnemyState { MOVE, ATTACK, BUILD, CLIMB, GOUP }
    private EnemyState _currentState;
    [SerializeField] private Spot _currentSpot;
    private Character _target;
    private Animator _animator;
    private NavMeshAgent _agent;
    [SerializeField] private Spot _spotToGo;
    [SerializeField] private string horizontalDirection;
    private int _previousLayer;
    private int moveToTheRight;
    private bool goingUp = false;
    private int path;
    private int path2;
    private bool up = false;


    private void Awake()
    {
        _agent = GetComponentInParent<NavMeshAgent>();
        _animator = GetComponentInParent<Animator>();
        horizontalDirection = "";
        _previousLayer = 0;
        moveToTheRight = 0;

    }

    // Start is called before the first frame update
    void Start()
    {
        _currentState = EnemyState.MOVE;
        foreach (var spot in GameObject.FindGameObjectsWithTag("A1"))
        {
            if (spot.transform.position == gameObject.transform.position)
            {
                var x = spot.GetComponent<MovementSpot>();
                if (x != null)
                {
                    _currentSpot = x;
                }
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case EnemyState.MOVE:
                move();
                break;
            case EnemyState.ATTACK:
                attack();
                break;
            case EnemyState.BUILD:
                build();
                break;
            case EnemyState.CLIMB:
                climb();
                break;
            case EnemyState.GOUP :
                goup();
                break;
        }
    }

    private void goup()
    {
        
        //Dejar de disparar y moverse
        _animator.SetBool("Shoot", false);
        _animator.SetBool("Walk", true);

        GameObject p1 = null;
        AttackSpot p2 = null;
        
        if (!goingUp)
        {
            path = Random.Range(0, 2);
            path2 = Random.Range(0, 2);
        }

        if (path == 0)
        {
            goingUp = true;
            p1 = GameObject.Find("P1_8"); 
            p2 = GameObject.Find("P1_12").GetComponent<AttackSpot>();
            
            if (GetComponentInParent<Transform>().position.x == p1.transform.position.x && GetComponentInParent<Transform>().position.z == p1.transform.position.z)
            {
                up = true;
                transform.parent.gameObject.SetActive(false);
                //TODO Esperar unos segundos a que suba
                transform.parent.position = p2.transform.position;
                transform.parent.gameObject.SetActive(true);
            
            }
             
            else if (GetComponentInParent<Transform>().position.x == p2.GetConnectionSpot.transform.position.x && GetComponentInParent<Transform>().position.z == p2.GetConnectionSpot.transform.position.z)
            {
                _currentSpot = p2.GetConnectionSpot;
                _currentState = EnemyState.ATTACK;
                return;
            }
            
            else if (up)
            {
                _agent.SetDestination(p2.GetConnectionSpot.transform.position);
            }
           

            else
                _agent.SetDestination(p1.transform.position);
        }
        
        else if (path == 1)
        {
            goingUp = true;
            p1 = GameObject.Find("P1_9");
            if(path2 == 0)
                p2 = GameObject.Find("P1_10").GetComponent<AttackSpot>();
            else
                p2 = GameObject.Find("P1_11").GetComponent<AttackSpot>();
            
            if (GetComponentInParent<Transform>().position.x == p1.transform.position.x && GetComponentInParent<Transform>().position.z == p1.transform.position.z)
            {
                up = true;
                transform.parent.gameObject.SetActive(false);
                //TODO Esperar unos segundos a que suba
                transform.parent.position = p2.transform.position;
                transform.parent.gameObject.SetActive(true);
            
            }
            
            else if (GetComponentInParent<Transform>().position.x == p2.GetConnectionSpot.transform.position.x && GetComponentInParent<Transform>().position.z == p2.GetConnectionSpot.transform.position.z)
            {
                _currentSpot = p2.GetConnectionSpot;
                _currentState = EnemyState.ATTACK;
                return;
            }
            
            else if (up)
            {
                _agent.SetDestination(p2.GetConnectionSpot.transform.position);
                
            }
           

            else
                _agent.SetDestination(p1.transform.position);
        }
        

        
        
            
    }

    private void climb()
    {
        
        //Moverse
        _animator.SetBool("Walk", true);
        _animator.SetBool("Shoot", false);    
        MovementSpot movementSpotAux = _currentSpot as MovementSpot;

        //Si estoy en la parte de arriba de la torre y no tengo un destino al que ir, cojo uno aleatorio.
        if (_spotToGo == null && GetComponentInParent<Transform>().position.x == movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.transform.position.x && GetComponentInParent<Transform>().position.z == movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.transform.position.z)
        {
            _spotToGo = movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.GetDestinationPoints[Random.Range(0, movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.GetDestinationPoints.Count)];
        }

        //Si mientras estoy realizando la subida tengo ya un punto al que ir.
        if (_spotToGo != null)
        {
            //Si ese punto al que ir no está cogido o soy yo el que lo tiene cogido:
            if (!_spotToGo.Taken || _spotToGo.Soldier.Id == GetComponentInParent<Character>().Id)
            {

                //desocupo la parte de arriba de la torre,
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.Taken = false;
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.Soldier = null;

                //ocupo ese punto
                _spotToGo.Taken = true;
                _spotToGo.Soldier = GetComponentInParent<Character>();

                //y por último me dirijo hacia él.
                _agent.isStopped = false;
                _agent.SetDestination(_spotToGo.transform.position);

                //Cuando por fín llegue a ese punto, paso a ver si puedo atacar a alguien. Además anoto que el punto en el que estoy es el punto que tenía como destino.
                if (GetComponentInParent<Transform>().position.x == _spotToGo.transform.position.x && GetComponentInParent<Transform>().position.z == _spotToGo.transform.position.z)
                {
                    _currentSpot = _spotToGo;
                    _currentState = EnemyState.ATTACK;
                    return;
                }
            }
            else
            {
                //Si no puedo moverme al punto cogido de forma aleatoria, me quedo parado y ya veré si el siguiente punto escogido está libre.
                _spotToGo = null;
                _agent.isStopped = true;
            }
        }

        //Si he llegado a la base de la torre, 
        else if (GetComponentInParent<Transform>().position.x == movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.transform.position.x && GetComponentInParent<Transform>().position.z == movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.transform.position.z)
        {
                
            //miro a ver si pudo subir.
            if (!movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.Taken || movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.Soldier.Id == GetComponentInParent<Character>().Id)
            {

                //Si puedo subir la torre, la base deja de estar ocupada.
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Taken = false;
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Soldier = null;

                //Y además se ocupa la parte de arriba de la torre.
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.Taken = true;
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.Soldier = GetComponentInParent<Character>();

                transform.parent.gameObject.SetActive(false);
                //TODO Esperar unos segundos a que suba.
                transform.parent.position = movementSpotAux.ClosestWallToClimb.GetSiegeTowerDestinationPoint.transform.position;
                transform.parent.gameObject.SetActive(true);
            }
            else
            {
                //Si no puedo subir a la parte de arriba de la torre, me quedo en la base.
                _agent.isStopped = true;
            }
        }



        //Si aún no estoy ni en la base ni en la parte de arriba, quiere decir que aún no he pasado por la base de la torre, así que veo si me puedo dirigir a ella.
        else if (!movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Taken || movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Soldier.Id == GetComponentInParent<Character>().Id)
        {
            //Si puedo

            //Además ocupo la base de la torre
            movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Taken = true;
            movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Soldier = GetComponentInParent<Character>();

            //desocupo el punto en el que estaba.
            if (_currentSpot.Soldier != null && _currentSpot.Soldier.Id == GetComponentInParent<Character>().Id)
            {
                _currentSpot.Taken = false;
                _currentSpot.Soldier = null;

            }


            //Y por último, me dirijo a ella.
            _agent.isStopped = false;
            _agent.SetDestination(movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.transform.position);
        }

        else if (movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Taken)
        {
            _currentState = EnemyState.MOVE;
            return;
        }
    }

    private void move()
    {
        _agent.isStopped = false;
        _animator.SetBool("Walk", true);
        _animator.SetBool("Shoot", false);
        
        //Si estoy en un punto de movimiento
        MovementSpot movementSpotAux = _currentSpot as MovementSpot;
        if (movementSpotAux != null)
        {
            //Si este punto está cerca de la puerta de la primera muralla o de la segunda muralla
		    //y la puerta ha sido destruida, subir por allí.
            if (movementSpotAux.Door.name != "Auxiliar Wall" && (GetComponentInParent<Weapon>().WeaponValues.Type == Class.INFANTERY
                || GetComponentInParent<Weapon>().WeaponValues.Type == Class.ARCHER))
            {
                Spot auxSpot = null;
                if (movementSpotAux.Door.IsWallDestroyed())
                {
                    //Busco el punto de la puerta para dirigirme a él.
                    var state = EnemyState.MOVE;
                    if (movementSpotAux.Door.name == "Door 2")
                        auxSpot = GameObject.Find("U1_1").GetComponent<MovementSpot>();
                    else
                    {
                        auxSpot = GameObject.Find("G1_3").GetComponent<AttackSpot>();
                        state = EnemyState.GOUP;
                    }

                    if (auxSpot.Taken == false || auxSpot.Soldier == GetComponentInParent<Character>())
                    {
                        //Desocupo el punto en el que estoy
                        movementSpotAux.Taken = false;
                        movementSpotAux.Soldier = null;
                        //Ocupo el punto al que voy a dirigirme.
                        auxSpot.Taken = true;
                        auxSpot.Soldier = GetComponentInParent<Character>();
                        //Y Me dirijo hacia él.
                        _agent.SetDestination(auxSpot.transform.position);
                    }
                    else
                    {
                        _agent.isStopped = true;
                    }
                    
                
                    //Cuando llegue al punto justo antes de la puerta, hay que determinar por dónde se sube al segundo piso.
                    if (transform.position.x ==
                        auxSpot.transform.position.x && transform.position.z == auxSpot.transform.position.z)
                    {
                        _currentState = state;
                        _currentSpot = auxSpot;
                        return;
                    }
                }
                //Si la puerta no ha sido destruida
                else
                {
                    //Si la unidad está fuera de las murallas
                    if (movementSpotAux.Door.name == "Door 2" && _currentSpot.tag != "U1")
                    {
                        //Si es un ariete, siempre va a derribar la puerta
                        if (GetComponentInParent<Weapon>().WeaponValues.Type == Class.ARIETE)
                        {
                            auxSpot = GameObject.Find("U1_1").GetComponent<MovementSpot>();
                            _agent.SetDestination(auxSpot.transform.position);
                        }
                    }
                }
                
            }
            
            //Si desde este punto se puede escalar y la torre está construida, miro a ver si puedo moverme hacia la torre.
            else if (movementSpotAux.ClosestWallToClimb.name != "Auxiliar Wall" && movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.IsSiegeTowerBuilt &&
                    !movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.Taken)
            {
                _currentState = EnemyState.CLIMB;
                return;
            }

            //Si no estoy en un punto desde el que pueda escalar, me tengo que mover.
            else if (_spotToGo == null)
            {
                //La primera vez, se decide de forma aleatoria hacia que lado se mueve la unidad y ya se moverá hacia ese lado siempre.
                if (horizontalDirection == "")
                {
                    moveToTheRight = Random.Range(0, 2);
                    if (moveToTheRight == 1)
                        horizontalDirection = "right";
                    else
                        horizontalDirection = "left";
                }

                _spotToGo = movementSpotAux.getSpotToMove(horizontalDirection);
                //Si se obtiene un destino, quiere decir que hay al menos un punto de los posibles hacia el que me puede mover.
                if (_spotToGo != null)
                {
                    if (movementSpotAux.EndPoint)
                    {
                        if (horizontalDirection == "right")
                            horizontalDirection = "left";
                        else
                            horizontalDirection = "right";
                    }
                    //Entonces desocupo el punto en el que estoy
                    movementSpotAux.Taken = false;
                    movementSpotAux.Soldier = null;

                    //Ocupo el punto al que voy a dirigirme.
                    _spotToGo.Taken = true;
                    _spotToGo.Soldier = GetComponentInParent<Character>();

                    //Y por último, me dirijo hacia él.
                    _agent.SetDestination(_spotToGo.transform.position);
                    return;
                }
                else
                {
                    //Si desde el punto en el que me encuentro no puedo moverme a absolutamente ningún punto, pues me quedo quieto.
                    _agent.isStopped = true;
                }

            }

            //Si llego al punto que tenía como destino, 
            else
            {
                _agent.SetDestination(_spotToGo.transform.position);
                if ((GetComponentInParent<Transform>().position.x == _spotToGo.transform.position.x) && (GetComponentInParent<Transform>().position.z == _spotToGo.transform.position.z))
                {
                    //lo primero es marcar ese punto destino como mi punto actual.
                    _currentSpot = _spotToGo;

                    if (_currentSpot.name == "U1_4" && GetComponentInParent<Weapon>().WeaponValues.Type == Class.ARIETE)
                    {
                        _spotToGo = GameObject.Find("G1_2").GetComponent<MovementSpot>();
                        return;
                    }
                    
                    //Si he avanzado de capa, lo anoto para elegir de manera aleatoria en qué dirección moverme horizontalmente.
                    if (_previousLayer != _currentSpot.gameObject.layer)
                    {
                        _previousLayer = _currentSpot.gameObject.layer;
                        horizontalDirection = "";
                    }

                    //Si el soldado se ha movido horizontalmente (está en la misma capa) en una dirección puede ser porque:
                    //1)es la dirección que ha obtenido de manera aleatoria 2)no es la dirección que ha obtenido de manera aleatoria pero la que ha obtenido estaba ocupada.
                    //En este último caso, hay que cambiar la dirección, para que siga moviéndose hacia ella.
                    else
                    {
                        MovementSpot currentSpotAux = _currentSpot as MovementSpot;
                        if (horizontalDirection == "right")
                        {
                            if (!currentSpotAux.CanMoveToTheRight())
                                horizontalDirection = "left";
                        }

                        else if (horizontalDirection == "left")
                        {
                            if (!currentSpotAux.CanMoveToTheLeft())
                                horizontalDirection = "right";
                        }
                    }
                    transform.parent.forward = _currentSpot.transform.forward;

                    //Cuando llego a un nuevo punto, veo si puedo atacar.
                    _currentState = EnemyState.ATTACK;
                    return;
                }
            }
        }
        //Si no estoy en un punto de movimiento estoy en un punto de torre
        else
        {
            TowerSpot towerSpotAux = _currentSpot as TowerSpot;
            if (!towerSpotAux.GetConnectionSpot.Taken ||
                towerSpotAux.GetConnectionSpot.Soldier == GetComponent<Character>())
            {
                //Ocupo el punto de conexión
                towerSpotAux.GetConnectionSpot.Taken = true;
                towerSpotAux.GetConnectionSpot.Soldier = GetComponent<Character>();
                
                //Desocupo el punto de torre
                towerSpotAux.Soldier = null;
                towerSpotAux.Taken = false;

                _agent.SetDestination(towerSpotAux.GetConnectionSpot.transform.position);
            }
        }
  
        _currentState = EnemyState.MOVE;
        return;

    }

    private void build()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Shoot", false);    
        MovementSpot movementSpotAux = _currentSpot as MovementSpot;

        //Si estoy en un punto cerca de una muralla y esa muralla ha sido destruida, miro a ver cómo está la torre de asedio.
        if (movementSpotAux.ClosestWallToClimb.name != "Auxiliar Wall" && movementSpotAux.ClosestWallToClimb.IsWallDestroyed())
        {
            //Primero de todo, me detengo.
            _agent.isStopped = true;

            //Si la torre está construida no me planteo más, paso a ver si puedo moverme hacia la base.
            if (movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.IsSiegeTowerBuilt)
            {
                _spotToGo = null;
                _currentState = EnemyState.MOVE;
                return;
            }

            //Si la torre no está construida, miro a ver si hay alguien construyéndola. Si no hay nadie, entonces me designo a mi como constructor.
            else if (movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.WhoIsBuilding == null)
            {
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.StartBuildingTower(GetComponentInParent<Character>());
                return;
            }

            //Si soy yo eel constructor, entonces tengo que construirla a cada frame.
            else if (movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.WhoIsBuilding.Id == GetComponentInParent<Character>().Id)
            {
                movementSpotAux.ClosestWallToClimb.GetSiegeTowerPoint.BuildTower();
                return;
            }

        }
        _currentState = EnemyState.MOVE;
        _spotToGo = null;
        return;
    }

    private void attack()
    {
        _animator.SetBool("Walk", false);
        _animator.SetBool("Shoot", true);


        _agent.isStopped = true;
        MovementSpot mySpot = _currentSpot as MovementSpot;

        //Guardar el arma del personaje
        Weapon weapon = GetComponentInParent<Character>().GetWeapon;

        //Hacer una lista con los puntos dentro del rango de ataque
        List<Spot> spotsWithinRange = new List<Spot>();
        foreach(var spot in _currentSpot.getSpotsToAttack)
        {
            if (weapon.WeaponValues.Range * 1.5F > spot.Value)
                spotsWithinRange.Add(spot.Key as Spot);
        }
        
        //Hacer una lista con los muros dentro del rango de ataque
        Dictionary<Wall, float> wallsInRange = new Dictionary<Wall, float>();
        foreach(var wall in mySpot.GetTargetWalls)
        {
            if (weapon.WeaponValues.Range * 1.5F > wall.Value)
                wallsInRange.Add(wall.Key, wall.Value);
        }

        List<Spot> spotsWithinRangeAndWithArtillery;
        List<Spot> spotsWithinRangeAndWithTroops;
        spotsWithinRangeAndWithTroops = PickSpotsWithTroops(spotsWithinRange);
        spotsWithinRangeAndWithArtillery = PickSpotsWithArtillery(spotsWithinRange);
        
            switch (weapon.WeaponValues.Type)
            {
                case Class.INFANTERY:
                    //Prioridad: Infantería -> Artillería

                    //Comprobar si algún punto dentor del rango de ataque contiene TROPAS
                    //Si existe al menos una unidad de TROPAS a la que se puede atacar, atacarla
                    if (spotsWithinRangeAndWithTroops.Count != 0)
                    {
                        ShootTarget(spotsWithinRangeAndWithTroops);
                        return;
                    }

                    //Si no hay TROPAS, comprobar si algún punto dentor del rango de ataque contiene ARTILLERÍA
                    //Si existe al menos una unidad de ARTILLERÍA a la que se puede atacar, atacarla
                    if (spotsWithinRangeAndWithArtillery.Count != 0)
                    {
                        ShootTarget(spotsWithinRangeAndWithArtillery);
                        return;
                    }

                    break;

                case Class.SMALL_ARTILLERY:
                    //Prioridad: Artillería -> Infantería -> Muros
                    
                    //Si existe al menos una unidad de ARTILLERÍA a la que se puede atacar, atacarla
                    if (spotsWithinRangeAndWithArtillery.Count != 0)
                    {
                        ShootTarget(spotsWithinRangeAndWithArtillery);
                        return;
                    }
                    
                    //Si existe al menos una unidad de TROPAS a la que se puede atacar, atacarla
                    if (spotsWithinRangeAndWithTroops.Count != 0)
                    {
                        ShootTarget(spotsWithinRangeAndWithTroops);
                        return;
                    }
                    
                    //Si no existe al menos una unidad de ARTILLERÍA o TROPA a la que se puede atacar, hay MUROS a los
                    //que se puede atacar desde este punto y estos MUROS están dentro del rango de ataque de
                    //la ARTILLERÍA pesada, atacar los MUROS
                    /*if (wallsInRange.Count != 0)
                    {
                        ShootWall(wallsInRange);
                        return;
                    }*/

                    break;

                case Class.BIG_ARTILLERY:
                    //Prioridad: Artillería -> Muros -> Infantería

                    //Si existe al menos una unidad de ARTILLERÍA a la que se puede atacar, atacarla
                    if (spotsWithinRangeAndWithArtillery.Count != 0)
                    {
                        ShootTarget(spotsWithinRangeAndWithArtillery);
                        return;
                    }

                    //Si no existe al menos una unidad de ARTILLERÍA a la que se puede atacar, hay MUROS a los que
                    //se puede atacar desde este punto y estos MUROS están dentro del rango de ataque de la ARTILLERÍA
                    //pesada, atacar los MUROS
                    /*if(wallsInRange.Count != 0)
                    {
                        ShootWall(wallsInRange);
                        return;
                    }*/

                    //Si existe al menos una unidad de TROPAS a la que se puede atacar, atacarla
                    if(spotsWithinRangeAndWithTroops.Count != 0)
                    {
                        ShootTarget(spotsWithinRangeAndWithTroops);
                        return;
                    }

                    break;

                case Class.MORTAR:
                    //Prioridad: Muros -> Artillería -> Infantería

                     //Si existen MUROS desde los que se puede atacar desde el punto en el que se encuentra el mortero
                     //y están dentro del rango del mortero, atacarlos
                     if (wallsInRange.Count != 0)
                     { 
                         ShootWall(wallsInRange);
                         return;
                     }

                    //Si existe al menos un punto dentro del rango de ataque que contiene ARTILLERÍA, atacarlo
                     if (spotsWithinRangeAndWithArtillery.Count != 0)
                     {
                         ShootTarget(spotsWithinRangeAndWithArtillery);
                         return;
                     }
                     
                     //Si no existe, pero sí existe al menos un punto dentro del rango de ataque que contiene
                     //TROPAS, atacarlo
                     else if(spotsWithinRangeAndWithTroops.Count != 0)
                     {
                         ShootTarget(spotsWithinRangeAndWithTroops);
                         return;
                     }

                     break;
                
                case Class.ARIETE:
                    //Ataca solo a las puertas

                    if (_currentSpot.name == "G1_2")
                    {
                        //Lanzar animación de ataque del ariete
                        return;
                    }

                    break;
                    

                default:
                    break;
            }
        _currentState = EnemyState.BUILD;
        _spotToGo = null;
        return;
    }

    private List<Spot> PickSpotsWithArtillery(List<Spot> spotsWithinRange)
    {
        List<Spot> spotsWithinRangeAndWithArtillery = new List<Spot>();

        foreach (Spot spot in spotsWithinRange)
        {
            if (spot.Soldier != null && !spot.Soldier.IsEnemy)
            {
                //Los defensores no tienen morteros, así que no se incluyen en los objetivos de los atacantes.
                if (spot.Soldier.GetWeapon.WeaponValues.Type == Class.SMALL_ARTILLERY || spot.Soldier.GetWeapon.WeaponValues.Type == Class.BIG_ARTILLERY)
                {
                    spotsWithinRangeAndWithArtillery.Add(spot);
                }
            }
            
        }

        return spotsWithinRangeAndWithArtillery;
    }

    private List<Spot> PickSpotsWithTroops(List<Spot> spotsWithinRange)
    {
        List<Spot> spotsWithinRangeAndWithTroops = new List<Spot>();

        foreach (Spot spot in spotsWithinRange)
        {
            if (spot.Soldier != null && !spot.Soldier.IsEnemy)
            {
                if (spot.Soldier.GetWeapon.WeaponValues.Type == Class.INFANTERY)
                {
                    spotsWithinRangeAndWithTroops.Add(spot);
                }
            }
        }

        return spotsWithinRangeAndWithTroops;
    }


    private void ShootTarget(List<Spot> Spots)
    {
        if (_target == null)
        {
            Spot spotToAttack = Spots[Random.Range(0, Spots.Count)];
            _target = spotToAttack.Soldier;
            LookAt(_target.transform.position);
        }

        if (GetComponentInParent<Weapon>().Shoot("Enemy Bullet", 2, _target, GetComponentInParent<Character>()))
            _animator.SetBool("Shoot", true);

    }

    private void LookAt(Vector3 targetPosition)
    {
        var directionLook = Vector3.Normalize(targetPosition - transform.position);
        transform.parent.forward = directionLook;
    }

    private void ShootWall(Dictionary<Wall, float> wallsInRange)
    {
        Wall wallToAttack = null;
        float minDistance = 100.0f;
        foreach (var wall in wallsInRange)
        {
            if (wall.Value < minDistance)
            {
                minDistance = wall.Value;
                wallToAttack = wall.Key;
            }
        }
        LookAt(wallToAttack.transform.position);
        if (GetComponentInParent<Weapon>().Shoot("Enemy Bullet", 2, null, GetComponentInParent<Character>()))
            _animator.SetBool("Shoot", true);

    }

    private void OnCollisionEnter(Collision collision)
    {
        _target = collision.gameObject.GetComponent<Character>(); 
        LookAt(_target.transform.position);
        if (GetComponentInParent<Weapon>().Shoot("Enemy Bullet", 2, _target, GetComponentInParent<Character>()))
            _animator.SetBool("Shoot", true);
    }
}
