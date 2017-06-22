using System.Collections.Generic;
using HearthStoneSim.Model.Enums;

namespace HearthStoneSim.Model.GameCore
{
   public class Core : ICard
   {
      private readonly CoreData _data;
      //public int Id { get; set; }
      public string Id => _data.Card.Id;
      public string Name => _data.Card.Name;
      public string CardTextInHand =>_data.Card.CardTextInHand;
      public string ArtImageSource => _data.Card.ArtImageSource;
      public string FrameImageSource => _data.Card.FrameImageSource;
      public int Cost => _data.Card.Cost;
      public int Attack => _data.Card.Attack;
      public int PreDamage
      {
         get => this[GameTag.PREDAMAGE];
         set => this[GameTag.PREDAMAGE] = value;
      }
      public bool IsDamaged { get; set; }
      public bool IsDead { get; set; }

      public Zone Zone;

      public int this[GameTag t]
      {
         get => _data[t];
         set
         {
            // if (value < 0) value = 0;
            // Ignore unchanged data
            var oldValue = _data[t];
            if (value == oldValue) return;
            //Changing(t, oldValue, value);
            _data[t] = value;
            //Game?.CoreChanged(this, t, oldValue, value);
         }
      }

      public int Damage
      {
         get => this[GameTag.DAMAGE];
         set => this[GameTag.DAMAGE] = value;
      }

      public int Health
      {
         get => this[GameTag.HEALTH] - this[GameTag.DAMAGE];
         set
         {
            // Absolute change in health, removes damage dealt (eg. Equality)
            this[GameTag.HEALTH] = value;
            this[GameTag.DAMAGE] = 0;
         }
      }

      protected internal Core(Card card, Dictionary<GameTag, int> tags = null)
      {
         _data = new CoreData(card, tags);
      }

      // Cloning copy constructor
      protected internal Core(Core cloneFrom)
      {
         Zone = cloneFrom.Zone;
         _data = new CoreData(cloneFrom._data);
      }
   }
}
