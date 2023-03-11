using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

public class MechanoidDropInventoryGizmo : Command
{

    public MechanoidDropInventoryGizmo()
    {
        defaultLabel = "DMI_GizmoLabel".Translate();
        defaultDesc = "DMI_GizmoDescription".Translate();
        //Translation isn't working for some reason, so I'll just default to English for now.
        //defaultLabel = "Drop Inventory";
        //defaultDesc = "All selected mechanoids will drop the contents of their inventories on the ground.";
        icon = ContentFinder<Texture2D>.Get("UI/Commands/DropInventory");
        hotKey = KeyBindingDefOf.Misc9;
    }

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);

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

public class CompMechanoidDropInventory : ThingComp
{
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        yield return new MechanoidDropInventoryGizmo();
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
