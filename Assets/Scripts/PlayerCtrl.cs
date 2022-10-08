using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class PlayerCtrl : MonoBehaviour
{

    public TextMesh pseudoText;
    public PhotonView view;
    public GPCtrl GP;
    public CinemachineVirtualCamera playerCamera;

    //movement
    public float speed;
    public float currentSpeed;
    private Rigidbody2D rb;
    private Vector2 playerDirection;
    public bool isDashing;
    public bool canDash = true;
    public float dashingTime;
    public float dashingPower;

    public Slider dashingBar;
    public float currentDashingEnergy;

    public float dashCost;
    public float dashRechargeAmount;
    public ParticleSystem dashParticles;
    public ParticleSystem dashParticles2;

    public BoxCollider2D col;
    public bool isChaser = false;

    public int score = 0;

    private void Start()
    {
        GP = FindObjectOfType<GPCtrl>();
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        pseudoText = GetComponentInChildren<TextMesh>();
        playerCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        dashingBar = GetComponentInChildren<Canvas>().transform.GetComponentInChildren<Slider>();
        dashParticles = transform.Find("ParticleSystem").GetComponent<ParticleSystem>();
        dashParticles2 = transform.Find("ParticleSystem2").GetComponent<ParticleSystem>();
        dashParticles.enableEmission = false;
        dashParticles2.enableEmission = false;
        col = GetComponent<BoxCollider2D>();
        if (!view.IsMine)
        {
            playerCamera.enabled = false;
        } 
        if (view.IsMine)
        {
            view.RPC(nameof(DisplayPseudo), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
            Color _color = GP.skinColors[Random.Range(0, GP.skinColors.Length-1)];
            var R = Color.white.r;
            var G = Color.white.g;
            var B = Color.white.b;
            view.RPC(nameof(DisplayColor), RpcTarget.AllBuffered, R, G, B);
            currentDashingEnergy = dashingBar.maxValue;
            currentSpeed = speed;
        }

    }

    private void Update()
    {
        if (view.IsMine)
        {
            PlayerMove();
            if (currentDashingEnergy < dashingBar.maxValue)
            {
                currentDashingEnergy += dashRechargeAmount * Time.deltaTime;
            }
            if (currentDashingEnergy < dashCost) canDash = false;
            else canDash = true;
            view.RPC(nameof(UpdateDashingBar), RpcTarget.AllBuffered, currentDashingEnergy);
        }
    }

    [PunRPC]
    public void DisplayPseudo(string _pseudo)
    {
        GetComponentInChildren<TextMesh>().text = _pseudo;
    }

    [PunRPC]
    public void DisplayColor(float R, float G, float B)
    {
        GetComponent<SpriteRenderer>().color = new Color(R, G, B, 1);
    }

    [PunRPC]
    public void UpdateDashingBar(float _currentDashingEnergy)
    {
        GetComponentInChildren<Canvas>().transform.GetComponentInChildren<Slider>().value = _currentDashingEnergy;
    }

    public void PlayerMove()
    {
        float directionX = Input.GetAxisRaw("Horizontal");
        float directionY = Input.GetAxisRaw("Vertical");

        playerDirection = new Vector2(directionX, directionY);
        if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (!isDashing) rb.velocity = new Vector2(playerDirection.x * currentSpeed, playerDirection.y * currentSpeed);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        currentDashingEnergy = currentDashingEnergy - dashCost;
        view.RPC(nameof(UpdateDashingBar), RpcTarget.AllBuffered, currentDashingEnergy);
        rb.velocity = new Vector2(playerDirection.x * dashingPower, playerDirection.y * dashingPower);
        dashParticles.transform.up = -playerDirection;
        dashParticles.enableEmission = true;
        dashParticles2.enableEmission = true;
        yield return new WaitForSeconds(dashingTime);
        currentSpeed = speed / 4 * 3;
        isDashing = false;
        dashParticles.enableEmission = false;
        dashParticles2.enableEmission = false;
        yield return new WaitForSeconds(dashingTime*2);
        currentSpeed = speed / 3 * 4;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDashing && isChaser)
        {
            if (collision.GetComponent<PlayerCtrl>() != null)
            {
                PlayerCtrl _otherPlayer = collision.GetComponent<PlayerCtrl>();
                _otherPlayer.view.RPC(nameof(SetToChaser), RpcTarget.AllBuffered);
                view.RPC(nameof(SetToEvader), RpcTarget.AllBuffered);
                view.RPC(nameof(SetScore), RpcTarget.AllBuffered, (score + Mathf.RoundToInt(_otherPlayer.score / 2)));
                _otherPlayer.view.RPC(nameof(_otherPlayer.SetScore), RpcTarget.AllBuffered, Mathf.RoundToInt(_otherPlayer.score / 2));
            }
        }
    }

    [PunRPC]
    public void SetToChaser()
    {
        isChaser = true;
        var R2 = Color.red.r;
        var G2 = Color.red.g;
        var B2 = Color.red.b;
        view.RPC(nameof(DisplayColor), RpcTarget.AllBuffered, R2, G2, B2);
        gameObject.layer = LayerMask.NameToLayer("Chaser");
    }

    [PunRPC]
    public void SetToEvader()
    {
        isChaser = false;
        Color _color = GP.skinColors[Random.Range(0, GP.skinColors.Length - 1)];
        var R1 = Color.white.r;
        var G1 = Color.white.g;
        var B1 = Color.white.b;
        view.RPC(nameof(DisplayColor), RpcTarget.AllBuffered, R1, G1, B1);
        gameObject.layer = LayerMask.NameToLayer("Evader");
        currentSpeed = speed;
    }

    public void PickedForChaser(int _playerNumber)
    {
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[_playerNumber])
        { 
            if (view.IsMine) view.RPC(nameof(SetToChaser), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void SetScore(int _score)
    {
        score = _score;
        GetComponentInChildren<TextMesh>().text = _score.ToString();
    }


}
