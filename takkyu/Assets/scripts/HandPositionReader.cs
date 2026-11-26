
using UnityEngine;

public class HandPositionReader : MonoBehaviour
{
    private readonly object dataLock = new object();

    private Vector3 handPosition;
    private float handSize;
    private Vector3 handTilt;
    private bool isHandDetected;

    public Vector3 HandPosition
    {
        get { lock (dataLock) return handPosition; }
    }

    public float HandSize
    {
        get { lock (dataLock) return handSize; }
    }

    public Vector3 HandTilt
    {
        get { lock (dataLock) return handTilt; }
    }

    public bool IsHandDetected
    {
        get { lock (dataLock) return isHandDetected; }
    }

    public void SetHandPosition(
        float x, float y, float z, float size)
    {
        lock (dataLock)
        {
            handPosition = new Vector3(x, y, z);
            handSize = size;
            isHandDetected = true;
        }
    }

    public void SetHandTilt(float x, float y, float z)
    {
        lock (dataLock)
        {
            handTilt = new Vector3(x, y, z);
        }
    }

    public void ClearHand()
    {
        lock (dataLock)
        {
            isHandDetected = false;
        }
    }

    public bool TryGetHand(
        out Vector3 position, out float size)
    {
        lock (dataLock)
        {
            position = handPosition;
            size = handSize;
            return isHandDetected;
        }
    }

    public bool TryGetHandTilt(out Vector3 tilt)
    {
        lock (dataLock)
        {
            tilt = handTilt;
            return isHandDetected;
        }
    }
}
