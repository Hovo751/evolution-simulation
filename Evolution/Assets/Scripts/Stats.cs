using UnityEngine;

public class Stats : MonoBehaviour
{
    public float maxHealth = 100;
    public float health;
    public float hunger = 1;
    public float hungrines = 1;
    public float hungerResistance = 1;
    public float thirsty = 1;
    public float thirstynes = 1;
    public float thirstResistance = 1;
    public float eyes = 1;
    public bool gender;
    public float canLove = 10;
    public AnimalAi ai;

    private bool isTouchingWater = false;
    private bool isTouchingBush = false;
    private Bush bush;
    void Start()
    {
        health = maxHealth;
        hunger = 1;
        thirsty = 1;
    }

    void Update()
    {
        hungrines = Mathf.Max(hungrines, 0.05f);
        thirstynes = Mathf.Max(thirstynes, 0.05f);

        hungerResistance = Mathf.Max(hungerResistance, 1);
        thirstResistance = Mathf.Max(thirstResistance, 1);

        canLove -= Time.deltaTime;
        hunger = Mathf.Max(0, hunger - Time.deltaTime * hungrines);
        thirsty = Mathf.Max(0, thirsty - Time.deltaTime * thirstynes);

        if (isTouchingWater) thirsty = 1;
        if (isTouchingBush && hunger <= 0.2f && bush.berries >= 1) {hunger += bush.hunger; bush.berries--;}

        if (hunger == 0)
        {
            health -= Time.deltaTime * hungerResistance;
        }

        if (thirsty == 0)
        {
            health -= Time.deltaTime * thirstResistance;
        }

        health += Time.deltaTime;
        health = Mathf.Min(maxHealth, health);

        if (health < 0)
        {
            ai.text.text = (int.Parse(ai.text.text) - 1).ToString();
            Destroy(transform.parent.gameObject);
        }

        CalculateState();
    }

    private void CalculateState()
    {
        float foodPrior = (0.2f - hunger) * hungerResistance;
        float waterPrior = (0.2f - thirsty) * thirstResistance;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            isTouchingWater = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 4)
        {
            isTouchingWater = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isTouchingBush = true;
            bush = collision.gameObject.GetComponent<Bush>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            isTouchingBush = false;
        }
    }
}
