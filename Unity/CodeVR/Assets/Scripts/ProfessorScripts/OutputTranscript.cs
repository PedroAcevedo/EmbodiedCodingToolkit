using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class OutputTranscript : MonoBehaviour
{
    public int userNo;
    public List<string> agentResponse;
    public List<string> userResponse;
    public List<float> userResponseTime;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        saveData();
    }
    public void ButtonSaveData() 
    {
        saveData();
        Debug.Log("Transcript saved!");
    }
    private void saveData() 
    {
        //save transcript to file
        string path = Path.Combine(Application.dataPath, "Output/");
        string filePath = Path.Combine(path, userNo.ToString() +"_"+ SceneManager.GetActiveScene().name + ".csv");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Agent,User,Wordcount,ResponseTime");

            int agentN = agentResponse.Count;
            int userN = userResponse.Count;
            int timeN = userResponseTime.Count;

            int i = 0, j = 0, k=0;

            //code assume that length of userResponse is the same as userResponseTime
            while (i < agentN || j < userN)
            {
                if (i < agentN && j < userN)
                {
                    writer.WriteLine(agentResponse[i] + "," + userResponse[j] + "," + GetWordCount(userResponse[j]) + "," + userResponseTime[k].ToString());
                    i++;
                    j++;
                    k++;
                }
                else if (i < agentN && j >= userN)
                {
                    writer.WriteLine(agentResponse[i] + "," + "," + ",");
                    i++;
                }
                else if(i>=agentN && j<userN)
                {
                    if(k<timeN)
                        writer.WriteLine("," + userResponse[j] + "," + GetWordCount(userResponse[j]) + "," + userResponseTime[k].ToString());
                    else
                        writer.WriteLine("," + userResponse[j] + "," + GetWordCount(userResponse[j]) + "," + "0");
                    j++;
                    k++;
                }
                
            }
        }
        
    }
    private int GetWordCount(string response) {
        //string[] split = response.Split(' ');
        string[] words = response.Split(new char[] { ' ', '\t', '\n', '\r' });
        return words.Length;
    }
}
