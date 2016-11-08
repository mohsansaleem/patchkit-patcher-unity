using UnityEngine;
using UnityEngine.UI;

namespace PatchKit.Unity.Patcher.UI
{
    public class ProgressBarFill : MonoBehaviour
    {
        public Text Text;

        public Slider Slider;

        private void SetProgress(float progress)
        {
            Text.text = progress.ToString("0.0%");
            Slider.value = progress;
        }

        private void Update()
        {
            var status = PatcherApplication.Instance.Patcher.Status;

            if (status.State == PatcherState.Patching)
            {
                SetProgress(status.Progress);
            }
            else if (status.State == PatcherState.Succeed)
            {
                SetProgress(1.0f);
            }
            else
            {
                SetProgress(0.0f);
            }
        }
    }
}