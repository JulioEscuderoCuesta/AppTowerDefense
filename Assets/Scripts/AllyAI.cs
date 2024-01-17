using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Weapons;
using System.Linq;

public class AllyAI : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Path desde el soldado hasta el posible punto de ataque destino")]
    private LineRenderer _path;

    [SerializeField] private Character _target;
    private NavMeshAgent _agent;
    private List<Spot> _shortestPath;
    private Animator _animator;
    [SerializeField] public Spot _currentSpot;
    private float _pathHightOffset = 0.2f;
    private bool _selected;

	//Permitir seleccionar personaje
	private bool _clickeable = true;
    
    //Si se está moviendo o no y en qué posición del camino se llega
    private bool _moving = false;
    private int index = 1;
    private List<Spot> shortestKeyPath = new List<Spot>();
    private MovementSpot rightStairs;
    private MovementSpot leftStairs;
    private bool datosObtenidos = false;

    //quizás mouse over
    public Spot CurrentSpot => _currentSpot;


    private void Awake()
    {
        //_currentSpot = GameObject.Find("B1_6").GetComponent<AttackSpot>();
        transform.rotation = _currentSpot.transform.rotation;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!GameManager.SharedInstance.InPositioningStage)
        {
            if (_selected)
                ShowPath();
            if (_moving)
                Move();
            else
                Attack();    
        }
    }

    private void OnMouseUp()
    {
        //Si tengo un soldado seleccionado y dejo de pulsar el botón encima de un punto de ataque, me muevo a ese punto
        //de ataque siempre y cuando esté en una oleada
        if (_selected && GameManager.SharedInstance.AttackSpot != null)
        {
            _path.positionCount = 0;
            _selected = false;
            GameManager.SharedInstance.SoldierSelected = null;
            _currentSpot.Taken = false;
            _currentSpot.Soldier = null;
            _moving = true;
			_clickeable = false;

        }
        else
        {
            _selected = false;
            GameManager.SharedInstance.SoldierSelected = null;
        }
    }

    private void OnMouseDrag()
    {
        if (!GameManager.SharedInstance.InPositioningStage && _clickeable)
        {
            _selected = true;
            GameManager.SharedInstance.SoldierSelected = gameObject;
        }
        

    }
    private void Attack()
    {
        _agent.isStopped = true;

        //Ver si existen puntos a los que puedo atacar desde esta posición.
        bool attackableSpots = false;
        if (_currentSpot.getSpotsToAttack.Count != 0)
            attackableSpots = true;

        //Los defensores no atacan a los muros

        //Guardar el arma del personaje
        Weapon weapon = GetComponent<Character>().GetWeapon;

        //Hacer una lista con los puntos dentro del rango de ataque
        List<Spot> spotsWithinRange = new List<Spot>();
        foreach(var spot in _currentSpot.getSpotsToAttack)
        {
            if (weapon.WeaponValues.Range * 1.5F > spot.Value)
                spotsWithinRange.Add(spot.Key as Spot);
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
                case Class.BIG_ARTILLERY:
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

                    break;
                
                default:
                    break;
            }
        
        _animator.SetBool("Shoot", false);

    }
    private List<Spot> PickSpotsWithArtillery(List<Spot> spotsWithinRange)
    {
        List<Spot> spotsWithinRangeAndWithArtillery = new List<Spot>();

        foreach (Spot spot in spotsWithinRange)
        {
            if (spot.Soldier != null && spot.Soldier.IsEnemy)
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
            if (spot.Soldier != null && spot.Soldier.IsEnemy)
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

        if (GetComponentInParent<Weapon>().Shoot("Ally Bullet", 2, _target, GetComponent<Character>()))
            _animator.SetBool("Shoot", true);

    }
    private void LookAt(Vector3 targetPosition)
    {
        var directionLook = Vector3.Normalize(targetPosition - transform.position);
        transform.forward = directionLook;
    }
    private void Move()
    {
        //Si alguien ataca, atacarlo
		_animator.SetBool("Walk", true);
        _animator.SetBool("Shoot", false);

        _agent.isStopped = false;
        _agent.SetDestination(_shortestPath[index].transform.position);
        
        //Desocupo el punto actual y ocupo el punto destino
        _currentSpot.Taken = false;
        _currentSpot.Soldier = null;
        
        _currentSpot = _shortestPath[index];
        _currentSpot.Taken = true;
        _currentSpot.Soldier = GetComponent<Character>();

        //Cuando llego al siguiente punto
        if (transform.position.z == _shortestPath[index].transform.position.z && transform.position.x == _shortestPath[index].transform.position.x)
        {

            //Si estoy en un punto del patio, me teleporto al siguiente punto de la lista y sigo
            if (_shortestPath[index].gameObject.layer == LayerMask.NameToLayer("Patio"))
            {
                gameObject.SetActive(false);
                gameObject.transform.position = _shortestPath[index + 1].transform.position;
                gameObject.SetActive(true);
                index += 2;

            }
            //Si estoy en un punto p y no es del patio, quiere decir que estoy a punto de entrar en una torre
            //con lo que desaparezco, y simulo que hay un soldado en el punto de ataque de la torre.
            else if (_shortestPath[index].gameObject.tag == "P1")
            {
                gameObject.SetActive(false);
                //Esperar unos segundos
                index++;
            }
            //Si es el último punto, el defensor ha llegado a su nueva posición
            else if (index == _shortestPath.Count - 1)
            {
                //Reseteo el contador
                index = 1;
                
                //Paro el movimiento y señalo que se puede mover de nuevo el soldado.
                _animator.SetBool("Walk", false);
                _agent.isStopped = true;
                _moving = false;
				_clickeable = true;
                return;
            }
            //Si no, continúo.
            else
            { 
                index++;
            }
        }
    }

    private void ShowPath()
    {
        AttackSpot currentAttackSpot = _currentSpot as AttackSpot;
        AttackSpot door = GameObject.Find("G1_3").GetComponent<AttackSpot>();
        string[] subStringAux = new string[2];
        string[] subStringAux2 = new string[2];
        string[] subStringAux3 = new string[2];
        string[] otroSubStringAux = new string[2];
        string checkDoorOrStairs;
        string tagSpot = "";
        int numberOfLayer2 = 0;
        int numberOfLayer3 = 0;
        int numberOfSpot = 0;
        int numberOfSpot2 = 0;
        int numberOfSpot3 = 0;
        int numberOfSpotsWithTag = 0;
        int numberOfSpotsWithTag2 = 0;
        string[] stringLayer = new string[2];
        if(!datosObtenidos)
        {
            datosObtenidos = true;
            stringLayer = new string[2];
            //Esta variable marca el inicio del camino en el piso uno.
            //Si está bajando, es la puerta. Si sube, es el propio punto en el que se encuentra el soldado.
            AttackSpot auxAttackSpot = null;
            if (_currentSpot.gameObject.layer == LayerMask.NameToLayer("AttackSpots"))
            {
                auxAttackSpot = _currentSpot as AttackSpot;
            }
            else
            {
                auxAttackSpot = door;
            }

            //Primero veo que keySpot es el de la izquierda y el de la derecha

            //Obtengo el tag de los spots
            tagSpot = auxAttackSpot.GetConnectionSpot.gameObject.tag;

            string[] subStringAuxCurrentSpot = new string[2];
            int numeroCurrentSpot = 0;
            subStringAuxCurrentSpot = auxAttackSpot.GetConnectionSpot.gameObject.name.Split('_');
            numeroCurrentSpot = int.Parse(subStringAuxCurrentSpot[1]);
            int numero = 0;
            int numero2 = 0;
            

            foreach (MovementSpot keySpot in auxAttackSpot.GetConnectionSpot.ClosestStairs)
            {
                if (subStringAux[0] == null)
                {
                    subStringAux = keySpot.name.Split('_');
                    numero = int.Parse(subStringAux[1]);
                }
                else
                {
                    subStringAux2 = keySpot.name.Split('_');
                    numero2 = int.Parse(subStringAux2[1]);

                }
            }
            if (numero > numero2)
            {
                //estoy en el medio
                if (numeroCurrentSpot > numero2 && numeroCurrentSpot < numero)
                {
                    leftStairs = GameObject.Find($"{tagSpot}_{numero2}").GetComponent<MovementSpot>();
                    rightStairs = GameObject.Find($"{tagSpot}_{numero}").GetComponent<MovementSpot>();
                }
                //Estoy en el medio pero en un spot como el 1, 2, 3... o de los últimos
                else
                {
                    rightStairs = GameObject.Find($"{tagSpot}_{numero2}").GetComponent<MovementSpot>();
                    leftStairs = GameObject.Find($"{tagSpot}_{numero}").GetComponent<MovementSpot>();
                }
                
            }
            else
            {
                //estoy en el medio
                if (numeroCurrentSpot > numero2 && numeroCurrentSpot < numero)
                {
                    leftStairs = GameObject.Find($"{tagSpot}_{numero}").GetComponent<MovementSpot>();
                    rightStairs = GameObject.Find($"{tagSpot}_{numero2}").GetComponent<MovementSpot>();
                }
                //Estoy en el medio pero en un spot como el 1, 2, 3... o de los últimos
                else
                {
                    rightStairs = GameObject.Find($"{tagSpot}_{numero}").GetComponent<MovementSpot>();
                    leftStairs = GameObject.Find($"{tagSpot}_{numero2}").GetComponent<MovementSpot>();
                }
            }

        }

        float lowestDistance = 1000f;
        if (GameManager.SharedInstance.AttackSpot != null)
        {
            float actualDistance = 0f;
            AttackSpot destination = GameManager.SharedInstance.AttackSpot;
            
            //Para evitar que se seleccione por error como destino el punto actual
            if (destination != _currentSpot)
            {
                int numberKeySpots = 0;
                List<Spot> shortestPathAux = new List<Spot>();
                //Calcular la distancia más corta.
                
                //Si el origen y el destino están en el mismo piso
                if(CheckSameFloor(destination))
                {
                    //Y es el segundo piso
                    if (_currentSpot.gameObject.layer == LayerMask.NameToLayer("AttackSpots2"))
                    {
                        shortestKeyPath.Add(_currentSpot);
                        CalculateKeyPathSecondFloor(currentAttackSpot.GetConnectionSpot, destination,
                            ref lowestDistance);
                    }
                    //Y es el primer piso
                    else
                    {
                        //Si origen y destino tienen los mismos puntos clave o el destino es un punto clave del origen
                        //o el origen es un punto clave del destino (Casos especiales para que funcione)
                        if (SameKeySpots(currentAttackSpot, destination) == 2 ||
                            KeySpotFromEachOther(currentAttackSpot, destination)
                            || _currentSpot.tag == "C1")
                        {
                            List<Spot> auxList = new List<Spot>();
                            List<Spot> auxList2 = new List<Spot>();
                            shortestKeyPath.Add(_currentSpot);
                            shortestKeyPath.Add(currentAttackSpot.GetConnectionSpot);
                            auxList = AddPathSameRing(currentAttackSpot.GetConnectionSpot, destination.GetConnectionSpot, ref lowestDistance, true);
                            float auxDistance = lowestDistance;
                            auxList2 = AddPathSameRing(currentAttackSpot.GetConnectionSpot, destination.GetConnectionSpot, ref lowestDistance, false);

                            if (auxDistance < lowestDistance)
                            {
                                lowestDistance = auxDistance;
                                shortestKeyPath.AddRange(auxList);
                            }
                            else
                            {
                                shortestKeyPath.AddRange(auxList2);
                            }
                            shortestKeyPath.Add(destination);
                        }
                        
                        //Si origen y destino tienen en común 1 solo punto clave, están cerca y se tarda menos
                        //yendo por las almenas.
                        else if (SameKeySpots(currentAttackSpot, destination) == 1)
                        {
                            CalculateKeyPathFirstFloor(currentAttackSpot.GetConnectionSpot, destination, ref lowestDistance, false);
                            
                        }
                        
                        //Si la distancia entre origen y destino es muy grande, tardará menos bajándose de las almenas y yendo
                        //por el suelo del primer piso.
                        //Se comprueba si tarda menos por la derecha o por la izquierda.
                        else
                        {
                            //Añado el punto actual y el de conexión
                            shortestKeyPath.Add(_currentSpot);
                            shortestKeyPath.Add(currentAttackSpot.GetConnectionSpot);
                            
                            List<Spot> auxList1 = new List<Spot>();
                            List<Spot> auxList2 = new List<Spot>();
                            List<Spot> auxList11 = new List<Spot>();
                            List<Spot> auxList22 = new List<Spot>();
                            float auxDistance = 0;
                            float auxDistance2 = 0;

                            //Añado el camino más cercano por las escaleras derechas hasta las escaleras del destino
                            auxList1 = AddDistanceFirstFloorJumpPoint(rightStairs.JumpPoint, destination, true, ref lowestDistance);
                            auxDistance = lowestDistance;
                            lowestDistance = 0;
                           
                            auxList2 = AddDistanceFirstFloorJumpPoint(leftStairs.JumpPoint, destination, false, ref lowestDistance);
                            auxDistance2 = lowestDistance;
                            lowestDistance = 0;
                                                  
                            MovementSpot auxMSpot = auxList1[auxList1.Count - 1] as MovementSpot;
                            MovementSpot auxMSpot2 = auxList2[auxList2.Count - 1] as MovementSpot;

                            //Añado la última parte del caminio, las almenas
                            auxList11 = AddPathSameRing(auxMSpot, destination.GetConnectionSpot, ref lowestDistance,
                                true);
                                auxDistance += lowestDistance;
                            lowestDistance = 0;
                          

                            auxList22 = AddPathSameRing(auxMSpot2, destination.GetConnectionSpot, ref lowestDistance,
                                false);
                            auxDistance2 += lowestDistance;

                            if (auxDistance < auxDistance2)
                            {
                                lowestDistance = auxDistance;
                                shortestKeyPath.Add(rightStairs);
                                shortestKeyPath.AddRange(auxList1);
                                shortestKeyPath.AddRange(auxList11);
                            }
                            else
                            {
                                lowestDistance = auxDistance2;
                                shortestKeyPath.Add(leftStairs);
                                shortestKeyPath.AddRange(auxList2);
                                shortestKeyPath.AddRange(auxList22);
                            }

                            shortestKeyPath.Add(destination);
                        }
                    }
                }
                //Si origen y destino están en distinto piso
                else
                {
                    List<Spot> auxList1 = new List<Spot>();
                    List<Spot> auxList2 = new List<Spot>();
                    List<Spot> auxList11 = new List<Spot>();
                    List<Spot> auxList22 = new List<Spot>();
                    float auxDistance = 0f;
                    float auxDistance2 = 1000f;
                    
                    //Si el origen es el primer piso
                    if (_currentSpot.gameObject.layer == LayerMask.NameToLayer("AttackSpots"))
                    {
                        //Añado el punto de ataque origen 
                        shortestKeyPath.Add(currentAttackSpot);
                        shortestKeyPath.Add(currentAttackSpot.GetConnectionSpot);
                            
                        //Añado el camino de las almenas hasta la escalera izquierda/derecha más cercana
                        auxList1 = AddPathSameRing(currentAttackSpot.GetConnectionSpot, leftStairs, ref lowestDistance, false);
                        auxDistance += lowestDistance;
                        lowestDistance = 0;

                        auxList2 = AddPathSameRing(currentAttackSpot.GetConnectionSpot, rightStairs, ref lowestDistance,
                            true);
                        auxDistance2 += lowestDistance;
                        lowestDistance = 0;
                        
                        //Añado el camino desde la escalera izquierda/derecha más cercana hasta la puerta.
                        auxList11 = AddPathSameRing(leftStairs.JumpPoint, door.GetConnectionSpot, ref lowestDistance, false);
                        auxDistance += lowestDistance;
                        lowestDistance = 0;
                        
                        auxList22 = AddPathSameRing(rightStairs.JumpPoint, door.GetConnectionSpot, ref lowestDistance, true);
                        auxDistance2 += lowestDistance;
                        
                        if (auxDistance < auxDistance2)
                        {
                            lowestDistance = auxDistance;
                            shortestKeyPath.AddRange(auxList1);
                            shortestKeyPath.AddRange(auxList11);
                        }
                        else
                        {
                            lowestDistance = auxDistance2;
                            shortestKeyPath.AddRange(auxList2);
                            shortestKeyPath.AddRange(auxList22);
                        }

                        shortestKeyPath.Add(door);
                        
                        lowestDistance += AddPathToGoUpStairs(destination);
                        lowestDistance +=
                            AddDistanceSecondFloor(shortestKeyPath[shortestKeyPath.Count - 1] as AttackSpot, destination);
                        
                    }
                    //Si el origen es el segundo piso
                    else if(_currentSpot.gameObject.layer == LayerMask.NameToLayer("AttackSpots2"))
                    {
                        shortestKeyPath.Add(_currentSpot);
                        float distanceToGoDown = 0;
                        distanceToGoDown = AddDistanceSecondFloor(currentAttackSpot, GetClosestStairsSecondFloor(null));
                        AttackSpot auxAttackSpot3 = shortestKeyPath[shortestKeyPath.Count - 1] as AttackSpot;
                        distanceToGoDown += AddPathToGoDownStairs(auxAttackSpot3);
                        
                        //Como shortestKeyPath solo calcula el path más corto del primer piso, tengo que guardar lo
                        //que ya tengo del segundo piso para no perderlo
                        List<Spot> shortestKeyPathSecondFloor = new List<Spot>();
                        
                        shortestKeyPathSecondFloor.AddRange(shortestKeyPath);
                        
                        MovementSpot auxMSpot = shortestKeyPath[shortestKeyPath.Count - 1] as MovementSpot;
                        MovementSpot auxMSpot2 = null;
                        lowestDistance = 0;

                        //Al bajar de la puerta o voy hacia la izquierda o hacia la derecha
                        auxList1 = AddDistanceFirstFloorJumpPoint(auxMSpot, destination, false, ref lowestDistance);
                        auxDistance = lowestDistance;
                        lowestDistance = 0;
                        
                        auxList2 = AddDistanceFirstFloorJumpPoint(auxMSpot, destination, true, ref lowestDistance);
                        auxDistance2 = lowestDistance;
                        lowestDistance = 0;
                        
                        auxMSpot = auxList1[auxList1.Count - 1] as MovementSpot;
                        auxMSpot2 = auxList2[auxList2.Count - 1] as MovementSpot;

                        //Añado la última parte del caminio, las almenas
                        auxList11 = AddPathSameRing(auxMSpot, destination.GetConnectionSpot, ref lowestDistance, false);
                        auxDistance += lowestDistance;
                        lowestDistance = 0;
                        
                        auxList22 = AddPathSameRing(auxMSpot2, destination.GetConnectionSpot, ref lowestDistance, true);
                        auxDistance2 += lowestDistance;

                        if (auxDistance < auxDistance2)
                        {
                            lowestDistance = auxDistance;
                            shortestKeyPath.AddRange(auxList1);
                            shortestKeyPath.AddRange(auxList11);
                        }
                        else
                        {
                            lowestDistance = auxDistance2;
                            shortestKeyPath.AddRange(auxList2);
                            shortestKeyPath.AddRange(auxList22);
                        }

                        shortestKeyPath.Add(destination);
                    }
                    
                }
                //Se tiene una lista con los puntos clave del camino. Construir una lista con TODOS los puntos del camino.
                shortestKeyPath = shortestKeyPath.Distinct().ToList();

                _shortestPath = GetActualList(shortestKeyPath);
                _shortestPath = _shortestPath.Distinct().ToList();

                //La línea visual tendrá tantos puntos como puntos tiene la lista con el camino más corto.
                _path.positionCount = _shortestPath.Count;
                for (int i = 0; i < _shortestPath.Count; i++)
                {
                    _path.SetPosition(i, _shortestPath[i].transform.position + Vector3.up * _pathHightOffset);

                }
                
                //Señalar que el destino está ocupado, para que no se pueda mandar otro soldado a él.
                //_shortestPath[_shortestPath.Count - 1].Soldier = GetComponent<Character>();
                //_shortestPath[_shortestPath.Count - 1].Taken = true;
                
                //Limpiar el camino con los puntos clave.
                shortestKeyPath.Clear();
            }
        }
        //Si no estoy seleccionando ningún soldado, el path no se muestra.
        else
        {
            _path.positionCount = 0;
        }
    }

    /// <summary>
    /// Añade al camino los puntos por los que baja el soldado dependiendo de la escalera que tome
    /// </summary>
    /// <param name="shortestKeyPath"></param>
    /// <param name="origin"></param>
    /// <returns>
    /// Distancia del recorrido añadido.
    /// </returns>
    private float AddPathToGoDownStairs(AttackSpot origin)
    {
        float distance = 0f;
        AttackSpot door = GameObject.Find("G1_3").GetComponent<AttackSpot>();
        AttackSpot p1Spot = null;
        switch (origin.gameObject.name)
        {
            case "P1_12":
                p1Spot = GameObject.Find("P1_8").GetComponent<AttackSpot>();
                break;
            case "P1_11":
                p1Spot = GameObject.Find("P1_9").GetComponent<AttackSpot>();
                break;
            case "P1_10":
                p1Spot = GameObject.Find("P1_9").GetComponent<AttackSpot>();
                break;
        }
        
        shortestKeyPath.Add(p1Spot);
        shortestKeyPath.Add(door);
        shortestKeyPath.Add(door.GetConnectionSpot);
        distance = Vector3.Distance(p1Spot.transform.position, door.transform.position);
        return distance;
    }

    /// <summary>
    /// Calcula el camino más corto por el que subir desde la primera planta al punto de la segunda planta
    /// más cercano al punto destino
    /// </summary>
    /// <param name="destination">
    /// Punto destino
    /// </param>
    /// <param name="shortestKeyPath">
    /// Camino con los puntos desde el origen hasta la puerta
    /// </param>
    /// <returns>
    /// Distancia añadida al camino
    /// </returns>
    private float AddPathToGoUpStairs(AttackSpot destination)
    {
        float distance = 0f;
        AttackSpot p1Spot = null;
        AttackSpot p2Spot = null;
        //El camino empieza en la puerta
        AttackSpot door = GameObject.Find("G1_3").GetComponent<AttackSpot>();
        //Calculo qué escalera está más cerca del punto de ataque destino de la segunda planta
        AttackSpot stairsSpot = GetClosestStairsSecondFloor(destination);
        switch (stairsSpot.gameObject.name)
        {
            case "P1_12":
                p1Spot = GameObject.Find("P1_8").GetComponent<AttackSpot>();
                p2Spot = GameObject.Find("P1_12").GetComponent<AttackSpot>();
                break;
            case "P1_11":
                p1Spot = GameObject.Find("P1_9").GetComponent<AttackSpot>();
                p2Spot = GameObject.Find("P1_11").GetComponent<AttackSpot>();
                break;
            case "P1_10":
                p1Spot = GameObject.Find("P1_9").GetComponent<AttackSpot>();
                p2Spot = GameObject.Find("P1_10").GetComponent<AttackSpot>();
                break;
        }
        
        shortestKeyPath.Add(p1Spot);
        shortestKeyPath.Add(p2Spot);
        distance = Vector3.Distance(door.transform.position, p1Spot.transform.position);
        return distance;
    }
    
    
    private List<Spot> AddDistanceFirstFloorJumpPoint(MovementSpot currentKeySpot, AttackSpot destination, bool right, ref float lowestDistance)
    {
        List<Spot> shortestPathAux = new List<Spot>();
        float auxDistance = 0f;
        
        //Coger la letra del tag del spot para saber si hay que añadir al path el jumpPoint.
        string letterTag = currentKeySpot.gameObject.tag.Substring(0, 1);
        
        MovementSpot previousKeySpot = currentKeySpot;
        bool terminado = false;

        string tagSpot = currentKeySpot.gameObject.tag;

        //Obtengo el número concreto del punto origen
        string[] subStringAux = currentKeySpot.gameObject.name.Split('_');

        int numberOfSpot = int.Parse(subStringAux[1]);
        
        int numberObjectsWithTag = GameObject.FindGameObjectsWithTag(tagSpot).Length;

        if (right)
        {
            for(int i = numberOfSpot; i < numberObjectsWithTag; i++)
            {
                MovementSpot possibleKeySpot = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                if (possibleKeySpot.IsKeySpot)
                {
                    auxDistance += Vector3.Distance(previousKeySpot.transform.position,
                        possibleKeySpot.transform.position);
                    previousKeySpot = possibleKeySpot;
                    shortestPathAux.Add(possibleKeySpot);
                    foreach (MovementSpot keySpotDestination in destination.GetConnectionSpot
                                     .ClosestStairs)
                    {
                        MovementSpot auxSpot = null;
                        if (destination.gameObject.name == "G1_3")
                        {
                            auxSpot = keySpotDestination;
                        }
                        else
                        {
                            auxSpot = keySpotDestination.JumpPoint;
                        }
                        
                        if (possibleKeySpot == auxSpot)
                        {
                            auxDistance += possibleKeySpot.DistanceToJumpPoint;
                            lowestDistance = auxDistance;
                            shortestPathAux.Add(keySpotDestination);
                            terminado = true;
                        }
                    }

                    if (terminado)
                        return shortestPathAux;
                }
            }

            for(int i = 1; i < numberObjectsWithTag; i++)
            {
                MovementSpot possibleKeySpot = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                if (possibleKeySpot.IsKeySpot)
                {
                    auxDistance += Vector3.Distance(previousKeySpot.transform.position,
                        possibleKeySpot.transform.position);
                    previousKeySpot = possibleKeySpot;
                    shortestPathAux.Add(possibleKeySpot);
                    foreach (MovementSpot keySpotDestination in destination.GetConnectionSpot
                                 .ClosestStairs)
                    {
                        MovementSpot auxSpot = null;
                        if (destination.gameObject.name == "G1_3")
                        {
                            auxSpot = keySpotDestination;
                        }
                        else
                        {
                            auxSpot = keySpotDestination.JumpPoint;
                        }

                        
                        if (possibleKeySpot == auxSpot)
                        {
                            auxDistance += possibleKeySpot.DistanceToJumpPoint;
                            lowestDistance = auxDistance;
                            shortestPathAux.Add(keySpotDestination);
                            terminado = true;
                        }
                    }

                    if (terminado)
                        return shortestPathAux;
                }
                    
            }
        }

        else
        {
            for (int i = numberOfSpot; i > 0; i--)
            {
                MovementSpot possibleKeySpot =
                    GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();

                if (possibleKeySpot.IsKeySpot)
                {
                    auxDistance += Vector3.Distance(previousKeySpot.transform.position,
                        possibleKeySpot.transform.position);
                    previousKeySpot = possibleKeySpot;
                    shortestPathAux.Add(possibleKeySpot);
                    foreach (MovementSpot keySpotDestination in destination.GetConnectionSpot
                                 .ClosestStairs)
                    {
                        MovementSpot auxSpot = null;
                        if (destination.gameObject.name == "G1_3")
                        {
                            auxSpot = keySpotDestination;
                        }
                        else
                        {
                            auxSpot = keySpotDestination.JumpPoint;
                        }


                        if (possibleKeySpot == auxSpot)
                        {
                            auxDistance += possibleKeySpot.DistanceToJumpPoint;
                            lowestDistance = auxDistance;
                            shortestPathAux.Add(keySpotDestination);
                            terminado = true;
                        }
                    }

                    if (terminado)
                        return shortestPathAux;
                }
            }

            for (int i = numberObjectsWithTag; i > numberOfSpot; i--)
            {
                MovementSpot possibleKeySpot = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                if (possibleKeySpot.IsKeySpot)
                {
                    auxDistance += Vector3.Distance(previousKeySpot.transform.position,
                        possibleKeySpot.transform.position);
                    previousKeySpot = possibleKeySpot;
                    shortestPathAux.Add(possibleKeySpot);
                    foreach (MovementSpot keySpotDestination in destination.GetConnectionSpot
                                 .ClosestStairs)
                    {
                        MovementSpot auxSpot = null;
                        if (destination.gameObject.name == "G1_3")
                        {
                            auxSpot = keySpotDestination;
                        }
                        else
                        {
                            auxSpot = keySpotDestination.JumpPoint;
                        }

                        if (possibleKeySpot == auxSpot)
                        {
                            auxDistance += possibleKeySpot.DistanceToJumpPoint;
                            lowestDistance = auxDistance;
                            shortestPathAux.Add(keySpotDestination);
                            terminado = true;
                        }
                    }

                    if (terminado)
                        return shortestPathAux;
                }
            }
        }

        return null;

    }

    /// <summary>
    /// Añade al camino a recorrer los puntos del segundo piso y devuelve la devuelve la distancia entre
    /// el origen y el destino
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <param name="shortestKeyPath"></param>
    /// <returns>
    /// Distancia entre el origen y el destino.
    /// </returns>
    private float AddDistanceSecondFloor(AttackSpot origin, AttackSpot destination)
    {
        shortestKeyPath.Add(origin.GetConnectionSpot);
        shortestKeyPath.Add(destination.GetConnectionSpot);
        shortestKeyPath.Add(destination);
        return Vector3.Distance(origin.transform.position, origin.GetConnectionSpot.transform.position)
            + Vector3.Distance(origin.GetConnectionSpot.transform.position, destination.GetConnectionSpot.transform.position)
            + Vector3.Distance(destination.GetConnectionSpot.transform.position, destination.transform.position);
    }

    /// <summary>
    /// Calcula los puntos exactos del recorrido a partir de la lista con los puntos claves del recorrido.
    /// </summary>
    /// <param name="shortestKeyPath">
    /// Lista con los puntos claves del recorrido.
    /// </param>
    /// <returns>
    /// Lista con todos los puntos del recorrido.
    /// </returns>
    private List<Spot> GetActualList(List<Spot> shortestKeyPath)
    {
        List<Spot> listWithTheShortestPath = new List<Spot>();
        listWithTheShortestPath.Add(shortestKeyPath[0]); //El punto origen

        //Si el punto de origen es una torre, añado el punto torre a continuación
        MovementSpot possibleTower = shortestKeyPath[1] as MovementSpot;
        if (possibleTower.TowerSpot.name != "Auxiliar Spot")
            listWithTheShortestPath.Add(possibleTower.TowerSpot);
        string[] subStringAux = new string[2];
        string[] subStringAux2 = new string[2];
        string[] subStringAux3 = new string[2];
        string[] otroSubStringAux = new string[2];
        string checkDoorOrStairs;
        string tagSpot = "";
        string tagSpot2 = "";
        int numberOfSpot = 0;
        int numberOfSpot2 = 0;
        int numberOfSpot3 = 0;
        int numberOfSpotsWithTag = 0;
        int numberOfSpotsWithTag2 = 0;
      
        //Si el número de puntos en la lista es de 4, estoy seguro en el mismo anillo y muy cerca del destino.
        //Si el número de putos en la lista de puntos clave es mayor que 4, es porque estoy en el mismo piso y anillo
        //pero el origen y el destino están muy lejos o están en distintos pisos o en el primer piso pero distinto anillo

        //Omito el punto origen y el destino. No hace falta estudiarlos.
        for (int i = 1; i < shortestKeyPath.Count - 2; i++)
        {
            tagSpot = shortestKeyPath[i].gameObject.tag;
            tagSpot2 = shortestKeyPath[i + 1].gameObject.tag;

            //Los puntos están en diferentes anillos. Añado el punto siguiente y sigo a partir de ahí
            if (tagSpot != tagSpot2)
            {
                listWithTheShortestPath.Add(shortestKeyPath[i + 1]);
                
                checkDoorOrStairs = shortestKeyPath[i + 1].gameObject.name.Substring(0, 1);

                //Si el siguiente spot es la puerta o una escalera, añado los 3 siguientes spots
                //y continúo
                if (checkDoorOrStairs == "P" || checkDoorOrStairs == "G")
                {
                    //Añado la escaleras y puerta o la puerta y escaleras
                    listWithTheShortestPath.Add(shortestKeyPath[i + 2]);
                    listWithTheShortestPath.Add(shortestKeyPath[i + 3]);
                    listWithTheShortestPath.Add(shortestKeyPath[i + 4]);
                    i += 3;
                }
            }
            else
            {
                //Obtengo el número de puntos por capa .
                numberOfSpotsWithTag = GameObject.FindGameObjectsWithTag(tagSpot).Length;
                subStringAux = shortestKeyPath[i].gameObject.name.Split('_');
                subStringAux2 = shortestKeyPath[i + 1].gameObject.name.Split('_');

                numberOfSpot = int.Parse(subStringAux[1]);
                numberOfSpot2 = int.Parse(subStringAux2[1]);

                //Si el punto están en el tercer punto, para ir al siguiente 
                if (tagSpot == "D3")
                {
                    if (numberOfSpot > numberOfSpot2)
                    {
                        for (int j = numberOfSpot; j >= numberOfSpot2; j--)
                        {
                            listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                        }
                    }
                    else
                    {
                        for (int j = numberOfSpot; j <= numberOfSpot2; j++)
                        {
                            listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                        }
                    }
                }
                else
                {
                    //Número origen mayor que destino
                    if (numberOfSpot > numberOfSpot2)
                    {
                        
                        if (numberOfSpot - numberOfSpot2 <= (numberOfSpotsWithTag - numberOfSpot + numberOfSpot2))
                        {
                            for (int j = numberOfSpot; j >= numberOfSpot2; j--)
                            {
                                listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                            }
                        }
                        else
                        {
                            for (int j = numberOfSpot; j >= numberOfSpotsWithTag; j++)
                            {
                                listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                            }

                            for (int j = 1; j <= numberOfSpot2; j++)
                            {
                                listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                            }
                        }
                    }
                    else
                    {
                        if (numberOfSpot2 - numberOfSpot <= (numberOfSpotsWithTag - numberOfSpot2 + numberOfSpot))
                        {
                            for (int j = numberOfSpot; j <= numberOfSpot2; j++)
                            {
                                listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                            }
                        }
                        else
                        {
                            for (int j = numberOfSpot; j >= 1; j--)
                            {
                                listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                            }

                            for (int j = numberOfSpotsWithTag; j >= numberOfSpot2; j--)
                            {
                                listWithTheShortestPath.Add(GameObject.Find($"{tagSpot}_{j}").GetComponent<MovementSpot>());
                            }
                        }
                    }
                }
            }
        }

        //Añado el punto de conexión
        listWithTheShortestPath.Add(shortestKeyPath[shortestKeyPath.Count - 2]);

        possibleTower = listWithTheShortestPath[listWithTheShortestPath.Count - 1] as MovementSpot;
        //Si el punto destino está en una torre, añado el punto torre antes.
        if (possibleTower.TowerSpot.name != "Auxiliar Spot")
        {
            listWithTheShortestPath.Add(possibleTower.TowerSpot);
        }

        //Añado en cualquier caso el punto destino.
        listWithTheShortestPath.Add(shortestKeyPath[shortestKeyPath.Count - 1]);
      
        return listWithTheShortestPath;

        
    }

    /// <summary>
    /// Devuelve el punto más cercano por el que el soldado se mueve entre pisos
    /// </summary>
    /// <returns>
    /// Punto por más cercano para moverse entre pisos.
    /// </returns>
    private AttackSpot GetClosestStairsSecondFloor(AttackSpot attackSpot)
    {
        float lowestDistance = 1000f;
        AttackSpot aux = null;
        AttackSpot stairs = null;
        if (attackSpot != null)
            aux = attackSpot;
        else
            aux = _currentSpot as AttackSpot;

        //Se mira los puntos clave del punto de ataque en el que estoy, que se corresponden con las escaleras
        //El más cercano es el adecuado
        foreach(KeyValuePair<MovementSpot, float> keySpot in aux.GetConnectionSpot.KeySpots)
        {
            if (keySpot.Value <= lowestDistance)
            {
                lowestDistance = keySpot.Value;
                switch (keySpot.Key.gameObject.name)
                {
                    case "D3_18":
                        stairs = GameObject.Find("P1_10").GetComponent<AttackSpot>(); 
                        break;
                    case "D3_28":
                        stairs = GameObject.Find("P1_11").GetComponent<AttackSpot>();
                        break;
                    case "D3_44":
                        stairs = GameObject.Find("P1_12").GetComponent<AttackSpot>();
                        break;
                }
            }
        }

        return stairs;
    }

    private void CalculateKeyPathFirstFloor(MovementSpot origin, AttackSpot destination, ref float lowestDistance, bool close)
    {
        shortestKeyPath.Add(_currentSpot);
        shortestKeyPath.Add(origin);
        if (!close)
        {
            foreach (KeyValuePair<MovementSpot, float> keySpotDestination in destination
                         .GetConnectionSpot.KeySpots)
            {
                if (origin.KeySpots.Keys.ToList().Contains(keySpotDestination.Key))
                {
                    shortestKeyPath.Add(keySpotDestination.Key);
                    lowestDistance += keySpotDestination.Value;
                }
            }

        }
        shortestKeyPath.Add(destination.GetConnectionSpot);
        shortestKeyPath.Add(destination);
        lowestDistance += Vector3.Distance(_currentSpot.transform.position,
                              origin.transform.position)
                          + Vector3.Distance(origin.transform.position,
                              destination.GetConnectionSpot.transform.position)
                          + Vector3.Distance(destination.GetConnectionSpot.transform.position,
                              destination.transform.position);
       
        
    }

    private List<Spot> AddPathSameRing(MovementSpot origin, MovementSpot connectionSpot, ref float lowestDistance, bool right)
    {
        List<Spot> shortestPathAux = new List<Spot>();
        shortestPathAux.Add(origin);
        string[] subStringOrigin = new string[2];
        string[] subStringDestination = new string[2];
        int numberOrigin = 0;
        int numberDestination = 0;
        subStringOrigin = origin.gameObject.name.Split('_');
        numberOrigin = int.Parse(subStringOrigin[1]);
        subStringDestination = connectionSpot.gameObject.name.Split('_');
        numberDestination = int.Parse(subStringDestination[1]);
        MovementSpot previousSpot = origin;
        string tagSpot = origin.gameObject.tag;
        int numberObjectsInTag = GameObject.FindGameObjectsWithTag(tagSpot).Length;
        if (numberOrigin == numberDestination)
        {
            return shortestPathAux;
        }

        if (right)
        {
            if (numberOrigin > numberDestination)
            {
            
                for (int i = numberOrigin; i <= numberObjectsInTag; i++)
                {
                    MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                    if (aux.IsKeySpot)
                    {
                        shortestPathAux.Add(aux);
                        lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                        previousSpot = aux;
                    }
                }
                for (int i = 1; i <= numberDestination; i++)
                {
                    MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                    if (aux.IsKeySpot)
                    {
                        shortestPathAux.Add(aux);
                        lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                        previousSpot = aux;
                    }
                }
            }
            else
            {
                for (int i = numberOrigin; i <= numberDestination; i++)
                {
                    MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                    if (aux.IsKeySpot)
                    {
                        shortestPathAux.Add(aux);
                        lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                        previousSpot = aux;
                    }
                }
            }
        }
        else
        {
            if (numberOrigin > numberDestination)
            {

                for (int i = numberOrigin; i >= numberDestination; i--)
                {
                    MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                    if (aux.IsKeySpot)
                    {
                        shortestPathAux.Add(aux);
                        lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                        previousSpot = aux;
                    }
                }
            }

            else
            {
                for (int i = numberOrigin; i >= numberObjectsInTag; i--)
                {
                    MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                    if (aux.IsKeySpot)
                    {
                        shortestPathAux.Add(aux);
                        lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                        previousSpot = aux;
                    }
                }

                for (int i = numberOrigin; i >= 1; i--)
                {
                    MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                    if (aux.IsKeySpot)
                    {
                        shortestPathAux.Add(aux);
                        lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                        previousSpot = aux;
                    }
                }
                for (int i = numberObjectsInTag; i >= numberDestination; i--)
                {
                    MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                    if (aux.IsKeySpot)
                    {
                        shortestPathAux.Add(aux);
                        lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                        previousSpot = aux;
                    }
                }
            }
        }
        
        shortestPathAux.Add(connectionSpot);
        return shortestPathAux;
    }
    
    private void CalculateKeyPathSecondFloor(MovementSpot origin, AttackSpot destination, ref float lowestDistance)
    {
        shortestKeyPath.Add(origin);
        string[] subStringOrigin = new string[2];
        string[] subStringDestination = new string[2];
        int numberOrigin = 0;
        int numberDestination = 0;
        subStringOrigin = origin.gameObject.name.Split('_');
        numberOrigin = int.Parse(subStringOrigin[1]);
        subStringDestination = destination.GetConnectionSpot.gameObject.name.Split('_');
        numberDestination = int.Parse(subStringDestination[1]);
        MovementSpot previousSpot = origin;
        string tagSpot = origin.gameObject.tag;

        //Voy hacia la izquierda
        if (numberOrigin > numberDestination)
        {
            for (int i = numberOrigin; i >= numberDestination; i--)
            {
                MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                if (aux.IsKeySpot)
                {
                    shortestKeyPath.Add(aux);
                    lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                    previousSpot = aux;
                }
            }
        }
        else
        {
            for (int i = numberOrigin; i <= numberDestination; i++)
            {
                MovementSpot aux = GameObject.Find($"{tagSpot}_{i}").GetComponent<MovementSpot>();
                if (aux.IsKeySpot)
                {
                    shortestKeyPath.Add(aux);
                    lowestDistance += Vector3.Distance(previousSpot.transform.position, aux.transform.position);
                    previousSpot = aux;
                }
            }
        }
        shortestKeyPath.Add(destination.GetConnectionSpot);
        shortestKeyPath.Add(destination);
    }

    /// <summary>
    /// Comprueba si el origen y el destino están en diferentes pisos.
    /// </summary>
    /// <param name="destination">
    /// Destino al que se mueve el personaje
    /// </param>
    /// <returns>
    /// Devuelve true si el punto actual está en el mismo piso que destino, false en caso contrario.
    /// </returns>
    private bool CheckSameFloor(AttackSpot destination)
    {
        if (_currentSpot.gameObject.layer != destination.gameObject.layer)
            return false;
        return true;
        
    }

    private int SameKeySpots(AttackSpot origin, AttackSpot destination)
    {
        int coincidences = 0;
        foreach (MovementSpot spot in origin.GetConnectionSpot.ClosestStairs)
        {
            if (destination.GetConnectionSpot.ClosestStairs.Contains(spot))
                coincidences++;
        }

        return coincidences;

    }

    private bool KeySpotFromEachOther(AttackSpot origin, AttackSpot destination)
    {
        if (origin.GetConnectionSpot.KeySpots.Keys.ToList().Contains(destination.GetConnectionSpot)
            || destination.GetConnectionSpot.KeySpots.Keys.ToList().Contains(origin.GetConnectionSpot))
                return true;
        return false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        _target = collision.gameObject.GetComponent<Character>(); 
        LookAt(_target.transform.position);
        if (GetComponentInParent<Weapon>().Shoot("Ally Bullet", 2, _target, GetComponent<Character>()))
            _animator.SetBool("Shoot", true);
    }
}