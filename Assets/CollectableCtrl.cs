using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollectableCtrl : MonoBehaviour
{
    public bool isActive = false;
    public PhotonView view;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCtrl>() == null || !PhotonNetwork.IsMasterClient) return; 
        PlayerCtrl _player = collision.GetComponent<PlayerCtrl>();
        if (_player.isChaser)
        {
            _player.speed++;
        } else
        {
            _player.view.RPC(nameof(_player.SetScore), RpcTarget.AllBuffered, (_player.score + 1));
        }
        view.RPC(nameof(DeactivateCollectable), RpcTarget.AllBuffered);
        //DeactivateCollectable();
    }

    [PunRPC]
    public void SpawnCollectable(Vector3 _position)
    {
        transform.position = _position;
        gameObject.SetActive(true);
    }

    [PunRPC]
    public void DeactivateCollectable()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

}
