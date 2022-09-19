using System.Collections;
using UnityEngine;

namespace AN_Match3
{
    public class Combo : MonoBehaviour
    {
        public AnimationClip clip;

        public int singleComboAmount;
        public int index;
        public Sprite[] numbers;

        private SpriteRenderer numberSpr;

        private void Awake()
        {
            numberSpr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            numberSpr.sprite = numbers[index];
            StartCoroutine(nameof(OnClipComplete));
        }

        private IEnumerator OnClipComplete()
        {
            yield return new WaitForSeconds(clip.length);
            gameObject.SetActive(false);
        }

        public void UpdateCombo()
        {
            if (singleComboAmount <= 1) return;
            
            index = singleComboAmount;
            gameObject.SetActive(true);
        }
    }
}