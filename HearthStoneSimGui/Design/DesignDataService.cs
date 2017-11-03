using System;
using System.Collections.Generic;
using HearthStoneSimCore.Model;

namespace HearthStoneSimGui.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }

        public void GetCardDefs(Action<Dictionary<string, Card>, Exception> callback)
        {
            var card1 = new Card() { CardId = "EX1_306", Name = "Тест1", Text = "asd"};
            var card2 = new Card() { CardId = "CS2_172", Name = "Тест2", Text = "asd" };
            var card3 = new Card() { CardId = "CS2_124", Name = "Тест3", Text = "asd" };
            var card4 = new Card() { CardId = "CS2_182", Name = "Тест4", Text = "asd" };
            var card5 = new Card() { CardId = "CS2_222", Name = "Тест5", Text = "asd" };
            var card6 = new Card() { CardId = "OG_279", Name = "Тест6", Text = "asd" };

            var cards = new Dictionary<string, Card>
            {
                {card1.CardId, card1},
                {card2.CardId, card2},
                {card3.CardId, card3},
                {card4.CardId, card4},
                {card5.CardId, card5},
                {card6.CardId, card6}
            };
            //var cards = new Dictionary<string, Card>()
            //{
            //    [card2.Id] = card1,
            //    [card2.Id] = card2,
            //    [card3.Id] = card3,
            //    [card4.Id] = card4
            //};
            callback(cards, null);
        }
    }
}