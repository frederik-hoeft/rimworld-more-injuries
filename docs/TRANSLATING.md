# Contributions for new Translations

We are always looking for new translations of the mod or even of the user manual to make the mod more accessible to players from all around the world. If you are interested in contributing a new translation, you can follow the steps below to set up the translation files, ensure that no strings are missing, and test the translation in the game.

## Getting Started

First, make sure that the language you want to translate the mod into is not already supported. You can check the [GitHub repository](https://github.com/frederik-hoeft/rimworld-more-injuries/tree/main/Languages) for a list of supported languages and their respective translation files. If the language is not listed, you can get started with contributing a new translation. To do this, please make yourself familiar with the [contribution guidelines](/CONTRIBUTING.md) of this project.

Once you have created the GitHub issue and received approval from the maintainers, you can start working on the translation. It is recommended to set up a local development environment as described in the [INSTALL.md](/INSTALL.md#compiling-from-source) file to verify all translation keys are present and to test the translation in the game.

## Translating the Mod

The primary language of the mod is English, and all translation files must be based on the English version. To start translating the mod, follow these steps:

1. Create a new folder in the `Languages` directory with the name of the language you are translating to (e.g., `German`, `French`, `Spanish`). See https://rimworldwiki.com/wiki/Modding_Tutorials/Localization and the RimWorld source files for the correct language names.
2. Copy the contents of the `English` folder into the new folder.
3. Translate all the XML string values in the new folder to the target language. Make sure to keep the XML structure intact and only change the string values. Leave the comments in English to help other contributors understand the context of the strings.
4. To verify that all translation keys are present and that the translation works correctly in the game, follow the steps in the [INSTALL.md](/INSTALL.md#compiling-from-source) file to set up the development environment and use the Visual Studio Unit Test runner to execute all tests. If any tests fail, make sure to correct the translation files accordingly.
5. Once you have completed the translation and verified that it works correctly in the game, commit the changes to your fork and create a pull request as described in the [CONTRIBUTING.md](/CONTRIBUTING.md) file.