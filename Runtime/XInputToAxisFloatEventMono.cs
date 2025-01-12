using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
// Required in C#
using XInputDotNetPure;

public class XInputToAxisFloatEventMono : MonoBehaviour
{
    public PlayerObserved[] m_players = new PlayerObserved[] {
        new PlayerObserved(PlayerIndex.One),
        new PlayerObserved(PlayerIndex.Two),
        new PlayerObserved(PlayerIndex.Three),
        new PlayerObserved(PlayerIndex.Four)
    };
    [System.Serializable]
    public class PlayerObserved
    {
        public PlayerIndex m_playerIndex = PlayerIndex.One;
        public GamePadState m_state = new GamePadState();

        public PlayerObserved(PlayerIndex playerIndex)
        {
            m_playerIndex = playerIndex;
        }

    }
    public List<ObservedAxis> m_axisObserved=new List<ObservedAxis>();
    [System.Serializable]
    public class ObservedAxis
    {
        public string m_arrayLabel = "";
        public PlayerIndex m_player;
        public XInputFloatableValue m_axis;
        public FloatEvent m_onFloatPushed;
        public float m_lastValuePushed;
        
        public ObservedAxis(PlayerIndex playerInex, XInputFloatableValue axisName)
        
        {
            this.m_arrayLabel = playerInex.ToString() + " " + axisName.ToString();
            this.m_player = playerInex;
            this.m_axis = axisName;
        }
       

        [System.Serializable]
        public class FloatEvent :UnityEvent<float>{ }
    }

    public bool m_useUnityRefreshHardware = true;
    public float m_timeBetweenRefreshInMs = 30f;


    [ContextMenu("Set to one player observed")]
    public void SetPlayerObservedToOne()
    {
        m_axisObserved.Clear();
        AddPlayer(PlayerIndex.One);
    }
    [ContextMenu("Set to two player observed")]
    public void SetPlayerObservedToTwo()
    {
        m_axisObserved.Clear();
        AddPlayer(PlayerIndex.One);
        AddPlayer(PlayerIndex.Two);
    }
    [ContextMenu("Set to four player observed")]
    public void SetPlayerObservedToFour()
    {
        m_axisObserved.Clear();
        AddPlayer(PlayerIndex.One);
        AddPlayer(PlayerIndex.Two);
        AddPlayer(PlayerIndex.Three);
        AddPlayer(PlayerIndex.Four);
    }

    private void AddPlayer(PlayerIndex index)
    {
        m_axisObserved.Add(new ObservedAxis (index,  XInputFloatableValue.TriggerLeft ));
        m_axisObserved.Add(new ObservedAxis (index,  XInputFloatableValue.TriggerRight));
        m_axisObserved.Add(new ObservedAxis (index,  XInputFloatableValue.JoystickLeftEast));
        m_axisObserved.Add(new ObservedAxis (index,  XInputFloatableValue.JoystickLeftNorth));
        m_axisObserved.Add(new ObservedAxis (index,  XInputFloatableValue.JoystickRightEast));
        m_axisObserved.Add(new ObservedAxis (index,  XInputFloatableValue.JoystickRightNorth));
    }

    private void Start()
    {
        if (m_useUnityRefreshHardware)
            InvokeRepeating("RefreshHardwareInfo", 0, m_timeBetweenRefreshInMs / 1000f);
    }

    
    public void RefreshHardwareInfo()
    {

        for (int y = 0; y < m_players.Length; y++)
        {
            PlayerObserved player = m_players[y];
            player.m_state = GamePad.GetState(player.m_playerIndex);
     
        }
        foreach (var item in m_axisObserved)
        {
            float value = GetBoolValueOfFloat(ref item.m_player, ref item.m_axis);
            item.m_onFloatPushed.Invoke(value);
            item.m_lastValuePushed = value;
        }

    }

    private float GetBoolValueOfFloat(ref PlayerIndex m_player, ref XInputFloatableValue m_axis)
    {
        foreach (var item in m_players)
        {
            if(item.m_playerIndex== m_player)
                return GetBoolValueOfFloat(ref item.m_state, ref m_axis);
        }
        return 0;
    }

    private float GetBoolValueOfFloat(ref GamePadState player, ref XInputFloatableValue value)
    {
        switch (value)
        {
            case XInputFloatableValue.TriggerLeft: return player.Triggers.Left;
            case XInputFloatableValue.TriggerRight: return player.Triggers.Right;
            case XInputFloatableValue.JoystickLeftEast: return player.ThumbSticks.Left.X;
            case XInputFloatableValue.JoystickLeftWest: return -player.ThumbSticks.Left.X;
            case XInputFloatableValue.JoystickLeftNorth: return player.ThumbSticks.Left.Y;
            case XInputFloatableValue.JoystickLeftSouth: return -player.ThumbSticks.Left.Y;
            case XInputFloatableValue.JoystickRightEast: return player.ThumbSticks.Right.X;
            case XInputFloatableValue.JoystickRightWest: return -player.ThumbSticks.Right.X;
            case XInputFloatableValue.JoystickRightNorth: return player.ThumbSticks.Right.Y;
            case XInputFloatableValue.JoystickRightSouth: return -player.ThumbSticks.Right.Y;
            default:
                break;
        }
        return 0;

    }

}
