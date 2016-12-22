using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This script handles Chat.
/// </summary>
/// 

public class Chat : MonoBehaviour {
    public Text ChatText; // Reference to the chat text.
    public InputField ChatInputField; // Reference to the Chat Input Field.
    public float FadeTime = 5.0f; // It is used to fade out the chat.

    private string LastMessage; // It is used to store the last message.

    public int CharacterLimt = 256; // Max character we can send in a message.
    public Image InputFieldImage; // Image to know where the chat is.
    public Text InputFieldTextPlaceholder; // Reference to the Input Field Text Placeholder (We need it to fade it out too)
    int CurrentMessageNumber = 0;

    public int MaxCachedMessage = 25; // Max cached messages.
    List<Hashtable> cachedMessages; // List with all the cached messages. 

    /// <summary> Limit the characters in the input field and init some variables </summary>
    void Awake()
    {
        ChatInputField.characterLimit = CharacterLimt;
        cachedMessages = new List<Hashtable>();
    }

    /// <summary> Clear the chat as soon as the gameobject is enabled and register the "OnEvent" function so we get the photon evets. Then fade out the chat. </summary>
    void OnEnable()
    {
        ChatText.text = "";
        PhotonNetwork.OnEventCall += this.OnEvent;
        Invoke("FadeOut", FadeTime);
    }

    /// <summary>  Unregister the "OnEvent" funciton. </summary>
    void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this.OnEvent;
    }

    /// <summary> Fade out the chat. Deactivate the chat input field. Set the input field text to an empty string "".</summary>
    void FadeOut()
    {
        ChatText.CrossFadeAlpha(.001f, 1.0f, false);
        InputFieldImage.CrossFadeAlpha(.001f, 1.0f, false);
        InputFieldTextPlaceholder.CrossFadeAlpha(.001f, 1.0f, false);
        ChatInputField.text = "";
        ChatInputField.DeactivateInputField();
    }

    /// <summary> Fade in the chat. Cancel fade out invoke just in case we already called it and invoke the fade out function </summary>
    public void FadeIn()
    {
        CancelInvoke("FadeOut");
        ChatText.CrossFadeAlpha(1.0f, 1.0f, false);
        InputFieldImage.CrossFadeAlpha(1.0f, 1.0f, false);
        InputFieldTextPlaceholder.CrossFadeAlpha(.5f, 1.0f, false);
        Invoke("FadeOut", FadeTime);
    }

    /// <summary> Function that gets all the photon events. 
    /// We use a Hashtable to keep track of the message number and the message so we can remove it if the max cached message is reached if so we delete the first in the list </summary>
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 0)
        {
            FadeIn();
            Hashtable message = (Hashtable)content;
            foreach(int key in message.Keys)
            {
                ChatText.text += (string)message[key] + System.Environment.NewLine;
                LastMessage = (string)message[key];
                cachedMessages.Add(message);
                if(cachedMessages.Count > MaxCachedMessage)
                {
                    if(PhotonNetwork.isMasterClient)
                    {
                        RemoveMesssageFromCache(cachedMessages[0]);
                    }
                    cachedMessages.Remove(cachedMessages[0]);
                }
            }

        }
    }

    /// <summary> Removes a message from the room cache </summary>
    void RemoveMesssageFromCache(Hashtable _message)
    {
        byte evCode = 0;
        bool reliable = true;
        RaiseEventOptions ro = new RaiseEventOptions();
        ro.Receivers = ReceiverGroup.All;
        ro.CachingOption = EventCaching.RemoveFromRoomCache;

        PhotonNetwork.RaiseEvent(evCode, _message, reliable, ro);
    }

    /// <summary> Reset the fade out invoke in case we are typing something in the input field </summary>
    public void ResetFade()
    {
        CancelInvoke("FadeOut");
        Invoke("FadeOut", FadeTime);
    }

    /// <summary> Clear all the chached events </summary>
    public void ClearCache()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        byte evCode = 0;
        bool reliable = true;
        RaiseEventOptions ro = new RaiseEventOptions();
        ro.Receivers = ReceiverGroup.All;
        ro.CachingOption = EventCaching.RemoveFromRoomCache;

        PhotonNetwork.RaiseEvent(evCode, null, reliable, ro);
    }

    /// <summary> Sends a message to all the players. Checks if the message is from the input field or it is called from a funciton.
    /// So if the chat input field is empty then function is called from a function. If not then we are trying to send a message from the input field.
    /// We also check if the message is null or equal to the last message if so we just exit the fuction. 
    /// </summary>
    public void AddMessage(string msg = "")
    {
        string content = PhotonNetwork.player.name + ": " + ChatInputField.text;
        Hashtable message = new Hashtable();

        if (msg == "")
        {
            if (string.IsNullOrEmpty(ChatInputField.text) || LastMessage == content)
            {
                ChatInputField.text = "";
                return;
            }
            message.Add(CurrentMessageNumber, content);
        }

        else
        {
            message.Add(CurrentMessageNumber, msg);
        }

        byte evCode = 0;
        bool reliable = true;
        RaiseEventOptions ro = new RaiseEventOptions();
        ro.Receivers = ReceiverGroup.All;

        ro.CachingOption = EventCaching.AddToRoomCache;
        PhotonNetwork.RaiseEvent(evCode, message, reliable, ro);

        ChatInputField.text = "";
        Invoke("FadeOut", FadeTime);
    }

    /// <summary> Shows that a player connected to the room in the chat. </summary>
    void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        if(PhotonNetwork.isMasterClient)
        {
            AddMessage(other.name + " has connected.");
        }
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
    }

    /// <summary> Activate the input field if we click on it or we press the Return/Enter key. </summary>
    void LateUpdate()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (ChatInputField.gameObject.Equals(selected))
            {
                EventSystem.current.SetSelectedGameObject(ChatInputField.gameObject, null);
                FadeIn();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.name == "InputField")
            {
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }
            else
                EventSystem.current.SetSelectedGameObject(ChatInputField.gameObject, null);
                FadeIn();
        }
    }

}
