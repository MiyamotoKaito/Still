using System;
using System.Collections.Generic;
using Still.Common;
using TMPro;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///出口の鍵
    ///</summary>
    public class KeyView : MonoBehaviour, IInteractable
    {
        public event Action OnInteracted;

        ///<summary>出現位置の候補</summary>
        public IReadOnlyList<GameObject> SpawnPositions => _spawnPositions;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameObject[] _spawnPositions;

        ///<summary>
        ///調べる
        ///</summary>
        public void Interact()
        {
            OnInteracted?.Invoke();
        }

        ///<summary>
        ///指定位置に出現させる
        ///</summary>
        public void Show(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
        }

        ///<summary>
        ///隠す
        ///</summary>
        public void Hide()
        {
            HideUI();
            gameObject.SetActive(false);
        }

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        public void ShowUI()
        {
            _text.gameObject.SetActive(true);
            _text.text = "Collect Key";
        }

        ///<summary>
        ///操作ガイドを隠す
        ///</summary>
        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }
    }
}
