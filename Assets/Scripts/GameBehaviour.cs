using UnityEngine;
using Mirror;

public class GameBehaviour : NetworkBehaviour
{
    private int playerCount = 0;
    private bool messageDisplayed = false;
    [SerializeField]
    private int maxConnectedPlayerCount = 2;

    // Méthode appelée lorsque qu'un joueur se connecte au serveur
    public void OnPlayerConnected()
    {
        playerCount++;
        CheckPlayerCount();
    }

    // Méthode appelée lorsque qu'un joueur se déconnecte du serveur
    public void OnPlayerDisconnected()
    {
        playerCount--;
        CheckPlayerCount();
    }

    // Vérifie si le nombre de joueurs est égal à 10 et affiche le message si c'est le cas
    private void CheckPlayerCount()
    {
        if (playerCount >= this.maxConnectedPlayerCount && !messageDisplayed)
        {
            Debug.Log("Les joueurs sont connectés !");
            messageDisplayed = true; // Pour éviter d'afficher le message à plusieurs reprises
        }
    }
}
