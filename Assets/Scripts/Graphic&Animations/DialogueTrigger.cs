// using UnityEngine;
// using Yarn.Unity;

// public class DialogueTrigger
// {
//     [Header("Dialogue Settings")]
//     [SerializeField] private string startNode = "Start";
//     [SerializeField] private bool canRepeat = false;
//     [SerializeField] private bool autoStart = true;

//     private bool hasTriggered = false;
//     private bool playerInRange = false;

//     public void Interact(GameObject interactor)
//     {
//         // Only allow interaction if autoStart is false and player is in range
//         if (!autoStart && playerInRange)
//         {
//             TryStartDialogue();
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInRange = true;
            
//             // If autoStart is enabled, start dialogue immediately
//             if (autoStart)
//             {
//                 TryStartDialogue();
//             }
//             // If autoStart is disabled, show interaction prompt or enable interaction
//             else
//             {
//                 // You can add visual feedback here like showing an "E to interact" prompt
//                 // For example: ShowInteractionPrompt(true);
//             }
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInRange = false;
            
//             // Hide interaction prompt if autoStart is disabled
//             if (!autoStart)
//             {
//                 // Hide visual feedback here
//                 // For example: ShowInteractionPrompt(false);
//             }
//         }
//     }

//     private void TryStartDialogue()
//     {
//         // Check if we should prevent starting dialogue
//         if (hasTriggered && !canRepeat) return;

//         var runner = FindObjectOfType<DialogueRunner>();
//         if (runner != null && !runner.IsDialogueRunning)
//         {
//             // Disable player input before starting dialogue
//             //TODO

//             runner.StartDialogue(startNode);
//             hasTriggered = true;

//             // Re-enable input when dialogue ends
//             runner.onDialogueComplete.AddListener(OnDialogueComplete);
//         }
//     }

//     private void OnDialogueComplete()
//     {

//         // Clean up the listener
//         var runner = FindObjectOfType<DialogueRunner>();
//         if (runner != null)
//         {
//             runner.onDialogueComplete.RemoveListener(OnDialogueComplete);
//         }
//     }

//     // Optional: Add visual feedback methods
//     /*
//     private void ShowInteractionPrompt(bool show)
//     {
//         // Implement visual feedback for interaction prompt
//         // This could show/hide a UI element or change sprite opacity
//     }
//     */
// }