using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
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

        public void DeathProcessingAndAuraUpdate()
        {
            //check for dead weapons, minions, heroes
            //GraveYard();

            //enable enchants and trigers
            //AuraUpdate();

            //while (TaskQueue.Count > 0)
            //{
            //    if (TaskQueue.Process() != TaskState.COMPLETE)
            //    {
            //        //Log(LogLevel.INFO, BlockType.PLAY, "Game", "Something really bad happend during proccessing, please analyze!");
            //    }
            //    //GraveYard();

            //    //AuraUpdate();
            //}
        }

        public void Attack(int indexPlayer1, int indexPlayer2)
        {
            Core source = Player1.Board.Cards[indexPlayer1];
            Core target = Player2.Board.Cards[indexPlayer2];

            foreach (var card in Player2.Board.Cards)
            {
                card.IsDamaged = false;
            }

            target.Damage += source.Attack;
            if (target.Health < 1) target.IsDead = true;
            source.Damage += target.Attack;
            if (source.Health < 1) source.IsDead = true;

            target.PreDamage = -source.Attack;
            target.IsDamaged = true;
        }

        public void PlayMinion(int cardIndex, int insertIndex)
        {
            var card = Player1.Hand.Cards[cardIndex];
            Player1.Board.Insert(new Core(card), insertIndex);
            //Player1.Hand.Cards.RemoveAt(indexCard);
        }

        public void ClearPreDamage()
        {
            foreach (var card in Player1.Board.Cards)
            {
                card.PreDamage = 0;
                card.IsDamaged = false;
            }
            foreach (var card in Player2.Board.Cards)
            {
                card.PreDamage = 0;
                card.IsDamaged = false;
            }
        }

    }
}
