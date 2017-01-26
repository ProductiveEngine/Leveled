using UnityEngine;

public class Db : MonoBehaviour
{

    public int test = 5;
    public static int testSta = 10;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    //private static Singleton instance;

    //private Singleton() { }

    //public static Singleton Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = new Singleton();
    //        }
    //        return instance;
    //    }
    //}


}
