using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatGauge : MonoBehaviour
{
    public Transform needle;
    [Range(1, 3)]public int lives = 3;

    private const float _startAngle = 210;
    private const float _angleModifier = 60;
    private int previousLives;

    // Start is called before the first frame update
    void Start()
    {
        previousLives = lives;
    }

    // Update is called once per frame
    void Update()
    {
        needle.eulerAngles = new Vector3(0, 0, _startAngle - lives*_angleModifier);

        if(previousLives != lives) {
            previousLives = lives;
            StartCoroutine(popAnimation());
        }
    }
     IEnumerator popAnimation()
    {
        float scaleStart = transform.localScale.x;
        yield return repeatScaleLerp(scaleStart, scaleStart*2.0f, 0.5f);
        yield return repeatScaleLerp(scaleStart*2.0f, scaleStart, 0.5f);
        yield return repeatScaleLerp(scaleStart, scaleStart*2.0f, 0.5f);
        yield return repeatScaleLerp(scaleStart*2.0f, scaleStart, 0.5f);
    }

    public IEnumerator repeatScaleLerp(float scaleStart, float scaleEnd, float time) {
        float i = 0.0f;
        float rate = (1.0f/time) * 2.0f;
        Vector3 start = new Vector3(scaleStart, scaleStart, scaleStart);
        Vector3 end = new Vector3(scaleEnd, scaleEnd, scaleEnd);
        while(i<1.0f) {
            i += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(start, end, i);
            yield return null;
        }
    }
}
