using Pathfinding;
using TMPro;
using UnityEngine;

public class AnimalAi : MonoBehaviour
{
    // Animal behavior states
    public enum State
    {
        Searchnig, // Typo, should be "Searching"
        Hungry,
        Thirsty,
        Lover
    }

    public State state = State.Searchnig;
    public Stats stats;
    public float searchTime = 5.0f;
    public float currentSearchTime = 0.0f;

    // Targets and references
    public Transform target;
    public Transform borderStart;
    public Transform borderEnd;
    public Transform closestBush;
    public Transform closestWater;
    public Transform closestWoman;
    public GameObject child;
    public GameObject worldController;
    public AIDestinationSetter dest;

    private void Start()
    {
        // Randomly assign gender
        stats.gender = Random.Range(0, 2) == 1;
    }

    private void StateAction()
    {
        // Set movement target based on state
        dest.target = target;

        if (state == State.Hungry && closestBush)
        {
            dest.target = closestBush;
        }
        else if (state == State.Thirsty && closestWater)
        {
            dest.target = closestWater;
        }
        else if (state == State.Lover && stats.canLove <= 0 && closestWoman)
        {
            // Set lover target and make the other animal follow this one
            dest.target = closestWoman;
            closestWoman.gameObject.GetComponent<AIDestinationSetter>().target = transform;
        }
        else
        {
            // Random roaming logic if no specific state
            if (Vector2.Distance(target.position, transform.position) <= 1 || target.position == transform.position || currentSearchTime > searchTime)
            {
                currentSearchTime = 0;
                // Pick random direction and position
                float randomAngle = Random.Range(0.001f, Mathf.PI * 2);
                target.position = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * Random.Range(0, stats.eyes) + transform.position;

                // Clamp position within borders
                if (target.position.x > borderEnd.position.x) target.position = new Vector3(borderEnd.position.x, target.position.y);
                if (target.position.y > borderEnd.position.y) target.position = new Vector3(target.position.x, borderEnd.position.y);
                if (target.position.x < borderStart.position.x) target.position = new Vector3(borderStart.position.x, target.position.y);
                if (target.position.y < borderStart.position.y) target.position = new Vector3(target.position.x, borderStart.position.y);

                // Avoid water
                RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, LayerMask.GetMask("Water"));
                if (hit)
                {
                    //Debug.DrawLine(transform.position, hit.point, new Color(0, 0, 1), 10);
                    target.position = new Vector3(hit.point.x, hit.point.y) + new Vector3(0, 0, 0.001f);
                }

                dest.target = target;
            }
        }
    }

    void Update()
    {
        // Constantly update state and environment awareness
        StateAction();
        GetEyes();
        currentSearchTime += Time.deltaTime;
    }

    private void GetEyes()
    {
        // Detect closest entities within vision range
        Transform womanVal = closestWoman;
        Transform waternVal = null;
        Transform foodVal = null;

        // Raycast in 360 degrees
        for (int i = 0; i < 360; i += 14)
        {
            Vector2 dir = new Vector2(Mathf.Cos(i * Mathf.PI / 180), Mathf.Sin(i * Mathf.PI / 180));
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, dir, stats.eyes, LayerMask.GetMask("Water", "Animal", "Bush"));

            Color color = new Color(1, 0, 0);

            for (int j = 0; j < hit.Length; j++)
            {
                GameObject obj = hit[j].collider.gameObject;

                // Find closest female (lover)
                if (womanVal)
                {
                    if (obj.layer == 6 && Vector2.Distance(transform.position, womanVal.position) > Vector2.Distance(transform.position, obj.transform.position) && obj.GetComponent<Stats>().canLove <= 0 && !obj.GetComponent<Stats>().gender)
                    {
                        womanVal = obj.transform;
                    }
                }
                else
                {
                    if (obj.layer == 6 && obj.GetComponent<Stats>().canLove <= 0 && !obj.GetComponent<Stats>().gender && obj.GetComponent<AnimalAi>().state != State.Lover)
                    {
                        womanVal = obj.transform;
                    }
                }

                // Find closest water
                if (waternVal)
                {
                    if (obj.layer == 4 && Vector2.Distance(transform.position, waternVal.position) > Vector2.Distance(transform.position, hit[j].point))
                    {
                        waternVal.position = hit[j].point;
                    }
                }
                else
                {
                    if (obj.layer == 4)
                    {
                        waternVal = new GameObject().transform;
                        waternVal.position = hit[j].point;
                        waternVal.parent = transform.parent;
                    }
                }

                // Find closest bush with berries
                if (foodVal)
                {
                    if (obj.layer == 3 && Vector2.Distance(transform.position, foodVal.position) > Vector2.Distance(transform.position, hit[j].point))
                    {
                        if (obj.GetComponent<Bush>().berries >= 1)
                            foodVal.position = hit[j].point;
                    }
                }
                else
                {
                    if (obj.layer == 3 && obj.GetComponent<Bush>().berries >= 1)
                    {
                        foodVal = new GameObject().transform;
                        foodVal.position = hit[j].point;
                        foodVal.parent = transform.parent;
                    }
                }

                // Stop drawing ray if object is not water and far
                if (Vector3.Distance(hit[j].collider.transform.position, transform.position) > 1 && obj.layer != 4)
                {
                    color = new Color(0, 1, 0);
                    break;
                }
            }
            //Debug.DrawRay(transform.position, dir * stats.eyes, color);
        }

        // Update detected targets
        closestWoman = womanVal;

        // Update closest bush
        if (foodVal)
        {
            if (closestBush)
            {
                closestBush.position = foodVal.position;
                Destroy(foodVal.gameObject);
            }
            else
            {
                closestBush = new GameObject().transform;
                closestBush.parent = transform;
                closestBush.position = foodVal.position;
                Destroy(foodVal.gameObject);
            }
        }
        else if (closestBush)
        {
            Destroy(closestBush.gameObject);
        }

        // Update closest water
        if (waternVal)
        {
            if (closestWater)
            {
                closestWater.position = waternVal.position;
                Destroy(waternVal.gameObject);
            }
            else
            {
                closestWater = new GameObject().transform;
                closestWater.position = waternVal.position;
                closestWater.parent = transform;
                Destroy(waternVal.gameObject);
            }
        }
        else if (closestWater)
        {
            Destroy(closestWater.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Handle mating logic when staying in contact
        if (collision.gameObject.layer == 6)
        {
            GameObject cow2 = collision.gameObject;
            if (!closestWoman) return;

            if (cow2 == closestWoman.gameObject && state == State.Lover)
            {
                if (stats.gender)
                {
                    // Mate and create child
                    dest.target = target;
                    stats.canLove = 10;
                    state = State.Searchnig;

                    cow2.GetComponent<Stats>().canLove = 10;
                    cow2.GetComponent<AnimalAi>().state = State.Searchnig;
                    cow2.GetComponent<AIDestinationSetter>().target = cow2.transform.parent.GetChild(1);

                    Debug.Log(1);

                    GameObject newChild = Instantiate(child);
                    newChild.transform.GetChild(1).position = cow2.transform.position;
                    newChild = newChild.transform.GetChild(0).gameObject;
                    newChild.transform.position = cow2.transform.position;

                    Stats childStats = newChild.GetComponent<Stats>();
                    AnimalAi childAi = newChild.GetComponent<AnimalAi>();

                    worldController.GetComponent<WorldController>().population++;

                    if (childStats)
                    {
                        // Inherit random traits from parents
                        childAi.worldController = worldController;
                        childStats.health = 100;
                        childStats.maxHealth = 100;
                        childStats.hunger = 1;
                        childStats.thirsty = 1;

                        float closerTo = Random.Range(0, 1);

                        childStats.thirstynes = (stats.thirstynes - cow2.GetComponent<Stats>().thirstynes) * closerTo + cow2.GetComponent<Stats>().thirstynes + Random.Range(-0.08f, 0.15f);
                        childStats.thirstResistance = (stats.thirstResistance - cow2.GetComponent<Stats>().thirstResistance) * closerTo + cow2.GetComponent<Stats>().thirstResistance + Random.Range(-0.08f, 0.15f);
                        childStats.hungrines = (stats.hungrines - cow2.GetComponent<Stats>().hungrines) * closerTo + cow2.GetComponent<Stats>().hungrines + Random.Range(-0.08f, 0.15f);
                        childStats.hungerResistance = (stats.hungerResistance - cow2.GetComponent<Stats>().hungerResistance) * closerTo + cow2.GetComponent<Stats>().hungerResistance + Random.Range(-0.08f, 0.15f);
                        childStats.eyes = (stats.eyes - cow2.GetComponent<Stats>().eyes) * closerTo + cow2.GetComponent<Stats>().eyes + Random.Range(-1.5f, 0.5f);
                    }

                    if (childAi)
                    {
                        // Inherit map borders
                        childAi.borderStart = borderStart;
                        childAi.borderEnd = borderEnd;
                    }
                }
            }
        }
    }
}