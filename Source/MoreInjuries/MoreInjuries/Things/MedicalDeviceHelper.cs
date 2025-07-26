using MoreInjuries.Localization;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MoreInjuries.Things;

public static class MedicalDeviceHelper
{
    internal const int MAX_MEDICAL_DEVICE_RESERVATIONS = 10;

    public static bool CanReachPatient(this Pawn doctor, Pawn patient, PathEndMode pathEndMode = PathEndMode.ClosestTouch, Danger maxDanger = Danger.Deadly)
    {
        if (doctor == patient)
        {
            return true;
        }
        return doctor.CanReach(patient, pathEndMode, maxDanger);
    }

    public static DisabledProcedureCause? GetCauseForDisabledProcedure(Pawn doctor, Pawn patient, string jobTitleTranslationKey, bool ignoreSelfTendSetting = false)
    {
        if (patient.playerSettings?.medCare is MedicalCareCategory.NoCare)
        {
            return DisabledProcedureCause.Soft(Named.Keys.ProcedureFailed_CareDisabled.Translate(jobTitleTranslationKey.Translate(), patient.Named(Named.Params.PATIENT)));
        }
        if (!ignoreSelfTendSetting 
            && doctor == patient && doctor.Faction == Faction.OfPlayer && doctor.playerSettings?.selfTend is false)
        {
            return DisabledProcedureCause.Soft(Named.Keys.ProcedureFailed_SelfTendDisabled.Translate(jobTitleTranslationKey.Translate()));
        }
        if (!doctor.CanReachPatient(patient))
        {
            return DisabledProcedureCause.Hard(Named.Keys.ProcedureFailed_NoPath.Translate(jobTitleTranslationKey.Translate()));
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
        {
            return DisabledProcedureCause.Hard(Named.Keys.ProcedureFailed_IncapableOfManipulation.Translate(jobTitleTranslationKey.Translate(), doctor.Named(Named.Params.DOCTOR)));
        }
        if (!doctor.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
        {
            return DisabledProcedureCause.Hard(Named.Keys.ProcedureFailed_IncapableOfSight.Translate(jobTitleTranslationKey.Translate(), doctor.Named(Named.Params.DOCTOR)));
        }
        if (doctor.WorkTagIsDisabled(WorkTags.Caring))
        {
            return DisabledProcedureCause.Hard(Named.Keys.ProcedureFailed_IncapableOfCaring.Translate(jobTitleTranslationKey.Translate(), doctor.Named(Named.Params.DOCTOR)));
        }
        if (doctor.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
        {
            return DisabledProcedureCause.Hard(Named.Keys.ProcedureFailed_IncapableOfMedicine.Translate(jobTitleTranslationKey.Translate(), doctor.Named(Named.Params.DOCTOR)));
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

    public readonly record struct DisabledProcedureCause(string FailureReason, bool IsSoftFailure)
    {
        public static DisabledProcedureCause Soft(string failureReason) => new(failureReason, IsSoftFailure: true);

        public static DisabledProcedureCause Hard(string failureReason) => new(failureReason, IsSoftFailure: false);
    }
}