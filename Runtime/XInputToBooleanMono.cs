using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// Required in C#
using XInputDotNetPure;

public class XInputToBooleanMono : MonoBehaviour
{

    public Namedboolean m_onNamedBooleanChanged;
    [System.Serializable]
    public class Namedboolean : UnityEvent<string, bool> { }

    public PlayerObserved[] m_player = new PlayerObserved[] {
        new PlayerObserved(PlayerIndex.One),
        new PlayerObserved(PlayerIndex.Two),
        new PlayerObserved(PlayerIndex.Three),
        new PlayerObserved(PlayerIndex.Four)
    };
    [System.Serializable]
    public class PlayerObserved {
        public PlayerIndex m_playerIndex = PlayerIndex.One;
        public GamePadState m_state = new GamePadState();
        public GamePadState m_prevState = new GamePadState();
        public List<NamedBoolValue> m_boolObserved = new List<NamedBoolValue>();
        public List<NamedFloatValue> m_floatObserved = new List<NamedFloatValue>();

        public PlayerObserved(PlayerIndex playerIndex)
        {
            m_playerIndex = playerIndex;
        }

        public void Clear()
        {
            m_boolObserved.Clear();
            m_floatObserved.Clear();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < m_player.Length; i++)
        {
            m_player[i].Clear();
        }
    }

    public bool m_useUnityRefreshHardware = true;
    public float m_timeBetweenRefreshInMs = 30f;

    private void Start()
    {
        if (m_useUnityRefreshHardware)
            InvokeRepeating("RefreshHardwareInfo", 0, m_timeBetweenRefreshInMs / 1000f);
    }

    public void RefreshHardwareInfo() {

        

        for (int y = 0; y < m_player.Length; y++)
        {
            PlayerObserved player = m_player[y];
            player.m_prevState = player.m_state;
            player.m_state = GamePad.GetState(player.m_playerIndex);

            for (int i = 0; i < player.m_boolObserved.Count; i++)
            {
                bool currentValue = GetBoolValue(ref player, player.m_boolObserved[i].m_value);
                bool previousValue = player.m_boolObserved[i].m_debugValue;
                //Not the most clean ever to use debug value
                player.m_boolObserved[i].m_debugValue = currentValue;

                if(currentValue!=previousValue)
                    m_onNamedBooleanChanged.Invoke(player.m_boolObserved[i].m_name, player.m_boolObserved[i].m_debugValue);
                //register.Set(player.m_boolObserved[i].m_name, player.m_boolObserved[i].m_debugValue);
            }
            for (int i = 0; i < player.m_floatObserved.Count; i++)
            {
                player.m_floatObserved[i].m_debugValue = GetBoolValueOfFloat(ref player, player.m_floatObserved[i].m_value);
                m_onNamedBooleanChanged.Invoke(player.m_floatObserved[i].m_name, player.m_floatObserved[i].m_debugValue);
                //register.Set(player.m_floatObserved[i].m_name, player.m_floatObserved[i].m_debugValue);
            }

            //// Detect if a button was pressed this frame
            //if (player.prevState.Buttons.A == ButtonState.Released && player.state.Buttons.A == ButtonState.Pressed)
            //{
            //    GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
            //}
            //// Detect if a button was released this frame
            //if (player.prevState.Buttons.A == ButtonState.Pressed && player.state.Buttons.A == ButtonState.Released)
            //{
            //    GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //}

            //// Make the current object turn
            //transform.localRotation *= Quaternion.Euler(0.0f, player.state.ThumbSticks.Left.X * 25.0f * Time.deltaTime, 0.0f);

        }


    }

    public void Add(uint selectedXbox, XInputFloatableValue floatable, string booleanName, float minValue, float maxValue)
    {
        if (selectedXbox < 1) return;
        if (selectedXbox > 4) return;
        m_player[selectedXbox-1].m_floatObserved.Add(new NamedFloatValue(floatable, booleanName, minValue, maxValue));
    }

    public void Add(uint selectedXbox, XInputBoolable button, string booleanName)
    {
        if (selectedXbox < 1) return;
        if (selectedXbox > 4) return;
        m_player[selectedXbox-1].m_boolObserved.Add(new NamedBoolValue(button, booleanName));
    }

