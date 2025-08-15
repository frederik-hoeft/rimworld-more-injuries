# Vasodilation

<!-- @generate_breadcrumb_trail {"template": "_:file_folder: {0}_", "connector": " :arrow_right: "} -->
_:file_folder: [More Injuries User Manual](/docs/wiki/README.md) :arrow_right: [Injuries and Medical Conditions A-Z](/docs/wiki/injuries/README.md) :arrow_right: [Vasodilation](/docs/wiki/injuries/vasodilation.md)_
<!-- @end_generated_block -->

> **In-Game Description**
> _"**Vasodilation** &mdash; Vasodilation, also known as vasorelaxation, is the widening of blood vessels, which decreases vascular resistance and increases blood flow. Due to this increase in blood flow, hemorrhagic resistance is reduced, leading to increased bleeding.  
> Vasodilation can be caused by various factors, including the use of certain opioid analgesics like morphine, which suppress the central nervous system and can lead to dangerous hypotension and shock."_

```mermaid
---
config:
  flowchart:
    htmlLabels: true
---
flowchart LR
  morphine_high[morphine high] ==> vasodilation[vasodilation]
  vasodilation ==> hemorrhagic_resistance[increased bleed rates]

  linkStyle 0,1 stroke: #b10000
  style vasodilation stroke-width: 4px
```

*See the section on the [pathophysiological system](/docs/wiki/pathophysiological-system.md#pathophysiological-system) for more information on the graphical representation.*

**Causes**: [Injecting](/docs/wiki/medical-devices.md#morphine-autoinjector) [morphine](/docs/wiki/injuries/morphine-high.md#morphine-high) into a patient's bloodstream.

**Effects**: Widening of blood vessels, decreased vascular resistance, increased blood flow, decreased cardiac output [(1)](https://www.accessdata.fda.gov/drugsatfda_docs/label/2011/202515s000lbl.pdf#:~:text=The%20vasodilation%20produced%20by%20Morphine,in%20patients%20in%20circulatory%20shock), reduced hemorrhagic resistance, and increased bleed rates ([Watso et al. (2022)](https://pubmed.ncbi.nlm.nih.gov/35452317/)), which is simulated as an up to 100% increase in bleeding rates, directly proportional to the severity of vasodilation.

**Treatment**: Vasodilation is a natural response to certain drugs like [morphine](/docs/wiki/injuries/morphine-high.md#morphine-high), and its effects are typically temporary, resolving as the drug wears off. In the game, there is no specific treatment for vasodilation, although preventative measures can be taken to prevent complications (such as not pumping hypovolemic patients full of morphine).

<!-- @generate_link_to_top {"template": "---\n_[back to the top]({1})_"} -->
---
_[back to the top](#vasodilation)_
<!-- @end_generated_block -->
