using System;
using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
   internal class CoreData
   {
      public int Id { get; internal set; }
      public Card Card { get; }
      public Dictionary<GameTag, int> Tags { get; }

      public int this[GameTag t]
      {
         get
         {
            // Use the core tag if available, otherwise the card tag
            if (Tags.ContainsKey(t)) return Tags[t];
            return Card.Tags.ContainsKey(t) ? Card[t] : 0;
         }
         set => Tags[t] = value;
      }

      internal CoreData(Card card, Dictionary<GameTag, int> tags = null)
      {
         Card = card;
         Tags = tags ?? new Dictionary<GameTag, int>(Enum.GetNames(typeof(GameTag)).Length);
      }

      // Cloning copy constructor
      internal CoreData(CoreData cloneFrom)
      {
         Id = cloneFrom.Id;
         Card = cloneFrom.Card;
         Tags = new Dictionary<GameTag, int>(cloneFrom.Tags);
      }
   }
}
