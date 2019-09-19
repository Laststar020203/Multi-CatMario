using UnityEngine;


public abstract class Player : MonoBehaviour, IStateController
{

    public float speed;
    public float jumpHoilderMaxTime;
    public float jumpPower;

    protected Transform tr;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    protected Controller _controller;

    [SerializeField]
    protected bool isRightLook;
    protected bool stunt;
    protected bool isJump;

    public StateMachine controller => _controller;

    private void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        _controller = new Controller(gameObject);
       
    }

    protected void Reverse()
    {
        Vector3 scale = tr.localScale;
        scale.z *= -1;
        tr.localScale = scale;
        isRightLook = !isRightLook;
    }

    protected void RightMove()
    {
        if (stunt) return;
        if (isRightLook) Reverse();
        rb.AddForce(Vector2.right * speed);
    }

    protected void LeftMove()
    {
        if (stunt) return;
        if (isRightLook) Reverse();
        rb.AddForce(-Vector2.right * speed);
    }

    protected void Jump()
    {

    }

    protected void FireBall()
    {

    }

    protected void setSpawnPoint()
    {

    }

    public void Die()
    {
        _controller.ChangeState<Dead>();
        Debug.Log("PLAYER DIE!");
    }

    public void Transformation(Sprite sprite)
    {
        sr.sprite = sprite;
        
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        /*
        if (coll.collider.CompareTag("GROUND"))
        {
            isJump = false;
        }
        */
    }


    #region State Machine
    public abstract class PlayerState : State
    {
        protected Player owner;
        protected StateMachine controller;

        private void Awake()
        {
            owner = GetComponent<Player>();
            controller = owner.controller;
        }

    }
    public class Idle : PlayerState
    {
        public override void Enter()
        {
            owner.stunt = false;
            
        }

        public override void Exit()
        {
            owner.stunt = true;
        }
    }

    public class Importent : PlayerState
    {
        public override void Enter()
        {

        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    public class Dead : PlayerState
    {
        public override void Enter()
        {


        }
    }

    //상태머신 안에 메소드는 상태만 변경해준다.
    public class Controller : StateMachine
    {
        public Controller(GameObject owner) : base(owner)
        {

        }

        protected override void Control()
        {

        }
    }

    #endregion
}


