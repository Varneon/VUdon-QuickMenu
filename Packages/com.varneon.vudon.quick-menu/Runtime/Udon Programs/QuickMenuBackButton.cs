using UnityEngine;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    [AddComponentMenu("")]
    [ExcludeFromPreset]
    [DisallowMultipleComponent]
    public class QuickMenuBackButton : QuickMenuItem
    {
        public override ItemType Type => ItemType.Back;
    }
}
