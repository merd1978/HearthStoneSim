using System.Collections.Generic;
using HearthStoneSim.Model.GameCore;

namespace HearthStoneSim.Model
{
   public class Board
   {
      public List<Core> Cards = new List<Core>();
      public int MaxSize { get; set; } = 7;

      public void Add(Core card)
      {
         Cards.Add(card);
      }
   }
}
