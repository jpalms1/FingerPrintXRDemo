// Take the force rendered by the finger this script is attached to and apply them to the object being interacted with

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class RenderForces : MonoBehaviour
{

    Rigidbody m_Rigidbody;
    GameObject index, thumb;

    Vector3 indexForce, thumbForce;

    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();

        // Fetch the index finger and thumb objects
        index = GameObject.Find("Index");
        thumb = GameObject.Find("Thumb");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // Get the forces rendered by index and thumb
        indexForce = index.GetComponent<FingerProxy>().force;
        thumbForce = thumb.GetComponent<FingerProxy>().force;

        //Apply a resultant force to this Rigidbody from index and thumb
        m_Rigidbody.AddForce(indexForce + thumbForce);
    }
}
