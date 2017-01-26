using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{

    public GUISkin CustomSkin;

    void OnGUI()
    {
        GUI.skin = CustomSkin;

        GUILayout.BeginArea(new Rect(50, 50, 900, 600));
        GUILayout.Label("                                         CREDITS\n");

        StringBuilder sb = new StringBuilder();
        sb.Append("Game Engine: Unity 3D \n\n");
        sb.Append("3D Modeling Applications : Autodesk Maya 2009 \n");
        sb.Append("2D Application : Photoshop CS4\n\n");
        sb.Append("Geometry\n");
        sb.Append(" Car : http://www.gotow.net/andrew/blog/?page_id=78 \n");
        sb.Append(" Lamp : Vag(www.madseeds.com)\n");
        sb.Append(" Garbage Bucket : Vag(www.madseeds.com) \n\n");
        sb.Append("Books\n");
        sb.Append(" [1] Start Your Engines : Developing Driving and Racing Games, Jim Parker (2005)\n");
        sb.Append(" [2] AI for Game Developers, David M. Bourg, Glenn Seeman (2004)\n");
        sb.Append(" [3] Programming Game AI by Example, Mat Bukcland  (2005)\n");
        sb.Append(" [4] Algorithms in Java, Robert Sedgewick\n");

        GUILayout.TextArea(sb.ToString());
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(20, Screen.height / 2 + 300, 100, 350));

        if (GUILayout.Button("Back"))
        {
            SceneManager.LoadScene("MainScene");
        }
        GUILayout.EndArea();
    }
}
