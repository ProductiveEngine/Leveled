using System.Collections;
using UnityEngine;

public class EndGameScript : MonoBehaviour
{
    GUISkin customSkin;

    void Awake()
    {
        Time.timeScale = 1;
    }

    private bool userIsDead = false;

    void OnGUI()
    {
        GUI.skin = customSkin;

        if (userIsDead)
        {
            Time.timeScale = 0;
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 50, Screen.height / 2, 150, 100));
            GUILayout.Label("LOOSER !!!!!!!!!!!");

            if (GUILayout.Button("Restart"))
            {
                Application.LoadLevel((Application.loadedLevelName));
            }
            if (GUILayout.Button("Quit"))
            {
                Application.LoadLevel("Go Race");
            }
            GUILayout.EndArea();
        }
    }

    public IEnumerator SetUserIsDead(bool dead)
    {
        yield return new WaitForSeconds(2);
        userIsDead = dead;
    }
}