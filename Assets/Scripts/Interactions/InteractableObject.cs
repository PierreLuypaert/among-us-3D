using Mirror;
using UnityEngine;

public abstract class InteractableObject : NetworkBehaviour, IInteractable
{

    [SerializeField]
    protected string taskName;

    [SerializeField]
    protected string taskDescription;


    private ParticleSystem particules;

    protected bool taskAlreadyDone = false;

    public abstract void Interact();

    void Start()
    {
        // R�cup�rez le GameObject enfant nomm� "particles" qui contient le Particle System
        GameObject childObject = transform.Find("Particles").gameObject;

        // V�rifiez si le GameObject enfant a �t� trouv�
        if (childObject != null)
        {
            // Obtenez le composant Particle System du GameObject enfant
            this.particules = childObject.GetComponent<ParticleSystem>();
            // V�rifiez si le composant Particle System a �t� trouv�
            if (this.particules == null)
            {
                Debug.LogError("Le composant Particle System n'a pas été trouvé sur le GameObject enfant.");
            } else
            {
                this.particules.Stop();
            }
        }
        else
        {
            Debug.LogError("Le GameObject enfant 'Particles' n'a pas été trouvé.");
        }
    }

    [Client]
    public void ActivateParticule()
    {

        if (this.particules != null && !this.taskAlreadyDone)
        {
            this.particules.Play();
        }
    }

    [Client]
    public void DesactivateParticule()
    {
        if (this.particules != null)
        {
            this.particules.Stop();
        }
    }

    public void EndInteraction()
    {
        //DesactivateParticule();
        this.taskAlreadyDone = true;
        Debug.Log("EndInteraction");
    }


    public string getName()        { return this.taskName;        }
    public string getDescription() { return this.taskDescription; }
    public string getUIText()      {
        return "<color=" + (this.taskAlreadyDone ? "green" : "red") + ">" + this.taskName + "</color>";
    }

    public bool isAlreadyDone() { return this.taskAlreadyDone; }

}
