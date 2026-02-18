using UnityEngine;
using Yarn.Unity;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string startNode = "Start";
    [SerializeField] private bool canRepeat = false;
    [SerializeField] private bool autoStart = true;

    [Header("Player Control")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;
    private bool playerInRange = false;
    private GameObject playerObject;
    private MonoBehaviour[] playerScripts;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerObject = other.gameObject;
            
            if (autoStart)
            {
                TryStartDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        // Manual trigger with E key if autoStart is disabled
        if (!autoStart && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryStartDialogue();
        }
    }

    private void TryStartDialogue()
    {
        if (hasTriggered && !canRepeat) return;

        var runner = FindObjectOfType<DialogueRunner>();
        if (runner != null && !runner.IsDialogueRunning)
        {
            // DisablePlayerInput();
            runner.StartDialogue(startNode);
            hasTriggered = true;
            runner.onDialogueComplete.AddListener(OnDialogueComplete);
        }
    }

    private void OnDialogueComplete()
    {
        // EnablePlayerInput();
        
        var runner = FindObjectOfType<DialogueRunner>();
        if (runner != null)
        {
            runner.onDialogueComplete.RemoveListener(OnDialogueComplete);
        }
    }
}