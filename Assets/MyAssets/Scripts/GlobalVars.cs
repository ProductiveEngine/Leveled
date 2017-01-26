using UnityEngine;

public class GlobalVars : MonoBehaviour
{
    public int test = 5;
    public static int testSta = 10;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        test = 6;
        testSta = 11;
    }


    //public class Singleton
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

    /*
ncar1 = new carInfo();
ncar1.manufacturer = "Bea";
ncar1.name = "Sting"; 
ncar1.productionYear = 2010;
ncar1.bhp = 250;
ncar1.valueBuy = 20000.0;
ncar1.valueSell = ncar1.valueBuy/2;
ncar1.listType = 1;
ncar1.listNumber = 1;
*/
}
