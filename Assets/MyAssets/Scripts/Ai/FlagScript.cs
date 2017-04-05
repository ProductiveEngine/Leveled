using System;
using UnityEngine;

public class FlagScript : MonoBehaviour
{

    int maxNumberOfColumns = 20;
    int maxNumberOfRows = 20;

    GameObject flagInstance;

    float threshold = 45.4f;

    private float neighbors_Gscore;
    private NodeInfo[][] grid;

    private NodeInfo startNode;

    private GridGenerator gridGenerator;

    void Start()
    {
        /*
        for (int i = 0; i < maxNumberOfRows; i++)
        {
            grid[i] = new Array(maxNumberOfColumns);

        }
        */

        gridGenerator = GameObject.Find("Waypoints").GetComponent<GridGenerator>();

        //grid = gridGenerator.grid.slice(0);

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //obj = DeserializeArrayList(Application.dataPath + "/projectdb/edge.xml", extraTypes);
        //neighbors_Gscore = obj.ToArray(EdgeInfo);

        Invoke("InstantiateFlag", 30);

    }

    Array pathArray;
    int x;
    int y;

    //-------------------------------------------------------------------------------
    void InstantiateFlag()
    {
        return;

        // Find all game objects with tag Enemy
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("carTag");

        while (true)
        {
            x = (int)Mathf.Round((UnityEngine.Random.value) * 19);
            y = (int)Mathf.Round((UnityEngine.Random.value) * 19);

            if ((grid[x][y] as NodeInfo).accessible)
            {
                break;
            }
        }

        GameObject tempFlag = (GameObject)Instantiate(flagInstance, new Vector3((grid[x][y] as NodeInfo).posX, 1, (grid[x][y] as NodeInfo).posY), transform.rotation);
        Camera.main.SendMessage("InstantiateArrow", tempFlag);

        //path identifier
        int pathID = 0;

        foreach (GameObject go in gos)
        {
            if (!go.GetComponent<ControlScript>().userControl)
            {
                startNode = FindStart(go.transform.position.x, go.transform.position.z);
                Search(grid, startNode, grid[x][y] as NodeInfo);

                // Ektipwsi monopatiou
                /*
                for(j=0;j<ret.length;j++)
                {
                    waypoint = GameObject("Path id : "+(pathID)+" "+j+" x : "+(ret[j].gridPosX+1)+" y : "+(ret[j].gridPosY+1) ) ;
                    waypoint.transform.position.z = ret[j].posY ;
                    waypoint.transform.position.x = ret[j].posX;

                }
                */
                go.BroadcastMessage("populateWaypoints", ret);
                ResetGrid();
            }
            //pathID++;				
        }
    }

    private GameObject currentGO;

    void ResetGrid()
    {
        //grid = new NodeInfo[10][10];

        for (int i = 0; i < maxNumberOfRows; i++)
        {
            grid[i] = new NodeInfo[maxNumberOfColumns];

        }

        //grid = gridGenerator.grid.slice(0);
    }

    public void ResetFlag()
    {

        // Find all game objects with tag Enemy
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("carTag");
        foreach (GameObject go in gos)
        {
            if (!go.GetComponent<ControlScript>().userControl)
            {
                go.BroadcastMessage("ResetFlag", 5);
            }
        }

        Camera.main.SendMessage("DestroyArrow", 5);
        Invoke("InstantiateFlag", 30);
    }

