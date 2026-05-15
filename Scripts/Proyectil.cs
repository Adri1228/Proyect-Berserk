using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public int daño = 1;
    public float velocidad = 15f;
    public float tiempoVida = 4f; // Se destruye solo si no golpea nada

    private Vector3 direccion;

    public void Inicializar(Vector3 dir, int dmg)
    {
        direccion = dir.normalized;
        daño = dmg;
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.position += direccion * velocidad * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Golpear enemigo
        EnemyController ec = other.GetComponent<EnemyController>();
        if (ec != null)
        {
            ec.TakeDamage(daño);
            Destroy(gameObject);
            return;
        }

        // Destruirse al tocar cualquier otra cosa (suelo, paredes...)
        // excepto al propio jugador
        if (!other.CompareTag("Player"))
            Destroy(gameObject);
    }
}