namespace RtShogi.Scripts.Battle
{
    public struct KomaId
    {
        private int _value;
        public int Value => _value;

        public static readonly KomaId InvalidId = new KomaId(0); 

        public KomaId(int value)
        {
            _value = value;
        }

        /// <summary>
        /// ルームのホストかどうかで分岐して発行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isRoomHost"></param>
        /// <returns></returns>
        public static KomaId PublishId(int id, bool isRoomHost)
        {
            return isRoomHost ? new KomaId(id) : new KomaId(-id);
        }

        public bool IsSame(KomaId id)
        {
            return id.Value == this.Value;
        }
    }
}