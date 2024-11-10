@import "/.crossnote/style.less";

# More Injuries User Documentation

_This document is intended to provide a comprehensive guide to the More Injuries mod for RimWorld. It is intended to be a reference for players who are looking to understand the mod's features and mechanics, as well as how to use them effectively._

## About More Injuries

The More Injuries mod aims to increase the simulation depth of RimWorld's medical system by adding a variety of new injuries, medical conditions, and treatment options, as well as simulating body part damage in more detail. Its goal is to make the medical system more challenging and interesting in some aspects, while also making it more realistic and immersive. A detailed mod settings menu is provided to allow players to customize many aspects of the mod to their liking.

## Table of Contents
<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [More Injuries User Documentation](#more-injuries-user-documentation)
  - [About More Injuries](#about-more-injuries)
  - [Table of Contents](#table-of-contents)
  - [New Injuries and Medical Conditions](#new-injuries-and-medical-conditions)
    - [Adrenaline Rush](#adrenaline-rush)
    - [Cardiac Arrest](#cardiac-arrest)
    - [Choking](#choking)
      - [Choking on Blood](#choking-on-blood)
      - [Choking on Tourniquet](#choking-on-tourniquet)
    - [Concussion](#concussion)
    - [EMP Shutdown](#emp-shutdown)
    - [Fractures](#fractures)
      - [Bone Fracture](#bone-fracture)
      - [Healing Bone Fracture](#healing-bone-fracture)
      - [Bone Fragment Laceration](#bone-fragment-laceration)
    - [Gangrene](#gangrene)
    - [Hearing Loss](#hearing-loss)
    - [Hemorrhagic Stroke](#hemorrhagic-stroke)
    - [Hypovolemic Shock :star:](#hypovolemic-shock-star)
    - [Intestinal Spilling](#intestinal-spilling)
    - [Inhalation Injury](#inhalation-injury)
    - [Lung Collapse](#lung-collapse)
    - [Organ Hypoxia](#organ-hypoxia)
    - [Paralysis](#paralysis)
    - [Spalling Injury](#spalling-injury)
  - [New Body Parts](#new-body-parts)
    - [Small and Large Intestines](#small-and-large-intestines)
    - [Femoral and Popliteal Arteries](#femoral-and-popliteal-arteries)
    - [Spinal Cord](#spinal-cord)
  - [Medical Devices and Treatments](#medical-devices-and-treatments)
    - [Splint](#splint)
    - [Defibrillator](#defibrillator)
    - [CPR](#cpr)
    - [Epinephrine Autoinjector](#epinephrine-autoinjector)
    - [Tourniquet](#tourniquet)
    - [Airway Suction Device](#airway-suction-device)
    - [Blood Bag](#blood-bag)
  - [New Work Types](#new-work-types)
  - [Known Issues and Incompatibilities](#known-issues-and-incompatibilities)

<!-- /code_chunk_output -->

## New Injuries and Medical Conditions

The following sections provide an overview of the new injuries and medical conditions added by the More Injuries mod, as well as their effects and treatment options.

### Adrenaline Rush

Adrenaline, also known as epinephrine, is a naturally occurring hormone that is released in response to stress or danger. It increases heart rate, blood pressure, and energy levels, preparing the body for a fight-or-flight response. In the game, an adrenaline rush can occur naturally in pawns as a result of combat and injury, or it can artificially be induced through the use of an [epinephrine injection](#epinephrine-autoinjector).

> **In-Game Description**
> _"**Adrenaline rush** &mdash; A rush of adrenaline temporarily increases heart rate and blood pressure, providing a boost of energy and alertness. The body's fight-or-flight response is triggered, increasing strength and speed.
> However, if the rush is too intense, it can cause anxiety, panic, and overdose symptoms such as dizziness, double vision, and nausea. In extreme overdose cases, the body can go into shock, causing heart attack, stroke, or death."_

**Causes**: Injuries or [epinephrine injections](#epinephrine-autoinjector).

**Effects**: At lower levels, an adrenaline rush can provide a temporary boost to consciousness, moving, and pain tolerance. At higher levels, it can cause a reduction in manipulation and sight. In extreme cases of overdose, it can lead to anxiety, panic, nausea, as well as, coma, [cardiac arrest](#cardiac-arrest), [hemorrhagic stroke](#hemorrhagic-stroke), and subsequent death.

**Treatment**: Adrenaline is naturally metabolized by the body over time and effects last between a few minutes to a few hours in severe cases. In cases of overdose, the pawn may require medical treatment to treat symptoms and secondary effects.

### Cardiac Arrest

Cardiac arrest is a sudden loss of blood flow resulting from the failure of the heart to effectively pump blood. It is generally divided into two categories: `ventricular fibrillation` and asystole (flatline, `clinical death`). `Ventricular fibrillation` is a condition in which the heart's electrical signals become disorganized, causing the heart to quiver or "fibrillate" instead of pumping blood effectively. In cases of `ventricular fibrillation`, a [defibrillator](#defibrillator) can be used to shock the heart back into a normal rhythm, which may be faster and more effective than [CPR](#cpr). If left untreated, `ventricular fibrillation` can progress to `clinical death`, which is a condition in which the heart stops beating completely and [CPR](#cpr) must be performed to restore blood flow and hopefully restart the heart. Applying a [defibrillator](#defibrillator) to a clinically dead patient will not be effective and may cause additional harm.

> **In-Game Description**
> _"**Cardiac arrest** &mdash; Cardiac arrest is a sudden loss of blood flow resulting from the failure of the heart to effectively pump blood. The lack of blood flow causes the body to stop working properly, resulting in loss of consciousness and death if not treated immediately. Causes for cardiac arrest include conditions that starve the heart of oxygen, such as extreme blood loss.
> A skilled doctor must perform CPR to restore blood flow and hopefully restart the heart. In early stages of cardiac arrest, during ventricular fibrillation, a defibrillator can also be used to shock the heart back into a normal rhythm, which may be faster and more effective than CPR."_

**Causes**: Extreme blood loss ([hypovolemic shock](#hypovolemic-shock-star)), [adrenaline overdose](#adrenaline-rush), and other conditions that starve the heart of oxygen.

**Effects**: Loss of consciousness, coma, multiple organ failure, and death if not treated immediately.

**Treatment**: Cardiac arrest must be treated immediately with [CPR](#cpr) to restore blood flow and hopefully restart the heart. In cases of `ventricular fibrillation`, a [defibrillator](#defibrillator) can be used to shock the heart back into a normal rhythm, which may be faster and more effective than [CPR](#cpr). In cases of `clinical death`, only [CPR](#cpr) will be effective.

### Choking

Choking is a medical emergency that occurs when a foreign object becomes lodged in the throat or windpipe, blocking the flow of air. It can be a life-threatening situation that requires immediate intervention to clear the airway and restore breathing.

#### Choking on Blood

> **In-Game Description**
> _"**Choking on blood** &mdash; Blood from traumatic injuries being aspirated into the airways triggers coughing and causes suffocation. Potentially fatal unless treated. Can be cleared by performing CPR or using a specialized airway suction device."_

**Causes**: Severe injuries to the mouth, throat, or chest that cause bleeding into the airways.

**Effects**: Coughing, suffocation, and death if not treated immediately.

**Treatment**: Stopping the bleeding of the mouth, throat, or chest will prevent further blood from entering the airways and slow the progression of the condition. The airways must be cleared using a specialized [airway suction device](#airway-suction-device) or by compressing the chest using [CPR](#cpr) to expel the blood and restore breathing.

#### Choking on Tourniquet

> **In-Game Description**  
> _"**Choking on tourniquet** &mdash; A severe lack of intellect caused a potentially fatal mishap in form of a misapplied tourniquet."_

**Causes**: Someone had the bright idea to apply a [tourniquet](#tourniquet) to the neck :woozy_face: Only a severe lack of intellect and medical incompetence can cause this condition.

**Effects**: Suffocation and death if not treated immediately.

**Treatment**: Removing the [tourniquet](#tourniquet) from the neck will restore breathing and prevent death.

### Concussion

> **In-Game Description**  
> _"**Concussion** &mdash; A concussion, also known as a mild traumatic brain injury (mTBI), is a head injury that temporarily affects brain functioning. Symptoms may include loss of consciousness; memory loss; headaches; difficulty with thinking, concentration, or balance; nausea; blurred vision; dizziness; sleep disturbances, and mood changes.
> Should resolve on its own within a few days, but can be worsened by repeated head injuries."_

**Causes**: Blunt force trauma to the head. May be caused by any violent impact to the head, such as hand-to-hand combat, being struck by a projectile, or explosions.

**Effects**: A concussion may range from a mild headache to full unconsciousness, depending on the severity of the injury. The pawn may experience a temporary loss of consciousness, memory loss, painful headaches, disorientation,difficulty with thinking, nausea and vomiting, blurred vision, and dizziness. Repeated head injuries may worsen the condition or lead to a [hemorrhagic stroke](#hemorrhagic-stroke).

**Treatment**: Concussions are generally self-limiting and will resolve on their own within a few days.

### EMP Shutdown

> **In-Game Description**
> _"**Servos disabled** &mdash; A bionic bodypart was disabled by an EMP blast. It will take around one day to reboot itself."_

**Causes**: Exposure to sources of strong electromagnetic interference, such as EMP grenades or psychic abilities.

**Effects**: The affected bionic body part will be disabled and unusable for a period of time.

**Treatment**: The bionic body part will reboot itself after around one day and return to normal function.

### Fractures

When a pawn takes damage to a bone or solid body part, there is a chance that a fracture will occur, based on the severity of the injury and the mod settings.

#### Bone Fracture

> **In-Game Description**  
> _"**Bone fracture** &mdash; A partial or complete break of a bone caused by trauma, overuse, or disease. The bone may be cracked, splintered, or completely broken into two or more pieces. Until properly treated, the affected limb will be unable to bear weight or move properly, causing severe pain and loss of function.  
> Must either be splinted to immobilize the bone and promote healing over time, or surgically repaired to realign and stabilize the bone, allowing for quick recovery."_

**Causes**: Sharp or blunt damage to a bone or solid body part.

**Effects**: A bone fracture will cause the affected limb to be unable to bear weight or move properly, causing full immobility of the corresponding body part and `+15%` pain. Bone fractures may also cause [bone fragment lacerations](#bone-fragment-laceration) if bone fragments break off and cut into the surrounding tissue.

**Treatment**: Bone fractures must be treated using a [splint](#splint) to immobilize the bone and promote [healing over time](#healing-bone-fracture), or surgery to realign and stabilize the bone, allowing for an immediate recovery.

#### Healing Bone Fracture

> **In-Game Description**  
> _"**Healing bone fracture** &mdash; A bone fracture that is in the process of healing. The bone is still weak and restricted in movement, but the limb is slowly regaining function as the bone knits back together.
> Over time, the bone will become stronger and the limb will regain full function."_

**Causes**: A [bone fracture](#bone-fracture) that has been stabilized with a [splint](#splint) and is in the process of healing.

**Effects**: The affected limb will be usable but restricted in movement. Over time, the effects will slowly diminish as the bone heals and the limb regains full function.

**Treatment**: Healing bone fractures do not require any additional treatment and will naturally heal over the course of several days to weeks, depending on how skillfully the [splint](#splint) was applied.

#### Bone Fragment Laceration

When a bone is fractured, there is a chance that fragments of the bone will break off and cause additional damage to the surrounding tissue, resulting in a laceration injury (a cut). The occurance and chance of bone fragment laceration can be adjusted in the mod settings.

> **In-Game Description**  
> _"**Cut (bone fragments)** &mdash; A cut caused by fragments of a bone."_

**Causes**: A [bone fracture](#bone-fracture) may result in sharp bone fragments breaking off and cutting into the surrounding tissue.

**Effects**: A bone fragment laceration will cause additional pain and bleeding, as well as a risk of infection if not properly treated.

**Treatment**: Bone fragment lacerations can be treated like any other cut or laceration, by treating the wound with or without medicine, preferably in a clean environment to reduce the risk of infection.

### Gangrene

### Hearing Loss

### Hemorrhagic Stroke

### Hypovolemic Shock :star:

### Intestinal Spilling

### Inhalation Injury

### Lung Collapse

### Organ Hypoxia

### Paralysis

### Spalling Injury

## New Body Parts

### Small and Large Intestines

### Femoral and Popliteal Arteries

### Spinal Cord

## Medical Devices and Treatments

### Splint

### Defibrillator

### CPR

### Epinephrine Autoinjector

### Tourniquet

### Airway Suction Device

### Blood Bag

## New Work Types

## Known Issues and Incompatibilities