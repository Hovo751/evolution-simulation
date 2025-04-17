using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public GameObject[] animals;
    public GameObject[] water;
    public GameObject[] bushes;
    public GameObject PopulationGraph;
    public float population;
    public float lastPopulation = 0;
    public GameObject SpeedGraph;
    public float speed;
    public float lastSpeed = 0;
    public Transform gridStart;
    public Transform gridEnd;
    void Start()
    {
        
    }
    void Update()
    {
        if (lastPopulation != population)
        {
            PopulationGraph.GetComponent<GraphEditor>().dots.Add(population);
            lastPopulation = population;
        }
        if (lastSpeed != speed)
        {
            SpeedGraph.GetComponent<GraphEditor>().dots.Add(speed / population);
            lastSpeed = speed;
        }
    }
}
