﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PlayerEventHandler
{
    public struct DataLog
    {
        public List<Tuple<string, ActionData>> actions;
        public List<DiagramLog> diagrams;
    };
    public static PlayerEventHandler instance;
    public static string playerName;
    public static string playerCaseId;
    public DataLog dataLog;

	public PlayerEventHandler()
	{
        dataLog.actions = new List<Tuple<string, ActionData>>();
        dataLog.diagrams = new List<DiagramLog>();
    }

    public static void SetPlayer(string nameValue)
	{
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int cur_time = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

        playerName = nameValue;
        playerCaseId = nameValue + "-" + cur_time.ToString();
    }

    //Generate Log file
    public static void GenerateLog()
    {
        instance.LogData();
        foreach (KeyValuePair<string, ERData> pair in DiagramKeeper.diagrams)
        {
            AddDiagramLog(pair.Value);
        }
        string path = Path.Combine(Path.Combine(Application.persistentDataPath, "Logs"), "LogFile-" + playerCaseId + ".json");
        using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string jsonString = JsonConvert.SerializeObject(instance.dataLog, Formatting.Indented);
                writer.Write(jsonString);
            }
        }
    }

    //Starting Log
    public void LogData()
	{
        ActionData newAction = new ActionData();
        string date_time = DateTime.UtcNow.ToString("dd/MM/yyyy H:mm:ss (zzz)");
        newAction.Add("CaseID", playerCaseId);
        newAction.Add("DateTime", date_time);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("LogGeneralData", newAction));
	}

	//Scenes
	static public void RoomMovement(string origin, string destination){
        ActionData newAction = new ActionData();
        newAction.Add("Origin", origin);
        newAction.Add("Destination", destination);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("RoomMovement", newAction));
	}

    static public void OpenDiagram(string room, string diagramCode){
        ActionData newAction = new ActionData();
        newAction.Add("Room", room);
        newAction.Add("DiagramCode", diagramCode);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenDiagram", newAction));
    }
    //Diagram Data
    static public void AddDiagramLog(ERData diagram)
	{
        instance.dataLog.diagrams.Add(new DiagramLog(diagram));
	}
    //Menu and UI
    static public void OpenMenu()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenMenu", newAction));
    }
    static public void CloseMenu()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("CloseMenu", newAction));
    }
    static public void OpenPhone()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("OpenPhone", newAction));
    }
    static public void ClosePhone()
    {
        ActionData newAction = new ActionData();
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ClosePhone", newAction));
    }
    static public void SeeMessages(string sender)
    {
        ActionData newAction = new ActionData();
        newAction.Add("Sender", sender);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("SeeMessages", newAction));
    }
    static public void ChangeMessageState(MessageData message, string oldState)
    {
        ActionData newAction = new ActionData();
        newAction.Add("Sender", message.entityCode);
        newAction.Add("Message", message.message);
        newAction.Add("oldState", oldState);
        newAction.Add("newState", message.state.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeMessageState", newAction));
    }
    //Diagram Event
    static public void SaveDiagram(ERData node)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("SaveDiagram", newAction));
    }
    static public void DeleteDiagram(ERData node)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("DeleteDiagram", newAction));
    }
    static public void AddNode(NodeData node){
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Name", node.nodeName);
        newAction.Add("Type", node.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("AddNode", newAction));
    }
    static public void RemNode(NodeData node)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Name", node.nodeName);
        newAction.Add("Id", node.id.ToString());
        newAction.Add("Type", node.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("RemoveNode", newAction));
    }
    static public void ChangeName(NodeData node, string oldName)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Id", node.id.ToString());
        newAction.Add("From", oldName);
        newAction.Add("To", node.nodeName);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeName", newAction));
    }
    static public void ChangeGenType(NodeData node, string oldState)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Id", node.id.ToString());
        newAction.Add("From", oldState);
        newAction.Add("To", node.nodeName);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeGenType", newAction));
    }
    static public void ChangeKeyAttribute(NodeData node, string oldState)
    {
        ActionData newAction = new ActionData();
        string keyValue = "";
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Name", node.nodeName);
        newAction.Add("Id", node.id.ToString());
        newAction.Add("Type", node.type.ToString());
        keyValue = node.isKey ? "Es" : "No es";
        newAction.Add("Estado", keyValue + " atributo clave");
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeKeyAttribute", newAction));
    }
    static public void AddLink(LinkData link){
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Node1-Id", link.linkedNodeId[0].ToString());
        newAction.Add("Node2-Id", link.linkedNodeId[1].ToString());
        newAction.Add("Type", link.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("AddLink", newAction));
    }
    static public void RemLink(LinkData link)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Node1-Id", link.linkedNodeId[0].ToString());
        newAction.Add("Node2-Id", link.linkedNodeId[1].ToString());
        newAction.Add("Type", link.type.ToString());
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("RemoveLink", newAction));
    }
    static public void ChangeCardinality(LinkData link)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Name", link.name);
        newAction.Add("New Cardinality", link.nodeState);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeCardinality", newAction));
    }
    static public void ChangeParticipation(LinkData link)
    {
        ActionData newAction = new ActionData();
        newAction.Add("DiagramCode", DiagramKeeper.GetCurrDiagramCode());
        newAction.Add("Name", link.name);
        string participation = link.participationIsTotal ? "Total" : "Parcial";
        newAction.Add("New Participation", link.nodeState);
        instance.dataLog.actions.Add(new Tuple<string, ActionData>("ChangeParticipation", newAction));
    }
}
