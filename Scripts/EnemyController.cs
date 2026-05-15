using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2f;
    public float rotationSpeed = 5f;
    public float detectionRadius = 5f;

    [Header("Patrulla aleatoria")]
    public float patrolRadius = 100f;
    public float patrolChangeTimeMin = 2f;
    public float patrolChangeTimeMax = 5f;

    [Header("Vida")]
    public int maxHealth = 5;
    private int currentHealth;

    public Transform player;
    public Animator animator;
    public Rigidbody rb;

    private Vector3 movement;
    private Quaternion targetRotation;

    private Vector3 patrolTarget;
    private float patrolTimer;
    private float nextPatrolChangeTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        targetRotation = transform.rotation;

        currentHealth = maxHealth;

        ChooseNewPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        if (currentHealth <= 0) return; // no moverse si está muerto

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectionRadius)
        {
            // ===== PERSEGUIR JUGADOR =====
            Vector3 direction = (player.position - transform.position).normalized;
            movement = new Vector3(direction.x, 0, direction.z);
        }
        else
        {
            // ===== PATRULLA ALEATORIA =====
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= nextPatrolChangeTime ||
                Vector3.Distance(transform.position, patrolTarget) < 1f)
            {
                ChooseNewPatrolPoint();
            }

            Vector3 direction = (patrolTarget - transform.position).normalized;
            movement = new Vector3(direction.x, 0, direction.z);
        }

        // ===== ROTACIÓN SEGURA =====
        if (movement.sqrMagnitude > 0.0001f)
        {
            targetRotation = Quaternion.LookRotation(movement.normalized);
        }

        // ===== ANIMATOR =====
        Vector3 localMove = transform.InverseTransformDirection(movement);
        animator.SetFloat("VelX", localMove.x);
        animator.SetFloat("VelY", localMove.z);
    }

    void FixedUpdate()
    {
        if (currentHealth <= 0) return; // no moverse si está muerto

        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void ChooseNewPatrolPoint()
    {
        patrolTimer = 0f;
        nextPatrolChangeTime = Random.Range(patrolChangeTimeMin, patrolChangeTimeMax);

        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        patrolTarget = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y
        );
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
{
    movement = Vector3.zero;
    animator.SetTrigger("Die");

    // XP al jugador
    PlayerMovement pm = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerMovement>();
    if (pm != null) pm.GanarXP(25f);

    // Avisar al spawner
    EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
    if (spawner != null) spawner.EnemyDefeated();

    Destroy(gameObject, 2f);
}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var pm = collision.gameObject.GetComponent<PlayerMovement>();
            if (pm != null)
                pm.RecibirGolpe(1);
        }
    }
}
