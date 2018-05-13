using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {

    public Transform pointPrefab;
    [Range(10, 100)] 
    public int resolution = 10;
    public GraphFunctionName function;

    Transform[] points;

    static GraphFunction[] functions = {
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, Ripple
    };

    void Awake() {
        float step          = 2f / resolution;   // 2f to limit values within -1, 1 domain
        Vector3 scale       = Vector3.one * step;
        points              = new Transform[resolution * resolution];

        for (int i = 0; i < points.Length; i++) {
            Transform point = Instantiate (pointPrefab);
            point.localScale = scale;
            point.SetParent (transform, false);
            points [i] = point;
        }
    }

    void Update() {
        float t = Time.time;
        GraphFunction f = functions [(int)function];
        float step = 2f / resolution;
        for (int z = 0, i = 0; z < resolution; z++) {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++) {
                float u = (x + 0.5f) * step - 1f;

                points [i].localPosition = f (u, v, t);
            }
        }
    }

    const float pi = Mathf.PI;

    // f(x,t) = sin(pi(x + t))
    static Vector3 SineFunction(float x, float z, float t) {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin (pi * (x + t));
        p.z = z;

        return p;
    }

    // f(x,z,t) = sin(pi(x + z + t))
    static Vector3 Sine2DFunction(float x, float z, float t) {
        Vector3 p;
        p.x = x;

        p.y = Mathf.Sin (pi * (x + t));
        p.y += Mathf.Sin (pi * (z + t));
        p.y *= 0.5f;

        p.z = z;

        return p;
    }

    static Vector3 MultiSineFunction(float x, float z, float t) {
        Vector3 p;
        p.x = x;

        p.y = Mathf.Sin (pi * (x + t));
        p.y += Mathf.Sin (2f * pi * (x + (2f * t))) * 0.5f;  // twice as fast, half the size
        // when adding the 2nd func we get values betw. -1.5, 1.5, so scale back down to -1, 1
        p.y *= 2f / 3f; // mult by 2/3

        p.z = z;

        return p;
    }

    // f(x,z,t) = M + Sx + Sz   // Main wave + 2ndary(x) + 2ndary(z)
    static Vector3 MultiSine2DFunction(float x, float z, float t) {
        Vector3 p;
        p.x = x;

        p.y = 4f * Mathf.Sin(pi * (x + z + t * .5f));
        p.y += Mathf.Sin (pi * (x + t));
        p.y += Mathf.Sin (2f * pi * (z + 2f * t) * .5f);
        p.y *= 1f / 5.5f; // normalize to -1, 1 range

        p.z = z;

        return p;
    }

    static Vector3 Ripple (float x, float z, float t) {
        Vector3 p;
        float d = Mathf.Sqrt (x * x + z * z);   // distance from origin, Pythagorean theorem

        p.x = x;
        p.y = Mathf.Sin(pi * (4f * d - t));
        p.y /= 1f + 10f * d;
        p.z = z;

        return p;
    }
}
