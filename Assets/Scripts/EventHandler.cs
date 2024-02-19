using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHandler : MonoBehaviour
{
    private static EventHandler EventHandlerInstance;
    void Awake()
    {
        gameObject.GetComponent<EventSystem>().enabled = false;
        gameObject.GetComponent<StandaloneInputModule>().enabled = false;
        /*gameObject.GetComponent<BaseInput>().enabled = false;*/
        StartCoroutine(Initialization());
        if (EventHandlerInstance == null)
        {
            EventHandlerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (EventHandlerInstance != this)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Initialization()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<EventSystem>().enabled = true;
        gameObject.GetComponent<StandaloneInputModule>().enabled = true;
        /*gameObject.GetComponent<BaseInput>().enabled = true;*/
        yield return null;
    }
}
