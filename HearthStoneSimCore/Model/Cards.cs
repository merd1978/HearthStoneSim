using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public static class Cards
    {
        /// <summary>
        /// The cards container
        /// </summary>
        private static Dictionary<string, Card> AllCards { get; }

        /// <summary>
        /// All known cards
        /// </summary>
        public static IEnumerable<Card> All => AllCards.Values;

        /// <summary>
        /// Returns the count of all known cards.
        /// </summary>
        public static int Count => AllCards.Count;

        /// <summary>
        /// All cards belonging to the Standard set.
        /// </summary>
        public static ReadOnlyCollection<Card> AllStandard { get; }

        /// <summary>
        /// Retrieves all standard cards ordered by card class.
        /// </summary>
        public static Dictionary<CardClass, IReadOnlyList<Card>> Standard { get; } = new Dictionary<CardClass, IReadOnlyList<Card>>();

        public static Card FromName(string cardName)
        {
            return AllCards.FirstOrDefault(x => x.Value.Name == cardName).Value;
        }

	    public static Card FromCardId(string cardId)
	    {
		    return AllCards[cardId];
	    }

        /// <summary>
        /// Parsing cards definition from CardDefs.xml
        /// </summary>
        public static Card[] Load()
        {
            // Get XML definitions from assembly embedded resource
            var assembly = Assembly.GetExecutingAssembly();
            var cardDefsXml = XDocument.Load(assembly.GetManifestResourceStream("HearthStoneSimCore.Model.CardDefs.xml"));

            // Parse XML
            var cardDefs = (from r in cardDefsXml.Descendants("Entity")
                select new
                {
                    Id = r.Attribute("CardID")?.Value,
                    AssetId = r.Attribute("ID")?.Value,
                    // Unfortunately the file contains some duplicate tags
                    // so we have to make a list first and weed out the unique ones
                    Tags = (from tag in r.Descendants("Tag")
                            select new Tag((GameTag)Enum.Parse(typeof(GameTag), tag.Attribute("enumID").Value),
                                tag.Attribute("value") != null ?
                                    (TagValue)int.Parse(tag.Attribute("value").Value) : tag.Attribute("type").Value == "String" ?
                                        (TagValue)tag.Value : tag.Attribute("type").Value == "LocString" ?
                                            tag.Element("enUS") != null ?
                                                (TagValue)tag.Element("enUS").Value : (TagValue)tag.Element("ruRU").Value : (TagValue)0
                                )).ToArray(),

                    Requirements = (from req in r.Descendants("PlayRequirement")
                                    select new
                                    {
                                        Req = (PlayReq)Enum.Parse(typeof(PlayReq), req.Attribute("reqID").Value),
                                        Param = (req.Attribute("param").Value != "" ? int.Parse(req.Attribute("param").Value) : 0)
                                    }).Distinct() // avoiding duplicate playrequirment
                                    .ToDictionary(x => x.Req, x => x.Param),

                    Entourage = (from ent in r.Descendants("EntourageCard")
                                 select ent.Attribute("cardID")?.Value).ToArray(),

                    ReferencedTag = (from rtag in r.Descendants("ReferencedTag")
                                     select new Tag((GameTag)Enum.Parse(typeof(GameTag), rtag.Attribute("enumID").Value),
                                         rtag.Attribute("value") != null ?
                                             (TagValue)int.Parse(rtag.Attribute("value").Value)
                                             : rtag.Attribute("type").Value == "String" ? (TagValue)rtag.Value
                                                 : rtag.Attribute("type").Value == "LocString" ?
                                                     (TagValue)rtag.Element("ruRU").Value : (TagValue)0
                                         )).ToArray()
                }).ToArray();

            // Build card database
            var cards = new Card[cardDefs.Length];
            for (int i = 0; i < cardDefs.Length; i++)
            {
                // Skip PlaceholderCard etc.
                //if (!dbfCards.ContainsKey(card.Id))
                //    continue;
                var card = cardDefs[i];
                cards[i] = new Card(card.Id, Int32.Parse(card.AssetId), card.Tags,
                    card.Requirements, card.Entourage, card.ReferencedTag);
            }
            return cards;
        }

        /// <summary>
        /// Returns the default hero class card.
        /// ex; Returns Grommash for the WARRIOR class.
        /// </summary>
        /// <param name="cardClass"></param>
        /// <returns></returns>
        public static Card HeroCard(CardClass cardClass)
        {
            switch (cardClass)
            {
                case CardClass.DRUID:
                    return AllCards["HERO_06"];
                case CardClass.HUNTER:
                    return AllCards["HERO_05"];
                case CardClass.MAGE:
                    return AllCards["HERO_08"];
                case CardClass.PALADIN:
                    return AllCards["HERO_04"];
                case CardClass.PRIEST:
                    return AllCards["HERO_09"];
                case CardClass.ROGUE:
                    return AllCards["HERO_03"];
                case CardClass.SHAMAN:
                    return AllCards["HERO_02"];
                case CardClass.WARLOCK:
                    return AllCards["HERO_07"];
                case CardClass.WARRIOR:
                    return AllCards["HERO_01"];
                default:
                    throw new NotImplementedException();
            }
        }

        static Cards()
        {
            // Fetch all cards.
            Card[] cards = Load();

            // Set cards (without behaviours)
            AllCards = (from c in cards select new { Key = c.Id, Value = c }).ToDictionary(x => x.Key, x => x.Value);

            //fill standart dictionary
            Enum.GetValues(typeof(CardClass)).Cast<CardClass>().ToList().ForEach(heroClass =>
            {
                Standard.Add(heroClass, All.Where(c =>
                    c.Collectible &&
                    (c.Class == heroClass ||
                     c.Class == CardClass.NEUTRAL && c.MultiClassGroup == 0 ||
                     c.MultiClassGroup == 1 && (c.Class == CardClass.NEUTRAL || c.Class == CardClass.HUNTER || c.Class == CardClass.PALADIN || c.Class == CardClass.WARRIOR) ||
                     c.MultiClassGroup == 2 && (c.Class == CardClass.NEUTRAL || c.Class == CardClass.DRUID || c.Class == CardClass.ROGUE || c.Class == CardClass.SHAMAN) ||
                     c.MultiClassGroup == 3 && (c.Class == CardClass.NEUTRAL || c.Class == CardClass.MAGE || c.Class == CardClass.PRIEST || c.Class == CardClass.WARLOCK)) &&
                    c.Type != CardType.HERO && StandardSets.Contains(c.Set)).ToList().AsReadOnly());
                //Log.Debug($"-> [{heroClass}] - {Standard[heroClass].Count} cards.");
            });

            //Log.Debug("AllStandard:");
            AllStandard = All.Where(c => c.Collectible && c.Type != CardType.HERO && StandardSets.Contains(c.Set)).ToList().AsReadOnly();

            // Add Powers

            // Add enchants
            //CoreCardsGen.Load();
        }

        /// <summary>
        /// Specifies which card sets combine into the STANDARD set.
        /// </summary>
        public static CardSet[] StandardSets { get; } = {

            CardSet.CORE,
            //CardSet.EXPERT1,
            //CardSet.UNGORO,
            //CardSet.ICECROWN,
            //CardSet.LOOTAPALOOZA,
            //CardSet.GILNEAS,
            //CardSet.BOOMSDAY,
            //CardSet.TROLL
        };
    }
}
