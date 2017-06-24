using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;

namespace Assets.Scripts.Common
{
    public class CommonManager
    {

        public static GameObject FindObjWithName(GameObject go, string name)
        {
            GameObject ob = null;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).name == name)
                {
                    ob = go.transform.GetChild(i).gameObject;
                    return ob;
                }
                else
                {
                    ob = FindObjWithName(go.transform.GetChild(i).gameObject, name);
                    if (ob != null)
                    {
                        return ob;
                    }
                }
            }
            return null;
        }

        public static T FindComponentWithName<T>(GameObject go, string name) where T : UnityEngine.Object
        {
            T ob = default(T);
            for (int i = 0; i < go.transform.childCount; i++)
            {
                if (go.transform.GetChild(i).name == name)
                {
                    ob = go.transform.GetChild(i).gameObject.GetComponent<T>();
                    return ob;
                }
                else
                {
                    ob = FindComponentWithName<T>(go.transform.GetChild(i).gameObject, name);
                    if (ob != null)
                    {
                        return ob;
                    }
                }
            }
            return default(T);
        }
        
    }

    public static class CommonStaticManager
    {
        public static T FindGameObjectByTagInParent<T>(this MonoBehaviour mono, string tagName) where T : MonoBehaviour
        {
            if (mono.CompareTag(tagName))
            {
                return mono.GetComponent<T>();
            }
            if (!mono.transform.parent)
                return null;
            var parent = mono.transform.parent;
            while (parent)
            {
                if (parent.CompareTag(tagName))
                {
                    return parent.GetComponent<T>();
                }
                else
                {
                    if (parent.parent)
                    {
                        parent = parent.parent;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            return null;

        }
    }


}



