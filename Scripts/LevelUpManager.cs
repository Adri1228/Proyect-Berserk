using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI (asigna el Canvas principal)")]
    public Canvas canvas;

    private PlayerMovement player;
    private GameObject panelLevelUp;
    private bool esperandoEleccion = false;
    private TextMeshProUGUI txtNivel; // Texto del nivel en pantalla

    public class Mejora
    {
        public string titulo;
        public string descripcion;
        public System.Action<PlayerMovement> aplicar;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();

        CrearPanelUI();
        CrearUIDeNivel(); // Crear el indicador de nivel
        panelLevelUp.SetActive(false);
    }

    void Update()
{
    // Ya no tocamos Time.timeScale aquí, lo gestiona PauseManager
    if (txtNivel != null && player != null)
        txtNivel.text = player.nivel.ToString();
}

    public void MostrarMejoras()
{
    List<Mejora> pool = GenerarPoolMejoras();
    for (int i = pool.Count - 1; i > 0; i--)
    {
        int j = Random.Range(0, i + 1);
        (pool[i], pool[j]) = (pool[j], pool[i]);
    }
    ActualizarBotones(pool[0], pool[1]);
    panelLevelUp.SetActive(true);

    if (PauseManager.Instance != null)
        PauseManager.Instance.SetLevelUp(true);
}

    void ElegirMejora(Mejora mejora)
{
    mejora.aplicar(player);
    player.vida = Mathf.Min(player.vida, player.maxVida);
    player.vida = Mathf.Max(player.vida, 1);
    panelLevelUp.SetActive(false);

    if (PauseManager.Instance != null)
        PauseManager.Instance.SetLevelUp(false);
}

    List<Mejora> GenerarPoolMejoras()
    {
        return new List<Mejora>
        {
            new Mejora {
                titulo = "Fuerza Bruta",
                descripcion = "+2 Daño\n-1 Vida máxima",
                aplicar = (p) => { p.attackDamage += 2; p.maxVida -= 1; }
            },
            new Mejora {
                titulo = "Espíritu Resistente",
                descripcion = "+2 Vida máxima\n-0.5 Velocidad",
                aplicar = (p) => { p.maxVida += 2; p.vida += 2; p.runSpeed -= 0.5f; }
            },
            new Mejora {
                titulo = "Paso Veloz",
                descripcion = "+1.5 Velocidad\n-1 Vida máxima",
                aplicar = (p) => { p.runSpeed += 1.5f; p.maxVida -= 1; }
            },
            new Mejora {
                titulo = "Alcance Largo",
                descripcion = "+15 Rango de ataque\n-1 Daño",
                aplicar = (p) => { p.attackRadius += 15f; p.attackDamage = Mathf.Max(1, p.attackDamage - 1); }
            },
            new Mejora {
                titulo = "Golpe Preciso",
                descripcion = "+1 Daño\n-10 Rango de ataque",
                aplicar = (p) => { p.attackDamage += 1; p.attackRadius = Mathf.Max(5f, p.attackRadius - 10f); }
            },
            new Mejora {
                titulo = "Frenesí",
                descripcion = "-0.4s Cooldown ataque\n-1 Vida máxima",
                aplicar = (p) => { p.attackCooldown = Mathf.Max(0.3f, p.attackCooldown - 0.4f); p.maxVida -= 1; }
            },
            new Mejora {
                titulo = "Cuerpo Templado",
                descripcion = "+3 Vida máxima\n-1 Daño",
                aplicar = (p) => { p.maxVida += 3; p.vida += 3; p.attackDamage = Mathf.Max(1, p.attackDamage - 1); }
            },
            new Mejora {
                titulo = "Paso Sigilo",
                descripcion = "+2 Velocidad\n+0.5s Cooldown ataque",
                aplicar = (p) => { p.runSpeed += 2f; p.attackCooldown += 0.5f; }
            },
        };
    }

    // INDICADOR DE NIVEL (esquina superior izquierda)
    void CrearUIDeNivel()
{
    GameObject nivelObj = CrearRectObj("TextoNivel", canvas.transform);
    RectTransform rect = nivelObj.GetComponent<RectTransform>();

    rect.anchorMin = new Vector2(0, 1);
    rect.anchorMax = new Vector2(0, 1);
    rect.pivot = new Vector2(0, 1);
    rect.sizeDelta = new Vector2(200, 60);
    rect.anchoredPosition = new Vector2(20, -50);

    txtNivel = nivelObj.AddComponent<TextMeshProUGUI>();
    txtNivel.text = "1";                                    
    txtNivel.fontSize = 72;
    txtNivel.fontStyle = FontStyles.Bold;
    txtNivel.alignment = TextAlignmentOptions.Left;
    txtNivel.color = new Color(0.2f, 0.6f, 1f);           

    var sombra = nivelObj.AddComponent<Shadow>();
    sombra.effectColor = new Color(0, 0, 0, 0.8f);
    sombra.effectDistance = new Vector2(2, -2);
}

    // PANEL DE MEJORAS
    private Button botonA, botonB;
    private TextMeshProUGUI tituloA, descA, tituloB, descB;
    private Mejora mejoraGuardadaA, mejoraGuardadaB;

    void CrearPanelUI()
    {
        panelLevelUp = new GameObject("PanelLevelUp");
        panelLevelUp.transform.SetParent(canvas.transform, false);

        RectTransform rectPanel = panelLevelUp.AddComponent<RectTransform>();
        rectPanel.anchorMin = Vector2.zero;
        rectPanel.anchorMax = Vector2.one;
        rectPanel.offsetMin = Vector2.zero;
        rectPanel.offsetMax = Vector2.zero;

        Image fondoOscuro = panelLevelUp.AddComponent<Image>();
        fondoOscuro.color = new Color(0, 0, 0, 0.75f);

        // Contenedor central — más alto para acomodar texto grande
        GameObject contenedor = CrearRectObj("Contenedor", panelLevelUp.transform);
        RectTransform rectCont = contenedor.GetComponent<RectTransform>();
        rectCont.anchorMin = new Vector2(0.5f, 0.5f);
        rectCont.anchorMax = new Vector2(0.5f, 0.5f);
        rectCont.pivot = new Vector2(0.5f, 0.5f);
        rectCont.sizeDelta = new Vector2(750, 420);
        rectCont.anchoredPosition = Vector2.zero;

        // Título
        GameObject tituloObj = CrearRectObj("Titulo", contenedor.transform);
        RectTransform rectTit = tituloObj.GetComponent<RectTransform>();
        rectTit.anchorMin = new Vector2(0, 1);
        rectTit.anchorMax = new Vector2(1, 1);
        rectTit.pivot = new Vector2(0.5f, 1);
        rectTit.sizeDelta = new Vector2(0, 80);
        rectTit.anchoredPosition = new Vector2(0, 0);

        TextMeshProUGUI txtTitulo = tituloObj.AddComponent<TextMeshProUGUI>();
        txtTitulo.text = "¡ LEVEL UP !";
        txtTitulo.fontSize = 64;
        txtTitulo.fontStyle = FontStyles.Bold;
        txtTitulo.alignment = TextAlignmentOptions.Center;
        txtTitulo.color = new Color(1f, 0.85f, 0.2f);

        // Subtítulo
        GameObject subTit = CrearRectObj("Subtitulo", contenedor.transform);
        RectTransform rectSub = subTit.GetComponent<RectTransform>();
        rectSub.anchorMin = new Vector2(0, 1);
        rectSub.anchorMax = new Vector2(1, 1);
        rectSub.pivot = new Vector2(0.5f, 1);
        rectSub.sizeDelta = new Vector2(0, 45);
        rectSub.anchoredPosition = new Vector2(0, -80);

        TextMeshProUGUI txtSub = subTit.AddComponent<TextMeshProUGUI>();
        txtSub.text = "Chose one upgrade:";
        txtSub.fontSize = 28;
        txtSub.alignment = TextAlignmentOptions.Center;
        txtSub.color = new Color(0.85f, 0.85f, 0.85f);

        // Botones
        (botonA, tituloA, descA) = CrearBotonMejora("BotonA", contenedor.transform, new Vector2(-190, -130));
        (botonB, tituloB, descB) = CrearBotonMejora("BotonB", contenedor.transform, new Vector2(190, -130));

        botonA.onClick.AddListener(() => ElegirMejora(mejoraGuardadaA));
        botonB.onClick.AddListener(() => ElegirMejora(mejoraGuardadaB));
    }

    void ActualizarBotones(Mejora a, Mejora b)
    {
        mejoraGuardadaA = a;
        mejoraGuardadaB = b;
        tituloA.text = a.titulo;
        descA.text = a.descripcion;
        tituloB.text = b.titulo;
        descB.text = b.descripcion;
    }

    (Button, TextMeshProUGUI, TextMeshProUGUI) CrearBotonMejora(string nombre,
        Transform padre, Vector2 posicion)
    {
        GameObject btnObj = CrearRectObj(nombre, padre);
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(330, 240); // Botones más grandes
        rect.anchoredPosition = posicion;

        Image fondo = btnObj.AddComponent<Image>();
        fondo.color = new Color(0.15f, 0.15f, 0.25f, 0.95f);

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(0.15f, 0.15f, 0.25f);
        cb.highlightedColor = new Color(0.25f, 0.25f, 0.45f);
        cb.pressedColor = new Color(1f, 0.75f, 0.1f);
        btn.colors = cb;

        // Título de la mejora — más grande
        GameObject titObj = CrearRectObj("Titulo", btnObj.transform);
        RectTransform rectTit = titObj.GetComponent<RectTransform>();
        rectTit.anchorMin = new Vector2(0, 1);
        rectTit.anchorMax = new Vector2(1, 1);
        rectTit.pivot = new Vector2(0.5f, 1);
        rectTit.sizeDelta = new Vector2(-16, 70);
        rectTit.anchoredPosition = new Vector2(0, -10);

        TextMeshProUGUI titulo = titObj.AddComponent<TextMeshProUGUI>();
        titulo.fontSize = 30; // Antes: 20
        titulo.fontStyle = FontStyles.Bold;
        titulo.alignment = TextAlignmentOptions.Center;
        titulo.color = new Color(1f, 0.85f, 0.2f);

        // Descripción — más grande
        GameObject descObj = CrearRectObj("Desc", btnObj.transform);
        RectTransform rectDesc = descObj.GetComponent<RectTransform>();
        rectDesc.anchorMin = new Vector2(0, 0);
        rectDesc.anchorMax = new Vector2(1, 1);
        rectDesc.offsetMin = new Vector2(12, 12);
        rectDesc.offsetMax = new Vector2(-12, -80);

        TextMeshProUGUI desc = descObj.AddComponent<TextMeshProUGUI>();
        desc.fontSize = 24; // Antes: 16
        desc.alignment = TextAlignmentOptions.Center;
        desc.color = new Color(0.9f, 0.9f, 0.9f);

        return (btn, titulo, desc);
    }

    GameObject CrearRectObj(string nombre, Transform padre)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        go.AddComponent<RectTransform>();
        return go;
    }
}