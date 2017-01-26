using UnityEngine;

public class CheckCrashedMissiles : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        var checkSpeedMissiles = GameObject.FindGameObjectsWithTag("speedMissileTag");

        foreach (var checkSpeedMissile in checkSpeedMissiles)
        {
            if (checkSpeedMissile.GetComponent("SpeedMissileScript").crashed)
            {
                Destroy(checkSpeedMissile);
            }
        }

        var checkHomingMissiles = GameObject.FindGameObjectsWithTag("homingMissileTag");

        foreach (var checkHomingMissile in checkHomingMissiles)
        {
            if (checkHomingMissile.GetComponent("HomingMissileScript").crashed)
            {
                Destroy(checkHomingMissile);
            }
        }

        var checkPowerMissiles = GameObject.FindGameObjectsWithTag("powerMissileTag");

        foreach (var checkPowerMissile in checkPowerMissiles)
        {
            if (checkPowerMissile.GetComponent("PowerMissileScript").crashed)
            {
                Destroy(checkPowerMissile);
            }
        }
    }
}
