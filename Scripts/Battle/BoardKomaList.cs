using System.Collections.Generic;

namespace RtShogi.Scripts.Battle
{
    public class BoardKomaList
    {
        private List<KomaUnit> _unitList = new List<KomaUnit>();

        public void AddUnit(KomaUnit unit)
        {
            _unitList.Add(unit);
        }

        public void Remove(KomaUnit unit)
        {
            _unitList.Remove(unit);
        }
    }
}