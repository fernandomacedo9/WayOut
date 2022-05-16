using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatGauge : MonoBehaviour
{
    public Transform needle;
    [Range(1, 3)]public int lives = 3;

    private const float _startAngle = 210;
    private const float _angleModifier = 60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        needle.eulerAngles = new Vector3(0, 0, _startAngle - lives*_angleModifier);
    }
}
