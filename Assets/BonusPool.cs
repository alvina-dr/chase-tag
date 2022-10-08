using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Photon.Pun;

[ExecuteInEditMode]
public class BonusPool : MonoBehaviour
{ 
    public int collectableNumber;
    public GameObject collectablePrefab;
    public List<CollectableCtrl> collectableList;

    void Start()
    {
        //if (instantiateButton) InstantiatePool(ref instantiateButton);
        //instantiateButton = false;
        InstantiatePool();
    }

    void InstantiatePool()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
            i--;
        }
        collectableList.Clear();
        for (int i = 0; i < collectableNumber; i++)
        {
            //CollectableCtrl _collectable = Instantiate(collectablePrefab, transform).GetComponent<CollectableCtrl>();
            CollectableCtrl _collectable = PhotonNetwork.Instantiate("Collectable", Vector3.zero, Quaternion.identity).GetComponent<CollectableCtrl>();
            _collectable.transform.SetParent(transform);
            collectableList.Add(_collectable);
            _collectable.view = _collectable.GetComponent<PhotonView>();
            //_collectable.gameObject.SetActive(false);
            _collectable.view.RPC(nameof(_collectable.DeactivateCollectable), RpcTarget.AllBuffered);
        }
    }

    public CollectableCtrl GetCollectable()
    {
        CollectableCtrl _collectable = collectableList.Find(x => x.isActive == false);
        if (_collectable != null) _collectable.isActive = true;
        return _collectable;
    }
}
