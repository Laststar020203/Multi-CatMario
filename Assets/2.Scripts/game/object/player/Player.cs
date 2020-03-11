using UnityEngine;
using UnityEngine.UI;


public enum PlayerStat { Player, Spectator }
public abstract class Player : MonoBehaviour
{

    protected Transform tr;
    protected SpriteRenderer sr;

    [SerializeField]
    protected bool isRightLook = false;

    protected Transform modelTr;
    protected Transform firePos;

    protected byte id;
    protected string name;
    protected byte characterCode;

    protected Animator animator;
    protected PlayerStat stat = PlayerStat.Player;
    protected Color myColor;

    protected readonly string IsJumping = "IsJumping";
    protected readonly string IsDead = "IsDead";
    protected readonly string IsWalking = "IsWalking";


    public byte ID { get { return id; } }
    public string Name { get { return name; } }
    public Vector2 Pos { get { return tr.position; } }
    public Transform FirePos { get { return firePos; } }
    public PlayerStat Stat { get { return stat; } set {
            gv_UpdateStat(value);
            stat = value;
        } }

    public abstract bool IsDying {get;}


    protected abstract void gv_UpdateStat(PlayerStat stat);


    public void Init(byte ID, string name, byte characterCode)
    {
        this.id = ID;
        this.name = name;
        this.characterCode = characterCode;


        switch (characterCode)
        {
            case 0:
                sr.color = myColor = new Color(255, 255, 255);
                break;
            case 1:
                sr.color = myColor = new Color(255, 0, 0);
                break;
            case 2:
                sr.color = myColor = new Color(0, 180, 255);
                break;
            case 3:
                sr.color = myColor = new Color(255, 255, 0);
                break;                
        }

        tr.GetComponentInChildren<Text>().text = name;
    }

    protected void Reverse()
    {
        Vector3 scale = modelTr.localScale;
        scale.x *= -1;
        modelTr.localScale = scale;
        isRightLook = !isRightLook;
    }

    protected virtual void RightMove()
    {
        if (!isRightLook) Reverse();
        animator.SetBool(IsWalking, true);
    }

    protected virtual void LeftMove()
    {
        if (isRightLook) Reverse();
        animator.SetBool(IsWalking , true);
    }

    protected virtual void FireBall()
    {

    }

    protected virtual void setSpawnPoint()
    {

    }

    public abstract void Die();

    public abstract void Respawn();


    public void Transformation(Sprite sprite)
    {
        animator.enabled = false;
        sr.sprite = sprite;
        
    }




    
}


