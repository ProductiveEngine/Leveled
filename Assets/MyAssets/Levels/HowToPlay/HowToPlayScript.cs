using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HowToPlayScript : MonoBehaviour {

    public GUISkin CustomSkin;

    private string Instructiontext1 = "Purpose of the game is to eliminate all your opponents.\n You can do that either by hitting them with missiles\\machineGun untill their health equals zero or by \"flag elimination\"\n\nFlag Elimination : \nEvery 30secs a flag is placed randomly in the arena.\n When somenone captures the flag the car that has the biggest euclideian distance from the flag is eliminated. ";
    private string ButtonInstrText = "Next";
    private int Times = 4;

    void OnGUI()
    {
        GUI.skin = CustomSkin;

        Rect baseRect = new Rect((float)(Screen.width/4), (float)(Screen.height/6),(float)(Screen.width/2) + 50f, (float)(Screen.height/1.8));
        GUI.Window(0, baseRect, DoMyInstructions, "How to play");

    }

    void DoMyInstructions(int windowID)
    {
        Rect rect1 = new Rect(30, 30, Screen.width/2 - 10, Screen.width/2 - 10);
        Rect rect2 = new Rect((float)(Screen.width/2.55), Screen.height/2, 80, 25);
        Rect rect3 = new Rect((float)(Screen.width/ 3.7), Screen.height/2, 80, 25);

        GUI.Label(rect1, Instructiontext1);

        if (GUI.Button(rect2, "close"))
        {
            SceneManager.LoadScene("MainScene");
        }
        if (GUI.Button(rect3, ButtonInstrText))
        {
            if (Times % 3 == 2)
            {
                Instructiontext1 = "Press \"A\" to fire homing missiles\nPress \"S\" to fire speed missiles\nPress \"D\" to fire power missiles \nPress \"W\" for the machine Gun  ";
                ButtonInstrText = "Next";
            }
            else if (Times % 3 == 1)
            {
                Instructiontext1 = "Arrows for the movement of the car\nPress \"Space\" for handbrake\nPress \"BackSpace\" for reseting the car ";
                ButtonInstrText = "Next";
            }
            else
            {
                Instructiontext1 = "Purpose of the game is to eliminate all your opponents.\n You can do that either by hitting them with missiles\\machineGun untill their health equals zero or by \"flag elimination\"\n\nFlag Elimination : \nEvery 30secs a flag is placed randomly in the arena.\n When somenone captures the flag the car that has the biggest euclideian distance from the flag is eliminated. ";
                ButtonInstrText = "Next";
            }
            Times++;
        }
    }
}
