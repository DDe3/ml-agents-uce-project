using UnityEngine;
using TMPro;
using System.Linq;


public class PositionUI : MonoBehaviour
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform Player;
    [SerializeField] private Animator animator;
    private TextMeshProUGUI positionUIDisplay;
    private CarPosition carPosition;
    private string position;
    private string numberOfCars;

    private void Update() 
    {
        UpdatePosition();
    }

    private void Start() 
    {
        positionUIDisplay = GetComponentInChildren<TextMeshProUGUI>();
        carPosition = trackCheckpoints.carPositions.Where(x => x.car == Player).First();
        position = (trackCheckpoints.carPositions.IndexOf(carPosition) + 1).ToString() + "/" + trackCheckpoints.GetAllCars().Count.ToString();
    }

    private void UpdatePosition() 
    {
        carPosition = trackCheckpoints.carPositions.Where(x => x.car == Player).First();
        int myPosition = trackCheckpoints.carPositions.IndexOf(carPosition) + 1;
        string positionCheck = myPosition.ToString() + "/" + trackCheckpoints.GetAllCars().Count.ToString();
        if (positionCheck != position) 
        {
            animator.SetTrigger("Change");
            position = positionCheck;
        }
        positionUIDisplay.text = position;
        Color lerpedColor = Color.Lerp(Color.white, Color.red, (float) myPosition / (float) trackCheckpoints.GetAllCars().Count);
        positionUIDisplay.color = lerpedColor;
    }


    
}
