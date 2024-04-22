using UnityEngine;

public class GestionPanneauFlottant : MonoBehaviour
{
    // Référence vers le script HandController où l'événement est déclenché
    public HandController handController;

    // Référence vers le panneau flottant que tu veux modifier
    public TextMesh texteMesh;

    void Start()
    {
        // Ecoute l'événement d'objet pris
        handController.objetPrisEvent.AddListener(OnObjetPris);
    }

    // Méthode appelée lorsque l'objet est pris
    void OnObjetPris()
    {
        // Change le texte du panneau flottant
        texteMesh.text = "Bravo ! Tu as pris l'objet !";
    }
}
