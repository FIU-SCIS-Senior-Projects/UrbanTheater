using UnityEngine;
using System.Collections;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using System;

public class fileUpload : MonoBehaviour {
	MongoCollection collection;
	MongoDatabase db;
	String username;
	string fileName;
	int game_over = 0;

	// Use this for initialization
	void Start () {
		username = "otkac001";
		connect ();	
	}

	class Document
	{
		public ObjectId id;
		public System.DateTime dt;
		public Byte[] bf;
	}

	void onGUI(){
		if (game_over == 1)
			GUI.Box (new Rect (0, 0, Screen.width / 3, Screen.height / 3), "Your Decisions were sucessfully uploaded to the database!");
		if (game_over == 2)
			GUI.Box (new Rect (0, 0, Screen.width / 3, Screen.height / 3), "Your Decisions could not be uploaded.\nPlease check your connection...");
	}

	void OnApplicationQuit()
	{
		Document dc = new Document();
		dc.id = ObjectId.GenerateNewId();
		dc.dt = System.DateTime.Now;
		//send the file to mongoDB
		try{
			dc.bf = File.ReadAllBytes(fileName);
			collection.Insert(dc);
			game_over = 1;		
			}
		catch{
			game_over = 2;
		}

	}

	//connect to Mongodb decisions within your username collection
	public void connect()
	{
		try{
		MongoClient mc = new MongoClient (new MongoUrl ("mongodb://player:rwdocattach@urbantheater-dev.cis.fiu.edu"));
		MongoServer server = mc.GetServer();
		server.Connect ();
		
		//Console.Error.WriteLine ("No server connection");
		db = server.GetDatabase ("decisions");
		collection = db.GetCollection (username);
		fileName = Application.persistentDataPath + "/local_decisions.txt";
		}
		catch{
			GUI.Box(new Rect(0,0,Screen.width/3,Screen.height/3), "Your Decisions could not be uploaded. Please check your connection or authentication...");}

	}
}
