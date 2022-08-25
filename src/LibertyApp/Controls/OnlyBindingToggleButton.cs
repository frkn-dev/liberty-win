using System.Windows.Controls.Primitives;

namespace LibertyApp.Controls
{
    class OnlyBindingToggleButton : ToggleButton
    {
        protected override void OnToggle()
        {
            if (IsChecked != false)
            {
                base.OnToggle();
            }
        }
    }
}
