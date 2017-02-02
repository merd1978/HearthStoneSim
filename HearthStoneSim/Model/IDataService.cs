using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HearthStoneSim.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
        void GetCardDefs(Action<List<Card>, Exception> callback);
    }
}
