using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    //Salud de inicio
    public float m_StartingHealth = 100f;   
    //Slider que representa la salud del tanque       
    public Slider m_Slider;   
    //Componente de imagen del slider                     
    public Image m_FillImage;   
    //Color del slider don salud completa (verde)                   
    public Color m_FullHealthColor = Color.green;
    //Color del slider con salud vacia (rojo)  
    public Color m_ZeroHealthColor = Color.red;
    //Prefab que instanciamos al inicio y usamos cuando el tanque se muere    
    public GameObject m_ExplosionPrefab;
    
    //La fuente de audio a reproducir cuando el tanque explota
    private AudioSource m_ExplosionAudio;
    //Sistema de particulas que se reproducen al destruir el tanque          
    private ParticleSystem m_ExplosionParticles;  
    //Variable para almacenar la salud del tanque 
    private float m_CurrentHealth;  
    //Variable para comprobar si el tanque tiene salud
    private bool m_Dead;            


    private void Awake()
    {
        //Instanciamos el prefab de la explosion
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        //Referencia de la fuente de audio para la explosion
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        //Deshabilitamos el sistema de particulas de la explosion(para activarlo cuando explote)
        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        //Al habilitar el tanque, reseteamos la salud y el booleano de si esta muerto o no
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        //Actualizamos el slider de salud (valor y color)
        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        //Reducimos la salud segun la cantidad de daño recibida.
        m_CurrentHealth -= amount;

        //Actualizamos el slider de salud con esos valores
        SetHealthUI();

        //Si la salud es menor que 0 y aun no lo he explotado, llamo al
        //metodo OnDeath (al morir).
        if(m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        //Ajusto el valor del slider.
        m_Slider.value = m_CurrentHealth;

        //Creo un color para el slider entre verde y rojo en funcion
        //del porcentaje de salud
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);

    }


    private void OnDeath()
    {
        //Configuro el booleano a true para asegurarme de que explota solo una vez.
        m_Dead = true;

        //Coloco el prefab de explosion en la posicion actual del tanque y lo activo.
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        //Reproduzco el sistema de particulas del tanque explotando.
        m_ExplosionParticles.Play();

        //Reproduzco el audio del tanque explotando
        m_ExplosionAudio.Play();

        //Desactivo el tanque.
        gameObject.SetActive(false);
    }
}