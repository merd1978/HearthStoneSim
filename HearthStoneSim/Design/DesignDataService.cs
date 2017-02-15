using System;
using System.Collections.Generic;
using HearthStoneSim.Model;

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
            var card = new Card() {Id="AT_002", Name="Тест"};
            var cards = new Dictionary<string, Card>()
            {
                {card.Id, card}
            };
            callback(cards, null);
        }
    }
}