    // Find the node that is closer to the car position
    public NodeInfo FindStart(float goX, float goZ)
    {
        float tempX = goX;
        float tempY = goZ;
        int tempGridX = 0;
        int tempGridY = 0;
        float closestNeighborDistance = Mathf.Infinity;
        int closestNeighbor = 0;

        bool ok1 = false;
        bool ok2 = false;
        bool ok3 = false;
        bool ok4 = false;
        bool ok5 = false;
        bool ok6 = false;
        bool ok7 = false;
        bool ok8 = false;

        NodeInfo temp;

        for (int i = 0; i < grid.Length; i++)
        {
            temp = grid[1][i] as NodeInfo;

            if (Mathf.Abs(tempX - temp.posX) <= threshold)
            {
                tempGridY = i;

                break;
            }

        }

        for (int j = 0; j < grid.Length; j++)
        {
            temp = grid[j][tempGridY] as NodeInfo;

            if (Mathf.Abs(tempY - temp.posY) <= threshold)
            {
                tempGridX = j;

                break;
            }

        }

        float zero = Heuristic(grid[tempGridX][tempGridY].posX, grid[tempGridX][tempGridY].posY, tempX, tempY);
        float one, two, three, four, five, six, seven, eight;

        try
        {
            one = Heuristic(grid[tempGridX - 1][tempGridY - 1].posX, grid[tempGridX - 1][tempGridY - 1].posY, tempX, tempY);
            ok1 = true;
        }
        catch (Exception) { one = Mathf.Infinity; }
        try
        {
            two = Heuristic(grid[tempGridX + 1][tempGridY + 1].posX, grid[tempGridX + 1][tempGridY + 1].posY, tempX, tempY);
            ok2 = true;
        }
        catch (Exception) { two = Mathf.Infinity; }
        try
        {
            three = Heuristic(grid[tempGridX - 1][tempGridY + 1].posX, grid[tempGridX - 1][tempGridY + 1].posY, tempX, tempY);
            ok3 = true;
        }
        catch (Exception) { three = Mathf.Infinity; }
        try
        {
            four = Heuristic(grid[tempGridX + 1][tempGridY - 1].posX, grid[tempGridX + 1][tempGridY - 1].posY, tempX, tempY);
            ok4 = true;
        }
        catch (Exception) { four = Mathf.Infinity; }
        try
        {
            five = Heuristic(grid[tempGridX - 1][tempGridY].posX, grid[tempGridX - 1][tempGridY].posY, tempX, tempY);
            ok5 = true;
        }
        catch (Exception) { five = Mathf.Infinity; }
        try
        {
            six = Heuristic(grid[tempGridX][tempGridY - 1].posX, grid[tempGridX][tempGridY - 1].posY, tempX, tempY);
            ok6 = true;
        }
        catch (Exception) { six = Mathf.Infinity; }
        try
        {
            seven = Heuristic(grid[tempGridX + 1][tempGridY].posX, grid[tempGridX + 1][tempGridY].posY, tempX, tempY);
            ok7 = true;
        }
        catch (Exception) { seven = Mathf.Infinity; }
        try
        {
            eight = Heuristic(grid[tempGridX][tempGridY + 1].posX, grid[tempGridX][tempGridY + 1].posY, tempX, tempY);
            ok8 = true;
        }
        catch (Exception) { eight = Mathf.Infinity; }



        if ((grid[tempGridX][tempGridY] as NodeInfo).accessible && zero < closestNeighborDistance)
        {
            closestNeighbor = 0;
            closestNeighborDistance = zero;
        }
        if (ok1 && (grid[tempGridX - 1][tempGridY - 1] as NodeInfo).accessible && one < closestNeighborDistance)
        {
            closestNeighbor = 1;
            closestNeighborDistance = one;
        }
        if (ok2 && (grid[tempGridX + 1][tempGridY + 1] as NodeInfo).accessible && two < closestNeighborDistance)
        {
            closestNeighbor = 2;
            closestNeighborDistance = two;
        }
        if (ok3 && (grid[tempGridX - 1][tempGridY + 1] as NodeInfo).accessible && three < closestNeighborDistance)
        {
            closestNeighbor = 3;
            closestNeighborDistance = three;
        }
        if (ok4 && (grid[tempGridX + 1][tempGridY - 1] as NodeInfo).accessible && four < closestNeighborDistance)
        {
            closestNeighbor = 4;
            closestNeighborDistance = four;
        }
        if (ok5 && (grid[tempGridX - 1][tempGridY] as NodeInfo).accessible && five < closestNeighborDistance)
        {
            closestNeighbor = 5;
            closestNeighborDistance = five;
        }
        if (ok6 && (grid[tempGridX][tempGridY - 1] as NodeInfo).accessible && six < closestNeighborDistance)
        {
            closestNeighbor = 6;
            closestNeighborDistance = six;
        }
        if (ok7 && (grid[tempGridX + 1][tempGridY] as NodeInfo).accessible && seven < closestNeighborDistance)
        {
            closestNeighbor = 7;
            closestNeighborDistance = seven;
        }
        if (ok8 && (grid[tempGridX][tempGridY + 1] as NodeInfo).accessible && eight < closestNeighborDistance)
        {
            closestNeighbor = 8;
            closestNeighborDistance = eight;
        }

        if (closestNeighbor == 0)
        {   /*
		waypoint = GameObject("aa"+0) ;
		waypoint.transform.position.z = grid[tempGridX][tempGridY].posY ;
		waypoint.transform.position.x = grid[tempGridX][tempGridY].posX;
		*/
            return grid[tempGridX][tempGridY];
        }

        else if (closestNeighbor == 1)
        {   /*
		waypoint = GameObject("aa"+1) ;
		waypoint.transform.position.z = grid[tempGridX-1][tempGridY-1].posY ;
		waypoint.transform.position.x = grid[tempGridX-1][tempGridY-1].posX;
		*/
            return grid[tempGridX - 1][tempGridY - 1];
        }
        else if (closestNeighbor == 2)
        {   /*
		waypoint = GameObject("aa"+2) ;
		waypoint.transform.position.z = grid[tempGridX+1][tempGridY+1].posY ;
		waypoint.transform.position.x = grid[tempGridX+1][tempGridY+1].posX;
		*/
            return grid[tempGridX + 1][tempGridY + 1];
        }
        else if (closestNeighbor == 3)
        {   /*
		waypoint = GameObject("aa"+3) ;
		waypoint.transform.position.z = grid[tempGridX-1][tempGridY+1].posY ;
		waypoint.transform.position.x = grid[tempGridX-1][tempGridY+1].posX;
		*/
            return grid[tempGridX - 1][tempGridY + 1];
        }
        else if (closestNeighbor == 4)
        {   /*
		waypoint = GameObject("aa"+4) ;
		waypoint.transform.position.z = grid[tempGridX+1][tempGridY-1].posY ;
		waypoint.transform.position.x = grid[tempGridX+1][tempGridY-1].posX;
		*/
            return grid[tempGridX + 1][tempGridY - 1];
        }
        else if (closestNeighbor == 5)
        {   /*
		waypoint = GameObject("aa"+5) ;
		waypoint.transform.position.z = grid[tempGridX-1][tempGridY].posY ;
		waypoint.transform.position.x = grid[tempGridX-1][tempGridY].posX;
		*/
            return grid[tempGridX - 1][tempGridY];
        }
        else if (closestNeighbor == 6)
        {   /*
		waypoint = GameObject("aa"+6) ;
		waypoint.transform.position.z = grid[tempGridX][tempGridY-1].posY ;
		waypoint.transform.position.x = grid[tempGridX][tempGridY-1].posX;
		*/
            return grid[tempGridX][tempGridY - 1];
        }
        else if (closestNeighbor == 7)
        {   /*
		waypoint = GameObject("aa"+7) ;
		waypoint.transform.position.z = grid[tempGridX+1][tempGridY].posY ;
		waypoint.transform.position.x = grid[tempGridX+1][tempGridY].posX;
		*/
            return grid[tempGridX + 1][tempGridY];
        }
        else if (closestNeighbor == 8)
        {   /*
		waypoint = GameObject("aa"+8) ;
		waypoint.transform.position.z = grid[tempGridX][tempGridY+1].posY ;
		waypoint.transform.position.x = grid[tempGridX][tempGridY+1].posX;
		*/
            return grid[tempGridX][tempGridY + 1];
        }

        return null;
    }

