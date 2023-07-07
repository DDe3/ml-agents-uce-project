using UnityEngine;
using TMPro;

public class LapUI : MonoBehaviour
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Animator animator;
    private TextMeshProUGUI lapUIDisplay;
    

    private void Start() {
        trackCheckpoints.OnLapUpdate += TrackCheckpoints_OnLapUpdate;
        lapUIDisplay = GetComponentInChildren<TextMeshProUGUI>();
        lapUIDisplay.text = "Lap 0"  + "/" + trackCheckpoints.laps;
    }

    private void TrackCheckpoints_OnLapUpdate(object sender, int lapNumber)
    {
        if (lapNumber < trackCheckpoints.laps) {
            animator.SetTrigger("Change");
            lapUIDisplay.text = "Lap " + lapNumber + "/" + trackCheckpoints.laps;
        } else if (lapNumber == trackCheckpoints.laps) {
            animator.SetTrigger("Change");
            lapUIDisplay.text = "Final lap!";
        } else {
            gameObject.SetActive(false);
        }
    }

}
