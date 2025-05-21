using Trading.Backend.Models;

namespace Trading.Backend.Test
{
    public class BiddingTest
    {
        [Fact]
        public void Redeemd_All()
        {
            //arrange
            var store = new Store();
            var apple = store.Stocks["AAPL"];
            apple.Bids.Add(10, new BidCollection(10));
            var collection = apple.Bids[10];
            collection.Add(new Bid("test", 10, 100));
            collection.Add(new Bid("test", 10, 100));
            collection.Add(new Bid("test", 10, 100));

            //act
            var bids = collection.Take(250, out var unredeemed);

            //assert
            Assert.Equal(3,bids.Count);
            Assert.Equal(100, bids[0].Quantity);
            Assert.Equal(100, bids[1].Quantity);
            Assert.Equal(50, bids[2].Quantity);

            Assert.Equal(0, unredeemed);
        }

        [Fact]
        public void Unreadeemed()
        {
            //arrange
            var store = new Store();
            var apple = store.Stocks["AAPL"];
            apple.Bids.Add(10, new BidCollection(10));
            var collection = apple.Bids[10];
            collection.Add(new Bid("test", 10, 100));
            collection.Add(new Bid("test", 10, 100));
            collection.Add(new Bid("test", 10, 100));

            //act
            var bids = collection.Take(350, out var unredeemed);

            //assert
            Assert.Equal(3, bids.Count);
            Assert.Equal(100, bids[0].Quantity);
            Assert.Equal(100, bids[1].Quantity);
            Assert.Equal(100, bids[2].Quantity);

            Assert.Equal(50, unredeemed);
        }
    }
}
