using UnityEngine;

public class DoorManager : MonoBehaviour
{
    private Animator doorAnimator; // R�f�rence � l'Animator de la porte

    // M�thode appel�e au d�marrage du jeu
    void Start()
    {
        // R�cup�rer l'Animator attach� au GameObject
        doorAnimator = GetComponent<Animator>();

        if (doorAnimator == null)
        {
            Debug.LogError("Aucun Animator n'a �t� trouv� sur ce GameObject.");
        }
    }

    // M�thode pour ouvrir la porte
    public void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("character_nearby", true);
        }
        else
        {
            Debug.LogError("Animator non d�fini. Assurez-vous d'avoir attach� un Animator � ce GameObject.");
        }
    }

    // M�thode pour fermer la porte
    public void CloseDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("character_nearby", false);
        }
        else
        {
            Debug.LogError("Animator non d�fini. Assurez-vous d'avoir attach� un Animator � ce GameObject.");
        }
    }

    // M�thode appel�e lorsque le joueur entre dans la bo�te de collision de la porte
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assurez-vous d'utiliser le tag appropri� pour le joueur
        {
            OpenDoor();
        }
    }

    // M�thode appel�e lorsque le joueur sort de la bo�te de collision de la porte
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Assurez-vous d'utiliser le tag appropri� pour le joueur
        {
            CloseDoor();
        }
    }
}
