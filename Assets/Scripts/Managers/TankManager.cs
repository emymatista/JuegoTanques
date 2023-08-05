using System;
using UnityEngine;

// Hace que los atributos aparezcan en el inspector (si no los escondemos) 
[Serializable]
public class TankManager
{
    //Esta clase gestiona la configuracion del tanque junto con el GameManager
    //Gestiona el comportamiento de los tanques y si los jugadores tienen control
    //sobre el tanque en los distintos momentos del juego

    //Color para el tanque
    public Color m_PlayerColor;     
    //Posicion y direccion en la que se generara el tanque       
    public Transform m_SpawnPoint;
    //Especifica con que jugador esta actuando el Game Manager      
    [HideInInspector] public int m_PlayerNumber;  
    //String que representa el color del tanque           
    [HideInInspector] public string m_ColoredPlayerText;
    //Referencia a la instancia del tanque cuando se crea
    [HideInInspector] public GameObject m_Instance;      
    //Numero de victorias del jugador    
    [HideInInspector] public int m_Wins;                     

    //Referencia al script de movimiento de tanque. Utilizado para deshabilitar y habilitar el control
    private TankMovement m_Movement;  
    //Referencia al script de disparo del tanque. Utilizado para deshabilitar y habilitar el control     
    private TankShooting m_Shooting;
    //Utilizado para deshabilitar el UI del mundo durante las fases de inicio y fin de cada ronda
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        //Cojo referencias de los componentes
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        //Ajusto los numero de jugadores para que sean iguales en todos los scripts
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        //Creo un string usando el colora del tanque que diga PLAYER 1, etc.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        //Cojo todos los renderers del tanque
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        //Los recorro...
        for (int i = 0; i < renderers.Length; i++)
        {
            //..y ajusto el color del material al del tanque
            renderers[i].material.color = m_PlayerColor;
        }
    }

    //Usado duarnte las fases del juego en las que el jugador no debe poder controlar el tanque
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    //Usado durante las fases del juego en las que el jugador no debe poder controlar el tanque
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    //Usado al inicio de cada ronda para poner el tanque en su estado inicial
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
