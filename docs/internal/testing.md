# Test Checklist

Features and changes that require manual testing in-game should be added to this checklist. This will help ensure that all necessary testing is completed before a feature is released.

- [x] #166 source generated XML bindings work correctly
- [ ] #111 No more float menus on hostile pawns
- [ ] #112 Adjustable defibrillator break chance in settings
- [ ] #176 Artificial lungs are exempt from lung collapse (maybe use `IsBetterThanNatural` instead of checking for any added parts?)
- [ ] #174 pawns with Breathless gene are more resistant to hypoxia and suffocation:
    - [ ] hypoxia
    - [ ] choking on blood/tourniquet
    - [ ] tourniquet-related ischemia/gangrene
- [ ] #178 choking hediff now correctly integrates with DeathRattle and the Deathless gene:
    - [x] with neither, choking should cause death after a certain time
    - [ ] with DeathRattle, choking should trigger the DeathRattle effect instead of causing death
    - [ ] with the Deathless gene, choking should not cause death but should still cause other negative effects (e.g. reduced movement speed, increased pain)
- [ ] #188 enhanced choking simulation is:
    - [x] working
    - [ ] balanced (not too punishing, not too easy)
    - [ ] interesting (adds depth to gameplay, not just a tedious micromanagement task)
    - [ ] can be treated with appropriate medical interventions
    - [ ] sounds are fine and not overlapping with other sounds
- [ ] ??? better injury state descriptions are now more detailed and informative
- [ ] ??? bloodloss still kills:
    - [x] when kill-by-hypoxia is enabled
    - [ ] when kill-by-hypoxia is disabled