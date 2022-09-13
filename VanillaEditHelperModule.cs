using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Mod.Meta;
using Monocle;

namespace Celeste.Mod.VanillaEditHelper {
    public class VanillaEditHelperModule : EverestModule {
        public static VanillaEditHelperModule Instance { get; private set; }

        private static Dictionary<AreaData, VanillaEditMeta> VanillaEditMetadata = new Dictionary<AreaData, VanillaEditMeta>();

        #region Constants

        private static readonly FieldInfo FallbackField = typeof(Dialog).GetField("FallbackLanguage", BindingFlags.Static | BindingFlags.NonPublic);
        internal static Language Fallback => (Language) FallbackField.GetValue(null);

        private static readonly HashSet<string> VanillaDialogKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "CH0_INTRO",
            "CH0_GRANNY",
            "CH0_END",
            "CH1_THEO_A",
            "CH1_THEO_B",
            "CH1_THEO_C",
            "CH1_THEO_D",
            "CH1_THEO_E",
            "CH1_THEO_F",
            "MEMORIAL",
            "CH1_END",
            "CH2_BADELINE_INTRO",
            "CH2_DREAM_PHONECALL",
            "CH2_THEO_INTRO_NEVER_MET",
            "CH2_THEO_INTRO_NEVER_INTRODUCED",
            "CH2_THEO_A_EXT",
            "CH2_THEO_A",
            "CH2_THEO_B",
            "CH2_THEO_C",
            "CH2_THEO_D",
            "CH2_THEO_E",
            "CH2_END_PHONECALL",
            "CH2_JOURNAL",
            "CH2_POEM",
            "CH3_OSHIRO_FRONT_DESK",
            "CH3_GUESTBOOK",
            "CH3_OSHIRO_HALLWAY_A",
            "CH3_OSHIRO_HALLWAY_B",
            "CH3_MEMO_OPENING",
            "CH3_MEMO",
            "CH3_OSHIRO_CLUTTER0",
            "CH3_OSHIRO_CLUTTER0_B",
            "CH3_OSHIRO_CLUTTER1",
            "CH3_OSHIRO_CLUTTER1_B",
            "CH3_OSHIRO_CLUTTER2",
            "CH3_OSHIRO_CLUTTER2_B",
            "CH3_OSHIRO_CLUTTER_ENDING",
            "CH3_THEO_NEVER_MET",
            "CH3_THEO_NEVER_INTRODUCED",
            "CH3_THEO_INTRO",
            "CH3_THEO_ESCAPING",
            "CH3_THEO_VENTS",
            "CH3_OSHIRO_BREAKDOWN",
            "CH3_DIARY",
            "CH3_OSHIRO_SUITE",
            "CH3_OSHIRO_SUITE_SAD0",
            "CH3_OSHIRO_SUITE_SAD1",
            "CH3_OSHIRO_SUITE_SAD2",
            "CH3_OSHIRO_SUITE_SAD3",
            "CH3_OSHIRO_SUITE_SAD4",
            "CH3_OSHIRO_SUITE_SAD5",
            "CH3_OSHIRO_SUITE_SAD6",
            "CH3_OSHIRO_START_CHASE",
            "CH3_OSHIRO_CHASE_END",
            "CH3_ENDING",
            "CH4_GRANNY_1",
            "CH4_GRANNY_2",
            "CH4_GRANNY_3",
            "CH4_GONDOLA",
            "CH4_GONDOLA_FEATHER_1",
            "CH4_GONDOLA_FEATHER_2",
            "CH4_GONDOLA_FEATHER_3",
            "CH4_GONDOLA_FEATHER_4",
            "CH4_GONDOLA_FEATHER_5",
            "CH5_ENTRANCE",
            "CH5_PHONE",
            "CH5_THEO_MIRROR",
            "CH5_SHADOW_MADDY_0",
            "CH5_SHADOW_MADDY_1",
            "CH5_SHADOW_MADDY_2",
            "CH5_SHADOW_MADDY_3",
            "CH5_REFLECTION",
            "CH5_SEE_THEO",
            "CH5_SEE_THEO_B",
            "CH5_FOUND_THEO",
            "CH5_THEO_SAVING_LIFT",
            "CH5_THEO_SAVING_A",
            "CH5_THEO_SAVING_NICE",
            "CH5_THEO_SAVING_B",
            "CH5_THEO_SAVING_C",
            "CH5_THEO_SAVING_SWITCH",
            "CH5_THEO_SAVING_DUNK",
            "CH5_THEO_SAVING_REALLY",
            "CH5_THEO_SAVING_D",
            "CH5_THEO_SAVING_E",
            "CH5_BSIDE_THEO_A",
            "CH5_BSIDE_THEO_B",
            "CH5_BSIDE_THEO_C",
            "CH6_THEO_INTRO",
            "CH6_THEO_ASK_OUTFOR",
            "CH6_THEO_SAY_OUTFOR",
            "CH6_THEO_ASK_EXPLAIN",
            "CH6_THEO_SAY_EXPLAIN",
            "CH6_THEO_ASK_THANKYOU",
            "CH6_THEO_SAY_THANKYOU",
            "CH6_THEO_ASK_TRUST",
            "CH6_THEO_SAY_TRUST",
            "CH6_THEO_ASK_WHY",
            "CH6_THEO_SAY_WHY",
            "CH6_THEO_ASK_DEPRESSION",
            "CH6_THEO_SAY_DEPRESSION",
            "CH6_THEO_ASK_DEFENSE",
            "CH6_THEO_SAY_DEFENSE",
            "CH6_THEO_ASK_VACATION",
            "CH6_THEO_SAY_VACATION",
            "CH6_THEO_ASK_FAMILY",
            "CH6_THEO_SAY_FAMILY",
            "CH6_THEO_ASK_GRANDPA",
            "CH6_THEO_SAY_GRANDPA",
            "CH6_THEO_ASK_TIPS",
            "CH6_THEO_SAY_TIPS",
            "CH6_THEO_ASK_TEMPLE",
            "CH6_THEO_SAY_TEMPLE",
            "CH6_THEO_ASK_SELFIE",
            "CH6_THEO_SAY_SELFIE",
            "CH6_THEO_ASK_SLEEP",
            "CH6_THEO_SAY_SLEEP",
            "CH6_THEO_ASK_SLEEP_CONFIRM",
            "CH6_THEO_SAY_SLEEP_CONFIRM",
            "CH6_THEO_ASK_SLEEP_CANCEL",
            "CH6_THEO_SAY_SLEEP_CANCEL",
            "CH6_DREAMING",
            "CH6_THEO_WATCHOUT",
            "CH6_REFLECT_AFTER",
            "CH6_OLDLADY",
            "CH6_OLDLADY_B",
            "CH6_OLDLADY_C",
            "CH6_BOSS_START",
            "CH6_BOSS_TIRED_A",
            "CH6_BOSS_TIRED_B",
            "CH6_BOSS_TIRED_C",
            "CH6_BOSS_MIDDLE",
            "CH6_BOSS_ENDING",
            "CH6_ENDING",
            "CH7_HEIGHT_START",
            "CH7_HEIGHT_0",
            "CH7_HEIGHT_1",
            "CH7_HEIGHT_2",
            "CH7_HEIGHT_3",
            "CH7_HEIGHT_4",
            "CH7_HEIGHT_5",
            "CH7_ASCEND_0",
            "CH7_ASCEND_1",
            "CH7_ASCEND_2",
            "CH7_ASCEND_3",
            "CH7_ASCEND_4",
            "CH7_ASCEND_5",
            "CH7_ENDING",
            "CH7_BSIDE_ASCEND",
            "CH7_CSIDE_OLDLADY",
            "CH7_CSIDE_OLDLADY_B",
            "EP_CABIN",
            "EP_PIE_START",
            "EP_PIE_DISAPPOINTED",
            "EP_PIE_GROSSED_OUT",
            "EP_PIE_OKAY",
            "EP_PIE_REALLY_GOOD",
            "EP_PIE_AMAZING",
            "APP_INTRO",
            "APP_OLDLADY_A",
            "APP_OLDLADY_B",
            "APP_OLDLADY_C",
            "APP_OLDLADY_D",
            "APP_OLDLADY_E",
            "APP_OLDLADY_LOCKED",
            "APP_ENDING",
            "CH9_GRAVESTONE",
            "CH9_FLYING_TO_THE_MOON",
            "CH9_LANDING",
            "CH9_FAKE_HEART",
            "CH9_KEEP_GOING",
            "CH9_MISS_THE_BIRD",
            "CH9_HELPING_HAND",
            "CH9_CATCH_THE_BIRD",
            "CH9_LAST_ROOM",
            "CH9_LAST_ROOM_ALT",
            "CH9_LAST_BOOST",
            "CH9_FAREWELL",
            "CH9_END_CINEMATIC",
            "WAVEDASH_DESKTOP_MYPC",
            "WAVEDASH_DESKTOP_POWERPOINT",
            "WAVEDASH_DESKTOP_RECYCLEBIN",
            "WAVEDASH_DESKTOP_STARTBUTTON",
            "WAVEDASH_PAGE1_TITLE",
            "WAVEDASH_PAGE1_SUBTITLE",
            "WAVEDASH_PAGE2_TITLE",
            "WAVEDASH_PAGE2_LIST",
            "WAVEDASH_PAGE2_IMPOSSIBLE",
            "WAVEDASH_PAGE3_TITLE",
            "WAVEDASH_PAGE3_INFO",
            "WAVEDASH_PAGE3_EASY",
            "WAVEDASH_PAGE4_TITLE",
            "WAVEDASH_PAGE4_LIST",
            "WAVEDASH_PAGE5_TITLE",
            "WAVEDASH_PAGE5_INFO1",
            "WAVEDASH_PAGE5_INFO2",
            "WAVEDASH_PAGE6_TITLE",
        };

