using System.Collections.Generic;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
   public class PlayerHuman : Controller
   {
      public PlayerHuman(Game game, string name, int playerId, Dictionary<GameTag, int> tags) : base(game, name, playerId, tags)
      {

      }
   }
}
