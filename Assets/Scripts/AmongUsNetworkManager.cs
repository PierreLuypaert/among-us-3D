using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class AmongUsNetworkManager : NetworkManager
{
    private List<PlayerController> players = new List<PlayerController>();
    [SerializeField]
    private int maxConnectedPlayersCount = 2;

    [SerializeField]
    private GameManager gameManager;


    // Ex�cut� lorsque qu'un joueur se connecte au serveur
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (players.Count < maxConnectedPlayersCount && gameManager.getGameState() == GameState.Lobby)
        {
            // Appelez la m�thode de base pour g�rer la cr�ation du joueur
            base.OnServerAddPlayer(conn);

            // Obtenez le PlayerController du joueur nouvellement cr��
            GameObject playerObject = conn.identity.gameObject;
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            playerController.SetConnectionId(conn.connectionId);

            // Ajoutez le joueur � la liste des joueurs
            players.Add(playerController);

            // Code � ex�cuter lorsqu'un joueur se connecte
            Debug.Log($"Joueur connect� : {conn.connectionId}");
        }
        else
        {
            conn.Disconnect(); // D�connectez le joueur s'il y a trop de joueurs
        }

        if (players.Count == maxConnectedPlayersCount)
        {
            this.StartGame();
        }
    }


    // Ex�cut� lorsque qu'un joueur se d�connecte du serveur
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        PlayerController playerToRemove = players.Find(player => player.GetConnectionId() == conn.connectionId);
        if (playerToRemove != null)
        {
            players.Remove(playerToRemove);
            Debug.Log($"Joueur d�connect� : {conn.connectionId}");
        }
    }

    private void StartGame()
    {
        // Appelez la m�thode StartGame du GameManager
        gameManager.StartGame(players);
    }
}
