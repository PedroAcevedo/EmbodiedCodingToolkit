using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using OpenAI;
using Newtonsoft.Json;

public class GPTProfessor : MonoBehaviour
{
    public ChatGPTManager chatGPTManager; // Reference to the existing ChatGPTManager
    public SceneData currentScene;

    [System.Serializable]
    public class PositionVector
    {
        public float[] position;
        public float displayedScalarValue;
        public float[] actualVectorValue;
        public float[] delta;
        public string label;
    }

    public class ChargePosition
    {
        public int chargeId;
        public float chargeValue;
        public float[] position;
    }

    [System.Serializable]
    public class MovedCharge
    {
        public int chargeId;
        public float[] from;
        public float[] to;
        public float chargeValue;
    }

    [System.Serializable]
    public class SceneData
    {
        public string lecturePart;
        public string task;
        public bool linesShowed;
    }

    [System.Serializable]
    public class StudentAction
    {
        public MovedCharge movedCharge;
    }

    [System.Serializable]
    public class FieldResult
    {
        public List<PositionVector> fieldAtPoints;
        public List<ChargePosition> AllChargesPosition;
    }

    [System.Serializable]
    public class UserData
    {
        public SceneData scene;
        public StudentAction studentAction;
        public FieldResult fieldResult;
    }

    public void SendPrompt(MovedCharge movedCharge, List<PositionVector> fieldVectors, List<ChargePosition> AllCharges)
    {
        UserData userData = new UserData
        {
            scene = currentScene,
            studentAction = new StudentAction { movedCharge = movedCharge },
            fieldResult = new FieldResult { fieldAtPoints = fieldVectors , AllChargesPosition = AllCharges }
        };

        // Combine the system message and serialized user data into a single prompt
        string fullPrompt = "Scene and Action:\n" + JsonConvert.SerializeObject(userData, Formatting.Indented);
        Debug.Log("Sending to ChatGPT: " + fullPrompt);
        // Send it using ChatGPTManager
        chatGPTManager.AskChatGPT(fullPrompt);
    }

    public void SendPromptForGuide()
    {
        // Combine the system message and serialized user data into a single prompt
        string fullPrompt = "Scene and Task:\n" + JsonConvert.SerializeObject(currentScene, Formatting.Indented);
        Debug.Log("Sending to ChatGPT: " + fullPrompt);
        // Send it using ChatGPTManager
        chatGPTManager.AskChatGPT(fullPrompt);
    }
}