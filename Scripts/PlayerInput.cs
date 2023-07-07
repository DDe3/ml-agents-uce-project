using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private CarController controller;
    
    void Start()
    {
        this.controller = GetComponent<CarController>();
    }

    void Update()
    {
        GetInput();
    }

     private void GetInput() {
        controller.SetInputs(
            Input.GetAxis("Vertical"), 
            Input.GetAxis("Horizontal"), 
            Input.GetKey(KeyCode.Space));
    }
}
