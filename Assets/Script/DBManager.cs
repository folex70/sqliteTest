using UnityEngine;
using System.Collections;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine.UI;

public class DBManager : MonoBehaviour {

	private string connectionString;

	private List<Hiscore> hiScoreLista = new List<Hiscore>();

	public GameObject scorePrefab;
	public Transform scoreParent;

	public int topRanks; //limitador para quanto quero mostrar

	public int saveScores;

	public InputField enterName;

	public GameObject nameDialog;

	// Use this for initialization
	void Start () {
		//connectionString = "URI=file:" + Application.dataPath + "/db/quizGame.sqlite";
		connectionString = "URI=file:" + Application.dataPath + "/quizGame.sqlite";
		CreateTable ();
		//GetDados ();
		//InsertDados ("ccc", 1000000);
		//InsertDados ("aaa", 10000);
		//InsertDados ("fffc", 10000);
		//InsertDados ("ceee", 1000);
		//InsertDados ("ctrwet", 100);
		//1InsertDados ("werwer", 10);
		//InsertDados ("ccwerwerrr", 333333);
		//DeleteDados(4);
		DeleteExtraScore();
		ShowScores();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			nameDialog.SetActive (!nameDialog.activeSelf);
		}
	}

	private void GetDados()
	{
		hiScoreLista.Clear ();
		using (IDbConnection dbConnection = new SqliteConnection (connectionString)) 
		{
			dbConnection.Open ();

			using (IDbCommand dbCmd = dbConnection.CreateCommand()) 
			{
				string sqlQuery = "SELECT * FROM highscores";

				dbCmd.CommandText = sqlQuery;
				using (IDataReader reader = dbCmd.ExecuteReader())
				{
					while (reader.Read ()) 
					{
						//Debug.Log (reader.GetString (1) +" - "+reader.GetInt32(2));
						hiScoreLista.Add(new Hiscore(reader.GetInt32(0),reader.GetString(1),reader.GetInt32(2),reader.GetDateTime(3)));
					}
					dbConnection.Close ();
					reader.Close ();
				}
			}
		}
		hiScoreLista.Sort (); // organiza do maior para o menor
	}

	private void CreateTable(){
		//Debug.Log ("entrou na createtable");
		using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
			dbConnection.Open ();

			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = String.Format ("CREATE TABLE if not exists highScores (PlayerId INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL  UNIQUE , Name TEXT, Score INTEGER, Date DATETIME DEFAULT CURRENT_DATE)");
				dbCmd.CommandText = sqlQuery;
				dbCmd.ExecuteScalar();
				dbConnection.Close();
			}
		}
	}

	private void InsertDados(string name, int pontuacao){
		int hsCount = hiScoreLista.Count;
		GetDados ();
		//esse if deleta um score menos antes de inserir um novo 
		if (hiScoreLista.Count > 0) {
			Hiscore lowestScore = hiScoreLista [hiScoreLista.Count - 1]; // menor valor da lista apos .sort()
			if (lowestScore != null && saveScores > 0 && hiScoreLista.Count >= saveScores && pontuacao > lowestScore.Score) {
				DeleteDados (lowestScore.IdScore);
				hsCount--;
			}
		}

		if (hsCount < saveScores) {
			using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
				dbConnection.Open ();

				using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
					string sqlQuery = String.Format ("insert into highscores(name,score) values (\"{0}\",\"{1}\");", name, pontuacao);
					dbCmd.CommandText = sqlQuery;
					dbCmd.ExecuteScalar();
					dbConnection.Close();
				}
			}
		}
	}

	private void DeleteDados(int id){
		using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
			dbConnection.Open ();

			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = String.Format ("delete from highscores where playerid = (\"{0}\");", id);
				dbCmd.CommandText = sqlQuery;
				dbCmd.ExecuteScalar();
				dbConnection.Close();
			}
		}
	}

	private void ShowScores()
	{
		GetDados ();
		//limpar caso tenha outro score na tela para não ter repetição
		foreach(GameObject score in GameObject.FindGameObjectsWithTag("score")){
			Destroy(score);
		}

		hiScoreLista.Sort ();
		//for (int i = 0; i < hiScoreLista.Count; i++) {
		for (int i = 0; i < topRanks; i++) {
			if (i <= hiScoreLista.Count - 1) {
				
				GameObject tmpObject = Instantiate (scorePrefab);

				Hiscore tmpScore = hiScoreLista [i];

				tmpObject.GetComponent<HighScoreScript> ().SetScore (tmpScore.Name, tmpScore.Score.ToString(), "#" + (i + 1).ToString ());

				tmpObject.transform.SetParent (scoreParent);		

				tmpObject.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1);			
			}						
		}
	}

	public void DeleteExtraScore(){
		 
		GetDados ();

		//Debug.Log (hiScoreLista.Count);

		if (saveScores <= hiScoreLista.Count)
		{
			int deleteCount = hiScoreLista.Count - saveScores;
			hiScoreLista.Reverse ();
			using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
				dbConnection.Open ();

				using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
					for (int i = 0; i < deleteCount; i++) {

						string sqlQuery = String.Format ("delete from highscores where playerid = (\"{0}\");", hiScoreLista[i].IdScore);
						Debug.Log (sqlQuery);
						dbCmd.CommandText = sqlQuery;
						dbCmd.ExecuteScalar();

					}
					dbConnection.Close();
				}
			}
		}
	}

	public void EnterName(){
		if (enterName.text != string.Empty) {
			int score = UnityEngine.Random.Range (1, 500);
			InsertDados (enterName.text, score);
			enterName.text = string.Empty;

			ShowScores ();
		}
	}

}
