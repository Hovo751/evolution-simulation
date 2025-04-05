using UnityEngine;

public class Stats : MonoBehaviour
{
    // Animal stat values
    public float maxHealth = 100;
    public float health;
    public float hunger = 1;
    public float hungrines = 1; // How fast hunger goes down
    public float hungerResistance = 1; // How much damage from hunger
    public float thirsty = 1;
    public float thirstynes = 1; // How fast thirst goes down
    public float thirstResistance = 1; // How much damage from thirst
    public float eyes = 1; // Sight range
    public bool gender; // True = male, False = female
    public float canLove = 10; // Cooldown before mating
    public AnimalAi ai; // Reference to the animal AI

    private bool isTouchingWater = false;
    private bool isTouchingBush = false;
    private Bush bush;

    void Start()
    {
        // Set health, hunger, and thirst to max at start
        health = maxHealth;
        hunger = 1;
        thirsty = 1;
    }

    void Update()
    {
        // Clamp some values to prevent going too low
        hungrines = Mathf.Max(hungrines, 0.05f);
        thirstynes = Mathf.Max(thirstynes, 0.05f);
        hungerResistance = Mathf.Max(hungerResistance, 1);
        thirstResistance = Mathf.Max(thirstResistance, 1);

        // Cooldown for mating
        canLove -= Time.deltaTime;

        // Reduce hunger and thirst over time
        hunger = Mathf.Max(0, hunger - Time.deltaTime * hungrines);
        thirsty = Mathf.Max(0, thirsty - Time.deltaTime * thirstynes);

        // Drink water when touching water
        if (isTouchingWater) thirsty = 1;

        // Eat berries if hungry and touching bush
        if (isTouchingBush && hunger <= 0.2f && bush.berries >= 1)
        {
            hunger += bush.hunger;
            bush.berries--;
        }

        // Take damage from starvation
        if (hunger == 0)
        {
            health -= Time.deltaTime * hungerResistance;
        }

        // Take damage from dehydration
        if (thirsty == 0)
        {
            health -= Time.deltaTime * thirstResistance;
        }

        // Slowly heal over time
        health += Time.deltaTime;
        health = Mathf.Min(maxHealth, health);

        // Die if health too low
        if (health < 0)
        {
            ai.text.text = (int.Parse(ai.text.text) - 1).ToString();
            Destroy(transform.parent.gameObject);
        }

        // Update AI state based on needs
        CalculateState();
    }

    private void CalculateState()
    {
        // Determine which need is most urgent
        float foodPrior = (0.2f - hunger) * hungerResistance;
        float waterPrior = (0.2f - thirsty) * thirstResistance;

        // Set AI state based on needs
        if (waterPrior >= foodPrior && waterPrior > 0 && ai.closestWater)
        {
            ai.state = AnimalAi.State.Thirsty;
        }
        else if (foodPrior > 0)
        {
            ai.state = AnimalAi.State.Hungry;
        }
        else if (canLove <= 0 && ai.closestWoman && gender)
        {
            ai.state = AnimalAi.State.Lover;
            ai.closestWoman.GetComponent<AnimalAi>().state = AnimalAi.State.Lover;
        }
        else
        {
            ai.state = AnimalAi.State.Searchnig;
        }
    }

    // Detect water collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            isTouchingWater = true;
        }
    }

    // Exit water
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            isTouchingWater = false;
        }
    }

    // Detect bush trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isTouchingBush = true;
            bush = collision.gameObject.GetComponent<Bush>();
        }
    }

    // Exit bush trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isTouchingBush = false;
        }
    }
}
