using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SubakGame.Gameplay
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeadlineTrigger : MonoBehaviour
    {
        [Header("판정")]
        [SerializeField, Tooltip("드롭 직후 그레이스(과일 생성 후 이 시간 안에는 카운트 안 함)")]
        private float graceTime = 0.5f;
        [SerializeField, Tooltip("데드라인 위에 누적 이 시간 머무르면 게임오버")]
        private float gameOverDelay = 1.5f;

        [Header("이벤트 (Inspector)")]
        [SerializeField] private UnityEvent<Collider2D> onEnter;
        [SerializeField] private UnityEvent<Collider2D> onExit;
        [SerializeField] private UnityEvent onGameOver;

        // 글로벌 구독 진입점 (Dropper / UI / SFX 가 구독)
        public static event System.Action GameOver;

        private readonly Dictionary<Fruit, float> enterTimes = new();
        private bool isGameOverFired;

        private void Reset()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        public void SetSize(Vector2 size)
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = size;
        }

        public void ResetState()
        {
            enterTimes.Clear();
            isGameOverFired = false;
        }

        private void OnEnable()
        {
            isGameOverFired = false;
            enterTimes.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            onEnter?.Invoke(other);
            var fruit = other.GetComponent<Fruit>();
            if (fruit == null) return;
            if (!enterTimes.ContainsKey(fruit))
            {
                enterTimes[fruit] = Time.time;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            onExit?.Invoke(other);
            var fruit = other.GetComponent<Fruit>();
            if (fruit == null) return;
            enterTimes.Remove(fruit);
        }

        private void Update()
        {
            if (isGameOverFired || enterTimes.Count == 0) return;

            List<Fruit> toRemove = null;
            float now = Time.time;

            foreach (var kv in enterTimes)
            {
                var fruit = kv.Key;
                if (fruit == null || !fruit.gameObject.activeInHierarchy)
                {
                    (toRemove ??= new List<Fruit>()).Add(fruit);
                    continue;
                }

                float graceEnd = fruit.SpawnedAt + graceTime;
                if (now < graceEnd) continue;

                float dwellStart = Mathf.Max(kv.Value, graceEnd);
                if (now - dwellStart >= gameOverDelay)
                {
                    FireGameOver();
                    return;
                }
            }

            if (toRemove != null)
            {
                foreach (var f in toRemove) enterTimes.Remove(f);
            }
        }

        private void FireGameOver()
        {
            if (isGameOverFired) return;
            isGameOverFired = true;
            Debug.Log("[Deadline] GAME OVER");
            onGameOver?.Invoke();
            GameOver?.Invoke();
        }
    }
}