    private NodeInfo[] openList;
    private NodeInfo[] closedList;
    private NodeInfo[] ret;

    private NodeInfo currentNode;

    //----------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------
    // Start the A* search
    int Search(Array grid, NodeInfo start, NodeInfo end)
    {
        // if start and end position are the same return the start position
        if (start.posX == end.posX && start.posY == end.posY)
        {
            //ret.push(grid[start.gridPosX][start.gridPosY]);
            Debug.Log("OK my man ;-) " + " X " + start.gridPosX + " Y " + start.gridPosY + "  end X " + end.gridPosX + "  Y " + end.gridPosY);
            return 1;
        }

        //openList.clear();
        //closedList.clear();
        N = 0;

        //Min Heap has his first element NULL
        //openList.push(null);

        /*the function "push" inserts the element at the end of the list
         *and "insert" places it at the right position in the mmin heap.
         */
        //openList.push(start);
        insert(start);

        while (openList.Length > 1)
        {
            //Take the node with the lowest cost and delete it from the openlist
            currentNode = deleteMin();

            //End case -- result has been found, return the traced path
            if (currentNode.posX == end.posX && currentNode.posY == end.posY)
            {
                NodeInfo curr = currentNode;
                //ret.clear();

                int counter = 0;

                while (true)
                {
                    //ret.push(grid[curr.gridPosX][curr.gridPosY]);
                    //curr = grid[curr.parentGridPosX][curr.parentGridPosY];
                    counter++;

                    if (counter == 400)
                    {
                        Debug.Log("error X " + start.gridPosX + " Y " + start.gridPosY + "  end X " + end.gridPosX + "  Y " + end.gridPosY);
                        break;
                    }
                    else if (curr.gridPosX == start.gridPosX && curr.gridPosY == start.gridPosY)
                    {
                        //Debug.Log("path complete");
                        break;
                    }

                }

                //ret.reverse();

                return 1;
            }
            //Normal case -- move currentNode from open to closed, process each of its neighbors

            //closedList.push(currentNode);
            /*
            neighbors = Neighbors(grid, currentNode);

            for (int i = 0; i < neighbors.Length; i++)
            {
                NodeInfo neighbor = neighbors[i];
                if (FindGraphNode1(closedList, neighbor.id))
                {
                    //not a valid node to process, skip to next neighbor
                    continue;
                }

                // g score is the shortest distance from start to current node, we need to check if
                //	the path we arrived at this neighbor is the shortest one we have seen yet

                //currentNode.g : the G cost since the current node
                //neighborGscore[i]: the G cost from the current node to the neighbor (The distance between the two nodes)
                float gScore = currentNode.g + neighborGscore[i];

                if (!FindGraphNode2(openList, neighbor.id))
                {
                    //This the first time we have arrived at this node, it must be the best
                    //Also, we need to take the h (heuristic) score since we haven't done so yet

                    neighbor.h = Heuristic(neighbor.posX, neighbor.posY, end.posX, end.posY);

                    //Debug.Log(" heyrestic for node : "+(neighbor.gridPosX+1)+"  "+(neighbor.gridPosY+1)+"    "+neighbor.h);

                    neighbor.parentID = currentNode.id;

                    neighbor.parentGridPosX = currentNode.gridPosX;
                    neighbor.parentGridPosY = currentNode.gridPosY;

                    neighbor.g = gScore;
                    neighbor.f = neighbor.g + neighbor.h;
                    //neighbor.debug = "F: "+neighbor.f+"<br />G: "+neighbor.g+"<br />H: "+neighbor.h;
                    //Debug.Log("  for node : "+(neighbor.gridPosX+1)+"  "+(neighbor.gridPosY+1)+"    "+neighbor.g+"   f :"+neighbor.f);

                    openList.push(neighbor);
                    insert(neighbor);

                }
                else if (gScore < neighbor.g)
                {
                    //We have already seen the node, but last time it had a worse g (distance from start)

                    neighbor.h = Heuristic(neighbor.posX, neighbor.posY, end.posX, end.posY);

                    neighbor.parentID = currentNode.id;

                    neighbor.parentGridPosX = currentNode.gridPosX;
                    neighbor.parentGridPosY = currentNode.gridPosY;

                    neighbor.g = gScore;
                    neighbor.f = neighbor.g + neighbor.h;
                    //neighbor.debug = "F: "+neighbor.f+"<br />G: "+neighbor.g+"<br />H: "+neighbor.h;

                    swim(neighbor.priorityQueueID);

                    //Debug.Log("common");
                }
            }
            */
        }
        //No result was found -- empty array signifies failure to find path	
        return -1;
    }

