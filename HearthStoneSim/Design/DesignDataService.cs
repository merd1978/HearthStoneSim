using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using HearthStoneSim.Model;
using HearthStoneSim.Model.Enums;

namespace HearthStoneSim.Design
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
            var card1 = new Card() { Id = "AT_002", Name = "Тест1", CardTextInHand = "asd"};
            var card2 = new Card() { Id = "CS2_072", Name = "Тест2", CardTextInHand = "asd" };
            var card3 = new Card() { Id = "EX1_620", Name = "Тест3", CardTextInHand = "asd" };
            var card4 = new Card() { Id = "CS2_203", Name = "Тест4", CardTextInHand = "asd" };

            var cards = new Dictionary<string, Card>
            {
                {card1.Id, card1},
                {card2.Id, card2},
                {card3.Id, card3},
                {card4.Id, card4}
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