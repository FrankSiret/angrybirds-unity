using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    public float getAccelerate() {
        Rigidbody2D bird = GetComponent<Rigidbody2D>();
        return bird.velocity.magnitude;
    }
}
