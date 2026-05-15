using UnityEngine;


public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    private bool pausadoPorLevelUp = false;
    private bool pausadoPorPausa = false;
    private bool pausadoPorGameOver = false;
    private bool pausadoPorFinNivel = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void ActualizarTimeScale()
    {
        bool debePausar = pausadoPorLevelUp || pausadoPorPausa
                       || pausadoPorGameOver || pausadoPorFinNivel;
        Time.timeScale = debePausar ? 0f : 1f;
    }

    public void SetLevelUp(bool activo)
    {
        pausadoPorLevelUp = activo;
        ActualizarTimeScale();
    }

    public void SetPausa(bool activo)
    {
        pausadoPorPausa = activo;
        ActualizarTimeScale();
    }

    public void SetGameOver(bool activo)
    {
        pausadoPorGameOver = activo;
        ActualizarTimeScale();
    }

    public void SetFinNivel(bool activo)
    {
        pausadoPorFinNivel = activo;
        ActualizarTimeScale();
    }

    public void ResetearTodo()
    {
        pausadoPorLevelUp = false;
        pausadoPorPausa = false;
        pausadoPorGameOver = false;
        pausadoPorFinNivel = false;
        Time.timeScale = 1f;
    }
}