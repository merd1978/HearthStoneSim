using System;
using System.Collections.Generic;

namespace HearthStoneSimCore.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }

        public void GetCardDefs(Action<Dictionary<string, Card>, Exception> callback)
        {

        }
    }
}