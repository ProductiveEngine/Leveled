
var customSkin : GUISkin;

var GUIOverlayTexture : Texture;
var speedometer : Texture;
var enemySkull : Texture;

function Update () {
}

function OnGUI()
{
	GUILayout.BeginArea(Rect(1,Screen.height-(GUIOverlayTexture.height+1),GUIOverlayTexture.width,GUIOverlayTexture.height));
	GUILayout.Label(GUIOverlayTexture);
	GUILayout.EndArea();
	
	GUILayout.BeginArea(Rect(Screen.width-350,Screen.height-350,speedometer.width,speedometer.height));
	GUILayout.Label(speedometer);
	GUILayout.EndArea();
	
	GUILayout.BeginArea(Rect(0,0,enemySkull.width,enemySkull.height));
	GUILayout.Label(enemySkull);
	GUILayout.EndArea();

}