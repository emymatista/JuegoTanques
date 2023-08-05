using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    //Usado para filtrar a que afecta la explosion de la bomba, Deberia ajustarse a "Players"
    public LayerMask m_TankMask;
    //Referencia a las particulas que se reporduciran en la explosion
    public ParticleSystem m_ExplosionParticles;
    //Referencia al audio que se reproducira en la explosion       
    public AudioSource m_ExplosionAudio; 
    //Cantidad de daño si la explsion esta centrada en el tanque             
    public float m_MaxDamage = 100f;                  
    //Cantidad de fuerza añadida al tanque en el centro de la explosion
    public float m_ExplosionForce = 1000f;            
    //Tiempo de visa en segundos de la bomba
    public float m_MaxLifeTime = 2f;        
    //Radio maximo desde la explosion para calcular los tanques que se veran afectados          
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        //Si no se ha destruido aun, destruir la bomba despues de su tiempo de vida
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Recoge los colliders en una esfera desde la posicion de la bomba con el radio maximo
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        //Recorro los colliders
        for(int i = 0; i < colliders.Length; i++)
        {
            //Seleccione su Rigidbody
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            
            //Si no tienen, paso al siguiente
            if(!targetRigidbody)
               continue;
            
            //Añado la fuerza de la explosion
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //Busco el script TankHealth asociado con el Rigidbody
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            //Si no hay script TankHealth, paso al siguiente
            if(!targetHealth)
               continue;
            
            //Calculo el daño a aplicar en funcion de la distancia a la bomba
            float damage = CalculateDamage(targetRigidbody.position);

            //Aplico el daño al tanque
            targetHealth.TakeDamage(damage);
        }

        //Desanclo el sistema de particulas de la bomba
        m_ExplosionParticles.transform.parent = null;

        //Reproduzco el sistema de particulas
        m_ExplosionParticles.Play();

        //Reproduzco el audio
        m_ExplosionAudio.Play();

        //Cuando las particulas han terminado, destruyo su objeto asociado
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        //Destruyo la bomba
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Creo un vector desde la bomba al objetivo
        Vector3 explosionToTarget = targetPosition - transform.position;

        //Calculo la distancia desde la bomba al objetivo
        float explosionDistance = explosionToTarget.magnitude;

        //Calculo la proporcion de maxima distancia (radio maximo) desde la explosion al tanque
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        //Calculo el daño a esa proporcion
        float damage = relativeDistance * m_MaxDamage;

        //Me aseguro de que el minimo daño siempre es 0
        damage = Mathf.Max(0f, damage);

        //Devuelvo el daño
        return damage;
    }
}