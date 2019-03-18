using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Image))]
    public class UI_Knob : MonoBehaviour
    {
        public enum Direction { CW, CCW };
        public Direction direction = Direction.CW;
        [HideInInspector]
        public float knobValue;
        public float maxValue = 0;
        public bool snapToPosition = false;
        [Space(30)]
        public KnobFloatValueEvent OnValueChanged;
        private float _previousValue = 0;
        private float _initAngle;
        private float _currentAngle;
        private Vector2 _currentVector;
        private Quaternion _initRotation;
        private bool _canDrag = false;

        public bool onEnterUseForHover = false;
        public bool onEnterUseForActivation = false;
        public UnityEvent onEnter;

        public bool onStayUseForHover = false;
        public bool onStayUseForActivation = false;
        public UnityEvent onHover;

        public bool onExitUseForHover = false;
        public bool onExitUseForActivation = false;
        public UnityEvent onExit;

        public bool detectOnEnter = true;
        public bool detectOnHover = true;
        public bool detectOnExit = true;
        public LayerMask hoverLayer;
        public LayerMask activationLayer;

        public bool disabled;
        /*
        //ONLY ALLOW ROTATION WITH POINTER OVER THE CONTROL
        public void OnPointerDown(PointerEventData eventData)
        {
            _canDrag = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _canDrag = false;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            _canDrag = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _canDrag = false;
        }
        */
        private void OnCollisionEnter(Collision coll)
        {
            coll.contacts;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (detectOnEnter && !disabled)
            {

                //LayerMask value is 2 to the power of its layer index
                if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && onEnterUseForActivation)
                {
                    Debug.Log("Act Layer Enter");
                    if (onEnter != null)
                    {
                        SetInitPointerData(other);
                        onEnter.Invoke();
                    }
                }

                else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onEnterUseForHover)
                {
                    Debug.Log("Hover Layer Enter");
                    if (onEnter != null)
                    {
                        onEnter.Invoke();
                    }
                }

                else if (onEnterUseForActivation && onEnterUseForHover)
                {
                    Debug.Log("Both Layer Enter");
                    if (onEnter != null)
                    {
                        onEnter.Invoke();
                    }
                }
            }
        }
        void OnTriggerStay(Collider other)
        {
            if (detectOnHover && !disabled)
            {
                if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && onStayUseForActivation)
                {
                    Debug.Log("Act Layer Stay");
                    if (onHover != null)
                    {
                        onHover.Invoke();
                    }
                }

                else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onStayUseForHover)
                {
                    Debug.Log("Hover Layer Stay");
                    if (onHover != null)
                    {
                        onHover.Invoke();
                    }
                }
                else if (onStayUseForActivation && onStayUseForHover)
                {
                    Debug.Log("Both Layer Enter");
                    if (onHover != null)
                    {
                        onHover.Invoke();
                    }
                }
            }

        }
        void OnTriggerExit(Collider other)
        {
            if (detectOnExit && !disabled)
            {
                if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && onExitUseForActivation)
                {
                    Debug.Log("Act Layer Exit");
                    if (onExit != null)
                    {
                        onExit.Invoke();
                    }
                }

                else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onExitUseForHover)
                {
                    Debug.Log("Hover Layer Exit");
                    if (onExit != null)
                    {
                        onExit.Invoke();
                    }
                }
                else if (onExitUseForActivation && onExitUseForHover)
                {
                    Debug.Log("Both Layer Enter");
                    if (onExit != null)
                    {
                        onExit.Invoke();
                    }
                }
            }
        }
        public void OnBeginDrag(Collider eventData)
        {
            SetInitPointerData(eventData);
        }
        void SetInitPointerData(Collider eventData)
        {
            _initRotation = transform.rotation;
            Vector3 position = new Vector3(0, 0, 0);
            ContactPoint[] contacts = new ContactPoint[5];
            eventData.GetContacts(contacts);
            foreach(ContactPoint i in contacts)
            {
                position += i.point;
            }
            int len = eventData.contacts.length;
            position /= len;
            Vector3 zeroz = transform.position;
            zeroz.z = 0;
            _currentVector = position - zeroz;
            _initAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;
        }
        public void OnDrag(Collider eventData)
        {
            //CHECK IF CAN DRAG
            if (!_canDrag)
            {
                SetInitPointerData(eventData);
                return;
            }
            _currentVector = eventData.position - (Vector2)transform.position;
            _currentAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;

            Quaternion addRotation = Quaternion.AngleAxis(_currentAngle - _initAngle, this.transform.forward);
            addRotation.eulerAngles = new Vector3(0, 0, addRotation.eulerAngles.z);

            Quaternion finalRotation = _initRotation * addRotation;

            if (direction == Direction.CW)
            {
                knobValue = 1 - (finalRotation.eulerAngles.z / 360f);

                if (snapToPosition)
                {
                    SnapToPosition(ref knobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 - 360 * knobValue);
                }
            }
            else
            {
                knobValue = (finalRotation.eulerAngles.z / 360f);

                if (snapToPosition)
                {
                    SnapToPosition(ref knobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 * knobValue);
                }
            }

            //CHECK MAX VALUE
            if (maxValue > 0)
            {
                if (knobValue > maxValue)
                {
                    knobValue = maxValue;
                    float maxAngle = direction == Direction.CW ? 360f - 360f * maxValue : 360f * maxValue;
                    transform.localEulerAngles = new Vector3(0, 0, maxAngle);
                    SetInitPointerData(eventData);
                    InvokeEvents(knobValue);
                    return;
                }
            }

            transform.rotation = finalRotation;
            InvokeEvents(knobValue);

            _previousValue = knobValue;
        }
        private void SnapToPosition(ref float knobValue)
        {
            float snapStep = 1 / (float)(1);
            float newValue = Mathf.Round(knobValue / snapStep) * snapStep;
            knobValue = newValue;
        }
        private void InvokeEvents(float value)
        {
            OnValueChanged.Invoke(value);
        }
    }

    [System.Serializable]
    public class KnobFloatValueEvent : UnityEvent<float> { }

}