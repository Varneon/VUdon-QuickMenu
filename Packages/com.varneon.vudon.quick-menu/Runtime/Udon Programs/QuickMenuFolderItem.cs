using UnityEngine;
using UnityEngine.UI;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    [AddComponentMenu("")]
    [ExcludeFromPreset]
    [DisallowMultipleComponent]
    public class QuickMenuFolderItem : QuickMenuItem
    {
        [SerializeField]
        private Image arrow;

        public override ItemType Type => ItemType.Folder;

        internal void Initialize(string path, string tooltip)
        {
            RegisterAbstractProperties(path, null, tooltip);

            SetLabel(path.Contains("/") ? path.Substring(path.LastIndexOf('/') + 1) : path);
        }

        protected override void OnEnabledStateChanged(bool enabled)
        {
            arrow.color = enabled ? defaultContentColor : disabledContentColor;
        }
    }
}