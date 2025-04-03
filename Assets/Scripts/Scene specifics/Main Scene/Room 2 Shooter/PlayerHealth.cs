using UnityEngine;

/// <summary>
/// Basic player health system.
/// Reduces health on collision with enemies and logs death when health is depleted.
/// But... since this prototype is for porfolio porpuses, nothing is set up for the death, it's too easy to survive
/// and there is no point on dying.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int playerHealth = 10;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackEnemy"))
        {
            playerHealth --;

            if(playerHealth < 0)
            {
                Debug.Log("You died");
                //Death() player and reset game (through game Manager)
            }
        }
    }
}
