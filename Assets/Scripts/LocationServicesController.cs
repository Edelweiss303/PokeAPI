using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationServicesController : Singleton<LocationServicesController>
{
    public enum LocationServicesState
    {
        Waiting,
        Searching,
        Ready,
        Failed
    }

    public LocationServicesState state = LocationServicesState.Waiting;

    public float latitude;
    public float longitude;

    public void GetLocation()
    {
        if (state != LocationServicesState.Searching)
        {
            state = LocationServicesState.Searching;
            StartCoroutine(LocationServiceUpdate());
        }
    }

    IEnumerator LocationServiceUpdate()
    {
        Input.location.Start();
        int waitTime = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && waitTime > 0)
        {
            yield return new WaitForSeconds(1);
            waitTime--;
        }

        if (waitTime <= 0)
        {
            state = LocationServicesState.Failed;
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed || Input.location.status == LocationServiceStatus.Stopped)
        {
            state = LocationServicesState.Failed;
            yield break;
        }

        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        Input.location.Stop();
        state = LocationServicesState.Ready;

        yield break;
    }
}
