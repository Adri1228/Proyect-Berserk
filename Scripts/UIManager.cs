using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WaveUIManager : MonoBehaviour
{
    [Header("Referencias")]
    public Canvas canvas;

    private TextMeshProUGUI txtOleadaActual;
    private TextMeshProUGUI txtOleadaTotal;
    private GameObject panelFinNivel;

    void Start()
    {
        if (canvas == null) canvas = FindObjectOfType<Canvas>();

        CrearUIDeOleada();
        CrearPanelFinNivel();
        panelFinNivel.SetActive(false);
    }
    void CrearUIDeOleada()
{
    GameObject obj = CrearRectObj("TextoOleada", canvas.transform);
    RectTransform rect = obj.GetComponent<RectTransform>();
    rect.anchorMin = new Vector2(1, 1);
    rect.anchorMax = new Vector2(1, 1);
    rect.pivot = new Vector2(1, 1);
    rect.sizeDelta = new Vector2(280, 60);
    rect.anchoredPosition = new Vector2(-40, -50);

    txtOleadaActual = obj.AddComponent<TextMeshProUGUI>();
    txtOleadaActual.text = "Wave - | -";
    txtOleadaActual.fontSize = 50;
    txtOleadaActual.fontStyle = FontStyles.Bold;
    txtOleadaActual.alignment = TextAlignmentOptions.Right;
    // Usar rich text para colorear cada parte distinto
    txtOleadaActual.richText = true;
    txtOleadaActual.color = Color.white;

    obj.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.8f);
}

    // PANEL FIN DE NIVEL
    void CrearPanelFinNivel()
    {
        panelFinNivel = new GameObject("PanelFinNivel");
        panelFinNivel.transform.SetParent(canvas.transform, false);

        RectTransform rectPanel = panelFinNivel.AddComponent<RectTransform>();
        rectPanel.anchorMin = Vector2.zero;
        rectPanel.anchorMax = Vector2.one;
        rectPanel.offsetMin = Vector2.zero;
        rectPanel.offsetMax = Vector2.zero;

        Image fondo = panelFinNivel.AddComponent<Image>();
        fondo.color = new Color(0, 0, 0, 0.8f);

        GameObject contenedor = CrearRectObj("Contenedor", panelFinNivel.transform);
        RectTransform rectCont = contenedor.GetComponent<RectTransform>();
        rectCont.anchorMin = new Vector2(0.5f, 0.5f);
        rectCont.anchorMax = new Vector2(0.5f, 0.5f);
        rectCont.pivot = new Vector2(0.5f, 0.5f);
        rectCont.sizeDelta = new Vector2(600, 350);
        rectCont.anchoredPosition = Vector2.zero;

        // Título
        GameObject titObj = CrearRectObj("Titulo", contenedor.transform);
        RectTransform rectTit = titObj.GetComponent<RectTransform>();
        rectTit.anchorMin = new Vector2(0, 1);
        rectTit.anchorMax = new Vector2(1, 1);
        rectTit.pivot = new Vector2(0.5f, 1);
        rectTit.sizeDelta = new Vector2(0, 90);
        rectTit.anchoredPosition = new Vector2(0, 0);

        TextMeshProUGUI txtTitulo = titObj.AddComponent<TextMeshProUGUI>();
        txtTitulo.text = "LEVEL COMPLETE!";
        txtTitulo.fontSize = 58;
        txtTitulo.fontStyle = FontStyles.Bold;
        txtTitulo.alignment = TextAlignmentOptions.Center;
        txtTitulo.color = new Color(1f, 0.85f, 0.2f);

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

        CrearBoton("BtnSiguiente", contenedor.transform,
            new Vector2(-155, -170), "Next level",
            new Color(0.1f, 0.5f, 0.2f), SiguienteNivel);

        CrearBoton("BtnMenu", contenedor.transform,
            new Vector2(155, -170), "Main menu",
            new Color(0.5f, 0.1f, 0.1f), VolverAlMenu);
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
    // MÉTODOS PÚBLICOS
    public void ActualizarOleada(int actual, int total)
{
    if (txtOleadaActual != null)
        // Amarillo para el número actual, gris para la barra y el total
        txtOleadaActual.text = $"<color=#FFD633>Wave {actual}</color> <color=#AAAAAA>| {total}</color>";
}

public void MostrarFinDeNivel()
{
    PlayerMovement pm = FindObjectOfType<PlayerMovement>();

    if (pm != null && PlayerStats.Instance != null)
    {
        // Guardar stats en memoria
        PlayerStats.Instance.GuardarDesdeJugador(pm);

        // Guardar en disco con la SIGUIENTE escena como destino
        int escenaActual = UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().buildIndex;
        int siguienteEscena = escenaActual + 1 < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings
            ? escenaActual + 1
            : escenaActual;

        PlayerStats.Instance.GuardarEnDisco(siguienteEscena);
    }

    if (PauseManager.Instance != null)
        PauseManager.Instance.SetFinNivel(true);

    panelFinNivel.SetActive(true);
}

    void SiguienteNivel()
{
    if (PauseManager.Instance != null)
        PauseManager.Instance.ResetearTodo();

    int escenaActual = SceneManager.GetActiveScene().buildIndex;
    int siguienteEscena = escenaActual + 1 < SceneManager.sceneCountInBuildSettings
        ? escenaActual + 1
        : escenaActual;

    SceneManager.LoadScene(siguienteEscena);
}

void VolverAlMenu()
{
    // Guardar antes de salir para que Continue funcione
    PlayerMovement pm = FindObjectOfType<PlayerMovement>();
    if (pm != null && PlayerStats.Instance != null)
    {
        PlayerStats.Instance.GuardarDesdeJugador(pm);

        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        int siguienteEscena = escenaActual + 1 < SceneManager.sceneCountInBuildSettings
            ? escenaActual + 1
            : escenaActual;

        PlayerStats.Instance.GuardarEnDisco(siguienteEscena);
    }

    if (PauseManager.Instance != null)
        PauseManager.Instance.ResetearTodo();

    SceneManager.LoadScene(0);
}

    GameObject CrearRectObj(string nombre, Transform padre)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        go.AddComponent<RectTransform>();
        return go;
    }
}