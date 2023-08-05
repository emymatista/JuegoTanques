using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Numero de rondas que un jugador debe ganar para ganar el juego
    public int m_NumRoundsToWin = 5;    
    //Delay entre las fases de RoundStarting y RoundPlaying    
    public float m_StartDelay = 3f;
    //Delay entre las fases de RoundPlaying y RoundEnding        
    public float m_EndDelay = 3f;     
    //Referencia al script de CameraControl      
    public CameraControl m_CameraControl; 
    //Referencia al texto para mostrar mensajes  
    public Text m_MessageText;       
    //Referncia al Prefab del Tanque       
    public GameObject m_TankPrefab;     
    //Array de TankManagers para controlar cada tanque    
    public TankManager[] m_Tanks;           


    //Numero de ronda
    private int m_RoundNumber;  
    //Delay hasta que la ronda empieza            
    private WaitForSeconds m_StartWait; 
    //Delay hasta que la ronda acaba    
    private WaitForSeconds m_EndWait;
    //Referencia al ganador de la ronda para anunciar quien ha ganado       
    private TankManager m_RoundWinner;
    //Referencia al ganador del juego para anunciar quien ha ganado
    private TankManager m_GameWinner;       


    private void Start()
    {
        //Creamos los delays para que solo se apliquen una vez
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        //Generar tanques
        SpawnAllTanks();
        //Ajustar camara
        SetCameraTargets();

        //Iniciar juego
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        //Recorro los tanques
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //...los creo, ajusto el numero de jugador y las referencias necesarias para controlarlo
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        //Creo un array de Transforms del mismo tamaño que el numero de tanques
        Transform[] targets = new Transform[m_Tanks.Length];

        //Recorro los Transforms...
        for (int i = 0; i < targets.Length; i++)
        {
            //...lo ajusto al transform del tanque apropiado
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        //Estos son los targets que la camara debe seguir
        m_CameraControl.m_Targets = targets;
    }

    //Llamado al principio y en cada fase del juego depues de otra
    private IEnumerator GameLoop()
    {
        //Empiezo con la corutina RoundStaring y no retorno hasta que finalice
        yield return StartCoroutine(RoundStarting());
        //Cuando finalice RoundStaring, empiezo con RoundPlaying y no retorno hasta que finalice 
        yield return StartCoroutine(RoundPlaying());
        //Cuando finalice RoundPlaying, empiezo con RoundEnding y no retorno hasta que finalice
        yield return StartCoroutine(RoundEnding());

        //Si aun no ha ganado ninguno
        if (m_GameWinner != null)
        {
            //Si hay un ganador, reinicio el nivel
            SceneManager.LoadScene(0);
        }
        else
        {
            //Si no, reinicio las corutinas para que continue el bucle
            //En este caso sin yiend, de modo que esta version del GameLoop finalizara siempre
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        //Cuando empiece la ronda reseteo los tanques e impido que se muevan
        ResetAllTanks();
        DisableTankControl();

        //Ajusto la camara a los tanques reseteados.
        m_CameraControl.SetStartPositionAndSize();

        //Incremento la ronda y muestro el texto informativo
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        //Espero a que pase el tiempo de espera antes de volver al bucle
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        //Cuando empiece la ronda dejo que los tanques se muevan
        EnableTankControl();

        //Borro el texto de la pantalla
        m_MessageText.text = string.Empty;

        //Mientras haya mas de un tanque
        while(!OneTankLeft())
        {
            //... vuelvo al frame siguiente
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        //Deshabilito el movimiento de los tanques
        DisableTankControl();

        //Borro al ganador de la ronda anterior
        m_RoundWinner = null;

        //Miro si hay un ganador de la ronda.
        m_RoundWinner = GetRoundWinner();

        //Si lo hay, incremento su puntuacion
        if(m_RoundWinner != null)
           m_RoundWinner.m_Wins++;
        
        //Compruebo si alguien ha ganado el juego
        m_GameWinner = GetGameWinner();

        //Genero el mensaje segun si hay un ganador del juego o no
        string message = EndMessage();
        m_MessageText.text = message;

        //Espero a que pase el tiempo de espera antes de volver al bucle
        yield return m_EndWait;
    }

    //Usado para comprobar si queda mas de un tanque
    private bool OneTankLeft()
    {
        //Contador de tanques
        int numTanksLeft = 0;

        //recorro los tanques...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //...si esta activo, incremento el contador
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        //Devuelvo true si queda 1 o menos, false si queda mas de uno.
        return numTanksLeft <= 1;
    }

    //Comprueba si algun tanque ha ganado la ronda (si queda un tanque o menos)
    private TankManager GetRoundWinner()
    {
        //Recorro los tanques...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //... si solo queda uno, es el ganador y lo devuelvo
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }
        //Si no hay ninguno activo es un empate, asi que devuelvo null
        return null;
    }

    //Comprueba si hay algun ganador del juego.
    private TankManager GetGameWinner()
    {
        //Recorro los tanques...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            //...sin alguno tiene las rondas necesarias, ha ganado y lo devuelvo
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        //Si no, devuelvo null.
        return null;
    }

    //Devuelve el texto del mensaje a mostrar al final de cada ronda
    private string EndMessage()
    {
        // Por defecto no hay ganadores, asi que es empate
        string message = "EMPATE!";

        //Si hay un ganador de ronda cambio el mensaje
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " GANA LA RONDA!";
        
        //Retornos de carro
        message += "\n\n\n\n";

        //Recorro los tanques y añado sus puntuaciones
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " GANA\n";
        }

        //Si hay un ganador del juego, cambio el mensaje entero para reflejarlo
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " GANA EL JUEGO!";

        return message;
    }

    //Para resetear los tanques (propiedades, posiciones, etc.).
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }

    //Habilita el control del tanque
    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }

    //Deshabilita el control del tanque
    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}