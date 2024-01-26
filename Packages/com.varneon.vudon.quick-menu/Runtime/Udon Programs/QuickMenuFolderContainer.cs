using UdonSharp;
using UnityEngine;

namespace Varneon.VUdon.QuickMenu
{
    [AddComponentMenu("")]
    [ExcludeFromPreset]
    [DisallowMultipleComponent]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class QuickMenuFolderContainer : UdonSharpBehaviour
    {
        public string Path => path;

        public int ItemCount => transform.childCount;

        [SerializeField, HideInInspector]
        private string path;

        internal void SetPath(string folderPath)
        {
            path = folderPath;
        }
    }
}
