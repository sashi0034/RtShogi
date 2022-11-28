
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.EditorUtil
{
    public class ObjectRenamer : MonoBehaviour
    {
        private const int editorSpace = 5;
        
        [Button, PropertySpace(editorSpace)]
        public void RenameParentNameToThisName()
        {
            this.gameObject.transform.parent.name = this.gameObject.name;
        }

        [Button, PropertySpace(editorSpace)]
        public void RenameParentNameToMeshName()
        {
            var mesh = this.gameObject.GetComponent<MeshFilter>();
            string name = mesh.sharedMesh.name;
            this.gameObject.transform.parent.name = name;
            Debug.Log("rename: " + name);
        }
    }
}