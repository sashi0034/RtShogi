#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace RtShogi.Scripts.Battle
{
    public interface IBoardKomaListGetter
    {
        public KomaUnit? GetOf(KomaId id);
    }
    
    public class BoardKomaList : IBoardKomaListGetter
    {
        private readonly List<KomaUnit> _unitList = new List<KomaUnit>();
        public List<KomaUnit> RawList => _unitList;

        public void Clear()
        {
            foreach (var unit in _unitList.Where(unit => unit != null))
                Util.DestroyGameObject(unit.gameObject);

            _unitList.Clear();
        }

        public void AddUnit(KomaUnit unit)
        {
            _unitList.Add(unit);
        }

        public void RemoveUnit(KomaUnit unit)
        {
            _unitList.Remove(unit);
        }

        public KomaUnit? GetOf(KomaId id)
        {
            return _unitList.FirstOrDefault(koma => koma.Id.IsSame(id));
        }
    }
}