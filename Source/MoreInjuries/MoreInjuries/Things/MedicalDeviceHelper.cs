using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.Things;

public static class MedicalDeviceHelper
{
    internal const int MAX_MEDICAL_DEVICE_RESERVATIONS = 10;

    public static string? GetReasonForDisabledProcedure(Pawn doctor, Pawn patient, string jobTitleTranslationKey)
    {
        if (patient.playerSettings?.medCare is MedicalCareCategory.NoCare)
        {
            return "MI_ProcedureFailed_CareDisabled".Translate(jobTitleTranslationKey.Translate(), patient);
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return "MI_ProcedureFailed_IncapableOfManipulation".Translate(jobTitleTranslationKey.Translate(), doctor);
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
        {
            return "MI_ProcedureFailed_IncapableOfSight".Translate(jobTitleTranslationKey.Translate(), doctor);
        }
        if (doctor.WorkTagIsDisabled(WorkTags.Caring))
        {
            return "MI_ProcedureFailed_IncapableOfCaring".Translate(jobTitleTranslationKey.Translate(), doctor);
        }
        if (doctor.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
        {
            return "MI_ProcedureFailed_IncapableOfMedicine".Translate(jobTitleTranslationKey.Translate(), doctor);
        }
        if (doctor == patient && doctor.Faction == Faction.OfPlayer && doctor.playerSettings?.selfTend is false)
        {
            return "MI_ProcedureFailed_SelfTendDisabled".Translate(jobTitleTranslationKey.Translate());
        }
        return null;
    }

    public static int GetMedicalDeviceCountToFullyHeal(Pawn pawn, Predicate<Hediff> isTreatableWithDevice)
    {
        int count = 0;
        List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
        for (int i = 0; i < hediffs.Count; ++i)
        {
            if (isTreatableWithDevice(hediffs[i]))
            {
                count++;
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

    public static Thing? FindMedicalDevice(Pawn doctor, Pawn patient, ThingDef deviceDef, HediffDef[] hediffDefs, bool fromInventoryOnly = false) =>
        FindMedicalDevice(doctor, patient, deviceDef, hediff => Array.IndexOf(hediffDefs, hediff.def) != -1, fromInventoryOnly);

    public static Thing? FindMedicalDevice(Pawn doctor, Pawn patient, ThingDef deviceDef, Predicate<Hediff>? isTreatableWithDevice = null, bool fromInventoryOnly = false)
    {
        if (patient.playerSettings?.medCare is MedicalCareCategory.NoCare)
        {
            return null;
        }
        if (isTreatableWithDevice is not null && GetMedicalDeviceCountToFullyHeal(patient, isTreatableWithDevice) <= 0)
        {
            return null;
        }
        Thing? thingInInventory = GetThingFromInventory(doctor.inventory.innerContainer, deviceDef);
        if (thingInInventory is Thing deviceInInventory)
        {
            return deviceInInventory;
        }
        if (!fromInventoryOnly)
        {
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
        }
        return null;
    }
}
