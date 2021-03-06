﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public GameObject chatContainer;
    public GameObject messagePrefab;

    private bool socketReady = false;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public void ConnectToSever()
    {
        // If already connected, ifnore this function
        if (socketReady)
        { return; }

        // Default host / port values
        string host = "127.0.0.1";
        int port = 6321;

        // Overwrite deafult host / port values, if there is something in those boxes
        string h;
        int p;
        h = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (h != "")
        { host = h; }
        int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out p);
        if (p != 0)
        { port = p; }

        // Create the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error :" + e.Message);
        }
    }

    private void Update()
    {
        if (socketReady)
        {
            if(stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if(data != null)
                { OnIncomingData(data); }
            }
        }
    }

    private void OnIncomingData(string data)
    {
        GameObject go = Instantiate(messagePrefab, chatContainer.transform) as GameObject;
        go.GetComponentInChildren<Text>().text = data;
    }

    private void Send(string data)
    {
        if (!socketReady)
        { return; }

        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSendButton()
    {
        string message = GameObject.Find("SendInput").GetComponent<InputField>().text;
        Send(message);
    }
}
