
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private TextMeshProUGUI taskText;
    private Image useImage; 
    private Animator fade;


    private TextMeshProUGUI roleText;
    private GameObject role;


    private void Start()
    {
        GameObject taskTextGO = transform.Find("Text").gameObject;
        this.taskText = taskTextGO.GetComponent<TextMeshProUGUI>();

        GameObject imageGO = transform.Find("Image").gameObject;
        this.useImage = imageGO.GetComponent<Image>();


        this.role = transform.Find("Role").gameObject;

        GameObject roleText = role.transform.Find("RoleText").gameObject;
        this.roleText = roleText.GetComponent<TextMeshProUGUI>();


        fade = GetComponent<Animator>();

    }

    public void canInteract(bool canInteract)
    {
        if (useImage == null) return;
        // Assombrir l'image en ajustant la composante alpha de la couleur
        Color imageColor = useImage.color;

        if (canInteract)
        {
            // Image normale (composante alpha à 1)
            imageColor.a = 1f;
        }
        else
        {
            // Image assombrie (composante alpha réduite)
            imageColor.a = 0.5f; // Vous pouvez ajuster cette valeur pour contrôler le niveau d'assombrissement
        }

        useImage.color = imageColor;
    }

    public void setTaskText(string text)
    {
        if (taskText!=null)
            taskText.text = text;
    }


    public void setFade(bool fadeIn, Role roleToDisplay=Role.Undefined)
    {
        fade.SetBool("fadeIn", fadeIn);
        if (roleToDisplay!=Role.Undefined)
        {
            this.role.SetActive(true);
            this.roleText.text = "<color=" + (roleToDisplay == Role.Crewmate ? "green" : "red") + ">" + (roleToDisplay == Role.Crewmate ? "CREWMATE" : "IMPOSTOR") + "</color>";
        }
        else
            this.role.SetActive(false);



    }
   
}
