using TMPro;
using UnityEngine;

public class FloatingName : MonoBehaviour
{
    
    [SerializeField] private CarController carController;
    private TextMeshPro floatingName;

    void Start()
    {
        floatingName = GetComponent<TextMeshPro>();
        floatingName.text = carController.gameObject.transform.name;
        floatingName.color = Random.ColorHSV();
    }

    // Update is called once per frame
    void Update()
    {
        floatingName.transform.LookAt(Camera.main.transform);
        floatingName.transform.Rotate(0f, 180f, 0f);
    }
}
