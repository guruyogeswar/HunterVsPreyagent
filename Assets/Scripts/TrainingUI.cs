using UnityEngine;
using TMPro;

public class TrainingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsText; // Text box for displaying agent statistics
    [SerializeField] private TextMeshProUGUI winText;   // Text box for displaying win counts

    private HunterController hunterController;
    private AgentController agentController;

    private int agentWins = 0;
    private int hunterWins = 0;

    void Start()
    {
        // Reference your agent scripts here
        hunterController = FindObjectOfType<HunterController>();
        agentController = FindObjectOfType<AgentController>();
    }

    void Update()
    {
        if (hunterController != null && agentController != null)
        {
            // Display combined stats for both agents
            statsText.text = $"Agent Statistics:\n" +
                             $"Steps: {agentController.StepCount}\n" +
                             $"Reward: {agentController.GetCumulativeReward():F2}\n\n" +
                             $"Hunter Statistics:\n" +
                             $"Steps: {hunterController.StepCount}\n" +
                             $"Reward: {hunterController.GetCumulativeReward():F2}\n";
        }

        // Update win counts dynamically (assuming methods to detect wins exist)
        if (agentController.HasWon()) // Example method, replace with your actual implementation
        {
            agentWins++;
            agentController.ResetWinFlag(); // Reset the win flag
        }

        if (hunterController.HasWon()) // Example method, replace with your actual implementation
        {
            hunterWins++;
            hunterController.ResetWinFlag(); // Reset the win flag
        }

        // Display win counts
        winText.text = $"Wins:\n" +
                       $"Agent: {agentWins}\n" +
                       $"Hunter: {hunterWins}";
    }
}
