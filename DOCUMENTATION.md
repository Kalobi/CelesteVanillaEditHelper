# Vanilla Edit Helper
Vanilla Edit Helper is a helper mod that allows mappers to create campaigns consisting of edited vanilla maps while fixing some of the hardcoded things that make those levels hard to work with. Its eventual goal is to be able to reproduce vanilla map behavior as closely as possible. However, it is currently **early in development**. I will try to avoid breaking changes, but cannot guarantee there will not be any. The behavior for B- and C-Sides in particular has not been figured out yet and will probably change. If you're using this mod and are worried about breaking changes, let me know (@Kalobi#2278 on the [Celeste Discord](https://discord.gg/celeste)) and I will warn you of any breaking changes in advance. **Any features not documented on this page are unfinished or broken and subject to change without notice.**
## Introduction
### What this mod is not for
- This mod is not for vanilla style campaigns with their own level structure, like D-Sides. These mods' goals can generally be achieved using the [Vanilla Metadata](https://github.com/EverestAPI/Resources/wiki/Vanilla-Metadata) listed on the Everest Wiki. If you plan to make a campaign of this kind, create your maps from scratch.
- This mod is not meant for things that Everest can already change easily. This includes:
  * Text with automatically generated dialog IDs that can be set easily as conventional [custom dialogue](https://github.com/EverestAPI/Resources/wiki/Adding-Custom-Dialogue), such as checkpoint names and heart poems.
  * Anything that can be set via Metadata, such as inventory, music, and endscreens. For these, refer to the [Vanilla Metadata Reference](https://github.com/EverestAPI/Resources/wiki/Vanilla-Metadata) and use the .meta.yaml files directly or transfer the desired properties into the map editor.
### What this mod is for
This mod is for campaigns that consist of modified versions of vanilla levels, such as [Celeste on the Moon](https://gamebanana.com/mods/368495). Some properties may expect the map to have the same global structure (room names, sizes, and positions) as the vanilla map.

## Setup
To set up your mod to use Vanilla Edit Helper:
1. Make sure your [mod structure](https://github.com/EverestAPI/Resources/wiki/Mod-Structure) is set up correctly.
2. Add `VanillaEditHelper` to your [everest.yaml](https://github.com/EverestAPI/Resources/wiki/everest.yaml-Setup) file as a dependency.
3. Create a `.vanillaedithelper.meta.yaml` file with the name of your map's A-side **(behavior re: B- and C-Sides subject to change)** in the same folder as your map file. For example, if your map is called `1-examplemap.bin`, create a file called `1-examplemap.vanillaedithelper.meta.yaml` in the same folder.
## Features
### Dialog Replacement
Most dialog within vanilla maps uses dialog IDs that are hardcoded within cutscenes. Adding dialog with the same dialog ID to your own dialog files will also override that dialog in the vanilla maps, which is not what you want. Vanilla Edit Helper allows you to change vanilla map dialog in a way that only affects your map. The dialog that can be overridden this way is all dialog that is map specific. This is any vanilla dialog whose ID starts with `CH0_` through `CH9_`, `APP_`, `EP_`, or `WAVEDASH_`, as well as `MEMORIAL`.
#### Basic dialog setup
First, you need to choose a dialog prefix unique to your map. The recommended format for this is `username_modname_`, where `username` is a username that you use online, such as your Discord or Gamebanana username, and `modname` is the name of your mod (ideally the name you use in your everest.yaml). For the purposes of this tutorial, I will be using `Kalobi_VanillaEditHelper_` as the dialog prefix.

Within your `.vanillaedithelper.meta.yaml` file, add the following line (using your own dialog prefix):
```yaml
DialogPrefix: Kalobi_VanillaEditHelper_
```
This tells the mod that it should use this dialog prefix for your map. Then, for any vanilla dialog within the map that you want to replace, find out the dialog ID. You can do this by checking the vanilla `English.txt` file, which can be found at `Content/Dialog/English.txt` in your Celeste install folder. Within [your mod's own English.txt file](https://github.com/EverestAPI/Resources/wiki/Adding-Custom-Dialogue#setting-up-the-dialogue-file), add a dialog key formed by adding your prefix to the start of the vanilla dialog ID, and set the dialog to whatever you want to change it to.

For example, in my copy of chapter 1, I want to change the dialog at the end so that Madeline says "Ugh, I'm exhausted from writing all this documentation." In the vanilla `English.txt`, there is the following section:
```
CH1_END=
	[MADELINE left distracted]
	Ugh, I'm exhausted.

  {trigger 0 MADELINE sits down to rest. A bird swoops in, lands on her head, and goes to sleep}

	[MADELINE left deadpan]
	{>> 0.4}This might have been a mistake.
```
To change this in my map, I add the following text to my mod's `English.txt`:
```
Kalobi_VanillaEditHelper_CH1_END=
	[MADELINE left distracted]
	Ugh, I'm exhausted from writing all this documentation.

	{trigger 0 MADELINE sits down to rest. A bird swoops in, lands on her head, and goes to sleep}

	[MADELINE left deadpan]
	{>> 0.4}This might have been a mistake.
```
#### Multiple languages
If you only want to replace English dialog for your map, you can skip this section.

Vanilla Edit Helper allows you to override dialog for any languages that include vanilla dialog. If you do not specify which languages you want to override, the list defaults to English only. To set this yourself, add a `ModifiedLanguages` list to your `.vanillaedithelper.meta.yaml` file which lists the languages you want to modify. For example, if you've written modified dialog for English and German, add the following:
```yaml
ModifiedLanguages:
    - english
    - german
```
Note that if you specify the `ModifiedLanguages`, you have to list all of them, including English. The modified dialog for the other languages is added to the appropriate custom dialog files within your mod (such as `German.txt` in this case) using the same prefixed dialog keys as above.

The dialog that the user sees within your map if you have a `DialogPrefix` set is chosen as follows:
1. If the user's current language is one of your modified languages, that language is used, showing your modified dialog where appropriate and vanilla dialog otherwise.
2. If the user's current language is not one of your modified languages, but English is, the map uses English dialog throughout, showing your modified dialog where appropriate and vanilla dialog otherwise. This mirrors the behavior if you were writing custom dialog for a regular custom map and prevents a mix of languages from appearing within the same map.
3. If your modified languages include neither the user's current language nor English, the map shows vanilla dialog in the user's selected language.
