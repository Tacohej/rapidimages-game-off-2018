using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScroller : MonoBehaviour
{
    float TEXT_SPEED = 0.04f;
    Text textComponent;
    
    int stringIndex = 0;
    string currentTextToScroll;
    List<string> textToScrollQueue = new List<string>();

    void Start()
    {
        this.textComponent = gameObject.GetComponent<Text>();
    }

    void ScrollText()
    {
        if (textToScrollQueue.Count > 0)
        {
            this.currentTextToScroll = this.textToScrollQueue[this.textToScrollQueue.Count - 1];
            this.textComponent.text = this.currentTextToScroll.Substring(0, stringIndex);
            stringIndex++;

            // If we have written the entire string sent, remove string from queue
            // Wait a bit to let user read the message
            if (this.textComponent.text == this.currentTextToScroll)
            {
                this.textToScrollQueue.RemoveAt(this.textToScrollQueue.Count - 1);
                stringIndex = 0;
                CancelInvoke("ScrollText");
                InvokeRepeating("ScrollText", 1.2f, TEXT_SPEED);
            }
        }
        // If we have scrolled all messages in the queue, stop invoking 
        else 
        {
            CancelInvoke("ScrollText");
        }
    }

    // Parameters:
    // Force: if true, cancels the current printout and prints new message
    // clearQueue: if force && clearQueue, stops all printing after the new message has been printed
    public void AddScrollText(string inputText, bool force = false, bool clearQueue = false) 
    {
        if(force) {  
            CancelInvoke("ScrollText");

            if (clearQueue)
            {
                this.textToScrollQueue.Clear();
            }
            else 
            {
                this.textToScrollQueue.RemoveAt(this.textToScrollQueue.Count - 1);
            }

            stringIndex = 0;
            this.textToScrollQueue.Add(inputText);
        } 
        else 
        {
            this.textToScrollQueue.Insert(0, inputText);
        }

        if(!IsInvoking("ScrollText")) {
            InvokeRepeating("ScrollText", 0, TEXT_SPEED);
        }
    }

    // Functions used for testing purposes.
    public void TestText(string input = "") {
        string textToPrint;
        if (input == "")
        {
            GameObject inputField = GameObject.Find("InputField");
            textToPrint = inputField.transform.Find("Text").GetComponent<Text>().text;
        } else {
            textToPrint = input;
        }
        
        AddScrollText(textToPrint);
    }

    public void TestForce(string input = "") {
        string textToPrint;
        if (input == "")
        {
            GameObject inputField = GameObject.Find("InputField");
            textToPrint = inputField.transform.Find("Text").GetComponent<Text>().text;
        } else {
            textToPrint = input;
        }
        
        AddScrollText(textToPrint, true);
    }

    public void TestForceClearQueue(string input = "") {
        string textToPrint;
        if (input == "")
        {
            GameObject inputField = GameObject.Find("InputField");
            textToPrint = inputField.transform.Find("Text").GetComponent<Text>().text;
        } else {
            textToPrint = input;
        }
        
        AddScrollText(textToPrint, true, true);
    }
}
