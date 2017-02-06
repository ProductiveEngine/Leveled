public class NodeInfo
{

    public float id;
    public float parentID;
    public float priorityQueueID;

    public bool accessible;

    public float posX;
    public float posY;

    public float gridPosX;
    public float gridPosY;

    public float g = 0;
    public float h = 0;
    public float f = 0;

    public float parentGridPosX;
    public float parentGridPosY;

    public NodeInfo()
    {

    }

    public NodeInfo(float i, bool c, float a, float b, float d, float e)
    {
        id = i;
        accessible = c;
        posX = a;
        posY = b;
        gridPosX = d;
        gridPosY = e;
    }

}