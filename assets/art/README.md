# `/assets/art` Directory Structure

This directory contains all the source files for the texture art and other assets used in the game. This document will provide a brief overview of what to find where.

- `DALLE-raw` contains the raw images of some textures that were created using generative AI models like DALL-E. Images in this directory are not directly used in the game, but are cleaned up and processed to create the final textures.
- `Source` contains the project files of the different image processing suites used to create the final textures. As of now, the choice of software is free to the artist, as long as the common file formats are supported. This is where you find the texture you are looking for:
    - bandages: `AdobeIllustrator/MoreInjuriesSource.ai`
    - blood bags: `AdobeIllustrator/MoreInjuriesSource.ai`
    - defibrillator: `AdobeIllustrator/MoreInjuriesSource.ai`
    - epinephrine: `AdobeIllustrator/MoreInjuriesSource.ai`
    - generic uncolored injector (currently unused): `GIMP/injector_neutral_*.xcf`
    - hemostatic agent: `AdobeIllustrator/MoreInjuriesSource.ai`
    - morphine (currently unused): `GIMP/injector_red_*.xcf`
    - splints: `AdobeIllustrator/MoreInjuriesSource.ai`
    - thorascope: `AdobeIllustrator/MoreInjuriesSource.ai`
    - tourniquet: `AdobeIllustrator/MoreInjuriesSource.ai`
    - saline bags:  `AdobeIllustrator/MoreInjuriesSource.ai`
    - thumbnail art and workshop header images:  `AdobeIllustrator/MoreInjuriesThumbnail.ai`
- `Sfx` contains some raw sound effects that may be used in future updates of the mod. These are not finalized and require some further processing before they can be used in the game.

> [!NOTE]
> Please update this document if you add new textures or change the directory structure.