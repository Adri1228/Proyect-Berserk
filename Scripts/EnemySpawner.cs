using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Configuración de oleadas")]
    public int totalOleadas = 3;
    public int enemigasPorOleada = 4;
    public float tiempoEntreOleadas = 5f;
    public float spawnInterval = 1.5f;

    [Header("Área de spawn")]
    public Vector3 areaMin;
    public Vector3 areaMax;

    private int oleadaActual = 0;
    private int enemigosVivos = 0;
    private int enemigosSpawneadosEnOleada = 0;
    private bool esperandoSiguienteOleada = false;
    private bool nivelTerminado = false;

    private WaveUIManager waveUI;

    void Start()
    {
        waveUI = FindObjectOfType<WaveUIManager>();

        // Esperamos un frame para que WaveUIManager termine su Start()
        StartCoroutine(EsperarYComenzar());
    }

    IEnumerator EsperarYComenzar()
    {
        yield return null;
        StartCoroutine(IniciarSiguienteOleada());
    }

    IEnumerator IniciarSiguienteOleada()
    {
        if (nivelTerminado) yield break;

        oleadaActual++;

        if (oleadaActual > totalOleadas)
        {
            nivelTerminado = true;
            waveUI?.MostrarFinDeNivel();
            yield break;
        }

        waveUI?.ActualizarOleada(oleadaActual, totalOleadas);

        yield return new WaitForSeconds(tiempoEntreOleadas);

        // Reiniciar contadores ANTES de spawnear
        enemigosVivos = 0;
        enemigosSpawneadosEnOleada = 0;
        esperandoSiguienteOleada = false;

        while (enemigosSpawneadosEnOleada < enemigasPorOleada)
        {
            SpawnEnemigo();
            enemigosSpawneadosEnOleada++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemigo()
    {
        Vector3 pos = new Vector3(
            Random.Range(areaMin.x, areaMax.x),
            Random.Range(areaMin.y, areaMax.y),
            Random.Range(areaMin.z, areaMax.z)
        );

        Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemigosVivos++;
    }

    public void EnemyDefeated()
    {
        // Nunca bajar de 0 para evitar el bug de contadores negativos
        if (enemigosVivos > 0)
            enemigosVivos--;

        // Solo pasar de oleada si: todos spawneados, todos muertos,
        // no estamos ya esperando, y el nivel no terminó
        if (enemigosVivos == 0
            && enemigosSpawneadosEnOleada >= enemigasPorOleada
            && !esperandoSiguienteOleada
            && !nivelTerminado)
        {
            esperandoSiguienteOleada = true; // Bloquear llamadas duplicadas
            StartCoroutine(IniciarSiguienteOleada());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = (areaMin + areaMax) / 2;
        Vector3 size = areaMax - areaMin;
        Gizmos.DrawWireCube(center, size);
    }

    
}