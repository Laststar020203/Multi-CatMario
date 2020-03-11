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

    public GameObject fireBall;
    public GameObject checkPoint;

    private Transform checkPos;
    int count = 0;
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
        firePos = modelTr.GetChild(0);
        checkPos = modelTr.GetChild(1);

        canJump = false;
        canAttack = true;
        canCheckPoint = true;

        myColor = sr.color;

        ws = new WaitForSeconds(sendDelay);
    }

    protected override void gv_UpdateStat(PlayerStat stat)
    {
        switch (stat)
        {
            case PlayerStat.Player:
                sr.color = myColor;
                break;
            case PlayerStat.Spectator:
                sr.color = new Color(0, 0, 0);
                break;
        }
    }

    private void Start()
    {
            StartCoroutine(SendMyPos());


    }

    public void LeftButtonDown(ButtonEvent eventData)
    {
        switch (stat)
        {
            case PlayerStat.Player:
                if (eventData.Phase == ButtonEvent.EXIT || stunt)
                {
                    animator.SetBool(IsWalking, false);
                    return;
                }
                LeftMove();
                break;
            case PlayerStat.Spectator:
                LeftMove();
                break;

        }

    }

    public void RightButtonDown(ButtonEvent eventData)
    {
        switch (stat)
        {
            case PlayerStat.Player:
                if (eventData.Phase == ButtonEvent.EXIT || stunt)
                {
                    animator.SetBool(IsWalking, false);
                    return;
                }
                RightMove();
                break;
            case PlayerStat.Spectator:
                RightMove();
                break;

        }
    }



    public void JumpButtonDown(ButtonEvent eventData)
    {

        switch (stat)
        {
            case PlayerStat.Player:
                if (stunt) return;

                switch (eventData.Phase)
                {
                    case ButtonEvent.Enter:
                        if (canJump)
                            audioSource.PlayOneShot(jumpSound, GameManager.instance.Setting.SoundValue);
                        break;
                    case ButtonEvent.EXIT:
                        canJump = false;
                        break;
                    case ButtonEvent.HOLD:
                        if (canJump && eventData.HoldingTime < jumpHoilderMaxTime)
                            Jump();
                        break;
                }
                break;
            case PlayerStat.Spectator:
                Jump();
                break;
            default:
                break;
        }
    }

    int s = 0;
    public void AttackButton(Image g)
    {
        if(stat == PlayerStat.Spectator)
        {
            if (count < 20 && count % 2 == 0)
            {
                GameManager.instance.ShowMessage("당신은 관전자입니다", 1.0f, MessageType.Important);
            }else if(count < 70 && count % 3 == 0)
            {
                GameManager.instance.ShowMessage("아니", 1.0f, MessageType.Important);
                GameManager.instance.ShowMessage("님", 1.0f, MessageType.Commmon);
                GameManager.instance.ShowMessage("관전자에요", 1.0f, MessageType.Commmon);
            }else if(count >  71 && count % 5 == 0)
            {
                GameManager.instance.ShowMessage("...", 1.0f, MessageType.Important);
                GameManager.instance.ShowMessage("싸이코세요?", 1.0f, MessageType.Commmon);
                GameManager.instance.ShowMessage("누구 죽이고 싶어 안달나셨는지...", 1.0f, MessageType.Commmon);
            }
            count++;

            return;
        }

        if (!canAttack) return;
        g.color = new Color(g.color.r, g.color.g, g.color.b, 18);
        canAttack = false;
        //Right 1, left 0
        PacketManager.instance.PutPacket(new Packet(ID, Packet.Target.ALL, Packet.Type.SYNC_PLAYER_ATTACK, new byte[2] { id, (byte)(isRightLook ? 1 : 0) }));
        EventManager.CallEvent(new PlayerAttackEvent(id));
        Shot();
        StartCoroutine(WaitCool(g, 0, attackCoolTime));
        if(s == 7)
        GameManager.instance.ShowMessage("월래 그러라고 만든 게임입니다", 1.0f, MessageType.Commmon);
        s++;
    }

    private void Shot()
    {
        GameObject.Instantiate(fireBall, firePos.position, isRightLook ? Quaternion.Euler(0,0,0) : Quaternion.Euler(0, 180,0));
    }
    int aCount;
    public void CheckPointButton(Image g)
    {
        if (stat == PlayerStat.Spectator)
        {
            if (aCount < 10 && aCount % 2 == 0)
            {
                GameManager.instance.ShowMessage("님은 관전자에요", 1.0f, MessageType.Important);
            }else if(aCount < 30 && aCount % 2 == 0)
            {
                GameManager.instance.ShowMessage("체크 포인트가 필요 있나? 관전자인데..", 1.0f, MessageType.Important);
            }else if(aCount > 31)
            {
                GameManager.instance.ShowMessage("손가락 안아파요?", 1.0f, MessageType.Important);
            }
            aCount++;

            return;
        }
        if (!canCheckPoint || !inGround) return;
        g.color = new Color(g.color.r, g.color.g, g.color.b, 18);
        canCheckPoint = false;
        GameObject obj =  Instantiate(checkPoint, checkPos.position, Quaternion.identity);
        EventManager.CallEvent(new PlayerSetCheckPointEvent(tr.position, obj));
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
        if (stat == PlayerStat.Spectator)
        {
            return;
        }

        _controller.ChangeState<Dead>();
        rb.velocity = Vector2.zero;
        //EventManager.CallEvent(new PlayerDeathEvent(tr.position));;
    }

    public override void Respawn()
    {
        if (stat == PlayerStat.Spectator)
        {
            return;
        }

        _controller.ChangeState<Idle>();
        tr.GetChild(0).localPosition = Vector3.zero;
        tr.position = ClientGameSystem.instance.SpawnPoint;
        EventManager.CallEvent(new PlayerRespawnEvent());
        PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_PLAYER_RESPAWN));
    }

    public void Teleport(Vector2 pos)
    {
        tr.position = pos;
    }

    private void OnCollisionStay2D(Collision2D coll)
    {

        if (coll.collider.CompareTag("GROUND")||(coll.collider.CompareTag("Wall")))
        {
            canJump = true;
            inGround = true;
            animator.SetBool(IsJumping, false);
        }

    }


    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("GROUND") || (coll.collider.CompareTag("Wall")))
        {
            inGround = false;
        }
    }

  
    public float sendDelay = 0.01f;
    private WaitForSeconds ws;
    private Vector2 beforPosition;

    private IEnumerator SendMyPos()
    {
        while (true)
        {
            while (stat == PlayerStat.Spectator || (beforPosition != null && beforPosition == (Vector2)tr.position) || controller.CurrentState is Dead) yield return null;

            PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_PLAYER_POS, new UnityPacketData((Vector2)tr.position)));

            beforPosition = tr.position;

            yield return ws;
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
            owner.animator.SetTrigger(owner.IsDead);
            owner.audioSource.PlayOneShot(owner.deathSound, GameManager.instance.Setting.SoundValue);
            PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_PLAYER_DEAD));
        }

        public override void Exit()
        {
            owner.stunt = false;
            //owner.animator.SetBool(owner.IsDead, false);
            
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


    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


}
