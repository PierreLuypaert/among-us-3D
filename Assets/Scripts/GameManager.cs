using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Lobby,           // État de l'attente dans le hall d'entrée
    Launching,      // État de la partie en lancement
    Processing,      // État de la partie en cours
    Paused,          // État de la partie en pause
    GameOver         // État de la partie terminée
}


public class GameManager : NetworkBehaviour
{
    [SyncVar]
    private GameState gameState = GameState.Lobby;

    [SerializeField]
    private List<InteractableObject> tasks = new List<InteractableObject>();

    [SerializeField]
    private int impostorsCount = 1;

    [SerializeField]
    private List<GameObject> spawnPoints;

    [SerializeField]
    private List<GameObject> cameraPoints;

    [SerializeField]
    private Camera gameCamera;
    [SyncVar(hook = nameof(OnCameraEnabledChanged))]
    private byte cameraEnabledByte = 0; // 0 represents false, 1 represents true

    [SerializeField]
    private float cameraIntroDuration = 4f; // en secondes


    private List<PlayerController> players = new List<PlayerController>();

    [Server]
    public void StartGame(List<PlayerController> playersList)
    {
        if (this.gameState == GameState.Lobby)
        {
            this.players = playersList;
            this.gameState = GameState.Launching;

            StartCoroutine(DelayedAssignRolesAndTasks());
        }
    }

    [Server]
    private IEnumerator DelayedAssignRolesAndTasks()
    {
        yield return new WaitForSeconds(1f);
        AssignImpostor();
        yield return new WaitForSeconds(1f);
        this.setPlayerFadeIn(true);
        yield return new WaitForSeconds(1f);
        AssignTasks();
        yield return new WaitForSeconds(1f);
        this.setPlayerFadeIn(false, true);
        StartGameIntro();
        yield return new WaitForSeconds(cameraIntroDuration-2);
        this.setPlayerFadeIn(true, true);
    }

    private void setPlayerFadeIn(bool fadeIn, bool displayRole=false)
    {

        foreach (var player in this.players)
        {
            player.RpcManageVision(fadeIn, (displayRole ? player.GetPlayerRole() : Role.Undefined));
        }
    }

    [Server]
    private void StartGameIntro()
    {
        // Animation duration (adjust as needed)
        int spawnedPlayerCount = 0;
        foreach (var player in this.players)
        {
            player.RpcTeleportPlayer(spawnPoints[spawnedPlayerCount++].transform.position);

            player.RpcSetCameraState(false);
            SetGameCameraEnabled(true);

            StartCoroutine(MoveCameraIntroCoroutine()); // Commencez l'animation d'introduction de la caméra
            //player.RpcPlayIntroAnimation(player.GetPlayerRole());

            // Attendez que l'animation d'introduction soit terminée
            //yield return new WaitForSeconds(introAnimationDuration);
        }

    }

    [Server]
    private void StartGameplay()
    {
        this.gameState = GameState.Processing;
        this.setPlayerFadeIn(false, false);
    }


    #region CameraSyncManagement 

    private IEnumerator MoveCameraIntroCoroutine()
    {
        Vector3 initialPosition = cameraPoints[0].transform.position;
        Vector3 targetPosition = cameraPoints[1].transform.position;
        float elapsedTime = 0.0f;

        while (elapsedTime < cameraIntroDuration)
        {
            float t = elapsedTime / cameraIntroDuration;
            gameCamera.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assurez-vous que la caméra soit exactement à la position cible à la fin de l'animation
        gameCamera.transform.position = targetPosition;
        foreach (var player in this.players)
        {
            player.RpcSetCameraState(true);
        }
        SetGameCameraEnabled(false);

        // Une fois toutes les animations d'introduction terminées, commencez la partie
        StartGameplay();
    }


    private void SetGameCameraEnabled(bool enabled)
    {
        // Convert the bool to a byte (0 or 1)
        cameraEnabledByte = (byte)(enabled ? 1 : 0);

        // Enable the camera on the server and synchronize it
        this.gameCamera.enabled = enabled;
    }

    private void OnCameraEnabledChanged(byte oldValue, byte newValue)
    {
        // Convert the byte back to a bool (0 -> false, 1 -> true)
        bool enabled = (newValue == 1);
        this.gameCamera.enabled = enabled;
    }
    #endregion


    #region TasksAndRoles

    [Server]
    private void AssignTasks()
    {
        foreach (var player in this.players)
        {
            if (player.GetPlayerRole() == Role.Crewmate)
            {
                // Distribuez des tâches aléatoirement aux joueurs
                List<InteractableObject> tasksForPlayer = new List<InteractableObject>();
                if (player.GetConnectionId() != 0)
                {
                    tasksForPlayer.Add(tasks[0]);
                }
                else
                {
                    tasksForPlayer.Add(tasks[1]);
                }

                Debug.Log("J'ajoute " + tasksForPlayer.Count + " taches au joueur " + player.GetConnectionId());
                player.RpcReceiveInteractableObjects(tasksForPlayer);
            }

        }
    }
    [Server]
    private void AssignImpostor()
    {
        int numberOfAssignedImpostors = 0;
        while (numberOfAssignedImpostors != this.impostorsCount)
        {
            PlayerController selectedPlayer = players[/*UnityEngine.Random.Range(0, players.Count)*/0];
            if (selectedPlayer.GetPlayerRole() != Role.Impostor)
            {
                selectedPlayer.RpcSetRole(Role.Impostor);
                numberOfAssignedImpostors++;
            }
        }
    }

    #endregion

    [Server]
    public bool AskInteract(InteractableObject obj)
    {
        Debug.Log("methode ask appellée!!");
        // Mettez ici la logique de vérification d'autorisation.
        // Par exemple, vérifiez si le joueur a terminé certaines tâches spécifiques, etc.
        // Renvoyez true si l'interaction est autorisée, sinon false.

        // Exemple : autoriser l'interaction si le joueur a encore des tâches à accomplir
        return true;
    }
    
    public GameState getGameState() { return this.gameState;  }
}
