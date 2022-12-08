#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RtShogi.Scripts.Battle.UI
{
    public class ObtainedKomaGroup : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup grid;
        [FormerlySerializedAs("element")] [SerializeField] private ObtainedKomaElement elementPrefab;

        private List<ObtainedKomaElement> _showingList = new List<ObtainedKomaElement>();

        [EventFunction]
        private void Awake()
        {
            destroyAllElements();
        }

        private void destroyAllElements()
        {
            foreach (var child in grid.transform.GetChildren())
            {
                Util.DestroyGameObject(child.gameObject);
            }

            _showingList = new List<ObtainedKomaElement>();
        }

        public void IncElement(ObtainedKomaElementProps props)
        {
            bool hasPushed = pushElementIfSameExistInCurrentList(props);
            if (hasPushed) return;

            var newElem = Instantiate(elementPrefab, grid.transform);
            Debug.Assert(newElem!=null);
            newElem.Init(props);
            _showingList.Add(newElem);
        }

        public void DecElement(ObtainedKomaElement element)
        {
            element.DecQuantity();
            if (element.NumQuantity>0) return;
            _showingList.Remove(element);
            Util.DestroyGameObject(element.gameObject);
        }

        public void FindAndDecElement(EKomaKind kind)
        {
            var element = FindElement(kind);
            if (element==null) return;
            DecElement(element);
        }

        private bool pushElementIfSameExistInCurrentList(ObtainedKomaElementProps props)
        {
            var indexAlreadyExist = findIndexOfElement(props.Kind);
            if (indexAlreadyExist == null) return false;
            _showingList[indexAlreadyExist.Value].IncQuantity();
            return true;
        }

        public ObtainedKomaElement? FindElement(EKomaKind kind)
        {
            int? index = findIndexOfElement(kind);
            return index != null
                ? _showingList[index.Value]
                : null;
        }

        private int? findIndexOfElement(EKomaKind kind)
        {
            for (int i = 0; i < _showingList.Count; i++)
            {
                if (_showingList[i].Kind == kind) return i;
            }
            return null;
        }

        public ObtainedKomaElement? FindDraggingElem()
        {
            return _showingList.FirstOrDefault(elem => elem.HasBeginDrag.TakeFlag());
        }
    }
}