using UnityEngine;


public class GridGenerator : MonoBehaviour
{

    float up = 639;
    float left = -288;

    float down = -269;
    float right = 620;

    int maxNumberOfColumns = 20;
    int maxNumberOfRows = 20;

    private float gridWidth;
    private float gridHeight;

    private float widthStep;
    private float heightStep;

    private float idCounter = 0.0f;

    void Awake()
    {
        NodeInfo[,] grid = new NodeInfo[maxNumberOfRows, maxNumberOfColumns];



        gridWidth = Mathf.Abs(left - right);
        gridHeight = Mathf.Abs(up - down);

        widthStep = gridWidth / maxNumberOfColumns;
        heightStep = gridHeight / maxNumberOfRows;

        int layerMask = 1 << 11;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        GameObject waypoint;

        for (int n = 0; n < maxNumberOfRows; n++)
        {
            for (int m = 0; m < maxNumberOfColumns; m++)
            {
                waypoint = new GameObject("" + (n + 1) + "_" + (m + 1));
                waypoint.transform.parent = transform;
                waypoint.transform.position = new Vector3((m) * widthStep + left, 1, -(n) * heightStep + up);

                bool checkAccessibility = !Physics.CheckSphere(new Vector3(waypoint.transform.position.x, waypoint.transform.position.y, waypoint.transform.position.z), 0.5f, layerMask);

                grid[n, m] = new NodeInfo(idCounter, checkAccessibility, waypoint.transform.position.x, waypoint.transform.position.z, n, m);
                idCounter = idCounter + 1.0f;
            }
        }
    }
}
