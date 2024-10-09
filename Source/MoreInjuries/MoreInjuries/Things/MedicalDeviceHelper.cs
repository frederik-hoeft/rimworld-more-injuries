using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.Things;

public static class MedicalDeviceHelper
{
    internal const int MAX_MEDICAL_DEVICE_RESERVATIONS = 10;

    public static string? GetReasonForDisabledProcedure(Pawn doctor, Pawn patient, string jobTitle)
    {
        if (patient.playerSettings?.medCare is MedicalCareCategory.NoCare)
        {
            return $"{jobTitle}: medical care for {patient} disabled";
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return $"{jobTitle}: {doctor} is incapable of manipulation";
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
        {
            return $"{jobTitle}: {doctor} is blind";
        }
        if (doctor.WorkTagIsDisabled(WorkTags.Caring))
        {
            return $"{jobTitle}: {doctor} is incapable of caring";
        }
        if (doctor.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
        {
            return $"{jobTitle}: {doctor} is incapable of medicine";
        }
        return null;
    }

    public static int GetMedicalDeviceCountToFullyHeal(Pawn pawn, HediffDef[] hediffDefs)
    {
        int count = 0;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
        for (int i = 0; i < hediffs.Count; ++i)
        {
            for (int j = 0; j < hediffDefs.Length; ++j)
            {
                if (hediffs[i].def == hediffDefs[j])
                {
                    count++;
                    break;
                }
            }
        }
        return count;
    }

    private static Thing? GetThingFromInventory(ThingOwner<Thing> inventory, ThingDef thingDef)
    {
        if (inventory.Count == 0)
        {
            return null;
        }
        List<Thing> things = inventory.InnerListForReading;
        for (int index = 0; index < things.Count; ++index)
        {
            if (things[index].def == thingDef)
            {
                return things[index];
            }
        }
        return null;
    }

    private static bool CanDoctorGetDevice(Pawn doctor, Thing device) => !device.IsForbidden(doctor) && doctor.CanReserve(device, MAX_MEDICAL_DEVICE_RESERVATIONS, stackCount: 1);

    public static Thing? FindMedicalDevice(Pawn doctor, Pawn patient, ThingDef deviceDef, HediffDef[] hediffDefs)
    {
        if (patient.playerSettings?.medCare is MedicalCareCategory.NoCare)
        {
            return null;
        }
        if (GetMedicalDeviceCountToFullyHeal(patient, hediffDefs) <= 0)
        {
            return null;
        }
        Thing? thingInInventory = GetThingFromInventory(doctor.inventory.innerContainer, deviceDef);
        if (thingInInventory is Thing deviceInInventory)
        {
            return deviceInInventory;
        }
        Thing? thingOnMap = GenClosest.ClosestThing_Global_Reachable(
            center: patient.Position,
            map: patient.MapHeld,
            searchSet: patient.MapHeld.listerThings.ThingsOfDef(deviceDef),
            peMode: PathEndMode.ClosestTouch,
            traverseParams: TraverseParms.For(doctor),
            validator: thing => CanDoctorGetDevice(doctor, thing));
        if (thingOnMap is Thing deviceOnMap)
        {
            return deviceOnMap;
        }
        if (doctor.IsColonist && doctor.Map is not null)
        {
            foreach (Pawn colonyAnimal in doctor.Map.mapPawns.SpawnedColonyAnimals)
            {
                if (GetThingFromInventory(colonyAnimal.inventory.innerContainer, deviceDef) is Thing deviceInAnimalInventory)
                {
                    return deviceInAnimalInventory;
                }
            }
        }
        return null;
    }
}
