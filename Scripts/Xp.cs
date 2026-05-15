using UnityEngine;
using UnityEngine.UI;

public class Xp : MonoBehaviour
{
    public Image rellenoBarraXP;
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerMovement == null) return;
        rellenoBarraXP.fillAmount = playerMovement.xpActual / playerMovement.xpParaSiguienteNivel;
    }
}