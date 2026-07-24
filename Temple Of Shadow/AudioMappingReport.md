# Temple Of Shadow — Audio Mapping & Technical Audit

Generated from the serialized `Manager.prefab` mapping.

- SFX master volume: `0.68`
- BGM master volume: `0.50`
- Peak/RMS are measured from decoded PCM samples.
- Estimated output peak includes the SoundManager master-volume multiplier.

## Dark-fantasy preset

- Combat/movement source: low-pass at `8.5 kHz`.
- BGM source: low-pass at `12 kHz`.
- UI and pickup sounds remain unfiltered so menu feedback stays clear.
- Human enemy hurt voices were replaced by CC0 monster growls.
- Bright `Wind`, `Ice` and `Thunder` variants were removed from combat mapping.
- The comedic PS1 death variant was removed.
- Player/enemy hurt voices retain the additional `0.55` per-call multiplier when layered with impacts.

## SFX

| Event key | Audio file | Duration | Peak | RMS | Est. output peak | Technical note |
|---|---|---:|---:|---:|---:|---|
| `jump` | [30_Jump_03](Assets/_Project/Audio/RPG_Essentials_Free/12_Player_Movement_SFX/30_Jump_03.wav) | 0.87s | -21.8 dBFS | -41.0 dBFS | -25.2 dBFS | Quiet SFX; listen for audibility in combat |
| `footstep` | [03_Step_grass_03](Assets/_Project/Audio/RPG_Essentials_Free/12_Player_Movement_SFX/03_Step_grass_03.wav) | 0.67s | -20.7 dBFS | -45.3 dBFS | -24.1 dBFS | Quiet SFX; listen for audibility in combat |
| `footstep` | [08_Step_rock_02](Assets/_Project/Audio/RPG_Essentials_Free/12_Player_Movement_SFX/08_Step_rock_02.wav) | 0.67s | -15.1 dBFS | -44.4 dBFS | -18.5 dBFS | Quiet SFX; listen for audibility in combat |
| `footstep` | [12_Step_wood_03](Assets/_Project/Audio/RPG_Essentials_Free/12_Player_Movement_SFX/12_Step_wood_03.wav) | 0.67s | -19.2 dBFS | -43.1 dBFS | -22.5 dBFS | Quiet SFX; listen for audibility in combat |
| `landing` | [45_Landing_01](Assets/_Project/Audio/RPG_Essentials_Free/12_Player_Movement_SFX/45_Landing_01.wav) | 0.67s | -16.0 dBFS | -39.4 dBFS | -19.3 dBFS | Quiet SFX; listen for audibility in combat |
| `player_slash` | [22_Slash_04](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/22_Slash_04.wav) | 1.33s | -8.0 dBFS | -33.2 dBFS | -11.4 dBFS | Quiet SFX; listen for audibility in combat |
| `player_kick` | [15_Impact_flesh_02](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/15_Impact_flesh_02.wav) | 0.67s | -10.0 dBFS | -29.3 dBFS | -13.4 dBFS | Level is technically reasonable |
| `player_kick` | [77_flesh_02](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/77_flesh_02.wav) | 0.67s | -14.4 dBFS | -34.8 dBFS | -17.8 dBFS | Quiet SFX; listen for audibility in combat |
| `player_throw` | [46_Poison_01](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/46_Poison_01.wav) | 2.00s | -10.1 dBFS | -26.7 dBFS | -13.5 dBFS | Level is technically reasonable |
| `player_throw` | [45_Charge_05](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/45_Charge_05.wav) | 5.33s | -3.7 dBFS | -22.0 dBFS | -7.0 dBFS | Level is technically reasonable |
| `player_hit` | [61_Hit_03](Assets/_Project/Audio/RPG_Essentials_Free/12_Player_Movement_SFX/61_Hit_03.wav) | 0.67s | -10.7 dBFS | -33.1 dBFS | -14.0 dBFS | Quiet SFX; listen for audibility in combat |
| `player_hit` | [15_Impact_flesh_02](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/15_Impact_flesh_02.wav) | 0.67s | -10.0 dBFS | -29.3 dBFS | -13.4 dBFS | Level is technically reasonable |
| `hurt` | [grunting_1_sean](Assets/_Project/Audio/Super%20Dialogue%20Audio%20Pack%20v1/Super%20Dialogue%20Audio%20Pack%20v1/Step%202%20-%20Audio%20Files/9%20-%20Grunting/Male/Sean%20Lenhart/grunting_1_sean.wav) | 0.44s | 0.0 dBFS | -12.8 dBFS | -3.4 dBFS | Source reaches digital full scale; monitor overlapping playback |
| `hurt` | [grunting_3_sean](Assets/_Project/Audio/Super%20Dialogue%20Audio%20Pack%20v1/Super%20Dialogue%20Audio%20Pack%20v1/Step%202%20-%20Audio%20Files/9%20-%20Grunting/Male/Sean%20Lenhart/grunting_3_sean.wav) | 0.92s | -0.1 dBFS | -14.0 dBFS | -3.5 dBFS | Level is technically reasonable |
| `hurt` | [grunting_6_sean](Assets/_Project/Audio/Super%20Dialogue%20Audio%20Pack%20v1/Super%20Dialogue%20Audio%20Pack%20v1/Step%202%20-%20Audio%20Files/9%20-%20Grunting/Male/Sean%20Lenhart/grunting_6_sean.wav) | 0.33s | 0.0 dBFS | -13.6 dBFS | -3.4 dBFS | Source reaches digital full scale; monitor overlapping playback |
| `player_death` | [grunting_2_ian](Assets/_Project/Audio/Super%20Dialogue%20Audio%20Pack%20v1/Super%20Dialogue%20Audio%20Pack%20v1/Step%202%20-%20Audio%20Files/9%20-%20Grunting/Male/Ian%20Lampert/grunting_2_ian.wav) | 0.57s | 0.0 dBFS | -13.4 dBFS | -3.4 dBFS | Source reaches digital full scale; monitor overlapping playback |
| `enemy_hit` | [77_flesh_02](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/77_flesh_02.wav) | 0.67s | -14.4 dBFS | -34.8 dBFS | -17.8 dBFS | Quiet SFX; listen for audibility in combat |
| `enemy_hit` | [15_Impact_flesh_02](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/15_Impact_flesh_02.wav) | 0.67s | -10.0 dBFS | -29.3 dBFS | -13.4 dBFS | Level is technically reasonable |
| `enemy_hurt` | [monster.1](Assets/_Project/Audio/External/OGA_MonsterGrowls_CC0/monster.1.ogg) | 0.53s | -2.4 dBFS | -22.1 dBFS | -5.8 dBFS | Level is technically reasonable |
| `enemy_hurt` | [monster.2](Assets/_Project/Audio/External/OGA_MonsterGrowls_CC0/monster.2.ogg) | 0.75s | -1.5 dBFS | -18.9 dBFS | -4.8 dBFS | Level is technically reasonable |
| `enemy_hurt` | [monster.4](Assets/_Project/Audio/External/OGA_MonsterGrowls_CC0/monster.4.ogg) | 0.79s | -1.4 dBFS | -18.9 dBFS | -4.7 dBFS | Level is technically reasonable |
| `enemy_hurt` | [monster.5](Assets/_Project/Audio/External/OGA_MonsterGrowls_CC0/monster.5.ogg) | 0.93s | -1.4 dBFS | -19.2 dBFS | -4.7 dBFS | Level is technically reasonable |
| `enemy_death` | [monster.8](Assets/_Project/Audio/External/OGA_MonsterGrowls_CC0/monster.8.ogg) | 1.66s | -4.1 dBFS | -21.5 dBFS | -7.4 dBFS | Level is technically reasonable |
| `enemy_death` | [monster.11](Assets/_Project/Audio/External/OGA_MonsterGrowls_CC0/monster.11.ogg) | 1.67s | -5.3 dBFS | -24.7 dBFS | -8.6 dBFS | Level is technically reasonable |
| `enemy_death` | [69_Enemy_death_01](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/69_Enemy_death_01.wav) | 2.67s | -14.3 dBFS | -36.7 dBFS | -17.6 dBFS | Quiet SFX; listen for audibility in combat |
| `enemy_melee_attack` | [03_Claw_03](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/03_Claw_03.wav) | 0.67s | -11.6 dBFS | -32.7 dBFS | -14.9 dBFS | Quiet SFX; listen for audibility in combat |
| `enemy_melee_attack` | [08_Bite_04](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/08_Bite_04.wav) | 0.67s | -13.2 dBFS | -31.9 dBFS | -16.6 dBFS | Quiet SFX; listen for audibility in combat |
| `enemy_ranged_attack` | [04_Fire_explosion_04_medium](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/04_Fire_explosion_04_medium.wav) | 2.00s | -12.9 dBFS | -29.3 dBFS | -16.3 dBFS | Level is technically reasonable |
| `enemy_ranged_attack` | [46_Poison_01](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/46_Poison_01.wav) | 2.00s | -10.1 dBFS | -26.7 dBFS | -13.5 dBFS | Level is technically reasonable |
| `boss_melee_attack` | [22_Slash_04](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/22_Slash_04.wav) | 1.33s | -8.0 dBFS | -33.2 dBFS | -11.4 dBFS | Quiet SFX; listen for audibility in combat |
| `boss_melee_attack` | [03_Claw_03](Assets/_Project/Audio/RPG_Essentials_Free/10_Battle_SFX/03_Claw_03.wav) | 0.67s | -11.6 dBFS | -32.7 dBFS | -14.9 dBFS | Quiet SFX; listen for audibility in combat |
| `boss_ranged_attack` | [30_Earth_02](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/30_Earth_02.wav) | 2.67s | -7.1 dBFS | -24.8 dBFS | -10.4 dBFS | Level is technically reasonable |
| `boss_ranged_attack` | [04_Fire_explosion_04_medium](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/04_Fire_explosion_04_medium.wav) | 2.00s | -12.9 dBFS | -29.3 dBFS | -16.3 dBFS | Level is technically reasonable |
| `boss_ranged_attack` | [21_Debuff_01](Assets/_Project/Audio/RPG_Essentials_Free/8_Buffs_Heals_SFX/21_Debuff_01.wav) | 3.33s | -6.5 dBFS | -25.3 dBFS | -9.8 dBFS | Level is technically reasonable |
| `bomber_explosion` | [04_Fire_explosion_04_medium](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/04_Fire_explosion_04_medium.wav) | 2.00s | -12.9 dBFS | -29.3 dBFS | -16.3 dBFS | Level is technically reasonable |
| `bomber_explosion` | [30_Earth_02](Assets/_Project/Audio/RPG_Essentials_Free/8_Atk_Magic_SFX/30_Earth_02.wav) | 2.67s | -7.1 dBFS | -24.8 dBFS | -10.4 dBFS | Level is technically reasonable |
| `coin_pickup` | [079_Buy_sell_01](Assets/_Project/Audio/RPG_Essentials_Free/10_UI_Menu_SFX/079_Buy_sell_01.wav) | 1.33s | -11.0 dBFS | -27.2 dBFS | -14.3 dBFS | Level is technically reasonable |
| `item_pickup` | [051_use_item_01](Assets/_Project/Audio/RPG_Essentials_Free/10_UI_Menu_SFX/051_use_item_01.wav) | 1.33s | -11.4 dBFS | -32.2 dBFS | -14.7 dBFS | Quiet SFX; listen for audibility in combat |
| `item_pickup` | [070_Equip_10](Assets/_Project/Audio/RPG_Essentials_Free/10_UI_Menu_SFX/070_Equip_10.wav) | 1.33s | -11.9 dBFS | -32.6 dBFS | -15.2 dBFS | Quiet SFX; listen for audibility in combat |
| `item_drop` | [071_Unequip_01](Assets/_Project/Audio/RPG_Essentials_Free/10_UI_Menu_SFX/071_Unequip_01.wav) | 1.33s | -10.1 dBFS | -32.9 dBFS | -13.4 dBFS | Quiet SFX; listen for audibility in combat |
| `click_button` | [013_Confirm_03](Assets/_Project/Audio/RPG_Essentials_Free/10_UI_Menu_SFX/013_Confirm_03.wav) | 1.33s | -16.3 dBFS | -32.3 dBFS | -19.6 dBFS | Quiet SFX; listen for audibility in combat |
| `pause` | [092_Pause_04](Assets/_Project/Audio/RPG_Essentials_Free/10_UI_Menu_SFX/092_Pause_04.wav) | 1.33s | -9.6 dBFS | -29.9 dBFS | -12.9 dBFS | Level is technically reasonable |
| `unpause` | [098_Unpause_04](Assets/_Project/Audio/RPG_Essentials_Free/10_UI_Menu_SFX/098_Unpause_04.wav) | 1.33s | -17.6 dBFS | -33.7 dBFS | -21.0 dBFS | Quiet SFX; listen for audibility in combat |

