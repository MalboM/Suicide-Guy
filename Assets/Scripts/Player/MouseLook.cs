//Copyright 2015 Michele Pirovano
using System;
using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;


/// <summary>
/// Allows looking around with a mouse.
/// Also allows the use of joystick axes.
/// </summary>
[Serializable]
public class MouseLook
{
    public float XSensitivity = 15f;
    public float YSensitivity = 15f;
    public float MinimumX = -360F;
    public float MaximumX = 360F;
    public float MinimumY = -90F;
    public float MaximumY = 90F;

    private float xvel = 0f;
    private float yvel = 0f;
    public bool smoothX;
    public bool smoothY;
    public float smoothtimeX;
    public float smoothtimeY;

    private float GetAxisX()
    {
        float v = CrossPlatformInputManager.GetAxis("Xbox360ControllerRightX");
        if (v == 0) v = CrossPlatformInputManager.GetAxis("Mouse X");
        return v;
    }

    private float GetAxisY()
    {
        float v = CrossPlatformInputManager.GetAxis("Xbox360ControllerRightY");
        if (v == 0) v = CrossPlatformInputManager.GetAxis("Mouse Y");
        return v;
    }

    public Vector2 UnClamped(float x, float y)
    {
        Vector2 value;
        value.x = y + GetAxisX() * XSensitivity;
        value.y = x + GetAxisY() * YSensitivity;

        if (smoothX)  value.x = Mathf.SmoothDamp(y, value.x, ref xvel, smoothtimeX);
        if (smoothY)  value.y = Mathf.SmoothDamp(x, value.y, ref yvel, smoothtimeY);
        return value;
    }


    public Vector2 Clamped(float x, float y)
    {
        Vector2 value;
        value.x = y + GetAxisX() * XSensitivity;
        value.y = x + GetAxisY() * YSensitivity;

        value.x = Mathf.Clamp(value.x, MinimumX, MaximumX);
        value.y = Mathf.Clamp(value.y, MinimumY, MaximumY);

        if (smoothX) value.x = Mathf.SmoothDamp(y, value.x, ref xvel, smoothtimeX);
        if (smoothY) value.y = Mathf.SmoothDamp(x, value.y, ref yvel, smoothtimeY);
        return value;
    }
}
