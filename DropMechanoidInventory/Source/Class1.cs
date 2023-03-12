using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

public class CompMechanoidDropInventory : ThingComp
{
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (this.parent.Faction == Faction.OfPlayer)
        {
            yield return new Command_Action
            {
                action = () =>
                {
                    DropInventory();
                },
                defaultLabel = "DMI_GizmoLabel".Translate(),
                defaultDesc = "DMI_GizmoDescription".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/DropInventory"),
                hotKey = KeyBindingDefOf.Misc9,
            };
        }
    }

    private void DropInventory()
    {
        if (this.parent.Faction != Faction.OfPlayer)
        {
            return;
        }

        List<Pawn> selectedMechanoids = new List<Pawn>();

        foreach (Thing thing in Find.Selector.SelectedObjects)
        {
            Pawn pawn = thing as Pawn;

            if (pawn != null && pawn.def.race.IsMechanoid)
            {
                selectedMechanoids.Add(pawn);
            }
        }

        if (selectedMechanoids.Count > 0)
        {
            foreach (Pawn pawn in selectedMechanoids)
            {
                ThingOwner<Thing> inventory = pawn.inventory.innerContainer;


                List<Thing> thingsToDrop = new List<Thing>();

                foreach (Thing thing in inventory)
                {
                    thingsToDrop.Add(thing);
                }

                foreach (Thing thing in thingsToDrop)
                {
                    inventory.Remove(thing);
                    GenPlace.TryPlaceThing(thing, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                }
            }
        }
    }
}

[StaticConstructorOnStartup]
public static class Startup
{
    static Startup()
    {
        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.category == ThingCategory.Pawn && thingDef.race.IsMechanoid)
            {
                thingDef.comps.Add(new CompProperties
                {
                    compClass = typeof(CompMechanoidDropInventory)
                });
            }
        }
    }
}