using UnityEngine;
using Mirror;

public class GameBehaviour : NetworkBehaviour
{
    private int playerCount = 0;
    private bool messageDisplayed = false;
    [SerializeField]
    private int maxConnectedPlayerCount = 2;

    // M�thode appel�e lorsque qu'un joueur se connecte au serveur
    public void OnPlayerConnected()
    {
        playerCount++;
        CheckPlayerCount();
    }

    // M�thode appel�e lorsque qu'un joueur se d�connecte du serveur
    public void OnPlayerDisconnected()
    {
        playerCount--;
        CheckPlayerCount();
    }

    // V�rifie si le nombre de joueurs est �gal � 10 et affiche le message si c'est le cas
    private void CheckPlayerCount()
    {
        if (playerCount >= this.maxConnectedPlayerCount && !messageDisplayed)
        {
            Debug.Log("Les joueurs sont connect�s !");
            messageDisplayed = true; // Pour �viter d'afficher le message � plusieurs reprises
        }
    }
}
