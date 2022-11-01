﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CryingBuffalo.RandomEvents.Events.CCEvents
{
    public class MassGrave : BaseEvent
    {
        private const string EventTitle = "The Mass Grave";

        // The letters correspond to the inquiry element ids
        private readonly int moraleLossA = 2;
        private readonly int moraleLossB = 4;
        private readonly int moraleLossC = 4;
        private readonly int moraleLossD = 5;
        
        private readonly int minSoldiers;
        private readonly int maxSoldiers;
        private readonly int minBodies;
        private readonly int maxBodies;
        

        public MassGrave() : base(Settings.ModSettings.RandomEvents.MassGraveData)
        {
            minSoldiers = Settings.ModSettings.RandomEvents.MassGraveData.minSoldiers;
            maxSoldiers = Settings.ModSettings.RandomEvents.MassGraveData.maxSoldiers;
            minBodies = Settings.ModSettings.RandomEvents.MassGraveData.minBodies;
            maxBodies = Settings.ModSettings.RandomEvents.MassGraveData.maxBodies;
        }

        public override void CancelEvent()
        {
        }

        public override bool CanExecuteEvent()
        {
            
            return true;
        }

        public override void StartEvent()
        {
            if (Settings.ModSettings.GeneralSettings.DebugMode)
            {
                InformationManager.DisplayMessage(new InformationMessage($"Starting {randomEventData.eventType}",
                    RandomEventsSubmodule.TextColor));
            }
            
            var settlements = Settlement.FindAll(s => s.IsTown || s.IsCastle || s.IsVillage ).ToList();
            var closestSettlement = settlements.MinBy(s => MobileParty.MainParty.GetPosition().DistanceSquared(s.GetPosition()));
            

            var soldiersDiscovery = MBRandom.RandomInt(minSoldiers, maxSoldiers);
            var bodiesInGrave = MBRandom.RandomInt(minBodies, maxBodies);
            
            var inquiryElements = new List<InquiryElement>
            {
                new InquiryElement("a", "Make individual graves", null, true, "They should be given a proper burial"),
                new InquiryElement("b", "Fill the grave with dirt", null, true, "The least you can do is fill the hole"),
                new InquiryElement("c", "Burn the bodies", null, true, "Quickest and easiest way"),
                new InquiryElement("d", "Leave them", null, true, "Not your problem")
            };


            var msid = new MultiSelectionInquiryData(
                EventTitle,
                $"Your party has set up camp near {closestSettlement} and you have sent out some men to gather resources and hunt. Out of the blue {soldiersDiscovery} of your men come back and tell you " +
                "that there is something you need to see. You join your men as they escort you to whatever it is they want to show you.\n" +
                "When you arrive your are shocked to see a fresh mass grave filled with men, women and children. Your men asks you what they should do.",
                inquiryElements,
                false,
                1,
                "Choose",
                null,
                elements =>
                {
                    switch ((string)elements[0].Identifier)
                    {
                        case "a":
                            InformationManager.ShowInquiry(
                                new InquiryData(EventTitle,
                                    "You order two of your men to go back to camp and fetch shovels, linen and more men while you and the others start to remove the bodies from the grave. You lay the bodies next " +
                                    "to each other in neat lines. Whenever there is a child brought up from the grave you feel a great sadness and you cannot help hold back a few tears. Several of your " +
                                    "men weep as well and you feel the mood is very dark. \n" +
                                    $"Your men return with the requested supplies and additional men. Some start to dig graves while others wrap bodies in linen. In total you pull {bodiesInGrave} bodies from the mass grave. " +
                                    "After spending several hours digging and burying you are finally done just before nightfall. You and your men return to camp and decided to sit around the campfire " +
                                    "discussing your feelings after the today's events.",
                                    true, false, "Done", null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= moraleLossA;
                            MobileParty.MainParty.MoraleExplained.Add(-moraleLossA, new TaleWorlds.Localization.TextObject("Random Event"));
                            
                            break;
                        case "b":
                            InformationManager.ShowInquiry(
                                new InquiryData(EventTitle,
                                    "You order two of your men to go back to camp and fetch shovels and more men while you and the others start to fill the hole. After a few minutes men with shovels join in. " +
                                    "You end up digging for a few hours until the grave is filled. As a group you decide to recite some prayers before leaving.",
                                    true, false, "Done", null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= moraleLossB;
                            MobileParty.MainParty.MoraleExplained.Add(-moraleLossB, new TaleWorlds.Localization.TextObject("Random Event"));
                            
                            break;
                        case "c":
                            InformationManager.ShowInquiry(
                                new InquiryData(EventTitle,
                                    "You order your men to start the burning of the bodies. It doesn't take long for them to burn. When you return after a few hours only ash and bone are left in the grave.",
                                    true, false, "Done", null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= moraleLossC;
                            MobileParty.MainParty.MoraleExplained.Add(-moraleLossC, new TaleWorlds.Localization.TextObject("Random Event"));
                            
                            break;
                        case "d":
                            InformationManager.ShowInquiry(
                                new InquiryData(EventTitle,
                                    "You decide to just leave the area as it is. You tell your men they can handle this in any way they want. Later that evening your men return having buried the bodies.",
                                    true, false, "Done", null, null, null), true);
                            
                            MobileParty.MainParty.RecentEventsMorale -= moraleLossD;
                            MobileParty.MainParty.MoraleExplained.Add(-moraleLossD, new TaleWorlds.Localization.TextObject("Random Event"));
                            
                            break;
                        default:
                            MessageBox.Show($"Error while selecting option for \"{randomEventData.eventType}\"");
                            break;
                    }
                },
                null);
            
            MBInformationManager.ShowMultiSelectionInquiry(msid, true);


            StopEvent();
        }

        private void StopEvent()
        {
            try
            {
                onEventCompleted.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error while stopping \"{randomEventData.eventType}\" event :\n\n {ex.Message} \n\n {ex.StackTrace}");
            }
        }
    }


    public class MassGraveData : RandomEventData
    {
        public readonly int minSoldiers;
        public readonly int maxSoldiers;
        public readonly int minBodies;
        public readonly int maxBodies;

        public MassGraveData(string eventType, float chanceWeight, int minSoldiers, int maxSoldiers, int minBodies, int maxBodies) : base(eventType,
            chanceWeight)
        {
            this.minSoldiers = minSoldiers;
            this.maxSoldiers = maxSoldiers;
            this.minBodies = minBodies;
            this.maxBodies = maxBodies;
        }

        public override BaseEvent GetBaseEvent()
        {
            return new MassGrave();
        }
    }
}