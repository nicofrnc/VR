using UnityEngine;

public class CagetteController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Vérifiez si l'objet en collision est un légume
        if (other.CompareTag("vegetable"))
        {
            // Déplacer le légume dans la cagette
            other.transform.SetParent(transform); // Définir le parent du légume sur la cagette
            other.transform.localPosition = Vector3.zero; // Réinitialiser la position locale du légume par rapport à la cagette
            Rigidbody rb = other.GetComponent<Rigidbody>(); // Récupérer le Rigidbody du légume
            if (rb != null)
            {
                rb.isKinematic = true; // Désactiver la physique du Rigidbody pour que le légume reste en place dans la cagette
            }
        }
    }
}
