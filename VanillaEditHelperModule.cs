using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.VanillaEditHelper {
    public class VanillaEditHelperModule : EverestModule {
        public static VanillaEditHelperModule Instance { get; private set; }

        public VanillaEditHelperModule() {
            Instance = this;
        }

        public override void Load() {
            // TODO: apply any hooks that should always be active
        }

        public override void Unload() {
            // TODO: unapply any hooks applied in Load()
        }
    }
}