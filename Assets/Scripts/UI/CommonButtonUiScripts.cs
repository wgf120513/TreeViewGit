using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class CommonButtonUiScripts : EventTrigger
    {
        public delegate void ListenDelgegata(GameObject go);
        public ListenDelgegata OnClick;
        public ListenDelgegata OnEnter;
        public ListenDelgegata OnExit;
        private int _hashCode;

        void Awake()
        {
            _hashCode = name.GetHashCode();
            //UiManager.Instance.RegistGameObject(name, gameObject);
            //UiManager.Instance.RegistGameObject(_hashCode, gameObject);
        }

        public static CommonButtonUiScripts Get(GameObject go)
        {
            var commonButtonUi = go.GetComponent<CommonButtonUiScripts>() ?? go.AddComponent<CommonButtonUiScripts>();
            return commonButtonUi;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                OnClick(gameObject);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (OnEnter != null)
            {
                OnEnter(gameObject);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (OnExit != null)
            {
                OnExit(gameObject);
            }
        }



    }
}
