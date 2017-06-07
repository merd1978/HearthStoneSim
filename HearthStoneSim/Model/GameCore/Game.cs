namespace HearthStoneSim.Model.GameCore
{
   public class Game
   {
      public Player Player1 { get; private set; }
      public Player Player2 { get; private set; }

      public Game()
      {
         var deck1 = new Deck();
         deck1.Add(new Card(Cards.All["EX1_306"]));
         deck1.Add(new Card(Cards.All["CS2_172"]));
         deck1.Add(new Card(Cards.All["CS2_124"]));
         deck1.Add(new Card(Cards.All["CS2_182"]));
         deck1.Add(new Card(Cards.All["CS2_222"]));
         deck1.Add(new Card(Cards.All["OG_279"]));
         Player1 = new Player(deck1);
         Player1.Hand.Add(new Card(Cards.All["EX1_306"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_172"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_124"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_182"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_222"]));
         Player1.Hand.Add(new Card(Cards.All["OG_279"]));
      }

      public void Attack(int indexPlayer1, int indexPlayer2)
      {
         Player1.Board.Cards[0].Damage += 1;
      }
   }
}
