using UnityEngine;
using System.Collections;

public class Vector3Compare : MonoBehaviour {

    //Compare two Vector3s with a variable "thats close enough" range.
    //0.0001 about 1cm
    //0.0000001 about 1mm
    //that doesn't sound right but thats what the internet told me. Have a play and see what works for you!
    public static bool V3Equal(Vector3 a, Vector3 b, float howClose)
    {
        return Vector3.SqrMagnitude(a - b) < howClose;
    }

    //same thing, but ignores the Y axis cause fuck the Y axis.
    public static bool V3EqualWithoutY(Vector3 a, Vector3 b, int howClose, float yVal)
    {
        a = new Vector3((float)System.Math.Round((double)a.x, howClose), yVal, (float)System.Math.Round((double)a.z, howClose));
        b = new Vector3((float)System.Math.Round((double)b.x, howClose), yVal, (float)System.Math.Round((double)b.z, howClose));
        float temp = Vector3.SqrMagnitude(a - b);
        bool temp2 = temp < 0.01f;
        return temp2;
    }
}
