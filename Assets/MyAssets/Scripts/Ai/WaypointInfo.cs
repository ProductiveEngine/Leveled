using UnityEngine;
using System.Collections;

public class WaypointInfo : MonoBehaviour {

float posX;
float posY;

void  Awake (){
    posX = transform.position.x;
    posY = transform.position.y;
}

}
