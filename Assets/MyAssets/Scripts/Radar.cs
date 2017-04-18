using System;
using UnityEngine;

public class Radar : MonoBehaviour
{
    //@script ExecuteInEditMode()?????
    //radar! by PsychicParrot, adapted from a Blitz3d script found in the public domain online somewhere ..
    //Modified by Dastardly Banana to add radar size configuration, different colors for enemies in different states (patrolling or chasing), ability to move radar to either one of 9 preset locations or to custom location.
    //some lines are particular to our AI script, you will need to change "EnemyAINew" to the name of your AI script, and change "isChasing" to the bool  within that AI script that is true when the enemy is active/can see the player/is chasing the player.

    public Texture blip; // texture to use when the enemy isn't chasing
    public Texture blipChasing; //When Chasing
    public Texture radarBG;

    public Transform centerObject;
    float mapScale = 0.3f;
    float mapSizePercent = 15;

    bool checkAIscript = true;


    enum radarLocationValues { topLeft, topCenter, topRight, middleLeft, middleCenter, middleRight, bottomLeft, bottomCenter, bottomRight, custom }
    radarLocationValues radarLocation = radarLocationValues.topRight;

    private float mapWidth;
    private float mapHeight;
    private Vector2 mapCenter;
    Vector2 mapCenterCustom;

    void Start()
    {
        setMapLocation();
    }
    void SetTarget(Transform t)
    {
        centerObject = t;
    }
    private bool openRadar = false;

    void OpenRadar(int tpt)
    {
        openRadar = true;
    }

    void OnGUI()
    {
        //  GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, Vector3(Screen.width / 600.0f, Screen.height / 450.0f, 1));

        // Draw player blip (centerObject)
        if (openRadar)
        {
            try
            {
                float bX = centerObject.transform.position.x * mapScale;
                float bY = centerObject.transform.position.z * mapScale;
                GUI.DrawTexture(new Rect(mapCenter.x - mapWidth / 2, mapCenter.y - mapHeight / 2, mapWidth, mapHeight), radarBG);

                // Draw blips for Enemies
                DrawBlipsForEnemies();
            }
            catch (Exception ex)
            {

            }
        }

    }

    void drawBlip(GameObject go, Texture aTexture)
    {

        Vector3 centerPos = centerObject.position;
        Vector3 extPos = go.transform.position;

        // first we need to get the distance of the enemy from the player
        float dist = Vector3.Distance(centerPos, extPos);

        float dx = centerPos.x - extPos.x; // how far to the side of the player is the enemy?
        float dz = centerPos.z - extPos.z; // how far in front or behind the player is the enemy?

        // what's the angle to turn to face the enemy - compensating for the player's turning?
        float deltay = Mathf.Atan2(dx, dz) * Mathf.Rad2Deg - 270 - centerObject.eulerAngles.y;

        // just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
        float bX = dist * Mathf.Cos(deltay * Mathf.Deg2Rad);
        float bY = dist * Mathf.Sin(deltay * Mathf.Deg2Rad);

        bX = bX * mapScale; // scales down the x-coordinate so that the plot stays within our radar
        bY = bY * mapScale; // scales down the y-coordinate so that the plot stays within our radar

        if (dist <= mapWidth * .5 / mapScale)
        {
            // this is the diameter of our largest radar circle
            GUI.DrawTexture(new Rect(mapCenter.x + bX, mapCenter.y + bY, 10, 10), aTexture);
        }
    }

    void DrawBlipsForEnemies()
    {
        //You will need to replace isChasing with a variable from your AI script that is true when     the enemy is chasing the player, or doing watever you want it to be doing when it is red on   
        //the radar.
        //You will need to replace "EnemyAINew with the name of your AI script
        // Find all game objects tagged Enemy
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("carTag");

        float distance = Mathf.Infinity;

        // Iterate through them and call drawBlip 
        foreach (GameObject go in gos)
        {
            Texture blipChoice = blip;
            if (!go.GetComponent<ControlScript>().userControl)
            {
                //EnemyAI aiScript = go.GetComponent<"EnemyAI">();
                //  if(aiScript.isChasing)
                blipChoice = blipChasing;
            }
            drawBlip(go, blipChoice);
        }
    }

    void setMapLocation()
    {
        mapWidth = Screen.width * mapSizePercent / 100.0f;
        mapHeight = mapWidth;

        //sets mapCenter based on enum selection
        if (radarLocation == radarLocationValues.topLeft)
        {
            mapCenter = new Vector2(mapWidth / 2, mapHeight / 2);
        }
        else if (radarLocation == radarLocationValues.topCenter)
        {
            mapCenter = new Vector2(Screen.width / 2, mapHeight / 2);
        }
        else if (radarLocation == radarLocationValues.topRight)
        {
            mapCenter = new Vector2(Screen.width - mapWidth / 2, mapHeight / 2);
        }
        else if (radarLocation == radarLocationValues.middleLeft)
        {
            mapCenter = new Vector2(mapWidth / 2, Screen.height / 2);
        }
        else if (radarLocation == radarLocationValues.middleCenter)
        {
            mapCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        }
        else if (radarLocation == radarLocationValues.middleRight)
        {
            mapCenter = new Vector2(Screen.width - mapWidth / 2, Screen.height / 2);
        }
        else if (radarLocation == radarLocationValues.bottomLeft)
        {
            mapCenter = new Vector2(mapWidth / 2, Screen.height - mapHeight / 2);
        }
        else if (radarLocation == radarLocationValues.bottomCenter)
        {
            mapCenter = new Vector2(Screen.width / 2, Screen.height - mapHeight / 2);
        }
        else if (radarLocation == radarLocationValues.bottomRight)
        {
            mapCenter = new Vector2(Screen.width - mapWidth / 2, Screen.height - mapHeight / 2);
        }
        else if (radarLocation == radarLocationValues.custom)
        {
            mapCenter = mapCenterCustom;
        }

    }

}

