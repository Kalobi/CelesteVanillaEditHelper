using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Celeste.Mod.Meta;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.VanillaEditHelper {
    public class VanillaEditHelperModule : EverestModule {
        public static VanillaEditHelperModule Instance { get; private set; }

        private static Dictionary<AreaData, VanillaEditMeta> VanillaEditMetadata = new Dictionary<AreaData, VanillaEditMeta>();

        private static HashSet<string> VanillaDialogKeys = new HashSet<string>();

        public VanillaEditHelperModule() {
            Instance = this;
        }

        private Hook hookDialog_origLoadLanguage;
        public override void Load() {
            On.Celeste.AreaData.Load += AreaDataOnLoad;
            On.Celeste.Dialog.Get += DialogOnGet;
            On.Celeste.Dialog.Clean += DialogOnClean;
            hookDialog_origLoadLanguage = new Hook(typeof(Dialog).GetMethod("orig_LoadLanguage"),
                typeof(VanillaEditHelperModule).GetMethod(nameof(DialogOnOrigLoadLanguage), BindingFlags.NonPublic | BindingFlags.Static));
        }
        
        public override void Unload() {
            On.Celeste.AreaData.Load -= AreaDataOnLoad;
            On.Celeste.Dialog.Get -= DialogOnGet;
            On.Celeste.Dialog.Clean -= DialogOnClean;
            hookDialog_origLoadLanguage?.Dispose();
        }
        
        #region meta
        
        private static void AreaDataOnLoad(On.Celeste.AreaData.orig_Load orig)
        {
            orig();
            foreach (AreaData map in AreaData.Areas)
            {
                if (Everest.Content.TryGet("Maps/" + map.Mode[0].Path + ".vanillaedithelper.meta",
                    out ModAsset metadata) && metadata.TryDeserialize(out VanillaEditMeta meta))
                {
                    VanillaEditMetadata[map] = meta;
                }
            }
        }

        private static VanillaEditMeta GetMetaForAreaData(AreaData area)
        {
            if (area == null || !VanillaEditMetadata.ContainsKey(area))
            {
                return null;
            }

            return VanillaEditMetadata[area];
        }
        private static VanillaEditMeta CurrentMeta => GetMetaForAreaData(AreaData.Get(
            ((Engine.Scene as Level ?? (Engine.Scene as LevelLoader)?.Level)?.Session.Area).GetValueOrDefault()));

        #endregion

        #region dialog

        private static Language DialogOnOrigLoadLanguage(Func<string, Language> orig, string filename)
        {
            Language language = orig(filename);
            VanillaDialogKeys.UnionWith(language.Dialog.Keys);
            return language;
        }
        
        private static string DialogOnGet(On.Celeste.Dialog.orig_Get orig, string name, Language language)
        {
            if (string.IsNullOrEmpty(name) || !(Regex.IsMatch(name, @"^CH\d_") || name.StartsWith("APP_")) || !VanillaDialogKeys.Contains(name))
            {
                return orig(name, language);
            }
            string prefix = CurrentMeta?.DialogPrefix;
            if (string.IsNullOrEmpty(prefix))
            {
                return orig(name, language);
            }
            
            language ??= Dialog.Language;

            if (!CurrentMeta.ModifiedLanguages.Contains(language.Id, StringComparer.InvariantCultureIgnoreCase))
            {
                return orig(name, language);
            }
            
            name = name.DialogKeyify();
            
            if (language.Dialog.TryGetValue(prefix + name, out string modifiedText))
            {
                return modifiedText;
            }

            return language.Dialog.TryGetValue(name, out string vanillaText) ? vanillaText : $"[{name}]";
        }
        
        private static string DialogOnClean(On.Celeste.Dialog.orig_Clean orig, string name, Language language)
        {
            if (string.IsNullOrEmpty(name) || !(Regex.IsMatch(name, @"^CH\d_") || name.StartsWith("APP_")) || !VanillaDialogKeys.Contains(name))
            {
                return orig(name, language);
            }
            string prefix = CurrentMeta?.DialogPrefix;
            if (string.IsNullOrEmpty(prefix))
            {
                return orig(name, language);
            }
            
            language ??= Dialog.Language;

            if (!CurrentMeta.ModifiedLanguages.Contains(language.Id, StringComparer.InvariantCultureIgnoreCase))
            {
                return orig(name, language);
            }
            
            name = name.DialogKeyify();
            
            if (language.Cleaned.TryGetValue(prefix + name, out string modifiedText))
            {
                return modifiedText;
            }

            return language.Dialog.TryGetValue(name, out string vanillaText) ? vanillaText : $"{{{name}}}";
        }     

        #endregion
    }

    public class VanillaEditMeta : IMeta
    {
        public string DialogPrefix { get; set; } = null;
        public string[] ModifiedLanguages { get; set; } = {"english"};
        public int? TreatAsChapter { get; set; }
    }
}