using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeslasuitAlyx
{
    public enum HLAlyxDamageTypeMask : int
    {
        DMG_GENERIC                 = 0,
        DMG_CRUSH                   = 1,
        DMG_BULLET                  = 2,
        DMG_SLASH                   = 4,
        DMG_BURN                    = 8,
        DMG_VEHICLE                 = 16,
        DMG_FALL                    = 32,
        DMG_BLAST                   = 64,
        DMG_CLUB                    = 128,
        DMG_SHOCK                   = 256,
        DMG_SONIC                   = 512,
        DMG_ENERGYBEAM              = 1024,
        DMG_PREVENT_PHYSICS_FORCE   = 2048,
        DMG_NEVERGIB                = 4096,
        DMG_ALWAYSGIB               = 8192,
        DMG_DROWN                   = 16384,
        DMG_PARALYZE                = 32768,
        DMG_NERVEGAS                = 65536,
        DMG_POISON                  = 131072,
        DMG_RADIATION               = 262144,
        DMG_DROWNRECOVER            = 524288,
        DMG_ACID                    = 1048576,
        DMG_SLOWBURN                = 2097152,
        DMG_REMOVENORAGDOLL         = 4194304,
        DMG_PHYSGUN                 = 8388608,
        DMG_PLASMA                  = 16777216,
        DMG_AIRBOAT                 = 33554432,
        DMG_DISSOLVE                = 67108864,
        DMG_BLAST_SURFACE           = 134217728,
        DMG_DIRECT                  = 268435456,
        DMG_BUCKSHOT                = 536870912
    }

    public static class HLAlyxDamageTypeExtensions
    {
        public static bool Contains(this HLAlyxDamageTypeMask mask, HLAlyxDamageTypeMask value)
        {
            return ((int)value & (int)mask) == (int)value;
        }

        public static List<HLAlyxDamageTypeMask> ToList(this HLAlyxDamageTypeMask mask)
        { 
            List<HLAlyxDamageTypeMask> items = new List<HLAlyxDamageTypeMask>();

            var allItems = Enum.GetValues(typeof(HLAlyxDamageTypeMask));

            foreach (HLAlyxDamageTypeMask item in allItems)
            {
                if(mask.Contains(item))
                {
                    items.Add(item);
                }
            }
            return items;
        }

        public static void ForEach(this HLAlyxDamageTypeMask mask, Action<HLAlyxDamageTypeMask> iterable)
        {
            var allItems = Enum.GetValues(typeof(HLAlyxDamageTypeMask));
            foreach (HLAlyxDamageTypeMask item in allItems)
            {
                if(mask.Contains(item))
                {
                    iterable(item);
                }
            }
        }
    }
}
