using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string SceneName;
    public float fadeDuration = 1.0f;
    public Material fadeMaterial; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start fade effect
            StartCoroutine(FadeOutAndLoad(SceneName));
        }
    }

    // Fonction pour l'effet de fondu
    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        // Créer un écran de fondu
        GameObject fadeScreen = new GameObject("FadeScreen");
        fadeScreen.AddComponent<MeshRenderer>().material = fadeMaterial;
        fadeScreen.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        fadeScreen.transform.LookAt(Camera.main.transform);
        fadeScreen.transform.localScale = new Vector3(10f, 10f, 0.01f);

        // Faire progressivement disparaître l'écran de fondu
        float timer = 0f;
        while (timer < fadeDuration)
        {
            fadeMaterial.color = new Color(0, 0, 0, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Charger la nouvelle scène
        SceneManager.LoadScene(sceneName);

        // Détruire l'écran de fondu
        Destroy(fadeScreen);
    }
}
