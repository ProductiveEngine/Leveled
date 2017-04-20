using UnityEngine;

public class GamePlayGui : MonoBehaviour
{
    GameObject arrowInstance;
    GameObject tempArrow;
    Transform customprefab;
    ControlScript carControlExternalScript;

    private bool ok = false;

    public Texture needle;

    float turn = 0.0f;

    void setUpGamePlayGui(Transform t)
    {
        customprefab = t;
    }

    void InstantiateArrow(Transform t)
    {
        tempArrow = Instantiate(arrowInstance, Vector3.zero, Quaternion.identity);

        tempArrow.transform.parent = transform;
        tempArrow.transform.localPosition = new Vector3(0, 1.7f, 3.7f);

        tempArrow.SendMessage("SetTarget", t);
    }
    void DestroyArrow()
    {
        Destroy(tempArrow.gameObject);
    }

    void Start()
    {
        Invoke("makeOK", 4);
    }

    void OnGUI()
    {
        GUI.depth = 0;
        //GUILayout.BeginArea( new Rect(Screen.width/2 +270,Screen.height/2 +200,300,350));
        GUILayout.BeginArea(new Rect(Screen.width / 2 + 390, Screen.height / 2 + 250, 50, 90));

        GUIUtility.RotateAroundPivot(turn, new Vector2(0.0f, 10.0f));
        //GUI.Label( new Rect(Screen.width/2,Screen.height/2,50,100),needle);

        //GUI.Label( new Rect(Screen.width/2,Screen.height/2,50,50),"fryf");
        GUILayout.Label(needle);
        GUILayout.EndArea();
    }

    void FixedUpdate()
    {
        if (ok)
        {
            turn = carControlExternalScript.engineRPM;
            turn = (turn / 3500) * 90 + 35;
        }

    }
    void makeOK()
    {
        ok = true;
        carControlExternalScript = customprefab.GetComponent<ControlScript>();
    }



}