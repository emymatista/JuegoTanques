using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    //Para identificar a los diferentes jugadores
    public int m_PlayerNumber = 1;
    //Prefab de la bomba       
    public Rigidbody m_Shell;
    //Hijo del tanque en el que se generara la bomba (desde donde se lanzara)            
    public Transform m_FireTransform;   
    //Hijo del tanque que muestra la fuerza de lanzamiento de la bomba 
    public Slider m_AimSlider;  
    //Referencia a la fuente de audio que se reproducira al lanzar la bomba        
    public AudioSource m_ShootingAudio;
    //Clip de audio que se reproduce cuando esta cargando el disparo  
    public AudioClip m_ChargingClip; 
    //Clip de audio que se reproduce al lanzar bomba    
    public AudioClip m_FireClip;  
    //Fuerza minima de disparo (si no se mantiene presionado el boton de disparo)       
    public float m_MinLaunchForce = 15f; 
    //Fuerza maxima de disparo (si se mantiene presionado el boton de disparo hasta la maxima carga)
    public float m_MaxLaunchForce = 30f; 
    //Tiempo maximo de carga antes de ser lanzado el disparo con maxima fuerza
    public float m_MaxChargeTime = 0.75f;

    //Eje de disparo utilizado para lanzar las bombas
    private string m_FireButton;         
    //Fuerza dada a la bomba cuando se suelta el boton de disparo
    private float m_CurrentLaunchForce;  
    //Velocidad de carga, basada en el maximo tiempo de carga
    private float m_ChargeSpeed;         
    //Booleano que comprueba si se ha lanzado la bomba
    private bool m_Fired;                


    private void OnEnable()
    {
        //Al crear el tanque, reseteo la fuerza de lanzamientoy la UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        //El eje de disparo basado en el numero de jugador
        m_FireButton = "Fire" + m_PlayerNumber;

        //Velocidad de carga, basada en el maximo tiempo de carga y los valores de carga maximo y minimo
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        //Asigno el valor minimo al slider
        m_AimSlider.value = m_MinLaunchForce;

        //Si llego al valor maximo y no lo he lanzado...
        if(m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            //...uso el valor maximo y disparo.
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        //Si no, si ya he pulsado el boton de disparo...
        else if(Input.GetButtonDown(m_FireButton))
        {
            //... reseteo el booleano de disparo y la fuerza de disparo
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;
            //Cambio el clip de audio al de cargando y lo reproduzco
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        //Si no, si estoy manteniendo presionado el boton de disparo y aun no he disparado...
        else if(Input.GetButton(m_FireButton) && !m_Fired)
        {
            //Incremento la fuerza de disparo y actualizo el slider.
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        //Si no, si ya he soltado el boton de disparo y aun no he lanzado...
        else if(Input.GetButtonUp (m_FireButton) && !m_Fired)
        {
            //...disparo
            Fire();
        }
    }


    private void Fire()
    {
        //Ajusto el booleano a true para que solo se lance una vez
        m_Fired = true;

        //Creo una instancia de la bomba y guardo una referencia en su Rigidbody.
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        //Ajusto la velocidad de la bomba en la direccion de disparo
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        //Cambio el audio al de disparo y lo reporduzco
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        //Reseteo la fuerza de lanzamiento como precaucion ante posibles eventos de boton "perdidos".
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}