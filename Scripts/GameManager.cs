using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool gameOverActivo = false;
    private GameObject panelGameOver;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PauseManager.Instance == null)
            new GameObject("PauseManager").AddComponent<PauseManager>();

        CrearPanelGameOver();
        panelGameOver.SetActive(false);
    }

    void Update()
    {
        if (!gameOverActivo) return;
        if (Input.GetKeyDown(KeyCode.R)) Retry();
        if (Input.GetKeyDown(KeyCode.Escape)) ReturnToMenu();
    }

    public void GameOver()
{
    if (gameOverActivo) return;
    gameOverActivo = true;

    // Guardar en disco con la escena ACTUAL para reanudar aquí
    PlayerMovement pm = FindObjectOfType<PlayerMovement>();
    if (pm != null && PlayerStats.Instance != null)
    {
        PlayerStats.Instance.GuardarDesdeJugador(pm);

        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        PlayerStats.Instance.GuardarEnDisco(escenaActual);
    }

    if (PauseManager.Instance != null)
        PauseManager.Instance.SetGameOver(true);

    panelGameOver.SetActive(true);
}

    void Retry()
    {
        gameOverActivo = false;
        if (PauseManager.Instance != null) PauseManager.Instance.ResetearTodo();
        if (PlayerStats.Instance != null) PlayerStats.Instance.Resetear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ReturnToMenu()
    {
        gameOverActivo = false;
        if (PauseManager.Instance != null) PauseManager.Instance.ResetearTodo();
        if (PlayerStats.Instance != null) PlayerStats.Instance.Resetear();
        SceneManager.LoadScene(0);
    }

    void CrearPanelGameOver()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Fondo oscuro a pantalla completa
        panelGameOver = new GameObject("PanelGameOver");
        panelGameOver.transform.SetParent(canvas.transform, false);

        RectTransform rectPanel = panelGameOver.AddComponent<RectTransform>();
        rectPanel.anchorMin = Vector2.zero;
        rectPanel.anchorMax = Vector2.one;
        rectPanel.offsetMin = Vector2.zero;
        rectPanel.offsetMax = Vector2.zero;

        Image fondo = panelGameOver.AddComponent<Image>();
        fondo.color = new Color(0f, 0f, 0f, 0.82f);

        // Contenedor central — mismo tamaño que el panel de nivel completado
        GameObject contenedor = CrearRectObj("Contenedor", panelGameOver.transform);
        RectTransform rectCont = contenedor.GetComponent<RectTransform>();
        rectCont.anchorMin = new Vector2(0.5f, 0.5f);
        rectCont.anchorMax = new Vector2(0.5f, 0.5f);
        rectCont.pivot = new Vector2(0.5f, 0.5f);
        rectCont.sizeDelta = new Vector2(600, 350);
        rectCont.anchoredPosition = Vector2.zero;

        // Título GAME OVER
        GameObject titObj = CrearRectObj("Titulo", contenedor.transform);
        RectTransform rectTit = titObj.GetComponent<RectTransform>();
        rectTit.anchorMin = new Vector2(0, 1);
        rectTit.anchorMax = new Vector2(1, 1);
        rectTit.pivot = new Vector2(0.5f, 1);
        rectTit.sizeDelta = new Vector2(0, 90);
        rectTit.anchoredPosition = Vector2.zero;

        TextMeshProUGUI txtTitulo = titObj.AddComponent<TextMeshProUGUI>();
        txtTitulo.text = "GAME OVER";
        txtTitulo.fontSize = 68;
        txtTitulo.fontStyle = FontStyles.Bold;
        txtTitulo.alignment = TextAlignmentOptions.Center;
        txtTitulo.color = new Color(0.9f, 0.15f, 0.15f);

        // Subtítulo
        GameObject subObj = CrearRectObj("Sub", contenedor.transform);
        RectTransform rectSub = subObj.GetComponent<RectTransform>();
        rectSub.anchorMin = new Vector2(0, 1);
        rectSub.anchorMax = new Vector2(1, 1);
        rectSub.pivot = new Vector2(0.5f, 1);
        rectSub.sizeDelta = new Vector2(0, 50);
        rectSub.anchoredPosition = new Vector2(0, -90);

        TextMeshProUGUI txtSub = subObj.AddComponent<TextMeshProUGUI>();
        txtSub.text = "What do you want to do?";
        txtSub.fontSize = 26;
        txtSub.alignment = TextAlignmentOptions.Center;
        txtSub.color = new Color(0.85f, 0.85f, 0.85f);

        // Botón Retry — verde (igual estilo que "Next Level")
        CrearBoton("BtnRetry", contenedor.transform,
            new Vector2(-155, -170), "Retry",
            new Color(0.1f, 0.5f, 0.2f), Retry);

        // Botón Return to Menu — rojo (igual estilo que "Main Menu")
        CrearBoton("BtnMenu", contenedor.transform,
            new Vector2(155, -170), "Return to Menu",
            new Color(0.5f, 0.1f, 0.1f), ReturnToMenu);
    }

    void CrearBoton(string nombre, Transform padre, Vector2 posicion,
        string texto, Color color, UnityEngine.Events.UnityAction accion)
    {
        GameObject btnObj = CrearRectObj(nombre, padre);
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(260, 80);
        rect.anchoredPosition = posicion;

        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = color * 1.3f;
        cb.pressedColor = Color.white;
        btn.colors = cb;
        btn.onClick.AddListener(accion);

        GameObject txtObj = CrearRectObj("Texto", btnObj.transform);
        RectTransform rectTxt = txtObj.GetComponent<RectTransform>();
        rectTxt.anchorMin = Vector2.zero;
        rectTxt.anchorMax = Vector2.one;
        rectTxt.offsetMin = Vector2.zero;
        rectTxt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = 24;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }

    GameObject CrearRectObj(string nombre, Transform padre)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        go.AddComponent<RectTransform>();
        return go;
    }
}