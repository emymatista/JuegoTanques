using UnityEngine;

public class TankMovement : MonoBehaviour
{
    //Utilizado para identificar que el tanque es de cada jugador. Configurado por el tank manager
    public int m_PlayerNumber = 1;   
    //Rapidez con la que el tanque se mueve alante y atras      
    public float m_Speed = 12f; 
    //Rapidez de giro del tanque en grados por segundo            
    public float m_TurnSpeed = 180f;  
    //Audio del motor del tanque     
    public AudioSource m_MovementAudio;  
    //Audio del tanque sin moverse  
    public AudioClip m_EngineIdling; 
    //Audio del tanque moviendose      
    public AudioClip m_EngineDriving;
    //Cantidad de variacion de afinacion del audio del motor   
    public float m_PitchRange = 0.2f;

    //Nombre del eje para moverse alante y atras
    private string m_MovementAxisName; 
    //Nombre del eje para girar    
    private string m_TurnAxisName;
    //Referencia del componente para mover el tanque         
    private Rigidbody m_Rigidbody;   
    //Valor actual de entrada para el movimiento      
    private float m_MovementInputValue; 
    //Valor actual de entrada para el giro   
    private float m_TurnInputValue;    
    //Valor del pitch de 1 a fuente de audio al inicio    
    private float m_OriginalPitch;         


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        // Al arrancar/habilitar el tanque, deshabilitamos la kinematica del
        //tanque para que se pueda mover.
        m_Rigidbody.isKinematic = false;
        //Reseteamos los valores de entrada
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        /// Al parar/deshabilitar el tanque, habilitamos la kinematica del
        //tanque para que se pare.
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        //Nombres de los ejes segun el numero de jugador
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        //Almaceno la afinacion original del audio del motor
        m_OriginalPitch = m_MovementAudio.pitch;
    }
    

    private void Update()
    {
        //Almaceno los valores de entrada
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        //Llamo a la funcion que gestiona el audio del motor
        EngineAudio();
    }


    private void EngineAudio()
    {
        //Si no hay entrada, es que esta quieto...
        if(Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
        {
            //....y si estaba reproduciendo el audio de moverse...
            if(m_MovementAudio.clip == m_EngineDriving)
            {
                //.... cambio el audio al de estar parado y lo reproduzco.
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            //Si hay entrada es que se esta moviendo. Si estaba reproduciendo el de estar parado...
            if(m_MovementAudio.clip == m_EngineIdling)
            {
                //.... cambio el audio al de moverse y lo reproduzoco.
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }

        }

    }


    private void FixedUpdate()
    {
        Move();
        Turn();
    }


    private void Move()
    {
        //Creo un vector en la direccion en la que apunta el tanque,
        //con una magnitud basada en la entrada, la velocidad y el tiempo entre frames.

        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        //Aplico ese vector de movimiento al rigidbody del tanque

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        //Calculo el numero de grados de rotacion basandome a entrada, la velocidad y el tiempo entre frames
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        //Convierto ese numero en una rotacion en el eje Y.
        Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

        //Aplico esa rotacion al rigidbody del tanque
        m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }
}