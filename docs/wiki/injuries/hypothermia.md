# Hypothermia

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [Injuries and Medical Conditions A-Z](/docs/wiki/injuries/README.md) :arrow_right: [Hypothermia](/docs/wiki/injuries/hypothermia.md)_
<!-- @end_generated_block -->

_Vanilla RimWorld condition, see [Hypothermia (RimWorld Wiki)](https://rimworldwiki.com/wiki/Ailments#Hypothermia)_

**Causes**: Exposure to cold temperatures (see [Hypothermia (RimWorld Wiki)](https://rimworldwiki.com/wiki/Ailments#Hypothermia)), hypoperfusion due to [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock) or [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest), and reduced thermogenesis due to [acidosis](/docs/wiki/injuries/acidosis.md#acidosis).

**Effects**: Hypothermia can cause a range of symptoms, including shivering, confusion, fatigue, and impaired motor function. In severe cases, it can lead to loss of consciousness, [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest), and death if not treated promptly. Due to reduced cellular metabolism at extremely low body temperatures, hypothermia can protect the body from further damage from [hypoxia](/docs/wiki/injuries/hypoxia.md#hypoxia), albeit within limits.

```mermaid
---
config:
  flowchart:
    htmlLabels: true
---
flowchart LR
  exposure[exposure] ==> hypothermia[hypothermia]
  hypovolemic_shock[hypovolemic shock] ==> hypothermia
  cardiac_arrest[cardiac arrest] ==> hypothermia
  acidosis[acidosis] ==> hypothermia
  hypothermia ==> | severity = 1 | death[death]
  hypothermia ==> cardiac_arrest
  hypothermia ==> hypoxia[hypoxia]

  linkStyle 0,1,2,3,4,5 stroke: #b10000
  linkStyle 6 stroke: #549b68, stroke-dasharray: 9,5
  style hypothermia stroke-width: 4px
```

*See the section on the [pathophysiological system](/docs/wiki/pathophysiological-system.md#pathophysiological-system) for more information on the graphical representation.*

**Treatment**: Hypothermia is primarily treated by warming the patient, either through external means (e.g., blankets, heating pads) or by ensuring adequate perfusion and thermogenesis (e.g., preventing [acidosis](/docs/wiki/injuries/acidosis.md#acidosis) and restoring blood volume using [blood bags](/docs/wiki/medical-devices.md#blood-bag)). Hypothermia caused by exposure is best pro-actively prevented by ensuring that pawns are properly equipped with warm clothing and shelter in cold environments.

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#hypothermia)_
<!-- @end_generated_block -->
