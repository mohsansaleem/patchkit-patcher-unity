using UnityEngine;
using UnityEngine.UI;

namespace PatchKit.Unity.Patcher.UI
{
    public class TitleBar : MonoBehaviour
    {
        public GameObject upperPanel;
        public RectTransform middlePanel;

        private void Update()
        {
            //bool isEnabled = false;//Application.platform == RuntimePlatform.WindowsPlayer;

            //upperPanel.SetActive(isEnabled);
            //if (!isEnabled)
            //{
            //    middlePanel.anchorMax.Set(middlePanel.anchorMax.x, 1);
            //    middlePanel.offsetMax = new Vector2(middlePanel.offsetMax.x, 0);
            //}
               
        }
    }
}