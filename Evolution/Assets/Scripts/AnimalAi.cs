using Pathfinding;
using TMPro;
using UnityEngine;

public class AnimalAi : MonoBehaviour
{
    public enum State
    {
        Searchnig, 
        Hungry,
        Thirsty,
        Lover
    }
    public State state = State.Searchnig;
    public Stats stats;
    public Transform target;
    public Transform borderStart;
    public Transform borderEnd;
    public Transform closestBush;
    public Transform closestWater;
    public Transform closestWoman;
    public GameObject child;
    public AIDestinationSetter dest;
    public TextMeshProUGUI text;

    private void Start()
    {
        stats.gender = Random.Range(0, 2) == 1;
    }

    private void StateAction()
    {
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
            dest.target = closestWoman;
            closestWoman.gameObject.GetComponent<AIDestinationSetter>().target = transform;
        }
        else
        {
            if (Vector2.Distance(target.position, transform.position) <= 1 || target.position == transform.position)
            {
                float randomAngle = Random.Range(0.001f, Mathf.PI * 2);
                target.position = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * Random.Range(0, stats.eyes) + transform.position;
                if (target.position.x > borderEnd.position.x) target.position = new Vector3(borderEnd.position.x, target.position.y);
                if (target.position.y > borderEnd.position.y) target.position = new Vector3(target.position.x, borderEnd.position.y);

                if (target.position.x < borderStart.position.x) target.position = new Vector3(borderStart.position.x, target.position.y);
                if (target.position.y < borderStart.position.y) target.position = new Vector3(target.position.x, borderStart.position.y);
                RaycastHit2D hit = Physics2D.Linecast(transform.position, target.position, LayerMask.GetMask("Water"));
                if (hit)
                {
                    Debug.DrawLine(transform.position, hit.point, new Color(0, 0, 1), 10);
                    target.position = hit.point;
                    target.position = target.position + new Vector3(0, 0, 0.001f);
                }
                dest.target = target;
            }
        }
    }

    void Update()
    {
        StateAction();
        GetEyes();
    }

    private void GetEyes()
    {
        Transform womanVal = closestWoman;
        Transform waternVal = null;
        Transform foodVal = null;

        for (int i = 0; i < 360; i += 14)
        {
            Vector2 dir = new Vector2(Mathf.Cos(i * Mathf.PI / 180), Mathf.Sin(i * Mathf.PI / 180));
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, dir, stats.eyes, LayerMask.GetMask("Water", "Animal", "Bush"));
            Color color = new Color(1, 0, 0);
            for (int j = 0; j < hit.Length; j++)
            {
                GameObject obj = hit[j].collider.gameObject;

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

                if (waternVal)
                {
                    if (obj.layer == 4 && Vector2.Distance(transform.position, waternVal.position) > Vector2.Distance(transform.position, hit[j].point))
                    {
                        waternVal.position = hit[j].point;
                    }
                }
                else
                {
                    if (obj.layer == 4) { waternVal = new GameObject().transform; waternVal.position = hit[j].point; waternVal.parent = transform.parent; }
                }

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
                    if (obj.layer == 3) if (obj.GetComponent<Bush>().berries >= 1) { foodVal = new GameObject().transform; foodVal.position = hit[j].point; foodVal.parent = transform.parent; }
                }

                if (Vector3.Distance(hit[j].collider.transform.position, transform.position) > 1 && obj.layer != 4)
                {
                    color = new Color(0, 1, 0);
                    break;
                }
            }
            //Debug.DrawRay(transform.position, dir * stats.eyes, color);
        }
        closestWoman = womanVal;
        if (foodVal) {
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
        if (collision.gameObject.layer == 6)
        {
            GameObject cow2 = collision.gameObject;
            if (!closestWoman) return;
            if (cow2 == closestWoman.gameObject && state == State.Lover)
            {
                if (stats.gender)
                {
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
                    text.text = (int.Parse(text.text) + 1).ToString();
                    if (childStats)
                    {
                        childAi.text = text;
                        childStats.health = 100;
                        childStats.maxHealth = 100;
                        childStats.hunger = 1;
                        childStats.thirsty = 1;
                        float closerTo = Random.Range(0, 1);
                        childStats.thirstynes = (stats.thirstynes - cow2.GetComponent<Stats>().thirstynes) * closerTo + cow2.GetComponent<Stats>().thirstynes + Random.Range(-0.1f, 0.1f);
                        childStats.thirstResistance = (stats.thirstResistance - cow2.GetComponent<Stats>().thirstResistance) * closerTo + cow2.GetComponent<Stats>().thirstResistance + Random.Range(-0.1f, 0.1f);
                        childStats.hungrines = (stats.hungrines - cow2.GetComponent<Stats>().hungrines) * closerTo + cow2.GetComponent<Stats>().hungrines + Random.Range(-0.1f, 0.1f);
                        childStats.hungerResistance = (stats.hungerResistance - cow2.GetComponent<Stats>().hungerResistance) * closerTo + cow2.GetComponent<Stats>().hungerResistance + Random.Range(-0.1f, 0.1f);
                        childStats.eyes = (stats.eyes - cow2.GetComponent<Stats>().eyes) * closerTo + cow2.GetComponent<Stats>().eyes + Random.Range(-1f, 1f);
                    }
                    if (childAi)
                    {
                        childAi.borderStart = borderStart;
                        childAi.borderEnd = borderEnd;
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            GameObject cow2 = collision.gameObject;
            if (!closestWoman) return;
            if (cow2 == closestWoman.gameObject && state == State.Lover)
            {
                if (stats.gender)
                {
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
                    text.text = (int.Parse(text.text) + 1).ToString();
                    if (childStats)
                    {
                        childAi.text = text;
                        childStats.health = 100;
                        childStats.maxHealth = 100;
                        childStats.hunger = 1;
                        childStats.thirsty = 1;
                        float closerTo = Random.Range(0, 1);
                        childStats.thirstynes = (stats.thirstynes - cow2.GetComponent<Stats>().thirstynes) * closerTo + cow2.GetComponent<Stats>().thirstynes + Random.Range(-0.1f, 0.1f);
                        childStats.thirstResistance = (stats.thirstResistance - cow2.GetComponent<Stats>().thirstResistance) * closerTo + cow2.GetComponent<Stats>().thirstResistance + Random.Range(-0.1f, 0.1f);
                        childStats.hungrines = (stats.hungrines - cow2.GetComponent<Stats>().hungrines) * closerTo + cow2.GetComponent<Stats>().hungrines + Random.Range(-0.1f, 0.1f);
                        childStats.hungerResistance = (stats.hungerResistance - cow2.GetComponent<Stats>().hungerResistance) * closerTo + cow2.GetComponent<Stats>().hungerResistance + Random.Range(-0.1f, 0.1f);
                        childStats.eyes = (stats.eyes - cow2.GetComponent<Stats>().eyes) * closerTo + cow2.GetComponent<Stats>().eyes + Random.Range(-1f, 1f);
                    }
                    if (childAi)
                    {
                        childAi.borderStart = borderStart;
                        childAi.borderEnd = borderEnd;
                    }
                }
            }
        }
    }
}