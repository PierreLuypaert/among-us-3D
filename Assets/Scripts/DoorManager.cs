using UnityEngine;

public class DoorManager : MonoBehaviour
{
    private Animator doorAnimator; // Référence à l'Animator de la porte

    // Méthode appelée au démarrage du jeu
    void Start()
    {
        // Récupérer l'Animator attaché au GameObject
        doorAnimator = GetComponent<Animator>();

        if (doorAnimator == null)
        {
            Debug.LogError("Aucun Animator n'a été trouvé sur ce GameObject.");
        }
    }

    // Méthode pour ouvrir la porte
    public void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("character_nearby", true);
        }
        else
        {
            Debug.LogError("Animator non défini. Assurez-vous d'avoir attaché un Animator à ce GameObject.");
        }
    }

    // Méthode pour fermer la porte
    public void CloseDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("character_nearby", false);
        }
        else
        {
            Debug.LogError("Animator non défini. Assurez-vous d'avoir attaché un Animator à ce GameObject.");
        }
    }

    // Méthode appelée lorsque le joueur entre dans la boîte de collision de la porte
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assurez-vous d'utiliser le tag approprié pour le joueur
        {
            OpenDoor();
        }
    }

    // Méthode appelée lorsque le joueur sort de la boîte de collision de la porte
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Assurez-vous d'utiliser le tag approprié pour le joueur
        {
            CloseDoor();
        }
    }
}
