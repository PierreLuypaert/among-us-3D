using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskInteract : InteractableObject
{
    [Client]
    public override void Interact()
    {
        if (!this.taskAlreadyDone)
        {
            this.DesactivateParticule();
            this.EndInteraction();
        }
    }
}
