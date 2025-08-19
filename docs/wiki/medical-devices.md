# Medical Devices and Procedures

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [Medical Devices and Procedures](/docs/wiki/medical-devices.md)_
<!-- @end_generated_block -->

In order to effectively treat the new injuries and medical conditions introduced by More Injuries, several new medical devices and treatment options have been added to the game.

<!-- @generate_toc {"source": "$self", "indent": 2} -->
- [Medical Devices and Procedures](/docs/wiki/medical-devices.md#medical-devices-and-procedures)
  - [Airway Suction Device](/docs/wiki/medical-devices.md#airway-suction-device)
  - [Bandage](/docs/wiki/medical-devices.md#bandage)
  - [Blood Bag](/docs/wiki/medical-devices.md#blood-bag)
  - [Chloroform-Soaked Cloth](/docs/wiki/medical-devices.md#chloroform-soaked-cloth)
  - [CPR](/docs/wiki/medical-devices.md#cpr)
  - [Defibrillator](/docs/wiki/medical-devices.md#defibrillator)
  - [Epinephrine Autoinjector](/docs/wiki/medical-devices.md#epinephrine-autoinjector)
  - [First Aid](/docs/wiki/medical-devices.md#first-aid)
  - [Hearing Protection](/docs/wiki/medical-devices.md#hearing-protection)
  - [Hemostatic Agent](/docs/wiki/medical-devices.md#hemostatic-agent)
  - [Ketamine Autoinjector](/docs/wiki/medical-devices.md#ketamine-autoinjector)
  - [Morphine Autoinjector](/docs/wiki/medical-devices.md#morphine-autoinjector)
  - [Saline IV Bag](/docs/wiki/medical-devices.md#saline-iv-bag)
  - [Splint](/docs/wiki/medical-devices.md#splint)
  - [Thoracoscope](/docs/wiki/medical-devices.md#thoracoscope)
  - [Tourniquet](/docs/wiki/medical-devices.md#tourniquet)
<!-- @end_generated_block -->

## Airway Suction Device

<p>
  <img align="right" src="/Textures/Thing/ASD.png" height="96" alt="Airway Suction Device" />
  
> **In-Game Description**
> _"**Airway suction device** &mdash; An airway suction device is a medical device used to clear the airway of blood, vomit, or other obstructions that may prevent a patient from breathing properly. It is commonly used in emergency situations, such as when a patient is choking from blood or other fluids, to prevent asphyxiation and death. The hand-operated device uses a vacuum to remove fluids from the airway, allowing the patient to breathe freely and restoring normal respiration."_

</p>

**Used for**: Clearing the airway of patients who are [choking on blood](/docs/wiki/injuries/choking.md#choking-on-blood) or other fluids to prevent asphyxiation. The airway suction device is a hand-operated device that uses a vacuum to remove fluids from the airway, allowing the patient to breathe freely and restoring normal respiration. It can be reused multiple times and is more effective than [CPR](/docs/wiki/medical-devices.md#cpr) in clearing the airway of obstructions.

**Usage**:

1. Tell a *drafted* pawn capable of doctoring to use the airway suction device on a patient using the `Clear airways` option in the right-click context menu of the patient. The airway suction device must be accessible in a stockpile or the doctor's inventory. Self-treatment is not possible. The device will not be consumed during the treatment and can be reused multiple times.
2. Clearing the airway with an airway suction device is part of the [First Aid](/docs/wiki/medical-devices.md#first-aid) order for *drafted* pawns.

**Production**: At a machining table.  
**Production Skill Requirement**: `Crafting` at level 4  
**Research Requirements**: [Emergency medicine](/docs/wiki/research.md#emergency-medicine)  
**Ingredients**: 25x `Steel`, 20x `Chemfuel`, 1x `Component`, 5x `Plasteel`  
**Success Chance**: `<medicine skill>  / 8`, but at least the configured minimum success chance in the mod settings.  
**Mass**: 0.4 kg  
**Bulk (Combat Extended)**: 1.5

## Bandage

<p>
  <img align="right" src="/Textures/Thing/Bandage/bandage_b.png" height="64" alt="Bandage" />
  <img align="right" src="/Textures/Thing/Bandage/bandage_a.png" height="64" alt="Bandage" />

> **In-Game Description**
> _"**Bandage** &mdash; Pieces of fabric prepared to slow the flow of blood from wounds. This type of elastic bandage is designed as a temporary measure to apply pressure to a wound to reduce the bleed rate and promote clotting. It is not a substitute for proper medical treatment, but can be used to stabilize a patient until they can be treated by a doctor. Moderately fast to apply, but may not be as effective as a hemostatic agent in stopping severe bleeding."_

</p>

**Used for**: Temporarily reducing the bleed rate of wounds to allow the patient to be stabilized by a doctor. Bandages are a simple and effective way to slow the progression of blood loss and prevent the patient from going into [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock) until more advanced medical treatment can be provided. This type of elastic bandage allows to reduce the bleed rate of the wound by up to 80% and lasts approximately 12 hours until fully soaked with blood. Effectiveness scales linearly with time during this period. Bandages are a good choice for treating minor to moderate bleeding wounds, but may not be as effective as [hemostatic agents](/docs/wiki/medical-devices.md#hemostatic-agent) in stopping severe bleeding.

**Usage**:

1. Tell a *drafted* pawn capable of doctoring to apply a bandage to a wound using one of the `Stabilize with bandages` options in the right-click context menu of the patient. The bandage must be accessible in a stockpile or the doctor's inventory. Self-treatment is possible.
2. Applying a bandage is part of the [First Aid](/docs/wiki/medical-devices.md#first-aid) order for *drafted* pawns.

**Production**: At a crafting spot or tailor bench.  
**Production Skill Requirement**: None  
**Research Requirements**: [Basic anatomy](/docs/wiki/research.md#basic-anatomy)  
**Ingredients**: 2x `any fabric`  
**Success Chance**: 100%  
**Mass**: 0.05 kg  
**Bulk (Combat Extended)**: 0.05

## Blood Bag

Blood bags are one of the two options for fluid resuscitation in patients suffering from severe blood loss and [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock), the other one being [saline IV infusions](#saline-iv-bag). While saline IV bags are generally easier to produce and store, blood bags provide the advantage of restoring actual blood components, making them the only safe option for treating patients with advanced [coagulopathy](/docs/wiki/injuries/coagulopathy.md#coagulopathy) or [acidosis](/docs/wiki/injuries/acidosis.md#acidosis).

<p>
  <img align="right" src="/Textures/Thing/BloodBag/blood_bag_b.png" height="64" alt="Blood Bag" />
  <img align="right" src="/Textures/Thing/BloodBag/blood_bag_a.png" height="64" alt="Blood Bag" />

> **In-Game Description**
> _"**Blood bag** &mdash; A 750ml bag of whole blood from a blood donation, ready for transfusion. It is used in the treatment of massive bleeding and allows quick restoration of blood volume to combat the effects of hypovolemic shock. Must be stored in a refrigerator or freezer.  
> Blood can be drawn from a healthy colonist or prisoner and stored in a blood bag for later use."_

</p>

**Used for**: Restoring blood volume in patients suffering from severe blood loss and [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock). Blood transfusion is the only way to fully stabilize and save the patient's life in severe cases of [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock). It is recommended to keep a ready supply of blood bags in your hospital supply room for emergency situations. You may even want to keep a few blood bags in the inventory of your combat medics to perform immediate blood transfusions on the battlefield.  
Blood bags are also the only way to stabilize pawns with severe [hemodilution](/docs/wiki/injuries/hemodilution.md#hemodilution) or patients with advanced [acidosis](/docs/wiki/injuries/acidosis.md#acidosis) and [coagulopathy](/docs/wiki/injuries/coagulopathy.md#coagulopathy) to restore their blood volume and allow them to recover. Blood bags can be stored in a refrigerator or freezer to keep them fresh for longer periods of time.

**Usage**:

1. Tell a *drafted* pawn capable of doctoring to transfuse blood from a blood bag to a patient using one of the `Transfuse blood` options in the right-click context menu of the patient. The blood bag must be accessible in a stockpile or the doctor's inventory. Self-treatment is not possible. Depending on the selected options (`stabilize` or `fully heal`), the patient's blood loss will be reduced by a certain amount, or the patient will be fully healed. Each blood bag is capable of restoring 35% blood volume.
2. Transfusing blood (`stabilize`) from a blood bag is part of the [First Aid](/docs/wiki/medical-devices.md#first-aid) order for *drafted* pawns.
3. Alternatively, restoring blood volume to a stable condition is part of a new general, high-priority work type for all doctors. So, if you have a patient in a hospital bed with severe blood loss, any non-drafted doctor will automatically attempt to restore blood volume using a blood bag, in accordance with the configured work priorities.

> [!TIP]
> Performing a life-saving blood transfusion on members of other factions can help improve faction relations.

**Production**: 

1. Colonists, visitors, or prisoners can donate blood by scheduling the `Extract blood bag` procedure on the *operations* tab of the selected pawn. Colonists will donate blood willingly and know that they are helping to save lives of their fellow colonists. Prisoners and guests, on the other hand, may require a little more "convincing" to donate blood - at the cost of faction relations.
2. Downed enemies can be executed through the `Harvest blood (finish off)` option in the right-click context menu to extract blood bags from them (requires a medically skilled, *drafted* pawn capable of doctoring). This is a quick and effective way to obtain blood bags from enemies on the battlefield, but is considered a war crime and will have severe consequences for your faction relations.
3. Blood bags can be automatically and safely extracted from prisoners using the `blood bag farm` prisoner interaction option. It works analogous to the `hemogen farm` option from the Biotech DLC and allows to automatically schedule the extraction of blood bags from prisoners at regular intervals when it is safe to do so and does not pose a risk to the prisoner's health. It is still considered an unethical and offensive practice and will have consequences for your faction relations.

**Research Requirements**: [Basic first aid](/docs/wiki/research.md#basic-first-aid)  
**Ingredients**: N/A  
**Success Chance**: 100%  
**Mass**: 0.75 kg  
**Bulk (Combat Extended)**: 1.5

## Chloroform-Soaked Cloth

<p>
  <img align="right" src="/Textures/Thing/Chloroform/chloroform_b.png" height="64" alt="Chloroform" />
  <img align="right" src="/Textures/Thing/Chloroform/chloroform_a.png" height="64" alt="Chloroform" />

> **In-Game Description**
> _"**Chloroform-soaked cloth** &mdash; A piece of cloth soaked in chloroform, used to render a patient unconscious for medical procedures. It is effective in emergency situations where rapid sedation is required. However, due to a variety of factors, such as duration of exposure, concentration of chloroform, and individual patient factors, administering an appropriate dose can be difficult and risk of overdose is high. Due to its high potential for side effects, such as respiratory depression, hepatic and renal damage, and cardiac arrest, modern medicine has largely replaced chloroform with safer alternatives."_

</p>

**Used for**: Administering [chloroform](/docs/wiki/injuries/chloroform-buildup.md#chloroform-buildup) to induce sedation in patients and render them unconscious. While used extensively in the past, modern medicine has largely replaced chloroform with safer alternatives due to its difficulty in controlling dosage and high potential for side effects, such as [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest), [hepatic and renal damage](/docs/wiki/injuries/chemical-damage.md#chemical-damage), and respiratory depression.

**Usage**:

1. Tell a *drafted* pawn capable of doctoring to use the chloroform-soaked cloth on a patient using the `Sedate (chloroform)` option in the right-click context menu. The chloroform must be accessible in a stockpile or the doctor's inventory. Self-treatment is possible.
2. Alternatively, since chloroform is considered a drug, it can be administered as part of the normal medical treatment using the *operations* tab of the patient or by telling the pawn to injest the drug directly by right-clicking on the drug and selecting `inhale chloroform`.

**Production**: At a drug lab.  
**Production Skill Requirement**: `Crafting` at level 4, `Intellectual` at level 2  
**Research Requirements**: [Chloroform synthesis](/docs/wiki/research.md#chloroform-synthesis)  
**Ingredients**: 4x `Smokeleaf leaves`, 1x `Chemfuel`  
**Success Chance**: 100%  
**Mass**: 0.05 kg  
**Bulk (Combat Extended)**: 0.05

## CPR

Cardiopulmonary resuscitation (CPR) is an emergency procedure that combines chest compressions with artificial ventilation to manually preserve brain function until further measures can be taken to restore spontaneous blood circulation and breathing in a person who is in [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest), suffering from a [heart attack](https://rimworldwiki.com/wiki/Ailments#Heart_attack), or is [choking on blood](/docs/wiki/injuries/choking.md#choking-on-blood).

> **In-Game Description**
> _"**CPR** &mdash; Perform CPR to stabilize patients with cardiac or respiratory arrest."_

**Used for**: Stabilizing patients suffering from [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest) (during `ventricular fibrillation` and `clinical death` stages), [heart attacks](https://rimworldwiki.com/wiki/Ailments#Heart_attack), and [choking on blood](/docs/wiki/injuries/choking.md#choking-on-blood).

**Usage**

1. Tell a *drafted* pawn capable of doctoring to perform CPR on a pawn suffering from [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest) (during `ventricular fibrillation` and `clinical death` stages), [heart attack](https://rimworldwiki.com/wiki/Ailments#Heart_attack), or [choking on blood](/docs/wiki/injuries/choking.md#choking-on-blood) using the `Perform CPR` option in the right-click context menu. Self-treatment is not possible.
2. CPR is part of the [First Aid](/docs/wiki/medical-devices.md#first-aid) order for *drafted* pawns.
3. Alternatively, resuscitating patients using CPR is now part of a new general, high-priority work type for all doctors. So, if you have a patient in a hospital bed with a condition that requires CPR, any non-drafted doctor will automatically attempt to resuscitate the patient using CPR or a [defibrillator](/docs/wiki/medical-devices.md#defibrillator), in accordance with the configured work priorities.

**Research Requirements**: [Cardiopulmonary Resuscitation (CPR)](/docs/wiki/research.md#cardiopulmonary-resuscitation-cpr)  
**Severity Reduction**: $f_{\text{sigmoid}}($ `medicine skill> / 15` $)$ where $f_{\text{sigmoid}}$ is a diffused sigmoid function defined as $f_{\text{sigmoid}}(x) = \frac{1}{1 + e^{-10\cdot(x - 0.5)}} + z$ where $z$ is a random factor between $-0.1$ and $0.1$. See [Wolfram Alpha](https://www.wolframalpha.com/input?i=f%28x%29+%3D+1+%2F+%281+%2B+e%5E%28-10*%28x%2F15+-+0.5%29%29%29+%2B+z%2C+x+in+%5B0%2C+20%5D%2C+z+in%5B-0.1%2C0.1%5D) for a visualization of the function.

> [!IMPORTANT]
> Note that CPR needs to be unlocked through research before it can be used. Make sure to prioritize researching [Cardiopulmonary Resuscitation (CPR)](/docs/wiki/research.md#cardiopulmonary-resuscitation-cpr) as soon as possible to avoid losing valuable pawns to treatable conditions like [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest), [heart attacks](https://rimworldwiki.com/wiki/Ailments#Heart_attack), or [respiratory arrest](/docs/wiki/injuries/choking.md#choking-on-blood).

> [!CAUTION]
> Depending on the randomized factor $z$, poor medical skill of the doctor may do more harm than good when performing CPR. It is recommended to have a decently skilled doctor perform CPR to ensure the best possible outcome.

## Defibrillator

TODO from below

Defibrillation is a treatment for life-threatening conditions that affect the rhythm of the heart, such as [heart attacks](https://rimworldwiki.com/wiki/Ailments#Heart_attack) and the `ventricular fibrillation` stage of [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest). It involves delivering a high-energy electric shock to the heart with a reusable device called a defibrillator to restore normal heart rhythm. It is a more specialized and faster treatment than [CPR](/docs/wiki/medical-devices.md#cpr) in these cases, however, it requires a defibrillator to be available and accessible to the treating doctor.


<p>
  <img align="right" src="/Textures/Thing/Defibrillator/defibrillator_b.png" height="64" alt="Defibrillator" />
  <img align="right" src="/Textures/Thing/Defibrillator/defibrillator_a.png" height="64" alt="Defibrillator" />

> **In-Game Description**
> _"**Defibrillator** &mdash; A defibrillator is a reusable device that gives a high energy electric shock to the heart through the chest wall to restore normal heart rhythm in cases of ventricular fibrillation (irregular heartbeat) which may be caused by heart attacks or in early stages of cardiac arrest.  
> Note that the defibrillator can only be used on patients suffering from heart attack or ventricular fibrillation. In case of full cardiac arrest (clinical death), only CPR can save the patient."_

</p>

**Used for**: Treating [heart attacks](https://rimworldwiki.com/wiki/Ailments#Heart_attack) and [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest) during the `ventricular fibrillation` stage.

**Usage**

1. Tell a *drafted* pawn capable of doctoring to use the defibrillator on a pawn suffering from a [heart attack](https://rimworldwiki.com/wiki/Ailments#Heart_attack) or [`ventricular fibrillation`](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest) using the `Defibrillate` option in the right-click context menu. The defibrillator must be accessible in a stockpile or the doctor's inventory. Self-treatment is not possible. In case of the `clinical death` stage of [cardiac arrest](/docs/wiki/injuries/cardiac-arrest.md#cardiac-arrest), only [CPR](/docs/wiki/medical-devices.md#cpr) can save the patient.
2. Defibrillation is part of the [First Aid](/docs/wiki/medical-devices.md#first-aid) order for *drafted* pawns.
3. Alternatively, resuscitating patients using a defibrillator is now part of a new general, high-priority work type for all doctors. So, if you have a patient in a hospital bed with a heart attack or ventricular fibrillation, any non-drafted doctor will automatically attempt to resuscitate the patient using a defibrillator or [CPR](/docs/wiki/medical-devices.md#cpr), in accordance with the configured work priorities.

**Production**: At a machining table.  
**Production Skill Requirement**: `Crafting` at level 6  
**Research Requirements**: [Emergency medicine](/docs/wiki/research.md#emergency-medicine)  
**Ingredients**: 25x `Steel`, 20x `Chemfuel`, 4x `Component`, 25x `Plasteel`, 10x `Gold`, 100x `Silver`  
**Success Chance**: `<medicine skill>  / 8`, but at least the configured minimum success chance in the mod settings.  
**Chance to Break[^1]**: 10%

## Epinephrine Autoinjector

<p>
  <img align="right" src="/Textures/Thing/Epinephrine/injector_yellow_b.png" height="64" alt="Epinephrine" />
  <img align="right" src="/Textures/Thing/Epinephrine/injector_yellow_a.png" height="64" alt="Epinephrine" />
  
> **In-Game Description**
> _"**Epinephrine autoinjector** &mdash; An epinephrine autoinjector (or adrenaline autoinjector) is a medical device for injecting a measured dose or doses of epinephrine (adrenaline) by means of autoinjector technology."_
  
</p>

**Used for**: Administering a large dose of [epinephrine (adrenaline)](/docs/wiki/injuries/adrenaline-rush.md#adrenaline-rush) to increase heart rate and blood pressure of the patient, which can help to slow the progression of [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock) for a short period of time until the patient can be fully stabilized using blood transfusion. Alternatively, the pain relief from the adrenaline rush can help to wake up a patient who has lost consciousness due to severe pain or other conditions. There have also been reports of epinephrine being used as a combat stimulant to increase the combat effectiveness of soldiers in the field, however, there is a risk of [adrenaline overdose](/docs/wiki/injuries/adrenaline-rush.md#adrenaline-rush) if used excessively.

**Usage**:

1. Tell a *drafted* pawn capable of doctoring to use the epinephrine autoinjector on a patient using the `Inject epinephrine` option in the right-click context menu. The epinephrine autoinjector must be accessible in a stockpile or the doctor's inventory. Self-treatment is possible.
2. Alternatively, as epinephrine is considered a drug, it can be administered as part of the normal medical treatment using the *operations* tab of the patient or by telling the pawn to injest the drug directly by right-clicking on the drug and selecting `Use epinephrine autoinjector`.

**Production**: At a drug lab.  
**Production Skill Requirement**: `Crafting` at level 6, `Intellectual` at level 4  
**Research Requirements**: [Epinephrine synthesis](/docs/wiki/research.md#epinephrine-synthesis)  
**Ingredients**: 1x `Neutroamine`, 1x `Yayo`  
**Success Chance**: 100%

## First Aid

First aid is the initial assistance or treatment given to a person who is injured or suddenly ill before full medical treatment can be provided. With More Injuries, `Provide first aid` is a new command that can be given to *drafted* pawns capable of doctoring to attempt to automatically stabilize patients with life-threatening conditions. It is an aggregated work type that includes application of [tourniquets](/docs/wiki/medical-devices.md#tourniquet), the use of [CPR](/docs/wiki/medical-devices.md#cpr), [defibrillation](/docs/wiki/medical-devices.md#defibrillator), [airway suction](/docs/wiki/medical-devices.md#airway-suction-device), [blood transfusion](/docs/wiki/medical-devices.md#blood-bag), stopping bleeding using [bandages](/docs/wiki/medical-devices.md#bandage) or [hemostatic agents](/docs/wiki/medical-devices.md#hemostatic-agent), and other emergency treatments.

> **In-Game Description**
> _"**Provide first aid** &mdash; Evaluating treatment options for &lt;patient name&gt;."_

**Used for**: Automatically stabilizing and treating patients with life-threatening conditions.

**Usage**

Tell a *drafted* pawn capable of doctoring to provide first aid to a patient using the `Provide first aid` option in the right-click context menu. The pawn will automatically go through a defined sequence of emergency treatments to stabilize the patient and prevent further deterioration of their condition. Self-treatment is not possible.

1. If the patient is suffering from severe bleeding on the limbs, the doctor will first apply a [tourniquet](/docs/wiki/medical-devices.md#tourniquet) to the affected limb(s) to stop the bleeding. The tourniquets are applied in the order of severity and must be in the doctor's inventory. Otherwise, this step will be skipped.
2. The pawn will first attempt to reduce the severity of any externally, bleeding injury by applying [bandages](/docs/wiki/medical-devices.md#bandage) or [hemostatic agents](/docs/wiki/medical-devices.md#hemostatic-agent) to the wounds.
3. The pawn will then attempt to treat cardiac arrest through [defibrillation](/docs/wiki/medical-devices.md#defibrillator) or [CPR](/docs/wiki/medical-devices.md#cpr), if necessary.
4. An [airway suction device](/docs/wiki/medical-devices.md#airway-suction-device) will be used to clear the airway of any obstructions.
5. If neither a [defibrillator](/docs/wiki/medical-devices.md#defibrillator) nor an [airway suction device](/docs/wiki/medical-devices.md#airway-suction-device) is available, but treatment is required, the pawn will start performing [CPR](/docs/wiki/medical-devices.md#cpr) to stabilize the patient.
6. An immediate [blood transfusion](/docs/wiki/medical-devices.md#blood-bag) will be performed to restore blood volume in cases of severe blood loss and [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock).
7. Normal medical treatment (vanilla RimWorld) will be started to fully stabilize the patient and treat any remaining injuries.
8. If all bleeding has stopped and the patient is stable, the doctor will remove any previously applied [tourniquets](/docs/wiki/medical-devices.md#tourniquet) from the patient to restore blood flow to the affected limb and prevent [gangrene](/docs/wiki/injuries/gangrene.md#gangrene) from developing.

After each step, the doctor will re-evaluate the patient's condition and decide on the next course of action. The treatment sequence will continue until the patient is fully stabilized or until the doctor is unable to access the necessary medical devices or treatments.

> [!CAUTION]
> Beware that due to the complexity of the treatment sequence, the doctor may not necessarily choose the most optimal treatment at each step. As the severity of the conditions are not taken into account, the doctor may not always prioritize the most life-threatening conditions first. It is recommended to keep a close eye on the treatment progress and intervene manually if necessary.

## Hearing Protection

> :construction: *This feature is still under development.*  
> Currently, hearing protection is evaluated by the number of clothing layers covering the ears. The more layers of clothing covering the ears, the higher the protection against hearing damage from loud noises. In the future, this feature may be expanded to include ear protection items, such as earplugs or earmuffs, that can be worn by pawns to further reduce the risk of hearing damage.

## Hemostatic Agent

<p>
  <img align="right" src="/Textures/Thing/Hemostat/hemostat_b.png" height="64" alt="Hemostatic Agent" />
  <img align="right" src="/Textures/Thing/Hemostat/hemostat_a.png" height="64" alt="Hemostatic Agent" />

> **In-Game Description**
> _"**Hemostatic agent** &mdash; A special chemical compound designed to promote blood clotting and reduce the flow of blood from wounds. Hemostatic agents are used to control bleeding in emergency situations, such as when a patient is suffering from severe trauma or has been injured in combat. The compound is fast-acting and can be used to stabilize a patient until they can be treated by a doctor. However, it is not a substitute for proper medical treatment.  
> Quick to apply and effective at stopping severe bleeding, hemostatic agents are an essential part of any first aid kit."_

</p>

**Used for**: Temporarily reducing the bleed rate of wounds to allow the patient to be stabilized by a doctor. Hemostatic agents are a fast-acting and effective way to stop severe bleeding and prevent the patient from going into [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock) until more advanced medical treatment can be provided. This chemical compound is designed to promote blood clotting and reduce the flow of blood from wounds, allowing to reduce the bleed rate of the wound by up to 80% and lasts approximately 8 hours until its active ingredients are depleted. Effectiveness scales linearly with time during this period. Hemostatic agents are an excellent choice for treating severe bleeding wounds, but may be overkill for minor to moderate bleeding.

**Usage**:

1. Tell a *drafted* pawn capable of doctoring to apply a hemostatic agent to a wound using one of the `Stabilize with hemostatic agent` options in the right-click context menu of the patient. The hemostatic agent must be accessible in a stockpile or the doctor's inventory. Self-treatment is possible.
2. Applying a hemostatic agent is part of the [First Aid](/docs/wiki/medical-devices.md#first-aid) order for *drafted* pawns.

**Production**: At a drug lab.  
**Production Skill Requirement**: `Crafting` at level 4  
**Research Requirements**: [Advanced first aid](/docs/wiki/research.md#advanced-first-aid)  
**Ingredients**: 5x `any textile`, 3x `Chemfuel`  
**Success Chance**: 100%

> [!TIP]
> Hemostatic agents are a powerful tool to stop severe bleeding in an emergency, so make sure you have a few hemostatic agents ready in your hospital or carried by your combat medics to quickly treat severe injuries on the battlefield.

## Ketamine Autoinjector

> [!CAUTION]
> TODO: update docs

## Morphine Autoinjector

> [!CAUTION]
> TODO: update docs

## Saline IV Bag

> [!CAUTION]
> TODO: update docs

## Splint

<p>
  <img align="right" src="/Textures/Thing/Splint/splint_b.png" height="64" alt="Splint" />
  <img align="right" src="/Textures/Thing/Splint/splint_a.png" height="64" alt="Splint" />
  
> **In-Game Description**
> _"**Splint** &mdash; A splint is a rigid device used for immobilizing and protecting an injured bone or joint. It is used to prevent further damage to the bone and surrounding tissue and promote healing over time. Splints are commonly used to treat bone fractures, sprains, and dislocations."_
  
</p>

**Used for**: Treating [bone fractures](/docs/wiki/injuries/fractures.md#bone-fracture), allowing them to heal over time (see [healing bone fracture](/docs/wiki/injuries/fractures.md#healing-bone-fracture)).

**Usage**

1. Tell a *drafted* pawn capable of doctoring to apply a splint to a pawn with a bone fracture using the `Splint fractures` option in the right-click context menu. The splint must be accessible in a stockpile or the doctor's inventory. Due to its simplicity, the procedure can be performed anywhere and even in combat situations, to quickly restore partial stability to the injured limb and allow the pawn to move again. However, this treatment method comes at the cost of treatment quality and a slightly longer time for the fracture to heal.
2. Use the *operations* tab of the injured pawn to schedule the application of a splint to a bone fracture in a controlled environment, such as a hospital bed, to ensure the highest treatment quality and fastest healing time.
3. Alternatively, splinting fractures is now part of a new general, low-priority work type for all doctors. So, if you have a patient in a hospital bed with a bone fracture, any non-drafted doctor will automatically attempt to splint the fracture, in accordance with the configured work priorities. Note, that this option must first be enabled in the mod settings. Doctors will not attempt to splint fractures if [osteosynthetic surgery](/docs/wiki/surgeries.md#osteosynthetic-surgery) is scheduled for the same limb.

> [!TIP]
> Alternatively to splinting the fracture, the bone can be [surgically repaired](/docs/wiki/surgeries.md#osteosynthetic-surgery) to realign and stabilize the bone, allowing for a quicker recovery.

**Production**: At a crafting spot or machining table.  
**Production Skill Requirement**: None  
**Research Requirements**: [Basic anatomy](/docs/wiki/research.md#basic-anatomy)  
**Ingredients**: 4x `Wood` or `Steel`, 1x `any textile`  
**Success Chance**: 100%

## Thoracoscope

<p>
  <img align="right" src="/Textures/Thing/Thoracoscope/thoracoscope_b.png" height="64" alt="Tourniquet" />
  <img align="right" src="/Textures/Thing/Thoracoscope/thoracoscope_a.png" height="64" alt="Tourniquet" />

> **In-Game Description**
> _"**Thoracoscope** &mdash; A thoracoscope is a specialized medical instrument equipped with a camera and light source, designed for minimally invasive examination and procedures within the chest cavity. It is inserted through a small incision in the chest wall, allowing surgeons to visualize and operate on the lungs, pleura, or other thoracic structures.  
> Thoracoscopes are commonly used in video-assisted thoracoscopic surgery (VATS), such as repairing collapsed lungs or removing diseased tissue."_

</p>

**Used for**: Performing [video-assisted thoracoscopic surgery](/docs/wiki/surgeries.md#video-assisted-thoracoscopic-surgery) to treat [lung collapse](/docs/wiki/injuries/lung-collapse.md#lung-collapse) with minimally invasive surgical techniques and a high success rate to repair the damage and restore the lung to full function.

**Usage**: As a required ingredient for the [video-assisted thoracoscopic surgery](/docs/wiki/surgeries.md#video-assisted-thoracoscopic-surgery) procedure, which can be found in the *operations* tab of the selected pawn.

**Production**: At a machining table.  
**Production Skill Requirement**: `Crafting` at level 8  
**Research Requirements**: [Advanced thoracic surgery](/docs/wiki/research.md#advanced-thoracic-surgery)  
**Ingredients**: 20x `Steel`, 20x `Chemfuel`, 4x `Component`, 50x `Plasteel`, 20x `Gold`, 250x `Silver`  
**Chance to Break[^1]**: 20%

## Tourniquet

<p>
  <img align="right" src="/Textures/Thing/Tourniquet/tourniquet_b.png" height="64" alt="Tourniquet" />
  <img align="right" src="/Textures/Thing/Tourniquet/tourniquet_a.png" height="64" alt="Tourniquet" />

> **In-Game Description**
> _"**Tourniquet** &mdash; A tourniquet is a constricting or compressing device used to control venous and arterial circulation to an extremity for a period of time. It is used to stop hemorrhaging (bleeding) and is most commonly used in emergency situations, such as battlefield injuries or accidents, to prevent hypovolemic shock and death.  
> As a tourniquet restricts blood flow to the limb, it should be removed as soon as possible to prevent ischemia (starvation of oxygen) and necrosis (cell death) in the limb, which can lead to gangrene or death from sepsis."_

</p>

**Used for**: Stopping severe bleeding in the corresponding limb to prevent extreme blood loss, [hypovolemic shock](/docs/wiki/injuries/hypovolemic-shock.md#hypovolemic-shock), and death. When applied to a limb, a tourniquet will restrict blood flow to the tissue below the tourniquet, which can severely reduce the bleed rate of injuries in that limb up to 95% until the tourniquet is removed. While a tourniquet can save a life in an emergency, prolonged use can cause severe damage to the limb in the form of ischemia (starvation of oxygen) and necrosis (cell death), which can lead to a life-threatening condition known as [gangrene](/docs/wiki/injuries/gangrene.md#gangrene). Therefore, a tourniquet should be removed as soon as possible after the bleeding has been stopped to prevent further complications.

**Usage**:

<ol>
<li> 

Tell a conscious pawn capable of doctoring to apply a tourniquet to a limb using the `Apply tourniquet to <limb> of <patient name>` option in the right-click context menu of the patient to apply a tourniquet to the corresponding limb. The tourniquet must be accessible in a stockpile or the doctor's inventory. Self-treatment is possible. Conversely, choose the `Remove tourniquet from <limb> of <patient name>` option to remove a previously applied tourniquet and restore blood flow to the limb.

</li>
<li>
<img align="right" src="/Textures/UI/tourniquet_gizmo.png" height="64" alt="Tourniquet" /> 

Alternatively, use the *tourniquet gizmo* (depicted on the right) to apply or remove a tourniquet from the currently selected pawn. The gizmo can be found in the bottom of the screen when a pawn is selected and is capable of applying or removing a tourniquet from themselves. You can choose to hide the gizmo in the mod settings if your UI is already cluttered with other gizmos and you don't want to see it all the time. In that case, you will have to rely on the right-click context menu to apply or remove a tourniquet.

</li>
<li>

Applying a tourniquet is part of the [First Aid](/docs/wiki/medical-devices.md#first-aid) order for *drafted* pawns, if the aggregated bleeding severity of the limb is above the threshold configured in the mod settings.

</li>
</ol>

**Production**: At a crafting spot or tailor bench.  
**Production Skill Requirement**: None  
**Research Requirements**: [Basic first aid](/docs/wiki/research.md#basic-first-aid)  
**Ingredients**: 1x `Wood` or `Steel`, 4x `any textile` or `leather`  
**Success Chance**: 100%

> [!TIP]
> Tourniquets are a powerful tool to stop severe bleeding in an emergency, so make sure you have a few tourniquets ready in your hospital or carried by your combat medics to quickly treat severe injuries on the battlefield.

> [!TIP]
> Should the patient decease during treatment, the tourniquet can be [recovered](/docs/wiki/work-types.md#tourniquet-recovery) from the corpse to be reused on another patient. This is automatically done as a new low-prority work type for all non-drafted doctors.

> [!WARNING]
> While generally easy to handle, severly incompetent doctors (medicine and intellectual skill below 3) may have a hard time applying a tourniquet correctly, potentially even having the not-so-bright idea to apply it to the neck. It goes without saying that such misuses of a tourniquet are not recommended and may lead to the patient [choking to death](/docs/wiki/injuries/choking.md#choking-on-tourniquet).

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#medical-devices-and-procedures)_
<!-- @end_generated_block -->

[^1]: Some items have the potential to be reusable and they will not necessarily be consumed during a surgery or procedure. However, there is a chance that the item will break after each use, either due to wear and tear or due to the nature of the procedure. The chance to break is indicated by the **Chance to Break** value in the item's description.
