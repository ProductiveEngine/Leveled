using UnityEngine;
using UnityEngine.SceneManagement;


public class MainSceneScript : MonoBehaviour
{
    public GUISkin CustomSkin;

    public void OnGUI()
    {

        GUI.skin = CustomSkin;

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height - 250, 200, 200));
        GUILayout.BeginVertical();

        if (GUILayout.Button("Start Game"))
        {
            SceneManager.LoadScene("Track");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("How to play"))
        {
            SceneManager.LoadScene("HowToPlay");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Credits"))
        {
            SceneManager.LoadScene("Credits");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}

