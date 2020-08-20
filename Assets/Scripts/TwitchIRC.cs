using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class TwitchIRC : MonoBehaviour
{
    TcpClient twitchClient;
    StreamReader reader;
    StreamWriter writer;

    [SerializeField]
    string username, password, channelName;

    [SerializeField]
    Transform trsCube;

    float rotSpeed = 0;

    [SerializeField]
    float moveSpeed;
    [SerializeField]
    bool isMoving;

    bool isGameEnded;

    [SerializeField]
    GameObject winText;
    
    void Start()
    {
        Connect();
    }

    void Update()
    {
        if(!Connected)
        {
            Connect();
        }
        //trsCube.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
        ReadChat();

        if(isMoving)
        {
            trsCube.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

   void Connect()
   {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        writer = new StreamWriter(twitchClient.GetStream());
        reader = new StreamReader(twitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
   }

   void ReadChat()
   {
       if(HasMessage && !isGameEnded)
       {
           string message = reader.ReadLine();
           if(message.Contains("PRIVMSG"))
           {
                int splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1).ToLower();

                if(message.Equals("!move"))
                {
                    isMoving = true;
                }
                if(message.Equals("!stop"))
                {
                    isMoving = false;
                }

                if(message.Equals("!front"))
                {
                    trsCube.rotation = Quaternion.LookRotation(Vector3.forward);
                }
                if(message.Equals("!left"))
                {
                    trsCube.rotation = Quaternion.LookRotation(Vector3.left);
                }
                if(message.Equals("!right"))
                {
                    trsCube.rotation = Quaternion.LookRotation(Vector3.right);
                }
           }
       }
   }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("end"))
        {
            isMoving = false;
            isGameEnded = true;
            winText.SetActive(true);
        }
    }

   bool Connected => twitchClient.Connected;

   bool HasMessage => twitchClient.Available > 0;
}
