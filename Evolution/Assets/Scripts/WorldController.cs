using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public GameObject[] animals;
    public GameObject[] water;
    public GameObject[] bushes;
    public GameObject PopulationGraph;
    public TextMeshProUGUI population;
    public float lastPopulation = 0;
    public Transform gridStart;
    public Transform gridEnd;
    void Start()
    {
        
    }
    void Update()
    {
        if (lastPopulation != float.Parse(population.text))
        {
            PopulationGraph.GetComponent<GraphEditor>().dots.Add(float.Parse(population.text));
            lastPopulation = float.Parse(population.text);
        }
    }
}
