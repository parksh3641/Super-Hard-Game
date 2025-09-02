using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Missile : MonoBehaviour
{
    GameObject player;

    public GameObject model;
    public GameObject fire;
    public BoxCollider boxCollider;

    [SerializeField] private float speed = 0;

    private float missileSpeed = 0;

    Vector3 forward;

    void Awake()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
    }

    public void SetSpeed(float speed)
    {
        missileSpeed = speed;
    }

    void OnEnable()
    {
        model.SetActive(true);
        boxCollider.enabled = true;

        speed = 0;

        Invoke("Delay", 0.3f);
    }

    void Delay()
    {
        speed = Random.Range(missileSpeed * 0.8f, missileSpeed * 1.2f);
    }

    void Update()
    {
        if (model.activeInHierarchy)
        {
            forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            transform.position += forward * speed * Time.deltaTime;

            if ((player.transform.position - transform.position).sqrMagnitude > 100f)
            {
                fire.SetActive(false);
            }
            else
            {
                fire.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ParticleManager.instance.GetParticle(transform.position, transform.rotation);

            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlaySFX(GameSfxType.Bomb);
            }

            gameObject.SetActive(false);
        }
        else if(other.gameObject.tag == "SafeZone")
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 10f)
            {
                ParticleManager.instance.GetParticle(transform.position, transform.rotation);
            }

            gameObject.SetActive(false);
        }
        else if(other.gameObject.tag == "LastLine")
        {
            gameObject.SetActive(false);
        }
    }
}
