import System;
import System.Xml; 
import System.Xml.XPath; 
import System.IO; 
import System.Xml.Serialization;
	
var extraTypes : Type[] ;
extraTypes = new Type[1];
extraTypes[0] = typeof(carInfo);
	
//var newCarsList : String[] = ["car1","Dodge Challenger 09"];
var nikos : String = " nothing";

class carInfo{

	var manufacturer : String;
	var name : String;
	var productionYear : int;
	var bhp : int;
	var valueBuy : float;
	var valueSell : float;
	
	var listType : int ; // 1->new , 2->used
	var listNumber : int ;
	
    function  carInfo(){}
}



function Start()
{
	
	
	//OneTime();
	
	
}

private function OneTime()
{
	
	var money : float = 60000.0;
	SerializeFloat(Application.dataPath+"/projectdb/money.xml",money);
	
	SerializeString(Application.dataPath+"/projectdb/activeCar.xml","");
	
	var ze :  ArrayList  = new ArrayList();
	var te : carInfo = new carInfo();
	te.name="NOCAR";
	ze.Add(te);
	SerializeArrayList(Application.dataPath+"/projectdb/activeCarInfo.xml",extraTypes,ze);
	//---------NEW CARS---------------------------------------------
	
	var newCarsList : ArrayList;
	newCarsList = new ArrayList();
	
	var ncar1 : carInfo;

	ncar1 = new carInfo();	
	ncar1.manufacturer = "Bea";
	ncar1.name = "Sting"; 
	ncar1.productionYear = 2010;
	ncar1.bhp = 250;
	ncar1.valueBuy = 20000.0;
	ncar1.valueSell = ncar1.valueBuy/2;
	ncar1.listType = 1;
	ncar1.listNumber = 1;
	
	newCarsList.Add(ncar1);
	
	var ncar2 : carInfo;

	ncar2 = new carInfo();	
	ncar2.manufacturer = "Just";
	ncar2.name = "Tux"; 
	ncar2.productionYear = 2010;
	ncar2.bhp = 250;
	ncar2.valueBuy = 20000.0;
	ncar2.valueSell = ncar2.valueBuy/2;
	ncar2.listType = 1;
	ncar2.listNumber = 2;
	
	newCarsList.Add(ncar2);
	
	var ncar3 : carInfo;

	ncar3 = new carInfo();	
	ncar3.manufacturer = "Lou";
	ncar3.name = "Tenant"; 
	ncar3.productionYear = 2010;
	ncar3.bhp = 250;
	ncar3.valueBuy = 20000.0;
	ncar3.valueSell = ncar3.valueBuy/2;
	ncar3.listType = 1;
	ncar3.listNumber = 3;
	
	newCarsList.Add(ncar3);

	var ncar4 : carInfo;

	ncar4 = new carInfo();	
	ncar4.manufacturer = "Flopoloco";
	ncar4.name = "Inferno"; 
	ncar4.productionYear = 2010;
	ncar4.bhp = 300;
	ncar4.valueBuy = 50000.0;
	ncar4.valueSell = ncar4.valueBuy/2;
	ncar4.listType = 1;
	ncar4.listNumber = 4;
	
	newCarsList.Add(ncar4);
	
	var ncar5 : carInfo;

	ncar5 = new carInfo();	
	ncar5.manufacturer = "Flopoloco";
	ncar5.name = "Foloi"; 
	ncar5.productionYear = 2010;
	ncar5.bhp = 300;
	ncar5.valueBuy = 50000.0;
	ncar5.valueSell = ncar5.valueBuy/2;
	ncar5.listType = 1;
	ncar5.listNumber = 5;
	
	newCarsList.Add(ncar5);
	
	var ncar6 : carInfo;

	ncar6 = new carInfo();	
	ncar6.manufacturer = "Flopoloco";
	ncar6.name = "Glacier"; 
	ncar6.productionYear = 2010;
	ncar6.bhp = 300;
	ncar6.valueBuy = 50000.0;
	ncar6.valueSell = ncar6.valueBuy/2;
	ncar6.listType = 1;
	ncar6.listNumber = 6;
	
	newCarsList.Add(ncar6);
	
	SerializeArrayList(Application.dataPath+"/projectdb/newCars.xml",extraTypes,newCarsList);
	//--------------------------------------------------------------------
	//---------USED CARS-------------------------------------------------
	var usedCarsList : ArrayList = new ArrayList();
	
	var ucar1 : carInfo = new carInfo();	
	ucar1.manufacturer = "Mortal";
	ucar1.name = "Kombat"; 
	ucar1.productionYear = 1992;
	ucar1.bhp = 420;
	ucar1.valueBuy = 10000.0;
	ucar1.valueSell = ucar1.valueBuy/2;
	ucar1.listType = 2;
	ucar1.listNumber = 1;
	
	usedCarsList.Add(ucar1);
	
	var ucar2 : carInfo = new carInfo();	
	ucar2.manufacturer = "Street";
	ucar2.name = "Fighter"; 
	ucar2.productionYear = 1987;
	ucar2.bhp = 420;
	ucar2.valueBuy = 10000.0;
	ucar2.valueSell = ucar2.valueBuy/2;
	ucar2.listType = 2;
	ucar2.listNumber = 2;
	
	usedCarsList.Add(ucar2);
	
	var ucar3 : carInfo = new carInfo();	
	ucar3.manufacturer = "Love";
	ucar3.name = "80's"; 
	ucar3.productionYear = 1980;
	ucar3.bhp = 100;
	ucar3.valueBuy = 5000.0;
	ucar3.valueSell = ucar3.valueBuy/2;
	ucar3.listType = 2;
	ucar3.listNumber = 3;
	
	usedCarsList.Add(ucar3);
	
	var ucar4 : carInfo = new carInfo();	
	ucar4.manufacturer = "Love";
	ucar4.name = "90's"; 
	ucar4.productionYear = 1990;
	ucar4.bhp = 100;
	ucar4.valueBuy = 5000.0;
	ucar4.valueSell = ucar4.valueBuy/2;
	ucar4.listType = 2;
	ucar4.listNumber = 4;
	
	usedCarsList.Add(ucar4);
	
	var ucar5 : carInfo = new carInfo();	
	ucar5.manufacturer = "Ades";
	ucar5.name = "RIPer"; 
	ucar5.productionYear = 0;
	ucar5.bhp = 885;
	ucar5.valueBuy = 10000.0;
	ucar5.valueSell = ucar5.valueBuy/2;
	ucar5.listType = 2;
	ucar5.listNumber = 5;
	
	usedCarsList.Add(ucar5);
	
	var ucar6 : carInfo = new carInfo();	
	ucar6.manufacturer = "Marvel";
	ucar6.name = "Punisher"; 
	ucar6.productionYear = 1974;
	ucar6.bhp = 550;
	ucar6.valueBuy = 20000.0;
	ucar6.valueSell = ucar6.valueBuy/2;
	ucar6.listType = 2;
	ucar6.listNumber = 6;
	
	usedCarsList.Add(ucar6);
	
	
	
	
	SerializeArrayList(Application.dataPath+"/projectdb/usedCars.xml",extraTypes,usedCarsList);
	
	
	
}
private function SerializeFloat( filename : String ,data : float )
{
	
	serializer = XmlSerializer(typeof( float ));
	writer = StreamWriter(filename,false);
	serializer.Serialize(writer,data);
	writer.Close();
}

private function SerializeArrayList( filename : String ,extra : Type[],data : ArrayList )
{
	

	serializer = XmlSerializer(typeof( ArrayList ),extra);
	writer = StreamWriter(filename,false);
	serializer.Serialize(writer,data);
	writer.Close();
}

private function DeserializeArrayList( filename : String,extra : Type[]) : Object
{
	
	
	s = XmlSerializer(typeof(ArrayList),extra);
	reader = FileStream(filename,FileMode.Open);
	
	obj  = s.Deserialize(reader);
	
	reader.Close();
	return obj;
	
	//inn = StreamReader("F:/mpla.xml");
	//var fragos : String = s.Deserialize(inn);
	//Debug.Log("Nai!!!!!!!   "+  ( ( list.Get(0) as carInfo).name    ) ) ;
	//Debug.Log("Nai!!!!!!!   "+list[1].name);
	//PrintValues(list);
}
private function SerializeString( filename : String ,data : String )
{
	
	serializer = XmlSerializer(typeof( String ));
	writer = StreamWriter(filename,false);
	serializer.Serialize(writer,data);
	writer.Close();
}


















