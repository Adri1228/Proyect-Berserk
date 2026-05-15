using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float runSpeed = 7f;
    public float rotationSpeed = 250f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Componentes")]
    public Animator animator;
    public Rigidbody rb;

    [Header("Vida")]
    public int maxVida = 5;
    public float vida = 5f;

    [Header("Experiencia y Nivel")]
    public float xpActual = 0f;
    public float xpParaSiguienteNivel = 100f;
    public int nivel = 1;
    public float xpMultiplicador = 1.4f;

    [Header("Proyectil")]
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;

    [Header("Ataque Automático")]
    public float attackRadius = 50f;
    public int attackDamage = 1;
    public float attackCooldown = 2f;
    public LayerMask enemyMask;

    private float x, y;
    private bool isGrounded;
    private float attackTimer = 0f;
    private bool muerto = false;

    private LevelUpManager levelUpManager;
    private GameManager gameManager;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        levelUpManager = FindObjectOfType<LevelUpManager>();
        gameManager = FindObjectOfType<GameManager>();

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.AplicarAlJugador(this);

        // Empezar el timer ya casi lleno para que dispare pronto al inicio
        attackTimer = attackCooldown * 0.9f;
    }

    public void RecibirGolpe(int daño)
    {
        if (muerto) return;
        vida -= daño;
        vida = Mathf.Max(vida, 0);

        if (vida <= 0)
            Morir();
    }

    void Morir()
{
    if (muerto) return;
    muerto = true;

    
    if (gameManager != null)
        gameManager.GameOver();
}

    public void GanarXP(float cantidad)
    {
        if (muerto) return;
        xpActual += cantidad;

        if (xpActual >= xpParaSiguienteNivel)
        {
            xpActual -= xpParaSiguienteNivel;
            xpParaSiguienteNivel *= xpMultiplicador;
            nivel++;

            int curacion = Mathf.Max(1, maxVida / 4);
            vida = Mathf.Min(vida + curacion, maxVida);

            if (levelUpManager != null)
                levelUpManager.MostrarMejoras();
        }
    }

    void Update()
    {
        if (muerto) return;

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (animator != null)
        {
            animator.SetFloat("VelX", x);
            animator.SetFloat("VelY", y);
            animator.SetBool("EnSuelo", isGrounded);
        }

        // Usar unscaledDeltaTime para que el timer siga contando
        // incluso cuando el juego está pausado por el LevelUp
        attackTimer += Time.unscaledDeltaTime;

        if (attackTimer >= attackCooldown)
        {
            Collider[] enemigos = Physics.OverlapSphere(
                transform.position, attackRadius, enemyMask);

            if (enemigos.Length > 0)
            {
                // Guardar el target en una variable — NO llamar GetClosestEnemy dos veces
                Transform target = GetClosestEnemy(enemigos);

                if (target != null)
                {
                    // Rotar hacia el enemigo
                    Vector3 lookDir = (target.position - transform.position).normalized;
                    lookDir.y = 0;
                    if (lookDir.sqrMagnitude > 0.001f)
                        rb.MoveRotation(Quaternion.LookRotation(lookDir));

                    DispararProyectil(target);
                    attackTimer = 0f;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (muerto) return;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(
            0, x * rotationSpeed * Time.fixedDeltaTime, 0));
        Vector3 move = transform.forward * y * runSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    void DispararProyectil(Transform objetivo)
{
    if (proyectilPrefab == null || puntoDisparo == null || objetivo == null) return;

    // Apuntar al centro del enemigo (ajusta Y si el pivote es raro)
    Vector3 destino = objetivo.position + Vector3.up * 0.5f;
    Vector3 direccion = (destino - puntoDisparo.position).normalized;

    GameObject bola = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
    Proyectil p = bola.GetComponent<Proyectil>();

    if (p != null)
        p.Inicializar(direccion, attackDamage);
}

    Transform GetClosestEnemy(Collider[] enemigos)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (Collider e in enemigos)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e.transform;
            }
        }
        return closest;
    }
}