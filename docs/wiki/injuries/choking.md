# Choking

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [Injuries and Medical Conditions A-Z](/docs/wiki/injuries/README.md) :arrow_right: [Choking](/docs/wiki/injuries/choking.md)_
<!-- @end_generated_block -->

Choking is a medical emergency that occurs when a foreign object becomes lodged in the throat or windpipe, blocking the flow of air. It can be a life-threatening situation that requires immediate intervention to clear the airway and restore breathing.

## Choking on Blood

> **In-Game Description**
> _"**Choking on blood** &mdash; Blood from traumatic injuries being aspirated into the airways triggers coughing and causes suffocation. The bleeding must be stopped and the airways cleared to prevent death. If the patient is conscious, they may be able to cough up the blood on their own and clear the airway. Otherwise, the airways must be cleared using a specialized airway suction device or by compressing the chest using CPR to expel the blood and restore breathing."_

```mermaid
---
config:
  flowchart:
    htmlLabels: true
---
flowchart LR
  bleeding[bleeding airway injury] ==> choking_on_blood[choking on blood]
  choking_on_blood ==> | severity = 1 | death[death]

  linkStyle 0,1 stroke: #b10000
  style choking_on_blood stroke-width: 4px
```

*See the section on the [pathophysiological system](/docs/wiki/pathophysiological-system.md#pathophysiological-system) for more information on the graphical representation.*

**Causes**: Severe injuries to the mouth, throat, or chest that cause bleeding into the airways.

**Effects**: Coughing, suffocation, loss of consciousness, and death if not treated immediately.

**Treatment**: Stopping the bleeding of the mouth, throat, or chest will prevent further blood from entering the airways and slow the progression of the condition. If the patient is conscious, they may be able to cough up the blood on their own and clear the airway. If the patient is unconscious, the airways must be cleared using a specialized [airway suction device](/docs/wiki/medical-devices.md#airway-suction-device) or by compressing the chest using [CPR](/docs/wiki/medical-devices.md#cpr) to expel the blood and restore breathing. To unlock [CPR](/docs/wiki/medical-devices.md#cpr), you must first complete the [cardiopulmonary resuscitation (CPR)](/docs/wiki/research.md#cardiopulmonary-resuscitation-cpr) research project.

> [!NOTE]
> **Biotech DLC**: Deathresting sanguaphages will always be able to cough up the blood on their own and clear the airway - once the bleeding has been stopped.

## Choking on Tourniquet

> **In-Game Description**  
> _"**Choking on tourniquet** &mdash; A severe lack of intellect caused a potentially fatal mishap in form of a misapplied tourniquet."_

**Causes**: Someone had the bright idea to apply a [tourniquet](/docs/wiki/medical-devices.md#tourniquet) to the neck :woozy_face: Only a severe lack of intellect and medical incompetence can cause this condition.

**Effects**: Suffocation and death if not treated immediately.

```mermaid
---
config:
  flowchart:
    htmlLabels: true
---
flowchart LR
  tourniquet[tourniquet applied to neck] ==> choking_on_tourniquet[choking on tourniquet]
  choking_on_tourniquet ==> | severity = 1 | death[death]

  linkStyle 0,1 stroke: #b10000
  style choking_on_tourniquet stroke-width: 4px
```

*See the section on the [pathophysiological system](/docs/wiki/pathophysiological-system.md#pathophysiological-system) for more information on the graphical representation.*

**Treatment**: Removing the [tourniquet](/docs/wiki/medical-devices.md#tourniquet) from the neck will restore breathing and prevent death.

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#choking)_
<!-- @end_generated_block -->
