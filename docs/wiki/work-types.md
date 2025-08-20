# Work Types

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [Work Types](/docs/wiki/work-types.md)_
<!-- @end_generated_block -->

More Injuries introduces several new automated work types for doctors to handle emergency situations and stabilize patients without the need for manual intervention. These new work types are designed to reduce the micromanagement required to treat patients with life-threatening conditions and allow the player to focus on managing the more severe injuries and medical conditions.

<!-- @generate_toc {"source": "$self", "indent": 2} -->
- [Work Types](#work-types)
  - [Defibrillation](#defibrillation)
  - [Airway Management](#airway-management)
  - [Performing CPR](#performing-cpr)
  - [Saline IV Infusions](#saline-iv-infusions)
  - [Blood Transfusions](#blood-transfusions)
  - [Splinting Fractures](#splinting-fractures)
  - [Tourniquet Recovery](#tourniquet-recovery)
<!-- @end_generated_block -->

## Defibrillation

> **In-Game Label**
> _"Stabilize or resuscitate patients with arrhythmia or cardiac arrest"_

Assigned doctors will automatically attempt to defibrillate patients in hospital beds who are suffering from a [heart attack](https://rimworldwiki.com/wiki/Ailments#Heart_attack) or [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest) during the `ventricular fibrillation` stage. The doctor will search for an accessible [defibrillator](/docs/wiki/medical-devices.md#defibrillator) and start the defibrillation procedure to restore normal heart rhythm and save the patient's life.

**Parent Work Type[^1]**: `Doctor`  
**Priority In Type[^2]**: 125

## Airway Management

> **In-Game Label**
> _"Manage airways of patients with respiratory distress"_

Assigned doctors will automatically attempt to clear the airways of patients in hospital beds who are [choking on blood](/docs/wiki/injuries/choking.md#choking-on-blood). The doctor will search for an accessible [airway suction device](/docs/wiki/medical-devices.md#airway-suction-device) and start the airway suction procedure to remove the obstruction and allow the patient to breathe normally again.

**Parent Work Type[^1]**: `Doctor`  
**Priority In Type[^2]**: 120

## Performing CPR

> **In-Game Label**
> _"Perform CPR to stabilize patients with cardiac or respiratory arrest"_

Assigned doctors will automatically start performing [CPR](/docs/wiki/medical-devices.md#cpr) on patients in hospital beds who are suffering from [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest), [heart attacks](https://rimworldwiki.com/wiki/Ailments#Heart_attack), or who are [choking on blood](/docs/wiki/injuries/choking.md#choking-on-blood). The doctor will attempt to stabilize the patient and restore normal heart rhythm and breathing to prevent further deterioration of the patient's condition.

**Parent Work Type[^1]**: `Doctor`  
**Priority In Type[^2]**: 115

## Saline IV Infusions

> **In-Game Label**
> _"Perform a saline IV infusion to stabilize patients with severe blood loss"_

Assigned doctors will automatically start administering [saline IV infusions](/docs/wiki/medical-devices.md#saline-iv-bag) to patients in hospital beds who are suffering from `severe` blood loss. The doctor will search for an accessible [saline IV bag](/docs/wiki/medical-devices.md#saline-iv-bag) and start the infusion procedure to restore fluid balance within the safety thresholds of [hemodilution](/docs/wiki/injuries/hemodilution.md#hemodilution) and prevent [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock).

**Parent Work Type[^1]**: `Doctor`  
**Priority In Type[^2]**: 106

> [!NOTE]
> Evaluating whether saline IV infusions are safe to perform requires the doctor to make a prognosis for the patient's condition, taking into account factors such as [coagulopathy](/docs/wiki/injuries/coagulopathy.md#coagulopathy) and [acidosis](/docs/wiki/injuries/acidosis.md#acidosis), which could lead to severe complications with [hemodilution](/docs/wiki/injuries/hemodilution.md#hemodilution) caused by saline infusions. You can customize the risk assessment process by adjusting the safety threshold (`Saline IV infusion safety threshold of independent coagulopathy`) in the mod settings, where values close to 0 will be highly restrictive, and values close to 1 will allow doctors to perform saline IV infusions, even in cases with high risk of complications.

## Blood Transfusions

> **In-Game Label**
> _"Perform a blood transfusion to stabilize patients with severe blood loss"_

Assigned doctors will automatically perform [blood transfusions](/docs/wiki/medical-devices.md#blood-bag) on patients in hospital beds who are suffering from `severe` blood loss. The doctor will search for an accessible [blood bag](/docs/wiki/medical-devices.md#blood-bag) and start the blood transfusion procedure to restore blood volume and prevent [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock).

**Parent Work Type[^1]**: `Doctor`  
**Priority In Type[^2]**: 105

## Splinting Fractures

> **In-Game Label**
> _"Apply a splint to an injured patient to stabilize a fracture and promote healing"_

If enabled in the mod settings, assigned doctors will automatically apply a [splint](/docs/wiki/medical-devices.md#splint) to patients in hospital beds who are suffering from a [bone fracture](/docs/wiki/injuries/fractures.md#bone-fracture). The doctor will search for an accessible splint and start the splinting procedure to stabilize the fracture and promote healing over time. This work type is skipped if the patient is already scheduled to receive a different treatment for the fracture, such as [surgical repair](/docs/wiki/surgeries.md#osteosynthetic-surgery). This work type is lower priority than most other doctoring tasks.

**Parent Work Type[^1]**: `Doctor`
**Priority In Type[^2]**: 15

## Tourniquet Recovery

> **In-Game Label**
> _"Recover a tourniquet from a deceased patient to reuse it in the future"_

Unless having other tasks to perform, assigned doctors will recover [tourniquets](/docs/wiki/medical-devices.md#tourniquet) from deceased patients for reuse in future treatments. This work type is a very low-priority task and will only be performed if no other tasks are available. The doctor will search for corpses with [tourniquets](/docs/wiki/medical-devices.md#tourniquet) applied and remove the tourniquet from the deceased patient.

**Parent Work Type[^1]**: `Doctor`  
**Priority In Type[^2]**: 5

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#work-types)_
<!-- @end_generated_block -->

[^1]: see [Work Types](https://rimworldwiki.com/wiki/Work#Work_types) on the RimWorld Wiki.

[^2]: Priority in Type indicates the priority of the work type within the parent work type. The higher the number, the higher the priority of the work type.
