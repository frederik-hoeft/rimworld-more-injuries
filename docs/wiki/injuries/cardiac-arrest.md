# Cardiac Arrest

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [Injuries and Medical Conditions A-Z](/docs/wiki/injuries/README.md) :arrow_right: [Cardiac Arrest](/docs/wiki/injuries/cardiac-arrest.md)_
<!-- @end_generated_block -->

Cardiac arrest is a sudden loss of blood flow resulting from the failure of the heart to effectively pump blood. It is generally divided into two categories: `ventricular fibrillation` and asystole (flatline, `clinical death`). `Ventricular fibrillation` is a condition in which the heart's electrical signals become disorganized, causing the heart to quiver or "fibrillate" instead of pumping blood effectively. In cases of `ventricular fibrillation`, a [defibrillator](/docs/wiki/medical-devices.md#defibrillator) can be used to shock the heart back into a normal rhythm, which may be faster and more effective than [CPR](/docs/wiki/medical-devices.md#cpr). If left untreated, `ventricular fibrillation` can progress to `clinical death`, which is a condition in which the heart stops beating completely and [CPR](/docs/wiki/medical-devices.md#cpr) must be performed to restore blood flow and hopefully restart the heart. Applying a [defibrillator](/docs/wiki/medical-devices.md#defibrillator) to a clinically dead patient will not be effective and may cause additional harm.

> **In-Game Description**
> _"**Cardiac arrest** &mdash; Cardiac arrest is a sudden loss of blood flow resulting from the failure of the heart to effectively pump blood. The lack of blood flow causes the body to stop working properly, resulting in loss of consciousness and death if not treated immediately. Causes for cardiac arrest include conditions that starve the heart of oxygen, such as extreme blood loss.  
> A skilled doctor must perform CPR to restore blood flow and hopefully restart the heart. In early stages of cardiac arrest, during ventricular fibrillation, a defibrillator can also be used to shock the heart back into a normal rhythm, which may be faster and more effective than CPR."_

**Causes**: Extreme blood loss ([hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock)), [pneumothorax](/docs/wiki/injuries/lung-collapse.md#lung-collapse), [adrenaline overdose](/docs/wiki/injuries/adrenaline-rush.md#adrenaline-rush), [ketamine overdose](/docs/wiki/injuries/ketamine-buildup.md#ketamine-buildup), [chloroform overdose](/docs/wiki/injuries/chloroform-buildup.md#chloroform-buildup), [morphine overdose](/docs/wiki/injuries/morphine-high.md#morphine-high), or other conditions that starve the heart of oxygen.

**Effects**: Loss of consciousness, coma, [hypoxia](/docs/wiki/injuries/hypoxia.md#hypoxia), multiple organ failure, and death if not treated immediately. [Hypothermia](/docs/wiki/injuries/hypothermia.md#hypothermia) will set in quickly as the body is unable to distribute heat effectively.

```mermaid
---
config:
  flowchart:
    htmlLabels: true
---
flowchart LR
hypovolemic_shock[hypovolemic shock] ==> cardiac_arrest[cardiac arrest]
pneumothorax["pneumothorax
(lung collapse)"] ==> cardiac_arrest
adrenaline[adrenaline] ==> cardiac_arrest
adrenaline ==> |overdose| cardiac_arrest
cardiac_arrest ==> hypoxia[hypoxia]
cardiac_arrest ==> hypothermia[hypothermia]
hypothermia ==> cardiac_arrest
ketamine[ketamine] ==> | overdose | cardiac_arrest
chloroform[chloroform] ==> | overdose | cardiac_arrest
morphine[morphine] ==> | overdose | cardiac_arrest

linkStyle 0,1,3,4,5,6,7,8,9 stroke: #b10000
linkStyle 2 stroke: #549b68, stroke-dasharray: 9,5
style cardiac_arrest stroke-width: 4px
```

*See the section on the [pathophysiological system](/docs/wiki/pathophysiological-system.md#pathophysiological-system) for more information on the graphical representation.*

**Treatment**: Cardiac arrest must be treated immediately with [CPR](/docs/wiki/medical-devices.md#cpr) (which needs to be researched first) to restore blood flow and hopefully restart the heart. In cases of `ventricular fibrillation`, a [defibrillator](/docs/wiki/medical-devices.md#defibrillator) can be used to shock the heart back into a normal rhythm, which may be faster and more effective than [CPR](/docs/wiki/medical-devices.md#cpr). In cases of `clinical death`, only [CPR](/docs/wiki/medical-devices.md#cpr) will be effective. [Epinephrine](/docs/wiki/medical-devices.md#epinephrine-autoinjector) may be administered to assist with defibrillation efforts and [CPR](/docs/wiki/medical-devices.md#cpr) in cases of cardiac arrest, but it is not required.

> [!NOTE]
> **Biotech DLC**: Sanguaphages are immune to cardiac arrest and will automatically recover from it once entering deathrest.

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#cardiac-arrest)_
<!-- @end_generated_block -->
