using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class ButtonListener : UnityEvent<ButtonEvent> { }

public class CMButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IStateController
{
    [Header("Button Sprite")]
    public Sprite normal;
    public Sprite pressed;

    private Image myImage;
    private Controller _controller;

    [Header("Button Event Handler"), SerializeField]
    public ButtonListener handler;


    private float time;

    private bool isEnd;


    public StateMachine controller => _controller;

    private void Awake()
    {
        myImage = GetComponent<Image>();
        _controller = new Controller(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isEnd = false;
        _controller.ChangeState<Hold>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isEnd)
        {
            _controller.ChangeState<Exit>();
            isEnd = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isEnd)
        {
            _controller.ChangeState<Exit>();
            isEnd = true;
        }
    }

    public abstract class ButtonState : State
    {
        protected CMButton owner;
        private void Awake()
        {
            owner = GetComponent<CMButton>();
        }
    }

    public class Hold : ButtonState
    {
        Coroutine execute;
        public override void Enter()
        {
            owner.time = Time.time;
            owner.handler.Invoke(new ButtonEvent(ButtonEvent.Enter, 0));
            execute = StartCoroutine(Execute());
            
        }

        private IEnumerator Execute()
        {
            while (true)
            {
                owner.handler.Invoke(new ButtonEvent(ButtonEvent.HOLD, Time.time - owner.time));
                yield return null;
            }
        }

        public override void Exit()
        {
            StopCoroutine(execute);
        }
    }

    public class Exit : ButtonState
    {
        public override void Enter()
        {
            //owner.myImage.sprite = owner.normal;
            owner.handler.Invoke(new ButtonEvent(ButtonEvent.EXIT, 0));
        }
    }

    public class Controller : StateMachine
    {
        public Controller(GameObject owner) : base(owner)
        {
        }

        protected override void Control()
        {
            
        }
    }
}

public class ButtonEvent : Event
{
    public const int Enter = 0;
    public const int HOLD = 1;
    public const int EXIT = 2;

    public int Phase { get { return _phase; } }
    private readonly int _phase;
    
    public float HoldingTime { get { return _holdingTime; } }
    private readonly float _holdingTime;

    public ButtonEvent(int phase, float holdingTime)
    {
        this._phase = phase;
        this._holdingTime = holdingTime;
    }
}
