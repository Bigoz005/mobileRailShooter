using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHandler : MonoBehaviour
{
    private static EventHandler EventHandlerInstance;
    void Awake()
    {
        if (EventHandlerInstance == null)
        {
            EventHandlerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (EventHandlerInstance != this)
        {
            Destroy(gameObject);
        }

        gameObject.GetComponent<EventSystem>().enabled = false;
        gameObject.GetComponent<StandaloneInputModule>().enabled = false;
        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<EventSystem>().enabled = true;
        gameObject.GetComponent<StandaloneInputModule>().enabled = true;
        yield return null;
    }
}
