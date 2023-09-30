using Mirror;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : NetworkBehaviour
{
    private GameManager gameManager;


    [SerializeField]
    private float distanceToUse = 2.5f;

    [SerializeField]
    private float speed = 10f;

    [SerializeField]
    private float mouseSensitivityX = 10f;
    [SerializeField]
    private float mouseSensitivityY = 15f;

    private PlayerMotor motor;

    private List<InteractableObject> tasks;

    private PlayerUI ui;


    private int connId;
    private Role role;

    private void Start()
    {
        this.motor = GetComponent<PlayerMotor>();
        this.gameManager = FindObjectOfType<GameManager>();
        this.role = Role.Crewmate;
    }
    private void Update()
    {
        // Calculer la vélocité du mouvement de notre joueur
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * this.speed;

        this.motor.Move(velocity);

        // On calcule la rotation du joueur en un Vector3
        float yRot = Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("RightJoystickX"); // Ajout du joystick droit

        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;

        motor.Rotate(rotation);

        // On calcule la rotation de la caméra en un Vector3
        float xRot = Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("RightJoystickY"); // Ajout du joystick droit

        Vector3 cameraRotation = new Vector3(xRot, 0, 0) * mouseSensitivityY;

        motor.RotateCamera(cameraRotation);

        InteractableObject target = motor.GetInteractableForward(this.distanceToUse);
        if (target != null && this.tasks != null && !target.isAlreadyDone())
        {
            canInteract(this.tasks.Contains(target));
        }
        else
        {
            canInteract(false);
        }

        // Vérifier si le joueur clique avec le bouton gauche de la souris
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1") ) // Utiliser un bouton de la manette (par exemple, A sur une manette Xbox)
        {
            InteractableObject obj = motor.GetInteractableForward(this.distanceToUse);
            if (obj != null && this.tasks != null)
            {
                if (this.tasks.Contains(obj))
                {
                    TryInteract(obj); ; // TODO: vérifier si le serveur est d'accord
                    canInteract(false);
                }
            }
        }

        ManageUI();

    }


    private void ManageUI()
    {
        if (this.tasks == null || this.ui == null || this.gameManager.getGameState() != GameState.Processing) return;

        string taskText = "";
        foreach (var task in this.tasks)
        {
            taskText += task.getUIText() + "\n";
        }

        this.ui.setTaskText(taskText);
    }

    #region Interactions
    public void canInteract(bool canInteract)
    {
        if (this.ui != null)
        {
            this.ui.canInteract(canInteract);
        }
    }

    public bool HasTaskToDo(InteractableObject task)
    {
        if (this.tasks == null) return false;
        return this.tasks.FindAll(t=> t.isAlreadyDone() == false).Contains(task);
    }

    [Client]
    public void TryInteract(InteractableObject obj)
    {
        if (!isLocalPlayer)
            return;

        // Demandez au serveur s'il est autorisé d'interagir avec l'objet
        if (isServer)
        {
            // Si vous êtes sur le serveur, vous pouvez décider directement
            // Si l'interaction est autorisée ou non. Vous pouvez appeler la fonction AskInteract
            // du GameManager pour cela.
            bool canInteract = gameManager.AskInteract(obj);
            if (canInteract)
            {
                obj.Interact();
            }
        }
        else
        {
            // Si vous êtes sur le client, demandez au serveur si vous pouvez interagir avec l'objet.
            CmdRequestInteract(obj);
        }
        this.canInteract(false);
    }

    [Command]
    private void CmdRequestInteract(InteractableObject obj)
    {
        // Demandez au serveur s'il est autorisé d'interagir avec l'objet
        bool canInteract = gameManager.AskInteract(obj);
        if (canInteract)
        {
            // Appelez la fonction TargetRpc sur le client pour effectuer l'interaction
            RpcPerformInteract(obj);
        }
    }

    #endregion

    #region RPC

    [ClientRpc]
    public void RpcReceiveInteractableObjects(List<InteractableObject> interactableObjects)
    {
        if (isLocalPlayer || isServer)
        {
            this.tasks = interactableObjects;
            foreach (var task in this.tasks)
            {
                if (isLocalPlayer)
                    task.ActivateParticule();
            }
        }
    }

    [ClientRpc]
    public void RpcSetRole(Role role)
    {
        if (isLocalPlayer || isServer)
        {
            this.role = role;
        }
    }

    [ClientRpc]
    public void RpcTeleportPlayer(Vector3 position) 
    {
        if (isLocalPlayer)
        {
            this.gameObject.transform.position = position;
        }
    }


    [ClientRpc]
    public void RpcManageVision(bool fade, Role roleToDisplay)
    {
        if (isLocalPlayer && this.ui != null)
        {
            this.ui.setFade(fade, roleToDisplay);
        }
    }



    [ClientRpc]
    private void RpcPerformInteract(InteractableObject obj)
    {
        // Assurez-vous que cette fonction est appelée uniquement sur le client
        if (!isLocalPlayer)
            return;

        // Appelez la fonction Interact() sur l'objet
        obj.Interact();
    }



    [ClientRpc]
    public void RpcSetCameraState(bool cameraState)
    {
        // Assurez-vous que cette fonction est appelée uniquement sur le client
        if (!isLocalPlayer)
            return;

        this.motor.getCamera().gameObject.SetActive(cameraState);

    }

    #endregion

    #region Getters and setters
    [Server]
    public int GetConnectionId()
    {
        return this.connId;
    }

    [Server]
    public void SetConnectionId(int connId)
    {
        this.connId = connId;
    }

    [Server]
    public Role GetPlayerRole()
    {
        return this.role;
    }

    public void SetPlayerUI(PlayerUI _ui)
    {
        this.ui = _ui;
    }
    #endregion


}