## BGM

| Event key | Audio file | Duration | Peak | RMS | Est. output peak | Technical note |
|---|---|---:|---:|---:|---:|---|
| `bgm_menu` | [02 - Title Theme](Assets/_Project/Audio/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/Loopable%20+%20one%20shots/ogg/02%20-%20Title%20Theme.ogg) | 132.82s | -0.8 dBFS | -13.1 dBFS | -6.8 dBFS | Level is technically reasonable |
| `bgm_dungeon` | [10 - Lost Shrine](Assets/_Project/Audio/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/Loopable%20+%20one%20shots/ogg/10%20-%20Lost%20Shrine.ogg) | 92.96s | -4.8 dBFS | -14.9 dBFS | -10.8 dBFS | Quietest BGM; leaves combat headroom |
| `bgm_level2` | [04 - Silent Forest](Assets/_Project/Audio/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/Loopable%20+%20one%20shots/ogg/04%20-%20Silent%20Forest.ogg) | 85.81s | -0.9 dBFS | -13.2 dBFS | -6.9 dBFS | Level is technically reasonable |
| `bgm_boss` | [17 - Decisive Battle 2 - The Calamity](Assets/_Project/Audio/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/Loopable%20+%20one%20shots/ogg/17%20-%20Decisive%20Battle%202%20-%20The%20Calamity.ogg) | 96.15s | -0.8 dBFS | -11.9 dBFS | -6.8 dBFS | Loudest average BGM; appropriate for boss |
| `bgm_shop` | [08 - Shop](Assets/_Project/Audio/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/xDeviruchi%20-%2016%20bit%20Fantasy%20&%20Adventure%20(2025)/Loopable%20+%20one%20shots/ogg/08%20-%20Shop.ogg) | 53.62s | -0.9 dBFS | -12.6 dBFS | -6.9 dBFS | Level is technically reasonable |

