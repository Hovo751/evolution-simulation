using System.Collections.Generic;
using UnityEngine;

public class GraphEditor : MonoBehaviour
{
    public List<float> dots;
    public List<RectTransform> dotsTF;
    public List<RectTransform> rectsTF;
    public int maxDots = 10;
    public GameObject dotExample;
    public GameObject rectExample;
    public float width;
    public float height;

    void Update()
    {
        List<RectTransform> dotsNew = new List<RectTransform>();
        List<RectTransform> rectsNew = new List<RectTransform>();
        int dotsOnScreen = Mathf.Min(maxDots, dots.Count);
        float max = GetMaxNumber(dots, maxDots);
        float min = GetMinNumber(dots, maxDots);
        Vector3 lastPos = new Vector3();
        Vector3 currentPos = new Vector3();

        for (int i = dotsOnScreen - 1; i >= 0; i--)
        {
            RectTransform newDot = Instantiate(dotExample, transform).GetComponent<RectTransform>();
            dotsNew.Add(newDot);
            newDot.localPosition = Vector3.Lerp(new Vector3(width / -2, 0, 0), new Vector3(width / 2 - 10, 0, 0), i / (dotsOnScreen - 1.0f));
            float dotVal = dots[dots.Count - dotsOnScreen + i];
            newDot.localPosition = Vector3.Lerp(newDot.localPosition - new Vector3(0, height / 2, 0), newDot.localPosition + new Vector3(0, height / 2 - 10, 0), dotVal / (max - min + 1));
            currentPos = newDot.localPosition + new Vector3(5 / Mathf.Sqrt(dotsOnScreen), 5 / Mathf.Sqrt(dotsOnScreen));
            newDot.localScale = new Vector3(1 / Mathf.Sqrt(dotsOnScreen), 1 / Mathf.Sqrt(dotsOnScreen));

            if (i != dotsOnScreen - 1)
            {
                RectTransform newRect = Instantiate(rectExample, transform).GetComponent<RectTransform>();
                rectsNew.Add(newRect);
                Vector3 dir = currentPos - lastPos;
                newRect.localPosition = (lastPos + currentPos) / 2;
                newRect.right = dir.normalized;
                newRect.sizeDelta = new Vector2(dir.magnitude, 2);
            }

            lastPos = newDot.localPosition + new Vector3(5 / Mathf.Sqrt(dotsOnScreen), 5 / Mathf.Sqrt(dotsOnScreen));
        }

        dotsTF.ForEach(delegate (RectTransform Obj)
        {
            Destroy(Obj.gameObject);
        });

        rectsTF.ForEach(delegate (RectTransform Obj)
        {
            Destroy(Obj.gameObject);
        });

        dotsTF = dotsNew;
        rectsTF = rectsNew;
    }

    float GetMaxNumber(List<float> list, int n)
    {
        float max = 0;
        n = Mathf.Min(list.Count, n);

        for(int i = list.Count - n; i < list.Count; i++)
        {
            if (list[i] > max) max = list[i];
        }

        return max;
    }
    float GetMinNumber(List<float> list, int n)
    {
        float min = 0;
        n = Mathf.Min(list.Count, n);

        for (int i = list.Count - n; i < list.Count; i++)
        {
            if (list[i] < min) min = list[i];
        }

        return min;
    }
}
