using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{

    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TMP_InputField pseudoInput;

    //block player from creating room with no text
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void Update()
    {
        if (pseudoInput.text != PhotonNetwork.LocalPlayer.NickName)
        {
            PhotonNetwork.LocalPlayer.NickName = pseudoInput.text;
        }
    }

}
