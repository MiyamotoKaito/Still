using TMPro;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///画面に出す脱出のチェックリスト
    ///</summary>
    public class EscapeChecklistView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _tv;
        [SerializeField] private TextMeshProUGUI _recordPlayer;
        [SerializeField] private TextMeshProUGUI _book;

        ///<summary>
        ///調べたものにチェックを付ける
        ///</summary>
        public void ShowProgress(EscapeProgress progress)
        {
            _tv.text = Format(progress.IsAchieved(EscapeItemType.Tv), "Tv");
            _recordPlayer.text = Format(progress.IsAchieved(EscapeItemType.Music), "PlayMusic");
            _book.text = Format(progress.IsAchieved(EscapeItemType.Book), "Book");
        }

        ///<summary>
        ///鍵を探す指示に切り替える
        ///</summary>
        public void ShowFindKey()
        {
            _tv.gameObject.SetActive(false);
            _book.gameObject.SetActive(false);
            _recordPlayer.text = "□:Find the key!!";
        }

        private static string Format(bool isDone, string label) => $"{(isDone ? "■" : "□")}:{label}";
    }
}
