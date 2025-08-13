# Concepts

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [Concepts](/docs/wiki/concepts.md)_
<!-- @end_generated_block -->

This section provides an overview of key concepts related to trauma and injury management simulated by the More Injuries mod. These concepts are crucial for understanding how injuries and medical conditions interact with each other, and how they can be treated effectively.

## Lethal Triad of Trauma

<p>
<img align="right" style="height: 5cm; background-color: white" src="https://upload.wikimedia.org/wikipedia/commons/d/d4/Trauma_triad_of_death.svg" alt="trauma triad of death (wikimedia commons)">
  
The "lethal triad of trauma" is a critical concept in trauma medicine that describes the dangerous cycle of three interrelated conditions: [acidosis](#acidosis), [hypothermia](#hypothermia), and [coagulopathy](#coagulopathy). These conditions often occur together in severely injured patients and can lead to a rapid deterioration of the patient's condition, ultimately resulting in death if not addressed promptly.

More Injuries tries to simulate these interactions in a gamified, yet realistic way, making severe blood loss and trauma more challenging and interesting to manage. An overview of the interactions between these conditions is provided below, along with a flowchart that illustrates the relationships between them.

</p>

```mermaid
---
config:
  flowchart:
    htmlLabels: true
---
flowchart LR

  external[external factors] ==> hemorrhage[hemorrhage]
  external ==> cardiac_arrest[cardiac arrest]
  external ==> hypothermia[hypothermia]
  hemorrhage ==> hypovolemic_shock[hypovolemic shock]
  hypovolemic_shock ==> cardiac_arrest
  hypovolemic_shock ==> hypothermia
  hemodilution[hemodilution] ==> hypoxia
  hemodilution ==> coagulopathy[coagulopathy]
  cardiac_arrest ==> hypoxia[hypoxia]
  coagulopathy ==> hemorrhage
  hypothermia ==> hypoxia
  hypovolemic_shock ==> hypoxia
  hypoxia ==> acidosis[acidosis]
  acidosis ==> hypothermia
  acidosis ==> coagulopathy
  hypothermia ==> coagulopathy
  hypothermia ==> cardiac_arrest
  blood_transfusion[blood transfusion] ==> hypovolemic_shock
  saline_infusion[saline IV infusion] ==> hypovolemic_shock
  saline_infusion ==> hemodilution
  cardiac_arrest ==> hypothermia
  external ~~~ hypothermia

  linkStyle 10,17,18 stroke: #549b68
  linkStyle 0,1,2,3,4,5,6,7,8,9,11,12,13,14,15,16,19,20 stroke: #b10000
```

*See the section on the [pathophysiological system](#pathophysiological-system) for more information on the graphical representation.*

As per basegame RimWorld, external factors like injuries or environmental conditions can quickly lead to hypothermia or bleeding injuries. More Injuries extends this simulation depth by simulating follow-up conditions and potential cascading effects that can arise from these initial injuries.

In the context of trauma management, this means that a seemingly minor injury can rapidly escalate into life-threatening conditions if not properly addressed. Let's explore some examples of these cascading effects.

If a pawn suffers a severe injury that causes significant blood loss, they may go into [hypovolemic shock](#hypovolemic-shock), meaning their body is unable to maintain adequate blood pressure and perfusion to vital organs. This, in turn, may cause a cascade of complications, further worsening the pawn's condition. For example, the reduced flow of oxygenated blood to the tissues can starve the cells of oxygen, which leads to [hypoxia](#hypoxia) and cell death, which not only affects the local tissue (e.g., causing organ failure, brain damage, or death), but can also trigger a systemic condition. In their oxygen-deprived state, the cells begin to switch to anaerobic metabolism, producing lactic acid which lowers the blood pH, leading to [acidosis](#acidosis). The increased acidity in the blood hinders the cells' ability to produce energy, which is required for thermogenesis and maintaining body temperature, leading to [hypothermia](#hypothermia), which is further accelerated by the reduced blood flow caused by hypovolemic shock, so warmer blood is not able to reach the extremities. Both, hypothermia and acidosis, negatively affect the blood's ability to clot, leading to [coagulopathy](#coagulopathy), a condition where even minor injuries can cause uncontrolled bleeding or even new spontaneous hemorrhages. This can create a dangerous cycle known as the "lethal triad of trauma," where uncontrolled bleeding further exacerbates hypovolemic shock, leading to more hypoxia, more acidosis, and more severe hypothermia, which in turn worsens coagulopathy and increases the risk of further hemorrhages.

In order to break this vicious cycle, it is crucial to address the underlying causes of each condition. For example, stopping severe arterial bleeding in the limbs with a [tourniquet](#tourniquet) will buy time to bandage the wound and perform fluid resuscitation with [blood transfusions](#blood-bag) or [IV saline infusion](#saline-iv-bag) to restore blood volume and combat hypovolemic shock. If the patient is hypothermic, they should be wrapped in warm blankets or placed in a warm environment to prevent further heat loss. This might involve warming up the operating room to uncomfortably high temperatures to ensure the patient retains heat. 

Depending on how far gone the patient is, continued medical treatment may be required to stabilize the patient and prevent further deterioration, even during recovery. [Defibrillation](#defibrillator) or [CPR](#cpr) may be required in cases of [cardiac arrest](#cardiac-arrest) due to hypothermia or hypovolemic shock, and [hemodilution](#hemodilution) caused by excessive use of [saline IVs](#saline-iv-bag) may require administering additional [blood transfusions](#blood-bag) to restore adequate blood volume and improve tissue perfusion. 

To reduce the amount of micro-management required, new [work types](#new-work-types) have been added to allow doctors to automatically triage and treat patients ***who are in hospital beds*** based on their injuries and conditions. Nevertheless, after severe injuries, it is still recommended to keep a close eye on the patient and monitor their condition, and to manually intervene if necessary, especially if new conditions arise while the doctor is busy treating unrelated conditions or other patients.

Keep in mind that the human body is a complex system and requires some time to stabilize and for conditions to start improving. Even after blood volume has been restored, it takes time to break down and clear metabolic byproducts like lactic acid from the body, and to restore normal blood pH levels. It is not uncommon for patients to require multiple rounds of triage and treatment before they are fully stabilized and start recovering by themselves.

> [!WARNING]
> Once a certain point of no return has been reached and the lethal triad of trauma has caused the patient to enter a death spiral, it may become impossible to save them, even with the best medical care. As such, it is crucial to act quickly and decisively in the face of severe injuries and complications. Keep trained medics with the appropriate equipment on standby when engaging in combat and have pawns carry life-saving equipment like [tourniquets](#tourniquet) at all times.

> [!TIP]
> While More Injuries attempts to approximate the complexities of real-world trauma care, it is understandable if you don't want to deal with the added complexity of the lethal triad of trauma in your game. Keep in mind that you can customize many aspects of the mod in the mod settings. If you want to disable the advanced trauma simulation, you can do so by un-checking the `Enable advanced trauma simulation` option.

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#concepts)_
<!-- @end_generated_block -->
