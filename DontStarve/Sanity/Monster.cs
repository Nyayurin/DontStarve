﻿using StardewModdingAPI;
using StardewValley;

namespace DontStarve.Sanity;

internal static class Monster {
    private static Dictionary<string, double> monsterSanity = null!;
    private static long wait;
    
    internal static void init(IModHelper helper) {
        monsterSanity = helper.ModContent.Load<Dictionary<string, double>>("assets/sanity/monster.json");
    }

    internal static void update(long _) {
        if (wait > 0) {
            wait--;
            return;
        }
        
        var player = Game1.player;
        var location = Game1.currentLocation;
        var playerPosition = player.Tile;
        var value = 0.0;
        foreach (var monster in location.characters.Where(npc => npc is StardewValley.Monsters.Monster)) {
            var monsterPosition = monster.Tile;
            var distance = Math.Sqrt(
                Math.Pow(monsterPosition.X - playerPosition.X, 2) +
                Math.Pow(monsterPosition.Y - playerPosition.Y, 2)
            );
            var percentage = 1 - distance / 10;
            if (percentage > 0) {
                value += monsterSanity.GetValueOrDefault(monster.Name, 0) * percentage;
            }
        }
        
        if (value > 0) {
            player.setSanity(player.getSanity() - value);
        }
    }

    internal static void sync(long time, long delta) {
        if (delta < 0) {
            wait += -delta;
        } else {
            for (var i = 0; i < delta; i++) {
                update(time);
            }
        }
    }
    
    internal static void load(IModHelper helper) {
        var data = helper.Data.ReadSaveData<MonsterData>("DontStarve.Sanity.Monster");
        wait = data?.wait ?? 0;
    }

    internal static void save(IModHelper helper) {
        helper.Data.WriteSaveData("DontStarve.Sanity.Monster", new MonsterData {
            wait = wait
        });
    }
}

internal class MonsterData {
    internal long wait { get; init; }
}