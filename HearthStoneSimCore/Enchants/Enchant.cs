using System.Collections.Generic;
using HearthStoneSimCore.Enums;
using HearthStoneSimCore.Model;

namespace HearthStoneSimCore.Enchants
{
    public class Enchant
    {
        /// <summary> Gets or sets the <see cref="GameTag"/>s which are affected by this enchant.
        /// </summary>
        /// <value> The effects. </value>
        public Dictionary<GameTag, int> Effects { get; set; } = new Dictionary<GameTag, int>();

        /// <summary> Applies this enchant on the specified entity and tag.
        ///	This method will process the stored data and apply it to the provided tag value, when applicable.
        /// </summary>
        /// <param name="target"> The entity subject.</param>
        /// <param name="gameTag"> The game tag which must change.</param>
        /// <param name="value"> The value of the corresponding tag. This value is either original (native) or
        /// could already be the result of another enchant.</param>
        /// <returns> An updated value for the specified tag.</returns>
        public int Apply(Core target, GameTag gameTag, int value)
        {
            int effect = Effects[gameTag];
            int result = value + effect;
            //Game.Log(LogLevel.DEBUG, BlockType.TRIGGER, "Enchant", $"Card[ind.{target?.OrderOfPlay}.{target}] got enchanted. {gameTag} = {value} + {Effects[gameTag]}");
            return result;
        }
    }
}
