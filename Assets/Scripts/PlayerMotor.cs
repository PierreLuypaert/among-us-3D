using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity;
    private Vector3 rotation;
    private Vector3 cameraRotation;


    private Rigidbody rb;

    private void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 _velocity)
    {
        this.velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        this.rotation = _rotation;
    }

    public void RotateCamera(Vector3 _cameraRotation)
    {
        this.cameraRotation = _cameraRotation;
    }

    private void FixedUpdate()
    {
        this.PerformMovement();
        this.PerformRotation();
    }

    private void PerformMovement()
    {
        if(this.velocity != Vector3.zero)
        {
            this.rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    private void PerformRotation()
    {
        this.rb.MoveRotation(this.rb.rotation * Quaternion.Euler(this.rotation));
        cam.transform.Rotate(-cameraRotation);

    }

    public InteractableObject GetInteractableForward(float distanceToUse)
    {
        // Lancez un rayon depuis la position de la souris
        RaycastHit hit;

        // Si le rayon touche un objet
        if (Physics.Raycast(this.cam.transform.position, this.cam.transform.forward, out hit, distanceToUse))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                return interactable;
            }
            else
            {
                Transform parent = hit.collider.transform.parent;
                if (parent == null) return null;
                InteractableObject parentInteractable = parent.GetComponent<InteractableObject>();
                if (parentInteractable != null)
                {
                    return parentInteractable;
                }
            }
        }
        return null;
    }

    public Camera getCamera()
    {
        return this.cam;
    }
}
