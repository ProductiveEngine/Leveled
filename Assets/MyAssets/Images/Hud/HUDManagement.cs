using UnityEngine;

public class HUDManagement : MonoBehaviour
{

    public GUISkin customSkin;

    public Texture GUIOverlayTexture;
    public Texture speedometer;
    public Texture enemySkull;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        GUI.depth = 10;
        GUILayout.BeginArea(new Rect(1, Screen.height - (GUIOverlayTexture.height + 1), GUIOverlayTexture.width, GUIOverlayTexture.height));
        GUILayout.Label(GUIOverlayTexture);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(Screen.width - 350, Screen.height - 350, speedometer.width, speedometer.height));
        GUILayout.Label(speedometer);
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(0, 0, enemySkull.width, enemySkull.height));
        GUILayout.Label(enemySkull);
        GUILayout.EndArea();
    }
}
