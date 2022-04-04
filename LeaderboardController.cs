using UnityEngine;
using System.Collections;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    public TMP_InputField user;
    public GameObject leaders;
    public int score;

    const string privateCode = "7W_A9HKXgUufW0j_OwasbgA92T8ltmzEy0ofxepTo3Bw";
	const string publicCode = "62494e988f40bc123c4c1d80";
	const string webURL = "http://dreamlo.com/lb/";

	public Highscore[] highscoresList;

    public void Awake(){
        DownloadHighscores();
    }

    public void SubmitScore(){
        AddNewHighscore(user.text,score);
    }


	public void AddNewHighscore(string username, int score) {
		StartCoroutine(UploadNewHighscore(username,score));
	}

	IEnumerator UploadNewHighscore(string username, int score) {
		WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
		yield return www;

		if (string.IsNullOrEmpty(www.error)){
            FindObjectOfType<GameManager>().SentScore();
            DownloadHighscores();
        }else {
            FindObjectOfType<GameManager>().FailedToSend();
		}
	}

	public void DownloadHighscores() {
		StartCoroutine("DownloadHighscoresFromDatabase");
	}

	IEnumerator DownloadHighscoresFromDatabase() {
		WWW www = new WWW(webURL + publicCode + "/pipe/");
		yield return www;
		
		if (string.IsNullOrEmpty(www.error)){
			FormatHighscores(www.text);
        }else {
            Debug.Log("Error Downloading");
		}
	}

	void FormatHighscores(string textStream) {
		string[] entries = textStream.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
		highscoresList = new Highscore[entries.Length];

		for (int i = 0; i <entries.Length; i ++) {
			string[] entryInfo = entries[i].Split(new char[] {'|'});
			string username = entryInfo[0];
			int score = int.Parse(entryInfo[1]);
			highscoresList[i] = new Highscore(username,score);
		}
        Transform[] leaderTransforms = leaders.GetComponentsInChildren<Transform>();
        for(int i = 0; i<7;i++){
            if(i==highscoresList.Length)return;
            TextMeshProUGUI[] leaderScore = leaderTransforms[(i*4+1)].GetComponentsInChildren<TextMeshProUGUI>();
            leaderScore[0].text = highscoresList[i].username;
            leaderScore[1].text = ""+highscoresList[i].score/1000000;
            leaderScore[2].text = ""+highscoresList[i].score%1000000;
        }
	}

}

public struct Highscore {
	public string username;
	public int score;

	public Highscore(string _username, int _score) {
		username = _username;
		score = _score;
	}
}
