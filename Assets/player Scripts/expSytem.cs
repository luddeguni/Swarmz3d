using UnityEngine;

public class Player : MonoBehaviour
{
    public int level = 1; // Current player level
    public int exp = 0; // Current experience points
    public int expRequired = 100; // EXP required to level up

    // Method to gain experience
    public void GainExp(int amount)
    {
        exp += amount;
        Debug.Log($"Player gained {amount} EXP. Total EXP: {exp}");

        // Check if the player has enough EXP to level up
        while (exp >= expRequired)
        {
            LevelUp();
        }
    }

    // Method to level up
    void LevelUp()
    {
        level++; // Increase level
        exp -= expRequired; // Deduct the EXP required for the level up
        expRequired *= 2; // Double the EXP required for the next level up
        Debug.Log($"Level up! Player is now level {level}. Next level requires {expRequired} EXP.");
    }

    // For testing purposes, we can call this method when the game starts or during an event
    private void Start()
    {
        // For example, gain 250 EXP to see a few level-ups
        GainExp(250);
    }
}
