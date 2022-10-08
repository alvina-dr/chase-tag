using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GPCtrl : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public UICtrl UI;
    public BonusPool Bonus;

    public Color[] skinColors = { Color.white, Color.blue, Color.yellow, Color.green, Color.magenta };
    public float chrono;
    public int points;

    private void Start()
    {
        UI = FindObjectOfType<UICtrl>();
        Bonus = FindObjectOfType<BonusPool>();
        PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        chrono += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.L) && PhotonNetwork.IsMasterClient)
        {
            LaunchGame();
        }
        if (PhotonNetwork.IsMasterClient && chrono >= 4)
        {
            if (Bonus.collectableList.Find(x => x.isActive == false) == null) return;
            SpawnBonus();
        }
    }

    public void LaunchGame()
    {
        int chaserPick = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        foreach (var _player in FindObjectsOfType<PlayerCtrl>())
        {
            _player.PickedForChaser(chaserPick);
        }
    }

    public void SpawnBonus()
    {
        //view.RPC(nameof(DisplayPseudo), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);

        CollectableCtrl _collectable = Bonus.GetCollectable();
        _collectable.view.RPC(nameof(_collectable.SpawnCollectable), RpcTarget.AllBuffered, new Vector3(Random.Range(-25, 25), Random.Range(-25, 25)));
        //Bonus.GetCollectable().SpawnCollectable(new Vector3(Random.Range(-25, 25), Random.Range(-25, 25)));
        //GameObject _bonus = Instantiate(collectablePrefab);
        //_bonus.transform.position = new Vector3(Random.Range(-25, 25), Random.Range(-25, 25));
        chrono = 0;
    }

    public void AddCollectable()
    {
        points++;
    }

}
