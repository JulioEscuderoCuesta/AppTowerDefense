using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProducerMovementSpotForDefender : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Si este spots es clave o no")]
    private bool keySpot;

    private Dictionary<string, float> keySpots;
    private MovementSpot leftKeySpot;
    private MovementSpot rightKeySpot;

    private List<GameObject> spotsInTheLayer;
    private List<string> listOfSpotsInTheLayer;
    private int numberOfObjectsWithSameTag;



    private void Start()
    {
        keySpots = new Dictionary<string, float>();
        spotsInTheLayer = new List<GameObject>();
        listOfSpotsInTheLayer = new List<string>();
        numberOfObjectsWithSameTag = GameObject.FindGameObjectsWithTag(gameObject.tag).Length;
        CalculateKeySpots();
        Consumer.SharedInstance.AddDataMovementSpotForDefenders(gameObject.name, keySpots, keySpot);

    }

    private void CalculateKeySpots()
    {
        //Cojo la posición que ocupo en la lista de objetos de mi capa y desde ahí recorro la lista hacia delante.
        //El primer objeto que encuentre que esté marcado como spot clave, será el spot clave de la derecha.
        //Lo mismo para calcular el spot clave izquierdo. Recorro la lista hacia atrás y el primer spot clave es el buscado.
        string[] currentSubString = gameObject.name.Split('_');
        int currentNumber = int.Parse(currentSubString[1]);

        for (int i = currentNumber + 1; i < numberOfObjectsWithSameTag; i++)
        {
            string actualSpot = currentSubString[0] + '_' + i;
            if (GameObject.Find(actualSpot).GetComponent<ProducerMovementSpotForDefender>().keySpot)
            {
                rightKeySpot = GameObject.Find(actualSpot).GetComponent<MovementSpot>();
				
                break;
            }
        }

        for (int i = currentNumber - 1; i > 0; i--)
        {
            string actualSpot = currentSubString[0] + '_' + i;
            if (GameObject.Find(actualSpot).GetComponent<ProducerMovementSpotForDefender>().keySpot)
            {
                leftKeySpot = GameObject.Find(actualSpot).GetComponent<MovementSpot>();
                break;
            }
        }

        // (cambiar)Si el punto es uno del piso superior y no hay punto clave a la izquierda o a la derecha
        //entonces no busco más.
        if (gameObject.tag != "D3")
        {
            //En cualquier otro caso
            //Si recorriendo hacia delante o hacia atrás no he encontrado algún punto clave, es porque:
            //El punto clave de la derecha es uno de los primeros puntos de la lista
            //El punto clave de la izquierda es uno de los últimos puntos de la lista.
            if (leftKeySpot == null)
            {
                for (int i = numberOfObjectsWithSameTag; i > 0; i--)
                {
                    string actualSpot = currentSubString[0] + '_' + i;
                    if (GameObject.Find(actualSpot).GetComponent<ProducerMovementSpotForDefender>().keySpot)
                    {
                        leftKeySpot = GameObject.Find(actualSpot).GetComponent<MovementSpot>();
                        break;
                    }
                }
            }
            if (rightKeySpot == null)
            {
                for (int i = 1; i <= numberOfObjectsWithSameTag; i++)
                {
                    string actualSpot = currentSubString[0] + '_' + i;
                    if (GameObject.Find(actualSpot).GetComponent<ProducerMovementSpotForDefender>().keySpot)
                    {
                        rightKeySpot = GameObject.Find(actualSpot).GetComponent<MovementSpot>();
                        break;
                    }
                }
            }
        }

		if(rightKeySpot != null)
		{
            float dist = Vector3.Distance(rightKeySpot.transform.position, transform.position);
            keySpots.Add(rightKeySpot.gameObject.name, Vector3.Distance(transform.position, rightKeySpot.transform.position));

		}
        if (leftKeySpot != null)
        {
            float dist2 = Vector3.Distance(leftKeySpot.transform.position, transform.position);
            keySpots.Add(leftKeySpot.gameObject.name, Vector3.Distance(transform.position, leftKeySpot.transform.position));
        }
    }
    


    private void OnDrawGizmosSelected()
    {

    }
}
