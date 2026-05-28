#!/usr/bin/env python3

import numpy as np
import matplotlib.pyplot as plt

output_path = "/lab/workspace/plots/cpr_choking_reduction_by_medical_skill.png"
title = "CPR choking severity reduction range by medical skill"
lower_min, upper_min = -0.05, 0.35
lower_max, upper_max = 0.15, 0.60

def logistic(x, midpoint=10.0, sharpness=0.2):
    return 1.0 / (1.0 + np.exp(-sharpness * (x - midpoint)))

def inverse_hill_factor(x, half_effect=2.0, exponent=2.0):
    x = np.maximum(x, 0.0)
    return 1.0 / (1.0 + (x / half_effect) ** exponent)

def lerp(a, b, t):
    return a + (b - a) * t

skills = np.linspace(0.0, 30.0, 601)

treatment_effectiveness = logistic(skills) * (1.0 - inverse_hill_factor(skills))

minimum_removal = lerp(lower_min, upper_min, treatment_effectiveness)
maximum_removal = lerp(lower_max, upper_max, treatment_effectiveness)
expected_removal = (minimum_removal + maximum_removal) / 2.0

plt.figure(figsize=(9, 5.5))
plt.plot(skills, minimum_removal * 100.0, label="Minimum")
plt.plot(skills, expected_removal * 100.0, label="Expected average")
plt.plot(skills, maximum_removal * 100.0, label="Maximum")
plt.axvline(20.0, linestyle="--", linewidth=1, label="Vanilla max skill")
plt.xlabel("Medical skill")
plt.ylabel("Severity reduction")
plt.title(title)
plt.legend()
plt.grid(True, alpha=0.3)
plt.tight_layout()

plt.savefig(output_path, dpi=160)
