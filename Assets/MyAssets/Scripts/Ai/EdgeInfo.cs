
public class EdgeInfo
{

    float id = 0;

    float up = 45.4f;
    //float upright = Mathf.Sqrt(2) * 45.4f;
    float right = 45.4f;
    //float downright = Mathf.Sqrt(2) * 45.4f;
    float down = 45.4f;
    //float downleft = Mathf.Sqrt(2) * 45.4f;
    float left = 45.4f;
    //float upleft = Mathf.Sqrt(2) * 45.4f;

    public EdgeInfo()
    {

    }
    public EdgeInfo(float z)
    {
        id = z;
    }
}

