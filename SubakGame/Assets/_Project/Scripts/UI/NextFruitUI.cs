using UnityEngine;
using UnityEngine.UI;
using SubakGame.Gameplay;

namespace SubakGame.UI
{
    public class NextFruitUI : MonoBehaviour
    {
        [SerializeField] private Image fruitImage;

        private void OnEnable()
        {
            Dropper.NextFruitChanged += UpdateUI;
        }

        private void OnDisable()
        {
            Dropper.NextFruitChanged -= UpdateUI;
        }

        private void UpdateUI(FruitData nextFruit)
        {
            if (fruitImage == null) return;

            if (nextFruit != null)
            {
                fruitImage.sprite = nextFruit.sprite;
                fruitImage.color = Color.white;
                fruitImage.enabled = true;
            }
            else
            {
                fruitImage.enabled = false;
            }
        }
    }
}
