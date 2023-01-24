using UnityEngine;
using Varneon.VUdon.QuickMenu.Abstract;

namespace Varneon.VUdon.QuickMenu
{
    public class QuickMenuFolderItem : QuickMenuItem
    {
        public override ItemType Type => ItemType.Folder;

        public string FolderPath => folderPath;

        [SerializeField, HideInInspector]
        private string folderPath;

        internal void SetFolderPath(string path)
        {
            folderPath = path;

            SetLabel(path.Contains("/") ? path.Substring(path.LastIndexOf('/') + 1) : path);
        }

        internal void SetTooltip(string tooltip)
        {
            _tooltip = tooltip;
        }
    }
}