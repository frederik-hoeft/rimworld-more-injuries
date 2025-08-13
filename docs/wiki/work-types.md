# New Work Types

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [New Work Types](/docs/wiki/work-types.md)_
<!-- @end_generated_block -->

More Injuries introduces several new automated work types for doctors to handle emergency situations and stabilize patients without the need for manual intervention. These new work types are designed to reduce the micromanagement required to treat patients with life-threatening conditions and allow the player to focus on managing the more severe injuries and medical conditions.

## Defibrillation

> **In-Game Label**
> _"Stabilize or resuscitate patients with arythmia or cardiac arrest"_

Assigned doctors will automatically attempt to defibrillate patients in hospital beds who are suffering from a [heart attack](https://rimworldwiki.com/wiki/Ailments#Heart_attack) or [cardiac arrest](#cardiac-arrest) during the `ventricular fibrillation` stage. The doctor will search for an accessible [defibrillator](#defibrillator) and start the defibrillation procedure to restore normal heart rhythm and save the patient's life.

**Parent Work Type[^4]**: `Doctor`  
**Priority In Type[^5]**: 125

## Airway Management

> **In-Game Label**
> _"Manage airways of patients with respiratory distress"_

Assigned doctors will automatically attempt to clear the airways of patients in hospital beds who are [choking on blood](#choking-on-blood). The doctor will search for an accessible [airway suction device](#airway-suction-device) and start the airway suction procedure to remove the obstruction and allow the patient to breathe normally again.

**Parent Work Type[^4]**: `Doctor`  
**Priority In Type[^5]**: 120

## Performing CPR

> **In-Game Label**
> _"Perform CPR to stabilize patients with cardiac or respiratory arrest"_

Assigned doctors will automatically start performing [CPR](#cpr) on patients in hospital beds who are suffering from [cardiac arrest](#cardiac-arrest), [heart attacks](https://rimworldwiki.com/wiki/Ailments#Heart_attack), or who are [choking on blood](#choking-on-blood). The doctor will attempt to stabilize the patient and restore normal heart rhythm and breathing to prevent further deterioration of the patient's condition.

**Parent Work Type[^4]**: `Doctor`  
**Priority In Type[^5]**: 115

## Blood Transfusions

> **In-Game Label**
> _"Perform a blood transfusion to stabilize patients with severe blood loss"_

Assigned doctors will automatically perform [blood transfusions](#blood-bag) on patients in hospital beds who are suffering from `severe` blood loss. The doctor will search for an accessible [blood bag](#blood-bag) and start the blood transfusion procedure to restore blood volume and prevent [hypovolemic shock](#hypovolemic-shock).

**Parent Work Type[^4]**: `Doctor`  
**Priority In Type[^5]**: 105

## Splinting Fractures

> **In-Game Label**
> _"Apply a splint to an injured patient to stabilize a fracture and promote healing"_

If enabled in the mod settings, assigned doctors will automatically apply a [splint](#splint) to patients in hospital beds who are suffering from a [bone fracture](#bone-fracture). The doctor will search for an accessible splint and start the splinting procedure to stabilize the fracture and promote healing over time. This work type is skipped if the patient is already scheduled to receive a different treatment for the fracture, such as [surgical repair](#osteosynthetic-surgery). This work type is lower priority than most other doctoring tasks.

**Parent Work Type[^4]**: `Doctor`
**Priority In Type[^5]**: 15

## Tourniquet Recovery

> **In-Game Label**
> _"Recover a tourniquet from a deceased patient to reuse it in the future"_

Unless having other tasks to perform, assigned doctors will recover [tourniquets](#tourniquet) from deceased patients for reuse in future treatments. This work type is a very low-priority task and will only be performed if no other tasks are available. The doctor will search for corpses with [tourniquets](#tourniquet) applied and remove the tourniquet from the deceased patient.

**Parent Work Type[^4]**: `Doctor`  
**Priority In Type[^5]**: 5

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#new-work-types)_
<!-- @end_generated_block -->
