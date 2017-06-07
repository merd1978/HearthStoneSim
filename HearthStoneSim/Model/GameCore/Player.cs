namespace HearthStoneSim.Model.GameCore
{
   public class Player
   {
      public Deck Deck { get; private set; }
      public Hand Hand { get; private set; }
      public Board Board { get; private set; }
      public string FriendlyName { get; }

      public Player(Deck deck)
      {
         Deck = deck;
         Hand = new Hand();
         Board = new Board();
      }
   }
}