    private float GetBoolValueOfFloat(ref PlayerObserved player, XInputFloatableValue value)
    {
        switch (value)
        {
            case XInputFloatableValue.TriggerLeft:return player.m_state.Triggers.Left;
            case XInputFloatableValue.TriggerRight: return player.m_state.Triggers.Right;
            case XInputFloatableValue.JoystickLeftEast: return player.m_state.ThumbSticks.Left.X;
            case XInputFloatableValue.JoystickLeftWest: return -player.m_state.ThumbSticks.Left.X;
            case XInputFloatableValue.JoystickLeftNorth: return player.m_state.ThumbSticks.Left.Y;
            case XInputFloatableValue.JoystickLeftSouth: return -player.m_state.ThumbSticks.Left.Y;
            case XInputFloatableValue.JoystickRightEast: return player.m_state.ThumbSticks.Right.X;
            case XInputFloatableValue.JoystickRightWest: return -player.m_state.ThumbSticks.Right.X;
            case XInputFloatableValue.JoystickRightNorth: return player.m_state.ThumbSticks.Right.Y;
            case XInputFloatableValue.JoystickRightSouth: return -player.m_state.ThumbSticks.Right.Y;
            default:
                break;
        }
        return 0;

    }

    private bool GetBoolValueOfFloat(ref PlayerObserved player, FloatValue value)
    {
        float f = GetBoolValueOfFloat(ref player, value.m_target);
       return f >= value.m_minValue &&  f <= value.m_maxValue;
    }

    private bool GetBoolValue(ref PlayerObserved player, XInputBoolable value)
    {
        switch (value)
        {
            case XInputBoolable.TriggerRight:
                return player.m_state.Triggers.Right > 0f;
            case XInputBoolable.TriggerLeft:
                return player.m_state.Triggers.Left > 0f;
            case XInputBoolable.SideButtonRight:
                return player.m_state.Buttons.RightShoulder == ButtonState.Pressed;
            case XInputBoolable.SideButtonLeft:
                return player.m_state.Buttons.LeftShoulder == ButtonState.Pressed;
            case XInputBoolable.ArrowLeft:
                return player.m_state.DPad.Left == ButtonState.Pressed;
            case XInputBoolable.ArrowRight:
                return player.m_state.DPad.Right == ButtonState.Pressed;
            case XInputBoolable.ArrowUp:
                return player.m_state.DPad.Up == ButtonState.Pressed;
            case XInputBoolable.ArrowDown:
                return player.m_state.DPad.Down == ButtonState.Pressed;
            case XInputBoolable.ButtonA:
                return player.m_state.Buttons.A == ButtonState.Pressed;
            case XInputBoolable.ButtonB:
                return player.m_state.Buttons.B == ButtonState.Pressed;
            case XInputBoolable.ButtonY:
                return player.m_state.Buttons.Y == ButtonState.Pressed;
            case XInputBoolable.ButtonX:
                return player.m_state.Buttons.X == ButtonState.Pressed;
            case XInputBoolable.MenuBackButton:
                return player.m_state.Buttons.Back== ButtonState.Pressed;
            case XInputBoolable.StartButton:
                return player.m_state.Buttons.Start == ButtonState.Pressed;
            case XInputBoolable.JoystickLeft:
                return player.m_state.Buttons.LeftStick == ButtonState.Pressed;
            case XInputBoolable.JoystickRight:
                return player.m_state.Buttons.RightStick == ButtonState.Pressed;
            default:
                break;
        }
        return false;
    }

    [System.Serializable]
    public class NamedFloatValue
    {
        public string m_name;
        public FloatValue m_value;
        public bool m_debugValue;
      

        public NamedFloatValue(XInputFloatableValue floatable, string booleanName, float minValue, float maxValue)
        {
            this.m_value = new FloatValue() { m_maxValue = maxValue, m_minValue=minValue, m_target= floatable};
            this.m_name = booleanName;
        }
    }
    [System.Serializable]
    public class NamedBoolValue
    {
        public string m_name;
        public XInputBoolable m_value;
        public bool m_debugValue;

        public NamedBoolValue(XInputBoolable button, string booleanName)
        {
            this.m_value = button;
            this.m_name = booleanName;
        }
    }

    [System.Serializable]
    public class FloatValue
    {
        public float m_minValue = 0, m_maxValue = 1;
        public XInputFloatableValue m_target;
    }

}


public enum XInputFloatableValue { 
    TriggerLeft,
    TriggerRight,

    JoystickLeftEast,
    JoystickLeftNorth,
    JoystickLeftSouth,
    JoystickLeftWest,

    JoystickRightEast,
    JoystickRightNorth,
    JoystickRightSouth,
    JoystickRightWest
}


public enum XInputBoolable {
    TriggerRight,
    TriggerLeft,
    SideButtonRight,
    SideButtonLeft,
    ArrowLeft, ArrowRight,
    ArrowUp,
    ArrowDown,
    ButtonA,
    ButtonB, 
    ButtonY,
    ButtonX,
    MenuBackButton,
    StartButton,
    JoystickLeft,
    JoystickRight
}