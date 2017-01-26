using UnityEngine;
using System.Collections;

public class Instatiation : MonoBehaviour {
import System;
import System.Xml; 
import System.Xml.XPath; 
import System.IO; 
import System.Xml.Serialization;

 Transform customPrefab;

void  Start (){
	Instantiate(customPrefab,Vector3(504.5801f , 8 , -95.47156f), Quaternion.identity);
}

}