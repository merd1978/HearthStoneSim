using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using HearthStoneSimCore.Enums;

namespace HearthStoneSimCore.Model
{
    public static class Cards
    {
        public static Dictionary<string, Card> All;

        public static Card FromName(string cardName)
        {
            return All.FirstOrDefault(x => x.Value.Name == cardName).Value;
        }

        public static Dictionary<string, Card> Load()
        {
            // Get XML definitions from assembly embedded resource
            var assembly = Assembly.GetExecutingAssembly();
            var def = XDocument.Load(assembly.GetManifestResourceStream("HearthStoneSimCore.Model.CardDefs.xml"));

            // Parse XML
            var cards = (from r in def.Descendants("Entity")
                         select new
                         {
                             Id = r.Attribute("CardID").Value,
                             // Unfortunately the file contains some duplicate tags
                             // so we have to make a list first and weed out the unique ones
                             Tags = (from tag in r.Descendants("Tag")
                                     select new Tag((GameTag)Enum.Parse(typeof(GameTag), tag.Attribute("enumID").Value),
                                         tag.Attribute("value") != null ?
                                             (TagValue)int.Parse(tag.Attribute("value").Value) : (tag.Attribute("type").Value == "String" ?
                                                 (TagValue)tag.Value : (tag.Attribute("type").Value == "LocString" ?
                                                     tag.Element("ruRU") != null ?
                                                         (TagValue)tag.Element("ruRU").Value : (TagValue)tag.Element("enUS").Value : (TagValue)0
                                                 )))).ToList(),

                             Requirements = (from req in r.Descendants("PlayRequirement")
                                             select new
                                             {
                                                 Req = (PlayRequirements)Enum.Parse(typeof(PlayRequirements), req.Attribute("reqID").Value),
                                                 Param = (req.Attribute("param").Value != "" ? int.Parse(req.Attribute("param").Value) : 0)
                                             }).ToDictionary(x => x.Req, x => x.Param),

                             Entourage = (from ent in r.Descendants("EntourageCard")
                                          select ent.Attribute("cardID").Value).ToList()
                         }).ToList();

            // Build card database
            var cardsDict = new Dictionary<string, Card>();

            foreach (var card in cards)
            {
                var c = new Card()
                {
                    Id = card.Id,
                    Tags = new Dictionary<GameTag, int>(),
                    Requirements = card.Requirements
                };
                // Get unique int and bool tags, ignore duplicate and string tags
                foreach (var tag in card.Tags)
                {
                    if (c.Tags.ContainsKey(tag.Name))
                        continue;
                    if (tag.Value.HasIntValue)
                    {
                        c.Tags.Add(tag.Name, tag.Value);
                    }
                    else if (tag.Value.HasBoolValue)
                    {
                        c.Tags.Add(tag.Name, tag.Value ? 1 : 0);
                    }
                    else if (tag.Value.HasStringValue)
                    {
                        if (tag.Name == GameTag.CARDNAME)
                            c.Name = tag.Value;
                        if (tag.Name == GameTag.CARDTEXT_INHAND)
                            c.CardTextInHand = tag.Value;
                    }
                }
                cardsDict.Add(c.Id, c);
            }
            return cardsDict;
        }

        static Cards()
        {
            Cards.All = Load();
        }
    }
}
