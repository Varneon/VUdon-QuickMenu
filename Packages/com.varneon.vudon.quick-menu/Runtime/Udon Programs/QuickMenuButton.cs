using Varneon.VUdon.Menus.Abstract;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    public class QuickMenuButton : QuickMenuItem
    {
        public override ItemType Type => ItemType.Button;

        public override bool OnClick()
        {
            if (_callbackReceiver) { _callbackReceiver.OnMenuButtonClicked(_path); }

            return true;
        }

        internal void Initialize(string itemPath, string label, MenuEventCallbackReceiver callbackReceiver, string tooltip = "")
        {
            RegisterAbstractProperties(itemPath, callbackReceiver, tooltip);

            SetLabel(label);
        }
    }
}
