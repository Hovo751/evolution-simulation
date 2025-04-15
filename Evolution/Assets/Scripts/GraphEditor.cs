using System.Collections.Generic;
using UnityEngine;

public class GraphEditor : MonoBehaviour
{
    public List<float> dots;
    public List<RectTransform> dotsTF;
    public int maxDots = 10;
    public GameObject dotExample;
    public float width;
    public float height;

    void Update()
    {
        List<RectTransform> dotsNew = new List<RectTransform>();
        int dotsOnScreen = Mathf.Max(maxDots, dots.Count);
        int max = 0;


        for (int i = 0; i < dotsOnScreen; i++)
        {
            RectTransform newDot = Instantiate(dotExample, transform).GetComponent<RectTransform>();
            dotsNew.Add(newDot);
            newDot.localPosition = Vector3.Lerp(new Vector3(width / -2, 0, 0), new Vector3(width / 2, 0, 0), i / (dotsOnScreen - 1.0f));
        }

        dotsTF.ForEach(delegate (RectTransform Obj)
        {
            Destroy(Obj.gameObject);
        });

        dotsTF = dotsNew;
    }
}