    /*
    bool FindGraphNode1(Array list, int node)
    {
        for (int i = 0; i < list.Length; i++)
        {

            if ((list[i] as NodeInfo).id == node)
            {
                return true;
            }
        }

        return false;
    }

    bool FindGraphNode2(Array list, int node)
    {
        for (int i = 1; i < list.Length; i++)
        {
            if ((list[i] as NodeInfo).id == node)
            {
                return true;
            }
        }

        return false;
    }
    */
    float Heuristic(float neighborPosX, float neighborPosY, float enDX, float enDY)
    {
        //This the Manhattan distance
        /*
        FIXME_VAR_TYPE d1= Mathf.Abs(enDX - neighborPosX);
        FIXME_VAR_TYPE d2= Mathf.Abs(enDY - neighborPosY);

        return d1 + d2;
        */

        //Euclideian distance
        float d1 = Mathf.Pow(enDX - neighborPosX, 2);
        float d2 = Mathf.Pow(enDY - neighborPosY, 2);

        return Mathf.Sqrt(d1 + d2);
    }

    private NodeInfo[] neighborGscore;
    private NodeInfo[] ret2;

    // Return the accessible neighbors of "node"
    Array Neighbors(NodeInfo[][] grid, NodeInfo node)
    {
        ret2 = null;
        neighborGscore = null;

        int x = (int)node.gridPosX;
        int y = (int)node.gridPosY;

        EdgeInfo tempInfo = null;//(neighbors_Gscore.Get(node.id) as EdgeInfo);

        if (x - 1 >= 0 && grid[x - 1] != null && grid[x - 1][y] != null && grid[x - 1][y].accessible)
        {
            //ret2.push(grid[x - 1][y]);
            //neighborGscore.push(tempInfo.up);

        }

        try
        {

            if (grid[x + 1] != null && grid[x + 1][y] != null && grid[x + 1][y].accessible)
            {

                //ret2.push(grid[x + 1][y]);
                //neighborGscore.push(tempInfo.down);

            }

        }
        catch (Exception)
        {
        }

        if (y - 1 >= 0 && grid[x][y - 1] != null && grid[x][y - 1] != null && grid[x][y - 1].accessible)
        {

            //ret2.push(grid[x][y - 1]);
            //neighborGscore.push(tempInfo.left);

        }

        try
        {

            if (grid[x][y + 1] != null && grid[x][y + 1].accessible)
            {

                //ret2.push(grid[x][y + 1]);
                //neighborGscore.push(tempInfo.right);

            }

        }
        catch (Exception)
        {
        }


        if (y - 1 >= 0 && x - 1 >= 0 && grid[x - 1][y - 1] != null && grid[x - 1][y - 1].accessible)
        {
            //ret2.push(grid[x - 1][y - 1]);
            //neighborGscore.push(tempInfo.upleft);

        }
        try
        {

            if (y - 1 >= 0 && grid[x + 1][y - 1] != null && grid[x + 1][y - 1].accessible)
            {

                //ret2.push(grid[x + 1][y - 1]);
                //neighborGscore.push(tempInfo.downleft);

            }

        }
        catch (Exception)
        {
        }
        try
        {

            if (x - 1 >= 0 && grid[x - 1][y + 1] != null && grid[x - 1][y + 1].accessible)
            {

                //ret2.push(grid[x - 1][y + 1]);
                //neighborGscore.push(tempInfo.upright);

            }

        }
        catch (Exception)
        {
        }
        try
        {
            if (grid[x + 1][y + 1] != null && grid[x + 1][y + 1].accessible)
            {
                //ret2.push(grid[x + 1, y + 1]);
                //neighborGscore.push(tempInfo.downright);
            }
        }
        catch (Exception)
        {
        }

        return ret2;
    }

