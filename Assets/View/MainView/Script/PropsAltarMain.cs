//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;




public class PropsAltarMain : MonoBehaviour
{
    public List<SpriteRenderer> runes;
    public float lerpSpeed;
    public Light2D BlueLight;

    private Color curColor;
    private Color onColor = new Color(1, 1, 1, 1);
    private Color offColor = new Color(1, 1, 1, 0);

    float curIntensity = 0;
    float time = 0;

    bool isOn;
    private void Awake()
    {
        isOn = false;
        curIntensity = 0;
        curColor = offColor;
    }

    private void Update()
    {
        //light.GetComponent<Light2D>
        // UnityEngine.U2D.Light2DBase.

        if (!isOn)
        {
            if(time * lerpSpeed >= 1)
            {
                time = 1f / lerpSpeed;
                curIntensity = 0;
                curColor = onColor;
                isOn = true;
            }


            //curIntensity = Mathf.Lerp(curIntensity, 0, lerpSpeed * Time.deltaTime);
            //curColor = Color.Lerp(curColor, onColor, lerpSpeed * Time.deltaTime);

            curIntensity = Mathf.Lerp(curIntensity, 0, lerpSpeed * time);
            curColor = Color.Lerp(curColor, onColor, lerpSpeed * time);

            time += Time.deltaTime;
          
        }
        else
        {
            if (time * lerpSpeed <= 0.001f)
            {
                time = 0;
                curIntensity = 1;
                curColor = offColor;
                isOn = false;
            }

         //  if( )
            curIntensity = Mathf.Lerp(curIntensity, 1, 1 - (lerpSpeed * time));
            curColor = Color.Lerp(curColor, offColor, 1 - (lerpSpeed * time));
            time -= Time.deltaTime;
           
          
        }

        BlueLight.falloffIntensity = curIntensity;
        foreach (var r in runes)
        {
            r.color = curColor;
        }
    }
}

