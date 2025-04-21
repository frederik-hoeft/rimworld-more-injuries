# Steam Workshop Docs

This directory contains all the user-facing documentation that is displayed on the Steam Workshop page of the mod. It includes the mod description, images used in the description, and images highlighting the features of the mod that are displayed in the image gallery on the Steam Workshop page.

## Directory Structure

- `README.md`: You are here, this file only contains documentation for developers and maintainers. It is not published on the Steam Workshop.
- `workshop.md`: Serves as a basis for layouting the content of the Steam Workshop page.  
This markdown file is used for layouting the content of the Steam Workshop page when we want to make changes to the mod description, features, or other information. When you are happy with the changes, you can then use it as a reference for updating `workshop.txt` which replaces the markdown formatting with the proprietary formatting required by the Steam Workshop.
- `workshop.txt`: The workshop description of the mod in a format that is compatible with the Steam Workshop.  
This file is based on `workshop.md` but with the markdown formatting replaced by the proprietary formatting required by the Steam Workshop. With every release, the contents of this file are (manually) copied into the Steam Workshop editor to update the mod description.
- `images/`: Contains inline images that are rendered in the mod description on the Steam Workshop page. **Make sure that image links in the `workshop.*` files point to the `main` branch of the repository.** Otherwise, the image links will break once your branch is merged into `main`.
- `gallery/`: Contains images that are displayed in the image gallery on the Steam Workshop page. These images are used to highlight the features of the mod and provide additional visual information to the users. They are manually uploaded to the Steam Workshop editor when changes are made. Make sure to include a post-fix number in the image name to ensure maintainers can easily identify the suggested order of the images in the gallery. The suggested format is `overview-01.png`, `feature-X-02.png`, `feature-Y-03.png`, etc., where `X` and `Y` are the primary features of the mod that are highlighted in the image.

> [!NOTE]
> The `workshop.md` file is the source of truth for the mod description and features. When making changes to the mod description or features, always update the `workshop.md` file first and then update the `workshop.txt` file accordingly once you are happy with the changes.