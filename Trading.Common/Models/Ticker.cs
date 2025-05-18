namespace Trading.Common.Models
{
    public class Ticker
    {
        public int Id { get; set; } 
        private float _price;
        public string Symbol { get; init; }
        public float Price 
        {
            get { return _price; }
            set
            {
                if(_price==0)
                {
                    _price = value;
                }
                else
                {
                    Changes = value - _price;
                    ChangePercent = Changes / _price * 100;
                    _price = value;
                }       
            }
        }
        public float Changes { get; private set; }
        public float ChangePercent { get; private set; }
    }
}
