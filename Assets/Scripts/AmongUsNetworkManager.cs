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


    // Exécuté lorsque qu'un joueur se connecte au serveur
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (players.Count < maxConnectedPlayersCount && gameManager.getGameState() == GameState.Lobby)
        {
            // Appelez la méthode de base pour gérer la création du joueur
            base.OnServerAddPlayer(conn);

            // Obtenez le PlayerController du joueur nouvellement créé
            GameObject playerObject = conn.identity.gameObject;
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            playerController.SetConnectionId(conn.connectionId);

            // Ajoutez le joueur à la liste des joueurs
            players.Add(playerController);

            // Code à exécuter lorsqu'un joueur se connecte
            Debug.Log($"Joueur connecté : {conn.connectionId}");
        }
        else
        {
            conn.Disconnect(); // Déconnectez le joueur s'il y a trop de joueurs
        }

        if (players.Count == maxConnectedPlayersCount)
        {
            this.StartGame();
        }
    }


    // Exécuté lorsque qu'un joueur se déconnecte du serveur
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        PlayerController playerToRemove = players.Find(player => player.GetConnectionId() == conn.connectionId);
        if (playerToRemove != null)
        {
            players.Remove(playerToRemove);
            Debug.Log($"Joueur déconnecté : {conn.connectionId}");
        }
    }

    private void StartGame()
    {
        // Appelez la méthode StartGame du GameManager
        gameManager.StartGame(players);
    }
}
