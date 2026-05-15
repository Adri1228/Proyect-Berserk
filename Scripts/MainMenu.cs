using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenu;

    [Header("Botón Continue")]
    public Button botonContinue;

    [Header("Audio")]
    public AudioMixer audioMixer; // Arrastra GameAudioMixer aquí

    [Header("Contenedor del slider (un GameObject vacío dentro de optionsMenu)")]
    public Transform contenedorOpciones; // Arrastra aquí un GameObject vacío dentro del optionsMenu

    private Slider sliderMusica;

    void Start()
    {
        if (botonContinue != null)
            botonContinue.interactable = PlayerStats.ExistePartidaGuardada();

        // Crear slider por código dentro del panel de opciones
        if (contenedorOpciones != null)
            CrearSliderMusica();

        // Aplicar volumen guardado al mixer inmediatamente
        float volGuardado = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        AplicarVolumenMusica(volGuardado);
    }

    void CrearSliderMusica()
    {
        // Etiqueta
        GameObject lblObj = new GameObject("LabelMusica");
        lblObj.transform.SetParent(contenedorOpciones, false);
        RectTransform rectLbl = lblObj.AddComponent<RectTransform>();
        rectLbl.anchorMin = new Vector2(0.5f, 1);
        rectLbl.anchorMax = new Vector2(0.5f, 1);
        rectLbl.pivot = new Vector2(0.5f, 1);
        rectLbl.sizeDelta = new Vector2(400, 45);
        rectLbl.anchoredPosition = new Vector2(0, -10);

        TextMeshProUGUI lbl = lblObj.AddComponent<TextMeshProUGUI>();
        lbl.text = "Music Volume";
        lbl.fontSize = 26;
        lbl.alignment = TextAlignmentOptions.Center;
        lbl.color = new Color(0.85f, 0.85f, 0.85f);

        // Slider
        GameObject slObj = new GameObject("SliderMusica");
        slObj.transform.SetParent(contenedorOpciones, false);
        RectTransform rectSl = slObj.AddComponent<RectTransform>();
        rectSl.anchorMin = new Vector2(0.5f, 1);
        rectSl.anchorMax = new Vector2(0.5f, 1);
        rectSl.pivot = new Vector2(0.5f, 1);
        rectSl.sizeDelta = new Vector2(400, 40); // Más grande que el de pausa
        rectSl.anchoredPosition = new Vector2(0, -65);

        sliderMusica = slObj.AddComponent<Slider>();
        sliderMusica.minValue = 0f;
        sliderMusica.maxValue = 1f;
        sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 1f);

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(slObj.transform, false);
        RectTransform rectBg = bg.AddComponent<RectTransform>();
        rectBg.anchorMin = new Vector2(0, 0.25f);
        rectBg.anchorMax = new Vector2(1, 0.75f);
        rectBg.offsetMin = Vector2.zero;
        rectBg.offsetMax = Vector2.zero;
        Image imgBg = bg.AddComponent<Image>();
        imgBg.color = new Color(0.2f, 0.2f, 0.2f);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(slObj.transform, false);
        RectTransform rectFA = fillArea.AddComponent<RectTransform>();
        rectFA.anchorMin = new Vector2(0, 0.25f);
        rectFA.anchorMax = new Vector2(1, 0.75f);
        rectFA.offsetMin = new Vector2(5, 0);
        rectFA.offsetMax = new Vector2(-5, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform rectFill = fill.AddComponent<RectTransform>();
        rectFill.anchorMin = Vector2.zero;
        rectFill.anchorMax = new Vector2(0.5f, 1);
        rectFill.offsetMin = Vector2.zero;
        rectFill.offsetMax = Vector2.zero;
        Image imgFill = fill.AddComponent<Image>();
        imgFill.color = new Color(0.2f, 0.6f, 1f);

        // Handle Slide Area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(slObj.transform, false);
        RectTransform rectHA = handleArea.AddComponent<RectTransform>();
        rectHA.anchorMin = Vector2.zero;
        rectHA.anchorMax = Vector2.one;
        rectHA.offsetMin = new Vector2(10, 0);
        rectHA.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform rectH = handle.AddComponent<RectTransform>();
        rectH.sizeDelta = new Vector2(30, 30); // Handle más grande
        Image imgHandle = handle.AddComponent<Image>();
        imgHandle.color = Color.white;

        sliderMusica.fillRect = rectFill;
        sliderMusica.handleRect = rectH;
        sliderMusica.targetGraphic = imgHandle;

        // Conectar evento
        sliderMusica.onValueChanged.AddListener((v) =>
        {
            PlayerPrefs.SetFloat("VolumenMusica", v);
            AplicarVolumenMusica(v);
        });
    }

    void AplicarVolumenMusica(float valor)
    {
        if (audioMixer == null) return;
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20f : -80f;
        audioMixer.SetFloat("VolMusica", db);
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado");
    }

    public void PlayGame()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.BorrarPartidaGuardada();
        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        if (!PlayerStats.ExistePartidaGuardada())
        {
            Debug.Log("No hay partida guardada");
            return;
        }

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.CargarDesdeDisco();

        int escena = PlayerStats.Instance != null
            ? PlayerStats.Instance.escenaGuardada
            : 1;

        SceneManager.LoadScene(escena);
    }
}