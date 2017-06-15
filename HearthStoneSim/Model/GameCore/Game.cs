using HearthStoneSim.Model.Enums;

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
         Player2 = new Player(deck1);
         Player1.Hand.Add(new Card(Cards.All["EX1_306"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_172"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_124"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_182"]));
         Player1.Hand.Add(new Card(Cards.All["CS2_222"]));
         Player1.Hand.Add(new Card(Cards.All["OG_279"]));
         Player1.Board.Add(new Core(Cards.All["EX1_306"]));
         Player2.Board.Add(new Core(Cards.All["EX1_306"]));
      }

      public void Attack(int indexPlayer1, int indexPlayer2)
      {
         Core source = Player1.Board.Cards[indexPlayer1];
         Core target = Player2.Board.Cards[indexPlayer2];
         target.Damage += source.Attack;
         source.Damage += target.Attack;

         target.PreDamage = source.Attack;
         target.IsDamaged = true;
      }

      public void PlayMinion(int cardIndex, int insertIndex)
      {
         var card = Player1.Hand.Cards[cardIndex];
         Player1.Board.Insert(new Core(card), insertIndex);
         //Player1.Hand.Cards.RemoveAt(indexCard);
      }
   }
}
