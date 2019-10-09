using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayer : Player, IStateController
{
    public float speed;
    public float jumpHoilderMaxTime = 0.7f;
   
    public float jumpPower = 1.62f;
    public float maxJumpRbPower = 3.79f;
    public float maxMoveRbPower = 5.7f;
    public float attackCoolTime;
    public float checkCoolTime;

    private bool stunt;
    private bool canJump;
    private bool canAttack;
    private bool canCheckPoint;
    private bool inGround;

    protected Rigidbody2D rb;
    protected Controller _controller;
    protected AudioSource audioSource;

    public AudioClip jumpSound;
    public AudioClip deathSound;

    public StateMachine controller => _controller;

    public override bool IsDying => _controller.CurrentState is Dead;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        _controller = new Controller(gameObject);

        modelTr = tr.GetChild(0).GetComponent<Transform>();

        canJump = true;
        canAttack = true;
        canCheckPoint = true;
    }

    public void LeftButtonDown(ButtonEvent eventData)
    {
        if (eventData.Phase == ButtonEvent.EXIT || stunt)
        {
            animator.SetBool(IsWalking, false);
            return;
        }
        LeftMove();
    }

    public void RightButtonDown(ButtonEvent eventData)
    {
        if (eventData.Phase == ButtonEvent.EXIT || stunt)
        {
            animator.SetBool(IsWalking, false);
            return;
        }
        RightMove();
    }

    bool firstClick = false;

    public void JumpButtonDown(ButtonEvent eventData)
    {
        if (stunt) return;

        switch (eventData.Phase)
        {
            case ButtonEvent.Enter:
                if(canJump)
                audioSource.PlayOneShot(jumpSound);
                break;
            case ButtonEvent.EXIT:
                canJump = false;
                break;
            case ButtonEvent.HOLD:
                if (canJump && eventData.HoldingTime < jumpHoilderMaxTime)
                    Jump();
                break;
        }
    }

    public void AttackButton(Image g)
    {
        if (!canAttack) return;
        g.color = new Color(g.color.r, g.color.g, g.color.b, 18);
        canAttack = false;
        Debug.Log("sd");
        StartCoroutine(WaitCool(g, 0, attackCoolTime));
    }

    public void CheckPointButton(Image g)
    {
        if (!canCheckPoint || !inGround) return;
        g.color = new Color(g.color.r, g.color.g, g.color.b, 18);
        canCheckPoint = false;
        EventManager.CallEvent(new PlayerSetCheckPointEvent(tr.position));
        StartCoroutine(WaitCool(g, 1, checkCoolTime));

    }

    protected IEnumerator WaitCool(Image g, int n, float coolTime)
    {
        yield return new WaitForSeconds(coolTime);
        if (n == 0)
            canAttack = true;
        else
            canCheckPoint = true;
        g.color = new Color(g.color.r, g.color.g, g.color.b, 255);
    }

    protected override void setSpawnPoint()
    {
        EventManager.CallEvent(new PlayerSetCheckPointEvent(tr.position));
    }

    protected override void LeftMove()
    {
        base.LeftMove();
        if(rb.velocity.x > -maxMoveRbPower)
        rb.AddForce(-Vector2.right * speed);
    }

    protected override void RightMove()
    {
        base.RightMove();
        if(rb.velocity.x < maxMoveRbPower)
        rb.AddForce(Vector2.right * speed);
    }

    protected void Jump()
    {

        animator.SetBool(IsJumping, true);
        if (rb.velocity.y < maxJumpRbPower)
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    public override void Die()
    {
        _controller.ChangeState<Dead>();
        EventManager.CallEvent(new PlayerDeathEvent(tr.position));
    }

    public override void Respawn()
    {
        _controller.ChangeState<Idle>();
        tr.position = ClientGameSystem.instance.SpawnPoint;
        EventManager.CallEvent(new PlayerRespawnEvent());
    }

    public void Teleport(Vector2 pos)
    {
        tr.position = pos;
    }

    private void OnCollisionStay2D(Collision2D coll)
    {

        if (coll.collider.CompareTag("GROUND"))
        {
            canJump = true;
            inGround = true;
            animator.SetBool(IsJumping, false);
        }

    }


    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("GROUND"))
        {
            inGround = false;
        }
    }

    private void Update()
    {

        if (GameManager.instance == null) return;
        if (rb.velocity != Vector2.zero && GameManager.instance.Part == SocketPart.Client && PacketManager.instance != null && !(controller.CurrentState is Dead))
        {
            PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.SERVER, Packet.Type.SYNC_PLAYER_POS_TOSERVER, new UnityPacketData((Vector2)tr.position)));
            Debug.Log("Send");
        }
    }

    #region State Machine
    public abstract class PlayerState : State
    {
        protected LocalPlayer owner;
        protected StateMachine controller;

        private void Awake()
        {
            owner = GetComponent<LocalPlayer>();
            controller = owner.controller;
        }

    }
    public class Idle : PlayerState
    {
        public override void Enter()
        {


        }

        public override void Exit()
        {

        }
    }

    public class Importent : PlayerState
    {
        public override void Enter()
        {
            owner.stunt = true;
        }

        public override void Exit()
        {
            owner.stunt = false;
        }
    }

    public class Dead : PlayerState
    {
        public override void Enter()
        {
            owner.stunt = true;
            owner.animator.SetBool(owner.IsDead, true);
            owner.audioSource.PlayOneShot(owner.deathSound);

        }

        public override void Exit()
        {
            owner.stunt = false;
            owner.animator.SetBool(owner.IsDead, false);
            PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_PLAYER_RESPAWN));
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
