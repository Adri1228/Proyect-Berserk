using UnityEngine;
using UnityEngine.UI;
public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private PlayerMovement playerMovement;
    private float vidaMaxima;
    void Start()
    {
        playerMovement = GameObject.Find("Jugador").GetComponent<PlayerMovement>();
        vidaMaxima = playerMovement.vida;
    }
    void Update()
    {
        rellenoBarraVida.fillAmount = playerMovement.vida / vidaMaxima;
    }
}
