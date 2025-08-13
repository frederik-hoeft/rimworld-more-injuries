# More Injuries User Manual

_This document is intended to provide a comprehensive guide to the More Injuries mod for RimWorld. It is intended to be a reference for players who are looking to understand the mod's features and mechanics, as well as how to use them effectively._

_For instructions on how to install the mod, please refer to the [installation guide](https://github.com/frederik-hoeft/rimworld-more-injuries/blob/main/INSTALL.md)._  
_If you would like to contribute to this project or improve the documentation, please refer to the [contribution guidelines](https://github.com/frederik-hoeft/rimworld-more-injuries/blob/main/CONTRIBUTING.md) to get started._

## About More Injuries

The More Injuries mod aims to increase the simulation depth of RimWorld's medical system by adding a variety of new injuries, medical conditions, and treatment options, as well as simulating body part damage in more detail and introducing pathophysiological interactions between injuries and medical conditions; meaning that injuries and medical conditions can affect each other in complex ways, leading to cascading effects that can be difficult to manage. The mod also adds new body parts, medical devices, procedures, surgeries, research projects, and work types to enhance the medical gameplay experience.
Its goal is to make the medical system more challenging and interesting in some aspects, while also making it more realistic and immersive. A detailed mod settings menu is provided to allow players to customize many aspects of the mod to their liking.

## Table of Contents
<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [More Injuries User Manual](#more-injuries-user-manual)
  - [About More Injuries](#about-more-injuries)
  - [Table of Contents](#table-of-contents)
    - [Cardiac Arrest](#cardiac-arrest)
    - [Chemical Damage](#chemical-damage)
    - [Chemical Peritonitis](#chemical-peritonitis)
    - [Chloroform Buildup](#chloroform-buildup)
    - [Choking](#choking)
      - [Choking on Blood](#choking-on-blood)
      - [Choking on Tourniquet](#choking-on-tourniquet)
    - [Coagulopathy](#coagulopathy)
    - [Concussion](#concussion)
    - [EMP Shutdown](#emp-shutdown)
    - [Enclosed Injuries](#enclosed-injuries)
    - [Fractures](#fractures)
      - [Bone Fracture](#bone-fracture)
      - [Healing Bone Fracture](#healing-bone-fracture)
      - [Bone Fragment Laceration](#bone-fragment-laceration)
    - [Gangrene](#gangrene)
      - [Dry Gangrene](#dry-gangrene)
      - [Wet Gangrene](#wet-gangrene)
    - [Hearing Loss](#hearing-loss)
      - [Temporary Hearing Loss](#temporary-hearing-loss)
      - [(Permanent) Hearing Loss](#permanent-hearing-loss)
    - [Hemodilution](#hemodilution)
    - [Hemorrhage (Spontaneous)](#hemorrhage-spontaneous)
    - [Hemorrhagic Stroke](#hemorrhagic-stroke)
    - [Hydrostatic Shock](#hydrostatic-shock)
    - [Hypothermia](#hypothermia)
    - [Hypovolemic Shock](#hypovolemic-shock)
    - [Hypoxia](#hypoxia)
      - [Hypoxia (Brain)](#hypoxia-brain)
    - [Inhalation Injury](#inhalation-injury)
    - [Ischemia (Tourniquet)](#ischemia-tourniquet)
    - [Ketamine Buildup](#ketamine-buildup)
    - [Lung Collapse](#lung-collapse)
    - [Mechanite Therapy](#mechanite-therapy)
    - [Morphine High](#morphine-high)
    - [Paralysis](#paralysis)
    - [Spalling Injury](#spalling-injury)
    - [Vasodilation](#vasodilation)
  - [New Body Parts](#new-body-parts)
    - [Small and Large Intestines](#small-and-large-intestines)
    - [Femoral and Popliteal Arteries](#femoral-and-popliteal-arteries)
    - [Spinal Cord](#spinal-cord)
  - [Medical Devices and Procedures](#medical-devices-and-procedures)
    - [Airway Suction Device](#airway-suction-device)
    - [Bandage](#bandage)
    - [Blood Bag](#blood-bag)
    - [Chloroform-Soaked Cloth](#chloroform-soaked-cloth)
    - [CPR](#cpr)
    - [Defibrillator](#defibrillator)
    - [Epinephrine Autoinjector](#epinephrine-autoinjector)
    - [First Aid](#first-aid)
    - [Hearing Protection](#hearing-protection)
    - [Hemostatic Agent](#hemostatic-agent)
    - [Ketamine Autoinjector](#ketamine-autoinjector)
    - [Morphine Autoinjector](#morphine-autoinjector)
    - [Saline IV Bag](#saline-iv-bag)
    - [Splint](#splint)
    - [Thoracoscope](#thoracoscope)
    - [Tourniquet](#tourniquet)
  - [Surgeries](#surgeries)
    - [Cellular Regenerative Neurosurgery](#cellular-regenerative-neurosurgery)
    - [Cellular Regenerative Otologic Surgery](#cellular-regenerative-otologic-surgery)
    - [Decompressive Craniectomy](#decompressive-craniectomy)
    - [Osteosynthetic Surgery](#osteosynthetic-surgery)
    - [Stereotactic Surgery](#stereotactic-surgery)
    - [Thoracotomy](#thoracotomy)
    - [Trepanation](#trepanation)
    - [Video-Assisted Thoracoscopic Surgery](#video-assisted-thoracoscopic-surgery)
  - [New Research Projects](#new-research-projects)
    - [Basic Anatomy](#basic-anatomy)
    - [Cardiopulmonary Resuscitation (CPR)](#cardiopulmonary-resuscitation-cpr)
    - [Basic First Aid](#basic-first-aid)
    - [Advanced First Aid](#advanced-first-aid)
    - [Emergency Medicine](#emergency-medicine)
    - [Neurosurgery](#neurosurgery)
    - [Advanced Thoracic Surgery](#advanced-thoracic-surgery)
    - [Epinephrine Synthesis](#epinephrine-synthesis)
    - [Cellular Regenerative Medicine](#cellular-regenerative-medicine)
  - [New Work Types](#new-work-types)
    - [Defibrillation](#defibrillation)
    - [Airway Management](#airway-management)
    - [Performing CPR](#performing-cpr)
    - [Blood Transfusions](#blood-transfusions)
    - [Splinting Fractures](#splinting-fractures)
    - [Tourniquet Recovery](#tourniquet-recovery)
  - [Known Issues and Incompatibilities](#known-issues-and-incompatibilities)

<!-- /code_chunk_output -->

## Known Issues and Incompatibilities

This section lists known issues and incompatibilities with other mods that have been identified during testing and development of More Injuries. If you encounter any issues or incompatibilities with other mods, please report them on the [GitHub issue tracker](https://github.com/frederik-hoeft/rimworld-more-injuries/issues) with a minimal, reproducible example. - meaning the minimum set of mods in a new save game that still reproduces the issue. To allow for a quick resolution of the issue, please provide as much information as possible, including the exact steps to reproduce the issue, the expected behavior, and the actual behavior observed.

*Currently, no incompatibilities have been verifiably pinpointed to any specific mods, although there have been reports of issues with massive mod lists that may or may not be related to More Injuries.*

> [!IMPORTANT]
> When reporting an issue, please make sure to include the following information:
> - The minimal set of mods required to reproduce the issue. This list can be obtained by starting a new game with only the required mods enabled and adding the mods back one by one until the issue reappears. This process may be sped up by using any helpful information from the game logs.
> - The exact steps necessary to reproduce the issue *in a new save game*. This information is crucial to help the developers understand the issue and find a solution.
> - The expected behavior and the actual behavior observed. This information helps the developers to understand the issue and verify that the issue has been resolved.
> - Any stack traces, error messages, or other relevant information from the game logs that may help to identify the cause of the issue. If possible, please load the [Visual Exceptions](https://steamcommunity.com/sharedfiles/filedetails/?id=2538411704) mod which will help indentify integration issues with other mods.

> [!CAUTION]
> Before reporting exceptions or errors from the debug console, please make sure that at least one line of the stack trace mentions `MoreInjuries` or that you are able to prove that the issue is caused by More Injuries. Otherwise, the issue may not be related to More Injuries and is unlikely to be resolved by the developers for lack of avenues to investigate the issue.

[^1]: Some items have the potential to be reusable and they will not necessarily be consumed during a surgery or procedure. However, there is a chance that the item will break after each use, either due to wear and tear or due to the nature of the procedure. The chance to break is indicated by the **Chance to Break** value in the item's description.

[^2]: see [Surgery Success Chance Factor](https://rimworldwiki.com/wiki/Surgery_Success_Chance_Factor) on the RimWorld Wiki for more information on how the success chance of surgeries is calculated in the base game.

[^3]: see [Doctoring#Failure](https://rimworldwiki.com/wiki/Doctoring#Failure) on the RimWorld Wiki for more information on how the chance of death on failed surgeries is calculated in the base game.

[^4]: see [Work Types](https://rimworldwiki.com/wiki/Work#Work_types) on the RimWorld Wiki.

[^5]: Priority in Type indicates the priority of the work type within the parent work type. The higher the number, the higher the priority of the work type.