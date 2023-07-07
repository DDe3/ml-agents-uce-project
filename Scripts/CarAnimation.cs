using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimation : MonoBehaviour
{
    private Animator myAnimator;
    [SerializeField] private CarController car;
    [SerializeField] private Collider carCapsulecollider;
    
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myAnimator.SetFloat("Steering", car.m_Steering);
        myAnimator.SetBool("Grounded", checkIfGrounded());
    }

    bool checkIfGrounded() {
        float DisstanceToTheGround = carCapsulecollider.bounds.extents.y;
        return Physics.Raycast(transform.position, Vector3.down, DisstanceToTheGround + 0.2f);
    }

}
