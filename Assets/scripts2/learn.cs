using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class learn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    public float speed = 1.0f;

    void Update()
    {
        // calculate the distance between this object and the target object
        // and move a small portion of that distance each frame:

        var delta = obj.transform.position - transform.position;
        transform.position += delta * Time.deltaTime * speed;
    }
}
