using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteBox : MonoBehaviour
{
    private SpriteRenderer sr;
    public int myIndex;
    private AudioSource audioSource;
    public AudioClip clip;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }


    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (GameManager.instance.GameStat == GameStat.Game) return;

        Player player;
        if ((player = coll.gameObject.GetComponent<Player>()) != null)
        {
            //Debug.Log(player.ID);
            audioSource.PlayOneShot(clip);
            GameRoomSceneManager.instance.Vote(player.ID, myIndex);
        }
    }
}