    //----------------------------------------------------------------
    //--- Priority queue , using minheap 

    private int N = 0;
    private NodeInfo minNodef;

    private void insert(NodeInfo newElement)
    {
        N = N + 1;

        newElement.priorityQueueID = N;
        swim(N);
    }

    private NodeInfo deleteMin()
    {
        if (N == 0) throw new Exception("Priority queue underflow");

        exch(1, N);
        sink(1, N - 1);
        //minNodef = openList.pop();
        N = N - 1;

        return minNodef;
    }

    private bool greater(int i, int j)
    {
        return (openList[i].f > openList[j].f);
    }

    private void exch(int i, int j)
    {
        int id1 = (int)openList[i].priorityQueueID;
        openList[i].priorityQueueID = openList[j].priorityQueueID;
        openList[j].priorityQueueID = id1;

        NodeInfo t = openList[i];
        openList[i] = openList[j];
        openList[j] = t;

    }

    private void swim(int k)
    {
        while (k > 1 && greater(k / 2, k))
        {
            exch(k, k / 2);
            k = k / 2;
        }

    }

    private void sink(int k, int N)
    {
        while (2 * k <= N)
        {
            int j = 2 * k;
            if (j < N && greater(j, j + 1)) { j++; }
            if (!greater(k, j)) { break; }
            exch(k, j);
            k = j;
        }
    }

}





