using UnityEngine;
using UnityEngine.Events;

namespace SubakGame.Gameplay
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeadlineTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Collider2D> onEnter;
        [SerializeField] private UnityEvent<Collider2D> onExit;

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

        private void OnTriggerEnter2D(Collider2D other) => onEnter?.Invoke(other);
        private void OnTriggerExit2D(Collider2D other) => onExit?.Invoke(other);
    }
}
