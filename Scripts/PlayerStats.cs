using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Estadísticas persistentes")]
    public int nivel = 1;
    public float xpActual = 0f;
    public float xpParaSiguienteNivel = 100f;
    public float xpMultiplicador = 1.4f;
    public int maxVida = 5;
    public float vida = 5f;
    public float runSpeed = 7f;
    public float attackRadius = 50f;
    public int attackDamage = 1;
    public float attackCooldown = 2f;
    public int escenaGuardada = 1; // Escena donde se guardó

    public bool tieneDataGuardada = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // MEMORIA (entre escenas, sin disco)
    public void GuardarDesdeJugador(PlayerMovement p)
    {
        nivel              = p.nivel;
        xpActual           = p.xpActual;
        xpParaSiguienteNivel = p.xpParaSiguienteNivel;
        xpMultiplicador    = p.xpMultiplicador;
        maxVida            = p.maxVida;
        vida               = p.vida;
        runSpeed           = p.runSpeed;
        attackRadius       = p.attackRadius;
        attackDamage       = p.attackDamage;
        attackCooldown     = p.attackCooldown;
        tieneDataGuardada  = true;
    }

    public void AplicarAlJugador(PlayerMovement p)
    {
        if (!tieneDataGuardada) return;
        p.nivel                = nivel;
        p.xpActual             = xpActual;
        p.xpParaSiguienteNivel = xpParaSiguienteNivel;
        p.xpMultiplicador      = xpMultiplicador;
        p.maxVida              = maxVida;
        p.vida                 = vida;
        p.runSpeed             = runSpeed;
        p.attackRadius         = attackRadius;
        p.attackDamage         = attackDamage;
        p.attackCooldown       = attackCooldown;
    }
    // DISCO (persiste entre sesiones con PlayerPrefs)
    public void GuardarEnDisco(int escena)
    {
        PlayerPrefs.SetInt  ("sv_existe",      1);
        PlayerPrefs.SetInt  ("sv_escena",      escena);
        PlayerPrefs.SetInt  ("sv_nivel",       nivel);
        PlayerPrefs.SetFloat("sv_xpActual",    xpActual);
        PlayerPrefs.SetFloat("sv_xpSiguiente", xpParaSiguienteNivel);
        PlayerPrefs.SetFloat("sv_xpMult",      xpMultiplicador);
        PlayerPrefs.SetInt  ("sv_maxVida",     maxVida);
        PlayerPrefs.SetFloat("sv_vida",        vida);
        PlayerPrefs.SetFloat("sv_speed",       runSpeed);
        PlayerPrefs.SetFloat("sv_radius",      attackRadius);
        PlayerPrefs.SetInt  ("sv_damage",      attackDamage);
        PlayerPrefs.SetFloat("sv_cooldown",    attackCooldown);
        PlayerPrefs.Save();

        escenaGuardada    = escena;
        tieneDataGuardada = true;

        Debug.Log("Partida guardada — Escena: " + escena + " | Nivel: " + nivel);
    }

    public void CargarDesdeDisco()
    {
        if (PlayerPrefs.GetInt("sv_existe", 0) == 0)
        {
            Debug.Log("No hay partida guardada");
            return;
        }

        escenaGuardada       = PlayerPrefs.GetInt  ("sv_escena",      1);
        nivel                = PlayerPrefs.GetInt  ("sv_nivel",       1);
        xpActual             = PlayerPrefs.GetFloat("sv_xpActual",    0f);
        xpParaSiguienteNivel = PlayerPrefs.GetFloat("sv_xpSiguiente", 100f);
        xpMultiplicador      = PlayerPrefs.GetFloat("sv_xpMult",      1.4f);
        maxVida              = PlayerPrefs.GetInt  ("sv_maxVida",     5);
        vida                 = PlayerPrefs.GetFloat("sv_vida",        5f);
        runSpeed             = PlayerPrefs.GetFloat("sv_speed",       7f);
        attackRadius         = PlayerPrefs.GetFloat("sv_radius",      50f);
        attackDamage         = PlayerPrefs.GetInt  ("sv_damage",      1);
        attackCooldown       = PlayerPrefs.GetFloat("sv_cooldown",    2f);
        tieneDataGuardada    = true;

        Debug.Log("Partida cargada — Escena: " + escenaGuardada + " | Nivel: " + nivel);
    }

    public static bool ExistePartidaGuardada()
    {
        return PlayerPrefs.GetInt("sv_existe", 0) == 1;
    }

    public void BorrarPartidaGuardada()
    {
        PlayerPrefs.DeleteKey("sv_existe");
        PlayerPrefs.Save();
        Resetear();
    }

    public void Resetear()
    {
        tieneDataGuardada    = false;
        nivel                = 1;
        xpActual             = 0f;
        xpParaSiguienteNivel = 100f;
        xpMultiplicador      = 1.4f;
        maxVida              = 5;
        vida                 = 5f;
        runSpeed             = 7f;
        attackRadius         = 50f;
        attackDamage         = 1;
        attackCooldown       = 2f;
    }
}