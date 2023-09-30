using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    Camera sceneCamera;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            this.DisableComponents();
        } else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            //Afficher le playerUI personnel
            this.playerUIInstance = Instantiate(playerUIPrefab);

            //Configuration du playerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if(ui == null)
            {
                Debug.LogError("Pas de component playerUi sur playerUIInstance");
            } else
            {
                GetComponent<PlayerController>().SetPlayerUI(ui);
            }
        }
    }

    private void DisableComponents()
    {
        //on bloque les trigger sur les autres joueurs
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(this.playerUIInstance);

        if (sceneCamera!=null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        //GameManager.UnregisterPlayer(transform.name);
    }
}
