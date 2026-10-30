using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Still.Escape
{
    ///<summary>
    ///画面に出すミッションのチェックリスト（見出し＋目標の行）
    ///</summary>
    public class EscapeChecklistView : MonoBehaviour
    {
        // 既に画面に並べてある行をそのまま使い、足りない分は最後の行を複製して同じ間隔で下へ並べる
        [SerializeField, FormerlySerializedAs("_tv")] private TextMeshProUGUI _line0;
        [SerializeField, FormerlySerializedAs("_recordPlayer")] private TextMeshProUGUI _line1;
        [SerializeField, FormerlySerializedAs("_book")] private TextMeshProUGUI _line2;

        private readonly List<TextMeshProUGUI> _lines = new();

        ///<summary>
        ///見出しと目標の行を表示する
        ///</summary>
        public void Show(string title, IReadOnlyList<string> objectives)
        {
            var texts = new List<string>();
            if (!string.IsNullOrEmpty(title)) texts.Add(title);
            texts.AddRange(objectives);

            EnsureLines(texts.Count);
            for (int i = 0; i < _lines.Count; i++)
            {
                bool isUsed = i < texts.Count;
                _lines[i].gameObject.SetActive(isUsed);
                if (isUsed) _lines[i].text = texts[i];
            }
        }

        private void EnsureLines(int count)
        {
            if (_lines.Count == 0)
            {
                foreach (var line in new[] { _line0, _line1, _line2 })
                {
                    if (line != null) _lines.Add(line);
                }
            }

            var step = LineStep();
            while (_lines.Count < count)
            {
                var last = _lines[_lines.Count - 1];
                var clone = Instantiate(last, last.transform.parent);
                clone.rectTransform.anchoredPosition = last.rectTransform.anchoredPosition + step;
                _lines.Add(clone);
            }
        }

        private Vector2 LineStep()
        {
            if (_lines.Count >= 2) return _lines[1].rectTransform.anchoredPosition - _lines[0].rectTransform.anchoredPosition;
            return new Vector2(0f, -_lines[0].rectTransform.rect.height);
        }
    }
}
