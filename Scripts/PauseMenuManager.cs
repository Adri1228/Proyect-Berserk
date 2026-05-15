using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;  // <- añadir
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    private Canvas canvas;
    private GameObject panelPausa;
    private GameObject panelOpciones;
    private bool pausaActiva = false;

    private Slider sliderMusica;
    private Slider sliderEfectos;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        CrearPanelPausa();
        CrearPanelOpciones();
        panelPausa.SetActive(false);
        panelOpciones.SetActive(false);

        // Cargar valores guardados y aplicarlos
        float volMusica  = PlayerPrefs.GetFloat("VolumenMusica",  1f);
        float volEfectos = PlayerPrefs.GetFloat("VolumenEfectos", 1f);

        if (sliderMusica  != null) sliderMusica.value  = volMusica;
        if (sliderEfectos != null) sliderEfectos.value = volEfectos;

        AplicarVolumenMusica(volMusica);
        AplicarVolumenEfectos(volEfectos);
    }

    void AplicarVolumenMusica(float valor)
    {
        if (audioMixer == null) return;
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
        audioMixer.SetFloat("VolMusica", db);
    }

    void AplicarVolumenEfectos(float valor)
    {
        if (audioMixer == null) return;
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
        audioMixer.SetFloat("VolEfectos", db);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelOpciones.activeSelf)
            {
                panelOpciones.SetActive(false);
                panelPausa.SetActive(true);
                return;
            }
            TogglePausa();
        }
    }

    void TogglePausa()
    {
        pausaActiva = !pausaActiva;
        panelPausa.SetActive(pausaActiva);

        if (PauseManager.Instance != null)
            PauseManager.Instance.SetPausa(pausaActiva);
    }

    void Continuar()
    {
        pausaActiva = false;
        panelPausa.SetActive(false);
        panelOpciones.SetActive(false);
        if (PauseManager.Instance != null)
            PauseManager.Instance.SetPausa(false);
    }

    void Retry()
    {
        Continuar();
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.Resetear();
        if (PauseManager.Instance != null)
            PauseManager.Instance.ResetearTodo();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void AbrirOpciones()
    {
        panelPausa.SetActive(false);
        panelOpciones.SetActive(true);
    }

    void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        panelPausa.SetActive(true);
    }

    void BackToTitle()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.Resetear();
        if (PauseManager.Instance != null)
            PauseManager.Instance.ResetearTodo();
        SceneManager.LoadScene(0);
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado");
    }

    // ─────────────────────────────────────────────
    // CREAR PANEL DE PAUSA
    // ─────────────────────────────────────────────
    void CrearPanelPausa()
    {
        panelPausa = new GameObject("PanelPausa");
        panelPausa.transform.SetParent(canvas.transform, false);

        RectTransform rectPanel = panelPausa.AddComponent<RectTransform>();
        rectPanel.anchorMin = Vector2.zero;
        rectPanel.anchorMax = Vector2.one;
        rectPanel.offsetMin = Vector2.zero;
        rectPanel.offsetMax = Vector2.zero;

        Image fondo = panelPausa.AddComponent<Image>();
        fondo.color = new Color(0f, 0f, 0f, 0.8f);

        // Contenedor central
        GameObject cont = CrearRectObj("Contenedor", panelPausa.transform);
        RectTransform rectCont = cont.GetComponent<RectTransform>();
        rectCont.anchorMin = new Vector2(0.5f, 0.5f);
        rectCont.anchorMax = new Vector2(0.5f, 0.5f);
        rectCont.pivot = new Vector2(0.5f, 0.5f);
        rectCont.sizeDelta = new Vector2(400, 480);
        rectCont.anchoredPosition = Vector2.zero;

        // Título PAUSED
        GameObject titObj = CrearRectObj("Titulo", cont.transform);
        RectTransform rectTit = titObj.GetComponent<RectTransform>();
        rectTit.anchorMin = new Vector2(0, 1);
        rectTit.anchorMax = new Vector2(1, 1);
        rectTit.pivot = new Vector2(0.5f, 1);
        rectTit.sizeDelta = new Vector2(0, 80);
        rectTit.anchoredPosition = Vector2.zero;

        TextMeshProUGUI txtTit = titObj.AddComponent<TextMeshProUGUI>();
        txtTit.text = "PAUSED";
        txtTit.fontSize = 56;
        txtTit.fontStyle = FontStyles.Bold;
        txtTit.alignment = TextAlignmentOptions.Center;
        txtTit.color = Color.white;

        // Botones apilados verticalmente
        float startY = -100f;
        float spacing = 75f;

        CrearBotonPausa("BtnContinue",  cont.transform, new Vector2(0, startY),             "Continue",          new Color(0.1f, 0.45f, 0.1f),  Continuar);
        CrearBotonPausa("BtnRetry",     cont.transform, new Vector2(0, startY - spacing),   "Retry",             new Color(0.15f, 0.3f, 0.55f), Retry);
        CrearBotonPausa("BtnOptions",   cont.transform, new Vector2(0, startY - spacing*2), "Options",           new Color(0.35f, 0.25f, 0.05f),AbrirOpciones);
        CrearBotonPausa("BtnTitle",     cont.transform, new Vector2(0, startY - spacing*3), "Back to Title",     new Color(0.35f, 0.1f, 0.1f),  BackToTitle);
        CrearBotonPausa("BtnQuit",      cont.transform, new Vector2(0, startY - spacing*4), "Quit Game",         new Color(0.2f, 0.2f, 0.2f),   QuitGame);
    }

    // ─────────────────────────────────────────────
    // CREAR PANEL DE OPCIONES
    // ─────────────────────────────────────────────
    void CrearPanelOpciones()
    {
        panelOpciones = new GameObject("PanelOpciones");
        panelOpciones.transform.SetParent(canvas.transform, false);

        RectTransform rectPanel = panelOpciones.AddComponent<RectTransform>();
        rectPanel.anchorMin = Vector2.zero;
        rectPanel.anchorMax = Vector2.one;
        rectPanel.offsetMin = Vector2.zero;
        rectPanel.offsetMax = Vector2.zero;

        Image fondo = panelOpciones.AddComponent<Image>();
        fondo.color = new Color(0f, 0f, 0f, 0.85f);

        // Contenedor
        GameObject cont = CrearRectObj("Contenedor", panelOpciones.transform);
        RectTransform rectCont = cont.GetComponent<RectTransform>();
        rectCont.anchorMin = new Vector2(0.5f, 0.5f);
        rectCont.anchorMax = new Vector2(0.5f, 0.5f);
        rectCont.pivot = new Vector2(0.5f, 0.5f);
        rectCont.sizeDelta = new Vector2(480, 420);
        rectCont.anchoredPosition = Vector2.zero;

        // Título OPTIONS
        CrearTexto("TitOpc", cont.transform, new Vector2(0, 0), new Vector2(0, 70),
            "OPTIONS", 44, FontStyles.Bold, Color.white, TextAlignmentOptions.Center, new Vector2(0.5f,1));

        // Slider Música
        sliderMusica = CrearSlider("SliderMusica", cont.transform, new Vector2(0, -135),
    (v) => {
        PlayerPrefs.SetFloat("VolumenMusica", v);
        AplicarVolumenMusica(v);
    });

    sliderEfectos = CrearSlider("SliderEfectos", cont.transform, new Vector2(0, -230),
    (v) => {
        PlayerPrefs.SetFloat("VolumenEfectos", v);
        AplicarVolumenEfectos(v);
    });

        // Toggle Pantalla Completa
        CrearToggleFullscreen("ToggleFS", cont.transform, new Vector2(0, -285));

        // Botón Back
        CrearBotonPausa("BtnBack", cont.transform, new Vector2(0, -350), "Back",
            new Color(0.3f, 0.3f, 0.3f), CerrarOpciones);
    }

    // ─────────────────────────────────────────────
    // HELPERS DE UI
    // ─────────────────────────────────────────────
    void CrearBotonPausa(string nombre, Transform padre, Vector2 pos,
        string texto, Color color, UnityEngine.Events.UnityAction accion)
    {
        GameObject btnObj = CrearRectObj(nombre, padre);
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(320, 58);
        rect.anchoredPosition = pos;

        Image img = btnObj.AddComponent<Image>();
        img.color = color;

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = color;
        cb.highlightedColor = color * 1.4f;
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
        tmp.fontSize = 22;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }

    Slider CrearSlider(string nombre, Transform padre, Vector2 pos,
        UnityEngine.Events.UnityAction<float> onChange)
    {
        GameObject slObj = CrearRectObj(nombre, padre);
        RectTransform rect = slObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(340, 30);
        rect.anchoredPosition = pos;

        Slider slider = slObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Background
        GameObject bg = CrearRectObj("Background", slObj.transform);
        RectTransform rectBg = bg.GetComponent<RectTransform>();
        rectBg.anchorMin = new Vector2(0, 0.25f);
        rectBg.anchorMax = new Vector2(1, 0.75f);
        rectBg.offsetMin = Vector2.zero;
        rectBg.offsetMax = Vector2.zero;
        Image imgBg = bg.AddComponent<Image>();
        imgBg.color = new Color(0.2f, 0.2f, 0.2f);

        // Fill Area
        GameObject fillArea = CrearRectObj("Fill Area", slObj.transform);
        RectTransform rectFA = fillArea.GetComponent<RectTransform>();
        rectFA.anchorMin = new Vector2(0, 0.25f);
        rectFA.anchorMax = new Vector2(1, 0.75f);
        rectFA.offsetMin = new Vector2(5, 0);
        rectFA.offsetMax = new Vector2(-5, 0);

        GameObject fill = CrearRectObj("Fill", fillArea.transform);
        RectTransform rectFill = fill.GetComponent<RectTransform>();
        rectFill.anchorMin = Vector2.zero;
        rectFill.anchorMax = new Vector2(0.5f, 1);
        rectFill.offsetMin = Vector2.zero;
        rectFill.offsetMax = Vector2.zero;
        Image imgFill = fill.AddComponent<Image>();
        imgFill.color = new Color(0.2f, 0.6f, 1f);

        // Handle
        GameObject handleArea = CrearRectObj("Handle Slide Area", slObj.transform);
        RectTransform rectHA = handleArea.GetComponent<RectTransform>();
        rectHA.anchorMin = Vector2.zero;
        rectHA.anchorMax = Vector2.one;
        rectHA.offsetMin = new Vector2(10, 0);
        rectHA.offsetMax = new Vector2(-10, 0);

        GameObject handle = CrearRectObj("Handle", handleArea.transform);
        RectTransform rectH = handle.GetComponent<RectTransform>();
        rectH.sizeDelta = new Vector2(20, 20);
        Image imgHandle = handle.AddComponent<Image>();
        imgHandle.color = Color.white;

        slider.fillRect = rectFill;
        slider.handleRect = rectH;
        slider.targetGraphic = imgHandle;
        slider.onValueChanged.AddListener(onChange);

        return slider;
    }

    void CrearToggleFullscreen(string nombre, Transform padre, Vector2 pos)
    {
        GameObject row = CrearRectObj(nombre, padre);
        RectTransform rectRow = row.GetComponent<RectTransform>();
        rectRow.anchorMin = new Vector2(0.5f, 1);
        rectRow.anchorMax = new Vector2(0.5f, 1);
        rectRow.pivot = new Vector2(0.5f, 1);
        rectRow.sizeDelta = new Vector2(340, 40);
        rectRow.anchoredPosition = pos;

        // Label
        GameObject lblObj = CrearRectObj("Label", row.transform);
        RectTransform rectLbl = lblObj.GetComponent<RectTransform>();
        rectLbl.anchorMin = new Vector2(0, 0);
        rectLbl.anchorMax = new Vector2(0.7f, 1);
        rectLbl.offsetMin = Vector2.zero;
        rectLbl.offsetMax = Vector2.zero;
        TextMeshProUGUI lbl = lblObj.AddComponent<TextMeshProUGUI>();
        lbl.text = "Fullscreen";
        lbl.fontSize = 22;
        lbl.alignment = TextAlignmentOptions.Left;
        lbl.color = new Color(0.85f, 0.85f, 0.85f);

        // Toggle box
        GameObject tgObj = CrearRectObj("Toggle", row.transform);
        RectTransform rectTg = tgObj.GetComponent<RectTransform>();
        rectTg.anchorMin = new Vector2(0.75f, 0.1f);
        rectTg.anchorMax = new Vector2(1f, 0.9f);
        rectTg.offsetMin = Vector2.zero;
        rectTg.offsetMax = Vector2.zero;

        Image imgTg = tgObj.AddComponent<Image>();
        imgTg.color = new Color(0.25f, 0.25f, 0.25f);

        Toggle toggle = tgObj.AddComponent<Toggle>();
        toggle.isOn = Screen.fullScreen;

        // Checkmark
        GameObject ckObj = CrearRectObj("Checkmark", tgObj.transform);
        RectTransform rectCk = ckObj.GetComponent<RectTransform>();
        rectCk.anchorMin = new Vector2(0.1f, 0.1f);
        rectCk.anchorMax = new Vector2(0.9f, 0.9f);
        rectCk.offsetMin = Vector2.zero;
        rectCk.offsetMax = Vector2.zero;
        Image imgCk = ckObj.AddComponent<Image>();
        imgCk.color = new Color(0.2f, 0.6f, 1f);

        toggle.graphic = imgCk;
        toggle.targetGraphic = imgTg;
        toggle.onValueChanged.AddListener((v) =>
        {
            Screen.fullScreen = v;
            PlayerPrefs.SetInt("Fullscreen", v ? 1 : 0);
        });
    }

    void CrearTexto(string nombre, Transform padre, Vector2 pos, Vector2 size,
        string texto, float fontSize, FontStyles style, Color color,
        TextAlignmentOptions align, Vector2 pivot)
    {
        GameObject obj = CrearRectObj(nombre, padre);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = pivot;
        rect.sizeDelta = new Vector2(size.x == 0 ? 400 : size.x, size.y);
        rect.anchoredPosition = pos;

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = align;
        tmp.color = color;
    }

    GameObject CrearRectObj(string nombre, Transform padre)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        go.AddComponent<RectTransform>();
        return go;
    }
}