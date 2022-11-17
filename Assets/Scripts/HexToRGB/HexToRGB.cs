using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexToRGB 
{
    // Start is called before the first frame update
    public Color HexToFloatNormalized(string hex)
    {
        float red = Mathf.RoundToInt(Convert.ToInt32(hex.Substring(0, 2), 16));
        float green = Mathf.RoundToInt(Convert.ToInt32(hex.Substring(2, 2), 16));
        float blue = Mathf.RoundToInt(Convert.ToInt32(hex.Substring(4, 2), 16));

        return new Color(red / 255, green / 255, blue / 255, 0.5f);
    }
}