        #endregion

        public VanillaEditHelperModule() {
            Instance = this;
        }

        public override void Load() {
            On.Celeste.AreaData.Load += AreaDataOnLoad;
            On.Celeste.Dialog.Get += DialogOnGet;
            On.Celeste.Dialog.Clean += DialogOnClean;

            //On.Celeste.BadelineOldsite.Added += BadelineOldsiteOnAdded;
        }
        
        public override void Unload() {
            On.Celeste.AreaData.Load -= AreaDataOnLoad;
            On.Celeste.Dialog.Get -= DialogOnGet;
            On.Celeste.Dialog.Clean -= DialogOnClean;

            //On.Celeste.BadelineOldsite.Added -= BadelineOldsiteOnAdded;
        }
        
        #region Meta
        
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
                else
                {
                    VanillaEditMetadata.Remove(map);
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

        #region Dialog

        private static string DialogOnGet(On.Celeste.Dialog.orig_Get orig, string name, Language language)
        {
            string prefix = CurrentMeta?.DialogPrefix;
            if (string.IsNullOrWhiteSpace(prefix) || !VanillaDialogKeys.Contains(name))
            {
                return orig(name, language);
            }
            language ??= Dialog.Language;
            Language desiredLanguage = CurrentMeta.ModifiedLanguages.Contains(language.Id, StringComparer.InvariantCultureIgnoreCase) ? language : CurrentMeta.ModifiedLanguages.Contains(Fallback.Id, StringComparer.InvariantCultureIgnoreCase) ? Fallback : null;
            if (desiredLanguage == null)
            {
                return orig(name, language);
            }
            name = name.DialogKeyify();
            if (desiredLanguage.Dialog.TryGetValue(prefix.DialogKeyify() + name, out string modifiedText))
            {
                return modifiedText;
            }
            return desiredLanguage.Dialog.TryGetValue(name, out string vanillaText) ? vanillaText : $"[{name}]";
        }
        
        private static string DialogOnClean(On.Celeste.Dialog.orig_Clean orig, string name, Language language)
        {
            string prefix = CurrentMeta?.DialogPrefix;
            if (name == "memorial")
            {
                Console.WriteLine("Get:");
                foreach (var key in Fallback.Dialog.Keys)
                {
                    Console.WriteLine(key);
                }
                Console.WriteLine("Clean:");
                foreach (var key in Fallback.Cleaned.Keys)
                {
                    Console.WriteLine(key);
                }
            }
            if (string.IsNullOrWhiteSpace(prefix) || !VanillaDialogKeys.Contains(name))
            {
                return orig(name, language);
            }
            language ??= Dialog.Language;
            Language desiredLanguage = CurrentMeta.ModifiedLanguages.Contains(language.Id, StringComparer.InvariantCultureIgnoreCase) ? language : CurrentMeta.ModifiedLanguages.Contains(Fallback.Id, StringComparer.InvariantCultureIgnoreCase) ? Fallback : null;
            if (desiredLanguage == null)
            {
                return orig(name, language);
            }
            name = name.DialogKeyify();
            if (desiredLanguage.Cleaned.TryGetValue(prefix.DialogKeyify() + name, out string modifiedText))
            {
                return modifiedText;
            }
            return desiredLanguage.Cleaned.TryGetValue(name, out string vanillaText) ? vanillaText : $"{{{name}}}";
        }     

        #endregion
        
        #region chapter2
        
        private static void BadelineOldsiteOnAdded(On.Celeste.BadelineOldsite.orig_Added orig, BadelineOldsite self, Scene scene)
        {
            VanillaEditMeta meta = CurrentMeta;
            if (!meta.BadelineStartingCutscene)
            {
                orig(self, scene);
            }
        }
        
        #endregion
    }

    public class VanillaEditMeta : IMeta
    {
        public string DialogPrefix { get; set; } = null;
        public string[] ModifiedLanguages { get; set; } = {VanillaEditHelperModule.Fallback.Id};
        public int? TreatAsChapter { get; set; }
        public bool BadelineStartingCutscene { get; set; } = true;
    }
}